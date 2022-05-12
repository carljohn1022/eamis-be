using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
   public class EamisResponsibilityCodeDTO
    {
        public int Id { get; set; }
        public string Office { get; set; }
        public string Department { get; set; }
        public bool isActive { get; set; }
    }
}
