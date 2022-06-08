using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISSUPPLIER
    {
        [Key]
        public int ID { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_DESCRIPTION { get; set; }
        public string CONTACT_PERSON_NAME { get; set; }
        public string CONTACT_PERSON_NUMBER { get; set; }
        public int REGION_CODE { get; set; }
        public int PROVINCE_CODE { get; set; }
        public int CITY_MUNICIPALITY_CODE { get; set; }
        public int BRGY_CODE { get; set; }
        public string STREET { get; set; }
        public string BANK { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public int ACCOUNT_NUMBER { get; set; }
        public string BRANCH { get; set; }
        public bool IS_ACTIVE { get; set; }

        [ForeignKey("BRGY_CODE")]
        public EAMISBARANGAY BARANGAY_GROUP { get; set; }
        public List<EAMISPROPERTYITEMS> PROPERTY_ITEMS { get; set; }
        public List<EAMISDELIVERYRECEIPT> DELIVERY_RECEIPT { get; set; }

    }
}
