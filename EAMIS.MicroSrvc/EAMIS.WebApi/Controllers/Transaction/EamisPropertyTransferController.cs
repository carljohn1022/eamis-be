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
    public class EamisPropertyTransferController : ControllerBase
    {
        IEamisPropertyTransferRepository _eamisPropertyTransferRepository;
        IEamisPropertyTransactionRepository _eamisPropertyTransactionRepository;
        public EamisPropertyTransferController(IEamisPropertyTransferRepository eamisPropertyTransferRepository, IEamisPropertyTransactionRepository eamisPropertyTransactionRepository)
        {
            _eamisPropertyTransferRepository = eamisPropertyTransferRepository;
            _eamisPropertyTransactionRepository = eamisPropertyTransactionRepository;
        }

        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync()
        {
            var nextId = await _eamisPropertyTransferRepository.GetNextSequenceNumber();
            return nextId;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> List([FromQuery] EamisPropertyTransactionDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransferRepository.List(filter, config));
        }


        /// <summary>
        /// this will create a new record/row in [EAMIS_PROPERTY_TRANSACTION]
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Add([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransferRepository.Insert(item));
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> Search(string type, string searchValue)
        {
            return Ok(await _eamisPropertyTransactionRepository.SearchReceivingforTransfer(type, searchValue));
        }
        [HttpGet("editbyid")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> getPropertyItemById(int itemID)
        {
            return Ok(await _eamisPropertyTransferRepository.getPropertyItemById(itemID));
        }
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Edit([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyTransferRepository.Update(item));
        }
    }
}
