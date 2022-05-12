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
    public class AisOfficeController : ControllerBase
    {
        IAisOfficeRepository _aisOfficeRepository;

        public AisOfficeController(IAisOfficeRepository aisOfficeRepository)
        {
            _aisOfficeRepository = aisOfficeRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<AisOfficeDTO>> List([FromQuery]AisOfficeDTO item,[FromQuery]PageConfig config)
        {
            if (item == null)
                item = new AisOfficeDTO();
            if (config == null)
                config = new PageConfig();
            return Ok(await _aisOfficeRepository.List(item, config));


        }
    }
}
