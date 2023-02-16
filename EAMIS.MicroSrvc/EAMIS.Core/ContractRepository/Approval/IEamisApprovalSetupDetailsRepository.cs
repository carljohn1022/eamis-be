using EAMIS.Common.DTO.Approval;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Approval
{
    public interface IEamisApprovalSetupDetailsRepository
    {
        Task<DataList<EamisApprovalSetupDetailsDTO>> List(EamisApprovalSetupDetailsDTO filter, PageConfig config);
        Task<EamisApprovalSetupDetailsDTO> Insert(EamisApprovalSetupDetailsDTO item);
        Task<EamisApprovalSetupDetailsDTO> Update(EamisApprovalSetupDetailsDTO item);
        Task<EamisApprovalSetupDetailsDTO> Delete(EamisApprovalSetupDetailsDTO item);
        Task<int> getApprovalSetupId(string transactionType);
        string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
