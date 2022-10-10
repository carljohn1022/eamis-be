using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Report_Catalog;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
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


        public async Task<List<LookupDTO>> FundSourceList()
        {
            var result = _ctx.EAMIS_PROPERTY_TRANSACTION
                                          .Where(f => !(f.FUND_SOURCE == null || f.FUND_SOURCE == string.Empty))
                                          .GroupBy(x => x.FUND_SOURCE)
                                          .Select(i => new LookupDTO
                                          { 
                                              LookUpValue = i.Key })
                                          .ToList();

            return result;
        }

    }
}
