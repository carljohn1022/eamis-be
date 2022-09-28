using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Transaction;
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
    public class EamisPropertyDisposalController : ControllerBase
    {
        IEamisPropertyDisposalRepository _eamisPropertyDisposalRepository;

        public EamisPropertyDisposalController(IEamisPropertyDisposalRepository eamisPropertyDisposalRepository)
        {
            _eamisPropertyDisposalRepository = eamisPropertyDisposalRepository;
        }

        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync()
        {
            var nextId = await _eamisPropertyDisposalRepository.GetNextSequenceNumber();
            return nextId;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> List([FromQuery] EamisPropertyTransactionDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyDisposalRepository.List(filter, config));
        }


        /// <summary>
        /// after 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Add([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();

            var result = await _eamisPropertyDisposalRepository.Insert(item);

            if (_eamisPropertyDisposalRepository.HasError)
                return BadRequest(_eamisPropertyDisposalRepository.ErrorMessage);

            return Ok(result);
        }

        [HttpPost("update")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Update([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();

            var result = await _eamisPropertyDisposalRepository.Update(item);

            if (_eamisPropertyDisposalRepository.HasError)
                return BadRequest(_eamisPropertyDisposalRepository.ErrorMessage);

            return Ok(result);
        }
        [HttpGet("editbyid")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> getPropertyItemById(int itemID)
        {
            return Ok(await _eamisPropertyDisposalRepository.getPropertyItemById(itemID));
        }
    }
}