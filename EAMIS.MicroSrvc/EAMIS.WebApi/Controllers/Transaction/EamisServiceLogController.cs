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
    public class EamisServiceLogController : ControllerBase
    {
        IEamisServiceLogRepository _eamisServiceLogRepository;
        public EamisServiceLogController(IEamisServiceLogRepository eamisServiceLogRepository)
        {
            _eamisServiceLogRepository = eamisServiceLogRepository;
        }


        /// <summary>
        /// call this method to generate next possible sequence number for service log
        /// </summary>
        /// <returns></returns>
        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync()
        {
            var nextId = await _eamisServiceLogRepository.GetNextSequenceNumber();
            return nextId;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISSERVICELOG>> List([FromQuery] EamisServiceLogDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisServiceLogDTO();
            return Ok(await _eamisServiceLogRepository.ListServiceLogs(filter, config));
        }

        [HttpPost("add")]
        public async Task<ActionResult<EAMISSERVICELOG>> AddServiceLog([FromBody] EamisServiceLogDTO eamisServiceLogDTO)
        {
            var result = await _eamisServiceLogRepository.InsertServiceLog(eamisServiceLogDTO);
            if (result == null)
                return BadRequest();

            return Ok(result); //return the item with ID, needed in constructing the payload of the Service log details
        }

        [HttpPost("update")]
        public async Task<ActionResult<EAMISSERVICELOG>> UpdateServiceLog([FromBody] EamisServiceLogDTO eamisServiceLogDTO)
        {
            var result = await _eamisServiceLogRepository.UpdateServiceLog(eamisServiceLogDTO);
            if (result == null || result.Id == 0)
                return BadRequest();

            return Ok(result);
        }
        //[HttpGet("editbyid")]
        //public async Task<ActionResult<EamisDeliveryReceiptDTO>> getServiceLogbyID(int itemID)
        //{
        //    return Ok(await _eamisServiceLogRepository.getServiceLogItemById(itemID));
        //}
    }
}
