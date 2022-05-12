using EAMIS.Common.DTO.Ais;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Ais
{
    public interface IAisPersonnelRepository
    {
        Task<DataList<AisPersonnelDTO>> List(AisPersonnelDTO filter, PageConfig config);
        Task<AisPersonnelDTO> GetPersonnelByAgencyEmployeeId(string AgencyEmployeeNumber);
    }
}
