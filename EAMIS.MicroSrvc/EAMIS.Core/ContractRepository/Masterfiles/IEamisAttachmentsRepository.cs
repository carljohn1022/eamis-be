using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisAttachmentsRepository
    {
        Task<DataList<EamisAttachmentsDTO>> List(EamisAttachmentsDTO filter, PageConfig config);
        Task<EamisAttachmentsDTO> Insert(EamisAttachmentsDTO item);
        Task<EamisAttachmentsDTO> Update(EamisAttachmentsDTO item);
        Task<EamisAttachmentsDTO> Delete(EamisAttachmentsDTO item);
    }
}
