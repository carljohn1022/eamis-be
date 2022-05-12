using System.Collections.Generic;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISSUBCLASSIFICATION
    {
        public int ID { get; set; }
        public int CLASSIFICATION_ID { get; set; }
        public string NAME_SUBCLASSIFICATION { get; set; }
        public EAMISCLASSIFICATION CLASSIFICATION { get; set; }
        public List<EAMISGROUPCLASSIFICATION> GROUPCLASSIFICATION { get; set; }
        public List<EAMISCHARTOFACCOUNTS> CHARTOFACCOUNTS { get; set; }
    }
}
