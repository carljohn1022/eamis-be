using EAMIS.Common.DTO.Inventory_Taking;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Common.DTO.Transaction;
using EAMIS.Core.ContractRepository.Inventory_Taking;
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

namespace EAMIS.Core.LogicRepository.Inventory_Taking
{
    public class EamisInventoryTakingRepository : IEamisInventoryTakingRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;

        public EamisInventoryTakingRepository(EAMISContext ctx)
        {      
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        private EAMISINVENTORYTAKING MapToEntity(EamisInventoryTakingDTO item)
        {
            if (item == null) return new EAMISINVENTORYTAKING();
            return new EAMISINVENTORYTAKING
            {
                PROPERTY_NUMBER = item.PropertyNumber,
                SERIAL_NUMBER = item.SerialNumber,
                OFFICE = item.Office,
                DEPARTMENT = item.Department,
                ACQUISITION_DATE = item.AcquisitionDate,
                REMARKS = item.Remarks,
                USER_STAMP = item.UserStamp
            };
        }
        public async Task<EamisInventoryTakingDTO> getScannedPropertyNumberList(string propertyNumber, string office, string department, string userStamp)
        {
            var result = await Task.Run(() => _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking()
            .OrderByDescending(x => x.ID)
            .FirstOrDefaultAsync(x => x.PROPERTY_NUMBER == propertyNumber)).ConfigureAwait(false);

            if (result == null)
            {
                throw new Exception("Result is null.");
            }

            string remarks = "";
            if (result.OFFICE == office && result.DEPARTMENT == department)
            {
                remarks = "FOUND IN STATION";
            }
            else
            {
                remarks = "INCORRECT LOCATION/SHOULD BE IN " + result.OFFICE + " - " + result.DEPARTMENT;
            }

            var dto = new EamisInventoryTakingDTO
            {
                PropertyNumber = result.PROPERTY_NUMBER,
                SerialNumber = result.SERIAL_NUMBER,
                Office = office,
                Department = department,
                AcquisitionDate = result.ACQUISITION_DATE,
                Remarks = remarks,
                UserStamp = userStamp
            };

            // Map DTO to entity
            var entity = MapToEntity(dto);

            // Insert entity into table
            _ctx.Entry(entity).State = EntityState.Added;
            await _ctx.SaveChangesAsync();

            return dto;
        }
        private IQueryable<EAMISINVENTORYTAKING> FilteredEntities(EamisInventoryTakingDTO filter, IQueryable<EAMISINVENTORYTAKING> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISINVENTORYTAKING>(true);
           
            if (!string.IsNullOrEmpty(filter.PropertyNumber)) predicate = (strict)
                   ? predicate.And(x => x.PROPERTY_NUMBER.ToLower() == filter.PropertyNumber.ToLower())
                   : predicate.And(x => x.PROPERTY_NUMBER.Contains(filter.PropertyNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.SerialNumber)) predicate = (strict)
                    ? predicate.And(x => x.SERIAL_NUMBER.ToLower() == filter.SerialNumber.ToLower())
                    : predicate.And(x => x.SERIAL_NUMBER.Contains(filter.SerialNumber.ToLower()));
            if (!string.IsNullOrEmpty(filter.Office)) predicate = (strict)
                   ? predicate.And(x => x.OFFICE.ToLower() == filter.Office.ToLower())
                   : predicate.And(x => x.OFFICE.Contains(filter.Office.ToLower()));
            if (!string.IsNullOrEmpty(filter.Department)) predicate = (strict)
                   ? predicate.And(x => x.DEPARTMENT.ToLower() == filter.Department.ToLower())
                   : predicate.And(x => x.DEPARTMENT.Contains(filter.Department.ToLower()));
            if (!string.IsNullOrEmpty(filter.Remarks)) predicate = (strict)
                   ? predicate.And(x => x.REMARKS.ToLower() == filter.Remarks.ToLower())
                   : predicate.And(x => x.REMARKS.Contains(filter.Remarks.ToLower()));

            var query = custom_query ?? _ctx.EAMIS_INVENTORY_TAKING;
            return query.Where(predicate);
        }

        public async Task<DataList<EamisInventoryTakingDTO>> List(EamisInventoryTakingDTO filter, PageConfig config)
        {
            IQueryable<EAMISINVENTORYTAKING> query = FilteredEntities(filter);
            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisInventoryTakingDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
        private IQueryable<EAMISINVENTORYTAKING> PagedQuery(IQueryable<EAMISINVENTORYTAKING> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EamisInventoryTakingDTO> QueryToDTO(IQueryable<EAMISINVENTORYTAKING> query)
        {
            return query.Select(x => new EamisInventoryTakingDTO
            {
                PropertyNumber = x.PROPERTY_NUMBER,
                SerialNumber = x.SERIAL_NUMBER,
                AcquisitionDate = x.ACQUISITION_DATE,
                Office = x.OFFICE,
                Department = x.DEPARTMENT,
                Remarks = x.REMARKS
            });
        }
        public Task<bool> CheckPropertyNumberExist(string propertyNumber)
        {
            return _ctx.EAMIS_PROPERTY_TRANSACTION_DETAILS.AsNoTracking().AnyAsync(x => x.PROPERTY_NUMBER == propertyNumber);
        }
        public Task<bool> CheckPropertyNumberExistIfScanned(string propertyNumber)
        {
            return _ctx.EAMIS_INVENTORY_TAKING.AsNoTracking().AnyAsync(x => x.PROPERTY_NUMBER == propertyNumber);
        }
        public async Task<List<EamisResponsibilityCenterDTO>> GetUnitDesc(bool isActive, string officeDesc)
        {
            var result = _ctx.EAMIS_RESPONSIBILITY_CENTER
                                          .Where(f => f.IS_ACTIVE == isActive && f.OFFICE_DESC == officeDesc )
                                          .GroupBy(x => x.UNIT_DESC)
                                          .Select(i => new EamisResponsibilityCenterDTO
                                          {
                                              unitDesc = i.Key
                                          })
                                          .ToList();

            return result;
        }
        public async Task<List<EamisResponsibilityCenterDTO>> GetOfficeDesc(bool isActive)
        {
            var result = _ctx.EAMIS_RESPONSIBILITY_CENTER
                                          .Where(f => f.IS_ACTIVE == isActive)
                                          .GroupBy(x => x.OFFICE_DESC)
                                          .Select(i => new EamisResponsibilityCenterDTO
                                          {
                                              officeDesc = i.Key
                                          })
                                          .ToList();

            return result;
        }
    }
}
