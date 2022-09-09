using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISGROUPCLASSIFICATION
    {
        public int ID { get; set; }
        public int CLASSIFICATION_ID { get; set; }
        public int SUB_CLASSIFICATION_ID { get; set; }
        public int CHART_OF_ACCOUNT_ID { get; set; }
        public string NAME_GROUPCLASSIFICATION { get; set; }
        public string PPE_SUB_MAJOR_ACCT_GRP { get; set; }
        public EAMISCLASSIFICATION CLASSIFICATION { get; set; }
        public EAMISSUBCLASSIFICATION SUBCLASSIFICATION { get; set; }
        public List<EAMISCHARTOFACCOUNTS> CHARTOFACCOUNTS { get; set; }

    }
}
