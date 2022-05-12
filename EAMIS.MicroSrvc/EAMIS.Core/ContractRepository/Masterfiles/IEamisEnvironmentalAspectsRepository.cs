using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Masterfiles
{
    public interface IEamisEnvironmentalAspectsRepository
    {
        Task<DataList<EamisEnvironmentalAspectsDTO>> List(EamisEnvironmentalAspectsDTO filter, PageConfig config);
        Task<EamisEnvironmentalAspectsDTO> Insert(EamisEnvironmentalAspectsDTO item);
        Task<EamisEnvironmentalAspectsDTO> Update(EamisEnvironmentalAspectsDTO item);
        Task<EamisEnvironmentalAspectsDTO> Delete(EamisEnvironmentalAspectsDTO item);
    }
}
