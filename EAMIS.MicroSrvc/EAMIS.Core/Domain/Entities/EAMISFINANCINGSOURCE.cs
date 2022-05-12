using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISFINANCINGSOURCE
    {
        [Key]
        public int ID { get; set; }
        public string FINANCING_SOURCE_NAME { get; set; }
        public List<EAMISFUNDSOURCE> FUND_SOURCE { get; set; }
    }
}
