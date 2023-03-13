using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisFundSourceDTO
    {
        public int Id { get; set; }
        public int GenFundId { get; set; }
        public string Code { get; set; }
        public int FinancingSourceId { get; set; }
        public int AuthorizationId { get; set; }
        public string FundCategory { get; set; }
        public string UserStamp { get; set; }
        public bool IsActive { get; set; }
        public EamisGeneralFundSourceDTO GeneralFundSource { get; set; }
        public EamisFinancingSourceDTO FinancingSource { get; set; }
        public EamisAuthorizationDTO Authorization { get; set; }
      
    }
}
