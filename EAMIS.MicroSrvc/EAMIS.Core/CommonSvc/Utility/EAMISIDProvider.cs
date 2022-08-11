using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.CommonSvc.Utility
{
    public class EAMISIDProvider : IEAMISIDProvider
    {
        private readonly EAMISContext _ctx;

        public EAMISIDProvider(EAMISContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<string> GetNextSequenceNumber(string TransactionType)
        {
            var maxId = 0;
            var nextId = 0;
            string _id = "";
            switch (TransactionType)
            {
                case TransactionTypeSettings.DeliveryReceipt:
                    maxId = await Task.Run(() => _ctx.EAMIS_DELIVERY_RECEIPT.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.DRPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
                case TransactionTypeSettings.Issuance:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.ISPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
                case TransactionTypeSettings.PropertyTransfer:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false);
                    nextId = maxId + 1;
                    _id = PrefixSettings.PTPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
                case TransactionTypeSettings.ServiceLog:
                    maxId = await Task.Run(() => _ctx.EAMIS_SERVICE_LOG.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false);
                    nextId = maxId + 1;
                    _id = PrefixSettings.SLPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
            }
            return _id;
        }
        public async Task<string> GetNextSequenceNumberPR(string TransactionNumber)
        {
            var maxId = 0;
            var nextId = 0;
            string _id = "";
            switch (TransactionNumber)
            {
                case TransactionTypeSettings.PropertyReceiving:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?) t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.PRPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
            }
            return _id;
        }
    }
  }
