using System;
using System.Collections.Generic;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyTransactionDetailsDTO
    {
        public int Id { get; set; }
        public int PropertyTransactionID { get; set; }
        public bool isDepreciation { get; set; }
        public int DeliveryRecieptID { get; set; }
        public string Dr { get; set; }
        public string PropertyNumber  { get; set; }
        public string ItemDescription  { get; set; }
        public string SerialNumber  { get; set; }
        public string Po  { get; set; }
        public string Pr  { get; set; }
        public DateTime AcquisitionDate  { get; set; }
        public int AssigneeCustodian  { get; set; }
        public string RequestedBy  { get; set; }
        public string Office  { get; set; }
        public string Department { get; set; }
        public string ResponsibilityCode  { get; set; }
        public int UnitCost  { get; set; }
        public int Qty  { get; set; }
        public int SalvageValue  { get; set; }
        public int BookValue  { get; set; }
        public int EstLife  { get; set; }
        public int Area { get; set; }
        public int Semi { get; set; }
        public string UserStamp  { get; set; }
        public DateTime TimeStamp  { get; set; }
        public DateTime WarrantyExpiry  { get; set; }
        public string Invoice  { get; set; }
        public string PropertyCondition  { get; set; }

        public EamisDeliveryReceiptDetailsDTO DeliveryReceiptDetails { get; set; }

    }

}
