using EAMIS.Common.DTO.Approval;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Approval
{
    public interface IEamisApprovalSetupRepository
    {
        Task<DataList<EamisApprovalSetupDTO>> List(EamisApprovalSetupDTO filter, PageConfig config);
        Task<EamisApprovalSetupDTO> Insert(EamisApprovalSetupDTO item);
        Task<EamisApprovalSetupDTO> Update(EamisApprovalSetupDTO item);
        Task<EamisApprovalSetupDTO> Delete(EamisApprovalSetupDTO item);
        string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
