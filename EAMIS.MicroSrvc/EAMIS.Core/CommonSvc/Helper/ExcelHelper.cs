using ClosedXML.Excel;
using EAMIS.Core.CommonSvc.Constant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Helper
{
    public class ExcelHelper
    {

        private string DownloadItemSupplierTemplate()
        {
            string filename = WorkSheetTemplateNames.Items + ".xlsx";
            try
            {

                using var wbook = new XLWorkbook();
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet category =
                    workbook.Worksheet(WorkSheetNames.CategoryList);
                    category.Cell(2, 1).Value = "1"; //Chart of Account ID
                    category.Cell(2, 2).Value = "Sample Category";//Category Name
                    category.Cell(2, 3).Value = "short desc";//Short Description

                    IXLWorksheet subcategory =
                    workbook.Worksheet(WorkSheetNames.SubCategoryList);
                    subcategory.Cell(2, 1).Value = "1"; //Category ID
                    subcategory.Cell(2, 2).Value = "Sample Sub Category";//Sub Category Name
                    subcategory.Cell(2, 3).Value = "1";//Property item id
                    subcategory.Cell(2, 4).Value = "short desc";//Short Description

                    IXLWorksheet warehouse =
                    workbook.Worksheet(WorkSheetNames.WarehouseList);
                    warehouse.Cell(2, 1).Value = "1"; // ID
                    warehouse.Cell(2, 2).Value = "Sample Sub Category";//Warehouse Description
                    warehouse.Cell(2, 3).Value = "1";//stret name
                    warehouse.Cell(2, 4).Value = "short desc";//Region code
                    warehouse.Cell(2, 4).Value = "short desc";//Municipality code
                    warehouse.Cell(2, 4).Value = "short desc";//Province code
                    warehouse.Cell(2, 4).Value = "short desc";//Barangay code
                    warehouse.Cell(2, 4).Value = "short desc";//IsActive?

                    //required using System.IO;
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                       
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return null;
        }
        public string GenerateExcel(string ExcelTemplateName)
        {
            switch (ExcelTemplateName)
            {
                case WorkSheetTemplateNames.Items:
                    return DownloadItemSupplierTemplate();
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
