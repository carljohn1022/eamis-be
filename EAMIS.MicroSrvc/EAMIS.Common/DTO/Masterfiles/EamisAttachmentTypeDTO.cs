using System.Collections.Generic;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisAttachmentTypeDTO
    {
        public int Id { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentTypeDescription { get; set; }

        //public EamisAttachmentsDTO AttachmentsDTO { get; set; }
    }
}
