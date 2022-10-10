using System;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISATTACHEDFILES
    {
        public int ID { get; set; }
        public string ATTACHED_FILENAME { get; set; }
        public string MODULE_NAME { get; set; }
        public string TRANID { get; set; }
        public string USERSTAMP { get; set; }
        public DateTime TIIMESTAMP { get; set; }
    }
}