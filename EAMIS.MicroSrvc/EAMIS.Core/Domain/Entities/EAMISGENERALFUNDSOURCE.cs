using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISGENERALFUNDSOURCE
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public List<EAMISFUNDSOURCE> FUNDSOURCE { get; set; }
    }
}
