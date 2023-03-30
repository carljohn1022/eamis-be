using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisServiceLogRepository
    {
        Task<string> GetNextSequenceNumber(string branchID);
        Task<DataList<EamisServiceLogDTO>> ListServiceLogs(EamisServiceLogDTO filter, PageConfig config);
        Task<EamisServiceLogDTO> InsertServiceLog(EamisServiceLogDTO item);
        Task<EamisServiceLogDTO> UpdateServiceLog(EamisServiceLogDTO item);
        Task<EamisServiceLogDTO> getServiceItemById(int itemID);
        //Task<EamisServiceLogDTO> getServiceLogItemById(int itemID);

    }
}
