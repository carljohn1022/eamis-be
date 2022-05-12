using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisRolesRepository
    {
        Task<DataList<EamisRolesDTO>> List(EamisRolesDTO filter, PageConfig config);
        Task<EamisRolesDTO> Insert(EamisRolesDTO item);
        Task<EamisRolesDTO> Update(EamisRolesDTO item);
        Task<EamisRolesDTO> Delete(EamisRolesDTO item);
    }
}
