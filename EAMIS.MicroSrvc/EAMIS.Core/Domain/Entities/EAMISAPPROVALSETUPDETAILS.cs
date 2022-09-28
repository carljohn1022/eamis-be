namespace EAMIS.Core.Domain.Entities
{
    public class EAMISAPPROVALSETUPDETAILS
    {
        public int ID { get; set; }
        public int APPROVALSETUP_ID { get; set; }
        public int SIGNATORY_ID { get; set; }
        public int VIEW_LEVEL { get; set; }
        public decimal MIN_AMOUNT { get; set; }
        public decimal MAX_AMOUNT { get; set; }
    }
}
