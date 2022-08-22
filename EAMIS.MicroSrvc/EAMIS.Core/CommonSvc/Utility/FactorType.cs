using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public class FactorType : IFactorType
    {
        private readonly EAMISContext _ctx;
        public FactorType(EAMISContext ctx)
        {
            _ctx = ctx;
        }

        public decimal GetFactorTypeValue(string FactorType)
        {
            return  _ctx.EAMIS_FACTOR_TYPE.Where(x => x.FACTOR_CODE == FactorType).Select(x => (decimal?)x.FACTOR_VALUE ?? 0).FirstOrDefault();
        }
    }
}
