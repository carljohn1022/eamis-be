using System.Collections.Generic;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISATTACHMENTS
    {
        public int ID { get; set; }
        public string ATTACHMENT_DESCRIPTION { get; set; }
        public bool IS_REQUIRED { get; set; }
        public List<EAMISATTACHMENTTYPE> ATTACHMENTTYPE { get; set; }
    }
}
