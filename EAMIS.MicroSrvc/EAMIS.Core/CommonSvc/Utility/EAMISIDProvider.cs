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
                    _id = PrefixSettings.DRPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.Issuance:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id =  DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PropertyTransfer:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = /*PrefixSettings.PTPrefix +*/ DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.ServiceLog:
                    maxId = await Task.Run(() => _ctx.EAMIS_SERVICE_LOG.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.SLPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PropertyDisposal:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.PDPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PropertyRevaluation:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_REVALUATION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.PVPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.IssuanceMaterial:
                    maxId = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Max(t => (int?)t.ID) ?? 0).ConfigureAwait(false); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.IMPrefix + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.ICSIssuance:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.ICSIssuance).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PARIssuance:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.PARIssuance).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PTRTransfer:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.PTRTransfer).ToList().Count();
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
                case TransactionTypeSettings.ITRTransfer:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.ITRTransfer).ToList().Count();
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
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
        public async Task<string> GetNextSequenceNumberPerBranch(string TransactionType, string branchID)
        {
            var maxId = 0;
            var nextId = 0;
            string _id = "";
            switch (TransactionType)
            {
                case TransactionTypeSettings.DeliveryReceipt:
                    maxId = _ctx.EAMIS_DELIVERY_RECEIPT.Where(t => t.BRANCH_ID == branchID).ToList().Count() ; //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.DRPrefix +"-"+branchID+"-"+DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PropertyReceiving:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.BRANCH_ID == branchID && t.TRANSACTION_TYPE == TransactionTypeSettings.PropertyReceiving).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.PRPrefix + "-" + branchID + "-" + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.IssuanceMaterials:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.BRANCH_ID == branchID && t.TRANSACTION_TYPE == TransactionTypeSettings.IssuanceMaterials).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.IMPrefix + "-" + branchID + "-" + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.ICSIssuance:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.ICSIssuance && t.BRANCH_ID == branchID).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PARIssuance:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.PARIssuance && t.BRANCH_ID == branchID).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PTRTransfer:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.PTRTransfer && t.BRANCH_ID == branchID).ToList().Count();
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
                case TransactionTypeSettings.ITRTransfer:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.TRAN_TYPE == TransactionTypeSettings.ITRTransfer && t.BRANCH_ID == branchID).ToList().Count();
                    nextId = maxId + 1;
                    _id = DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');
                    break;
                case TransactionTypeSettings.ServiceLog:
                    maxId = _ctx.EAMIS_SERVICE_LOG.Where(t => t.BRANCH_ID == branchID && t.SERVICE_LOG_TYPE == TransactionTypeSettings.ServiceLog).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.SLPrefix + "-" + branchID + "-" + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PropertyDisposal:
                    maxId = _ctx.EAMIS_PROPERTY_TRANSACTION.Where(t => t.BRANCH_ID == branchID && t.TRANSACTION_TYPE == TransactionTypeSettings.PropertyDisposal).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.PDPrefix + "-" + branchID + "-" + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
                case TransactionTypeSettings.PropertyRevaluation:
                    maxId = _ctx.EAMIS_PROPERTY_REVALUATION.Where(t => t.BRANCH_ID == branchID).ToList().Count(); //returns the maximum value in the sequence. note, read from identity type column only
                    nextId = maxId + 1;
                    _id = PrefixSettings.PVPrefix + "-" + branchID + "-" + DateTime.Now.Year.ToString() + nextId.ToString().PadLeft(6, '0');//change according to the business rule
                    break;
            }
            return _id;
        }
    }
  }
