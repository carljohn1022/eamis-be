using EAMIS.Common.DTO.Rolemanager;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Rolemanager
{
    public interface IEamisModulesRepository
    {
        Task<DataList<EamisModulesDTO>> List(EamisModulesDTO filter, PageConfig config);
        Task<EamisModulesDTO> Insert(EamisModulesDTO item);
        Task<EamisModulesDTO> Update(EamisModulesDTO item);
        Task<EamisModulesDTO> Delete(EamisModulesDTO item);
        string ErrorMessage { get; set; }
        public bool HasError { get; set; }
    }
}
