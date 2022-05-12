using System.Collections.Generic;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisAttachmentsDTO
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string AttachmentDescription { get; set; }
        public string AttachmentTypeDescription { get; set; }
        public bool Is_Required { get; set; }
    }
}
