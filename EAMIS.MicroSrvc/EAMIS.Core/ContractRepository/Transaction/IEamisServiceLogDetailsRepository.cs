using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Transaction
{
    public interface IEamisServiceLogDetailsRepository
    {
        Task<DataList<EamisServiceLogDetailsDTO>> ListServiceLogDetails(EamisServiceLogDetailsDTO filter, PageConfig config);
        Task<DataList<EamisServiceLogDetailsCreationDTO>> ListServiceLogDetailsForCreation(EamisPropertyTransactionDetailsDTO filter, PageConfig config);
        Task<EamisServiceLogDetailsDTO> InsertServiceLogDetails(EamisServiceLogDetailsDTO item);
        Task<EamisServiceLogDetailsDTO> UpdateServiceLogDetails(EamisServiceLogDetailsDTO item);
        int GetSupplierId(string transactionType);
        string GetSupplierCompany(int supplierId);
        Task<string> GetGroupByLand(string itemCode);

        string ErrorMessage { get; set; }
        bool HasError { get; set; }

        Task<List<EamisTranTypeDTO>> GetTranTypeList();
        Task<List<EamisAssetConditionTypeDTO>> GetAssetCondition();
        Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsIssued(EamisPropertyTransactionDetailsDTO filter, PageConfig config);
        //Task<EamisServiceLogDetailsDTO> ListServiceLogDetailsByServiceLogId(int serviceLogId);
    }
}
