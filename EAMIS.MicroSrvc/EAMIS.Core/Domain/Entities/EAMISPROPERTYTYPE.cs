using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYTYPE
    {
        public int ID { get; set; }
        public string PROPERTY_TYPE_CODE { get; set; }
        public string PROPERTY_TYPE_NAME { get; set; }
        public string PROPERTY_TYPE_DESCRIPTION { get; set; }
        public decimal PROPERTY_TYPE_AMOUNT { get; set; }

    }
}
