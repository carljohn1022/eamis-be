using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Inventory_Taking
{
    public class EamisInventoryTakingDTO
    {   
        public string PropertyNumber { get; set; }  
        public string SerialNumber { get; set; } 
        public DateTime AcquisitionDate { get; set; }
        public string Office { get; set; }
        public string Department { get; set; }
        public string Remarks { get; set; }
        public string UserStamp { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

}
