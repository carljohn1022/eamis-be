using EAMIS.Common.DTO.Ais;
using EAMIS.Core.ContractRepository.Ais;
using EAMIS.Core.Domain.Entities.AIS;
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
    public class AisPersonnelController : ControllerBase
    {
        private readonly IAisPersonnelRepository _aisPersonnelRepository;
        public AisPersonnelController(IAisPersonnelRepository aisPersonnelRepository)
        {
            _aisPersonnelRepository = aisPersonnelRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<AISPERSONNEL>> List([FromQuery] AisPersonnelDTO filter, [FromQuery] PageConfig config)
        {
            if (config == null)
                config = new PageConfig();
            if (filter == null)
                filter = new AisPersonnelDTO();
            return Ok(await _aisPersonnelRepository.List(filter, config));
        }
        [HttpGet("GetByEmployeeId")]
        public async Task<ActionResult<AisPersonnelDTO>> GetByEmployeeId(string AgencyEmployeeNumber)
        {
            if (AgencyEmployeeNumber == null)
                return BadRequest("EmployeeId must have a value.");
            return Ok(await _aisPersonnelRepository.GetPersonnelByAgencyEmployeeId(AgencyEmployeeNumber));

        }
    }
}
