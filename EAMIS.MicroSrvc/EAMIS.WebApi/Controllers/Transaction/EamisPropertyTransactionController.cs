using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
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
    public class EamisPropertyTransactionController : ControllerBase
    {
        IEamisPropertyTransactionRepository _eamisPropertyTransactionRepository;
        public EamisPropertyTransactionController(IEamisPropertyTransactionRepository eamisPropertyTransactionRepository)
        {
            _eamisPropertyTransactionRepository = eamisPropertyTransactionRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> List([FromQuery] EamisPropertyTransactionDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Add([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Edit([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.Update(item));
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Delete([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransactionRepository.Delete(item));
        }
    }
}
