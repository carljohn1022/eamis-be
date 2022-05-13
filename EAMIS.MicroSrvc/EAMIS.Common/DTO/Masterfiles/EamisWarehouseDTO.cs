using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.Masterfiles
{
    public class EamisWarehouseDTO
    {
        public int Id { get; set; }
        public string Warehouse_Description { get; set; }
        public string Street_Name { get; set; }
        public int Region_Code { get; set; }
        public int Municipality_Code { get; set; }
        public int Province_Code { get; set; }
        public int Barangay_Code { get; set; }
        public bool IsActive { get; set; }
        //public EamisRegionDTO RegionDTO { get; set; }
        public EamisBarangayDTO Barangay { get; set; }
    }
}
