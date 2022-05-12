using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisUserRolesRepository
    {
        Task<DataList<EamisUserRolesDTO>> List(EamisUserRolesDTO filter);
        Task<EamisUserRolesDTO> Insert(EamisUserRolesDTO item);
        Task<EamisUserRolesDTO> Update(EamisUserRolesDTO item);
        Task<EamisUserRolesDTO> Delete(EamisUserRolesDTO item);
    }
}
