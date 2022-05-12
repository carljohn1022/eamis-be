using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
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
    public class EamisItemCategoryController : ControllerBase
    {
        IEamisItemCategoryRepository _eamisItemCategoryRepository;
        public EamisItemCategoryController(IEamisItemCategoryRepository eamisItemCategoryRepository)
        {
            _eamisItemCategoryRepository = eamisItemCategoryRepository;
        }

        //[HttpGet("MapToDTOList")]
        //public async Task<ActionResult<EamisItemCategoryDTO>> MapToDTOList(string searchKey, [FromQuery] PageConfig config)
        //{
        //    return Ok(await _eamisItemCategoryRepository.MapToDTOList(searchKey, config));
        //}

        [HttpGet("SearchItemCategory")]
        public async Task<ActionResult<EAMISITEMCATEGORY>> SearchItemCategory(string searchValue)
        {
            return Ok(await _eamisItemCategoryRepository.SearchItemCategory(searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISITEMCATEGORY>> List([FromQuery] EamisItemCategoryDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisItemCategoryDTO();
            return Ok(await _eamisItemCategoryRepository.List(filter, config));

        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisItemCategoryDTO>> Add([FromBody] EamisItemCategoryDTO item)
        {
            if (item == null)
                item = new EamisItemCategoryDTO();
            return Ok(await _eamisItemCategoryRepository.Insert(item));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisItemCategoryDTO>> Edit([FromBody] EamisItemCategoryDTO item)
        {
            if (item == null)
                item = new EamisItemCategoryDTO();
            return Ok(await _eamisItemCategoryRepository.Update(item));
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisItemCategoryDTO>> Delete([FromBody] EamisItemCategoryDTO item)
        {
            if (item == null)
                item = new EamisItemCategoryDTO();
            return Ok(await _eamisItemCategoryRepository.Delete(item));
        }
    }
}
