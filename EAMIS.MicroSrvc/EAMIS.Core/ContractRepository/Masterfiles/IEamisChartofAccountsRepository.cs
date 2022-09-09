using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisChartofAccountsRepository
    {
        Task<DataList<EamisChartofAccountsDTO>> SearchChart(string type, string searchValue);
        Task<DataList<EamisChartofAccountsDTO>> List(EamisChartofAccountsDTO filter, PageConfig config);
        Task<EamisChartofAccountsDTO> Insert(EamisChartofAccountsDTO item);
        Task<EamisChartofAccountsDTO> Update(EamisChartofAccountsDTO item, int Id);
        Task<EamisChartofAccountsDTO> Delete(EamisChartofAccountsDTO item, int Id);
        Task<bool> ValidateExistingAccountCode(string accountCode);
        Task<bool> EditValidationAccountCode(int id, string accountCode);
        Task<EamisChartofAccountsDTO> InsertFromExcel(EamisChartofAccountsDTO item);
        Task<List<EAMISCHARTOFACCOUNTS>> ListCOA(string searchValue);
        Task<List<EAMISCHARTOFACCOUNTS>> ListCOAById(int coaId);

        Task<List<EAMISCHARTOFACCOUNTS>> ListAllCOA();
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
        Task<bool> InsertFromExcel(List<EamisChartofAccountsDTO> Items);
    }
}
