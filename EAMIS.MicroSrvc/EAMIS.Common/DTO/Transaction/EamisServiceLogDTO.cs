using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisServiceLogDTO
    {
        public int Id { get; set; }
        public string ServiceLogType { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionStatus { get; set; }
        public string UserStamp { get; set; }
        public string BranchID { get; set; }
        public List<EamisServiceLogDetailsDTO> ServiceLogDetails { get; set; }
    }
}
