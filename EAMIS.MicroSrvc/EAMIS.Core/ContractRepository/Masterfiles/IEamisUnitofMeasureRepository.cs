using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisUnitofMeasureRepository
    {
        Task<DataList<EamisUnitofMeasureDTO>> List(EamisUnitofMeasureDTO filter, PageConfig config);
        Task<EamisUnitofMeasureDTO> Insert(EamisUnitofMeasureDTO item);
        Task<EamisUnitofMeasureDTO> Update(EamisUnitofMeasureDTO item);
        Task<EamisUnitofMeasureDTO> Delete(EamisUnitofMeasureDTO item);
    }
}
