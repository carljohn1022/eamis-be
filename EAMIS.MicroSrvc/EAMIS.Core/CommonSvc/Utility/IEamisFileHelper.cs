using ClosedXML.Excel;
using EAMIS.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public interface IEamisFileHelper
    {

        Task<List<EAMISPROPERTYITEMS>> DownloadPropertyItems();

        Task<List<EAMISSUPPLIER>> DownloadSuppliers();

        Task<List<EAMISITEMCATEGORY>> DownloadCategories();

        Task<List<EAMISITEMSUBCATEGORY>> DownloadSubCategories();

        Task<List<EAMISCHARTOFACCOUNTS>> DownloadChartOfAccounts();

        Task<List<EAMISFUNDSOURCE>> DownloadFundSources();

        Task<List<EAMISPROCUREMENTCATEGORY>> DownloadProcurements();

        Task<List<EAMISRESPONSIBILITYCENTER>> DownloadResponsibilityCenters();

        Task<List<EAMISUNITOFMEASURE>> DownloadUOM();
        Task<string> DownloadExcelTemplate(string WorkSheetTemplateName);
        //Task<bool> UploadExcelToDB(string ExcelFilePath, string TemplateName);
        Task<bool> UploadFileToDB(string fileFormat, string FilePath, string TemplateName);
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
    }
}