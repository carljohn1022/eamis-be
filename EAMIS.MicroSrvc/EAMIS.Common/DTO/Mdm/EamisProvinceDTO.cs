using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO
{
    public class EamisProvinceDTO
    {
        public string Psgccode { get; set; }
        public string ProvinceDescription { get; set; }
        public int RegionCode { get; set; }
        public int ProvinceCode { get; set; }
        public EamisRegionDTO Region { get; set; }
    }
}
