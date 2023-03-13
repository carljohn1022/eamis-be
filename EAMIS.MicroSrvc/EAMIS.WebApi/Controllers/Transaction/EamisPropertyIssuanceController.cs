using EAMIS.Common.DTO.Masterfiles;
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
    public class EamisPropertyIssuanceController : ControllerBase
    {
        IEamisPropertyIssuanceRepository _eamisPropertyIssuanceRepository;
        IEamisPropertyTransactionRepository _eamisPropertyTransactionRepository;
        IEamisAttachedFilesRepository _eamisAttachedFilesRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EamisPropertyIssuanceController(IEamisPropertyIssuanceRepository eamisPropertyIssuanceRepository, IEamisPropertyTransactionRepository eamisPropertyTransactionRepository
            , IEamisAttachedFilesRepository eamisAttachedFilesRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _eamisPropertyIssuanceRepository = eamisPropertyIssuanceRepository;
            _eamisPropertyTransactionRepository = eamisPropertyTransactionRepository;
            _eamisAttachedFilesRepository = eamisAttachedFilesRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("getNextSequence")]
        public async Task<string> GetNextSequenceAsync(string tranType)
        {
            var nextId = await _eamisPropertyIssuanceRepository.GetNextSequenceNumber(tranType);
            return nextId;
        }
        [HttpGet("getNextSequenceForMaterialIssuance")]
        public async Task<string> GetNextSequenceForMaterialIssuanceAsync()
        {
            var nextId = await _eamisPropertyIssuanceRepository.GetNextSequenceNumberForMaterialIssuance();
            return nextId;
        }
        [HttpGet("Search")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> Search(string type, string searchValue, bool isProperty)
        {
            return Ok(await _eamisPropertyTransactionRepository.SearchReceivingforIssuance(type, searchValue, isProperty));
        }
        //[HttpGet("list")]
        //public async Task<ActionResult<EAMISPROPERTYDETAILS>> List([FromQuery] EamisPropertyItemsDTO filter, [FromQuery] PageConfig config)
        //{
        //    if (filter == null)
        //        filter = new EamisPropertyItemsDTO();
        //    return Ok(await _eamisPropertyIssuanceRepository.List(filter, config));
        //}
        
        //[HttpGet("list")]
        //public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> List([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config)
        //{
        //    if (filter == null)
        //        filter = new EamisPropertyTransactionDetailsDTO();
        //    return Ok(await _eamisPropertyIssuanceRepository.List(filter, config));
        //}


        [HttpPost("AddPropertyTransaction")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransaction([FromBody] EamisPropertyTransactionDTO eamisPropertyTransactionDTO)
        {
            //Steps
            //1. Create Property Transaction
            var result = await _eamisPropertyIssuanceRepository.InsertProperty(eamisPropertyTransactionDTO);
            if (result == null)
                return BadRequest();
            //2. Create Property Transaction Details
            //3. Update Property Items In Stock Quantity
            return Ok(result); //return the item with ID, needed in constructing the payload of the Property Transaction details
        }
        [HttpPost("AddPropertyTransactionForMaterial")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransactionIssuanceMaterial([FromBody] EamisPropertyTransactionDTO eamisPropertyTransactionDTO)
        {
            //Steps
            //1. Create Property Transaction
            var result = await _eamisPropertyIssuanceRepository.InsertPropertyForMaterialIssuance(eamisPropertyTransactionDTO);
            if (result == null)
                return BadRequest();
            //2. Create Property Transaction Details
            //3. Update Property Items In Stock Quantity
            return Ok(result); //return the item with ID, needed in constructing the payload of the Property Transaction details
        }
        [HttpPost("UpdatePropertyTransaction")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> Edit([FromBody] EamisPropertyTransactionDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDTO();
            return Ok(await _eamisPropertyIssuanceRepository.UpdateProperty(item));
        }

        [HttpPut("UpdatePropertyTransactionDetails")]
        public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Edit([FromBody] EamisPropertyTransactionDetailsDTO item)
        {
            if (item == null)
                item = new EamisPropertyTransactionDetailsDTO();
            return Ok(await _eamisPropertyIssuanceRepository.UpdateDetails(item));
        }
        //[HttpGet("editbyid")]
        //public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> getPropertyItemById(int itemID)
        //{
        //    return Ok(await _eamisPropertyIssuanceRepository.getPropertyItemById(itemID));
        //}
        [HttpGet("editbyid")]
        public async Task<ActionResult<EamisPropertyTransactionDTO>> getPropertyItemById(int itemID)
        {
            return Ok(await _eamisPropertyIssuanceRepository.getPropertyItemById(itemID));
        }
        [HttpPost("AddPropertyTransactionDetails")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransactionDetails([FromBody] EamisPropertyTransactionDetailsDTO eamisPropertyTransactionDetailsDTO)
        {
            //Steps
            //2. Create Property Transaction Details
            //Note: make sure that Property Transaction ID generated from  AddPropertyTransaction method is assigned to  PropertyTransactionDetails.PropertyTransactionID
            //and other required fields are properly filled-up before calling this method
            var result = await _eamisPropertyIssuanceRepository.InsertPropertyTransaction(eamisPropertyTransactionDetailsDTO);
            if (result == null)
                return BadRequest();
            //3. Update Property Items In Stock Quantity
            return Ok(result);
        }

        [HttpPost("UpdateItemQtyInStock")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTION>> AddPropertyTransactionDetails([FromBody] EamisDeliveryReceiptDetailsDTO eamisDeliveryReceiptDetailsDTO)
        {
            //Steps
            //2. Create Property Transaction Details
            //3. Update Property Items In Stock Quantity
            var result = await _eamisPropertyIssuanceRepository.UpdatePropertyItemQty(eamisDeliveryReceiptDetailsDTO);
            if (result == null)
                return BadRequest();

            return Ok();//use "return Ok(result) if the result is needed to return.
        }

        //[HttpGet("listForReceivingItems")]
        //public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> ListItemsForReceivingItems()
        //{
        //    return Ok(await _eamisPropertyIssuanceRepository.ListItemsForReceivingItems());
        //}
        [HttpGet("listitemsforreceiving")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> ListItemsForReceiving([FromQuery] EamisPropertyTransactionDetailsDTO filter, [FromQuery] PageConfig config, bool bolIsProperty, string tranType, int assigneeCustodian)
        {
            if (filter == null)
                filter = new EamisPropertyTransactionDetailsDTO();
            if (bolIsProperty)
                return Ok(await _eamisPropertyIssuanceRepository.ListItemsForReceiving(filter, config, tranType, assigneeCustodian));
            else
                return Ok(await _eamisPropertyIssuanceRepository.ListSupplyItemsForReceiving(filter, config, tranType, assigneeCustodian));
        }
        [HttpGet("SearchReceiving")]
        public async Task<ActionResult<EAMISPROPERTYTRANSACTIONDETAILS>> SearchReceiving(string type, string searchValue)
        {
            return Ok(await _eamisPropertyIssuanceRepository.SearchReceiving(type, searchValue));
        }
        [HttpGet("SearchDRForIssuanceMaterial")]
        public async Task<ActionResult<EAMISDELIVERYRECEIPTDETAILS>> SearchDRForIssuanceMaterial(string type, string searchValue)
        {
            return Ok(await _eamisPropertyIssuanceRepository.SearchDRForMaterialIssuance(type, searchValue));
        }
        
        [HttpGet("GeneratePropertyNumber")]
        public async Task<string> GeneratePropertyNumber(DateTime acquisitionDate, string itemCode, string responsibilityCode, int counter)
        {
            var result = await _eamisPropertyIssuanceRepository.GeneratePropertyNumber(acquisitionDate, itemCode, responsibilityCode, counter);

            if (_eamisPropertyIssuanceRepository.HasError)
                return _eamisPropertyIssuanceRepository.ErrorMessage;

            return result;
        }
        //[HttpGet("GeneratePropertyNumber")]
        //public async Task<string> GeneratePropertyNumber(int transactionDetailId, string itemCode, string responsibilityCode)
        //{
        //    var result = await _eamisPropertyIssuanceRepository.GeneratePropertyNumber(transactionDetailId, itemCode, responsibilityCode);

        //    if (_eamisPropertyIssuanceRepository.HasError)
        //        return _eamisPropertyIssuanceRepository.ErrorMessage;

        //    return result;
        //}

        //[HttpDelete("Delete")]
        //public async Task<ActionResult<EamisPropertyTransactionDetailsDTO>> Delete([FromBody] EamisPropertyTransactionDetailsDTO item)
        //{
        //    if (item == null)
        //        item = new EamisPropertyTransactionDetailsDTO();
        //    return Ok(await _eamisPropertyIssuanceRepository.Delete(item));
        //}
        [HttpGet("DownloadFile")]
        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "StaticFiles/EAMIS_Attached_Files/Property_Issuance/") + fileName;

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
                                          ModuleName.PropertyIssuanceName + @"\");
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
                        objeamisAttachedFilesDTO.ModuleName = ModuleName.PropertyIssuanceName;
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
                        targetPath = Path.Combine(_hostingEnvironment.WebRootPath, FolderName.StaticFolderLocation + @"\" + FolderName.EAMISAttachmentLocation + @"\" + ModuleName.PropertyIssuanceName);
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
        [HttpGet("getResponsibilityCenter")]
        public async Task<string> GetResponsibilityCenterByID(string responsibilityCode)
        {
            var response = await _eamisPropertyIssuanceRepository.GetResponsibilityCenterByID(responsibilityCode);
            return response;
        }
        [HttpGet("GetPropertyNumber")]
        public async Task<string> GetPropertyNo(DateTime acquisitionDate, string responsibilityCode, string serialNumber)
        {
            var response = await _eamisPropertyIssuanceRepository.GetPropertyNumber(acquisitionDate, responsibilityCode, serialNumber);
            return response;
        }
        [HttpGet("GetDRNumFrSupplier")]
        public async Task<string> GetDRNum(string dr)
        {
            var response = await _eamisPropertyIssuanceRepository.GetDRNumFrSupplier(dr);
            return response;
        }
        [HttpGet("GetAPRNumber")]
        public async Task<string> GetAPRNum(string dr)
        {
            var response = await _eamisPropertyIssuanceRepository.GetDRNumFrSupplier(dr);
            return response;
        }
        [HttpGet("DRListForIssuanceSupplies")]
        public async Task<ActionResult<EamisDeliveryReceiptDetailsDTO>> List([FromQuery] EamisDeliveryReceiptDetailsDTO filter, [FromQuery] PageConfig config)
        {
            if (filter == null)
                filter = new EamisDeliveryReceiptDetailsDTO();
            return Ok(await _eamisPropertyIssuanceRepository.ListSuppliesDRForIssuance(filter, config));
        }
        
    }
}
