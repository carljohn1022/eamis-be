using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Authorization;
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
    public class EamisPropertyItemsController : ControllerBase
    {
        IEamisPropertyItemsRepository _eamisPropertyItemsRepository;
        public EamisPropertyItemsController(IEamisPropertyItemsRepository eamisPropertyItemsRepository)
        {
            _eamisPropertyItemsRepository = eamisPropertyItemsRepository;
        }

        [HttpGet("GeneratedProperty")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> GeneratedProperty()
        {
            return Ok(await _eamisPropertyItemsRepository.GeneratedProperty());
        }

        [HttpGet("PublicSearchPropertyItems")]
        public async Task<ActionResult<EAMISPROPERTYITEMS>> PublicSearchPropertyItems(string SearchValue)
        {
            return Ok(await _eamisPropertyItemsRepository.PublicSearch(SearchValue));

        }
       
        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYITEMS>> List([FromQuery] EamisPropertyItemsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.List(filter, config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Add([FromBody] EamisPropertyItemsDTO item)
        {
            if (item == null)
                item = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Edit([FromBody] EamisPropertyItemsDTO item)
        {
            if (item == null)
                item = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.Update(item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Delete([FromBody] EamisPropertyItemsDTO item)
        {
            if (item == null)
                item = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.Delete(item));
        }
    }
}
