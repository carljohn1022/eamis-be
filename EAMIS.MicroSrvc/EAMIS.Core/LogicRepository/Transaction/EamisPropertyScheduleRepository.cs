﻿using EAMIS.Common.DTO.Transaction;
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

namespace EAMIS.Core.LogicRepository.Transaction
{
    public class EamisPropertyScheduleRepository : IEamisPropertyScheduleRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisPropertyScheduleRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
              : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<DataList<EamisPropertyScheduleDTO>> Search(string type, string searchValue)
        {
            IQueryable<EAMISPROPERTYSCHEDULE> query = null;
            if (type == "Serial Number")
            {
                query = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Where(x => x.SERIAL_NO.Contains(searchValue)).AsQueryable();
            }
            if (type == "Item Description")
            {
                query = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Where(x => x.ITEM_DESCRIPTION.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Item Category")
            {
                query = _ctx.EAMIS_PROPERTY_SCHEDULE.AsNoTracking().Where(x => x.CATEGORY.Contains(searchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisPropertyScheduleDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISPROPERTYSCHEDULE> PagedQueryForSearch(IQueryable<EAMISPROPERTYSCHEDULE> query)
        {
            return query;
        }
        public async Task<DataList<EamisPropertyScheduleDTO>> List(EamisPropertyScheduleDTO filter, PageConfig config)
        {
            IQueryable<EAMISPROPERTYSCHEDULE> query = FilteredEntites(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;


            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisPropertyScheduleDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }


        private IQueryable<EAMISPROPERTYSCHEDULE> FilteredEntites(EamisPropertyScheduleDTO filter, IQueryable<EAMISPROPERTYSCHEDULE> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISPROPERTYSCHEDULE>(true);

            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.ForDepreciation != null && filter.ForDepreciation != false)
                predicate = predicate.And(x => x.FOR_DEPRECIATION == filter.ForDepreciation);
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                     ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                     : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.ItemDescription)) predicate = (strict)
                     ? predicate.And(x => x.ITEM_DESCRIPTION.ToLower() == filter.ItemDescription.ToLower())
                     : predicate.And(x => x.ITEM_DESCRIPTION.Contains(filter.ItemDescription.ToLower()));
            if (!string.IsNullOrEmpty(filter.SerialNo)) predicate = (strict)
                     ? predicate.And(x => x.SERIAL_NO.ToLower() == filter.SerialNo.ToLower())
                     : predicate.And(x => x.SERIAL_NO.Contains(filter.SerialNo.ToLower()));
            if (filter.AcquisitionDate != null && filter.AcquisitionDate != DateTime.MinValue)
                predicate = predicate.And(x => x.ACQUISITION_DATE == filter.AcquisitionDate);

            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                     ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                     : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));
          
            if (filter.SalvageValue != null && filter.SalvageValue != 0)
                predicate = predicate.And(x => x.SALVAGE_VALUE == filter.SalvageValue);
            if (filter.BookValue != null && filter.BookValue != 0)
                predicate = predicate.And(x => x.BOOK_VALUE == filter.BookValue);
            if (filter.ESTLife != null && filter.ESTLife != 0)
                predicate = predicate.And(x => x.EST_LIFE == filter.ESTLife);
            if (filter.AreaSQM != null && filter.AreaSQM != 0)
                predicate = predicate.And(x => x.AREA_SQM == filter.AreaSQM);
            
            var query = custom_query ?? _ctx.EAMIS_PROPERTY_SCHEDULE;
            return query.Where(predicate);
        }

        private IQueryable<EAMISPROPERTYSCHEDULE> PagedQuery(IQueryable<EAMISPROPERTYSCHEDULE> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EamisPropertyScheduleDTO> QueryToDTO(IQueryable<EAMISPROPERTYSCHEDULE> query)
        {
            return query.Select(d => new EamisPropertyScheduleDTO
            {
                Id = d.ID,
                AcquisitionCost = d.ACQUISITION_COST,
                AcquisitionDate = d.ACQUISITION_DATE,
                Appraisalincrement = d.APPRAISAL_INCREMENT,
                AppraisedValue = d.APPRAISED_VALUE,
                AreaSQM = d.AREA_SQM,
                AssessedValue = d.ASSESSED_VALUE,
                AssetCondition = d.ASSET_CONDITION,
                AssetTag = d.ASSET_TAG,
                BookValue = d.BOOK_VALUE,
                Category = d.CATEGORY,
                CostCenter = d.COST_CENTER,
                Department = d.DEPARTMENT,
                DeprecAmount = d.DEPREC_AMOUNT,
                Details = d.DETAILS,
                DisposedAmount = d.DISPOSED_AMOUNT,
                ESTLife = d.EST_LIFE,
                ForDepreciation = d.FOR_DEPRECIATION,
                InvoiceNo = d.INVOICE_NO,
                ItemDescription = d.ITEM_DESCRIPTION,
                LastDepartment = d.LAST_DEPARTMENT,
                LastPostedDate = d.LAST_POSTED_DATE,
                Location = d.LOCATION,
                Names = d.NAMES,
                PORef = d.POREF,
                PropertyNumber = d.PROPERTY_NUMBER,
                RealEstateTaxPayment = d.REAL_ESTATE_TAX_PAYMENT,
                RevaluationCost = d.REVALUATION_COST,
                RRDate = d.RRDATE,
                RRRef = d.RRREF,
                SalvageValue = d.SALVAGE_VALUE,
                SerialNo = d.SERIAL_NO,
                Status = d.STATUS,
                SubCategory = d.SUB_CATEGORY,
                SvcAgreementNo = d.SVC_AGREEMENT_NO,
                VendorName = d.VENDORNAME,
                Warranty = d.WARRANTY,
                WarrantyDate = d.WARRANTY_DATE
            });
        }

        public async Task<EamisPropertyScheduleDTO> Update(EamisPropertyScheduleDTO item)
        {
            EAMISPROPERTYSCHEDULE schedule = new EAMISPROPERTYSCHEDULE
            {
                ID = item.Id,
                ACCUMULATED_DEPREC_AMT = item.AccumulatedDepreciationAmount,
                REMAINING_LIFE = item.RemainingLife,
                BOOK_VALUE = item.BookValue,
                ACQUISITION_COST = item.AcquisitionCost,
                ACQUISITION_DATE = item.AcquisitionDate,
                APPRAISAL_INCREMENT = item.Appraisalincrement,
                APPRAISED_VALUE = item.AppraisedValue,
                AREA_SQM = item.AreaSQM,
                ASSESSED_VALUE = item.AssessedValue,
                ASSET_CONDITION = item.AssetCondition,
                ASSET_TAG = item.AssetTag,
                CATEGORY = item.Category, //get from category, link to property item
                COST_CENTER = item.CostCenter,
                DEPARTMENT = item.Department,
                DEPREC_AMOUNT = item.DeprecAmount,
                DETAILS = item.Details,
                DISPOSED_AMOUNT = item.DisposedAmount,
                EST_LIFE = item.ESTLife,
                FOR_DEPRECIATION = item.ForDepreciation, //to do: include
                INVOICE_NO = item.InvoiceNo,
                ITEM_DESCRIPTION = item.ItemDescription,
                LAST_DEPARTMENT = item.LastDepartment,
                LAST_POSTED_DATE = item.LastPostedDate,
                LOCATION = item.Location,
                NAMES = item.Names,
                POREF = item.PORef,
                PROPERTY_NUMBER = item.PropertyNumber,
                REAL_ESTATE_TAX_PAYMENT = item.RealEstateTaxPayment,
                REVALUATION_COST = item.RevaluationCost,
                RRDATE = item.RRDate,
                RRREF = item.RRRef,
                SALVAGE_VALUE = item.SalvageValue, //calculated salvage valued must be included when inserting record to property schedule //salvage value = item.ACQUISITION_COST * salvageValue (85000 * 0.05)
                SERIAL_NO = item.SerialNo,
                STATUS = item.Status,
                SUB_CATEGORY = item.SubCategory, //get from sub category, link to property item/Category
                SVC_AGREEMENT_NO = item.SvcAgreementNo,
                VENDORNAME = item.VendorName,
                WARRANTY = item.Warranty, //item.WarrantyExpiry?
                WARRANTY_DATE = item.WarrantyDate,  //item.WarrantyExpiry?
                ITEM_CODE = item.ItemCode,
                REFERENCE_ID = item.ReferenceId
            };
            _ctx.Entry(schedule).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
