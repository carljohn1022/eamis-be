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
    public class EamisPropertyTransactionDetailsController : ControllerBase
    {
        IEamisPropertyTransactionDetailsRepository _eamisPropertyTransactionDetailsRepository;
        public EamisPropertyTransactionDetailsController(IEamisPropertyTransactionDetailsRepository eamisPropertyTransactionDetailsRepository)
        {
            _eamisPropertyTransactionDetailsRepository = eamisPropertyTransactionDetailsRepository;

        }
        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> List([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyTransactionDetailsRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Add([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyTransactionDetailsRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Edit([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyTransactionDetailsRepository.Update(item));
        }
        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Delete([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyTransactionDetailsRepository.Delete(item));
        }
    }
}
