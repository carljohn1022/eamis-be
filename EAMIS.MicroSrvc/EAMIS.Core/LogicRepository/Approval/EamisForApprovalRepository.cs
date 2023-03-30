using EAMIS.Common.DTO.Approval;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.CommonSvc.Constant;
using EAMIS.Core.ContractRepository.Approval;
using EAMIS.Core.ContractRepository.Transaction;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using EAMIS.Core.Response.DTO;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Approval
{
    public class EamisForApprovalRepository : IEamisForApprovalRepository
    {


        private EAMISContext _ctx;
        private readonly int _maxPageSize;
        private readonly IEamisPropertyTransactionDetailsRepository _eamisPropertyTransactionDetailsRepository;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisForApprovalRepository(EAMISContext ctx, IEamisPropertyTransactionDetailsRepository eamisPropertyTransactionDetailsRepository)
        {
            _ctx = ctx;
            _eamisPropertyTransactionDetailsRepository = eamisPropertyTransactionDetailsRepository;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }


        public async Task<MyApprovalListDTO> SubmitApproval(MyApprovalListDTO item)
        {
            try
            {
                bool bolClosed = false;
                if (item.ApprovalLevel == 1)
                {
                    var result = _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER1_ID == item.UserId &&
                                          i.TRANSACTION_TYPE == item.TransactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER1_STATUS == DocStatus.ForApproval &&
                                          i.ID == item.SourceId)
                              .FirstOrDefault();
                    if (result != null)
                    {
                        if (result.APPROVER2_ID == 0)
                        {
                            item.DocStatus = item.DocStatus;

                            if (item.Status == DocStatus.Approved || item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                                bolClosed = true;
                        }

                        if (item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                            bolClosed = true;

                        EamisForApprovalDTO temp = new EamisForApprovalDTO
                        {
                            Id = item.SourceId,
                            DocStatus = bolClosed == true ? item.Status : item.DocStatus,
                            TimeStamp = result.TIMESTAMP,
                            TransactionNumber = item.TransactionNumber,
                            TransactionType = item.TransactionType,
                            Approver1Id = item.UserId,
                            Approver1Status = item.Status,
                            Approver1Trandate = item.Trandate,
                            Approver1RejectedReason = item.RejectedReason,

                            Approver2Id = result.APPROVER2_ID,
                            Approver2Status = result.APPROVER2_STATUS,
                            Approver2Trandate = result.APPROVER2_TRANDATE,
                            Approver2RejectedReason = result.APPROVER2_REJECTEDREASON,

                            Approver3Id = result.APPROVER3_ID,
                            Approver3Status = result.APPROVER3_STATUS,
                            Approver3Trandate = result.APPROVER3_TRANDATE,
                            Approver3RejectedReason = result.APPROVER3_REJECTEDREASON,

                            Approver4Id = result.APPROVER4_ID,
                            Approver4Status = result.APPROVER4_STATUS,
                            Approver4Trandate = result.APPROVER4_TRANDATE,
                            Approver4RejectedReason = result.APPROVER4_REJECTEDREASON,

                            Approver5Id = result.APPROVER5_ID,
                            Approver5Status = result.APPROVER5_STATUS,
                            Approver5Trandate = result.APPROVER5_TRANDATE,
                            Approver5RejectedReason = result.APPROVER5_REJECTEDREASON,
                        };
                        EAMISFORAPPROVAL data = MapToEntity(temp);
                        _ctx.Entry(data).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                }
                else if (item.ApprovalLevel == 2)
                {
                    var result = _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER2_ID == item.UserId &&
                                          i.TRANSACTION_TYPE == item.TransactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER2_STATUS == DocStatus.ForApproval &&
                                          i.APPROVER1_STATUS == DocStatus.Approved &&
                                          i.ID == item.SourceId)
                              .FirstOrDefault();
                    if (result != null)
                    {
                        if (result.APPROVER3_ID == 0)
                        {
                            item.DocStatus = item.DocStatus;

                            if (item.Status == DocStatus.Approved || item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                                bolClosed = true;
                        }

                        if (item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                            bolClosed = true;

                        EamisForApprovalDTO temp = new EamisForApprovalDTO
                        {
                            Id = item.SourceId,
                            DocStatus = bolClosed == true ? item.Status : item.DocStatus,
                            TimeStamp = result.TIMESTAMP,
                            TransactionNumber = result.TRANSACTION_NUMBER,
                            TransactionType = result.TRANSACTION_TYPE,
                            Approver1Id = result.APPROVER1_ID,
                            Approver1Status = result.APPROVER1_STATUS,
                            Approver1Trandate = result.APPROVER1_TRANDATE,
                            Approver1RejectedReason = result.APPROVER1_REJECTEDREASON,

                            Approver2Id = item.UserId,
                            Approver2Status = item.Status,
                            Approver2Trandate = item.Trandate,
                            Approver2RejectedReason = item.RejectedReason,

                            Approver3Id = result.APPROVER3_ID,
                            Approver3Status = result.APPROVER3_STATUS,
                            Approver3Trandate = result.APPROVER3_TRANDATE,
                            Approver3RejectedReason = result.APPROVER3_REJECTEDREASON,

                            Approver4Id = result.APPROVER4_ID,
                            Approver4Status = result.APPROVER4_STATUS,
                            Approver4Trandate = result.APPROVER4_TRANDATE,
                            Approver4RejectedReason = result.APPROVER4_REJECTEDREASON,

                            Approver5Id = result.APPROVER5_ID,
                            Approver5Status = result.APPROVER5_STATUS,
                            Approver5Trandate = result.APPROVER5_TRANDATE,
                            Approver5RejectedReason = result.APPROVER5_REJECTEDREASON,
                        };
                        EAMISFORAPPROVAL data = MapToEntity(temp);
                        _ctx.Entry(data).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                }
                else if (item.ApprovalLevel == 3)
                {
                    var result = _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER3_ID == item.UserId &&
                                          i.TRANSACTION_TYPE == item.TransactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER3_STATUS == DocStatus.ForApproval &&
                                          i.APPROVER2_STATUS == DocStatus.Approved &&
                                          i.ID == item.SourceId)
                              .FirstOrDefault();
                    if (result != null)
                    {
                        if (result.APPROVER4_ID == 0)
                        {
                            item.DocStatus = item.DocStatus;

                            if (item.Status == DocStatus.Approved || item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                                bolClosed = true;
                        }

                        if (item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                            bolClosed = true;

                        EamisForApprovalDTO temp = new EamisForApprovalDTO
                        {
                            Id = item.SourceId,
                            DocStatus = bolClosed == true ? item.Status : item.DocStatus,
                            TimeStamp = result.TIMESTAMP,
                            TransactionNumber = result.TRANSACTION_NUMBER,
                            TransactionType = result.TRANSACTION_TYPE,
                            Approver1Id = result.APPROVER1_ID,
                            Approver1Status = result.APPROVER1_STATUS,
                            Approver1Trandate = result.APPROVER1_TRANDATE,
                            Approver1RejectedReason = result.APPROVER1_REJECTEDREASON,

                            Approver2Id = result.APPROVER2_ID,
                            Approver2Status = result.APPROVER2_STATUS,
                            Approver2Trandate = result.APPROVER2_TRANDATE,
                            Approver2RejectedReason = result.APPROVER2_REJECTEDREASON,

                            Approver3Id = item.UserId,
                            Approver3Status = item.Status,
                            Approver3Trandate = item.Trandate,
                            Approver3RejectedReason = item.RejectedReason,

                            Approver4Id = result.APPROVER4_ID,
                            Approver4Status = result.APPROVER4_STATUS,
                            Approver4Trandate = result.APPROVER4_TRANDATE,
                            Approver4RejectedReason = result.APPROVER4_REJECTEDREASON,

                            Approver5Id = result.APPROVER5_ID,
                            Approver5Status = result.APPROVER5_STATUS,
                            Approver5Trandate = result.APPROVER5_TRANDATE,
                            Approver5RejectedReason = result.APPROVER5_REJECTEDREASON,
                        };
                        EAMISFORAPPROVAL data = MapToEntity(temp);
                        _ctx.Entry(data).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                }
                else if (item.ApprovalLevel == 4)
                {
                    var result = _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER4_ID == item.UserId &&
                                          i.TRANSACTION_TYPE == item.TransactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER4_STATUS == DocStatus.ForApproval &&
                                          i.APPROVER3_STATUS == DocStatus.Approved &&
                                          i.ID == item.SourceId)
                              .FirstOrDefault();
                    if (result != null)
                    {
                        if (result.APPROVER5_ID == 0)
                        {
                            item.DocStatus = item.DocStatus;

                            if (item.Status == DocStatus.Approved || item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                                bolClosed = true;
                        }

                        if (item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                            bolClosed = true;

                        EamisForApprovalDTO temp = new EamisForApprovalDTO
                        {
                            Id = item.SourceId,
                            DocStatus = bolClosed == true ? item.Status : item.DocStatus,
                            TimeStamp = result.TIMESTAMP,
                            TransactionNumber = result.TRANSACTION_NUMBER,
                            TransactionType = result.TRANSACTION_TYPE,
                            Approver1Id = result.APPROVER1_ID,
                            Approver1Status = result.APPROVER1_STATUS,
                            Approver1Trandate = result.APPROVER1_TRANDATE,
                            Approver1RejectedReason = result.APPROVER1_REJECTEDREASON,

                            Approver2Id = result.APPROVER2_ID,
                            Approver2Status = result.APPROVER2_STATUS,
                            Approver2Trandate = result.APPROVER2_TRANDATE,
                            Approver2RejectedReason = result.APPROVER2_REJECTEDREASON,

                            Approver3Id = result.APPROVER3_ID,
                            Approver3Status = result.APPROVER3_STATUS,
                            Approver3Trandate = result.APPROVER3_TRANDATE,
                            Approver3RejectedReason = result.APPROVER3_REJECTEDREASON,

                            Approver4Id = item.UserId,
                            Approver4Status = item.Status,
                            Approver4Trandate = item.Trandate,
                            Approver4RejectedReason = item.RejectedReason,

                            Approver5Id = result.APPROVER5_ID,
                            Approver5Status = result.APPROVER5_STATUS,
                            Approver5Trandate = result.APPROVER5_TRANDATE,
                            Approver5RejectedReason = result.APPROVER5_REJECTEDREASON,
                        };
                        EAMISFORAPPROVAL data = MapToEntity(temp);
                        _ctx.Entry(data).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                }
                else if (item.ApprovalLevel == 5)
                {
                    var result = _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER5_ID == item.UserId &&
                                          i.TRANSACTION_TYPE == item.TransactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER4_STATUS == DocStatus.Approved &&
                                          i.APPROVER5_STATUS == DocStatus.ForApproval &&
                                          i.ID == item.SourceId)
                              .FirstOrDefault();
                    if (result != null)
                    {
                        if (item.Status == DocStatus.Approved || item.Status == DocStatus.Cancelled || item.Status == DocStatus.DisApproved)
                            bolClosed = true;

                        EamisForApprovalDTO temp = new EamisForApprovalDTO
                        {
                            Id = item.SourceId,
                            DocStatus = bolClosed == true ? item.Status : item.DocStatus,
                            TimeStamp = result.TIMESTAMP,
                            TransactionNumber = result.TRANSACTION_NUMBER,
                            TransactionType = result.TRANSACTION_TYPE,
                            Approver1Id = result.APPROVER1_ID,
                            Approver1Status = result.APPROVER1_STATUS,
                            Approver1Trandate = result.APPROVER1_TRANDATE,
                            Approver1RejectedReason = result.APPROVER1_REJECTEDREASON,

                            Approver2Id = result.APPROVER2_ID,
                            Approver2Status = result.APPROVER2_STATUS,
                            Approver2Trandate = result.APPROVER2_TRANDATE,
                            Approver2RejectedReason = result.APPROVER2_REJECTEDREASON,

                            Approver3Id = result.APPROVER3_ID,
                            Approver3Status = result.APPROVER3_STATUS,
                            Approver3Trandate = result.APPROVER3_TRANDATE,
                            Approver3RejectedReason = result.APPROVER3_REJECTEDREASON,

                            Approver4Id = result.APPROVER4_ID,
                            Approver4Status = result.APPROVER4_STATUS,
                            Approver4Trandate = result.APPROVER4_TRANDATE,
                            Approver4RejectedReason = result.APPROVER4_REJECTEDREASON,

                            Approver5Id = item.UserId,
                            Approver5Status = item.Status,
                            Approver5Trandate = item.Trandate,
                            Approver5RejectedReason = item.RejectedReason
                        };

                        EAMISFORAPPROVAL data = MapToEntity(temp);
                        _ctx.Entry(data).State = EntityState.Modified;
                        await _ctx.SaveChangesAsync();
                    }
                }
                //if there are no remaining approvals and/or underlying record status is not for approval
                if (bolClosed)
                {
                    //Update Transaction table (Delivery and Property Transaction)
              
                    switch (item.TransactionType)
                    {
                        case TransactionTypeSettings.DeliveryReceipt:
                            await UpdateDeliveryReceipt(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.PropertyReceiving:
                            await UpdatePropertyTransaction(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.PropertyTransfer:
                            await UpdatePropertyTransaction(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.IssuanceProperties:
                            await UpdatePropertyTransaction(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.IssuanceMaterials:
                            await UpdatePropertyTransaction(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.PropertyDisposal:
                            await UpdatePropertyTransaction(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.PropertyRevaluation:
                            await UpdatePropertyRevaluation(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                        case TransactionTypeSettings.ServiceLog:
                            await UpdateServiceLog(item.TransactionNumber, item.DocStatus, item.Status);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        private async Task UpdateDeliveryReceipt(string transactionNumber, string docStatus, string newStatus)
        {
            try
            {
                EamisDeliveryReceiptDTO data = _ctx.EAMIS_DELIVERY_RECEIPT
                                                .Where(d => d.TRANSACTION_TYPE == transactionNumber &&
                                                            d.TRANSACTION_STATUS == docStatus)
                                                .Select(x => new EamisDeliveryReceiptDTO
                                                {
                                                    Id = x.ID,
                                                    TransactionType = x.TRANSACTION_TYPE,
                                                    ReceivedBy = x.RECEIVED_BY,
                                                    DateReceived = x.DATE_RECEIVED,
                                                    SupplierId = x.SUPPLIER_ID,
                                                    PurchaseOrderNumber = x.PURCHASE_ORDER_NUMBER,
                                                    PurchaseOrderDate = x.PURCHASE_ORDER_DATE,
                                                    PurchaseRequestNumber = x.PURCHASE_REQUEST_NUMBER,
                                                    PurchaseRequestDate = x.PURCHASE_REQUEST_DATE,
                                                    SaleInvoiceNumber = x.SALE_INVOICE_NUMBER,
                                                    SaleInvoiceDate = x.SALE_INVOICE_DATE,
                                                    TotalAmount = x.TOTAL_AMOUNT,
                                                    TransactionStatus = x.TRANSACTION_STATUS,
                                                    StockroomId = x.WAREHOUSE_ID,
                                                    DRNumFrSupplier = x.DR_BY_SUPPLIER_NUMBER,
                                                    DRDate = x.DR_BY_SUPPLIER_DATE,
                                                    AprDate = x.APR_DATE,
                                                    AprNum = x.APR_NUMBER,
                                                    UserStamp = x.USER_STAMP,
                                                    BranchID = x.BRANCH_ID
                                                })
                                                .FirstOrDefault();
                if (newStatus == DocStatus.Approved) 
                { 
                    List<EamisDeliveryReceiptDetailsDTO> details = _ctx.EAMIS_DELIVERY_RECEIPT_DETAILS
                            .Where(d => d.DELIVERY_RECEIPT_ID == data.Id)
                            .Select(x => new EamisDeliveryReceiptDetailsDTO
                            {
                                Id = x.ID,
                                DeliveryReceiptId = x.DELIVERY_RECEIPT_ID,
                                ItemId = x.ITEM_ID,
                                QtyReceived = x.QTY_RECEIVED
                            })
                            .ToList();
                    foreach (var item in details)
                    {
                        await _eamisPropertyTransactionDetailsRepository.UpdatePropertyItemQty(item);
                    }
                }
                if (data != null)
                {
                    EAMISDELIVERYRECEIPT value = new EAMISDELIVERYRECEIPT
                    {
                        ID = data.Id,
                        TRANSACTION_TYPE = data.TransactionType,
                        TRANSACTION_STATUS = newStatus,
                        RECEIVED_BY = data.ReceivedBy,
                        DATE_RECEIVED = data.DateReceived,
                        SUPPLIER_ID = data.SupplierId,
                        PURCHASE_ORDER_NUMBER = data.PurchaseOrderNumber,
                        PURCHASE_ORDER_DATE = data.PurchaseOrderDate,
                        PURCHASE_REQUEST_DATE = data.PurchaseRequestDate,
                        PURCHASE_REQUEST_NUMBER = data.PurchaseRequestNumber,
                        SALE_INVOICE_NUMBER = data.SaleInvoiceNumber,
                        SALE_INVOICE_DATE = data.SaleInvoiceDate,
                        TOTAL_AMOUNT = data.TotalAmount,
                        DR_BY_SUPPLIER_NUMBER = data.DRNumFrSupplier,
                        DR_BY_SUPPLIER_DATE = data.DRDate,
                        WAREHOUSE_ID = data.StockroomId,
                        APR_NUMBER = data.AprNum,
                        APR_DATE = data.AprDate,
                        BRANCH_ID = data.BranchID
                    };
                    _ctx.Entry(value).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
        }
        private async Task UpdatePropertyTransaction(string transactionNumber, string docStatus, string newStatus)
        {
            try
            {
                EamisPropertyTransactionDTO data = _ctx.EAMIS_PROPERTY_TRANSACTION
                                                .Where(d => d.TRANSACTION_NUMBER == transactionNumber &&
                                                            d.TRANSACTION_STATUS == docStatus)
                                                .Select(x => new EamisPropertyTransactionDTO
                                                {
                                                    Id = x.ID,
                                                    TransactionNumber = x.TRANSACTION_NUMBER,
                                                    TransactionDate = x.TRANSACTION_DATE,
                                                    FiscalPeriod = x.FISCALPERIOD,
                                                    TransactionType = x.TRANSACTION_TYPE,
                                                    Memo = x.MEMO,
                                                    ReceivedBy = x.RECEIVED_BY,
                                                    ApprovedBy = x.APPROVED_BY,
                                                    DeliveryDate = x.DELIVERY_DATE,
                                                    UserStamp = x.USER_STAMP,
                                                    TransactionStatus = x.TRANSACTION_STATUS,
                                                    FundSource = x.FUND_SOURCE,
                                                    IsProperty = x.IS_PROPERTY,
                                                    TranType = x.TRAN_TYPE,
                                                    BranchID = x.BRANCH_ID
                                                })
                                                .FirstOrDefault();
                if (data.TransactionType == TransactionTypeSettings.IssuanceMaterials )
                { 
                    if (newStatus == DocStatus.Approved)
                    {
                        List<EamisPropertyTransactionDetailsDTO> details = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                .Where(d => d.PROPERTY_TRANS_ID == data.Id)
                                .Select(x => new EamisPropertyTransactionDetailsDTO
                                {
                                    Id = x.ID,
                                    PropertyTransactionID = x.PROPERTY_TRANS_ID,
                                    ItemCode = x.ITEM_CODE,
                                    Qty = x.QTY,
                                    UnitCost = x.UNIT_COST,
                                })
                                .ToList();
                        foreach (var item in details)
                        {
                            await _eamisPropertyTransactionDetailsRepository.UpdateIssuedPropertyItemQty(item);
                        }
                    }
                }
                if (data.TransactionType == TransactionTypeSettings.IssuanceProperties)
                {
                    if (newStatus == DocStatus.Approved)
                    {
                        List<EamisPropertyTransactionDetailsDTO> details = _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS
                                .Where(d => d.PROPERTY_TRANS_ID == data.Id)
                                .Select(x => new EamisPropertyTransactionDetailsDTO
                                {
                                    Id = x.ID,
                                    PropertyTransactionID = x.PROPERTY_TRANS_ID,
                                    ItemCode = x.ITEM_CODE,
                                    Qty = x.QTY,
                                    UnitCost = x.UNIT_COST,
                                    transactionDetailId = x.REFERENCE_ID
                                })
                                .ToList();
                        foreach (var item in details)
                        {
                            await _eamisPropertyTransactionDetailsRepository.UpdateIssuedPropertyItemQty(item);
                        }
                    }
                }
                if (data != null)
                {
                    EAMISPROPERTYTRANSACTION value = new EAMISPROPERTYTRANSACTION
                    {
                        ID = data.Id,
                        TRANSACTION_NUMBER = data.TransactionNumber,
                        TRAN_TYPE = data.TranType,
                        TRANSACTION_DATE = data.TransactionDate,
                        FISCALPERIOD = data.FiscalPeriod,
                        FUND_SOURCE = data.FundSource,
                        TRANSACTION_TYPE = data.TransactionType,
                        MEMO = data.Memo,
                        RECEIVED_BY = data.ReceivedBy,
                        APPROVED_BY = data.ApprovedBy,
                        DELIVERY_DATE = data.DeliveryDate,
                        USER_STAMP = data.UserStamp,
                        TRANSACTION_STATUS = newStatus,
                        IS_PROPERTY = data.IsProperty,
                        BRANCH_ID = data.BranchID
                    };
                    _ctx.Entry(value).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
        }

        private async Task UpdatePropertyRevaluation(string transactionNumber, string docStatus, string newStatus)
        {
            try
            {
                EamisPropertyRevaluationDTO data = _ctx.EAMIS_PROPERTY_REVALUATION
                                                .Where(d => d.TRAN_ID == transactionNumber)
                                                .Select(x => new EamisPropertyRevaluationDTO
                                                {
                                                    Id = x.ID,
                                                    IsActive = x.IS_ACTIVE,
                                                    Particulars = x.PARTICULARS,
                                                    TransactionDate = x.TRAN_DATE,
                                                    TransactionId = x.TRAN_ID,
                                                    UserStamp = x.USER_STAMP,
                                                    Status = x.STATUS,
                                                    BranchID = x.BRANCH_ID
                                                })
                                                .FirstOrDefault();
                if (data != null)
                {
                    EAMISPROPERTYREVALUATION value = new EAMISPROPERTYREVALUATION
                    {
                        ID = data.Id,
                        IS_ACTIVE = data.IsActive,
                        PARTICULARS = data.Particulars,
                        TRAN_ID = data.TransactionId,
                        TRAN_DATE = data.TransactionDate,
                        USER_STAMP = data.UserStamp,
                        BRANCH_ID = data.BranchID,
                        STATUS = newStatus
                    };
                    _ctx.Entry(value).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
        }
        private async Task UpdateServiceLog(string transactionNumber, string docStatus, string newStatus)
        {
            try
            {
                EamisServiceLogDTO data = _ctx.EAMIS_SERVICE_LOG
                                                .Where(d => d.TRAN_ID == transactionNumber
                                                 && d.TRANSACTION_STATUS == docStatus)
                                                .Select(x => new EamisServiceLogDTO
                                                {
                                                    Id = x.ID,
                                                    TransactionStatus = x.TRANSACTION_STATUS,
                                                    ServiceLogType = x.SERVICE_LOG_TYPE,
                                                    TransactionDate = x.TRAN_DATE,
                                                    TransactionId = x.TRAN_ID,
                                                    UserStamp = x.USER_STAMP,
                                                    BranchID = x.BRANCH_ID
                                                })
                                                .FirstOrDefault();
                if (data != null)
                {
                    EAMISSERVICELOG value = new EAMISSERVICELOG
                    {
                        ID = data.Id,
                        TRANSACTION_STATUS = newStatus,
                        TRAN_ID = data.TransactionId,
                        SERVICE_LOG_TYPE = data.ServiceLogType,
                        TRAN_DATE = data.TransactionDate,
                        USER_STAMP = data.UserStamp,
                        BRANCH_ID = data.BranchID
                    };
                    _ctx.Entry(value).State = EntityState.Modified;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
        }
        public async Task<List<MyApprovalListDTO>> MyApprovalList(int userId, string transactionType)
        {
            try
            {
                List<MyApprovalListDTO> lstResult = new List<MyApprovalListDTO>();
                MyApprovalListDTO objResult;
                var firstApproval = await FirstApproverList(userId, transactionType);
                foreach (var item in firstApproval)
                {
                    objResult = new MyApprovalListDTO();
                    objResult.SourceId = item.Id;
                    objResult.UserId = item.Approver1Id;
                    objResult.TransactionNumber = item.TransactionNumber;
                    objResult.TransactionType = item.TransactionType;
                    objResult.DocStatus = item.DocStatus;
                    objResult.ApprovalLevel = 1;
                    lstResult.Add(objResult);
                }
                var secondApproval = await SecondApproverList(userId, transactionType);
                foreach (var item in secondApproval)
                {
                    objResult = new MyApprovalListDTO();
                    objResult.SourceId = item.Id;
                    objResult.UserId = item.Approver2Id;
                    objResult.TransactionNumber = item.TransactionNumber;
                    objResult.TransactionType = item.TransactionType;
                    objResult.DocStatus = item.DocStatus;
                    objResult.ApprovalLevel = 2;
                    lstResult.Add(objResult);
                }
                var thirdApproval = await ThirdApproverList(userId, transactionType);
                foreach (var item in thirdApproval)
                {
                    objResult = new MyApprovalListDTO();
                    objResult.SourceId = item.Id;
                    objResult.UserId = item.Approver3Id;
                    objResult.TransactionNumber = item.TransactionNumber;
                    objResult.TransactionType = item.TransactionType;
                    objResult.DocStatus = item.DocStatus;
                    objResult.ApprovalLevel = 3;
                    lstResult.Add(objResult);
                }
                var fourthApproval = await FourthApproverList(userId, transactionType);
                foreach (var item in fourthApproval)
                {
                    objResult = new MyApprovalListDTO();
                    objResult.SourceId = item.Id;
                    objResult.UserId = item.Approver4Id;
                    objResult.TransactionNumber = item.TransactionNumber;
                    objResult.TransactionType = item.TransactionType;
                    objResult.DocStatus = item.DocStatus;
                    objResult.ApprovalLevel = 4;
                    lstResult.Add(objResult);
                }
                var fifthApproval = await FifthApproverList(userId, transactionType);
                foreach (var item in fifthApproval)
                {
                    objResult = new MyApprovalListDTO();
                    objResult.SourceId = item.Id;
                    objResult.UserId = item.Approver5Id;
                    objResult.TransactionNumber = item.TransactionNumber;
                    objResult.TransactionType = item.TransactionType;
                    objResult.DocStatus = item.DocStatus;
                    objResult.ApprovalLevel = 5;
                    lstResult.Add(objResult);
                }
                return lstResult;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return null;
        }

        public async Task<List<EamisForApprovalDTO>> FirstApproverList(int userId, string transactionType)
        {
            if (transactionType.ToLower() == DocStatus.AllStatus.ToLower())
            {
                var AllResult = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                             .Where(i => i.APPROVER1_ID == userId &&
                                         i.DOCSTATUS == DocStatus.ForApproval &&
                                         i.APPROVER1_STATUS == DocStatus.ForApproval)
                             .Select(d => new EamisForApprovalDTO
                             {
                                 Id = d.ID,
                                 DocStatus = d.DOCSTATUS,
                                 TransactionNumber = d.TRANSACTION_NUMBER,
                                 TransactionType = d.TRANSACTION_TYPE,
                                 Approver1Id = userId
                             })
                             .ToList()).ConfigureAwait(false);
                return AllResult;
            }
            var result = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                             .Where(i => i.APPROVER1_ID == userId &&
                                         i.TRANSACTION_TYPE == transactionType &&
                                         i.DOCSTATUS == DocStatus.ForApproval &&
                                         i.APPROVER1_STATUS == DocStatus.ForApproval
                                         )
                             .Select(d => new EamisForApprovalDTO
                             {
                                 Id = d.ID,
                                 DocStatus = d.DOCSTATUS,
                                 TransactionNumber = d.TRANSACTION_NUMBER,
                                 TransactionType = d.TRANSACTION_TYPE,
                                 Approver1Id = userId
                             })
                             .ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<List<EamisForApprovalDTO>> SecondApproverList(int userId, string transactionType)
        {
            if (transactionType.ToLower() == DocStatus.AllStatus.ToLower())
            {
                var AllResult = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER2_ID == userId &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER1_STATUS == DocStatus.Approved &&
                                         i.APPROVER2_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver2Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
                return AllResult;
            }
            var result = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER2_ID == userId &&
                                          i.TRANSACTION_TYPE == transactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER1_STATUS == DocStatus.Approved &&
                                         i.APPROVER2_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver2Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<List<EamisForApprovalDTO>> ThirdApproverList(int userId, string transactionType)
        {
            if (transactionType.ToLower() == DocStatus.AllStatus.ToLower())
            {
                var AllResult = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER3_ID == userId &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER2_STATUS == DocStatus.Approved &&
                                         i.APPROVER3_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver3Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
                return AllResult;
            }
            var result = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER3_ID == userId &&
                                          i.TRANSACTION_TYPE == transactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER2_STATUS == DocStatus.Approved &&
                                         i.APPROVER3_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver3Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<List<EamisForApprovalDTO>> FourthApproverList(int userId, string transactionType)
        {
            if (transactionType.ToLower() == DocStatus.AllStatus.ToLower())
            {
                var AllResult = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER4_ID == userId &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER3_STATUS == DocStatus.Approved &&
                                         i.APPROVER4_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver4Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
                return AllResult;
            }
            var result = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER4_ID == userId &&
                                          i.TRANSACTION_TYPE == transactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER3_STATUS == DocStatus.Approved &&
                                         i.APPROVER4_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver4Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<List<EamisForApprovalDTO>> FifthApproverList(int userId, string transactionType)
        {
            if (transactionType.ToLower() == DocStatus.AllStatus.ToLower())
            {
                var AllResult = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER5_ID == userId &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER4_STATUS == DocStatus.Approved &&
                                         i.APPROVER5_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver5Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
                return AllResult;
            }
            var result = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
                              .Where(i => i.APPROVER5_ID == userId &&
                                          i.TRANSACTION_TYPE == transactionType &&
                                          i.DOCSTATUS == DocStatus.ForApproval &&
                                          i.APPROVER4_STATUS == DocStatus.Approved &&
                                         i.APPROVER5_STATUS == DocStatus.ForApproval)
                              .Select(d => new EamisForApprovalDTO
                              {
                                  Id = d.ID,
                                  DocStatus = d.DOCSTATUS,
                                  TransactionNumber = d.TRANSACTION_NUMBER,
                                  TransactionType = d.TRANSACTION_TYPE,
                                  Approver5Id = userId
                              })
                              .ToList()).ConfigureAwait(false);
            return result;
        }

        public async Task<DataList<EamisForApprovalDTO>> List(EamisForApprovalDTO filter, PageConfig config)
        {
            IQueryable<EAMISFORAPPROVAL> query = FilteredEntities(filter);
            bool resolves_isAscending = (config.IsAscending) ? config.IsAscending : false;

            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;
            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisForApprovalDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisForApprovalDTO> QueryToDTO(IQueryable<EAMISFORAPPROVAL> query)
        {
            return query.Select(x => new EamisForApprovalDTO
            {
                Id = x.ID,
                TransactionType = x.TRANSACTION_TYPE,
                DocStatus = x.DOCSTATUS,
                TransactionNumber = x.TRANSACTION_NUMBER,
                TimeStamp = x.TIMESTAMP,
                Approver1Id = x.APPROVER1_ID,
                Approver1Status = x.APPROVER1_STATUS,
                Approver1Trandate = x.APPROVER1_TRANDATE,
                Approver1RejectedReason = x.APPROVER1_REJECTEDREASON,
                Approver2Id = x.APPROVER2_ID,
                Approver2Status = x.APPROVER2_STATUS,
                Approver2Trandate = x.APPROVER2_TRANDATE,
                Approver2RejectedReason = x.APPROVER2_REJECTEDREASON,
                Approver3Id = x.APPROVER3_ID,
                Approver3Status = x.APPROVER3_STATUS,
                Approver3Trandate = x.APPROVER3_TRANDATE,
                Approver3RejectedReason = x.APPROVER3_REJECTEDREASON,
                Approver4Id = x.APPROVER4_ID,
                Approver4Status = x.APPROVER4_STATUS,
                Approver4Trandate = x.APPROVER4_TRANDATE,
                Approver4RejectedReason = x.APPROVER4_REJECTEDREASON,
                Approver5Id = x.APPROVER5_ID,
                Approver5Status = x.APPROVER5_STATUS,
                Approver5Trandate = x.APPROVER5_TRANDATE,
                Approver5RejectedReason = x.APPROVER5_REJECTEDREASON
            });
        }

        private IQueryable<EAMISFORAPPROVAL> FilteredEntities(EamisForApprovalDTO filter, IQueryable<EAMISFORAPPROVAL> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISFORAPPROVAL>(true);
            if (filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.TransactionType != null && !string.IsNullOrEmpty(filter.TransactionType))
                predicate = predicate.And(x => x.TRANSACTION_TYPE == filter.TransactionType);
            if (filter.DocStatus != null && !string.IsNullOrEmpty(filter.DocStatus))
                predicate = predicate.And(x => x.DOCSTATUS == filter.DocStatus);
            if (filter.TransactionNumber != null && !string.IsNullOrEmpty(filter.TransactionNumber))
                predicate = predicate.And(x => x.TRANSACTION_NUMBER == filter.TransactionNumber);
            if (filter.Approver1Id != 0)
                predicate = predicate.And(x => x.APPROVER1_ID == filter.Approver1Id);
            if (filter.Approver1Status != null && !string.IsNullOrEmpty(filter.Approver1Status))
                predicate = predicate.And(x => x.APPROVER1_STATUS == filter.Approver1Status);

            if (filter.Approver2Id != 0)
                predicate = predicate.And(x => x.APPROVER2_ID == filter.Approver2Id);
            if (filter.Approver2Status != null && !string.IsNullOrEmpty(filter.Approver2Status))
                predicate = predicate.And(x => x.APPROVER2_STATUS == filter.Approver2Status);

            if (filter.Approver3Id != 0)
                predicate = predicate.And(x => x.APPROVER3_ID == filter.Approver3Id);
            if (filter.Approver3Status != null && !string.IsNullOrEmpty(filter.Approver3Status))
                predicate = predicate.And(x => x.APPROVER3_STATUS == filter.Approver3Status);

            var query = custom_query ?? _ctx.EAMIS_FOR_APPROVAL;
            return query.Where(predicate);
        }

        public IQueryable<EAMISFORAPPROVAL> PagedQuery(IQueryable<EAMISFORAPPROVAL> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        public async Task<EamisForApprovalDTO> Insert(EamisForApprovalDTO item, decimal totalAmount)
        {
            try
            {
                //var result = _ctx.EAMIS_APPROVAL_SETUP_DETAILS
                //                 .Where(i => i.APPROVALSETUP_ID ==
                //                               (_ctx.EAMIS_APPROVAL_SETUP
                //                                    .Where(m => m.MODULE_NAME == item.TransactionType)
                //                                    .Select(v => v.ID).FirstOrDefault())
                //                              && i.MAX_AMOUNT >= totalAmount
                //                              && i.MIN_AMOUNT <= totalAmount)
                //                 .ToList();
                //if (result != null)
                //{
                //    foreach (var setup in result)
                //    {
                //        if (setup.VIEW_LEVEL == 1)
                //        {
                //            item.Approver1Id = setup.SIGNATORY_ID;
                //            item.Approver1Status = item.DocStatus;
                //        }
                //        if (setup.VIEW_LEVEL == 2)
                //        {
                //            item.Approver2Id = setup.SIGNATORY_ID;
                //            item.Approver2Status = item.DocStatus;
                //        }
                //        if (setup.VIEW_LEVEL == 3)
                //        {
                //            item.Approver3Id = setup.SIGNATORY_ID;
                //            item.Approver3Status = item.DocStatus;
                //        }
                //        if (setup.VIEW_LEVEL == 4)
                //        {
                //            item.Approver4Id = setup.SIGNATORY_ID;
                //            item.Approver4Status = item.DocStatus;
                //        }
                //        if (setup.VIEW_LEVEL == 5)
                //        {
                //            item.Approver5Id = setup.SIGNATORY_ID;
                //            item.Approver5Status = item.DocStatus;
                //        }
                //    }
                //}

                EAMISFORAPPROVAL data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                await _ctx.SaveChangesAsync();
                item.Id = data.ID;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        private EAMISFORAPPROVAL MapToEntity(EamisForApprovalDTO item)
        {
            if (item == null) return new EAMISFORAPPROVAL();
            return new EAMISFORAPPROVAL
            {
                ID = item.Id,
                TRANSACTION_TYPE = item.TransactionType,
                TRANSACTION_NUMBER = item.TransactionNumber,
                TIMESTAMP = item.TimeStamp,
                DOCSTATUS = item.DocStatus,
                APPROVER1_ID = item.Approver1Id,
                APPROVER1_STATUS = item.Approver1Status,
                APPROVER1_TRANDATE = item.Approver1Trandate,
                APPROVER1_REJECTEDREASON = item.Approver1RejectedReason,
                APPROVER2_ID = item.Approver2Id,
                APPROVER2_STATUS = item.Approver2Status,
                APPROVER2_TRANDATE = item.Approver2Trandate,
                APPROVER2_REJECTEDREASON = item.Approver2RejectedReason,
                APPROVER3_ID = item.Approver3Id,
                APPROVER3_STATUS = item.Approver3Status,
                APPROVER3_TRANDATE = item.Approver3Trandate,
                APPROVER3_REJECTEDREASON = item.Approver3RejectedReason,
                APPROVER4_ID = item.Approver4Id,
                APPROVER4_STATUS = item.Approver4Status,
                APPROVER4_TRANDATE = item.Approver4Trandate,
                APPROVER4_REJECTEDREASON = item.Approver4RejectedReason,
                APPROVER5_ID = item.Approver5Id,
                APPROVER5_STATUS = item.Approver5Status,
                APPROVER5_TRANDATE = item.Approver5Trandate,
                APPROVER5_REJECTEDREASON = item.Approver5RejectedReason,
            };
        }

        public async Task<EamisForApprovalDTO> Update(EamisForApprovalDTO item)
        {
            try
            {
                EAMISFORAPPROVAL data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Modified;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }

        public async Task<EamisForApprovalDTO> Delete(EamisForApprovalDTO item)
        {
            try
            {
                EAMISFORAPPROVAL data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Deleted;
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.Message;
            }
            return item;
        }
        public async Task<EamisForApprovalDTO> getForApprovalStatus(string transactionNumber)
        {
            var result = await Task.Run(() => _ctx.EAMIS_FOR_APPROVAL.AsNoTracking()
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync(x => x.TRANSACTION_NUMBER == transactionNumber)).ConfigureAwait(false);

            if (result != null)
            {
                return new EamisForApprovalDTO
                {
                    Id = result.ID,
                    TransactionType = result.TRANSACTION_TYPE,
                    TransactionNumber = result.TRANSACTION_NUMBER,
                    Approver1Id = result.APPROVER1_ID,
                    Approver1Status = result.APPROVER1_STATUS,
                    Approver1RejectedReason = result.APPROVER1_REJECTEDREASON,
                    Approver2Id = result.APPROVER2_ID,
                    Approver2Status = result.APPROVER2_STATUS,
                    Approver2RejectedReason = result.APPROVER2_REJECTEDREASON,
                    Approver3Id = result.APPROVER3_ID,
                    Approver3Status = result.APPROVER3_STATUS,
                    Approver3RejectedReason = result.APPROVER3_REJECTEDREASON,
                    Approver4Id = result.APPROVER4_ID,
                    Approver4Status = result.APPROVER4_STATUS,
                    Approver4RejectedReason = result.APPROVER4_REJECTEDREASON,
                    Approver5Id = result.APPROVER5_ID,
                    Approver5Status = result.APPROVER5_STATUS,
                    Approver5RejectedReason = result.APPROVER5_REJECTEDREASON,
                    Approver1Trandate = result.APPROVER1_TRANDATE,
                    Approver2Trandate = result.APPROVER2_TRANDATE,
                    Approver3Trandate = result.APPROVER3_TRANDATE,
                    Approver4Trandate = result.APPROVER4_TRANDATE,
                    Approver5Trandate = result.APPROVER5_TRANDATE,
                };
            }
            return null;
        }
        public async Task<int> getTransactionID(string transactionNumber)
        {
            int retValue = 0;
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION.Where(s => s.TRANSACTION_NUMBER == transactionNumber).AsNoTracking().ToList()).ConfigureAwait(false);
            retValue = result[0].ID;
            return retValue;
        }
        public async Task<int> getDeliveryID(string transactionType)
        {
            int retValue = 0;
            var result = await Task.Run(() => _ctx.EAMIS_DELIVERY_RECEIPT.Where(s => s.TRANSACTION_TYPE == transactionType).AsNoTracking().ToList()).ConfigureAwait(false);

            retValue = result[0].ID;
            return retValue;
        }
        public async Task<int> getServiceLogID(string tranID)
        {
            int retValue = 0;
            var result = await Task.Run(() => _ctx.EAMIS_SERVICE_LOG.Where(s => s.TRAN_ID == tranID).AsNoTracking().ToList()).ConfigureAwait(false);

            retValue = result[0].ID;
            return retValue;
        }
        public async Task<int> getRevaluationID(string tranID)
        {
            int retValue = 0;
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_REVALUATION.Where(s => s.TRAN_ID == tranID).AsNoTracking().ToList()).ConfigureAwait(false);

            retValue = result[0].ID;
            return retValue;
        }
    }
}
