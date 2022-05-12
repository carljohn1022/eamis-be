using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisAuthorizationRepository
    {
        Task<DataList<EamisAuthorizationDTO>> List(EamisAuthorizationDTO filter, PageConfig config);
    }
}
