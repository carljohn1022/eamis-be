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
    public class EamisPropertyScheduleController : ControllerBase
    {
        IEamisPropertyScheduleRepository _eamisPropertyScheduleRepository;
        public EamisPropertyScheduleController(IEamisPropertyScheduleRepository eamisPropertyScheduleRepository)
        {
            _eamisPropertyScheduleRepository = eamisPropertyScheduleRepository;
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISPROPERTYSCHEDULE>> Search(string type, string searchValue)
        {
            return Ok(await _eamisPropertyScheduleRepository.Search(type, searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYSCHEDULE>> List([FromQuery] EamisPropertyScheduleDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyScheduleDTO();
            var result = await _eamisPropertyScheduleRepository.List(filter, config);
            if (_eamisPropertyScheduleRepository.HasError)
                return BadRequest(_eamisPropertyScheduleRepository.ErrorMessage);
            return Ok(result);
        }
    }
}
