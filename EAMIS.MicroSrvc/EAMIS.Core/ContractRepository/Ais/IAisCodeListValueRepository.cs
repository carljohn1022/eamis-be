using EAMIS.Common.DTO.Ais;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Ais
{
    public interface IAisCodeListValueRepository
    {
        Task<DataList<AisCodeListValueDTO>> List(AisCodeListValueDTO item, PageConfig config);
    }
}
