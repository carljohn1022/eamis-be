using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisPropertyLedgerController : ControllerBase
    {
        private readonly IEamisPropertyLedgerRepository _eamisPropertyLedger;
        public EamisPropertyLedgerController(IEamisPropertyLedgerRepository eamisPropertyLedger)
        {
            _eamisPropertyLedger = eamisPropertyLedger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYLEDGER>> List([FromQuery] EamisPropertyLedgerDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyLedgerDTO();
            return Ok(await _eamisPropertyLedger.List(filter, config));
        }

        [HttpGet("listforcreation")]
        public async Task<ActionResult<EAMISPROPERTYLEDGER>> ListForCreation([FromQuery] EamisPropertyLedgerDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyLedgerDTO();
            return Ok(await _eamisPropertyLedger.ListForCreation(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyLedgerDTO>> Add([FromBody] EamisPropertyLedgerDTO item)
        {
            if (item == null)
                item = new EamisPropertyLedgerDTO();
            var result = await _eamisPropertyLedger.Insert(item);
            if (_eamisPropertyLedger.HasError)
                return BadRequest(_eamisPropertyLedger.ErrorMessage);
            return Ok(result);
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyLedgerDTO>> Edit([FromBody] EamisPropertyLedgerDTO item)
        {
            if (item == null)
                item = new EamisPropertyLedgerDTO();
            var result = await _eamisPropertyLedger.Update(item);
            if (_eamisPropertyLedger.HasError)
                return BadRequest(_eamisPropertyLedger.ErrorMessage);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisPropertyLedgerDTO>> Delete([FromBody] EamisPropertyLedgerDTO item)
        {
            if (item == null)
                item = new EamisPropertyLedgerDTO();
            var result = await _eamisPropertyLedger.Delete(item);
            if (_eamisPropertyLedger.HasError)
                return BadRequest(_eamisPropertyLedger.ErrorMessage);
            return Ok(result);
        }
    }
}