using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EAMIS.WebApi.Controllers.Masterfiles
{
    [Route("api/[controller]")]
    [ApiController]
    public class EamisPropertyItemsController : ControllerBase
    {
        IEamisPropertyItemsRepository _eamisPropertyItemsRepository;
        IEamisItemCategoryRepository _eamisItemCategoryRepository;
        private string filePath;
        private string targetPath;
        public readonly IWebHostEnvironment _hostingEnvironment;
        public EamisPropertyItemsController(IEamisPropertyItemsRepository eamisPropertyItemsRepository,
            IEamisItemCategoryRepository eamisItemCategoryRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _eamisPropertyItemsRepository = eamisPropertyItemsRepository;
            _eamisItemCategoryRepository = eamisItemCategoryRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("GeneratedProperty")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> GeneratedProperty()
        {
            return Ok(await _eamisPropertyItemsRepository.GeneratedProperty());
        }

        [HttpGet("PublicSearchPropertyItems")]
        public async Task<ActionResult<EAMISPROPERTYITEMS>> PublicSearchPropertyItems(string type, string SearchValue)
        {
            return Ok(await _eamisPropertyItemsRepository.PublicSearch(type, SearchValue));

        }

        [HttpGet("list")]
        public async Task<ActionResult<EAMISPROPERTYITEMS>> List([FromQuery] EamisPropertyItemsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.List(filter, config));
        }

        //[HttpPost("Add")]
        //public async Task<ActionResult<EamisPropertyItemsDTO>> Add([FromForm] EamisPropertyItemsDTO item)
        //{
        //    if (await _eamisPropertyItemsRepository.ValidateExistingItem(item.PropertyNo))
        //    {
        //        return Unauthorized();
        //    }

        //    if (item == null)
        //        item = new EamisPropertyItemsDTO();

        //     //Check if the request contains multipart/form-data.
        //    if (item.Photo == null)
        //    {
        //        return new UnsupportedMediaTypeResult();
        //    }
        //    IFormFile formFile = item.Photo;
        //    string fileName = "";
        //    if (System.IO.Path.GetExtension(formFile.FileName).ToLower() == ".jpg") //Change the file type according to the business rule
        //    {
        //        string targetPath = Path.Combine(_hostingEnvironment.WebRootPath, @"StaticFiles\Uploaded\PropertyImages\");
        //        fileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(formFile.FileName);
        //        string filePath = Path.Combine(targetPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            formFile.CopyTo(stream);
        //        }
        //        item.ImageURL = fileName;
        //    }
        //    return Ok(await _eamisPropertyItemsRepository.Insert(item));
        //}
        [HttpPost("Add")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Add([FromBody] EamisPropertyItemsDTO item)
        {
            if (await _eamisPropertyItemsRepository.ValidateExistingItem(item.PropertyNo))
            {
                return Unauthorized();
            }
            if (item == null)
                item = new EamisPropertyItemsDTO();
            return Ok(await _eamisPropertyItemsRepository.Insert(item));
        }

        //[HttpPut("Edit")]
        //public async Task<ActionResult<EamisPropertyItemsDTO>> Edit([FromForm] EamisPropertyItemsDTO item)
        //{
        //    var data = new EamisPropertyItemsDTO();
        //    var itemindb = await _eamisPropertyItemsRepository.UpdateValidateExistingItem(item.PropertyNo, item.Id);
        //    if (!itemindb)
        //        return NotFound();


        //    if (item.Photo == null)
        //    {
        //        return new UnsupportedMediaTypeResult();
        //    }
        //    string targetPath = Path.Combine(_hostingEnvironment.WebRootPath, @"StaticFiles\Uploaded\PropertyImages\");
        //    //if propertyItemId is not empty, get the image file name from DB
        //    //check the file if it exist in the image repository,
        //    //if file found/exist then delete it
        //    if (item.Id > 0)
        //    {
        //        //get the image file name from DB
        //        string imageInDB = _eamisPropertyItemsRepository.GetPropertyImageFileName(item.Id);
        //        if (imageInDB != null)
        //            if (imageInDB != "")
        //            {
        //                filePath = Path.Combine(targetPath, imageInDB);
        //                FileInfo file = new FileInfo(filePath);
        //                if (file.Exists) //check the file if it exist in the image repository
        //                    file.Delete(); //if file found/exist then delete it
        //            }
        //    }
        //    IFormFile formFile = item.Photo;
        //    string fileName = "";
        //    if (System.IO.Path.GetExtension(formFile.FileName).ToLower() == ".jpg") //Change the file type according to the business rule
        //    {

        //        fileName = Guid.NewGuid().ToString() + "" + Path.GetExtension(formFile.FileName);
        //        string filePath = Path.Combine(targetPath, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            formFile.CopyTo(stream);
        //        }
        //        item.ImageURL = fileName;
        //        return Ok(await _eamisPropertyItemsRepository.Update(item));
        //    }
        //    else
        //        return new UnsupportedMediaTypeResult();
        //}
        [HttpPut("Edit")]
        public async Task<ActionResult<EamisPropertyItemsDTO>> Edit([FromBody] EamisPropertyItemsDTO item)
        {
            var data = new EamisPropertyItemsDTO();
            if (await _eamisPropertyItemsRepository.UpdateValidateExistingItem(item.PropertyNo, item.Id))
            {
                if (item == null)
                    item = new EamisPropertyItemsDTO();
                return Ok(await _eamisPropertyItemsRepository.Update(item));
            }
            else if (await _eamisPropertyItemsRepository.ValidateExistingItem(item.PropertyNo))
            {
                return Unauthorized();
            }
            else
            {
                return Ok(await _eamisPropertyItemsRepository.Update(item));
            }
        }
        [HttpGet("getPropertyNo")]
        public async Task<string> GetSupplier(int categoryId)
        {
            var response = await _eamisItemCategoryRepository.GetPropertyNo(categoryId);
            return response;
        }

        [HttpGet("getPropertyImage")]
        public async Task<IActionResult> GetImage(int propertyItemId)
        {
            if (propertyItemId > 0)
            {
                string targetPath = Path.Combine(_hostingEnvironment.WebRootPath, @"StaticFiles\Uploaded\PropertyImages\");
                string imageInDB = await Task.Run(() => _eamisPropertyItemsRepository.GetPropertyImageFileName(propertyItemId)).ConfigureAwait(false);
                if (imageInDB != null)
                    if (imageInDB != "")
                    {
                        filePath = Path.Combine(targetPath, imageInDB);
                        return Ok(filePath);
                    }
            }
            return NotFound();
        }
    }


}

