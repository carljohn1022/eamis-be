using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisEnvironmentalImpactsRepository
    {
        Task<DataList<EamisEnvironmentalImpactsDTO>> List(EamisEnvironmentalImpactsDTO filter, PageConfig config);
        Task<EamisEnvironmentalImpactsDTO> Insert(EamisEnvironmentalImpactsDTO item);
        Task<EamisEnvironmentalImpactsDTO> Update(EamisEnvironmentalImpactsDTO item);
        Task<EamisEnvironmentalImpactsDTO> Delete(EamisEnvironmentalImpactsDTO item);
    }
}
