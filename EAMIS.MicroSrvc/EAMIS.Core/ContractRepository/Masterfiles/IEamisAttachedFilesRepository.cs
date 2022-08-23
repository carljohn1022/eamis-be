using EAMIS.Common.DTO.Masterfiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisAttachedFilesRepository
    {
        Task<bool> Insert(List<EamisAttachedFilesDTO> files);
        Task<bool> Delete(EamisAttachedFilesDTO file);
    }
}