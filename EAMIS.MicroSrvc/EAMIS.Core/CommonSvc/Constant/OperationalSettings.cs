using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Constant
{
    public static class TransactionTypeSettings
    {
        public const string DeliveryReceipt = "Delivery Receipt";
        public const string PropertyReceiving = "Property Receiving";
    }

    public static class PrefixSettings
    {
        public const string DRPrefix = "DR";
        public const string PRPrefix = "PR";
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
    }
    public static class WorkSheetNames
    {
        public const string Items = "Item List";
        public const string Supplier = "Supplier List";
        public const string CategoryList = "Category List";
        public const string SubCategoryList = "Sub_Category List";
        public const string WarehouseList = "Warehouse List";
    }

    public static class FileType
    {
        public const string SpreadSheetType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
