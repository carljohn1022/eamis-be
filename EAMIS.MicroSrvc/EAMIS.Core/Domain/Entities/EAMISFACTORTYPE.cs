using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain
{
    public class EAMISFACTORTYPE
    {
        public int ID { get; set; }
        public string FACTOR_CODE { get; set; }
        public string FACTOR_NAME { get; set; }
        public decimal FACTOR_VALUE { get; set; }
        public string REMARKS { get; set; }
    }
}
