using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Domain.Entities;
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
        Task<bool> UpdateValidateExistingCode(string ResponsibilityCenter, int id);
        Task<EamisResponsibilityCenterDTO> InsertFromExcel(EamisResponsibilityCenterDTO item);
        Task<List<EAMISRESPONSIBILITYCENTER>> GetAllResponsibilityCenters();

        Task<bool> InsertFromExcel(List<EamisResponsibilityCenterDTO> Items);
        string ErrorMessage { get; set; }

        bool HasError { get; set; }
    }
}
