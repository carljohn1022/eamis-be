using System;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyTransactionDetailsDTO
    {
        public int Id { get; set; }
        public int PropertyTransactionId  { get; set; }
        public string PropertyNumber  { get; set; }
        public string PropertyDescription  { get; set; }
        public DateTime ReceivingDate  { get; set; }
        public string DeliveryReceiptNumber  { get; set; }
        public string PurchaseRequestNumber  { get; set; }
        public DateTime PurchaseRequestDate  { get; set; }
        public string PurchaseOrderNumber  { get; set; }
        public DateTime PurchaseOrderDate  { get; set; }
        public string SerialNumber  { get; set; }
        public int CustodianId  { get; set; }
        public string Department  { get; set; }
        public int Qty  { get; set; }
        public int UnitCost  { get; set; }
        public int SalvageValue  { get; set; }
        public int EstimatedLife  { get; set; }
        public int BookValue  { get; set; }
        public int FordDepreciation  { get; set; }
        public string UserStamp  { get; set; }
        public DateTime TimeStamp  { get; set; }
        public int ItemId  { get; set; }
        public DateTime WarrantyStartDate  { get; set; }
        public string PropertyCondition  { get; set; }
        public int SemiExpandableAmount  { get; set; }
        public int PropertyKitId  { get; set; }
        public int AreaSqm  { get; set; }
        public string Office  { get; set; }
    }
}
