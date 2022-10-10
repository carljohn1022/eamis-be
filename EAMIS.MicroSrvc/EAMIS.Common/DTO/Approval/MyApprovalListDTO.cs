using System;

namespace EAMIS.Common.DTO.Approval
{
    public class MyApprovalListDTO
    {
        public int SourceId { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string DocStatus { get; set; }
        public int UserId { get; set; }
        public int ApprovalLevel { get; set; }
        public string Status { get; set; }
        public DateTime? Trandate { get; set; }
        public string RejectedReason { get; set; }

    }
}
