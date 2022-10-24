using EAMIS.Common.DTO.Masterfiles;

namespace EAMIS.Common.DTO.Approval
{
    public class EamisApprovalSetupDetailsDTO
    {
        public int Id { get; set; }
        public int ApprovalSetupId { get; set; }
        public int SignatoryId { get; set; }
        public int ViewLevel { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public EamisApprovalSetupDTO ApprovalSetup { get; set; }
        public EamisUsersDTO User { get; set; }
    }
}
