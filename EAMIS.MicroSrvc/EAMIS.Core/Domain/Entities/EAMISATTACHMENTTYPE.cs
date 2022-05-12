using System.Collections.Generic;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISATTACHMENTTYPE
    {
        public int ID { get; set; }
        public int ATTACHMENT_ID { get; set; }
        public string ATTACHMENT_TYPE_DESCRIPTION { get; set; }

        public EAMISATTACHMENTS ATTACHMENTS { get; set; }

    }
}
