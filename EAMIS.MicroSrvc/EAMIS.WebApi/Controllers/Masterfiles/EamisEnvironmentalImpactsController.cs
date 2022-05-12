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
    public class EamisEnvironmentalImpactsController : ControllerBase
    {
        IEamisEnvironmentalImpactsRepository _eamisEnvironmentalImpactsRepository;

        public EamisEnvironmentalImpactsController(IEamisEnvironmentalImpactsRepository eamisEnvironmentalImpactsRepository)
        {
            _eamisEnvironmentalImpactsRepository = eamisEnvironmentalImpactsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISENVIRONMENTALIMPACTS>> List([FromQuery] EamisEnvironmentalImpactsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisEnvironmentalImpactsDTO();
            return Ok(await _eamisEnvironmentalImpactsRepository.List(filter, config)); 
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisEnvironmentalImpactsDTO>> Add([FromBody] EamisEnvironmentalImpactsDTO item)
        {
            if (item == null)
                item = new EamisEnvironmentalImpactsDTO();
            return Ok(await _eamisEnvironmentalImpactsRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisEnvironmentalImpactsDTO>> Update([FromBody] EamisEnvironmentalImpactsDTO item)
        {
            if (item == null)
                item = new EamisEnvironmentalImpactsDTO();
            return Ok(await _eamisEnvironmentalImpactsRepository.Update(item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisEnvironmentalImpactsDTO>> Delete([FromBody] EamisEnvironmentalImpactsDTO item)
        {
            if (item == null)
                item = new EamisEnvironmentalImpactsDTO();
            return Ok(await _eamisEnvironmentalImpactsRepository.Delete(item));
        }
    }
}
