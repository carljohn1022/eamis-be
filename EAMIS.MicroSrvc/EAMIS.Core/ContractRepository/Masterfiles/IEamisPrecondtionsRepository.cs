using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;


namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisPrecondtionsRepository
    {
        Task<DataList<EamisPreconditionsDTO>> List(EamisPreconditionsDTO filter, PageConfig config);
        Task<EamisPreconditionsDTO> Insert(EamisPreconditionsDTO item);
        Task<EamisPreconditionsDTO> Update(EamisPreconditionsDTO item);
        Task<EamisPreconditionsDTO> Delete(EamisPreconditionsDTO item);
    }
}
