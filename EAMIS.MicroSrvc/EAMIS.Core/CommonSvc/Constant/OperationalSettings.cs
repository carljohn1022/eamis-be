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
        public const string Issuance = "Issuance";
        public const string PropertyTransfer = "Property Transfer";
        public const string ServiceLog = "Service Log";

    }

    public static class PrefixSettings
    {
        public const string DRPrefix = "DR";
        public const string PRPrefix = "RV";
        public const string ISPrefix = "I";
        public const string PTPrefix = "PT";
        public const string SLPrefix = "SL";
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
    }
    public static class FileType
    {
        public const string SpreadSheetType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
