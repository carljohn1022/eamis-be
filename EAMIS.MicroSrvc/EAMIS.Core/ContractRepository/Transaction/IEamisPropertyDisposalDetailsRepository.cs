using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyDisposalDetailsRepository
    {
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
        Task<DataList<EamisPropertyTransactionDetailsDTO>> ListForDisposal(EamisPropertyTransactionDetailsDTO filter, PageConfig config);
        Task<DataList<EamisPropertyTransactionDetailsDTO>> List(EamisPropertyTransactionDetailsDTO filter, PageConfig config);
        Task<EamisPropertyTransactionDetailsDTO> Insert(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDetailsDTO> Update(EamisPropertyTransactionDetailsDTO item);
    }
}