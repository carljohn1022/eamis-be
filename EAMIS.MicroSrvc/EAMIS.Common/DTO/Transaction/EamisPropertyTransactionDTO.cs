using System;
using System.Collections.Generic;

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
        public string TimeStamp  { get; set; }
        public string TransactionStatus { get; set; }

    }
}
