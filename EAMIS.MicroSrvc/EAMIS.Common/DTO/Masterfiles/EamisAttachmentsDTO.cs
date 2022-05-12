using System.Collections.Generic;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisAttachmentsDTO
    {
        public int Id { get; set; }
        public string AttachmentDescription { get; set; }
        public bool Is_Required { get; set; }
        public List<EamisAttachmentTypeDTO> AttachmentTypeDTO { get; set; }
    }
}
