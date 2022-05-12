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
    public class EamisItemSubCategoryController : ControllerBase
    {
        IEamisItemSubCategoryRepository _eamisItemSubCategoryRepository;
        public EamisItemSubCategoryController(IEamisItemSubCategoryRepository eamisItemSubCategoryRepository)
        {
            _eamisItemSubCategoryRepository = eamisItemSubCategoryRepository;
        }

        [HttpGet("SearchItemSubCategory")]
        public async Task<ActionResult<EAMISITEMSUBCATEGORY>> SearchItemSubCategory(string type, string searchValue)
        {
            return Ok(await _eamisItemSubCategoryRepository.SearchItemSubCategory(type, searchValue));
        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISITEMSUBCATEGORY>> List([FromQuery] EamisItemSubCategoryDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisItemSubCategoryDTO();
            return Ok(await _eamisItemSubCategoryRepository.List(filter, config));

        }

        [HttpPost("Add")]
        public async Task<ActionResult<EamisItemSubCategoryDTO>> Add([FromBody] EamisItemSubCategoryDTO item)
        {
            if (await _eamisItemSubCategoryRepository.Validation(item.CategoryId, item.SubCategoryName))
            {
                //
                return Unauthorized();
            }
            else if (await _eamisItemSubCategoryRepository.ValidateExistingSub(item.CategoryId))
            {
                return Ok(await _eamisItemSubCategoryRepository.Insert(item));
            }
            else if(await _eamisItemSubCategoryRepository.ValidateExistingCategoryId(item.CategoryId))
            {
                return Ok(await _eamisItemSubCategoryRepository.Insert(item));
            }
            if (item == null)
                item = new EamisItemSubCategoryDTO();
            return Ok(await _eamisItemSubCategoryRepository.Insert(item));


        }

        [HttpPut("Edit")]
        public async Task<ActionResult<EamisItemSubCategoryDTO>> Edit([FromBody] EamisItemSubCategoryDTO item)
        {
            if (await _eamisItemSubCategoryRepository.ValidateExistingSubUpdate(item.SubCategoryName, item.CategoryId))
            {
                if (item == null)
                    item = new EamisItemSubCategoryDTO();
                return Ok(await _eamisItemSubCategoryRepository.Update(item));
            }
            else if(await _eamisItemSubCategoryRepository.ValidateExistingSub(item.CategoryId))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisItemSubCategoryRepository.Update(item));
            }
            
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<EamisItemSubCategoryDTO>> Delete([FromBody] EamisItemSubCategoryDTO item)
        {
            if (item == null)
                item = new EamisItemSubCategoryDTO();
            return Ok(await _eamisItemSubCategoryRepository.Delete(item));
        }
    }
}
