using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYSCHEDULE
    {

        public int ID { get; set; }
        public string PROPERTY_NUMBER { get; set; } //to verify the correct data type with Justin
        public string STATUS { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public string CATEGORY { get; set; }
        public string SUB_CATEGORY { get; set; }
        public bool FOR_DEPRECIATION { get; set; }
        public decimal AREA_SQM { get; set; }
        public string SERIAL_NO { get; set; } //to verify the correct data type with Justin
        public string LOCATION { get; set; }
        public string DEPARTMENT { get; set; }
        public string LAST_DEPARTMENT { get; set; }
        public int WARRANTY { get; set; }
        public string INVOICE_NO { get; set; } //to verify the correct data type with Justin
        public int SVC_AGREEMENT_NO { get; set; }
        public string VENDORNAME { get; set; }
        public string ASSET_TAG { get; set; }
        public string COST_CENTER { get; set; }
        public string NAMES { get; set; }
        public string DETAILS { get; set; }
        public DateTime WARRANTY_DATE { get; set; }
        public DateTime RRDATE { get; set; }
        public string ASSET_CONDITION { get; set; }
        public decimal REVALUATION_COST { get; set; }
        public decimal DISPOSED_AMOUNT { get; set; }
        public DateTime ACQUISITION_DATE { get; set; }

        public string POREF { get; set; }
        public string RRREF { get; set; }
        public decimal ACQUISITION_COST { get; set; }
        public decimal SALVAGE_VALUE { get; set; }
        public int EST_LIFE { get; set; } //to verify the correct data type with Justin
        public decimal DEPREC_AMOUNT { get; set; }
        public decimal DEPRECIABLE_COST { get; set; } //  public decimal BOOK_VALUE { get; set; }
        public decimal BOOK_VALUE { get; set; }
        public decimal ASSESSED_VALUE { get; set; }
        public decimal APPRAISED_VALUE { get; set; }
        public decimal APPRAISAL_INCREMENT { get; set; }
        public decimal REAL_ESTATE_TAX_PAYMENT { get; set; }
        public DateTime LAST_POSTED_DATE { get; set; }
        public string ITEM_CODE { get; set; }
        public int REMAINING_LIFE { get; set; }
        public int REFERENCE_ID { get; set; } //Property Transaction Details ID
        public decimal ACCUMULATED_DEPREC_AMT { get; set; }
    }
}
