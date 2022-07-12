using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public interface IEAMISIDProvider
    {
        Task<string> GetNextSequenceNumber(string TransactionType);
    }
}
