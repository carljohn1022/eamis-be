using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisEnvironmentalAspectsController : ControllerBase
    {
        IEamisEnvironmentalAspectsRepository _eamisEnvironmentalAspectsRepository;
        
        public EamisEnvironmentalAspectsController(IEamisEnvironmentalAspectsRepository eamisEnvironmentalAspectsRepository)
        {
            _eamisEnvironmentalAspectsRepository = eamisEnvironmentalAspectsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISSIGNIFICANTENVIRONMENTALASPECTS>> List([FromQuery] EamisEnvironmentalAspectsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisEnvironmentalAspectsDTO();
            return Ok(await _eamisEnvironmentalAspectsRepository.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisEnvironmentalAspectsDTO>> Add([FromBody] EamisEnvironmentalAspectsDTO item)
        {
            if (item == null)
                item = new EamisEnvironmentalAspectsDTO();
            return Ok(await _eamisEnvironmentalAspectsRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisEnvironmentalAspectsDTO>> Edit([FromBody] EamisEnvironmentalAspectsDTO item)
        {
            if (item == null)
                item = new EamisEnvironmentalAspectsDTO();
            return Ok(await _eamisEnvironmentalAspectsRepository.Update(item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisEnvironmentalAspectsDTO>> Delete([FromBody] EamisEnvironmentalAspectsDTO item)
        {
            if (item == null)
                item = new EamisEnvironmentalAspectsDTO();
            return Ok(await _eamisEnvironmentalAspectsRepository.Delete(item));
        }
    }
}
