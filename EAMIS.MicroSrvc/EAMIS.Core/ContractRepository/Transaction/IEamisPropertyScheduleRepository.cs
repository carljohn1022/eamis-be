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
        Task<EamisPropertyScheduleDTO> Update(EamisPropertyScheduleDTO item);
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
    }
}
