using System.Collections.Generic;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISCLASSIFICATION
    {
        public int ID { get; set; }
        public string NAME_CLASSIFICATION { get; set; }

        public List<EAMISSUBCLASSIFICATION> SUBCLASSIFICIATION { get; set; }
        public List<EAMISGROUPCLASSIFICATION> GROUPCLASSIFICATION { get; set; }
        public List<EAMISCHARTOFACCOUNTS> CHARTOFACCOUNTS { get; set; }
    }
}
