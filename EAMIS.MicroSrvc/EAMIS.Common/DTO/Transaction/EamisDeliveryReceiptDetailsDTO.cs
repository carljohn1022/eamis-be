using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisDeliveryReceiptDetailsDTO
    {
        public int Id { get; set; }
        public int DeliveryReceiptId  { get; set; }
        public int ItemId { get; set; }
        //public string ItemCode { get; set; }
        //public string ItemDescription { get; set; }
        public int QtyOrder  { get; set; }
        public int QtyDelivered  { get; set; }
        public int QtyRejected  { get; set; }
        public int QtyReceived  { get; set; }
        public decimal UnitCost  { get; set; }
        public int SerialNumber  { get; set; }
        public string UnitOfMeasurement  { get; set; }
        public decimal SubTotal { get; set; }
        //public DateTime WarrantyExpiryDate  { get; set; }
        public Masterfiles.EamisPropertyItemsDTO PropertyItem { get; set; }
        public EamisDeliveryReceiptDTO DeliveryReceipt { get; set; }
        public EamisSerialTranDTO SerialTran { get; set; }
        public List<EamisSerialTranDTO> PropertySerialTran { get; set; }
        public List<EamisDeliveryReceiptDTO> DeliveryReceiptGroup { get; set; }
        public int IssuedQty { get; set; }
        public int RemainingQty { get; set; }

    }
}
