using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyScheduleRepository
    {
        Task<DataList<EamisPropertyScheduleDTO>> List(EamisPropertyScheduleDTO filter, PageConfig config);
        Task<DataList<EamisPropertyScheduleDTO>> Search(string type, string searchValue);
        Task<EamisPropertyScheduleDTO> Update(EamisPropertyScheduleDTO item);

        Task<DataList<EamisPropertyScheduleDTO>> ListItemsForRevaluationCreation(EamisPropertyScheduleDTO filter, PageConfig config);

        Task<string> GetEstimatedLife(string itemCode);
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
    }
}
