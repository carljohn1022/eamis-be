using EAMIS.Common.DTO;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository
{
    public interface IEamisRegionRepository
    {
        Task<DataList<EamisRegionDTO>> List(EamisRegionDTO filter,PageConfig config);
        Task<List<EAMISREGION>> ListRegion(string searchValue);
    }
}
