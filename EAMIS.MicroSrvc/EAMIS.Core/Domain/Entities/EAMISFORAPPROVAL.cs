using System;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISFORAPPROVAL
    {
        public int ID { get; set; }
        public string TRANSACTION_NUMBER { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string DOCSTATUS { get; set; }
        public DateTime? TIMESTAMP { get; set; }

        public int APPROVER1_ID { get; set; }
        public string APPROVER1_STATUS { get; set; }
        public DateTime? APPROVER1_TRANDATE { get; set; }
        public string APPROVER1_REJECTEDREASON { get; set; }

        public int APPROVER2_ID { get; set; }
        public string APPROVER2_STATUS { get; set; }
        public DateTime? APPROVER2_TRANDATE { get; set; }
        public string APPROVER2_REJECTEDREASON { get; set; }
        public int APPROVER3_ID { get; set; }
        public string APPROVER3_STATUS { get; set; }
        public DateTime? APPROVER3_TRANDATE { get; set; }
        public string APPROVER3_REJECTEDREASON { get; set; }


    }
}
