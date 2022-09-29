using EAMIS.Common.DTO.Masterfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.ContractRepository.Report_Catalog
{
    public interface IEamisReportCatalogRepository
    {
        Task<List<EamisFundSourceDTO>> GetFundSourceList();
    }
}
