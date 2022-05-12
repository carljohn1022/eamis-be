using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
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
    public class EamisAuthorizationController : ControllerBase
    {
        IEamisAuthorizationRepository _eamisAuthorizationRepository;
        public EamisAuthorizationController(IEamisAuthorizationRepository eamisAuthorizationRepository)
        {
            _eamisAuthorizationRepository = eamisAuthorizationRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisAuthorizationDTO>> List([FromQuery] EamisAuthorizationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisAuthorizationDTO();
            return Ok(await _eamisAuthorizationRepository.List(filter, config));
        }
    }
}
