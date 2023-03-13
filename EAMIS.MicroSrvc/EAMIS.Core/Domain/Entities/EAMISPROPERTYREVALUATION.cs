using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYREVALUATION
    {
        public int ID { get; set; }
        public string TRAN_ID { get; set; }
        public DateTime TRAN_DATE { get; set; }
        public string PARTICULARS { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string USER_STAMP { get; set; }
        public string STATUS { get; set; }
        //public List<EAMISPROPERTYREVALUATIONDETAILS> PropertyRevaluationDetails { get; set; }
    }
}
