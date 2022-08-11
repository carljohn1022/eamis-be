using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
   public interface IEamisPropertyTransferRepository
    {
        Task<DataList<EamisPropertyTransactionDTO>> List(EamisPropertyTransactionDTO filter, PageConfig config);
        Task<DataList<EamisPropertyTransferDetailsDTO>> List(EamisPropertyTransferDetailsDTO filter, PageConfig config);
        Task<string> GetNextSequenceNumber();
        Task<EamisPropertyTransactionDTO> Insert(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransferDetailsDTO> Insert(EamisPropertyTransferDetailsDTO item);
    }
}
