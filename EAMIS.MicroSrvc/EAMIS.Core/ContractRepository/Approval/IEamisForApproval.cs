using EAMIS.Common.DTO.Approval;
using EAMIS.Core.Response.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Approval
{
    public interface IEamisForApprovalRepository
    {
        Task<DataList<EamisForApprovalDTO>> List(EamisForApprovalDTO filter, PageConfig config);
        Task<EamisForApprovalDTO> Insert(EamisForApprovalDTO item, decimal totalAmount);
        Task<MyApprovalListDTO> SubmitApproval(MyApprovalListDTO item);
        Task<EamisForApprovalDTO> Update(EamisForApprovalDTO item);
        Task<EamisForApprovalDTO> Delete(EamisForApprovalDTO item);
        string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        Task<List<EamisForApprovalDTO>> FirstApproverList(int userId, string transactionType);
        Task<List<EamisForApprovalDTO>> SecondApproverList(int userId, string transactionType);
        Task<List<EamisForApprovalDTO>> ThirdApproverList(int userId, string transactionType);

        Task<List<MyApprovalListDTO>> MyApprovalList(int userId, string transactionType);
    }
}
