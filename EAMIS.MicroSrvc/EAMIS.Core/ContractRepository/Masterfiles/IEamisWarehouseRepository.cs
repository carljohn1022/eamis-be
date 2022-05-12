using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisWarehouseRepository
    {
        Task<DataList<EamisWarehouseDTO>> List(EamisWarehouseDTO filter, PageConfig config);
        Task<EamisWarehouseDTO> Insert(EamisWarehouseDTO item);
        Task<EamisWarehouseDTO> Update(EamisWarehouseDTO item);
        Task<EamisWarehouseDTO> Delete(EamisWarehouseDTO item);
        Task<bool> ValidateExistingCode(string warehouseDescription);
        Task<bool> UpdateValidateExistingCode(string warehouseDescription, int id);
    }
}
