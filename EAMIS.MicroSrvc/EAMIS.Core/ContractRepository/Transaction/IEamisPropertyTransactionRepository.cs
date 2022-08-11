﻿using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyTransactionRepository
    {
        Task<DataList<EamisPropertyTransactionDTO>> List(EamisPropertyTransactionDTO filter, PageConfig config);
        Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDTO> Update(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDTO> Delete(EamisPropertyTransactionDTO item);
        Task<string> GetNextSequenceNumberPR();
        Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID);
    }
}
