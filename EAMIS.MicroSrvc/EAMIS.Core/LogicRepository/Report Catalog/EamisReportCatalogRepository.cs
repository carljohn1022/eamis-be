using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Report_Catalog;
using EAMIS.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Report_Catalog
{
    public class EamisReportCatalogRepository : IEamisReportCatalogRepository
    {


        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
       
        public EamisReportCatalogRepository(EAMISContext ctx)
        {
           
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }


        public async Task<List<EamisFundSourceDTO>> GetFundSourceList()
        {
            var arrDistinctDetailID = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f =>  !(f.FUND_SOURCE == null || f.FUND_SOURCE != string.Empty))
                                          .GroupBy(x => x.FUND_SOURCE)
                                          .Select(i => i.Max(x => x.ID))
                                          .ToList();

            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.AsNoTracking().Select(t =>
                    new EamisFundSourceDTO
                    {
                        Id = t.ID,
                        FundCategory = t.FUND_SOURCE
                    }).Where(v => arrDistinctDetailID.Contains(v.Id)).ToList()).ConfigureAwait(false);
            return result;
        }

    }
}
