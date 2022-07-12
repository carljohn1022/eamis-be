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
    public interface IEamisFinancingSourceRepository
    {
        Task<DataList<EamisFinancingSourceDTO>> List(EamisFinancingSourceDTO filter, PageConfig config);
        Task<List<EAMISFINANCINGSOURCE>> ListFinancingSource(string searchValue);
    }
}
