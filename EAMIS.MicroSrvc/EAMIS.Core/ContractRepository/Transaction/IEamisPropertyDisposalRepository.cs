using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyDisposalRepository
    {
        Task<DataList<EamisPropertyTransactionDTO>> List(EamisPropertyTransactionDTO filter, PageConfig config, string branchID);
        Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDTO> Update(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID);
        Task<string> GetNextSequenceNumber(string branchID);
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
    }
}