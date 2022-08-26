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
    public class EamisPropertyDepreciationController : ControllerBase
    {
        private readonly IEamisPropertyDepreciationRepository _depreciationRepository;
        public EamisPropertyDepreciationController(IEamisPropertyDepreciationRepository depreciationRepository)
        {
            _depreciationRepository = depreciationRepository;
        }

        /// <summary>
        /// this method should be called when listing created depreciaion only
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYDEPRECIATION>> List([FromQuery] EamisPropertyDepreciationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyDepreciationDTO();
            var result = await _depreciationRepository.ListForDepreciationCreation(filter, config);
            if (_depreciationRepository.HasError)
                return BadRequest(_depreciationRepository.ErrorMessage);
            return Ok(result);
        }


        /// <summary>
        /// this method should be called when creating new depreciation
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpGet("listpropertyfordepreciation")]
        public async Task<ActionResult<EAMISPROPERTYDEPRECIATION>> ListPropertyForDepreciationCreation([FromQuery] EamisPropertyDepreciationDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyDepreciationDTO();
            var result = await _depreciationRepository.ListForDepreciationCreation(filter, config);
            if (_depreciationRepository.HasError)
                return BadRequest(_depreciationRepository.ErrorMessage);
            return Ok(result);
        }

        /// <summary>
        /// this will add/insert/create new depreciation
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyDepreciationDTO>> Add([FromBody] EamisPropertyDepreciationDTO item)
        {
            if (item == null)
                item = new EamisPropertyDepreciationDTO();
            var result = await _depreciationRepository.Insert(item);
            if (_depreciationRepository.HasError)
                return BadRequest(_depreciationRepository.ErrorMessage);
            return Ok(result);
        }
    }
}
