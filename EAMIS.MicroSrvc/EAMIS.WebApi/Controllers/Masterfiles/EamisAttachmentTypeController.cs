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
    public class EamisAttachmentTypeController : ControllerBase
    {
        IEamisAttachmentTypeRepository _eamisAttachmentTypeRepository;
        public EamisAttachmentTypeController(IEamisAttachmentTypeRepository eamisAttachmentTypeRepository)
        {
            _eamisAttachmentTypeRepository = eamisAttachmentTypeRepository;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisAttachmentTypeDTO>> List([FromQuery] EamisAttachmentTypeDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisAttachmentTypeDTO();
            return Ok(await _eamisAttachmentTypeRepository.List(filter, config));
        }
        [HttpPost("Add")]
        public async Task<ActionResult<EamisAttachmentTypeDTO>> Add([FromBody] EamisAttachmentTypeDTO item)
        {
            if (item == null)
                item = new EamisAttachmentTypeDTO();
            return Ok(await _eamisAttachmentTypeRepository.Insert(item));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisAttachmentTypeDTO>> Edit([FromBody] EamisAttachmentTypeDTO item)
        {
            if (item == null)
                item = new EamisAttachmentTypeDTO();
            return Ok(await _eamisAttachmentTypeRepository.Update(item));
        }
        [HttpPost("Delete")]
        public async Task<ActionResult<EamisAttachmentTypeDTO>> Delete([FromBody] EamisAttachmentTypeDTO item)
        {
            if (item == null)
                item = new EamisAttachmentTypeDTO();
            return Ok(await _eamisAttachmentTypeRepository.Delete(item));
        }
    }
}
