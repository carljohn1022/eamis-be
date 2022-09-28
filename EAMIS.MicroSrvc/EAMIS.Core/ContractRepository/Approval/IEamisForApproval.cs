using EAMIS.Common.DTO.Approval;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Approval
{
    public interface IEamisForApprovalRepository
    {
        Task<DataList<EamisForApprovalDTO>> List(EamisForApprovalDTO filter, PageConfig config);
        Task<EamisForApprovalDTO> Insert(EamisForApprovalDTO item);
        Task<EamisForApprovalDTO> Update(EamisForApprovalDTO item);
        Task<EamisForApprovalDTO> Delete(EamisForApprovalDTO item);
        string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
