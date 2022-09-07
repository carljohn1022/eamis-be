using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisFundSourceRepository
    {
        Task<DataList<EamisFundSourceDTO>> SearchFunds(string type, string searchValue);
        Task<DataList<EamisFundSourceDTO>> List(EamisFundSourceDTO filter, PageConfig config);
        Task<EamisFundSourceDTO> Insert(EamisFundSourceDTO item);
        Task<EamisFundSourceDTO> Update(EamisFundSourceDTO item,int Id);
        Task<EamisFundSourceDTO> Delete(EamisFundSourceDTO item,int Id);
        Task<bool> ValidateExistingCode(string code);
        Task<bool> UpdateValidateExistingCode(string code, int id);
        Task<EamisFundSourceDTO> InsertFromExcel(EamisFundSourceDTO item);
        Task<List<EAMISFUNDSOURCE>> ListAllFundSources();
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
        Task<bool> InsertFromExcel(List<EamisFundSourceDTO> Items);
    }
}
