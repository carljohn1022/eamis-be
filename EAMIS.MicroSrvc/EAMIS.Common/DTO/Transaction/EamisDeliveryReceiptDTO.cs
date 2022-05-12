using System;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisDeliveryReceiptDTO
    {
        public int Id { get; set; }
        public string TransactionType  { get; set; }
        public string Supplier  { get; set; }
        public string PurchaseOrderNumber  { get; set; }
        public DateTime PurchaseOrderDate  { get; set; }
        public string PurchaseRequestNumber  { get; set; }
        public DateTime PurchaseRequestDate  { get; set; }
        public string SaleInvoiceNumber  { get; set; }
        public DateTime SaleInvoiceDate  { get; set; }
        public int TotalAmount  { get; set; }
        public string TransactionStatus  { get; set; }
        public int SerialLot  { get; set; }
    }
}
