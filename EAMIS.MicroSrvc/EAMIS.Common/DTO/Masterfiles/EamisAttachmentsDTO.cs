using System.Collections.Generic;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisAttachmentsDTO
    {
        public int Id { get; set; }
        public string AttachmentDescription { get; set; }
        public string AttachmentTypeDescription { get; set; }
        public string ModuleName { get; set; }
        public List<EamisAttachmentsDTO> AttachmentDTO { get; set; }
    }
}
