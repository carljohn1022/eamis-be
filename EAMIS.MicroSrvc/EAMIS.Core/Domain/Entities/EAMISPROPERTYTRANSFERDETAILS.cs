using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISPROPERTYTRANSFERDETAILS
    {
        public int ID { get; set; }
        public int FROM_USER_ID { get; set; }
        public int TO_USER_ID { get; set; }
        public DateTime TRANSFER_DATE { get; set; }
        public string PROPERTY_TRANSFER_DESCRIPTION { get; set; }
    }
}
