using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisProcurementCategoryDTO
    {
        public int Id { get; set; }
        public string ProcurementDescription { get; set; }
        public bool isActive { get; set; }
    }
}
