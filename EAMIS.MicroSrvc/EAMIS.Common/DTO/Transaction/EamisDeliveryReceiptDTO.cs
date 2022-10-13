using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisDeliveryReceiptDTO
    {
        public int Id { get; set; }
        public string TransactionType { get; set; }
        public string ReceivedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateReceived { get; set; }
        public int SupplierId { get; set; }
        public string PurchaseOrderNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime PurchaseOrderDate { get; set; }
        public string PurchaseRequestNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime PurchaseRequestDate { get; set; }
        public string SaleInvoiceNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime SaleInvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionStatus { get; set; }
        public int StockroomId { get; set; }
        public string DRNumFrSupplier { get; set; }
        public DateTime DRDate { get; set; }
        public string AprNum { get; set; }
        public DateTime AprDate { get; set; }
        public EamisWarehouseDTO Warehouse { get; set; }
        public EamisSupplierDTO Supplier { get; set; }
        public List<EamisAttachedFilesDTO> DeliveryImages { get; set; } = new List<EamisAttachedFilesDTO>();
        public List<EamisDeliveryReceiptDetailsDTO> DeliveryReceiptDetails { get; set; } = new List<EamisDeliveryReceiptDetailsDTO>();
        //public List<EamisDeliveryReceiptDetailsDTO> DeliveryReceiptDetailsList { get; set; } = new List<EamisDeliveryReceiptDetailsDTO>();
    }
}
