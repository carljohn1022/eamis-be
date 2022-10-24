using EAMIS.Common.DTO.Masterfiles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisAttachedFilesRepository
    {
        Task<bool> Insert(List<EamisAttachedFilesDTO> files);
        Task<string> GetTranFileName(string transactionNumber, string fileName);
        Task<bool> DeleteImageFileName(string transactionNumber, string fileName);
        Task<bool> Delete(EamisAttachedFilesDTO file);
        Task<string> GetTranFileName(string transactionNumber);

    }
}