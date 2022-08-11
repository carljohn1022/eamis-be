using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYTRANSACTIONDETAILS
    {
        public int ID { get; set; }
        public int PROPERTY_TRANS_ID { get; set; }
        public bool IS_DEPRECIATION { get; set; }
        public string DR { get; set; }
        public string ITEM_CODE { get; set; }
        public string PROPERTY_NUMBER { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string PO { get; set; }
        public string PR { get; set; }
        public DateTime ACQUISITION_DATE { get; set; }
        public int ASSIGNEE_CUSTODIAN { get; set; }
        public string REQUESTED_BY { get; set; }
        public string OFFICE { get; set; }
        public string DEPARTMENT { get; set;}
        public string RESPONSIBILITY_CODE { get; set; }
        public decimal UNIT_COST { get; set; }
        public int QTY { get; set; }
        public decimal SALVAGE_VALUE { get; set; }
        public decimal BOOK_VALUE { get; set; }
        public int ESTIMATED_LIFE { get; set; }
        public int AREA { get; set; }
        public decimal SEMI_EXPANDABLE_AMOUNT { get; set; }
        public string USER_STAMP { get; set; }
        public DateTime TIME_STAMP { get; set; }
        public int WARRANTY_EXPIRY { get; set; }
        public string INVOICE { get; set;}
        public string PROPERTY_CONDITION { get; set; }
        public EAMISPROPERTYTRANSACTION PROPERTY_TRANSACTION_GROUP { get; set; }

    }
}
