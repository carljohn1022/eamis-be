using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public interface IEamisFileHelper
    {
        Task<string> DownloadExcelTemplate(string WorkSheetTemplateName);
        Task<bool> UploadExcelToDB(string ExcelFilePath, string TemplateName);
        string ErrorMessage { get; set; }
        bool bolError { get; set; }
    }
}
