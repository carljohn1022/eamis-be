using System;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisAttachedFilesDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ModuleName { get; set; }
        public string TransactionNumber { get; set; }
        public string UserStamp { get; set; }
        public DateTime TimeStamp { get; set; }
        public int IsNew { get; set; }
    }
}