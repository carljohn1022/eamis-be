using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyRevaluationDTO
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Particulars { get; set; }
        public bool IsActive { get; set; }
        public string UserStamp { get; set; }
        public string Status { get; set; }
        public List<EamisPropertyRevaluationDetailsDTO> PropertyRevaluationDetails { get; set; }
    }
}
