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
    public class EamisPropertyDetailsController : ControllerBase
    {
        private readonly IEamisPropertyDetailsRepository _eamisPropertyDetailsRepository;
        public EamisPropertyDetailsController(IEamisPropertyDetailsRepository eamisPropertyDetailsRepository)
        {
            _eamisPropertyDetailsRepository = eamisPropertyDetailsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYDETAILS>> List([FromQuery] EamisPropertyDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyDetailsDTO();
            return Ok(await _eamisPropertyDetailsRepository.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyDetailsDTO>> Add([FromBody] EamisPropertyDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyDetailsDTO();
            return Ok(await _eamisPropertyDetailsRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyDetailsDTO>> Edit([FromBody] EamisPropertyDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyDetailsDTO();
            return Ok(await _eamisPropertyDetailsRepository.Update(item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisPropertyDetailsDTO>> Delete([FromBody] EamisPropertyDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyDetailsDTO();
            return Ok(await _eamisPropertyDetailsRepository.Delete(item));
        }

    }
}
