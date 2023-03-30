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
    public class EamisPropertyRevaluationController : ControllerBase
    {
        IEamisPropertyRevalutionRepository _eamisPropertyRevalutionRepository;
        public EamisPropertyRevaluationController(IEamisPropertyRevalutionRepository eamisPropertyRevalutionRepository)
        {
            _eamisPropertyRevalutionRepository = eamisPropertyRevalutionRepository;
        }

        /// <summary>
        /// call this method to generate next possible sequence number for service log
        /// </summary>
        /// <returns></returns>
        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync(string branchID)
        {
            var nextId = await _eamisPropertyRevalutionRepository.GetNextSequenceNumber(branchID);
            return nextId;
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYREVALUATION>> List([FromQuery] EamisPropertyRevaluationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyRevaluationDTO();
            return Ok(await _eamisPropertyRevalutionRepository.List(filter, config));
        }


        /// <summary>
        /// after creation of the revaluation header, get the generated revaluation id and pass it on to revaluation details(property revaluation id)
        /// then call/trigger the insert method of revaluation details
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyRevaluationDTO>> Add([FromBody] EamisPropertyRevaluationDTO item)
        {
            if (item == null)
                item = new EamisPropertyRevaluationDTO();

            var result = await _eamisPropertyRevalutionRepository.Insert(item);

            if (_eamisPropertyRevalutionRepository.HasError)
                return BadRequest(_eamisPropertyRevalutionRepository.ErrorMessage);

            return Ok(result);
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyRevaluationDTO>> Edit([FromBody] EamisPropertyRevaluationDTO item)
        {
            if (item == null)
                item = new EamisPropertyRevaluationDTO();

            var result = await _eamisPropertyRevalutionRepository.Update(item);

            if (_eamisPropertyRevalutionRepository.HasError)
                return BadRequest(_eamisPropertyRevalutionRepository.ErrorMessage);

            return Ok(result);
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISPROPERTYREVALUATION>> Search(string type, string searchValue)
        {
            return Ok(await _eamisPropertyRevalutionRepository.SearchPropertyRevaluation(type, searchValue));
        }

        [HttpGet("editbyid")]
        public async Task<ActionResult<EamisPropertyRevaluationDTO>> getAssetItemById(int itemID)
        {
            return Ok(await _eamisPropertyRevalutionRepository.getAssetItemById(itemID));
        }

    }
}
