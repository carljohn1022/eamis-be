using System;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYTRANSACTIONDETAILS
    {
        public int ID { get; set; }
        public int PROPERTY_TRANSACTION_ID { get; set; }
        public string PROPERTY_NUMBER { get; set; }
        public string PROPERTY_DESCRIPTION { get; set; }
        public DateTime RECEIVING_DATE { get; set; }
        public string DELIVERY_RECEIPT_NUMBER { get; set; }
        public string PURCHASE_REQUEST_NUMBER { get; set; }
        public DateTime PURCHASE_REQUEST_DATE { get; set; }
        public string PURCHASE_ORDER_NUMBER { get; set; }
        public DateTime PURCHASE_ORDER_DATE { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public int CUSTODIAN_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public int QTY { get; set; }
        public int UNIT_COST { get; set; }
        public int SALVAGE_VALUE { get; set; }
        public int ESTIMATED_LIFE { get; set; }
        public int BOOK_VALUE { get; set; }
        public int FORD_DEPRECIATION { get; set; }
        public string USER_STAMP { get; set; }
        public DateTime TIME_STAMP { get; set; }
        public int ITEM_ID { get; set; }
        public DateTime WARRANTY_START_DATE { get; set; }
        public string PROPERTY_CONDITION { get; set;}
        public int SEMI_EXPANDABLE_AMOUNT { get; set; }
        public int PROPERTY_KIT_ID { get; set; }
        public int AREA_SQM { get; set; }
        public string OFFICE { get; set; }
    }
}
