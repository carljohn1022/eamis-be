using EAMIS.Common.DTO.Ais;
using EAMIS.Core.ContractRepository.Ais;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Ais
{
    [Route("api/[controller]")]
    [ApiController]
    public class AisCodeListValueController : ControllerBase
    {
        IAisCodeListValueRepository _aisCodeListValueRepository;
        public AisCodeListValueController(IAisCodeListValueRepository aisCodeListValueRepository)
        {
            _aisCodeListValueRepository = aisCodeListValueRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<AisCodeListValueDTO>> List([FromQuery] AisCodeListValueDTO item, [FromQuery] PageConfig config)
        {
            if (item == null)
                item = new AisCodeListValueDTO();
            if (config == null)
                config = new PageConfig();
            return Ok(await _aisCodeListValueRepository.List(item, config));


        }
    }
}
