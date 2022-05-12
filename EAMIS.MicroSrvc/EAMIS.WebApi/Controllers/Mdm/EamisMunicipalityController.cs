using EAMIS.Common.DTO;
using EAMIS.Core.ContractRepository;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisMunicipalityController : ControllerBase
    {
        private IEamisMunicipalityRepository _eamisMunicipalityRepository;
        public EamisMunicipalityController(IEamisMunicipalityRepository eamisMunicipalityRepository)
        {
            _eamisMunicipalityRepository = eamisMunicipalityRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISMUNICIPALITY>> List([FromQuery] EamisMunicipalityDTO filter,[FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisMunicipalityDTO();
            if (config == null)
                config = new PageConfig();
            return Ok(await _eamisMunicipalityRepository.List(filter,config));
        }
    }
}
