﻿using EAMIS.Common.DTO.Transaction;
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
        Task<string> GetNextSequenceNumber();
        Task<DataList<EamisServiceLogDTO>> ListServiceLogs(EamisServiceLogDTO filter, PageConfig config);
        Task<EamisServiceLogDTO> InsertServiceLog(EamisServiceLogDTO item);
        Task<EamisServiceLogDTO> UpdateServiceLog(EamisServiceLogDTO item);
    }
}