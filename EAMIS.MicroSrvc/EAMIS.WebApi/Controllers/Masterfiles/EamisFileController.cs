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
        IEamisItemSubCategoryRepository _eamisItemSubCategoryRepository;
        IEamisWarehouseRepository _eamisWareHouseRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;

        

        public EamisFileController(IEamisFileHelper eamisFileHelper, IEamisItemCategoryRepository eamisItemCategoryRepository,
                                    IEamisItemSubCategoryRepository eamisItemSubCategoryRepository,
                                    IEamisWarehouseRepository eamisWareHouseRepository,
                                    IWebHostEnvironment hostingEnvironment)
        {
            _eamisFileHelper = eamisFileHelper;
            _eamisItemCategoryRepository = eamisItemCategoryRepository;
            _eamisItemSubCategoryRepository = eamisItemSubCategoryRepository;
            _eamisWareHouseRepository = eamisWareHouseRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("DownloadExcelWorkSheetTemplate")]
        public async Task<ActionResult> ExportToExcel(string WorkSheetTemplateName)
        {
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
                subcategory.Cell(1, 1).Value = "Category ID"; //Category ID
                subcategory.Cell(1, 2).Value = "Sub Category Name"; //Sub Category Name
                subcategory.Cell(1, 3).Value = "Property item id"; //Property item id
                subcategory.Cell(1, 4).Value = "Short Description"; //Short Description

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

                IXLWorksheet coa = workbook.Worksheet(WorkSheetTemplateNames.Funds);
                coa.Cell(1, 1).Value = "FUND";
                coa.Cell(1, 2).Value = "CODE";
                coa.Cell(1, 3).Value = "FINANCIAL_SOURCE";
                coa.Cell(1, 4).Value = "AUTHORIZATION";
                coa.Cell(1, 5).Value = "FUND_CATEGORY";
                coa.Cell(1, 6).Value = "ACTIVE";

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
                IXLWorksheet coa = workbook.Worksheet(WorkSheetTemplateNames.UnitofMeasurement);
                coa.Cell(1, 1).Value = "UOM_SHORTDESC";
                coa.Cell(1, 2).Value = "UOM_DESC";
                coa.Cell(1, 3).Value = "ACTIVE";

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

        [HttpPost("UploadExcelFile")]
        public ActionResult UploadExcel(IFormFile file, string TemplateName = "")
        {
            string fileName = "";
            if (file != null && System.IO.Path.GetExtension(file.FileName).ToLower() == ".xlsx")
            {
                string targetPath = Path.Combine(_hostingEnvironment.ContentRootPath, @"StaticFiles\Uploaded\Excel\");
                fileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(targetPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                _eamisFileHelper.UploadExcelToDB(filePath, TemplateName);
                if (_eamisFileHelper.bolError)
                    return BadRequest(_eamisFileHelper.ErrorMessage);

            }
            return Ok();
        }

    }
}
