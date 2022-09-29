using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Report_Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Report_Catalog
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisReportCatalogController : ControllerBase
    {
        private readonly IEamisReportCatalogRepository _eamisReportCatalogRepository;
        public EamisReportCatalogController(IEamisReportCatalogRepository eamisReportCatalogRepository)
        {
            _eamisReportCatalogRepository = eamisReportCatalogRepository;
        }

        [HttpGet("GetFundSource")]
        public async Task<ActionResult<EamisFundSourceDTO>> ListFundSource()
        {

            return Ok(await _eamisReportCatalogRepository.GetFundSourceList());
        }
    }
}
