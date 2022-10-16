using EAMIS.Common.DTO.LookUp;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Report_Catalog;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Report_Catalog;
using EAMIS.Core.Domain.Entities;
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

        [HttpGet("list")]
        public async Task<ActionResult<EAMISREPORTCATALOG>> List([FromQuery] EamisReportCatalogDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisReportCatalogDTO();
            return Ok(await _eamisReportCatalogRepository.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisReportCatalogDTO>> Add([FromBody] EamisReportCatalogDTO item)
        {
            if (item == null)
                item = new EamisReportCatalogDTO();
            var result = await _eamisReportCatalogRepository.Insert(item);
            if (_eamisReportCatalogRepository.HasError)
                return BadRequest(_eamisReportCatalogRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisReportCatalogDTO>> Edit([FromBody] EamisReportCatalogDTO item)
        {
            if (item == null)
                item = new EamisReportCatalogDTO();
            var result = await _eamisReportCatalogRepository.Update(item);
            if (_eamisReportCatalogRepository.HasError)
                return BadRequest(_eamisReportCatalogRepository.ErrorMessage);
            return Ok(result);
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisReportCatalogDTO>> Delete([FromBody] EamisReportCatalogDTO item)
        {
            if (item == null)
                item = new EamisReportCatalogDTO();
            var result = await _eamisReportCatalogRepository.Delete(item);
            if (_eamisReportCatalogRepository.HasError)
                return BadRequest(_eamisReportCatalogRepository.ErrorMessage);
            return Ok(result);
        }

    }
}
