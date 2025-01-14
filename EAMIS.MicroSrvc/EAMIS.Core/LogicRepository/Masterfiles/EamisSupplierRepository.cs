﻿using EAMIS.Common.DTO;
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

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
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

        public async Task<bool> InsertFromExcel(List<EamisSupplierDTO> Items)
        {
            List<EAMISSUPPLIER> lstSupplier = new List<EAMISSUPPLIER>();
            try
            {
                for (int intItems = 0; intItems < Items.Count(); intItems++)
                {
                    EAMISSUPPLIER objSupplier = MapToEntity(Items[intItems]);

                    lstSupplier.Add(objSupplier);
                }
                _ctx.EAMIS_SUPPLIER.AddRange(lstSupplier);
                 _ctx.SaveChangesAsync().GetAwaiter().GetResult();
                bolerror = false;
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.InnerException.Message;
            }
            return HasError;
        }
        public async Task<EamisSupplierDTO> InsertFromExcel(EamisSupplierDTO item)
        {
           
            try
            {
                EAMISSUPPLIER data = MapToEntity(item);
                _ctx.Entry(data).State = EntityState.Added;
                _ctx.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                bolerror = true;
                _errorMessage = ex.InnerException.Message;
            }
            return item;
        }
        public async Task<List<EAMISSUPPLIER>> GetAllSuppliers()
        {
            var result = await Task.Run(() => _ctx.EAMIS_SUPPLIER.ToList()).ConfigureAwait(false);
            return result;
        }
        public async Task<List<EAMISSUPPLIER>> ListAllSuppliers()
        {

            //IQueryable<EAMISSUPPLIER> query = _ctx.EAMIS_SUPPLIER;
            //var result = query.ToListAsync().GetAwaiter().GetResult();
            var result = _ctx.EAMIS_SUPPLIER.AsNoTracking().ToList();
            return result;

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
                EMAIL_ADD = item.EmailAdd,
                COMPANY_DESCRIPTION = item.CompanyDescription,
                STREET = item.Street,
                COMPANY_NAME = item.CompanyName,
                CONTACT_PERSON_NAME = item.ContactPersonName,
                CONTACT_PERSON_NUMBER = item.ContactPersonNumber,
                IS_ACTIVE = item.IsActive,
                BRGY_CODE = item.BrgyCode,
                CITY_MUNICIPALITY_CODE = item.CityMunicipalityCode,
                PROVINCE_CODE = item.ProvinceCode,
                REGION_CODE = item.RegionCode,
                USER_STAMP = item.UserStamp
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
                EmailAdd = x.EMAIL_ADD,
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
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
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

        private IQueryable<EAMISSUPPLIER> PagedQueryForSearch(IQueryable<EAMISSUPPLIER> query)
        {
            return query;
        }

        public async Task<EamisSupplierDTO> Update(EamisSupplierDTO item, int id)
        {
            EAMISSUPPLIER data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }


        public async Task<bool> ValidateExistingCode(string companyname)
        {
            return await _ctx.EAMIS_SUPPLIER.AsNoTracking().AnyAsync(x => x.COMPANY_NAME == companyname);
        }
        public async Task<string> GetSupplieryById(int supplierId)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_SUPPLIER.Where(s => s.ID == supplierId).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].BANK.ToString() + ":" +
                              result[0].ACCOUNT_NAME.ToString() + ":" +
                              result[0].ACCOUNT_NUMBER.ToString() + ":" +
                              result[0].BRANCH.ToString();
            }
            return retValue;
        }
        public async Task<bool> UpdateValidationCode(int id, string companyname)
        {
            return await _ctx.EAMIS_SUPPLIER.AsNoTracking().AnyAsync(x => x.ID == id && x.COMPANY_NAME == companyname);
        }

        public async Task<DataList<EamisSupplierDTO>> SearchSuppliers(string type, string searchValue)
        {
            IQueryable<EAMISSUPPLIER> query = null;
            if (type == "Company Name")
            {
                query = _ctx.EAMIS_SUPPLIER.AsNoTracking().Where(x => x.COMPANY_NAME.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Contact Person")
            {
                query = _ctx.EAMIS_SUPPLIER.AsNoTracking().Where(x => x.CONTACT_PERSON_NAME.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_SUPPLIER.AsNoTracking().Where(x => x.CONTACT_PERSON_NUMBER.Contains(searchValue)).AsQueryable();
            }


            var paged = PagedQueryForSearch(query);
            return new DataList<EamisSupplierDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }
    }

}
