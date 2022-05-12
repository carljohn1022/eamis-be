using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisPropertySuppliesRepository
    {
        Task<DataList<EamisPropertySuppliesDTO>> List(EamisPropertySuppliesDTO filter, PageConfig config);
        Task<EamisPropertySuppliesDTO> Insert(EamisPropertySuppliesDTO item);
        Task<EamisPropertySuppliesDTO> Update(EamisPropertySuppliesDTO item);
        Task<EamisPropertySuppliesDTO> Delete(EamisPropertySuppliesDTO item);
    }
}
