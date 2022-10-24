using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisWarehouseRepository
    {
        Task<DataList<EamisWarehouseDTO>> SearchWarehouse(string searchType, string searchValue);
        Task<DataList<EamisWarehouseDTO>> List(EamisWarehouseDTO filter, PageConfig config);
        Task<EamisWarehouseDTO> Insert(EamisWarehouseDTO item);
        Task<EamisWarehouseDTO> Update(EamisWarehouseDTO item, int id);
        Task<EamisWarehouseDTO> Delete(EamisWarehouseDTO item);
        Task<bool> ValidateExistingWarehouse(string warehouseDesc);
        Task<bool> EditValidationWarehouse(int id, string warehouseDesc);
        Task<List<EAMISWAREHOUSE>> ListAllWarehouse();

        Task<EamisWarehouseDTO> InsertFromExcel(EamisWarehouseDTO item);

        Task<bool> InsertFromExcel(List<EamisWarehouseDTO> Items);
        string ErrorMessage { get; set; }
    }
}
