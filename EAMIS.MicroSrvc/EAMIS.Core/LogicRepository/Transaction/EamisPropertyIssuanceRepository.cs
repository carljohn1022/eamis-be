using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Utility;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;



namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyIssuanceRepository : IEamisPropertyIssuanceRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEAMISIDProvider _EAMISIDProvider;
        private readonly IEamisPropertyTransactionDetailsRepository _eamisPropertyTransactionDetailsRepository;

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisPropertyIssuanceRepository(EAMISContext ctx,
                                               IEAMISIDProvider EAMISIDProvider,
                                               IEamisPropertyTransactionDetailsRepository eamisPropertyTransactionDetailsRepository)
        {
            _ctx = ctx;
            _EAMISIDProvider = EAMISIDProvider;
            _eamisPropertyTransactionDetailsRepository = eamisPropertyTransactionDetailsRepository;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<string> GetNextSequenceNumber(string tranType, string branchID)
        {
            if (tranType == TransactionTypeSettings.ICSIssuance)
            {
                var nextId = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.ICSIssuance, branchID);
                return nextId;
            }
            if (tranType == TransactionTypeSettings.PARIssuance)
            {
                var nextId = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.PARIssuance, branchID);
                return nextId;
            }
            return null;
        }
        public async Task<string> GetNextSequenceNumberForMaterialIssuance(string branchID)
        {
            var nextId = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.IssuanceMaterials, branchID);
            return nextId;
        }
        private string issuanceString()
        {
            return "anyTask";
        }

        public async Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item)
        {
            string strResult = "";
            //check item in DB
            var itemInDB = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.FirstOrDefault(i => i.ID == item.ItemId)).ConfigureAwait(false);
            if (itemInDB != null)
            {
                //update item qty
                itemInDB.QUANTITY = itemInDB.QUANTITY + item.QtyReceived;
                var result = await _ctx.SaveChangesAsync();
                if (result > 0)
                {
                    strResult = "successfully updated.";
                }
            }
            return strResult;
        }

        public async Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYITEMS> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyItemsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };

        }

        private IQueryable<EamisPropertyItemsDTO> QueryToDTO(IQueryable<EAMISPROPERTYITEMS> query)
        {
            //var category = _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking().ToList();

            return query.Select(x => new EamisPropertyItemsDTO
            {
                Id = x.ID,
                AppNo = x.APP_NO,
                PropertyNo = x.PROPERTY_NO,
                PropertyName = x.PROPERTY_NAME,
                CategoryId = x.CATEGORY_ID,
                SubCategoryId = x.SUBCATEGORY_ID,
                Brand = x.BRAND,
                UomId = x.UOM_ID,
                WarehouseId = x.WAREHOUSE_ID,
                PropertyType = x.PROPERTY_TYPE,
                Model = x.MODEL,
                Quantity = x.QUANTITY,
                SupplierId = x.SUPPLIER_ID,
                IsActive = x.IS_ACTIVE,
                ItemCategory = new EamisItemCategoryDTO
                {
                    Id = x.ITEM_CATEGORY.ID,
                    CategoryName = x.ITEM_CATEGORY.CATEGORY_NAME,
                    ShortDesc = x.ITEM_CATEGORY.SHORT_DESCRIPTION,
                    SubCategory = x.ITEM_CATEGORY.ITEM_SUB_CATEGORY.Select(y => new EamisItemSubCategoryDTO { Id = y.ID, CategoryId = y.CATEGORY_ID, SubCategoryName = y.SUB_CATEGORY_NAME }).ToList()
                },
                UnitOfMeasure = new EamisUnitofMeasureDTO
                {
                    Id = x.UOM_GROUP.ID,
                    Uom_Description = x.UOM_GROUP.UOM_DESCRIPTION
                },
                Warehouse = new EamisWarehouseDTO
                {
                    Id = x.WAREHOUSE_GROUP.ID,
                    Warehouse_Description = x.WAREHOUSE_GROUP.WAREHOUSE_DESCRIPTION
                },
                Supplier = new EamisSupplierDTO
                {
                    Id = x.SUPPLIER_GROUP.ID,
                    CompanyName = x.SUPPLIER_GROUP.COMPANY_NAME
                }


            });
        }

        private IQueryable<EAMISPROPERTYITEMS> PagedQuery(IQueryable<EAMISPROPERTYITEMS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISPROPERTYITEMS> FilteredEntites(EamisPropertyItemsDTO filter, IQueryable<EAMISPROPERTYITEMS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYITEMS>(true);
            predicate = predicate.And(x => x.IS_ACTIVE == true);
            predicate = predicate.And(q => q.QUANTITY > 0);
            var query = custom_query ?? _ctx.EAMIS_PROPERTYITEMS;
            return query.Where(predicate);
        }

        public async Task<EamisPropertyTransactionDTO> UpdateProperty(EamisPropertyTransactionDTO item)
        {
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
        public async Task<EamisPropertyTransactionDetailsDTO> UpdateDetails(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = data.ID == 0 ? EntityState.Added : EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().FirstOrDefaultAsync(x => x.ID == itemID)).ConfigureAwait(false);
            return new EamisPropertyTransactionDTO
            {
                Id = result.ID,
                TransactionNumber = result.TRANSACTION_NUMBER,
                TranType = result.TRAN_TYPE.Trim(),
                TransactionDate = result.TRANSACTION_DATE,
                FundSource = result.FUND_SOURCE,
                FiscalPeriod = result.FISCALPERIOD,
                TransactionType = result.TRANSACTION_TYPE,
                Memo = result.MEMO,
                ReceivedBy = result.RECEIVED_BY,
                ApprovedBy = result.APPROVED_BY,
                DeliveryDate = result.DELIVERY_DATE,
                TransactionStatus = result.TRANSACTION_STATUS,
                IsProperty = result.IS_PROPERTY,
                PropertyTransactionDetails = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Select(x => new EamisPropertyTransactionDetailsDTO
                {
                    Id = x.ID,
                    PropertyTransactionID = x.PROPERTY_TRANS_ID,
                    isDepreciation = x.IS_DEPRECIATION,
                    Dr = x.DR,
                    PropertyNumber = x.PROPERTY_NUMBER,
                    ItemCode = x.ITEM_CODE,
                    ItemDescription = x.ITEM_DESCRIPTION,
                    SerialNumber = x.SERIAL_NUMBER,
                    Po = x.PO,
                    Pr = x.PR,
                    AcquisitionDate = x.ACQUISITION_DATE,
                    AssigneeCustodian = x.ASSIGNEE_CUSTODIAN,
                    RequestedBy = x.REQUESTED_BY,
                    Office = x.OFFICE,
                    Department = x.DEPARTMENT,
                    ResponsibilityCode = x.RESPONSIBILITY_CODE,
                    UnitCost = x.UNIT_COST,
                    Qty = x.QTY,
                    SalvageValue = x.SALVAGE_VALUE,
                    BookValue = x.BOOK_VALUE,
                    EstLife = x.ESTIMATED_LIFE,
                    Area = x.AREA,
                    Semi = x.SEMI_EXPANDABLE_AMOUNT,
                    UserStamp = x.USER_STAMP,
                    //TimeStamp = x.TIME_STAMP,
                    WarrantyExpiry = x.WARRANTY_EXPIRY,
                    Invoice = x.INVOICE,
                    PropertyCondition = x.PROPERTY_CONDITION,
                    transactionDetailId = x.REFERENCE_ID,
                    Remarks = x.REMARKS
                }).Where(i => i.PropertyTransactionID == result.ID).ToList(),
                DeliveryImages = _ctx.EAMIS_ATTACHED_FILES.AsNoTracking().Select(v => new EamisAttachedFilesDTO
                {
                    Id = v.ID,
                    FileName = v.ATTACHED_FILENAME,
                    ModuleName = v.MODULE_NAME,
                    TransactionNumber = v.TRANID
                }).Where(i => i.TransactionNumber == result.TRANSACTION_NUMBER).ToList()
            };
        }


        public async Task<EamisPropertyTransactionDTO> InsertProperty(EamisPropertyTransactionDTO item)
        {
            

            var result = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(i => i.TRANSACTION_NUMBER == item.TransactionNumber).FirstOrDefault();
            if (result != null)
            {
                if (item.TranType == TransactionTypeSettings.ICSIssuance)
                {
                    var nextIdProvided = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.ICSIssuance, item.BranchID);
                    item.TransactionNumber = item.TranType+"-"+item.BranchID+"-"+ nextIdProvided;
                }
                if (item.TranType == TransactionTypeSettings.PARIssuance)
                {
                    var nextIdProvided = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.PARIssuance, item.BranchID);
                    item.TransactionNumber = item.TranType+"-"+item.BranchID+"-"+ nextIdProvided;
                }
            }
            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            //ensure that recently added record has the correct transaction id number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB
            

            //string _drType = item.TranType + nextId;

            //if (item.TransactionNumber != _drType)
            //{
            //    item.TransactionNumber = _drType;

            //    //reset context state to avoid error
            //    _ctx.Entry(data).State = EntityState.Detached;

            //    //call the update method, force to update the transaction number in DB
            //    await this.UpdateProperty(item);
            //}
            //item.Id = data.ID;
            return item;
        }
        public async Task<EamisPropertyTransactionDTO> InsertPropertyForMaterialIssuance(EamisPropertyTransactionDTO item)
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(i => i.TRANSACTION_NUMBER == item.TransactionNumber).FirstOrDefault();
            if (result != null)
            {    
                    var nextIdProvided = await _EAMISIDProvider.GetNextSequenceNumberPerBranch(TransactionTypeSettings.IssuanceMaterials, item.BranchID);
                    item.TransactionNumber = nextIdProvided;
            }

            EAMISPROPERTYTRANSACTION data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            //ensure that recently added record has the correct transaction id number
            item.Id = data.ID; //data.ID --> generated upon inserting a new record in DB

            //string _drType = PrefixSettings.IMPrefix + DateTime.Now.Year.ToString() + Convert.ToString(data.ID).PadLeft(6, '0');

            //if (item.TransactionNumber != _drType)
            //{
            //    item.TransactionNumber = _drType;

            //    //reset context state to avoid error
            //    _ctx.Entry(data).State = EntityState.Detached;

            //    //call the update method, force to update the transaction number in DB
            //    await this.UpdateProperty(item);
            //}
            item.Id = data.ID;
            return item;
        }
        private EAMISPROPERTYTRANSACTION MapToEntity(EamisPropertyTransactionDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTION();
            return new EAMISPROPERTYTRANSACTION
            {
                ID = item.Id,
                TRANSACTION_NUMBER = item.TransactionNumber,
                TRAN_TYPE = item.TranType.Trim(),
                TRANSACTION_DATE = item.TransactionDate,
                FUND_SOURCE = item.FundSource,
                FISCALPERIOD = item.FiscalPeriod,
                TRANSACTION_TYPE = item.TransactionType,
                MEMO = item.Memo,
                RECEIVED_BY = item.ReceivedBy,
                APPROVED_BY = item.ApprovedBy,
                DELIVERY_DATE = item.DeliveryDate,
                USER_STAMP = item.UserStamp,
                TRANSACTION_STATUS = item.TransactionStatus,
                IS_PROPERTY = item.IsProperty,
                BRANCH_ID = item.BranchID
            };

        }

        public async Task<EamisPropertyTransactionDetailsDTO> InsertPropertyTransaction(EamisPropertyTransactionDetailsDTO item)
        {
            EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }
        private EAMISPROPERTYTRANSACTIONDETAILS MapToEntity(EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null) return new EAMISPROPERTYTRANSACTIONDETAILS();
            return new EAMISPROPERTYTRANSACTIONDETAILS
            {
                ID = item.Id,
                PROPERTY_TRANS_ID = item.PropertyTransactionID,
                IS_DEPRECIATION = item.isDepreciation,
                DR = item.Dr,
                PROPERTY_NUMBER = item.PropertyNumber,
                ITEM_CODE = item.ItemCode,
                ITEM_DESCRIPTION = item.ItemDescription,
                SERIAL_NUMBER = item.SerialNumber,
                PO = item.Po,
                PR = item.Pr,
                ACQUISITION_DATE = item.AcquisitionDate,
                ASSIGNEE_CUSTODIAN = item.AssigneeCustodian,
                REQUESTED_BY = item.RequestedBy,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                RESPONSIBILITY_CODE = item.ResponsibilityCode,
                UNIT_COST = item.UnitCost,
                QTY = item.Qty,
                SALVAGE_VALUE = item.SalvageValue,
                BOOK_VALUE = item.BookValue,
                ESTIMATED_LIFE = item.EstLife,
                AREA = item.Area,
                SEMI_EXPANDABLE_AMOUNT = item.Semi,
                USER_STAMP = item.UserStamp,
                TIME_STAMP = item.TimeStamp,
                WARRANTY_EXPIRY = item.WarrantyExpiry,
                INVOICE = item.Invoice,
                PROPERTY_CONDITION = item.PropertyCondition,
                REFERENCE_ID = item.transactionDetailId,
                REMARKS = item.Remarks


            };
        }
        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> SearchReceiving(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = null;
            if (type == "Item Code")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving && x.ITEM_CODE.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Item Description")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving && x.ITEM_DESCRIPTION.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Transaction Number")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving && x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Serial Number")
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving && x.SERIAL_NUMBER.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().Where(x => x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving && x.PROPERTY_TRANSACTION_GROUP.TRANSACTION_NUMBER.Contains(searchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTOItemsForReceiving(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQueryForSearch(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query;
        }

        public async Task<string> GeneratePropertyNumber(DateTime acquisitionDate, string itemCode, string responsibilityCode, int counter)
        {
            //check item's category
           
                string yearPurchased = acquisitionDate.Year.ToString();
                //get property item's category
                var itemCategory = await Task.Run(() => _ctx.EAMIS_PROPERTYITEMS.AsNoTracking()
                                                            .Where(i => i.PROPERTY_NO == itemCode)
                                                            .Select(i => new { i.CATEGORY_ID })
                                                            .FirstOrDefault()).ConfigureAwait(false);
                if (string.IsNullOrEmpty(itemCategory.CATEGORY_ID.ToString()))
                {
                    bolerror = true;
                    _errorMessage = "Property item's is either empty or invalid.";
                }
                else
                {
                    var cat = await Task.Run(() => _ctx.EAMIS_ITEM_CATEGORY.AsNoTracking()
                                                       .Where(c => c.ID == itemCategory.CATEGORY_ID)
                                                       .Select(c => new { c.CHART_OF_ACCOUNT_ID, c.CATEGORY_NAME })
                                                       .FirstOrDefault()).ConfigureAwait(false);
                    if (string.IsNullOrEmpty(cat.CHART_OF_ACCOUNT_ID.ToString()))
                    {
                        bolerror = true;
                        _errorMessage = "Item's category could not be found.";
                    }
                    else
                    {
                        //get responsibility center office and location
                        var loc = await Task.Run(() => _ctx.EAMIS_RESPONSIBILITY_CENTER.AsNoTracking()
                                                           .Where(r => r.RESPONSIBILITY_CENTER == responsibilityCode)
                                                           .Select(s => new { s.OFFICE_CODE, s.LOCATION_CODE })
                                                           .FirstOrDefault()).ConfigureAwait(false);
                        
                        if (string.IsNullOrEmpty(loc.OFFICE_CODE) || string.IsNullOrEmpty(loc.LOCATION_CODE))
                        {
                            bolerror = true;
                            _errorMessage = "Responsibility center office/location code is either empty or invalid.";
                        }
                        else if (loc.LOCATION_CODE.Length < 3)
                        {
                            bolerror = true;
                            _errorMessage = "Responsibility center location code is either empty or invalid.";
                        }
                        else
                        {
                            //obtain group id and general ledger
                            var coa = await Task.Run(() => _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking()
                                                               .Join(_ctx.EAMIS_GROUP_CLASSIFICATION,
                                                               c => c.GROUP_ID,
                                                               g => g.ID,
                                                               (c, g) => new { c, g })
                                                               .Where(c => c.c.ID == cat.CHART_OF_ACCOUNT_ID)
                                                               .Select(c => new
                                                               {
                                                                   c.c.GROUP_ID,
                                                                   c.c.GENERAL_LEDGER_ACCOUNT,
                                                                   c.g.PPE_SUB_MAJOR_ACCT_GRP
                                                               })
                                                               .FirstOrDefault()).ConfigureAwait(false);
                            if (string.IsNullOrEmpty(coa.GROUP_ID.ToString()))
                            {
                                bolerror = true;
                                _errorMessage = "Item's group is either empty or invalid.";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(coa.GENERAL_LEDGER_ACCOUNT))
                                {
                                    bolerror = true;
                                    _errorMessage = "Item's general ledger account is either empty or invalid.";
                                }
                                else
                                {
                                    //When all validation is passed
                                    //Get total count issued for the underlying item category
                                    var totalIssuedCount = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                                               .Join(_ctx.EAMIS_PROPERTYITEMS,
                                                               d => d.ITEM_CODE,
                                                               p => p.PROPERTY_NO,
                                                               (d, p) => new { d, p })
                                                               .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                                               pi => pi.p.CATEGORY_ID,
                                                               c => c.ID,
                                                               (pi, c) => new { pi, c })
                                                               .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                                               di => di.pi.d.PROPERTY_TRANS_ID,
                                                               h => h.ID,
                                                               (di, h) => new { di, h })
                                                               .Where(i => i.di.c.ID == itemCategory.CATEGORY_ID &&
                                                                           !(i.di.pi.d.PROPERTY_NUMBER == null || i.di.pi.d.PROPERTY_NUMBER == string.Empty) &&
                                                                           i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving)
                                                               .GroupBy(g => new { g.di.c.ID })
                                                               .Select(s => new { Count = s.Count() })
                                                               .FirstOrDefault();
                                    int totalCount = 0;
                                    if (totalIssuedCount != null)
                                        if (totalIssuedCount.Count > 0)
                                            totalCount = totalIssuedCount.Count + counter;
                                    if (totalIssuedCount == null)
                                        totalCount = 0 + counter;

                                    //construct the property number
                                    //YEAR PURCHASED +
                                    //PPE SUB-MAJOR ACCOUNT GROUP +
                                    //GEN.LEDGER ACCOUNT +
                                    //"SERIAL NUMBER (first three digit - category type, next four digit - series per category type, last three digit - location)" +
                                    //OFFICE

                                    //- first 3 digits of category type - Get from Eamis_Item_Category, ID column. if length is less than 3, pad zero(0)
                                    //-next four digit-series per category type - count no if issuance/issued item then increment by 1
                                    //- last three digit-location

                                    string serialNumber = "";
                                    string propertyNumber = "";

                                    //int locStart = loc.LOCATION_CODE.Length - (loc.LOCATION_CODE.Length - 3
                                    int locStart = loc.LOCATION_CODE.Length - 3;
                                    if (itemCategory.CATEGORY_ID.ToString().Length < 3)
                                        serialNumber = itemCategory.CATEGORY_ID.ToString().PadLeft(3, '0');
                                    else
                                        serialNumber = itemCategory.CATEGORY_ID.ToString().Substring(0, 3);

                                    serialNumber +=  totalCount.ToString().PadLeft(4, '0') +
                                                       loc.LOCATION_CODE.Substring(locStart);
                                    string officeCode = loc.OFFICE_CODE.ToString();
                                    string officeCodeLimit = officeCode.Length > 5 ? officeCode.Substring(0, 5) : officeCode;

                                propertyNumber = yearPurchased + "-" +
                                                     coa.PPE_SUB_MAJOR_ACCT_GRP.ToString() + "-" +
                                                     coa.GENERAL_LEDGER_ACCOUNT.ToString() + "-" +
                                                     serialNumber + "-" +
                                                     officeCodeLimit;
                                    return propertyNumber;

                                }
                            }
                        }
                    }
                }
            
            return string.Empty;
        }


        private List<IssuedQtyDTO> GetIssuedQtyDTO()
        {
            var itemIssued = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                d => d.PROPERTY_TRANS_ID,
                                h => h.ID,
                                (d, h) => new { d, h })
                                .Where(t => t.h.TRANSACTION_TYPE == TransactionTypeSettings.Issuance)
                                .GroupBy(x => new { x.d.ITEM_CODE, x.d.PO, x.d.REFERENCE_ID })
                                .Select(i => new IssuedQtyDTO
                                {
                                    ID = i.Key.REFERENCE_ID,
                                    ItemCode = i.Key.ITEM_CODE,
                                    PO = i.Key.PO,
                                    IssuedQty = i.Sum(q => q.d.QTY)
                                })
                                .ToList();
            return itemIssued;
        }

        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> ListSupplyItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config, string tranType, int assigneeCustodian)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredSuppliesItemsForReceiving(filter, tranType, assigneeCustodian);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQueryItemsForReceiving(query, resolved_size, resolved_index);
            var result = new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTOItemsForReceiving(paged).ToListAsync()
            };

            return result;
        }
        public async Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config, string tranType, int assigneeCustodian, string branchID, string responsibilityCode)
        {
            IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query = FilteredItemsForReceiving(filter, tranType, assigneeCustodian, branchID, responsibilityCode);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQueryItemsForReceiving(query, resolved_size, resolved_index);
            var result = new DataList<EamisPropertyTransactionDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTOItemsForReceiving(paged).ToListAsync()
            };

            ////Display only property items with remaining qty 
            //List<EamisPropertyTransactionDetailsDTO> lstNew = new List<EamisPropertyTransactionDetailsDTO>();

            //List<IssuedQtyDTO> lstIssuedQty = GetIssuedQtyDTO();
            //for (int intItem = 0; intItem < result.Items.Count(); intItem++)
            //{
            //    bool bolFound = false;
            //    int remainingQty = 0;
            //    int issuedQty = 0;
            //    for (int intQty = 0; intQty < lstIssuedQty.Count(); intQty++)
            //    {
            //        if (result.Items[intItem].ItemCode == lstIssuedQty[intQty].ItemCode &&
            //           result.Items[intItem].Po == lstIssuedQty[intQty].PO &&
            //           result.Items[intItem].Id == lstIssuedQty[intQty].ID
            //           )
            //        {
            //            bolFound = true;
            //            remainingQty = result.Items[intItem].Qty - lstIssuedQty[intQty].IssuedQty;
            //            issuedQty = lstIssuedQty[intQty].IssuedQty;
            //            break;
            //        }
            //    }
            //    if (bolFound)
            //    {
            //        if (remainingQty > 0) //item have issuance
            //        {
            //            //add item to the list
            //            result.Items[intItem].IssuedQty = issuedQty;
            //            result.Items[intItem].RemainingQty = remainingQty;
            //            lstNew.Add(result.Items[intItem]);
            //        }
            //    }
            //    else
            //        lstNew.Add(result.Items[intItem]); //item has no issuane yet, display as it is
            //}

            //result.Items = lstNew;
            //result.Count = result.Items.Count();
            return result;
        }
        private IQueryable<EamisPropertyTransactionDetailsDTO> QueryToDTOItemsForReceiving(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query)
        {
            return query.Select(x => new EamisPropertyTransactionDetailsDTO
            {
                Id = x.ID,
                PropertyTransactionID = x.PROPERTY_TRANS_ID,
                isDepreciation = x.IS_DEPRECIATION,
                ItemCode = x.ITEM_CODE,
                Dr = x.DR,
                PropertyNumber = x.PROPERTY_NUMBER,
                ItemDescription = x.ITEM_DESCRIPTION,
                SerialNumber = x.SERIAL_NUMBER,
                Po = x.PO,
                Pr = x.PR,
                AcquisitionDate = x.ACQUISITION_DATE,
                AssigneeCustodian = x.ASSIGNEE_CUSTODIAN,
                RequestedBy = x.REQUESTED_BY,
                Office = x.OFFICE,
                Department = x.DEPARTMENT,
                ResponsibilityCode = x.RESPONSIBILITY_CODE,
                UnitCost = x.UNIT_COST,
                Qty = x.QTY,
                IssuedQty = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                .Where(r => r.REFERENCE_ID == x.ID)
                                .GroupBy(g => g.REFERENCE_ID)
                                .Select(i => i.Sum(v => v.QTY)).FirstOrDefault(),
                RemainingQty = x.QTY -
                                _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                .Where(r => r.REFERENCE_ID == x.ID)
                                .GroupBy(g => g.REFERENCE_ID)
                                .Select(i => i.Sum(v => v.QTY)).FirstOrDefault(),
                //IsAssigneeCustodianChange = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                //                .Where(r => r.REFERENCE_ID == x.ID )
                //                .GroupBy(g => g.REFERENCE_ID)
                //                .Select(i => i.Any(v => v.ASSIGNEE_CUSTODIAN != x.ASSIGNEE_CUSTODIAN))
                //                .FirstOrDefault() ? "yes" : "no",
            SalvageValue = x.SALVAGE_VALUE,
                BookValue = x.BOOK_VALUE,
                EstLife = x.ESTIMATED_LIFE,
                Area = x.AREA,
                Semi = x.SEMI_EXPANDABLE_AMOUNT,
                WarrantyExpiry = x.WARRANTY_EXPIRY,
                UserStamp = x.USER_STAMP,
                TimeStamp = x.TIME_STAMP,
                Invoice = x.INVOICE,
                PropertyCondition = x.PROPERTY_CONDITION,
                transactionDetailId = x.REFERENCE_ID,
                PropertyTransactionGroup = _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(x => new EamisPropertyTransactionDTO
                {
                    Id = x.ID,
                    TransactionStatus = x.TRANSACTION_STATUS,
                    TransactionType = x.TRANSACTION_TYPE,
                    TransactionNumber = x.TRANSACTION_NUMBER,
                    TranType = x.TRAN_TYPE,
                    TransactionDate = x.TRANSACTION_DATE,
                    FundSource = x.FUND_SOURCE,
                    FiscalPeriod = x.FISCALPERIOD,
                    ReceivedBy = x.RECEIVED_BY,
                    BranchID = x.BRANCH_ID
                }).Where(i => i.Id == x.PROPERTY_TRANS_ID).FirstOrDefault()

            }

            );
        }

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredSuppliesItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, string tranType, int assigneeCustodian, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            //Do not display items under service logs
            var arrservicelogs = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking()
                                .Where(pn => !(pn.PROPERTY_NUMBER == null || pn.PROPERTY_NUMBER.Trim() == string.Empty))
                                .Select(x => x.PROPERTY_NUMBER)
                                .ToList();
            if (tranType == "PAR")
            {
                var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                                   d => d.PROPERTY_TRANS_ID,
                                                   h => h.ID,
                                                   (d, h) => new { d, h })
                                            .Join(_ctx.EAMIS_PROPERTYITEMS,
                                            i => i.d.ITEM_CODE,
                                            p => p.PROPERTY_NO,
                                            (i, p) => new { i, p })
                                            .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                            c => c.p.CATEGORY_ID,
                                            ic => ic.ID,
                                            (c, ic) => new { ic, c })
                                            .Where(x => x.c.i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving &&
                                                        x.ic.IS_SUPPLIES == true)
                                            .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                            {
                                                ID = x.c.i.d.ID,
                                                PROPERTY_TRANS_ID = x.c.i.d.PROPERTY_TRANS_ID,
                                                IS_DEPRECIATION = x.c.i.d.IS_DEPRECIATION,
                                                DR = x.c.i.d.DR,
                                                PROPERTY_NUMBER = x.c.i.d.PROPERTY_NUMBER,
                                                ITEM_CODE = x.c.i.d.ITEM_CODE,
                                                ITEM_DESCRIPTION = x.c.i.d.ITEM_DESCRIPTION,
                                                SERIAL_NUMBER = x.c.i.d.SERIAL_NUMBER,
                                                PO = x.c.i.d.PO,
                                                PR = x.c.i.d.PR,
                                                ACQUISITION_DATE = x.c.i.d.ACQUISITION_DATE,
                                                ASSIGNEE_CUSTODIAN = x.c.i.d.ASSIGNEE_CUSTODIAN,
                                                REQUESTED_BY = x.c.i.d.REQUESTED_BY,
                                                OFFICE = x.c.i.d.OFFICE,
                                                DEPARTMENT = x.c.i.d.DEPARTMENT,
                                                RESPONSIBILITY_CODE = x.c.i.d.RESPONSIBILITY_CODE,
                                                UNIT_COST = x.c.i.d.UNIT_COST,
                                                QTY = x.c.i.d.QTY,
                                                SALVAGE_VALUE = x.c.i.d.SALVAGE_VALUE,
                                                BOOK_VALUE = x.c.i.d.BOOK_VALUE,
                                                ESTIMATED_LIFE = x.c.i.d.ESTIMATED_LIFE,
                                                AREA = x.c.i.d.AREA,
                                                SEMI_EXPANDABLE_AMOUNT = x.c.i.d.SEMI_EXPANDABLE_AMOUNT,
                                                USER_STAMP = x.c.i.d.USER_STAMP,
                                                TIME_STAMP = x.c.i.d.TIME_STAMP,
                                                WARRANTY_EXPIRY = x.c.i.d.WARRANTY_EXPIRY,
                                                INVOICE = x.c.i.d.INVOICE,
                                                PROPERTY_CONDITION = x.c.i.d.PROPERTY_CONDITION,
                                                REFERENCE_ID = x.c.i.d.REFERENCE_ID
                                            }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
                                                           (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                    .Where(r => r.REFERENCE_ID == s.ID)
                                                                    .GroupBy(g => g.REFERENCE_ID)
                                                                    .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                                    && s.UNIT_COST >= 50000 && s.ASSIGNEE_CUSTODIAN == assigneeCustodian                                                            
                                                    );
                return query.Where(predicate);
            }
            if (tranType == "ICS")
            {
                var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                                   d => d.PROPERTY_TRANS_ID,
                                                   h => h.ID,
                                                   (d, h) => new { d, h })
                                            .Join(_ctx.EAMIS_PROPERTYITEMS,
                                            i => i.d.ITEM_CODE,
                                            p => p.PROPERTY_NO,
                                            (i, p) => new { i, p })
                                            .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                            c => c.p.CATEGORY_ID,
                                            ic => ic.ID,
                                            (c, ic) => new { ic, c })
                                            .Where(x => x.c.i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving &&
                                                        x.ic.IS_SUPPLIES == true)
                                            .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                            {
                                                ID = x.c.i.d.ID,
                                                PROPERTY_TRANS_ID = x.c.i.d.PROPERTY_TRANS_ID,
                                                IS_DEPRECIATION = x.c.i.d.IS_DEPRECIATION,
                                                DR = x.c.i.d.DR,
                                                PROPERTY_NUMBER = x.c.i.d.PROPERTY_NUMBER,
                                                ITEM_CODE = x.c.i.d.ITEM_CODE,
                                                ITEM_DESCRIPTION = x.c.i.d.ITEM_DESCRIPTION,
                                                SERIAL_NUMBER = x.c.i.d.SERIAL_NUMBER,
                                                PO = x.c.i.d.PO,
                                                PR = x.c.i.d.PR,
                                                ACQUISITION_DATE = x.c.i.d.ACQUISITION_DATE,
                                                ASSIGNEE_CUSTODIAN = x.c.i.d.ASSIGNEE_CUSTODIAN,
                                                REQUESTED_BY = x.c.i.d.REQUESTED_BY,
                                                OFFICE = x.c.i.d.OFFICE,
                                                DEPARTMENT = x.c.i.d.DEPARTMENT,
                                                RESPONSIBILITY_CODE = x.c.i.d.RESPONSIBILITY_CODE,
                                                UNIT_COST = x.c.i.d.UNIT_COST,
                                                QTY = x.c.i.d.QTY,
                                                SALVAGE_VALUE = x.c.i.d.SALVAGE_VALUE,
                                                BOOK_VALUE = x.c.i.d.BOOK_VALUE,
                                                ESTIMATED_LIFE = x.c.i.d.ESTIMATED_LIFE,
                                                AREA = x.c.i.d.AREA,
                                                SEMI_EXPANDABLE_AMOUNT = x.c.i.d.SEMI_EXPANDABLE_AMOUNT,
                                                USER_STAMP = x.c.i.d.USER_STAMP,
                                                TIME_STAMP = x.c.i.d.TIME_STAMP,
                                                WARRANTY_EXPIRY = x.c.i.d.WARRANTY_EXPIRY,
                                                INVOICE = x.c.i.d.INVOICE,
                                                PROPERTY_CONDITION = x.c.i.d.PROPERTY_CONDITION,
                                                REFERENCE_ID = x.c.i.d.REFERENCE_ID
                                            }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
                                                           (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                    .Where(r => r.REFERENCE_ID == s.ID)
                                                                    .GroupBy(g => g.REFERENCE_ID)
                                                                    .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                                    && s.UNIT_COST < 50000 && s.ASSIGNEE_CUSTODIAN == assigneeCustodian
                                                    );
                return query.Where(predicate);
            }
            return null;
        }
        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, string tranType, int assigneeCustodian, string branchID, string responsibilityCode, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> additional_query = null)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
            //Do not display items under service logs
            var arrservicelogs = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking()
                                 .Join(_ctx.EAMIS_SERVICE_LOG,
                                            d => d.SERVICE_LOG_ID,
                                            h => h.ID,
                                            (d, h) => new { d, h })
                                .Where(pn => !(pn.d.PROPERTY_NUMBER == null || pn.d.PROPERTY_NUMBER.Trim() == string.Empty) && 
                                        pn.d.ASSET_CONDITION == "UNSERVICEABLE" &&
                                        pn.h.TRANSACTION_STATUS == PropertyItemStatus.Approved)
                                .Select(x => x.d.PROPERTY_NUMBER)
                                .ToList();

            
            if (tranType == "PAR") 
            { 
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                               d => d.PROPERTY_TRANS_ID,
                                               h => h.ID,
                                               (d, h) => new { d, h })
                                        .Join(_ctx.EAMIS_PROPERTYITEMS,
                                        i => i.d.ITEM_CODE,
                                        p => p.PROPERTY_NO,
                                        (i, p) => new { i, p })
                                        .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                        c => c.p.CATEGORY_ID,
                                        ic => ic.ID,
                                        (c, ic) => new { ic, c })
                                        .Where(x => x.c.i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving &&
                                                    x.ic.IS_ASSET == true && x.c.i.h.TRANSACTION_STATUS == PropertyItemStatus.Approved &&
                                                    x.c.i.h.BRANCH_ID == branchID) //added Property Transfer
                                        .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                        {
                                            ID = x.c.i.d.ID,
                                            PROPERTY_TRANS_ID = x.c.i.d.PROPERTY_TRANS_ID,
                                            IS_DEPRECIATION = x.c.i.d.IS_DEPRECIATION,
                                            DR = x.c.i.d.DR,
                                            PROPERTY_NUMBER = x.c.i.d.PROPERTY_NUMBER,
                                            ITEM_CODE = x.c.i.d.ITEM_CODE,
                                            ITEM_DESCRIPTION = x.c.i.d.ITEM_DESCRIPTION,
                                            SERIAL_NUMBER = x.c.i.d.SERIAL_NUMBER,
                                            PO = x.c.i.d.PO,
                                            PR = x.c.i.d.PR,
                                            ACQUISITION_DATE = x.c.i.d.ACQUISITION_DATE,
                                            ASSIGNEE_CUSTODIAN = x.c.i.d.ASSIGNEE_CUSTODIAN,
                                            FROM_END_USER = x.c.i.d.FROM_END_USER,
                                            REQUESTED_BY = x.c.i.d.REQUESTED_BY,
                                            OFFICE = x.c.i.d.OFFICE,
                                            DEPARTMENT = x.c.i.d.DEPARTMENT,
                                            RESPONSIBILITY_CODE = x.c.i.d.RESPONSIBILITY_CODE,
                                            UNIT_COST = x.c.i.d.UNIT_COST,
                                            QTY = x.c.i.d.QTY,
                                            SALVAGE_VALUE = x.c.i.d.SALVAGE_VALUE,
                                            BOOK_VALUE = x.c.i.d.BOOK_VALUE,
                                            ESTIMATED_LIFE = x.c.i.d.ESTIMATED_LIFE,
                                            AREA = x.c.i.d.AREA,
                                            SEMI_EXPANDABLE_AMOUNT = x.c.i.d.SEMI_EXPANDABLE_AMOUNT,
                                            USER_STAMP = x.c.i.d.USER_STAMP,
                                            //TIME_STAMP = x.c.i.d.TIME_STAMP,
                                            WARRANTY_EXPIRY = x.c.i.d.WARRANTY_EXPIRY,
                                            INVOICE = x.c.i.d.INVOICE,
                                            PROPERTY_CONDITION = x.c.i.d.PROPERTY_CONDITION,
                                            REFERENCE_ID = x.c.i.d.REFERENCE_ID
                                        }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
                                                       (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                .Where(r => r.REFERENCE_ID == s.ID)
                                                                .GroupBy(g => g.REFERENCE_ID)
                                                                .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                                && s.UNIT_COST >= 50000 && s.ASSIGNEE_CUSTODIAN == assigneeCustodian
                                                                && s.RESPONSIBILITY_CODE == responsibilityCode
                                                );
                var addition_query = additional_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                        .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                               d => d.PROPERTY_TRANS_ID,
                                               h => h.ID,
                                               (d, h) => new { d, h })
                                        .Join(_ctx.EAMIS_PROPERTYITEMS,
                                        i => i.d.ITEM_CODE,
                                        p => p.PROPERTY_NO,
                                        (i, p) => new { i, p })
                                        .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                        c => c.p.CATEGORY_ID,
                                        ic => ic.ID,
                                        (c, ic) => new { ic, c })
                                        .Where(x => x.c.i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer &&
                                                    x.ic.IS_ASSET == true && x.c.i.h.TRANSACTION_STATUS == PropertyItemStatus.Approved &&
                                                    x.c.i.h.BRANCH_ID == branchID) //added Property Transfer
                                        .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                        {
                                            ID = x.c.i.d.ID,
                                            PROPERTY_TRANS_ID = x.c.i.d.PROPERTY_TRANS_ID,
                                            IS_DEPRECIATION = x.c.i.d.IS_DEPRECIATION,
                                            DR = x.c.i.d.DR,
                                            PROPERTY_NUMBER = x.c.i.d.PROPERTY_NUMBER,
                                            ITEM_CODE = x.c.i.d.ITEM_CODE,
                                            ITEM_DESCRIPTION = x.c.i.d.ITEM_DESCRIPTION,
                                            SERIAL_NUMBER = x.c.i.d.SERIAL_NUMBER,
                                            PO = x.c.i.d.PO,
                                            PR = x.c.i.d.PR,
                                            ACQUISITION_DATE = x.c.i.d.ACQUISITION_DATE,
                                            ASSIGNEE_CUSTODIAN = x.c.i.d.ASSIGNEE_CUSTODIAN,
                                            FROM_END_USER = x.c.i.d.FROM_END_USER,
                                            REQUESTED_BY = x.c.i.d.REQUESTED_BY,
                                            OFFICE = x.c.i.d.OFFICE,
                                            DEPARTMENT = x.c.i.d.DEPARTMENT,
                                            RESPONSIBILITY_CODE = x.c.i.d.RESPONSIBILITY_CODE,
                                            UNIT_COST = x.c.i.d.UNIT_COST,
                                            QTY = x.c.i.d.QTY,
                                            SALVAGE_VALUE = x.c.i.d.SALVAGE_VALUE,
                                            BOOK_VALUE = x.c.i.d.BOOK_VALUE,
                                            ESTIMATED_LIFE = x.c.i.d.ESTIMATED_LIFE,
                                            AREA = x.c.i.d.AREA,
                                            SEMI_EXPANDABLE_AMOUNT = x.c.i.d.SEMI_EXPANDABLE_AMOUNT,
                                            USER_STAMP = x.c.i.d.USER_STAMP,
                                            //TIME_STAMP = x.c.i.d.TIME_STAMP,
                                            WARRANTY_EXPIRY = x.c.i.d.WARRANTY_EXPIRY,
                                            INVOICE = x.c.i.d.INVOICE,
                                            PROPERTY_CONDITION = x.c.i.d.PROPERTY_CONDITION,
                                            REFERENCE_ID = x.c.i.d.REFERENCE_ID
                                        }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
                                                       (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                .Where(r => r.REFERENCE_ID == s.ID)
                                                                .GroupBy(g => g.REFERENCE_ID)
                                                                .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                                && s.UNIT_COST >= 50000 && s.ASSIGNEE_CUSTODIAN == assigneeCustodian
                                                                && s.FROM_END_USER != s.ASSIGNEE_CUSTODIAN
                                                                && s.RESPONSIBILITY_CODE == responsibilityCode
                                                );
                if (addition_query != null)
                {
                    query = query.Concat(addition_query);
                }
                return query.Where(predicate);
            }
            if (tranType == "ICS")
            {
                var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                                   d => d.PROPERTY_TRANS_ID,
                                                   h => h.ID,
                                                   (d, h) => new { d, h })
                                            .Join(_ctx.EAMIS_PROPERTYITEMS,
                                            i => i.d.ITEM_CODE,
                                            p => p.PROPERTY_NO,
                                            (i, p) => new { i, p })
                                            .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                            c => c.p.CATEGORY_ID,
                                            ic => ic.ID,
                                            (c, ic) => new { ic, c })
                                            .Where(x => x.c.i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving &&
                                                        x.ic.IS_ASSET == true && x.c.i.h.TRANSACTION_STATUS == PropertyItemStatus.Approved &&
                                                        x.c.i.h.BRANCH_ID == branchID)
                                            .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                            {
                                                ID = x.c.i.d.ID,
                                                PROPERTY_TRANS_ID = x.c.i.d.PROPERTY_TRANS_ID,
                                                IS_DEPRECIATION = x.c.i.d.IS_DEPRECIATION,
                                                DR = x.c.i.d.DR,
                                                PROPERTY_NUMBER = x.c.i.d.PROPERTY_NUMBER,
                                                ITEM_CODE = x.c.i.d.ITEM_CODE,
                                                ITEM_DESCRIPTION = x.c.i.d.ITEM_DESCRIPTION,
                                                SERIAL_NUMBER = x.c.i.d.SERIAL_NUMBER,
                                                PO = x.c.i.d.PO,
                                                PR = x.c.i.d.PR,
                                                ACQUISITION_DATE = x.c.i.d.ACQUISITION_DATE,
                                                ASSIGNEE_CUSTODIAN = x.c.i.d.ASSIGNEE_CUSTODIAN,
                                                FROM_END_USER = x.c.i.d.FROM_END_USER,
                                                REQUESTED_BY = x.c.i.d.REQUESTED_BY,
                                                OFFICE = x.c.i.d.OFFICE,
                                                DEPARTMENT = x.c.i.d.DEPARTMENT,
                                                RESPONSIBILITY_CODE = x.c.i.d.RESPONSIBILITY_CODE,
                                                UNIT_COST = x.c.i.d.UNIT_COST,
                                                QTY = x.c.i.d.QTY,
                                                SALVAGE_VALUE = x.c.i.d.SALVAGE_VALUE,
                                                BOOK_VALUE = x.c.i.d.BOOK_VALUE,
                                                ESTIMATED_LIFE = x.c.i.d.ESTIMATED_LIFE,
                                                AREA = x.c.i.d.AREA,
                                                SEMI_EXPANDABLE_AMOUNT = x.c.i.d.SEMI_EXPANDABLE_AMOUNT,
                                                USER_STAMP = x.c.i.d.USER_STAMP,
                                                //TIME_STAMP = x.c.i.d.TIME_STAMP,
                                                WARRANTY_EXPIRY = x.c.i.d.WARRANTY_EXPIRY,
                                                INVOICE = x.c.i.d.INVOICE,
                                                PROPERTY_CONDITION = x.c.i.d.PROPERTY_CONDITION,
                                                REFERENCE_ID = x.c.i.d.REFERENCE_ID
                                            }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
                                                           (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                    .Where(r => r.REFERENCE_ID == s.ID)
                                                                    .GroupBy(g => g.REFERENCE_ID)
                                                                    .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                                    && s.UNIT_COST < 50000 && s.ASSIGNEE_CUSTODIAN == assigneeCustodian
                                                                    && s.RESPONSIBILITY_CODE == responsibilityCode
                                                    );
                var addition_query = additional_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                            .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
                                                   d => d.PROPERTY_TRANS_ID,
                                                   h => h.ID,
                                                   (d, h) => new { d, h })
                                            .Join(_ctx.EAMIS_PROPERTYITEMS,
                                            i => i.d.ITEM_CODE,
                                            p => p.PROPERTY_NO,
                                            (i, p) => new { i, p })
                                            .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                            c => c.p.CATEGORY_ID,
                                            ic => ic.ID,
                                            (c, ic) => new { ic, c })
                                            .Where(x => x.c.i.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyTransfer &&
                                                        x.ic.IS_ASSET == true && x.c.i.h.TRANSACTION_STATUS == PropertyItemStatus.Approved &&
                                                        x.c.i.h.BRANCH_ID == branchID)
                                            .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
                                            {
                                                ID = x.c.i.d.ID,
                                                PROPERTY_TRANS_ID = x.c.i.d.PROPERTY_TRANS_ID,
                                                IS_DEPRECIATION = x.c.i.d.IS_DEPRECIATION,
                                                DR = x.c.i.d.DR,
                                                PROPERTY_NUMBER = x.c.i.d.PROPERTY_NUMBER,
                                                ITEM_CODE = x.c.i.d.ITEM_CODE,
                                                ITEM_DESCRIPTION = x.c.i.d.ITEM_DESCRIPTION,
                                                SERIAL_NUMBER = x.c.i.d.SERIAL_NUMBER,
                                                PO = x.c.i.d.PO,
                                                PR = x.c.i.d.PR,
                                                ACQUISITION_DATE = x.c.i.d.ACQUISITION_DATE,
                                                ASSIGNEE_CUSTODIAN = x.c.i.d.ASSIGNEE_CUSTODIAN,
                                                FROM_END_USER = x.c.i.d.FROM_END_USER,
                                                REQUESTED_BY = x.c.i.d.REQUESTED_BY,
                                                OFFICE = x.c.i.d.OFFICE,
                                                DEPARTMENT = x.c.i.d.DEPARTMENT,
                                                RESPONSIBILITY_CODE = x.c.i.d.RESPONSIBILITY_CODE,
                                                UNIT_COST = x.c.i.d.UNIT_COST,
                                                QTY = x.c.i.d.QTY,
                                                SALVAGE_VALUE = x.c.i.d.SALVAGE_VALUE,
                                                BOOK_VALUE = x.c.i.d.BOOK_VALUE,
                                                ESTIMATED_LIFE = x.c.i.d.ESTIMATED_LIFE,
                                                AREA = x.c.i.d.AREA,
                                                SEMI_EXPANDABLE_AMOUNT = x.c.i.d.SEMI_EXPANDABLE_AMOUNT,
                                                USER_STAMP = x.c.i.d.USER_STAMP,
                                                //TIME_STAMP = x.c.i.d.TIME_STAMP,
                                                WARRANTY_EXPIRY = x.c.i.d.WARRANTY_EXPIRY,
                                                INVOICE = x.c.i.d.INVOICE,
                                                PROPERTY_CONDITION = x.c.i.d.PROPERTY_CONDITION,
                                                REFERENCE_ID = x.c.i.d.REFERENCE_ID
                                            }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
                                                           (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                    .Where(r => r.REFERENCE_ID == s.ID)
                                                                    .GroupBy(g => g.REFERENCE_ID)
                                                                    .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                                    && s.UNIT_COST < 50000 && s.ASSIGNEE_CUSTODIAN == assigneeCustodian
                                                                    && s.FROM_END_USER != s.ASSIGNEE_CUSTODIAN
                                                                    && s.RESPONSIBILITY_CODE == responsibilityCode
                                                    );
                if (addition_query != null)
                {
                    query = query.Concat(addition_query);
                }
                return query.Where(predicate);
            }
            return null;
        }
        //private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> FilteredItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> custom_query = null, bool strict = false)
        //{
        //    var predicate = PredicateBuilder.New<EAMISPROPERTYTRANSACTIONDETAILS>(true);
        //    //Do not display items under service logs
        //    var arrservicelogs = _ctx.EAMIS_SERVICE_LOG_DETAILS.AsNoTracking()
        //                        .Where(pn => !(pn.PROPERTY_NUMBER == null || pn.PROPERTY_NUMBER.Trim() == string.Empty))
        //                        .Select(x => x.PROPERTY_NUMBER)
        //                        .ToList();

        //    var query = custom_query ?? _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
        //                                .Join(_ctx.EAMIS_PROPERTY_TRANSACTION,
        //                                       d => d.PROPERTY_TRANS_ID,
        //                                       h => h.ID,
        //                                       (d, h) => new { d, h })
        //                                .Where(x => x.h.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving)
        //                                .Select(x => new EAMISPROPERTYTRANSACTIONDETAILS
        //                                {
        //                                    ID = x.d.ID,
        //                                    PROPERTY_TRANS_ID = x.d.PROPERTY_TRANS_ID,
        //                                    IS_DEPRECIATION = x.d.IS_DEPRECIATION,
        //                                    DR = x.d.DR,
        //                                    PROPERTY_NUMBER = x.d.PROPERTY_NUMBER,
        //                                    ITEM_CODE = x.d.ITEM_CODE,
        //                                    ITEM_DESCRIPTION = x.d.ITEM_DESCRIPTION,
        //                                    SERIAL_NUMBER = x.d.SERIAL_NUMBER,
        //                                    PO = x.d.PO,
        //                                    PR = x.d.PR,
        //                                    ACQUISITION_DATE = x.d.ACQUISITION_DATE,
        //                                    ASSIGNEE_CUSTODIAN = x.d.ASSIGNEE_CUSTODIAN,
        //                                    REQUESTED_BY = x.d.REQUESTED_BY,
        //                                    OFFICE = x.d.OFFICE,
        //                                    DEPARTMENT = x.d.DEPARTMENT,
        //                                    RESPONSIBILITY_CODE = x.d.RESPONSIBILITY_CODE,
        //                                    UNIT_COST = x.d.UNIT_COST,
        //                                    QTY = x.d.QTY,
        //                                    SALVAGE_VALUE = x.d.SALVAGE_VALUE,
        //                                    BOOK_VALUE = x.d.BOOK_VALUE,
        //                                    ESTIMATED_LIFE = x.d.ESTIMATED_LIFE,
        //                                    AREA = x.d.AREA,
        //                                    SEMI_EXPANDABLE_AMOUNT = x.d.SEMI_EXPANDABLE_AMOUNT,
        //                                    USER_STAMP = x.d.USER_STAMP,
        //                                    TIME_STAMP = x.d.TIME_STAMP,
        //                                    WARRANTY_EXPIRY = x.d.WARRANTY_EXPIRY,
        //                                    INVOICE = x.d.INVOICE,
        //                                    PROPERTY_CONDITION = x.d.PROPERTY_CONDITION,
        //                                    REFERENCE_ID = x.d.REFERENCE_ID
        //                                }).Where(s => !arrservicelogs.Contains(s.PROPERTY_NUMBER) &&
        //                                               (s.QTY - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
        //                                                        .Where(r => r.REFERENCE_ID == s.ID)
        //                                                        .GroupBy(g => g.REFERENCE_ID)
        //                                                        .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
        //                                        );
        //    return query.Where(predicate);
        //}

        private IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> PagedQueryItemsForReceiving(IQueryable<EAMISPROPERTYTRANSACTIONDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public async Task<string> GetResponsibilityCenterByID(string responsibilityCode)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_RESPONSIBILITY_CENTER.Where(s => s.RESPONSIBILITY_CENTER == responsibilityCode).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].OFFICE_DESC.ToString() + ":" +
                           result[0].UNIT_DESC.ToString();
            }
            return retValue;
        }
        public async Task<string> GetPropertyNumber(DateTime acquisitionDate, string responsibilityCode, string serialNumber)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_RESPONSIBILITY_CENTER.Where(s => s.RESPONSIBILITY_CENTER == responsibilityCode).AsNoTracking().ToList()).ConfigureAwait(false);
            retValue = acquisitionDate.Year.ToString() + "-" +
                serialNumber.ToString() + "-" +
                result[0].OFFICE_CODE.ToString();
            return retValue;
        }
        public async Task<string> GetDRNumFrSupplier(string dr)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_DELIVERY_RECEIPT.Where(s => s.TRANSACTION_TYPE == dr).AsNoTracking().ToList()).ConfigureAwait(false);
            retValue = result[0].DR_BY_SUPPLIER_NUMBER.ToString();
            return retValue;
        }
        public async Task<string> GetAPRNum(string dr)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_DELIVERY_RECEIPT.Where(s => s.TRANSACTION_TYPE == dr).AsNoTracking().ToList()).ConfigureAwait(false);
            retValue = result[0].APR_NUMBER.ToString();
            return retValue;
        }
        public async Task<string> GetBranchAreaOfUserForTranType(string branchID)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.branch.Where(s => s.BranchID == branchID).AsNoTracking().ToList()).ConfigureAwait(false);
            retValue = result[0].AreaID.ToString();
            return retValue;
        }

        //public async Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item)
        //{
        //    EAMISPROPERTYTRANSACTIONDETAILS data = MapToEntity(item);
        //    _ctx.Entry(data).State = EntityState.Deleted;
        //    await _ctx.SaveChangesAsync();
        //    return item;
        //}
        private IQueryable<EamisDeliveryReceiptDetailsDTO> QueryToDTODRForIssuance(IQueryable<EAMISDELIVERYRECEIPTDETAILS> query)
        {
            return query.Select(x => new EamisDeliveryReceiptDetailsDTO
            {
                Id = x.ID,
                ItemId = x.ITEM_ID,
                QtyReceived = x.QTY_RECEIVED,
                UnitCost = x.UNIT_COST,
                DeliveryReceiptId = x.DELIVERY_RECEIPT_ID,
                IssuedQty = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                            .Where(r => r.REFERENCE_ID == x.ID)
                            .GroupBy(g => g.REFERENCE_ID)
                            .Select(i => i.Sum(v => v.QTY)).FirstOrDefault(),
                RemainingQty = x.QTY_RECEIVED -
                            _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                            .Where(r => r.REFERENCE_ID == x.ID)
                            .GroupBy(g => g.REFERENCE_ID)
                            .Select(i => i.Sum(v => v.QTY)).FirstOrDefault(),
                DeliveryReceipt = _ctx.EAMIS_DELIVERY_RECEIPT.AsNoTracking().Select(x => new EamisDeliveryReceiptDTO
                {
                    Id = x.ID,
                    TransactionType = x.TRANSACTION_TYPE,
                    TransactionStatus = x.TRANSACTION_STATUS,
                    PurchaseOrderNumber = x.PURCHASE_ORDER_NUMBER,
                    PurchaseRequestNumber = x.PURCHASE_REQUEST_NUMBER,
                    SaleInvoiceNumber = x.SALE_INVOICE_NUMBER,
                    DRNumFrSupplier = x.DR_BY_SUPPLIER_NUMBER,
                    DateReceived = x.DATE_RECEIVED
                }).Where(i => i.Id == x.DELIVERY_RECEIPT_ID).FirstOrDefault(),
                PropertyItem = _ctx.EAMIS_PROPERTYITEMS.AsNoTracking().Select(p => new EamisPropertyItemsDTO
                {
                    Id = p.ID,
                    PropertyNo = p.PROPERTY_NO,
                    PropertyName = p.PROPERTY_NAME,
                }).Where(i => i.Id == x.ITEM_ID).FirstOrDefault()

            });
        }
        private IQueryable<EAMISDELIVERYRECEIPTDETAILS> FilteredDRForIssuance(EamisDeliveryReceiptDetailsDTO filter, string branchID, IQueryable<EAMISDELIVERYRECEIPTDETAILS> custom_query = null)
        {
            var predicate = PredicateBuilder.New<EAMISDELIVERYRECEIPTDETAILS>(true);

            var query = custom_query ?? _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS
                                    .Join(_ctx.EAMIS_DELIVERY_RECEIPT,
                                        d => d.DELIVERY_RECEIPT_ID,
                                        h => h.ID,
                                        (d, h) => new { d, h })
                                   .Join(_ctx.EAMIS_PROPERTYITEMS,
                                        i => i.d.ITEM_ID,
                                        p => p.ID,
                                        (i, p) => new { i, p })
                                    .Join(_ctx.EAMIS_ITEM_CATEGORY,
                                        c => c.p.CATEGORY_ID,
                                        ic => ic.ID,
                                        (c, ic) => new { ic, c })
                                    .Where(x => x.ic.IS_SUPPLIES == true && x.c.i.h.TRANSACTION_STATUS == DocStatus.Approved && x.c.i.h.BRANCH_ID == branchID)
                                    .Select(x => new EAMISDELIVERYRECEIPTDETAILS
                                    {
                                        ID = x.c.i.d.ID,
                                        DELIVERY_RECEIPT_ID = x.c.i.d.DELIVERY_RECEIPT_ID,
                                        ITEM_ID = x.c.i.d.ITEM_ID,
                                        UNIT_COST = x.c.i.d.UNIT_COST,
                                        QTY_RECEIVED = x.c.i.d.QTY_RECEIVED,
                                    }).Where(s => (s.QTY_RECEIVED - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
                                                                .Where(r => r.REFERENCE_ID == s.ID)
                                                                .GroupBy(g => g.REFERENCE_ID)
                                                                .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
                                                );


            return query.Where(predicate);

        }
        private IQueryable<EAMISDELIVERYRECEIPTDETAILS> PagedQueryDRForIssuance(IQueryable<EAMISDELIVERYRECEIPTDETAILS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        public async Task<DataList<EamisDeliveryReceiptDetailsDTO>> ListSuppliesDRForIssuance(EamisDeliveryReceiptDetailsDTO filter, PageConfig config, string branchID)
        {
            IQueryable<EAMISDELIVERYRECEIPTDETAILS> query = FilteredDRForIssuance(filter, branchID);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQueryDRForIssuance(query, resolved_size, resolved_index);
            var result = new DataList<EamisDeliveryReceiptDetailsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTODRForIssuance(paged).ToListAsync()
            };

            return result;
        }
        private IQueryable<EAMISDELIVERYRECEIPTDETAILS> PagedQueryForSearchDRForMaterialIssuance(IQueryable<EAMISDELIVERYRECEIPTDETAILS> query)
        {
            return query;
        }
        public async Task<DataList<EamisDeliveryReceiptDetailsDTO>> SearchDRForMaterialIssuance(string type, string searchValue)
        {
            IQueryable<EAMISDELIVERYRECEIPTDETAILS> query = null;
            //var query = custom_query ?? _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS
            //                                .Join(_ctx.EAMIS_DELIVERY_RECEIPT,
            //                                    d => d.DELIVERY_RECEIPT_ID,
            //                                    h => h.ID,
            //                                    (d, h) => new { d, h })
            //                               .Join(_ctx.EAMIS_PROPERTYITEMS,
            //                                    i => i.d.ITEM_ID,
            //                                    p => p.ID,
            //                                    (i, p) => new { i, p })
            //                                .Join(_ctx.EAMIS_ITEM_CATEGORY,
            //                                    c => c.p.CATEGORY_ID,
            //                                    ic => ic.ID,
            //                                    (c, ic) => new { ic, c })
            //                                .Where(x => x.ic.IS_SUPPLIES == true)
            //                                .Select(x => new EAMISDELIVERYRECEIPTDETAILS
            //                                {
            //                                    ID = x.c.i.d.ID,
            //                                    DELIVERY_RECEIPT_ID = x.c.i.d.DELIVERY_RECEIPT_ID,
            //                                    ITEM_ID = x.c.i.d.ITEM_ID,
            //                                    UNIT_COST = x.c.i.d.UNIT_COST,
            //                                    QTY_RECEIVED = x.c.i.d.QTY_RECEIVED,
            //                                }).Where(s => (s.QTY_RECEIVED - _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
            //                                                            .Where(r => r.REFERENCE_ID == s.ID)
            //                                                            .GroupBy(g => g.REFERENCE_ID)
            //                                                            .Select(i => i.Sum(v => v.QTY)).FirstOrDefault()) > 0
            //                                            );
            if (type == "Item Code")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS.AsNoTracking().Where(x => x.ITEMS_GROUP.PROPERTY_NO.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Item Description")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS.AsNoTracking().Where(x => x.ITEMS_GROUP.PROPERTY_NAME.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Transaction Number")
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS.AsNoTracking().Where(x => x.DELIVERY_RECEIPT_GROUP.TRANSACTION_TYPE.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS.AsNoTracking().Where(x => x.DELIVERY_RECEIPT_GROUP.TRANSACTION_TYPE.Contains(searchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearchDRForMaterialIssuance(query);
            return new DataList<EamisDeliveryReceiptDetailsDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTODRForIssuance(paged).ToListAsync()
            };
        }
        partial class IssuedQtyDTO
        {
            public int ID { get; set; }
            public string ItemCode { get; set; }
            public string PO { get; set; }
            public int IssuedQty { get; set; }
        }
    }
}