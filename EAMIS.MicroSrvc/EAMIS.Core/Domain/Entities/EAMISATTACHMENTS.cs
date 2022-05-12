using System.Collections.Generic;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISATTACHMENTS
    {
        public int ID { get; set; }
        public int PARENTID { get; set; }
        public string ATTACHMENT_DESCRIPTION { get; set; }
        public string ATTACHMENT_TYPE_DESCRIPTION { get; set; }
        public bool IS_REQUIRED { get; set; }
        public EAMISATTACHMENTS ATTACHMENTPARENT { get; set; }
        public HashSet<EAMISATTACHMENTS> ATTACHMENTSCHILD { get; set; }
    }
}
