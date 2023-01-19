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

        Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config, string tranType, int assigneeCustodian);
        Task<DataList<EamisPropertyTransactionDetailsDTO>> ListSupplyItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config, string tranType, int assigneeCustodian);
        Task<EamisPropertyTransactionDTO> InsertProperty(EamisPropertyTransactionDTO item);       
        Task<EamisPropertyTransactionDTO> InsertPropertyForMaterialIssuance(EamisPropertyTransactionDTO item);
        Task<EamisPropertyTransactionDetailsDTO> InsertPropertyTransaction(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDTO> UpdateProperty(EamisPropertyTransactionDTO item);
        Task<string> UpdatePropertyItemQty(EamisDeliveryReceiptDetailsDTO item);
        Task<EamisPropertyTransactionDetailsDTO> UpdateDetails(EamisPropertyTransactionDetailsDTO item);
        Task<EamisPropertyTransactionDTO> getPropertyItemById(int itemID);
        Task<string> GetNextSequenceNumber(string tranType);
        Task<string> GetNextSequenceNumberForMaterialIssuance();
        Task<DataList<EamisPropertyTransactionDetailsDTO>> SearchReceiving(string type, string searchValue);
        Task<DataList<EamisDeliveryReceiptDetailsDTO>> SearchDRForMaterialIssuance(string type, string searcValue);
        Task<string> GetResponsibilityCenterByID(string responsibilityCode);
        Task<string> GetPropertyNumber(DateTime acquisitionDate, string responsibilityCode, string serialNumber);
        //Task<EamisPropertyTransactionDetailsDTO> Delete(EamisPropertyTransactionDetailsDTO item);
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
        //Task<string> GeneratePropertyNumber(int transactionDetailId, string itemCode, string responsibilityCode); Remove Saglit
        Task<string> GeneratePropertyNumber(DateTime acquisitionDate, string itemCode, string responsibilityCode);
        Task<string> GetDRNumFrSupplier(string dr);
        Task<string> GetAPRNum(string dr);
        Task<DataList<EamisDeliveryReceiptDetailsDTO>> ListSuppliesDRForIssuance(EamisDeliveryReceiptDetailsDTO filter, PageConfig config);
    }
}