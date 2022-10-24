using ClosedXML.Excel;
using EAMIS.Common.DTO;
using EAMIS.Common.DTO.Classification;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Helper;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public class EamisFileHelper : IEamisFileHelper
    {
        #region dependency injection
        private readonly IEamisPropertyItemsRepository _eamisPropertyItemsRepository;
        private readonly IEamisWarehouseRepository _eamisWarehouseRepository;
        private readonly IEamisUnitofMeasureRepository _eamisUnitofMeasureRepository;
        private readonly IEamisItemCategoryRepository _eamisItemCategoryRepository;
        private readonly IEamisItemSubCategoryRepository _eamisItemSubCategoryRepository;
        private readonly IEamisSupplierRepository _eamisSupplierRepository;
        private readonly IEamisBarangayReporsitory _eamisBarangayRepository;
        private readonly IEamisRegionRepository _eamisRegionRepository;
        private readonly IEamisProvinceRepository _eamisProvinceRepository;
        private readonly IEamisMunicipalityRepository _eamisMunicipalityRepository;
        private readonly IEamisChartofAccountsRepository _eamisChartofAccountsRepository;

        private readonly IEamisClassificationRepository _eamisClassificationRepository;
        private readonly IEamisGroupClassificationRepository _eamisGroupClassificationRepository;
        private readonly IEamisSubClassificationRepository _eamisSubClassificationRepository;

        private readonly IEamisFundSourceRepository _eamisFundSourceRepository;
        private readonly IEamisAuthorizationRepository _eamisAuthorizationRepository;
        private readonly IEamisFinancingSourceRepository _eamisFinancingSourceRepository;
        private readonly IEamisGeneralFundSourceRepository _eamisGeneralFundSourceRepository;

        private readonly IEamisProcurementCategoryRepository _eamisProcurementCategoryRepository;
        private readonly IEamisResponsibilityCenterRepository _eamisResponsibilityCenterRepository;
        private readonly EAMISContext _ctx;
        #endregion dependency injection


        private string _errorMessage = "";
        private bool bolerror = false;
        private string reportFileName = "";
        private string reportStatus = "";
        private int isReportReady = 0;
        private byte[] fileImage;

        public int IsReportReady { get => isReportReady; set => value = isReportReady; }
        public string ReportFileName { get => reportFileName; set => value = reportFileName; }

        public string RptStatus { get => reportStatus; set => value = reportStatus; }

        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }
        public bool HasError { get => bolerror; set => value = bolerror; }

        public byte[] FileImage { get => fileImage; set => value = fileImage; }

        #region constructor
        public EamisFileHelper(IEamisPropertyItemsRepository eamisPropertyItemsRepository,
                               IEamisWarehouseRepository eamisWarehouseRepository,
                               IEamisUnitofMeasureRepository eamisUnitofMeasureRepository,
                               IEamisItemCategoryRepository eamisItemCategoryRepository,
                               IEamisItemSubCategoryRepository eamisItemSubCategoryRepository,
                               IEamisSupplierRepository eamisSupplierRepository,
                               IEamisBarangayReporsitory eamisBarangayRepository,
                               IEamisRegionRepository eamisRegionRepository,
                               IEamisProvinceRepository eamisProvinceRepository,
                               IEamisMunicipalityRepository eamisMunicipalityRepository,
                               IEamisChartofAccountsRepository eamisChartofAccountsRepository,
                               IEamisGroupClassificationRepository eamisGroupClassificationRepository,
                               IEamisClassificationRepository eamisClassificationRepository,
                               IEamisSubClassificationRepository eamisSubClassificationRepository,
                               IEamisFundSourceRepository eamisFundSourceRepository,
                               IEamisAuthorizationRepository eamisAuthorizationRepository,
                               IEamisFinancingSourceRepository eamisFinancingSourceRepository,
                               IEamisGeneralFundSourceRepository eamisGeneralFundSourceRepository,
                               IEamisProcurementCategoryRepository eamisProcurementCategoryRepository,
                               IEamisResponsibilityCenterRepository eamisResponsibilityCenterRepository,
                               EAMISContext ctx)
        {
            _eamisPropertyItemsRepository = eamisPropertyItemsRepository;
            _eamisWarehouseRepository = eamisWarehouseRepository;
            _eamisUnitofMeasureRepository = eamisUnitofMeasureRepository;
            _eamisItemCategoryRepository = eamisItemCategoryRepository;
            _eamisItemSubCategoryRepository = eamisItemSubCategoryRepository;
            _eamisSupplierRepository = eamisSupplierRepository;
            _eamisBarangayRepository = eamisBarangayRepository;
            _eamisRegionRepository = eamisRegionRepository;
            _eamisProvinceRepository = eamisProvinceRepository;
            _eamisMunicipalityRepository = eamisMunicipalityRepository;
            _eamisChartofAccountsRepository = eamisChartofAccountsRepository;
            _eamisGroupClassificationRepository = eamisGroupClassificationRepository;
            _eamisClassificationRepository = eamisClassificationRepository;
            _eamisSubClassificationRepository = eamisSubClassificationRepository;
            _eamisFundSourceRepository = eamisFundSourceRepository;
            _eamisAuthorizationRepository = eamisAuthorizationRepository;
            _eamisFinancingSourceRepository = eamisFinancingSourceRepository;
            _eamisGeneralFundSourceRepository = eamisGeneralFundSourceRepository;
            _eamisProcurementCategoryRepository = eamisProcurementCategoryRepository;
            _eamisResponsibilityCenterRepository = eamisResponsibilityCenterRepository;
            _ctx = ctx;
        }
        #endregion constructor

        public async Task<List<EAMISPROPERTYITEMS>> DownloadPropertyItems()
        {
            var result = await _eamisPropertyItemsRepository.GetAllPropertyItems();

            var warehouses = await _eamisWarehouseRepository.ListAllWarehouse();
            var uom = await _eamisUnitofMeasureRepository.ListAllIUnitOfMeasurement();
            var supplier = await _eamisSupplierRepository.ListAllSuppliers();
            var category = await _eamisItemCategoryRepository.ListAllItemCategories();
            var subcategory = await _eamisItemSubCategoryRepository.ListAllItemSubCategories();

            foreach (var propItem in result)
            {
                propItem.WAREHOUSE_GROUP = new EAMISWAREHOUSE();
                propItem.SUPPLIER_GROUP = new EAMISSUPPLIER();
                propItem.ITEM_CATEGORY = new EAMISITEMCATEGORY();
                propItem.UOM_GROUP = new EAMISUNITOFMEASURE();

                foreach (var item in warehouses)
                {
                    if (item.ID == propItem.WAREHOUSE_ID)
                    {
                        propItem.WAREHOUSE_GROUP.WAREHOUSE_DESCRIPTION = item.WAREHOUSE_DESCRIPTION;
                        break;
                    }
                }

                foreach (var item in uom)
                {
                    if (item.ID == propItem.UOM_ID)
                    {
                        propItem.UOM_GROUP.UOM_DESCRIPTION = item.UOM_DESCRIPTION;
                        break;
                    }
                }

                foreach (var item in supplier)
                {
                    if (item.ID == propItem.SUPPLIER_ID)
                    {
                        propItem.SUPPLIER_GROUP.COMPANY_DESCRIPTION = item.COMPANY_DESCRIPTION;
                        break;
                    }
                }

                foreach (var item in category)
                {
                    if (item.ID == propItem.CATEGORY_ID)
                    {
                        propItem.ITEM_CATEGORY.CATEGORY_NAME = item.CATEGORY_NAME;
                        break;
                    }
                }

                foreach (var item in subcategory)
                {
                    if (item.ID == propItem.SUBCATEGORY_ID)
                    {
                        propItem.ITEM_CATEGORY.SHORT_DESCRIPTION = item.SUB_CATEGORY_NAME;
                        break;
                    }
                }
            }
            return result;
        }

        public async Task<List<EAMISSUPPLIER>> DownloadSuppliers()
        {
            var result = await _eamisSupplierRepository.GetAllSuppliers();
            foreach (var item in result)
            {
                item.BARANGAY_GROUP = new EAMISBARANGAY();
                item.BARANGAY_GROUP.PROVINCE = new EAMISPROVINCE();
                item.BARANGAY_GROUP.MUNICIPALITY = new EAMISMUNICIPALITY();
                item.BARANGAY_GROUP.REGION = new EAMISREGION();

                var region = _eamisRegionRepository.ListRegionById(item.REGION_CODE);
                item.BARANGAY_GROUP.REGION.REGION_DESCRIPTION = region.Result == null ? "" : region.Result[0].REGION_DESCRIPTION;

                var province = _eamisProvinceRepository.ListProvinceById(item.PROVINCE_CODE);
                item.BARANGAY_GROUP.PROVINCE.PROVINCE_DESCRITION = province.Result == null ? "" : province.Result[0].PROVINCE_DESCRITION;

                var city = _eamisMunicipalityRepository.ListMunicipalityById(item.CITY_MUNICIPALITY_CODE);
                item.BARANGAY_GROUP.MUNICIPALITY.CITY_MUNICIPALITY_DESCRIPTION = city.Result == null ? "" : city.Result[0].CITY_MUNICIPALITY_DESCRIPTION;

                var brgy = _eamisBarangayRepository.ListBarangayById(item.BRGY_CODE);
                item.BARANGAY_GROUP.BRGY_DESCRIPTION = brgy.Result == null ? "" : brgy.Result[0].BRGY_DESCRIPTION;
            }
            return result;
        }

        public async Task<List<EAMISITEMCATEGORY>> DownloadCategories()
        {
            var result = await _eamisItemCategoryRepository.GetAllItemCategories();

            foreach (var item in result)
            {
                var coa = _eamisChartofAccountsRepository.ListCOAById(item.CHART_OF_ACCOUNT_ID);
                item.CHART_OF_ACCOUNTS = new EAMISCHARTOFACCOUNTS();
                item.CHART_OF_ACCOUNTS.ACCOUNT_CODE = coa.Result == null ? "" : coa.Result[0].ACCOUNT_CODE;
            }
            return result;
        }

        public async Task<List<EAMISCHARTOFACCOUNTS>> DownloadChartOfAccounts()
        {
            var result = await _eamisChartofAccountsRepository.ListAllCOA();

            foreach (var item in result)
            {
                item.GROUPCLASSIFICATION = new EAMISGROUPCLASSIFICATION();
                var groupId = _eamisGroupClassificationRepository.ListGroupById(item.GROUP_ID);
                item.GROUPCLASSIFICATION.NAME_GROUPCLASSIFICATION = groupId.Result == null ? "" : groupId.Result[0].NAME_GROUPCLASSIFICATION;
            }
            return result;
        }

        public async Task<List<EAMISFUNDSOURCE>> DownloadFundSources()
        {
            var result = await _eamisFundSourceRepository.ListAllFundSources();
            foreach (var item in result)
            {
                item.AUTHORIZATION = new EAMISAUTHORIZATION();
                item.FINANCING_SOURCE = new EAMISFINANCINGSOURCE();
                item.GENERALFUNDSOURCE = new EAMISGENERALFUNDSOURCE();

                var fundId = _eamisGeneralFundSourceRepository.ListGeneralFundsById(item.GENERAL_FUND_SOURCE_ID);
                item.GENERALFUNDSOURCE.NAME = fundId.Result == null ? "" : fundId.Result[0].NAME;

                var financeSourceId = _eamisFinancingSourceRepository.ListFinancingSourceById(item.FINANCING_SOURCE_ID);
                item.FINANCING_SOURCE.FINANCING_SOURCE_NAME = financeSourceId.Result == null ? "" : financeSourceId.Result[0].FINANCING_SOURCE_NAME;

                var authorizationId = _eamisAuthorizationRepository.ListAuthorizationById(item.AUTHORIZATION_ID);
                item.AUTHORIZATION.AUTHORIZATION_NAME = authorizationId.Result == null ? "" : authorizationId.Result[0].AUTHORIZATION_NAME;
            }
            return result;
        }

        public async Task<List<EAMISITEMSUBCATEGORY>> DownloadSubCategories()
        {
            var result = await _eamisItemSubCategoryRepository.ListAllItemSubCategories();
            foreach (var item in result)
            {
                item.ITEM_CATEGORY = new EAMISITEMCATEGORY();
                var catmain = _eamisItemCategoryRepository.ListCategoriesById(item.CATEGORY_ID);
                item.ITEM_CATEGORY.CATEGORY_NAME = catmain.Result == null ? "" : catmain.Result[0].CATEGORY_NAME;
            }
            return result;
        }

        public async Task<List<EAMISWAREHOUSE>> DownloadWarehouse()
        {
            var result = await _eamisWarehouseRepository.ListAllWarehouse();
            return result;
        }

        public async Task<List<EAMISPROCUREMENTCATEGORY>> DownloadProcurements()
        {
            var result = await _eamisProcurementCategoryRepository.ListAllProcurements();
            return result;
        }

        public async Task<List<EAMISUNITOFMEASURE>> DownloadUOM()
        {
            var result = await _eamisUnitofMeasureRepository.ListAllIUnitOfMeasurement();
            return result;
        }

        public async Task<List<EAMISRESPONSIBILITYCENTER>> DownloadResponsibilityCenters()
        {
            var result = await _eamisResponsibilityCenterRepository.GetAllResponsibilityCenters();
            return result;
        }

        public Task<string> DownloadExcelTemplate(string WorkSheetTemplateName)
        {
            //var file = await Task.Run(() => ExcelHelper.GenerateExcel(WorkSheetTemplateName)).ConfigureAwait(false);
            //return file;
            return null;
        }
        public async Task<bool> IsReportCompleted(int Id)
        {
            try
            {
                var result = await Task.Run(() => _ctx.EAMIS_REPORT_REQUEST_LISTENER
                          .Where(r => r.ID == Id)
                          .Select(v => new { v.RptIsReady, v.GenRptFilNam, v.RptStatus, v.FileImage }).FirstOrDefault()).ConfigureAwait(false);
                if (result != null)
                {
                    if (result.GenRptFilNam == ReportStatus.ErrorFound)
                    {
                        isReportReady = ReportStatus.ReportReady;
                        reportFileName = result.GenRptFilNam;
                        bolerror = true;
                    }
                    else
                    {
                        if (result.RptIsReady == ReportStatus.ReportNotReady)
                        {
                            isReportReady = ReportStatus.ReportNotReady;
                            bolerror = true;
                        }
                        else
                        {
                            isReportReady = result.RptIsReady;
                            reportFileName = result.GenRptFilNam;
                            fileImage = result.FileImage;
                        }
                    }
                    reportStatus = result.RptStatus;
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                bolerror = true;
            }
            return HasError;
        }

        public async Task<bool> Delete(int reportId)
        {
            EAMISREPORTREQUESTLISTENER data = new EAMISREPORTREQUESTLISTENER
            {
                ID = reportId
            };
            _ctx.Entry(data).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return true;
        }

        public async Task<EamisReportRequestListener> GenerateReport(string RptReqCode, string RptCode, string ParFldVal, int GenTyp)
        {
            EamisReportRequestListener eamisReportRequestListener = new EamisReportRequestListener();
            bolerror = false;
            try
            {
                EAMISREPORTREQUESTLISTENER report = new EAMISREPORTREQUESTLISTENER
                {
                    ID = 0,
                    RptReqCode = RptReqCode,
                    RptCode = RptCode,
                    ParFldVal = ParFldVal,
                    GenTyp = GenTyp,
                    EntCre = DateTime.Now
                };
                _ctx.Entry(report).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                await _ctx.SaveChangesAsync();
                eamisReportRequestListener.ID = report.ID;
                eamisReportRequestListener.GenTyp = report.GenTyp;
                eamisReportRequestListener.GenRptFilNam = report.GenRptFilNam;
                eamisReportRequestListener.RptIsReady = report.RptIsReady;
                eamisReportRequestListener.RptReqCode = report.RptReqCode;
                eamisReportRequestListener.RptStatus = report.RptStatus;
                eamisReportRequestListener.EntCre = report.EntCre;
                eamisReportRequestListener.FileImage = report.FileImage;
                return eamisReportRequestListener;
            }
            catch (Exception ex)
            {
                _errorMessage = ex.InnerException.Message;
                bolerror = true;
            }
            return eamisReportRequestListener;
        }
        public async Task<bool> UploadFileToDB(string fileFormat, string FilePath, string TemplateName)
        {
            try
            {
                if (fileFormat == FileFormat.ExcelFile)
                    return await UploadExcelToDB(FilePath, TemplateName);
                else
                    return await UploadCSVFileToDB(FilePath, TemplateName);
            }
            catch (Exception ex)
            {
                bolerror = true;
                throw;
            }
        }

        private static EamisPropertyItemsDTO GetCSVPropertyItemRowValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            return new EamisPropertyItemsDTO
            {
                Id = 0,
                PropertyName = rowValues[1].ToString(),
                Brand = rowValues[2].ToString(),
                Model = rowValues[3].ToString(),
                PropertyType = rowValues[4].ToString(),
                Quantity = Convert.ToInt32(rowValues[7]),
                AppNo = rowValues[8].ToString(),
                IsActive = Convert.ToBoolean(rowValues[10]),
                PropertyNo = rowValues[11].ToString(),
                Warehouse = new EamisWarehouseDTO { Warehouse_Description = rowValues[0].ToString() },
                UnitOfMeasure = new EamisUnitofMeasureDTO { Uom_Description = rowValues[5].ToString() },
                Supplier = new EamisSupplierDTO { CompanyName = rowValues[6].ToString() },
                ItemCategory = new EamisItemCategoryDTO { CategoryName = rowValues[9].ToString(), ShortDesc = rowValues[12].ToString() }
            };
        }

        private static EamisSupplierDTO GetCSVSupplierRowValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            EamisSupplierDTO eamisSupplierDTO = new EamisSupplierDTO();
            eamisSupplierDTO.Id = 0;
            eamisSupplierDTO.CompanyName = rowValues[0].ToString();
            eamisSupplierDTO.CompanyDescription = rowValues[1].ToString();
            eamisSupplierDTO.ContactPersonName = rowValues[2].ToString();
            eamisSupplierDTO.ContactPersonNumber = rowValues[3].ToString();
            eamisSupplierDTO.Barangay = new EamisBarangayDTO { BrgyDescription = rowValues[7].ToString() };
            eamisSupplierDTO.Barangay.Region = new EamisRegionDTO { RegionDescription = rowValues[4].ToString() };
            eamisSupplierDTO.Barangay.Province = new EamisProvinceDTO { ProvinceDescription = rowValues[5].ToString() };
            eamisSupplierDTO.Barangay.Municipality = new EamisMunicipalityDTO { CityMunicipalityDescription = rowValues[6].ToString() };

            eamisSupplierDTO.Street = rowValues[8].ToString();
            eamisSupplierDTO.Bank = rowValues[9].ToString();
            eamisSupplierDTO.AccountName = rowValues[10].ToString();
            eamisSupplierDTO.AccountNumber = rowValues[11].ToString(); //Convert.ToInt32(row.Cell(12).Value);
            eamisSupplierDTO.Branch = rowValues[12].ToString();
            eamisSupplierDTO.IsActive = Convert.ToBoolean(rowValues[13]);

            return eamisSupplierDTO;
        }

        private static EamisItemCategoryDTO GetCSVCategoryValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            return new EamisItemCategoryDTO
            {
                ChartOfAccounts = new EamisChartofAccountsDTO { AccountCode = rowValues[0].ToString() },
                CategoryName = rowValues[1].ToString(),
                ShortDesc = rowValues[2].ToString(),
                IsSupplies = Convert.ToBoolean(rowValues[3]),
                IsAsset = Convert.ToBoolean(rowValues[4]),
                IsSerialized = Convert.ToBoolean(rowValues[5]),
                IsStockable = Convert.ToBoolean(rowValues[6]),
                CostMethod = rowValues[7].ToString(),
                EstimatedLife = Convert.ToInt32(rowValues[8]),
                DepreciationMethod = rowValues[9].ToString().ToLower() == "" ? "Straight Line(Default)" : rowValues[9].ToString(),
                IsActive = Convert.ToBoolean(rowValues[10])
            };
        }

        private static EamisItemSubCategoryDTO GetCSVSubCategoryValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            EamisItemSubCategoryDTO eamisSubCategoryDTO = new EamisItemSubCategoryDTO
            {
                ItemCategory = new EamisItemCategoryDTO { CategoryName = rowValues[0].ToString() },
                SubCategoryName = rowValues[1].ToString(),
                IsActive = Convert.ToBoolean(rowValues[2])
            };
            return eamisSubCategoryDTO;
        }

        private static EamisWarehouseDTO GetCSVWarehouseValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            EamisWarehouseDTO eamisWarehouseDTO = new EamisWarehouseDTO
            {
                Id = 0,
                Warehouse_Description = rowValues[0].ToString(),
                Street_Name = rowValues[1].ToString(),
                Region_Code = Convert.ToInt32(rowValues[2]),
                Municipality_Code = Convert.ToInt32(rowValues[3]),
                Province_Code = Convert.ToInt32(rowValues[4]),
                Barangay_Code = Convert.ToInt32(rowValues[5]),
                IsActive = Convert.ToBoolean(rowValues[6])
            };
            return eamisWarehouseDTO;
        }

        private static EamisChartofAccountsDTO GetCSVCOAValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            EamisChartofAccountsDTO eamisChartofAccountsDTO = new EamisChartofAccountsDTO
            {
                Id = 0,
                ObjectCode = rowValues[3].ToString(),
                AccountCode = rowValues[4].ToString(),
                IsActive = Convert.ToBoolean(rowValues[6]),
                IsPartofInventroy = Convert.ToBoolean(rowValues[5]),
                ClassificationDTO = new EamisClassificationDTO { NameClassification = rowValues[0].ToString() },
                SubClassificationDTO = new EamisSubClassificationDTO { NameSubClassification = rowValues[1].ToString() },
                GroupClassificationDTO = new EamisGroupClassificationDTO { NameGroupClassification = rowValues[2].ToString() }
            };
            return eamisChartofAccountsDTO;
        }

        private static EamisFundSourceDTO GetCSVFundValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            return new EamisFundSourceDTO
            {
                Id = 0,
                GeneralFundSource = new EamisGeneralFundSourceDTO { Name = rowValues[0].ToString() },
                Code = rowValues[1].ToString(),
                FinancingSource = new EamisFinancingSourceDTO { FinancingSourceName = rowValues[2].ToString() },
                Authorization = new EamisAuthorizationDTO { AuthorizationName = rowValues[3].ToString() },
                FundCategory = rowValues[4].ToString(),
                IsActive = Convert.ToBoolean(rowValues[5])
            };
        }

        private static EamisProcurementCategoryDTO GetCSVProcurementValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            return new EamisProcurementCategoryDTO
            {
                Id = 0,
                ProcurementDescription = rowValues[0].ToString(),
                IsActive = Convert.ToBoolean(rowValues[1])
            };
        }

        private static EamisUnitofMeasureDTO GetCSVUOMValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator);
            return new EamisUnitofMeasureDTO
            {
                Id = 0,
                Short_Description = rowValues[0].ToString(),
                Uom_Description = rowValues[1].ToString(),
                isActive = Convert.ToBoolean(rowValues[2])
            };
        }
        private static EamisResponsibilityCenterDTO GetCSVResponsibilityCenterValue(string csvLine)
        {
            string[] rowValues = csvLine.Split(FileFormat.Separator, StringSplitOptions.RemoveEmptyEntries);
            return new EamisResponsibilityCenterDTO
            {
                Id = 0,
                mainGroupCode = rowValues[0].ToString(),
                mainGroupDesc = rowValues[1].ToString(),
                subGroupCode = rowValues[2].ToString(),
                subGroupDesc = rowValues[3].ToString(),
                officeCode = rowValues[4].ToString().PadLeft(10, '0'),
                officeDesc = rowValues[5].ToString(),
                unitCode = rowValues[6].ToString(),
                unitDesc = rowValues[7].ToString(),
                isActive = Convert.ToBoolean(rowValues[8])
            };
        }

        private async Task<bool> UploadCSVFileToDB(string CSVFilePath, string TemplateName)
        {
            try
            {
                var warehouses = await _eamisWarehouseRepository.ListAllWarehouse();
                var uom = await _eamisUnitofMeasureRepository.ListAllIUnitOfMeasurement();
                var supplier = await _eamisSupplierRepository.ListAllSuppliers();
                var category = await _eamisItemCategoryRepository.ListAllItemCategories();
                var subcategory = await _eamisItemSubCategoryRepository.ListAllItemSubCategories();

                switch (TemplateName)
                {
                    #region Item
                    case WorkSheetTemplateNames.Items:
                        List<EamisPropertyItemsDTO> lsteamisPropertyItemsDTO = File.ReadAllLines(CSVFilePath)
                                           .Skip(1) //skip header
                                           .Select(csvRow => GetCSVPropertyItemRowValue(csvRow))
                                           .ToList();

                        for (int intItem = 0; intItem < lsteamisPropertyItemsDTO.Count(); intItem++)
                        {
                            var warehouseId = warehouses.Find(x => x.WAREHOUSE_DESCRIPTION.ToLower().Trim() == lsteamisPropertyItemsDTO[intItem].Warehouse.Warehouse_Description.ToLower().Trim());
                            lsteamisPropertyItemsDTO[intItem].WarehouseId = warehouseId == null ? 0 : warehouseId.ID; //obtain from warehouse table

                            var uomID = uom.Find(x => x.UOM_DESCRIPTION.ToLower().Trim() == lsteamisPropertyItemsDTO[intItem].UnitOfMeasure.Uom_Description.ToLower().Trim());
                            lsteamisPropertyItemsDTO[intItem].UomId = uomID == null ? 0 : uomID.ID;

                            var supplierId = supplier.Find(x => x.COMPANY_NAME.ToLower().Trim() == lsteamisPropertyItemsDTO[intItem].Supplier.CompanyName.ToLower().Trim());
                            lsteamisPropertyItemsDTO[intItem].SupplierId = supplierId == null ? 0 : supplierId.ID;

                            var categoryId = category.Find(x => x.CATEGORY_NAME.ToLower().Trim() == lsteamisPropertyItemsDTO[intItem].ItemCategory.CategoryName.ToLower().Trim());
                            lsteamisPropertyItemsDTO[intItem].CategoryId = categoryId == null ? 0 : categoryId.ID;

                            var subCategoryId = subcategory.Find(x => x.SUB_CATEGORY_NAME.ToLower().Trim() == lsteamisPropertyItemsDTO[intItem].ItemCategory.ShortDesc.ToLower().Trim());
                            lsteamisPropertyItemsDTO[intItem].SubCategoryId = subCategoryId == null ? 0 : subCategoryId.ID;

                            //var result = await _eamisPropertyItemsRepository.InsertFromExcel(lsteamisPropertyItemsDTO[intItem]);
                            //if (_eamisPropertyItemsRepository.HasError)
                            //{
                            //    _errorMessage = _eamisPropertyItemsRepository.ErrorMessage;
                            //    bolerror = true;
                            //    return HasError;
                            //}
                        }

                        var itemResult = await _eamisPropertyItemsRepository.InsertFromExcel(lsteamisPropertyItemsDTO);
                        if (_eamisPropertyItemsRepository.HasError)
                        {
                            _errorMessage = _eamisPropertyItemsRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion Item

                    #region supplier
                    case WorkSheetTemplateNames.Suppliers:
                        List<EamisSupplierDTO> lsteamisSupplierDTO = File.ReadAllLines(CSVFilePath)
                                           .Skip(1) //skip header
                                           .Select(csvRow => GetCSVSupplierRowValue(csvRow))
                                           .ToList();
                        for (int intSupplier = 0; intSupplier < lsteamisSupplierDTO.Count(); intSupplier++)
                        {
                            var region = _eamisRegionRepository.ListRegion(lsteamisSupplierDTO[intSupplier].Barangay.Region.RegionDescription);
                            lsteamisSupplierDTO[intSupplier].RegionCode = region == null ? 0 : region.Result[0].REGION_CODE;

                            var province = _eamisProvinceRepository.ListProvince(lsteamisSupplierDTO[intSupplier].Barangay.Province.ProvinceDescription);
                            lsteamisSupplierDTO[intSupplier].ProvinceCode = province == null ? 0 : province.Result[0].PROVINCE_CODE;

                            var city = _eamisMunicipalityRepository.ListMunicipality(lsteamisSupplierDTO[intSupplier].Barangay.Municipality.CityMunicipalityDescription);
                            lsteamisSupplierDTO[intSupplier].CityMunicipalityCode = city == null ? 0 : city.Result[0].MUNICIPALITY_CODE;

                            var brgy = _eamisBarangayRepository.ListBarangay(lsteamisSupplierDTO[intSupplier].Barangay.BrgyDescription);
                            lsteamisSupplierDTO[intSupplier].BrgyCode = brgy == null ? 0 : brgy.Result[0].BRGY_CODE;
                        }
                        var supplierResult = await _eamisSupplierRepository.InsertFromExcel(lsteamisSupplierDTO);
                        if (_eamisSupplierRepository.HasError)
                        {
                            _errorMessage = _eamisSupplierRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion supplier

                    #region category
                    case WorkSheetTemplateNames.Category:
                        List<EamisItemCategoryDTO> lsteamisCategoryDTO = File.ReadAllLines(CSVFilePath)
                                           .Skip(1) //skip header
                                           .Select(csvRow => GetCSVCategoryValue(csvRow))
                                           .ToList();
                        for (int intItem = 0; intItem < lsteamisCategoryDTO.Count(); intItem++)
                        {
                            var coa = _eamisChartofAccountsRepository.ListCOA(lsteamisCategoryDTO[intItem].ChartOfAccounts.AccountCode.ToString());
                            lsteamisCategoryDTO[intItem].ChartOfAccountId = coa.Result == null ? 0 : coa.Result[0].ID;
                        }
                        var categoryResult = await _eamisItemCategoryRepository.InsertFromExcel(lsteamisCategoryDTO);
                        if (_eamisItemCategoryRepository.HasError)
                        {
                            _errorMessage = _eamisItemCategoryRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion category

                    #region subcategory
                    case WorkSheetTemplateNames.SubCategory:
                        List<EamisItemSubCategoryDTO> lsteamisSubCategoryDTO = File.ReadAllLines(CSVFilePath)
                                           .Skip(1) //skip header
                                           .Select(csvRow => GetCSVSubCategoryValue(csvRow))
                                           .ToList();
                        for (int intItem = 0; intItem < lsteamisSubCategoryDTO.Count(); intItem++)
                        {
                            var catmain = _eamisItemCategoryRepository.ListCategories(lsteamisSubCategoryDTO[intItem].ItemCategory.CategoryName);
                            lsteamisSubCategoryDTO[intItem].CategoryId = catmain.Result == null ? 0 : catmain.Result[0].ID;
                        }
                        var subcategoryResult = await _eamisItemSubCategoryRepository.InsertFromExcel(lsteamisSubCategoryDTO);
                        if (_eamisItemSubCategoryRepository.HasError)
                        {
                            _errorMessage = _eamisItemSubCategoryRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion subcategory

                    #region chartofaccount
                    case WorkSheetTemplateNames.ChartOfAccount:
                        List<EamisChartofAccountsDTO> lsteamisCOA = File.ReadAllLines(CSVFilePath)
                                          .Skip(1) //skip header
                                          .Select(csvRow => GetCSVCOAValue(csvRow))
                                          .ToList();
                        for (int intItem = 0; intItem < lsteamisCOA.Count(); intItem++)
                        {

                            var classificationId = _eamisClassificationRepository.ListClassifications(lsteamisCOA[intItem].ClassificationDTO.NameClassification);
                            //uncomment next line when classification id is added on the chart of account model/dto
                            //lsteamisCOA[intItem].ClassificationId = classificationId.Result == null ? 0 : classificationId.Result[0].ID;

                            var subClassificationId = _eamisSubClassificationRepository.ListSubClassifications(lsteamisCOA[intItem].SubClassificationDTO.NameSubClassification);
                            //uncomment next line when classification id is added on the chart of account model/dto
                            //lsteamisCOA[intItem].SubClassificationId = subClassificationId.Result == null ? 0 : subClassificationId.Result[0].ID;

                            var groupId = _eamisGroupClassificationRepository.ListGroups(lsteamisCOA[intItem].GroupClassificationDTO.NameGroupClassification);
                            lsteamisCOA[intItem].GroupId = groupId.Result == null ? 0 : groupId.Result[0].ID;
                        }
                        var coaResult = await _eamisChartofAccountsRepository.InsertFromExcel(lsteamisCOA);
                        if (_eamisChartofAccountsRepository.HasError)
                        {
                            _errorMessage = _eamisChartofAccountsRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion chartofaccount

                    #region fund
                    case WorkSheetTemplateNames.Funds:
                        List<EamisFundSourceDTO> lsteamisFund = File.ReadAllLines(CSVFilePath)
                                          .Skip(1) //skip header
                                          .Select(csvRow => GetCSVFundValue(csvRow))
                                          .ToList();
                        for (int intItem = 0; intItem < lsteamisFund.Count(); intItem++)
                        {
                            var fundId = _eamisGeneralFundSourceRepository.ListGeneralFunds(lsteamisFund[intItem].GeneralFundSource.Name);
                            lsteamisFund[intItem].GenFundId = fundId.Result == null ? 0 : fundId.Result[0].ID;

                            var financeSourceId = _eamisFinancingSourceRepository.ListFinancingSource(lsteamisFund[intItem].FinancingSource.FinancingSourceName);
                            lsteamisFund[intItem].FinancingSourceId = financeSourceId.Result == null ? 0 : financeSourceId.Result[0].ID;

                            var authorizationId = _eamisAuthorizationRepository.ListAuthorization(lsteamisFund[intItem].Authorization.AuthorizationName);
                            lsteamisFund[intItem].AuthorizationId = authorizationId.Result == null ? 0 : authorizationId.Result[0].ID;
                        }
                        var fundResult = await _eamisFundSourceRepository.InsertFromExcel(lsteamisFund);
                        if (_eamisFundSourceRepository.HasError)
                        {
                            _errorMessage = _eamisFundSourceRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion fund

                    #region Procurement
                    case WorkSheetTemplateNames.ProcurementCategory:
                        List<EamisProcurementCategoryDTO> lsteamisProcurement = File.ReadAllLines(CSVFilePath)
                                          .Skip(1) //skip header
                                          .Select(csvRow => GetCSVProcurementValue(csvRow))
                                          .ToList();

                        var procurementResult = await _eamisProcurementCategoryRepository.InsertFromExcel(lsteamisProcurement);
                        if (_eamisProcurementCategoryRepository.HasError)
                        {
                            _errorMessage = _eamisProcurementCategoryRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion Procurement

                    #region UOM
                    case WorkSheetTemplateNames.UnitofMeasurement:
                        List<EamisUnitofMeasureDTO> lsteamisUOM = File.ReadAllLines(CSVFilePath)
                                          .Skip(1) //skip header
                                          .Select(csvRow => GetCSVUOMValue(csvRow))
                                          .ToList();

                        var uomResult = await _eamisUnitofMeasureRepository.InsertFromExcel(lsteamisUOM);
                        if (_eamisUnitofMeasureRepository.HasError)
                        {
                            _errorMessage = _eamisUnitofMeasureRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion UOM
                    #region Responsibility Center
                    case WorkSheetTemplateNames.ResponsibilityCenter:
                        List<EamisResponsibilityCenterDTO> lsteamisResponsibilityCenter = File.ReadAllLines(CSVFilePath)
                                          .Skip(1) //skip header
                                          .Select(csvRow => GetCSVResponsibilityCenterValue(csvRow))
                                          .ToList();

                        var responsibilitycenterResult = await _eamisResponsibilityCenterRepository.InsertFromExcel(lsteamisResponsibilityCenter);
                        if (_eamisResponsibilityCenterRepository.HasError)
                        {
                            _errorMessage = _eamisResponsibilityCenterRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                    #endregion Responsibility Center

                    #region Warehouse
                    case WorkSheetTemplateNames.Warehouse:
                        List<EamisWarehouseDTO> lsteamisWarehouse = File.ReadAllLines(CSVFilePath)
                                          .Skip(1) //skip header
                                          .Select(csvRow => GetCSVWarehouseValue(csvRow))
                                          .ToList();

                        var warehouseResult = await _eamisWarehouseRepository.InsertFromExcel(lsteamisWarehouse);
                        if (_eamisResponsibilityCenterRepository.HasError)
                        {
                            _errorMessage = _eamisWarehouseRepository.ErrorMessage;
                            bolerror = true;
                            return HasError;
                        }
                        break;
                        #endregion Warehouse
                }
                bolerror = false;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return HasError;
        }
        private async Task<bool> UploadExcelToDB(string ExcelFilePath, string TemplateName)
        {
            try
            {
                using (var excelWorkbook = new XLWorkbook(ExcelFilePath))
                {
                    //check first row if it's not empty
                    var worksheet = excelWorkbook.Worksheet(1);
                    int rowCtr = 1;
                    var row = worksheet.Row(rowCtr);

                    if (row.IsEmpty())
                    {
                        return false;
                    }
                    int itemId = 0;

                    var warehouses = await _eamisWarehouseRepository.ListAllWarehouse();
                    var uom = await _eamisUnitofMeasureRepository.ListAllIUnitOfMeasurement();
                    var supplier = await _eamisSupplierRepository.ListAllSuppliers();
                    var category = await _eamisItemCategoryRepository.ListAllItemCategories();
                    var subcategory = await _eamisItemSubCategoryRepository.ListAllItemSubCategories();

                    switch (TemplateName)
                    {
                        #region Items
                        case WorkSheetTemplateNames.Items:

                            EamisWarehouseDTO eamisWarehouseDTO = new EamisWarehouseDTO();
                            //eamisWarehouseDTO.Warehouse_Description = "DENR CENTRAL";
                            //eamisWarehouseDTO.Street_Name = "VISAYAS AVE.";
                            PageConfig config = new PageConfig();



                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisPropertyItemsDTO eamisPropertyItemsDTO = new EamisPropertyItemsDTO();
                                eamisPropertyItemsDTO.Id = 0;

                                foreach (var item in warehouses)
                                {
                                    if (item.WAREHOUSE_DESCRIPTION.ToLower().Trim() == row.Cell(1).Value.ToString().ToLower().Trim())
                                    {
                                        itemId = item.ID;
                                        break;
                                    }
                                }

                                eamisPropertyItemsDTO.WarehouseId = itemId; //obtain from warehouse table
                                eamisPropertyItemsDTO.PropertyName = row.Cell(2).Value.ToString();
                                eamisPropertyItemsDTO.Brand = row.Cell(3).Value.ToString();
                                eamisPropertyItemsDTO.Model = row.Cell(4).Value.ToString();
                                eamisPropertyItemsDTO.PropertyType = row.Cell(5).Value.ToString();

                                itemId = 0;
                                foreach (var item in uom)
                                {
                                    if (item.UOM_DESCRIPTION.ToLower().Trim() == row.Cell(6).Value.ToString().ToLower().Trim())
                                    {
                                        itemId = item.ID;
                                        break;
                                    }
                                }
                                eamisPropertyItemsDTO.UomId = itemId; //obtain from unitofmeasurement table

                                itemId = 0;
                                foreach (var item in supplier)
                                {
                                    if (item.COMPANY_NAME.ToLower().Trim() == row.Cell(7).Value.ToString().ToLower().Trim())
                                    {
                                        itemId = item.ID;
                                        break;
                                    }
                                }
                                eamisPropertyItemsDTO.SupplierId = itemId; //obtain from supplier table

                                eamisPropertyItemsDTO.Quantity = Convert.ToInt32(row.Cell(8).Value);
                                eamisPropertyItemsDTO.AppNo = row.Cell(9).Value.ToString();

                                itemId = 0;
                                foreach (var item in category)
                                {
                                    if (item.CATEGORY_NAME.ToLower().Trim() == row.Cell(10).Value.ToString().ToLower().Trim())
                                    {
                                        itemId = item.ID;
                                        break;
                                    }
                                }
                                eamisPropertyItemsDTO.CategoryId = itemId;

                                eamisPropertyItemsDTO.IsActive = Convert.ToBoolean(row.Cell(11).Value);
                                eamisPropertyItemsDTO.PropertyNo = row.Cell(12).Value.ToString();

                                itemId = 0;
                                foreach (var item in subcategory)
                                {
                                    if (item.SUB_CATEGORY_NAME.ToLower().Trim() == row.Cell(13).Value.ToString().ToLower().Trim())
                                    {
                                        itemId = item.ID;
                                        break;
                                    }
                                }
                                eamisPropertyItemsDTO.SubCategoryId = itemId; // obtain id from subcategory table
                                var result = await _eamisPropertyItemsRepository.InsertFromExcel(eamisPropertyItemsDTO);
                                rowCtr += 1;
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion items

                        #region suppliers
                        case WorkSheetTemplateNames.Suppliers:

                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisSupplierDTO eamisSupplierDTO = new EamisSupplierDTO();

                                eamisSupplierDTO.CompanyName = row.Cell(1).Value.ToString();
                                eamisSupplierDTO.CompanyDescription = row.Cell(2).Value.ToString();
                                eamisSupplierDTO.ContactPersonName = row.Cell(3).Value.ToString();
                                eamisSupplierDTO.ContactPersonNumber = row.Cell(4).Value.ToString();

                                var region = _eamisRegionRepository.ListRegion(row.Cell(5).Value.ToString());
                                eamisSupplierDTO.RegionCode = region.Result == null ? 0 : region.Result[0].REGION_CODE;

                                var province = _eamisProvinceRepository.ListProvince(row.Cell(6).Value.ToString());
                                eamisSupplierDTO.ProvinceCode = province.Result == null ? 0 : province.Result[0].PROVINCE_CODE;

                                var city = _eamisMunicipalityRepository.ListMunicipality(row.Cell(7).Value.ToString());
                                eamisSupplierDTO.CityMunicipalityCode = city.Result == null ? 0 : city.Result[0].MUNICIPALITY_CODE;

                                var brgy = _eamisBarangayRepository.ListBarangay(row.Cell(8).Value.ToString());
                                eamisSupplierDTO.BrgyCode = brgy.Result == null ? 0 : brgy.Result[0].BRGY_CODE;

                                eamisSupplierDTO.Street = row.Cell(9).Value.ToString();
                                eamisSupplierDTO.Bank = row.Cell(10).Value.ToString();
                                eamisSupplierDTO.AccountName = row.Cell(11).Value.ToString();
                                eamisSupplierDTO.AccountNumber = row.Cell(12).Value.ToString(); //Convert.ToInt32(row.Cell(12).Value);
                                eamisSupplierDTO.Branch = row.Cell(13).Value.ToString();
                                eamisSupplierDTO.IsActive = Convert.ToBoolean(row.Cell(14).Value);
                                rowCtr += 1;
                                var result = await _eamisSupplierRepository.InsertFromExcel(eamisSupplierDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion suppliers

                        #region category
                        case WorkSheetTemplateNames.Category:
                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisItemCategoryDTO eamisItemCategoryDTO = new EamisItemCategoryDTO();
                                var coa = _eamisChartofAccountsRepository.ListCOA(row.Cell(1).Value.ToString());
                                eamisItemCategoryDTO.ChartOfAccountId = coa.Result == null ? 0 : coa.Result[0].ID;
                                eamisItemCategoryDTO.CategoryName = row.Cell(2).Value.ToString();
                                eamisItemCategoryDTO.ShortDesc = row.Cell(3).Value.ToString();
                                eamisItemCategoryDTO.IsSupplies = Convert.ToBoolean(row.Cell(4).Value); //row.Cell(4).Value.ToString().ToLower() == "supplies" ? true : false;
                                eamisItemCategoryDTO.IsAsset = Convert.ToBoolean(row.Cell(5).Value); //row.Cell(5).Value.ToString().ToLower() == "asset" ? true : false;
                                eamisItemCategoryDTO.IsSerialized = Convert.ToBoolean(row.Cell(6).Value);  //row.Cell(6).Value.ToString().ToLower() == "serialized" ? true : false;
                                eamisItemCategoryDTO.IsStockable = Convert.ToBoolean(row.Cell(7).Value); //row.Cell(7).Value.ToString().ToLower() == "stockable" ? true : false;
                                eamisItemCategoryDTO.CostMethod = row.Cell(8).Value.ToString();
                                eamisItemCategoryDTO.EstimatedLife = Convert.ToInt32(row.Cell(9).Value);
                                eamisItemCategoryDTO.DepreciationMethod = row.Cell(10).Value.ToString().ToLower() == "" ? "Straight Line(Default)" : row.Cell(10).Value.ToString();
                                eamisItemCategoryDTO.IsActive = Convert.ToBoolean(row.Cell(11).Value);
                                var result = _eamisItemCategoryRepository.InsertFromExcel(eamisItemCategoryDTO);
                                rowCtr += 1;
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion category

                        #region subcategory
                        case WorkSheetTemplateNames.SubCategory:
                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);
                            while (!row.IsEmpty())
                            {
                                EamisItemSubCategoryDTO eamisItemSubCategoryDTO = new EamisItemSubCategoryDTO();

                                var catmain = _eamisItemCategoryRepository.ListCategories(row.Cell(1).Value.ToString());
                                eamisItemSubCategoryDTO.CategoryId = catmain.Result == null ? 0 : catmain.Result[0].ID;
                                eamisItemSubCategoryDTO.SubCategoryName = row.Cell(2).Value.ToString();
                                eamisItemSubCategoryDTO.IsActive = Convert.ToBoolean(row.Cell(3).Value);
                                rowCtr += 1;
                                var result = await _eamisItemSubCategoryRepository.InsertFromExcel(eamisItemSubCategoryDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion subcategory

                        #region chartofaccount
                        case WorkSheetTemplateNames.ChartOfAccount:

                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisChartofAccountsDTO eamisChartofAccountsDTO = new EamisChartofAccountsDTO();
                                var classificationId = _eamisClassificationRepository.ListClassifications(row.Cell(1).Value.ToString());
                                //uncomment next line when classification id is added on the chart of account model/dto
                                //eamisChartofAccountsDTO.ClassificationId = classificationId.Result == null ? 0 : classificationId.Result[0].ID;

                                var subClassificationId = _eamisSubClassificationRepository.ListSubClassifications(row.Cell(2).Value.ToString());
                                //uncomment next line when classification id is added on the chart of account model/dto
                                //eamisChartofAccountsDTO.SubClassificationId = subClassificationId.Result == null ? 0 : subClassificationId.Result[0].ID;

                                eamisChartofAccountsDTO.ObjectCode = row.Cell(4).Value.ToString();
                                eamisChartofAccountsDTO.AccountCode = row.Cell(5).Value.ToString();
                                eamisChartofAccountsDTO.IsActive = Convert.ToBoolean(row.Cell(6).Value);
                                eamisChartofAccountsDTO.IsPartofInventroy = Convert.ToBoolean(row.Cell(7).Value);

                                var groupId = _eamisGroupClassificationRepository.ListGroups(row.Cell(3).Value.ToString());
                                eamisChartofAccountsDTO.GroupId = groupId.Result == null ? 0 : groupId.Result[0].ID;

                                rowCtr += 1;
                                var result = await _eamisChartofAccountsRepository.InsertFromExcel(eamisChartofAccountsDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion chartofaccount

                        #region fund
                        case WorkSheetTemplateNames.Funds:

                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisFundSourceDTO eamisFundSourceDTO = new EamisFundSourceDTO();
                                var fundId = _eamisGeneralFundSourceRepository.ListGeneralFunds(row.Cell(1).Value.ToString());
                                eamisFundSourceDTO.GenFundId = fundId.Result == null ? 0 : fundId.Result[0].ID;

                                var financeSourceId = _eamisFinancingSourceRepository.ListFinancingSource(row.Cell(3).Value.ToString());
                                eamisFundSourceDTO.FinancingSourceId = financeSourceId.Result == null ? 0 : financeSourceId.Result[0].ID;

                                var authorizationId = _eamisAuthorizationRepository.ListAuthorization(row.Cell(4).Value.ToString());
                                eamisFundSourceDTO.AuthorizationId = authorizationId.Result == null ? 0 : authorizationId.Result[0].ID;


                                eamisFundSourceDTO.Code = row.Cell(2).Value.ToString();
                                eamisFundSourceDTO.FundCategory = row.Cell(5).Value.ToString();
                                eamisFundSourceDTO.IsActive = Convert.ToBoolean(row.Cell(6).Value);
                                rowCtr += 1;
                                var result = await _eamisFundSourceRepository.InsertFromExcel(eamisFundSourceDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion fund

                        #region Procurement
                        case WorkSheetTemplateNames.ProcurementCategory:

                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisProcurementCategoryDTO eamisProcurementCategoryDTO = new EamisProcurementCategoryDTO();
                                eamisProcurementCategoryDTO.ProcurementDescription = row.Cell(1).Value.ToString();
                                eamisProcurementCategoryDTO.IsActive = Convert.ToBoolean(row.Cell(2).Value);
                                rowCtr += 1;
                                var result = await _eamisProcurementCategoryRepository.InsertFromExcel(eamisProcurementCategoryDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion Procurement

                        #region UOM
                        case WorkSheetTemplateNames.UnitofMeasurement:

                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisUnitofMeasureDTO eamisUnitofMeasureDTO = new EamisUnitofMeasureDTO();
                                eamisUnitofMeasureDTO.Short_Description = row.Cell(1).Value.ToString();
                                eamisUnitofMeasureDTO.Uom_Description = row.Cell(2).Value.ToString();
                                eamisUnitofMeasureDTO.isActive = Convert.ToBoolean(row.Cell(3).Value);
                                rowCtr += 1;
                                var result = await _eamisUnitofMeasureRepository.InsertFromExcel(eamisUnitofMeasureDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion UOM

                        #region Responsibility Center
                        case WorkSheetTemplateNames.ResponsibilityCenter:
                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisResponsibilityCenterDTO eamisResponsibilityCenterDTO = new EamisResponsibilityCenterDTO();
                                string officecode = row.Cell(5).Value.ToString().PadLeft(10, '0');
                                eamisResponsibilityCenterDTO.mainGroupCode = row.Cell(1).Value.ToString();
                                eamisResponsibilityCenterDTO.mainGroupDesc = row.Cell(2).Value.ToString();
                                eamisResponsibilityCenterDTO.subGroupCode = row.Cell(3).Value.ToString();
                                eamisResponsibilityCenterDTO.subGroupDesc = row.Cell(4).Value.ToString();
                                eamisResponsibilityCenterDTO.officeCode = officecode;
                                eamisResponsibilityCenterDTO.officeDesc = row.Cell(6).Value.ToString();
                                eamisResponsibilityCenterDTO.unitCode = row.Cell(7).Value.ToString();
                                eamisResponsibilityCenterDTO.unitDesc = row.Cell(8).Value.ToString();
                                eamisResponsibilityCenterDTO.isActive = Convert.ToBoolean(row.Cell(9).Value);
                                rowCtr += 1;
                                var result = await _eamisResponsibilityCenterRepository.InsertFromExcel(eamisResponsibilityCenterDTO);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                        #endregion Responsibility Center

                        #region Warehouse
                        case WorkSheetTemplateNames.Warehouse:
                            rowCtr += 1;
                            row = worksheet.Row(rowCtr);

                            while (!row.IsEmpty())
                            {
                                EamisWarehouseDTO eamisWarehouseDTO1 = new EamisWarehouseDTO();

                                eamisWarehouseDTO1.Warehouse_Description = row.Cell(0).Value.ToString();
                                eamisWarehouseDTO1.Street_Name = row.Cell(1).Value.ToString();
                                eamisWarehouseDTO1.Region_Code = Convert.ToInt32(row.Cell(2).Value);
                                eamisWarehouseDTO1.Municipality_Code = Convert.ToInt32(row.Cell(3).Value);
                                eamisWarehouseDTO1.Province_Code = Convert.ToInt32(row.Cell(4).Value);
                                eamisWarehouseDTO1.Barangay_Code = Convert.ToInt32(row.Cell(5).Value);
                                eamisWarehouseDTO1.IsActive = Convert.ToBoolean(row.Cell(6).Value);
                                rowCtr += 1;
                                var result = await _eamisWarehouseRepository.InsertFromExcel(eamisWarehouseDTO1);
                                row = worksheet.Row(rowCtr);
                            }
                            break;
                            #endregion Warehouse
                    }

                }
                bolerror = false;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return bolerror;
        }
    }
}