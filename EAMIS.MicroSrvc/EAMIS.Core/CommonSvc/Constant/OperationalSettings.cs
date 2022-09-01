using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Constant
{
    public static class ModuleName
    {
        public const string DeliveryReceiptName = "Delivery_Receipt";
        public const string PropertyReceivingName = "Property_Receiving";
        public const string PropertyTransferName = "Property_Transfer";
        public const string PropertyIssuanceName = "Property_Issuance";
        public const string PropertyDisposalName = "Property_Disposal";
    }
    public static class TransactionTypeSettings
    {
        public const string DeliveryReceipt = "Delivery Receipt";
        public const string PropertyReceiving = "Property Receiving";
        public const string Issuance = "Issuance/Releasing"; // "Property Issuance";
        public const string PropertyTransfer = "Property Transfer";
        public const string ServiceLog = "Service Log";
        public const string PropertyDisposal = "Property Disposal";
        public const string PropertyRevaluation = "Property Revaluation";
        public const string AssetSchedule = "Asset Schedule";
    }

    public static class PropertyItemStatus
    {
        public const string ForCondemn = "FOR CONDEMN";
    }

    public static class FactorTypes
    {
        public const string SalvageValue = "SV";
    }

    public static class PrefixSettings
    {
        public const string DRPrefix = "DR";//Delivery receipt prefix. change according to the business rule.
        public const string PRPrefix = "PR";//Property receiving prefix. change according to the business rule.
        public const string ISPrefix = "I"; //Propery issuance prefix. change according to the business rule.
        public const string PTPrefix = "PT"; //Property transfer prefix. change according to the business rule.
        public const string SLPrefix = "SL"; //Service Log prefix. change according to the business rule.
        public const string PDPrefix = "PD"; //Property Disposal prefix. change according to the business rule.
        public const string ASPrefix = "AS"; //Asset Schedule prefix. change according to the business rule.
        public const string PVPrefix = "PV"; //Property Revaluation prefix. change according to the business rule.
    }

    public static class WorkSheetTemplateNames
    {
        public const string Items = "Items";
        public const string Suppliers = "Suppliers";
        public const string Category = "Category";
        public const string SubCategory = "SubCategory";
        public const string Warehouse = "Warehouse";
        public const string ChartOfAccount = "ChartOfAccount";
        public const string Funds = "Funds";
        public const string ProcurementCategory = "ProcurementCategory";
        public const string UnitofMeasurement = "UnitofMeasurement";
        public const string ResponsibilityCenter = "Responsibility Center";
    }
    public static class WorkSheetNames
    {
        public const string Items = "Item List";
        public const string Supplier = "Supplier List";
        public const string CategoryList = "Category List";
        public const string SubCategoryList = "Sub_Category List";
        public const string WarehouseList = "Warehouse List";
    }
    public static class FolderName
    {
        public const string StaticFolderLocation = "StaticFiles";
        public const string EAMISAttachmentLocation = "EAMIS_Attached_Files";
        public const string PropertyItemExcelFileLocation = @"Uploaded\Excel\";
        public const string PropertyItemCSVFileLocation = @"Uploaded\CSV\";
    }

    public static class FileFormat
    {
        public const string ExcelFile = ".xlsx";
        public const string CSVFile = ".csv";
        public const string Separator = ","; //to do: change when separator is finalized/decided
    }

    public static class FileType
    {
        public const string SpreadSheetType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
