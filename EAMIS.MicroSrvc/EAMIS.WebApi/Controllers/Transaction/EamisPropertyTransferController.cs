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
        public EamisPropertyTransferController(IEamisPropertyTransferRepository eamisPropertyTransferRepository)
        {
            _eamisPropertyTransferRepository = eamisPropertyTransferRepository;
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

        [HttpGet("GetPropertyTransferDetailsById")]
        public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> GetPropertyTransferDetailsById([FromQuery] EamisPropertyTransferDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransferDetailsDTO();
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

        /// <summary>
        /// this will create a new record/row in [EAMIS_PROPERTY_TRANSACTION_DETAILS]
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("AddPropertyTransferDetails")]
        public async Task<ActionResult<EamisPropertyTransferDetailsDTO>> AddPropertyTransferDetails([FromBody] EamisPropertyTransferDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransferDetailsDTO();
            return Ok(await _eamisPropertyTransferRepository.Insert(item));
        }
    }
}
