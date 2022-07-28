using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISSERVICELOGDETAILS
    {
        [Key]
        public int ID { get; set; }
        public int SERVICE_LOG_ID { get; set; }
        public int RECEIVING_TRAN_ID { get; set; }
        public string PROPERTY_NUMBER { get; set; }
        public string PROPERTY_DESC { get; set; }
        public string ASSET_CONDITION { get; set; }
        public decimal RECEIVING_AMOUNT { get; set; }
        public int SUPPLIER_ID { get; set; }
        public string SUPPLIER_DESC { get; set; }
        public DateTime SERVICE_DATE { get; set; }
        public DateTime DUE_DATE { get; set; }
        public decimal ASSESSED_VALUE { get; set; }
        public decimal APPRAISED_VALUE { get; set; }
        public decimal APPRAISAL_INCREMENT { get; set; }
        public decimal REAL_ESTATE_TAX_PAYMENT { get; set; }
        public decimal AREA_SQM { get; set; }
        public string NOTES { get; set; }
        public EAMISSUPPLIER SUPPLIER_GROUP { get; set; }
        public EAMISPROPERTYTRANSACTION RECEIVING_GROUP { get; set; }
        public EAMISSERVICELOG SERVICE_LOG_GROUP { get; set; }

    }
}
