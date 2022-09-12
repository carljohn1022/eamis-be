using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyRevalutionRepository
    {
        Task<string> GetNextSequenceNumber();
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
        Task<DataList<EamisPropertyRevaluationDTO>> List(EamisPropertyRevaluationDTO filter, PageConfig config);

        Task<EamisPropertyRevaluationDTO> Insert(EamisPropertyRevaluationDTO item);
        Task<EamisPropertyRevaluationDTO> Update(EamisPropertyRevaluationDTO item);
        Task<DataList<EamisPropertyRevaluationDTO>> SearchPropertyRevaluation(string type, string searchValue);
    }
}
