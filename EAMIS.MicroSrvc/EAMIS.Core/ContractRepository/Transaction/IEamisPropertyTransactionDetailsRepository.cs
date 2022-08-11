using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System.Threading.Tasks;


namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyTransactionDetailsRepository
    {
        Task<DataList<EamisPropertyTransactionDetailsDTO>> List(EamisPropertyTransactionDetailsDTO filter, PageConfig config);
        Task<EamisPropertyTransactionDetailsDTO> Insert(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDetailsDTO> Update(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDetailsDTO> getPropertyItemById(int itemID);

        Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item);
    }
}
