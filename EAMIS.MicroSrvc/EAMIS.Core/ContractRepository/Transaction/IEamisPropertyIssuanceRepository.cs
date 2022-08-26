using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisPropertyIssuanceRepository
    {
        Task<DataList<EamisPropertyItemsDTO>> List(EamisPropertyItemsDTO filter, PageConfig config);

        Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config);
        Task<EamisPropertyTransactionDTO> InsertProperty(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDetailsDTO> InsertPropertyTransaction(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDTO> UpdateProperty(EamisPropertyTransactionDTO item);
        Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item);
        Task<EamisPropertyTransactionDetailsDTO> UpdateDetails(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID);
        Task<string> GetNextSequenceNumber();
        Task<DataList<EamisPropertyTransactionDetailsDTO>> SearchReceiving(string type, string searchValue);
        Task<string> GetResponsibilityCenterByID(string responsibilityCode);
        Task<string> GetPropertyNumber(DateTime acquisitionDate, string responsibilityCode);
        //Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item);
    }
}