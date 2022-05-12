using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPERSONNELINFO
    {
        [Key]
        public int ID { get; set; }
        public int USER_ID { get; set; }
        public int DIRECTORY_ID { get; set; }
        public string EMPLOYEE_ID { get; set; } 
        public string FIRST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string POSITION { get; set; }
        public string CONTACT_NUMBER { get; set; }
        public string E_MAIL { get; set; }
        public string REGION { get; set; }
        public string PROVINCE { get; set; }
        public string CITY { get; set; }
        public string BARANGAY { get; set; }
        public string STREET { get; set; }
        public bool IS_ACTIVE { get; set; }
        public bool IS_DELETED { get; set; }
        public EAMISUSERS USERS { get; set; }
    }
}
