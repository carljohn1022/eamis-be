using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;


namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisResponsibilityCodeRepository
    {
        Task<DataList<EamisResponsibilityCodeDTO>> SearchResCenter(string type, string searchValue);
        Task<DataList<EamisResponsibilityCodeDTO>> List(EamisResponsibilityCodeDTO filter, PageConfig config);
        Task<EamisResponsibilityCodeDTO> Insert(EamisResponsibilityCodeDTO item);
        Task<EamisResponsibilityCodeDTO> Update(EamisResponsibilityCodeDTO item, int id);
        Task<bool> ValidateExistingCode(string office, string department);
        Task<bool> UpdateValidateExistingCode(string office, string department, int id);
    }
}
