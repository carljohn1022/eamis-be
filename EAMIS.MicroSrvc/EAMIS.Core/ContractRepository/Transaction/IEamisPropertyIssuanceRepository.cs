using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyIssuanceRepository
    {
        Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config);
        Task<EamisPropertyTransactionDTO> InsertProperty(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDetailsDTO> InsertPropertyTransaction(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDTO> UpdateProperty(EamisPropertyTransactionDTO item);
        Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item);
        Task<string> GetNextSequenceNumber();
    }
}
