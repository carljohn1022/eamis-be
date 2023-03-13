using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISUNITOFMEASURE
    {
        public int ID { get; set; }
        public string SHORT_DESCRIPTION { get; set; }
        public string UOM_DESCRIPTION { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string USER_STAMP { get; set; }
        public List<EAMISPROPERTYITEMS> PROPERTY_ITEM { get; set; }
      


    }
}
