using System;

namespace EAMIS.Common.DTO.Approval
{
    public class EamisForApprovalDTO
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionType { get; set; }
        public string DocStatus { get; set; }
        public DateTime? TimeStamp { get; set; }

        public int Approver1Id { get; set; }
        public string Approver1Status { get; set; }
        public DateTime? Approver1Trandate { get; set; }
        public string Approver1RejectedReason { get; set; }

        public int Approver2Id { get; set; }
        public string Approver2Status { get; set; }
        public DateTime? Approver2Trandate { get; set; }
        public string Approver2RejectedReason { get; set; }
       
        public int Approver3Id { get; set; }
        public string Approver3Status { get; set; }
        public DateTime? Approver3Trandate { get; set; }
        public string Approver3RejectedReason { get; set; }

        public int Approver4Id { get; set; }
        public string Approver4Status { get; set; }
        public DateTime? Approver4Trandate { get; set; }
        public string Approver4RejectedReason { get; set; }

        public int Approver5Id { get; set; }
        public string Approver5Status { get; set; }
        public DateTime? Approver5Trandate { get; set; }
        public string Approver5RejectedReason { get; set; }


    }
}
