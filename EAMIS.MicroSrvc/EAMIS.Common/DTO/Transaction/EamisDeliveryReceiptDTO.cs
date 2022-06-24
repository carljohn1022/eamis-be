using EAMIS.Common.DTO.Masterfiles;
using System;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisDeliveryReceiptDTO
    {
        public int Id { get; set; }
        public string TransactionType  { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime DateReceived { get; set; }
        public int SupplierId  { get; set; }
        public string PurchaseOrderNumber  { get; set; }
        public DateTime PurchaseOrderDate  { get; set; }
        public string PurchaseRequestNumber  { get; set; }
        public DateTime PurchaseRequestDate  { get; set; }
        public string SaleInvoiceNumber  { get; set; }
        public DateTime SaleInvoiceDate  { get; set; }
        public int TotalAmount  { get; set; }
        public string TransactionStatus  { get; set; }
        public int StockroomId { get; set; }
        public EamisWarehouseDTO Warehouse { get; set; }
        public EamisSupplierDTO Supplier { get; set; }


    }
}
