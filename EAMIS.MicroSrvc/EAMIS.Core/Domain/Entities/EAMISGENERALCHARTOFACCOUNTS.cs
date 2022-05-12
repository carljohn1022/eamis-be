using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISGENERALCHARTOFACCOUNTS
    {
        public int ID { get; set; }
        public string CLASSIFICATION { get; set; }
        public string SUB_CLASSIFICATION { get; set; }
        public string CLASSIFICATION_GROUP { get; set; }
        public List<EAMISCHARTOFACCOUNTS> EAMIS_CHART_OF_ACCOUNTS { get; set; }
    }
}
