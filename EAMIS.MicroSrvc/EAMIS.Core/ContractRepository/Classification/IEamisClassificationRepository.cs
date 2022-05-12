using EAMIS.Common.DTO.Classification;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Classification
{
    public interface IEamisClassificationRepository
    {
        Task<DataList<EamisClassificationDTO>> List(EamisClassificationDTO filter, PageConfig config);
        Task<EamisClassificationDTO> Insert(EamisClassificationDTO item);
        Task<EamisClassificationDTO> Update(EamisClassificationDTO item, int Id);
        Task<EamisClassificationDTO> Delete(EamisClassificationDTO item, int Id);
    }
}
