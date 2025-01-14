﻿using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyTransferDetailsRepository
    {
        Task<DataList<EamisPropertyTransferDetailsDTO>> List(EamisPropertyTransferDetailsDTO filter, PageConfig config);
        Task<EamisPropertyTransferDetailsDTO> Insert(EamisPropertyTransferDetailsDTO item);
        Task<DataList<EamisPropertyTransferDetailsDTO>> ListItemsForTranser(EamisPropertyTransferDetailsDTO filter, string tranType, PageConfig config, string branchID, string responsibilityCode);
        Task<EamisPropertyTransferDetailsDTO> Update(EamisPropertyTransferDetailsDTO item);
        Task<string> GetResponsibilityCenterByID(string newResponsibilityCode);
        Task<DataList<EamisPropertyTransferDetailsDTO>> SearchIssuance(string type, string searchValue, int assigneeCustodian);
    }
}