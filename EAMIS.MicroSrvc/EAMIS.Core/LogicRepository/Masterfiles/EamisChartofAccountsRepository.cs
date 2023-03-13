using EAMIS.Common.DTO.Classification;
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
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisChartofAccountsRepository : IEamisChartofAccountsRepository
    {
        private readonly EAMISContext _ctx;
        private readonly int _maxPageSize;
        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => value = _errorMessage; }

        private bool bolerror = false;
        public bool HasError { get => bolerror; set => value = bolerror; }
        public EamisChartofAccountsRepository(EAMISContext ctx)
        {
            _ctx = ctx;
            _maxPageSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("MaxPageSize")) ? 100
               : int.Parse(ConfigurationManager.AppSettings.Get("MaxPageSize").ToString());
        }
        public async Task<List<EAMISCHARTOFACCOUNTS>> ListAllCOA()
        {
            var result = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().ToList();
            return result;
        }
        public async Task<List<EAMISCHARTOFACCOUNTS>> ListCOA(string searchValue)
        {
            var result = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.ACCOUNT_CODE == searchValue).ToList();
            return result;
        }
        public async Task<List<EAMISCHARTOFACCOUNTS>> ListCOAById(int coaId)
        {
            var result = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.ID == coaId).ToList();
            return result;
        }


        public async Task<bool> InsertFromExcel(List<EamisChartofAccountsDTO> Items)
        {
            List<EAMISCHARTOFACCOUNTS> lstCategory = new List<EAMISCHARTOFACCOUNTS>();
            try
            {
                for (int intItems = 0; intItems < Items.Count(); intItems++)
                {
                    EAMISCHARTOFACCOUNTS objCOA = MapToEntity(Items[intItems]);

                    lstCategory.Add(objCOA);
                }
                _ctx.EAMIS_CHART_OF_ACCOUNTS.AddRange(lstCategory);
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

        public async Task<EamisChartofAccountsDTO> InsertFromExcel(EamisChartofAccountsDTO item)
        {
            try
            {
                EAMISCHARTOFACCOUNTS data = MapToEntity(item);
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
        public async Task<EamisChartofAccountsDTO> Delete(EamisChartofAccountsDTO item, int Id)
        {
            EAMISCHARTOFACCOUNTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Deleted;
            await _ctx.SaveChangesAsync();
            return item;
        }

        private EAMISCHARTOFACCOUNTS MapToEntity(EamisChartofAccountsDTO item)
        {
            if (item == null) return new EAMISCHARTOFACCOUNTS();
            return new EAMISCHARTOFACCOUNTS
            {
                ID = item.Id,
                GROUP_ID = item.GroupId,
                OBJECT_CODE = item.ObjectCode,
                ACCOUNT_CODE = item.AccountCode,
                IS_ACTIVE = item.IsActive,
                IS_PART_OF_INVENTORY = item.IsPartofInventroy,
                USER_STAMP = item.UserStamp,
            };
        }

        public async Task<EamisChartofAccountsDTO> Insert(EamisChartofAccountsDTO item)
        {
            EAMISCHARTOFACCOUNTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Added;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisChartofAccountsDTO>> List(EamisChartofAccountsDTO filter, PageConfig config)
        {
            IQueryable<EAMISCHARTOFACCOUNTS> query = FilteredEntities(filter);

            string resolved_sort = config.SortBy ?? "Id";
            bool resolve_isAscending = (config.IsAscending) ? config.IsAscending : false;
            int resolved_size = config.Size ?? _maxPageSize;
            if (resolved_size > _maxPageSize) resolved_size = _maxPageSize;
            int resolved_index = config.Index ?? 1;

            var paged = PagedQuery(query, resolved_size, resolved_index);
            return new DataList<EamisChartofAccountsDTO>
            {
                Count = await query.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync(),
            };
        }

        private IQueryable<EamisChartofAccountsDTO> QueryToDTO(IQueryable<EAMISCHARTOFACCOUNTS> query)
        {
            return query.Select(x => new EamisChartofAccountsDTO
            {
                Id = x.ID,
                GroupId = x.GROUP_ID,
                ObjectCode = x.OBJECT_CODE,
                AccountCode = x.ACCOUNT_CODE,
                IsActive = x.IS_ACTIVE,
                IsPartofInventroy = x.IS_PART_OF_INVENTORY,
                //ClassificationDTO = new EamisClassificationDTO
                //{
                //    Id = x.CLASSIFICATION.ID,
                //    NameClassification = x.CLASSIFICATION.NAME_CLASSIFICATION,
                //},
                //SubClassificationDTO = new EamisSubClassificationDTO
                //{
                //    Id = x.SUBCLASSIFICATION.ID,
                //    ClassificationId = x.CLASSIFICATION.ID,
                //    NameSubClassificiation = x.SUBCLASSIFICATION.NAME_SUBCLASSIFICATION,
                //    ClassificationDTO = new EamisClassificationDTO
                //    {
                //        Id = x.CLASSIFICATION.ID,
                //        NameClassification = x.CLASSIFICATION.NAME_CLASSIFICATION
                //    },

                //},
                //GroupClassificationDTO = new EamisGroupClassificationDTO
                //{
                //    Id = x.GROUPCLASSIFICATION.ID,
                //    ClassificationId = x.CLASSIFICATION.ID,
                //    NameGroupClassification = x.GROUPCLASSIFICATION.NAME_GROUPCLASSIFICATION,
                //}

                GroupClassificationDTO = new EamisGroupClassificationDTO
                {
                    Id = x.GROUPCLASSIFICATION.ID,
                    ClassificationId = x.GROUPCLASSIFICATION.CLASSIFICATION_ID,
                    SubClassificationId = x.GROUPCLASSIFICATION.SUB_CLASSIFICATION_ID,
                    NameGroupClassification = x.GROUPCLASSIFICATION.NAME_GROUPCLASSIFICATION,
                    SubClassificationDTO = new EamisSubClassificationDTO
                    {
                        Id = x.GROUPCLASSIFICATION.SUBCLASSIFICATION.ID,
                        ClassificationId = x.GROUPCLASSIFICATION.SUBCLASSIFICATION.CLASSIFICATION_ID,
                        NameSubClassification = x.GROUPCLASSIFICATION.SUBCLASSIFICATION.NAME_SUBCLASSIFICATION
                        

                    },
                    ClassificationDTO = new EamisClassificationDTO
                    {
                        Id = x.GROUPCLASSIFICATION.CLASSIFICATION.ID,
                        NameClassification = x.GROUPCLASSIFICATION.CLASSIFICATION.NAME_CLASSIFICATION,
                    }
                },
                SubClassificationDTO = new EamisSubClassificationDTO
                {
                    Id = x.GROUPCLASSIFICATION.SUBCLASSIFICATION.ID,
                    ClassificationId = x.GROUPCLASSIFICATION.SUBCLASSIFICATION.CLASSIFICATION_ID,
                    NameSubClassification = x.GROUPCLASSIFICATION.SUBCLASSIFICATION.NAME_SUBCLASSIFICATION,
                },
                ClassificationDTO = new EamisClassificationDTO
                {
                    Id = x.GROUPCLASSIFICATION.CLASSIFICATION.ID,
                    NameClassification = x.GROUPCLASSIFICATION.CLASSIFICATION.NAME_CLASSIFICATION,
                },
             
            });
        }

        private IQueryable<EAMISCHARTOFACCOUNTS> PagedQuery(IQueryable<EAMISCHARTOFACCOUNTS> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }

        private IQueryable<EAMISCHARTOFACCOUNTS> FilteredEntities(EamisChartofAccountsDTO filter, IQueryable<EAMISCHARTOFACCOUNTS> custom_query = null, bool strict = false)
        {
            var predicate = PredicateBuilder.New<EAMISCHARTOFACCOUNTS>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);
            if (filter.GroupId != null && filter.GroupId != 0)
                predicate = predicate.And(x => x.GROUP_ID == filter.GroupId);
            if (!string.IsNullOrEmpty(filter.ObjectCode)) predicate = (strict)
                     ? predicate.And(x => x.OBJECT_CODE.ToLower() == filter.ObjectCode.ToLower())
                     : predicate.And(x => x.OBJECT_CODE.Contains(filter.ObjectCode.ToLower()));
            if (!string.IsNullOrEmpty(filter.AccountCode)) predicate = (strict)
                     ? predicate.And(x => x.ACCOUNT_CODE.ToLower() == filter.AccountCode.ToLower())
                     : predicate.And(x => x.ACCOUNT_CODE.Contains(filter.AccountCode.ToLower()));
            if (filter.IsActive != null && filter.IsActive != false)
                predicate = predicate.And(x => x.IS_ACTIVE == filter.IsActive);
            if (filter.IsPartofInventroy != null && filter.IsPartofInventroy != false)
                predicate = predicate.And(x => x.IS_PART_OF_INVENTORY == filter.IsPartofInventroy);
            var query = custom_query ?? _ctx.EAMIS_CHART_OF_ACCOUNTS;
            return query.Where(predicate);
        }

        public async Task<EamisChartofAccountsDTO> Update(EamisChartofAccountsDTO item, int Id)
        {
            EAMISCHARTOFACCOUNTS data = MapToEntity(item);
            _ctx.Entry(data).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return item;
        }

        public async Task<DataList<EamisChartofAccountsDTO>> SearchChart(string type, string searchValue)
        {
            IQueryable<EAMISCHARTOFACCOUNTS> query = null;
            if (type == "UACS")
            {
                query = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.ACCOUNT_CODE.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Object Code")
            {
                query = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.OBJECT_CODE.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Classification")
            {
                query = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.GROUPCLASSIFICATION.CLASSIFICATION.NAME_CLASSIFICATION.Contains(searchValue)).AsQueryable();
            }
            else if (type == "Sub-Classification")
            {
                query = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.GROUPCLASSIFICATION.SUBCLASSIFICATION.NAME_SUBCLASSIFICATION.Contains(searchValue)).AsQueryable();
            }
            else
            {
                query = _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().Where(x => x.GROUPCLASSIFICATION.NAME_GROUPCLASSIFICATION.Contains(searchValue)).AsQueryable();
            }

            var paged = PagedQueryForSearch(query);
            return new DataList<EamisChartofAccountsDTO>
            {
                Count = await paged.CountAsync(),
                Items = await QueryToDTO(paged).ToListAsync()
            };
        }

        private IQueryable<EAMISCHARTOFACCOUNTS> PagedQueryForSearch(IQueryable<EAMISCHARTOFACCOUNTS> query)
        {
            return query;
        }

        public async Task<bool> ValidateExistingAccountCode(string accountCode)
        {
            return await _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().AnyAsync(x => x.ACCOUNT_CODE == accountCode);
        }

        public async Task<bool> EditValidationAccountCode(int id, string accountCode)
        {
            return await _ctx.EAMIS_CHART_OF_ACCOUNTS.AsNoTracking().AnyAsync(x => x.ID == id && x.ACCOUNT_CODE == accountCode);
        }
    }
}
