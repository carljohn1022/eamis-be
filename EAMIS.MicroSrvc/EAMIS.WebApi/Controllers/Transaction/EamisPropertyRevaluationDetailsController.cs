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
    public class EamisPropertyRevaluationDetailsController : ControllerBase
    {
        IEamisPropertyRevalutionDetailsRepository _eamisPropertyRevalutionDetailsRepository;
        public EamisPropertyRevaluationDetailsController(IEamisPropertyRevalutionDetailsRepository eamisPropertyRevalutionDetailsRepository)
        {
            _eamisPropertyRevalutionDetailsRepository = eamisPropertyRevalutionDetailsRepository;
        }


        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYREVALUATIONDETAILS>> List([FromQuery] EamisPropertyRevaluationDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyRevaluationDetailsDTO();
            return Ok(await _eamisPropertyRevalutionDetailsRepository.List(filter, config));
        }


        /// <summary>
        /// after creation of the revaluation header, get the generated revaluation id and pass it on to revaluation details(property revaluation id)
        /// then call/trigger the insert method of revaluation details
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyRevaluationDetailsDTO>> Add([FromBody] EamisPropertyRevaluationDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyRevaluationDetailsDTO();

            var result = await _eamisPropertyRevalutionDetailsRepository.Insert(item);

            if (_eamisPropertyRevalutionDetailsRepository.HasError)
                return BadRequest(_eamisPropertyRevalutionDetailsRepository.ErrorMessage);

            return Ok(result);
        }

        [HttpPost("Edit")]
        public async Task<ActionResult<EamisPropertyRevaluationDetailsDTO>> Edit([FromBody] EamisPropertyRevaluationDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyRevaluationDetailsDTO();

            var result = await _eamisPropertyRevalutionDetailsRepository.Update(item);

            if (_eamisPropertyRevalutionDetailsRepository.HasError)
                return BadRequest(_eamisPropertyRevalutionDetailsRepository.ErrorMessage);

            return Ok(result);
        }
    }
}
