using ClosedXML.Excel;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.CommonSvc.Utility;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisFileController : ControllerBase
    {
        IEamisFileHelper _eamisFileHelper;
        IEamisItemCategoryRepository _eamisItemCategoryRepository;
        IEamisAttachedFilesRepository _eamisAttachedFilesRepository;
        IEamisItemSubCategoryRepository _eamisItemSubCategoryRepository;
        IEamisWarehouseRepository _eamisWareHouseRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private bool bolWithData = false;
 

        private bool TemplateWithData { get => bolWithData; set => value = bolWithData; }

        public EamisFileController(IEamisFileHelper eamisFileHelper, IEamisItemCategoryRepository eamisItemCategoryRepository,
                                    IEamisItemSubCategoryRepository eamisItemSubCategoryRepository,
                                    IEamisWarehouseRepository eamisWareHouseRepository,
                                    IWebHostEnvironment hostingEnvironment, IEamisAttachedFilesRepository eamisAttachedFilesRepository)
        {
            _eamisFileHelper = eamisFileHelper;
            _eamisItemCategoryRepository = eamisItemCategoryRepository;
            _eamisItemSubCategoryRepository = eamisItemSubCategoryRepository;
            _eamisAttachedFilesRepository = eamisAttachedFilesRepository;
            _eamisWareHouseRepository = eamisWareHouseRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet("getFileName")]
        public async Task<string> getFileName(string transactionNumber)
        {
            var response = await _eamisAttachedFilesRepository.GetTranFileName(transactionNumber);
            return response;
        }

        [HttpGet("DownloadExcelWorkSheetTemplate")]
        public async Task<ActionResult> ExportToExcel(string WorkSheetTemplateName, bool bolWithData = false)
        {
            this.bolWithData = bolWithData;
            switch (WorkSheetTemplateName)
            {
                case WorkSheetTemplateNames.Items:
                    return await Item();
                case WorkSheetTemplateNames.Suppliers:
                    return await Supplier();
                case WorkSheetTemplateNames.Category:
                    return await Category();
                case WorkSheetTemplateNames.SubCategory:
                    return await SubCategory();
                case WorkSheetTemplateNames.Warehouse:
                    return await Warehouse();
                case WorkSheetTemplateNames.ChartOfAccount:
                    return await ChartOfAccount();
                case WorkSheetTemplateNames.Funds:
                    return await Funds();
                case WorkSheetTemplateNames.ProcurementCategory:
                    return await ProcurementCategory();
                case WorkSheetTemplateNames.UnitofMeasurement:
                    return await UnitOfMeasurement();
                case WorkSheetTemplateNames.ResponsibilityCenter:
                    return await ResponsibilityCenter();
            }
            return null;
        }

        #region export
        private async Task<ActionResult> Item()
        {
            string filename = WorkSheetTemplateNames.Items + ".xlsx";
            using (var workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(WorkSheetNames.Items);

                IXLWorksheet item =
                workbook.Worksheet(WorkSheetNames.Items);
                item.Cell(1, 1).Value = "STOCKROOM ADDRESS";
                item.Cell(1, 2).Value = "PROPERTY_NAME";
                item.Cell(1, 3).Value = "BRAND";
                item.Cell(1, 4).Value = "MODEL";
                item.Cell(1, 5).Value = "PROPERTY_TYPE";
                item.Cell(1, 6).Value = "UOM";
                item.Cell(1, 7).Value = "SUPPLIER";
                item.Cell(1, 8).Value = "QUANTITY";
                item.Cell(1, 9).Value = "APP_NO";
                item.Cell(1, 10).Value = "CATEGORY NAME";
                item.Cell(1, 11).Value = "IS_ACTIVE";
                item.Cell(1, 12).Value = "PROPERTY_NO";
                item.Cell(1, 13).Value = "SUBCATEGORY_NAME";
                //required using System.IO;
                int rowCtr = 2;
                if (TemplateWithData)
                {
                    List<EAMISPROPERTYITEMS> result = await _eamisFileHelper.DownloadPropertyItems();
                    foreach (var propItem in result)
                    {
                        item.Cell(rowCtr, 1).Value = propItem.WAREHOUSE_GROUP.WAREHOUSE_DESCRIPTION;
                        item.Cell(rowCtr, 2).Value = propItem.PROPERTY_NAME;
                        item.Cell(rowCtr, 3).Value = propItem.BRAND;
                        item.Cell(rowCtr, 4).Value = propItem.MODEL;
                        item.Cell(rowCtr, 5).Value = propItem.PROPERTY_TYPE;
                        item.Cell(rowCtr, 6).Value = propItem.UOM_GROUP.UOM_DESCRIPTION;
                        item.Cell(rowCtr, 7).Value = propItem.SUPPLIER_GROUP.COMPANY_DESCRIPTION;
                        item.Cell(rowCtr, 8).Value = propItem.QUANTITY;
                        item.Cell(rowCtr, 9).Value = propItem.APP_NO;
                        item.Cell(rowCtr, 10).Value = propItem.ITEM_CATEGORY.CATEGORY_NAME;
                        item.Cell(rowCtr, 11).Value = propItem.IS_ACTIVE;
                        item.Cell(rowCtr, 12).Value = propItem.PROPERTY_NO;
                        item.Cell(rowCtr, 13).Value = propItem.ITEM_CATEGORY.SHORT_DESCRIPTION;
                        rowCtr++;
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> Supplier()
        {
            string filename = WorkSheetTemplateNames.Suppliers + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetNames.Supplier);

                IXLWorksheet supplier =
                workbook.Worksheet(WorkSheetNames.Supplier);
                supplier.Cell(1, 1).Value = "COMPANY_NAME";
                supplier.Cell(1, 2).Value = "COMPANY_DESCRIPTION";
                supplier.Cell(1, 3).Value = "CONTACT_PERSON_NAME";
                supplier.Cell(1, 4).Value = "CONTACT_PERSON_NUMBER";
                supplier.Cell(1, 5).Value = "REGION";
                supplier.Cell(1, 6).Value = "PROVINCE";
                supplier.Cell(1, 7).Value = "CITY_MUNICIPALITY";
                supplier.Cell(1, 8).Value = "BRGY";
                supplier.Cell(1, 9).Value = "STREET";
                supplier.Cell(1, 10).Value = "BANK";
                supplier.Cell(1, 11).Value = "ACCOUNT_NAME";
                supplier.Cell(1, 12).Value = "ACCOUNT_NUMBER";
                supplier.Cell(1, 13).Value = "BRANCH";
                supplier.Cell(1, 14).Value = "IS_ACTIVE";
                int rowCtr = 2;
                if (TemplateWithData)
                {
                    List<EAMISSUPPLIER> suppliers = await _eamisFileHelper.DownloadSuppliers();
                    foreach (var item in suppliers)
                    {
                        supplier.Cell(rowCtr, 1).Value = item.COMPANY_NAME;
                        supplier.Cell(rowCtr, 2).Value = item.COMPANY_DESCRIPTION;
                        supplier.Cell(rowCtr, 3).Value = item.CONTACT_PERSON_NAME;
                        supplier.Cell(rowCtr, 4).Value = item.CONTACT_PERSON_NUMBER;
                        supplier.Cell(rowCtr, 5).Value = item.BARANGAY_GROUP.REGION.REGION_DESCRIPTION;
                        supplier.Cell(rowCtr, 6).Value = item.BARANGAY_GROUP.PROVINCE.PROVINCE_DESCRITION;
                        supplier.Cell(rowCtr, 7).Value = item.BARANGAY_GROUP.MUNICIPALITY.CITY_MUNICIPALITY_DESCRIPTION;
                        supplier.Cell(rowCtr, 8).Value = item.BARANGAY_GROUP.BRGY_DESCRIPTION;
                        supplier.Cell(rowCtr, 9).Value = item.STREET;
                        supplier.Cell(rowCtr, 10).Value = item.BANK;
                        supplier.Cell(rowCtr, 11).Value = item.ACCOUNT_NAME;
                        supplier.Cell(rowCtr, 12).Value = item.ACCOUNT_NUMBER;
                        supplier.Cell(rowCtr, 13).Value = item.BRANCH;
                        supplier.Cell(rowCtr, 14).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }

                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> Category()
        {
            string filename = WorkSheetTemplateNames.Category + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetNames.CategoryList);

                IXLWorksheet category =
                workbook.Worksheet(WorkSheetNames.CategoryList);


                category.Cell(1, 1).Value = "CHART_OF_ACCOUNT_CODE";
                category.Cell(1, 2).Value = "CATEGORY_NAME";
                category.Cell(1, 3).Value = "SHORT_DESCRIPTION";
                category.Cell(1, 4).Value = "SUPPLIES";
                category.Cell(1, 5).Value = "ASSET";
                category.Cell(1, 6).Value = "SERIALIZED";
                category.Cell(1, 7).Value = "STOCKABLE";
                category.Cell(1, 8).Value = "COST_METHOD";
                category.Cell(1, 9).Value = "ESTIMATED_LIFE";
                category.Cell(1, 10).Value = "DEPRECIATION_METHOD";
                category.Cell(1, 11).Value = "ACTIVE";

                if (TemplateWithData)
                {
                    List<EAMISITEMCATEGORY> categories = await _eamisFileHelper.DownloadCategories();
                    int rowCtr = 2;
                    foreach (var item in categories)
                    {
                        category.Cell(rowCtr, 1).Value = item.CHART_OF_ACCOUNTS.ACCOUNT_CODE;
                        category.Cell(rowCtr, 2).Value = item.CATEGORY_NAME;
                        category.Cell(rowCtr, 3).Value = item.SHORT_DESCRIPTION;
                        category.Cell(rowCtr, 4).Value = item.IS_SUPPLIES;
                        category.Cell(rowCtr, 5).Value = item.IS_ASSET;
                        category.Cell(rowCtr, 6).Value = item.IS_SERIALIZED;
                        category.Cell(rowCtr, 7).Value = item.IS_STOCKABLE;
                        category.Cell(rowCtr, 8).Value = item.COST_METHOD;
                        category.Cell(rowCtr, 9).Value = item.ESTIMATED_LIFE;
                        category.Cell(rowCtr, 10).Value = item.DEPRECIATION_METHOD;
                        category.Cell(rowCtr, 11).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> SubCategory()
        {
            string filename = WorkSheetTemplateNames.SubCategory + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetNames.SubCategoryList);

                IXLWorksheet subcategory =
                workbook.Worksheet(WorkSheetNames.SubCategoryList);
                subcategory.Cell(1, 1).Value = "Category_Name"; //Category ID
                subcategory.Cell(1, 2).Value = "Sub_Category_Name"; //Sub Category Name
                //subcategory.Cell(1, 3).Value = "Property item id"; //Property item id
                subcategory.Cell(1, 3).Value = "Active"; //Short Description

                if (TemplateWithData)
                {
                    List<EAMISITEMSUBCATEGORY> subcategories = await _eamisFileHelper.DownloadSubCategories();
                    int rowCtr = 2;
                    foreach (var item in subcategories)
                    {
                        subcategory.Cell(rowCtr, 1).Value = item.ITEM_CATEGORY.CATEGORY_NAME; //Category ID
                        subcategory.Cell(rowCtr, 2).Value = item.SUB_CATEGORY_NAME; //Sub Category Name
                        //subcategory.Cell(rowCtr, 3).Value = item.PROPERTY_ITEM; //Property item id
                        subcategory.Cell(rowCtr, 3).Value = item.IS_ACTIVE; //Short Description
                        rowCtr++;
                    }
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> Warehouse()
        {
            string filename = WorkSheetTemplateNames.Warehouse + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetNames.WarehouseList);

                IXLWorksheet warehouse =
                workbook.Worksheet(WorkSheetNames.WarehouseList);
                warehouse.Cell(1, 1).Value = "ID"; // ID
                warehouse.Cell(1, 2).Value = "Warehouse Description";//Warehouse Description
                warehouse.Cell(1, 3).Value = "Street Name";//street name
                warehouse.Cell(1, 4).Value = "Region Code";//Region code
                warehouse.Cell(1, 5).Value = "Municipality Code";//Municipality code
                warehouse.Cell(1, 6).Value = "Province Code";//Province code
                warehouse.Cell(1, 7).Value = "Barangay Code";//Barangay code
                warehouse.Cell(1, 8).Value = "IsActive";//IsActive?

                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> ChartOfAccount()
        {
            string filename = WorkSheetTemplateNames.ChartOfAccount + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetTemplateNames.ChartOfAccount);

                IXLWorksheet coa = workbook.Worksheet(WorkSheetTemplateNames.ChartOfAccount);
                coa.Cell(1, 1).Value = "CLASSIFICATION";
                coa.Cell(1, 2).Value = "SUBCLASSIFICATION";
                coa.Cell(1, 3).Value = "GROUP";
                coa.Cell(1, 4).Value = "OBJECT_CODE";
                coa.Cell(1, 5).Value = "UACS";
                coa.Cell(1, 6).Value = "PART_OF_INVENTORY";
                coa.Cell(1, 7).Value = "ACTIVE";
                if (TemplateWithData)
                {
                    List<EAMISCHARTOFACCOUNTS> coas = await _eamisFileHelper.DownloadChartOfAccounts();
                    int rowCtr = 2;
                    foreach (var item in coas)
                    {
                        //coa.Cell(rowCtr, 1).Value = item.CLASSIFICATION;
                        //coa.Cell(rowCtr, 2).Value = "SUBCLASSIFICATION";
                        coa.Cell(rowCtr, 3).Value = item.GROUPCLASSIFICATION.NAME_GROUPCLASSIFICATION;
                        coa.Cell(rowCtr, 4).Value = item.OBJECT_CODE;
                        coa.Cell(rowCtr, 5).Value = item.ACCOUNT_CODE;
                        coa.Cell(rowCtr, 6).Value = item.IS_PART_OF_INVENTORY;
                        coa.Cell(rowCtr, 7).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> Funds()
        {
            string filename = WorkSheetTemplateNames.Funds + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetTemplateNames.Funds);

                IXLWorksheet fundsource = workbook.Worksheet(WorkSheetTemplateNames.Funds);
                fundsource.Cell(1, 1).Value = "FUND";
                fundsource.Cell(1, 2).Value = "CODE";
                fundsource.Cell(1, 3).Value = "FINANCIAL_SOURCE";
                fundsource.Cell(1, 4).Value = "AUTHORIZATION";
                fundsource.Cell(1, 5).Value = "FUND_CATEGORY";
                fundsource.Cell(1, 6).Value = "ACTIVE";

                if (TemplateWithData)
                {
                    List<EAMISFUNDSOURCE> result = await _eamisFileHelper.DownloadFundSources();
                    int rowCtr = 2;
                    foreach (var item in result)
                    {
                        fundsource.Cell(rowCtr, 1).Value = item.FUND_CATEGORY;
                        fundsource.Cell(rowCtr, 2).Value = item.CODE;
                        fundsource.Cell(rowCtr, 3).Value = item.FINANCING_SOURCE.FINANCING_SOURCE_NAME;
                        fundsource.Cell(rowCtr, 4).Value = item.AUTHORIZATION.AUTHORIZATION_NAME;
                        fundsource.Cell(rowCtr, 5).Value = item.GENERALFUNDSOURCE.NAME;
                        fundsource.Cell(rowCtr, 6).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }
                }

                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> ProcurementCategory()
        {
            string filename = WorkSheetTemplateNames.ProcurementCategory + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetTemplateNames.ProcurementCategory);

                IXLWorksheet coa = workbook.Worksheet(WorkSheetTemplateNames.ProcurementCategory);
                coa.Cell(1, 1).Value = "CATEGORY_DESC";
                coa.Cell(1, 2).Value = "ACTIVE";

                if (TemplateWithData)
                {
                    List<EAMISPROCUREMENTCATEGORY> result = await _eamisFileHelper.DownloadProcurements();
                    int rowCtr = 2;
                    foreach (var item in result)
                    {
                        coa.Cell(rowCtr, 1).Value = item.PROCUREMENT_DESCRIPTION;
                        coa.Cell(rowCtr, 2).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> UnitOfMeasurement()
        {
            string filename = WorkSheetTemplateNames.UnitofMeasurement + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetTemplateNames.UnitofMeasurement);
                IXLWorksheet uom = workbook.Worksheet(WorkSheetTemplateNames.UnitofMeasurement);
                uom.Cell(1, 1).Value = "UOM_SHORTDESC";
                uom.Cell(1, 2).Value = "UOM_DESC";
                uom.Cell(1, 3).Value = "ACTIVE";

                if (TemplateWithData)
                {
                    List<EAMISUNITOFMEASURE> result = await _eamisFileHelper.DownloadUOM();
                    int rowCtr = 2;
                    foreach (var item in result)
                    {
                        uom.Cell(rowCtr, 1).Value = item.SHORT_DESCRIPTION;
                        uom.Cell(rowCtr, 2).Value = item.UOM_DESCRIPTION;
                        uom.Cell(rowCtr, 3).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }

        private async Task<ActionResult> ResponsibilityCenter()
        {
            string filename = WorkSheetTemplateNames.ResponsibilityCenter + ".xlsx";
            using (var workbook = new XLWorkbook())
            {

                workbook.Worksheets.Add(WorkSheetTemplateNames.ResponsibilityCenter);
                IXLWorksheet center = workbook.Worksheet(WorkSheetTemplateNames.ResponsibilityCenter);
                center.Cell(1, 1).Value = "Main Code";
                center.Cell(1, 2).Value = "Main Description";
                center.Cell(1, 3).Value = "Sub Code";
                center.Cell(1, 4).Value = "Sub Description";
                center.Cell(1, 5).Value = "Office Code";
                center.Cell(1, 6).Value = "Office Description";
                center.Cell(1, 7).Value = "Unit Code";
                center.Cell(1, 8).Value = "Unit Description";
                center.Cell(1, 9).Value = "Active";

                if (TemplateWithData)
                {
                    List<EAMISRESPONSIBILITYCENTER> result = await _eamisFileHelper.DownloadResponsibilityCenters();
                    int rowCtr = 2;
                    foreach (var item in result)
                    {
                        center.Cell(rowCtr, 1).Value = item.MAIN_GROUP_CODE;
                        center.Cell(rowCtr, 2).Value = item.MAIN_GROUP_DESC;
                        center.Cell(rowCtr, 3).Value = item.SUB_GROUP_CODE;
                        center.Cell(rowCtr, 4).Value = item.SUB_GROUP_DESC;
                        center.Cell(rowCtr, 5).Value = item.OFFICE_CODE;
                        center.Cell(rowCtr, 6).Value = item.OFFICE_DESC;
                        center.Cell(rowCtr, 7).Value = item.UNIT_CODE;
                        center.Cell(rowCtr, 8).Value = item.UNIT_DESC;
                        center.Cell(rowCtr, 9).Value = item.IS_ACTIVE;
                        rowCtr++;
                    }
                }
                //required using System.IO;
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return await Task.Run(() => File(
                       content,
                       FileType.SpreadSheetType,
                       filename)).ConfigureAwait(false);
                }
            }
        }
        #endregion

        [HttpPost("UploadFile")]
        public ActionResult UploadExcel(IFormFile file, string TemplateName = WorkSheetTemplateNames.Items)
        {
            if(file == null)
                return BadRequest("File not found.");

            string fileName = string.Empty;
            string extensionName = System.IO.Path.GetExtension(file.FileName).ToLower();
            string fileFormat = string.Empty;
            if (file != null &&
                (extensionName == FileFormat.ExcelFile || extensionName == FileFormat.CSVFile))
            {
                string targetPath = Path.Combine(_hostingEnvironment.WebRootPath, FolderName.StaticFolderLocation + @"\" +
                                                 extensionName == FileFormat.ExcelFile ?
                                                 FolderName.PropertyItemExcelFileLocation : FolderName.PropertyItemCSVFileLocation);
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(targetPath, fileName);

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath); //create the target path if not yet exist

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                fileFormat = extensionName == FileFormat.ExcelFile ? FileFormat.ExcelFile : FileFormat.CSVFile;
                _eamisFileHelper.UploadFileToDB(fileFormat, filePath, TemplateName);

                if (_eamisFileHelper.HasError)
                    return BadRequest(_eamisFileHelper.ErrorMessage);
                else
                {
                    //Delete file when uploaded successfully
                    FileInfo fileinfo = new FileInfo(filePath);
                    if (fileinfo.Exists) //check the file if it exist in the repository
                        fileinfo.Delete(); //if file found/exist then delete it
                }
            }
            return Ok();
        }

        [HttpGet("DownloadReport")]
        public async Task<ActionResult> DownloadReport(string RptReqCode,
                                                       string RptCode,
                                                       string ParFldVal,
                                                       int GenTyp)
        {
            var result = await _eamisFileHelper.GenerateReport(RptReqCode, RptCode, ParFldVal, GenTyp);

            if (_eamisFileHelper.HasError)
                return BadRequest(_eamisFileHelper.ErrorMessage);

            //Check generated filename
            while (_eamisFileHelper.IsReportReady == ReportStatus.ReportNotReady)
            {
                await _eamisFileHelper.IsReportCompleted(result.ID);
                if (_eamisFileHelper.IsReportReady == ReportStatus.ReportReady)
                {
                    result.RptIsReady = ReportStatus.ReportReady;
                    result.GenRptFilNam = _eamisFileHelper.ReportFileName;
                    result.RptStatus = _eamisFileHelper.RptStatus;
                    result.FileImage = _eamisFileHelper.FileImage;
                }
            }

            if (result.GenRptFilNam == ReportStatus.ErrorFound)
                return BadRequest(result.RptStatus);

            //create physical file of image
            string targetPath = Path.Combine(_hostingEnvironment.WebRootPath, FolderName.StaticFolderLocation + @"\" + FolderName.ReportFileLocation);
            string fileName = result.GenRptFilNam;
            byte[] byteFile = result.FileImage;

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath); //create the target path if not yet exist

            using (System.IO.FileStream fs = new System.IO.FileStream(targetPath + fileName, FileMode.Create, System.IO.FileAccess.ReadWrite))
            {
                using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                {
                    bw.Write(byteFile);
                    bw.Close();
                }
            }

            string fileType = fileName.Substring(fileName.IndexOf('.') + 1);
            bool bolReportTypeIsValid = true;
            switch (fileType.ToLower())
            {
                case FileFormat.CSV:
                    fileType = FileType.CSVType;
                    break;
                case FileFormat.Pdf:
                    fileType = FileType.PDFType;
                    break;
                case FileFormat.Excel:
                    fileType = FileType.ExcelType;
                    break;
                default:
                    bolReportTypeIsValid = false;
                    break;
            }

            if (!bolReportTypeIsValid)
                return BadRequest("Either report type or format is invalid.");

            //delete request 
            await _eamisFileHelper.Delete(result.ID);

            byte[] pdfBytes = System.IO.File.ReadAllBytes(targetPath + fileName);
            using (var stream = new MemoryStream())
            {
                var content = stream.ToArray();
                return await Task.Run(() => File(
                   pdfBytes,
                   fileType,
                   fileName)).ConfigureAwait(false);
            }

        }
    }
}