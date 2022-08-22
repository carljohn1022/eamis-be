using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public interface IFactorType
    {
        decimal GetFactorTypeValue(string FactorType);
    }
}