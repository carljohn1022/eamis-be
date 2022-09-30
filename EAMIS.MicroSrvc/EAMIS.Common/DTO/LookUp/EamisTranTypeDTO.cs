using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Common.DTO.LookUp
{
    public class EamisTranTypeDTO
    {
        public int Id { get; set; }
        public int AssetID { get; set; }
        public string TranType { get; set; }
        public EamisAssetConditionTypeDTO AssetConditionType { get; set; }
    }
}
