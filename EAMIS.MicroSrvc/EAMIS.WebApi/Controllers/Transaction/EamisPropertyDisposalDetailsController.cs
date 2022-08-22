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
    public class EamisPropertyDisposalDetailsController : ControllerBase
    {
        IEamisPropertyDisposalDetailsRepository _eamisPropertyDisposalDetailsRepository;
        public EamisPropertyDisposalDetailsController(IEamisPropertyDisposalDetailsRepository eamisPropertyDisposalDetailsRepository)
        {
            _eamisPropertyDisposalDetailsRepository = eamisPropertyDisposalDetailsRepository;
        }


        /// <summary>
        /// this method returns existing/created property items with transaction type equal to Property Disposal
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> List([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyDisposalDetailsRepository.List(filter, config));
        }

        /// <summary>
        /// this method returns list property items with service log type equal to For Condemn
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("listpropertyitemsfordisposal")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> ListDetailsForDisposal([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyDisposalDetailsRepository.ListForDisposal(filter, config));
        }

        /// <summary>
        /// after 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Add([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();

            var result = await _eamisPropertyDisposalDetailsRepository.Insert(item);

            if (_eamisPropertyDisposalDetailsRepository.HasError)
                return BadRequest(_eamisPropertyDisposalDetailsRepository.ErrorMessage);

            return Ok(result);
        }

        [HttpPost("update")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Update([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();

            var result = await _eamisPropertyDisposalDetailsRepository.Update(item);

            if (_eamisPropertyDisposalDetailsRepository.HasError)
                return BadRequest(_eamisPropertyDisposalDetailsRepository.ErrorMessage);

            return Ok(result);
        }
    }
}