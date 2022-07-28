﻿using EAMIS.Common.DTO.Masterfiles;
using System;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisDeliveryReceiptDetailsDTO
    {
        public int Id { get; set; }
        public int DeliveryReceiptId  { get; set; }
        public int ItemId { get; set; }
        public int QtyOrder  { get; set; }
        public int QtyDelivered  { get; set; }
        public int QtyRejected  { get; set; }
        public int QtyReceived  { get; set; }
        public decimal UnitCost  { get; set; }
        public int SerialNumber  { get; set; }
        public string UnitOfMeasurement  { get; set; }
        //public DateTime WarrantyExpiryDate  { get; set; }
        public EamisPropertyItemsDTO PropertyItem { get; set; }
        public EamisDeliveryReceiptDTO DeliveryReceipt { get; set; }

    }
}
