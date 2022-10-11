using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Report_Catalog;
using EAMIS.Core.Response.DTO;
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
        [HttpGet("GetFundSourceList")]
        public async Task<ActionResult<LookupDTO>> FundSource()
        {

            return Ok(await _eamisReportCatalogRepository.FundSourceList());
        }

        [HttpGet("GetTransactionNumberICS")]
        public async Task<ActionResult<LookupDTO>> TranNumberICS()
        {

            return Ok(await _eamisReportCatalogRepository.TransactionNumberICS());
        }

        [HttpGet("GetTransactionNumberPAR")]
        public async Task<ActionResult<LookupDTO>> TranNumberPAR()
        {

            return Ok(await _eamisReportCatalogRepository.TransactionNumberPAR());
        }
        [HttpGet("GetTransactionNumberForIssuance")]
        public async Task<ActionResult<LookupDTO>> TranNumIssuance()
        {

            return Ok(await _eamisReportCatalogRepository.TransactionNumberIssuance());
        }
        [HttpGet("GetPropertyNumber")]
        public async Task<ActionResult<LookupDTO>> PropertyNumberList()
        {

            return Ok(await _eamisReportCatalogRepository.PropertyNumberList());
        }
        [HttpGet("ItemCodeList")]
        public async Task<ActionResult<LookupDTO>> ItemCodeList()
        {

            return Ok(await _eamisReportCatalogRepository.ItemCodeList());
        }
        [HttpGet("OfficeList")]
        public async Task<ActionResult<LookupDTO>> OfficeList()
        {

            return Ok(await _eamisReportCatalogRepository.OfficeList());
        }

    }
}
