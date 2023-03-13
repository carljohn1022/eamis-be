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
        public string ACCOUNT_NUMBER { get; set; }
        public string BRANCH { get; set; }
        public string EMAIL_ADD { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string USER_STAMP { get; set; }

        [ForeignKey("BRGY_CODE")]
        public EAMISBARANGAY BARANGAY_GROUP { get; set; }
        public List<EAMISPROPERTYITEMS> PROPERTY_ITEMS { get; set; }
        public List<EAMISDELIVERYRECEIPT> DELIVERY_RECEIPT { get; set; }
        public List<EAMISSERVICELOGDETAILS> SERVICE_LOG_DETAILS { get; set; }

    }
}
