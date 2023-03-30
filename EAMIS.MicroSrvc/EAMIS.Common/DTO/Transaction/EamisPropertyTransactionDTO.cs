using System;
using System.Collections.Generic;
using EAMIS.Common.DTO.Masterfiles;

namespace EAMIS.Common.DTO.Transaction
{
    public class EamisPropertyTransactionDTO
    {
        public int Id { get; set; }
        public string TransactionNumber  { get; set; }
        public DateTime TransactionDate  { get; set; }
        public string FiscalPeriod  { get; set; }
        public string TransactionType  { get; set; }
        public string Memo  { get; set; }
        public string ReceivedBy  { get; set; }
        public string ApprovedBy  { get; set; }
        public DateTime DeliveryDate  { get; set; }
        public string UserStamp  { get; set; }
        public string TransactionStatus { get; set; }
        public string FundSource { get; set; }
        public string TranType { get; set; }
        public bool IsProperty { get; set; }
        public string BranchID { get; set; }
        public bool ForDonation { get; set; }
        public List<EamisAttachedFilesDTO> DeliveryImages { get; set; } = new List<EamisAttachedFilesDTO>();
        public List<EamisPropertyTransactionDetailsDTO> PropertyTransactionDetails { get; set; } = new List<EamisPropertyTransactionDetailsDTO>();
    }
}
