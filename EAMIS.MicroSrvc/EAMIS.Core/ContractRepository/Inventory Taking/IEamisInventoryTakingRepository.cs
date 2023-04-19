using EAMIS.Common.DTO.Inventory_Taking;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Inventory_Taking
{
    public interface IEamisInventoryTakingRepository
    {
        Task<EamisInventoryTakingDTO> getScannedPropertyNumberList(string propertyNumber, string office, string department, string userStamp);
        Task<DataList<EamisInventoryTakingDTO>> List(EamisInventoryTakingDTO filter, PageConfig config);
        Task<bool> CheckPropertyNumberExist(string propertyNumber);
        Task<bool> CheckPropertyNumberExistIfScanned(string propertyNumber);
        Task<List<EamisResponsibilityCenterDTO>> GetUnitDesc(bool isActive, string officeDesc);
        Task<List<EamisResponsibilityCenterDTO>> GetOfficeDesc(bool isActive);
        Task<List<EamisPropertyTransactionDetailsDTO>> GetPropertyNumberAccountability(string assigneeCustodianName);
        //Task<DataList<EamisPropertyTransactionDetailsDTO>> ListItemsForReceiving(EamisPropertyTransactionDetailsDTO filter, PageConfig config, string agencyEmployeeNumber); //try
    }
}
