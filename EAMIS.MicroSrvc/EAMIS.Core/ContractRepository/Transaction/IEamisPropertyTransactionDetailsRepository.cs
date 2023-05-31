using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
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
        Task<string> UpdateIssuedPropertyItemQty(EamisPropertyTransactionDetailsDTO item);
        Task<string> UpdateIssuedMaterialItemQty(EamisPropertyTransactionDetailsDTO item);
        Task<string> GeneratePropertyNumber(DateTime acquisitionDate, string responsibilityCode, string serialNumber);
        Task<string> GeneratePropertyNumber(DateTime acquisitionDate, string itemCode, string responsibilityCode, int counter);
        //  Task<DataList<EamisDeliveryReceiptDTO>> DeliveryReceiptHeaderToDetailsList(EamisDeliveryReceiptDTO filter, PageConfig config);
        Task<string> GetPropertyNumberExisted (string propertyNumber);
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
    }
}
