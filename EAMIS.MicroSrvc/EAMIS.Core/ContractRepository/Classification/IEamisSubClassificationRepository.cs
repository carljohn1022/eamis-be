using EAMIS.Common.DTO.Classification;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Classification
{
    public interface IEamisSubClassificationRepository
    {
        Task<DataList<EamisSubClassificationDTO>> List(EamisSubClassificationDTO filter, PageConfig config);
        Task<EamisSubClassificationDTO> Insert(EamisSubClassificationDTO item);
        Task<EamisSubClassificationDTO> Update(EamisSubClassificationDTO item, int Id);
        Task<EamisSubClassificationDTO> Delete(EamisSubClassificationDTO item, int Id);
        Task<List<EAMISSUBCLASSIFICATION>> ListSubClassifications(string searchValue);
    }
}
