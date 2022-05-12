using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;


namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisAttachmentTypeRepository
    {
        Task<DataList<EamisAttachmentTypeDTO>> List(EamisAttachmentTypeDTO filter, PageConfig config);
        Task<EamisAttachmentTypeDTO> Insert(EamisAttachmentTypeDTO item);
        Task<EamisAttachmentTypeDTO> Update(EamisAttachmentTypeDTO item);
        Task<EamisAttachmentTypeDTO> Delete(EamisAttachmentTypeDTO item);
    }
}
