using EAMIS.Common.DTO.Branch_Maintenance;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Branch_Maintenance
{
    public interface IEamisBranchMaintenanceRepository
    {
        Task<DataList<EamisBranchDTO>> List(EamisBranchDTO filter, PageConfig config);
        Task<EamisBranchDTO> Insert(EamisBranchDTO item);
        Task<EamisBranchDTO> Update(int seqID, EamisBranchDTO item);
        Task<EamisBranchDTO> Delete(EamisBranchDTO item);
        Task<bool> ValidateExistingBranchID(string branchID);
        Task<bool> ValidateExistingBranchUpdate(int seqID, string branchID);
    }
}
