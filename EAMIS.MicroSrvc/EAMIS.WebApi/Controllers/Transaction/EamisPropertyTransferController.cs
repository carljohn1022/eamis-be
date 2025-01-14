﻿using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        IEamisAttachedFilesRepository _eamisAttachedFilesRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EamisPropertyTransferController(IEamisPropertyTransferRepository eamisPropertyTransferRepository, IEamisPropertyTransactionRepository eamisPropertyTransactionRepository,IEamisAttachedFilesRepository eamisAttachedFilesRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _eamisPropertyTransferRepository = eamisPropertyTransferRepository;
            _eamisPropertyTransactionRepository = eamisPropertyTransactionRepository;
            _eamisAttachedFilesRepository = eamisAttachedFilesRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync(string tranType, string branchID)
        {
            var nextId = await _eamisPropertyTransferRepository.GetNextSequenceNumber(tranType, branchID);
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
        [HttpGet("DownloadFile")]
        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "StaticFiles/EAMIS_Attached_Files/Property_Transfer/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        [HttpPost("UploadImages")]
        public async Task<ActionResult> UploadImages(List<IFormFile> imgFiles, string TransactionNumber)
        {

            //if (imgFiles == null || imgFiles.Count == 0)
            //    return BadRequest("No file is uploaded.");

            if (imgFiles.Count > 5)
                return BadRequest("You can only upload up to five files.");


            var targetPath = Path.Combine(_hostingEnvironment.WebRootPath,
                                          FolderName.StaticFolderLocation + @"\" +
                                          FolderName.EAMISAttachmentLocation + @"\" +
                                          ModuleName.PropertyTransferName + @"\");
            /*DateTime.Now.Date.ToString("MMddyyyy") + @"\");*/  // determine the destination for file storage

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath); //create the target path if not yet exist

            List<EamisAttachedFilesDTO> lstattachedFiles = new List<EamisAttachedFilesDTO>();

            foreach (var img in imgFiles)
            {
                //get current file name
                string curFileName = img.FileName;
                //check if file is already exist on the repository

                var fileExist = Directory.Exists(targetPath + curFileName);

                if (!fileExist)
                {

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName); // if we want to use the uploaded file name, replace this line with >> string fileName = img.FileName; 

                    string filePath = Path.Combine(targetPath, fileName);
                    try
                    {
                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            img.CopyTo(stream);
                        }
                        EamisAttachedFilesDTO objeamisAttachedFilesDTO = new EamisAttachedFilesDTO();
                        objeamisAttachedFilesDTO.Id = 0;
                        objeamisAttachedFilesDTO.FileName = fileName;
                        objeamisAttachedFilesDTO.ModuleName = ModuleName.PropertyTransferName;
                        objeamisAttachedFilesDTO.TransactionNumber = TransactionNumber;
                        objeamisAttachedFilesDTO.UserStamp = "user"; //please change this to the actual/global variable used (where we store the user name?)
                        objeamisAttachedFilesDTO.TimeStamp = DateTime.Now;
                        lstattachedFiles.Add(objeamisAttachedFilesDTO);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
            }

            if (lstattachedFiles.Count > 0) //at least a file is successfully copied/uploaded to the server before we insert it to the database
            {
                //save to DB
                var result = await _eamisAttachedFilesRepository.Insert(lstattachedFiles);
            }

            return Ok();
        }
        [HttpDelete("deleteImage")]
        public async Task<ActionResult> DeleteImage(string transactionNumber, string fileName)
        {
            if (transactionNumber != string.Empty)
            {
                string filePath;
                string targetPath;

                //get the image file name from DB
                string imageInDB = await _eamisAttachedFilesRepository.GetTranFileName(transactionNumber, fileName);
                if (imageInDB != null)
                    if (imageInDB != "")
                    {
                        targetPath = Path.Combine(_hostingEnvironment.WebRootPath, FolderName.StaticFolderLocation + @"\" + FolderName.EAMISAttachmentLocation + @"\" + ModuleName.PropertyTransferName);
                        filePath = Path.Combine(targetPath, imageInDB);
                        FileInfo file = new FileInfo(filePath);
                        if (file.Exists) //check the file if it exist in the image repository
                        {
                            file.Delete(); //if file found/exist then delete it
                            await _eamisAttachedFilesRepository.DeleteImageFileName(transactionNumber, fileName); //delete record in DB
                        }
                        return Ok();
                    }
            }
            return BadRequest("Required parameter is missing.");
        }
    }
}
