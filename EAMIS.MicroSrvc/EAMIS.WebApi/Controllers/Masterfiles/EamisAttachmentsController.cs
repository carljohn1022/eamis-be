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
    public class EamisAttachmentsController : ControllerBase
    {
        IEamisAttachmentsRepository _eamisAttachmentsRepository;
        public EamisAttachmentsController(IEamisAttachmentsRepository eamisAttachmentsRepository)
        {
            _eamisAttachmentsRepository = eamisAttachmentsRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisAttachmentsDTO>> List([FromQuery] EamisAttachmentsDTO filter, [FromQuery]PageConfig config)
        {
            if (filter == null)
                filter = new EamisAttachmentsDTO();
            return Ok(await _eamisAttachmentsRepository.List(filter,config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisAttachmentsDTO>> Add([FromBody] EamisAttachmentsDTO item)
        {
            if (item == null)
                item = new EamisAttachmentsDTO();
            return Ok(await _eamisAttachmentsRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisAttachmentsDTO>> Edit([FromBody] EamisAttachmentsDTO item)
        {
            if (item == null)
                item = new EamisAttachmentsDTO();
            return Ok(await _eamisAttachmentsRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisAttachmentsDTO>> Delete([FromBody] EamisAttachmentsDTO item)
        {
            if (item == null)
                item = new EamisAttachmentsDTO();
            return Ok(await _eamisAttachmentsRepository.Delete(item));
        }
    }
}
