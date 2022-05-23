using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
   public interface IEamisResponsibilityCenterRepository
    {
        Task<DataList<EamisResponsibilityCenterDTO>> SearchResCenter(string type, string searchValue);
        Task<DataList<EamisResponsibilityCenterDTO>> List(EamisResponsibilityCenterDTO filter, PageConfig config);
        Task<EamisResponsibilityCenterDTO> Insert(EamisResponsibilityCenterDTO item);
        Task<EamisResponsibilityCenterDTO> Update(EamisResponsibilityCenterDTO item, int id);
        Task<bool> ValidateExistingCode(string ResponsibilityCenter);
        
    }
}
