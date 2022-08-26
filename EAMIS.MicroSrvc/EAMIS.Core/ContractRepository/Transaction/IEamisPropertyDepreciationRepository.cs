using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyDepreciationRepository
    {
        Task<DataList<EamisPropertyDepreciationDTO>> List(EamisPropertyDepreciationDTO filter, PageConfig config);
        Task<DataList<EamisPropertyDepreciationDTO>> ListForDepreciationCreation(EamisPropertyDepreciationDTO filter, PageConfig config);
        Task<EamisPropertyDepreciationDTO> Insert(EamisPropertyDepreciationDTO item);
        Task<EamisPropertyDepreciationDTO> Update(EamisPropertyDepreciationDTO item);
        Task<EamisPropertyDepreciationDTO> Delete(EamisPropertyDepreciationDTO item);
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
    }
}
