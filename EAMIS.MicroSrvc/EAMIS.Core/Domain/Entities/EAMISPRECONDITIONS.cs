using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPRECONDITIONS
    {
        public int ID { get; set; }
        public int PARENT_ID { get; set; }
        public string PRE_CONDITION_DESCRIPTION { get; set; }
    }
}
