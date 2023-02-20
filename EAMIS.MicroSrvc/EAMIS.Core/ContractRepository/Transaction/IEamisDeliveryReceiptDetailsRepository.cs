using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisDeliveryReceiptDetailsRepository
    {
        Task<DataList<EamisDeliveryReceiptDetailsDTO>> List(EamisDeliveryReceiptDetailsDTO filter, PageConfig config);
        Task<EamisDeliveryReceiptDetailsDTO> Insert(EamisDeliveryReceiptDetailsDTO item);
        Task<EamisDeliveryReceiptDetailsDTO> Update(EamisDeliveryReceiptDetailsDTO item);
        Task<EamisDeliveryReceiptDetailsDTO> Delete(EamisDeliveryReceiptDetailsDTO item);
        Task<List<EAMISSERIALTRAN>> updateSerial(List<EamisSerialTranDTO> items, int deliveryReceiptDetailID, int qty);
        Task<string> GetItemById(int itemId);
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
        Task<DataList<EamisDeliveryReceiptDetailsDTO>> SearchDeliveryDetailsforReceiving(string type, string searchvalue);
        Task<EAMISSERIALTRAN> PostSerialTranByItem(EamisSerialTranDTO item);
        Task<List<EamisSerialTranDTO>> PostSerialTran(List<EamisSerialTranDTO> items);
        //Task<EamisSerialTranDTO> getSerialNumber(int deliveryReceiptDetailID);

        //Task<List<EamisTranTypeDTO>> GetTranTypeList();
        //Task<List<EamisAssetConditionTypeDTO>> GetAssetCondition();
    }
}
