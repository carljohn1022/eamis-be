using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISFUNDSOURCE
    {
        [Key]
        public int ID { get; set; }
        public string CODE { get; set; }
        public int GENERAL_FUND_SOURCE_ID { get; set; }
        public int FINANCING_SOURCE_ID { get; set; }
        public int AUTHORIZATION_ID { get; set; }
        public string FUND_CATEGORY { get; set; }
        public bool IS_ACTIVE { get; set; }
        public EAMISGENERALFUNDSOURCE GENERALFUNDSOURCE { get; set; }
        public EAMISFINANCINGSOURCE FINANCING_SOURCE { get; set; }
        public EAMISAUTHORIZATION AUTHORIZATION { get; set; }
    }
}
