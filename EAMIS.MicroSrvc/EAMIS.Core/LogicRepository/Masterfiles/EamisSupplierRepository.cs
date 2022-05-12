using EAMIS.Common.DTO;
using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
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

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisSupplierRepository : IEamisSupplierRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        public EamisSupplierRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
                : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }

        public async Task<EamisSupplierDTO> Delete(EamisSupplierDTO item)
        {
            EAMISSUPPLIER data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISSUPPLIER MapToEntity(EamisSupplierDTO item)
        {
            if (item == null) return new EAMISSUPPLIER();
            return new EAMISSUPPLIER
            {
                ID = item.Id,
                ACCOUNT_NAME = item.AccountName,
                ACCOUNT_NUMBER = item.AccountNumber,
                BANK = item.Bank,
                BRANCH = item.Branch,
                COMPANY_DESCRIPTION = item.CompanyDescription,
                STREET = item.Street,
                COMPANY_NAME = item.CompanyName,
                CONTACT_PERSON_NAME = item.ContactPersonName,
                CONTACT_PERSON_NUMBER = item.ContactPersonNumber,
                IS_ACTIVE = item.IsActive,
                BRGY_CODE = item.BrgyCode,
                CITY_MUNICIPALITY_CODE = item.CityMunicipalityCode,
                PROVINCE_CODE = item.ProvinceCode,
                REGION_CODE = item.RegionCode
            };
        }

        public async Task<EamisSupplierDTO> Insert(EamisSupplierDTO item)
        {
            EAMISSUPPLIER data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisSupplierDTO>> List(EamisSupplierDTO filter, PageConfig config)
        {
            IQueryable<EAMISSUPPLIER> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisSupplierDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisSupplierDTO> QueryToDTO(IQueryable<EAMISSUPPLIER> query)
        {
            return query.Select(x => new EamisSupplierDTO
            {
                Id = x.ID,
                CompanyName = x.COMPANY_NAME,
                CompanyDescription = x.COMPANY_DESCRIPTION,
                RegionCode = x.REGION_CODE,
                ProvinceCode = x.PROVINCE_CODE,
                CityMunicipalityCode = x.CITY_MUNICIPALITY_CODE,
                BrgyCode = x.BRGY_CODE,
                Street = x.STREET,
                ContactPersonName = x.CONTACT_PERSON_NAME,
                ContactPersonNumber = x.CONTACT_PERSON_NUMBER,
                Bank = x.BANK,
                AccountName = x.ACCOUNT_NAME,
                AccountNumber = x.ACCOUNT_NUMBER,
                Branch = x.BRANCH,
                IsActive = x.IS_ACTIVE,
                Barangay = new Common.DTO.EamisBarangayDTO
                {
                    BrgyCode = x.BARANGAY_GROUP.BRGY_CODE,
                    BrgyDescription = x.BARANGAY_GROUP.BRGY_DESCRIPTION,
                    Municipality = new EamisMunicipalityDTO
                    {
                        CityMunicipalityCode = x.BARANGAY_GROUP.MUNICIPALITY.MUNICIPALITY_CODE,
                        CityMunicipalityDescription = x.BARANGAY_GROUP.MUNICIPALITY.CITY_MUNICIPALITY_DESCRIPTION
                    },
                    Province = new EamisProvinceDTO
                    {
                        ProvinceCode = x.BARANGAY_GROUP.PROVINCE.PROVINCE_CODE,
                        ProvinceDescription = x.BARANGAY_GROUP.PROVINCE.PROVINCE_DESCRITION
                    },
                    Region = new EamisRegionDTO
                    {
                        RegionCode = x.BARANGAY_GROUP.REGION.REGION_CODE,
                        RegionDescription = x.BARANGAY_GROUP.REGION.REGION_DESCRIPTION
                    }

                    
                    
                }
            });
        }

        private IQueryable<EAMISSUPPLIER> PagedQuery(IQueryable<EAMISSUPPLIER> query, int resolved_size, int resolved_index)
        {
            return query.Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISSUPPLIER> FilteredEntities(EamisSupplierDTO filter, IQueryable<EAMISSUPPLIER> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISSUPPLIER>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (filter.RegionCode != null && filter.RegionCode != 0)
                predicate = predicate.And(x => x.REGION_CODE == filter.RegionCode);

            if (filter.ProvinceCode != null && filter.ProvinceCode != 0)
                predicate = predicate.And(x => x.PROVINCE_CODE == filter.ProvinceCode);

            if (filter.CityMunicipalityCode != null && filter.CityMunicipalityCode != 0)
                predicate = predicate.And(x => x.CITY_MUNICIPALITY_CODE == filter.CityMunicipalityCode);

            if (filter.BrgyCode != null && filter.BrgyCode != 0)
                predicate = predicate.And(x => x.BRGY_CODE == filter.BrgyCode);

            if (!string.IsNullOrEmpty(filter.CompanyName)) predicate = (strict)
                    ? predicate.And(x => x.COMPANY_NAME.ToLower() == filter.CompanyName.ToLower())
                    : predicate.And(x => x.COMPANY_NAME.ToLower() == filter.CompanyName.ToLower());

            if (!string.IsNullOrEmpty(filter.CompanyDescription)) predicate = (strict)
                    ? predicate.And(x => x.COMPANY_DESCRIPTION.ToLower() == filter.CompanyDescription.ToLower())
                    : predicate.And(x => x.COMPANY_DESCRIPTION.ToLower() == filter.CompanyDescription.ToLower());

            if (!string.IsNullOrEmpty(filter.ContactPersonName)) predicate = (strict)
                    ? predicate.And(x => x.CONTACT_PERSON_NAME.ToLower() == filter.ContactPersonName.ToLower())
                    : predicate.And(x => x.CONTACT_PERSON_NAME.ToLower() == filter.ContactPersonName.ToLower());

            if (!string.IsNullOrEmpty(filter.ContactPersonNumber)) predicate = (strict)
                    ? predicate.And(x => x.CONTACT_PERSON_NUMBER.ToLower() == filter.ContactPersonNumber.ToLower())
                    : predicate.And(x => x.CONTACT_PERSON_NUMBER.ToLower() == filter.ContactPersonNumber.ToLower());

            if (filter.IsActive != null && filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);

            var query = custom_query ?? _ctx.EAMIS_SUPPLIER;
            return query.Where(predicate);
        }

        public async Task<DataList<EamisSupplierDTO>> SearchSupplier(string searchValue)
        {
            IQueryable<EAMISSUPPLIER> query = null;

            query = _ctx.EAMIS_SUPPLIER.AsNoTracking().Where(x => x.COMPANY_NAME.Contains(searchValue)).AsQueryable();


            var paged = PagedQueryForSearch(query);
            return new DataList<EamisSupplierDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISSUPPLIER> PagedQueryForSearch(IQueryable<EAMISSUPPLIER> query)
        {
            return query;
        }

        public async Task<EamisSupplierDTO> Update(EamisSupplierDTO item)
        {
            EAMISSUPPLIER data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }
    }
}
