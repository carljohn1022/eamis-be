using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.Domain.Entities
{
    public class EAMISINVENTORYTAKING
    {
        public int ID { get; set; }
        public string PROPERTY_NUMBER { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public DateTime ACQUISITION_DATE { get; set; }
        public string OFFICE { get; set; }
        public string DEPARTMENT { get; set; }
        public string REMARKS { get; set; }
        public string USER_STAMP { get; set; }
    }
}
