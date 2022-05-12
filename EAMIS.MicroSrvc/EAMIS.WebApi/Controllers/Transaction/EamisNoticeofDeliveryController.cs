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
    public class EamisNoticeofDeliveryController : ControllerBase
    {
        private readonly IEamisNoticeofDeliveryRepository _eamisNoticeofDeliveryRepository;
        public EamisNoticeofDeliveryController(IEamisNoticeofDeliveryRepository eamisNoticeofDeliveryRepository)
        {
            _eamisNoticeofDeliveryRepository = eamisNoticeofDeliveryRepository;
        }
        [HttpGet("list")]
        public async Task<ActionResult<EAMISNOTICEOFDELIVERY>> List([FromQuery] EamisNoticeofDeliveryDTO filter,[FromQuery]PageConfig config)
        {
            if (filter == null)
                filter = new EamisNoticeofDeliveryDTO();
            return Ok(await _eamisNoticeofDeliveryRepository.List(filter,config));
        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisNoticeofDeliveryDTO>> Add([FromBody] EamisNoticeofDeliveryDTO item)
        {
            if (item == null)
                item = new EamisNoticeofDeliveryDTO();
            return Ok(await _eamisNoticeofDeliveryRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisNoticeofDeliveryDTO>> Edit([FromBody] EamisNoticeofDeliveryDTO item)
        {
            if (item == null)
                item = new EamisNoticeofDeliveryDTO();
            return Ok(await _eamisNoticeofDeliveryRepository.Update(item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisNoticeofDeliveryDTO>> Delete([FromBody] EamisNoticeofDeliveryDTO item)
        {
            if (item == null)
                item = new EamisNoticeofDeliveryDTO();
            return Ok(await _eamisNoticeofDeliveryRepository.Delete(item));
        }
    }
}
