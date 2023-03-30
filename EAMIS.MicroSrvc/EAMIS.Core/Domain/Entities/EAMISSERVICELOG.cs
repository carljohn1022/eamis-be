using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISSERVICELOG
    {
        [Key]
        public int ID { get; set; }
        public string SERVICE_LOG_TYPE { get; set; }
        public string TRAN_ID { get; set; }
        public DateTime TRAN_DATE { get; set; }
        public string TRANSACTION_STATUS { get; set; }
        public string USER_STAMP { get; set; }
        public string BRANCH_ID { get; set; }
        public List<EAMISSERVICELOGDETAILS> SERVICE_LOG_DETAILS { get; set; }
    }
}
