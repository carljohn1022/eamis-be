using ClosedXML.Excel;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Helper;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.ContractRepository.Classification;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
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
        #endregion dependency injection


        private string _errorMessage = "";
        private bool bolerror = false;
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }
        public bool bolError { get => bolerror; set => value = bolerror; }

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
                               IEamisProcurementCategoryRepository eamisProcurementCategoryRepository)
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
        }
        #endregion constructor


        public Task<string> DownloadExcelTemplate(string WorkSheetTemplateName)
        {
            //var file = await Task.Run(() => ExcelHelper.GenerateExcel(WorkSheetTemplateName)).ConfigureAwait(false);
            //return file;
            return null;
        }

        public async Task<bool> UploadExcelToDB(string ExcelFilePath, string TemplateName)
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
                                eamisSupplierDTO.AccountNumber = Convert.ToInt32(row.Cell(12).Value);
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
                                eamisItemCategoryDTO.IsSupplies = row.Cell(4).Value.ToString().ToLower() == "supplies" ? true : false;
                                eamisItemCategoryDTO.IsAsset = row.Cell(5).Value.ToString().ToLower() == "asset" ? true : false;
                                eamisItemCategoryDTO.IsSerialized = row.Cell(6).Value.ToString().ToLower() == "serialized" ? true : false;
                                eamisItemCategoryDTO.IsStockable = row.Cell(7).Value.ToString().ToLower() == "stockable" ? true : false;
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
                    }

                }
                bolerror = true;
            }
            catch (Exception ex)
            {
                bolerror = false;
                _errorMessage = ex.Message;
                return false;
                
            }
            return bolerror;
        }
    }
}
