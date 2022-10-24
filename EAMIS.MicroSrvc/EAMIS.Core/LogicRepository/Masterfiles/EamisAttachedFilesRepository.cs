using EAMIS.Common.DTO.Masterfiles;
using EAMIS.Core.ContractRepository.Masterfiles;
using EAMIS.Core.Domain;
using EAMIS.Core.Domain.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMIS.Core.LogicRepository.Masterfiles
{
    public class EamisAttachedFilesRepository : IEamisAttachedFilesRepository
    {
        private readonly EAMISContext _ctx;
        public EamisAttachedFilesRepository(EAMISContext ctx)
        {
            _ctx = ctx;
        }

        private EAMISATTACHEDFILES MapToEntity(EamisAttachedFilesDTO item)
        {
            if (item == null) return new EAMISATTACHEDFILES();
            return new EAMISATTACHEDFILES
            {
                ID = item.Id,
                ATTACHED_FILENAME = item.FileName,
                MODULE_NAME = item.ModuleName,
                TRANID = item.TransactionNumber,
                USERSTAMP = item.UserStamp,
                TIIMESTAMP = item.TimeStamp,
            };
        }
        public async Task<string> GetTranFileName(string transactionNumber)
        {
            string retValue = "";
            var result = await Task.Run(() => _ctx.EAMIS_ATTACHED_FILES.Where(s => s.TRANID == transactionNumber).AsNoTracking().ToList()).ConfigureAwait(false);
            if (result != null)
            {
                retValue = result[0].ATTACHED_FILENAME.ToString();
            }
            return retValue;
        }
        public async Task<string> GetTranFileName(string transactionNumber, string fileName)
        {
            var result = await Task.Run(() => _ctx.EAMIS_ATTACHED_FILES
                                                  .Where(s => s.TRANID == transactionNumber && s.ATTACHED_FILENAME == fileName)
                                                  .AsNoTracking()
                                                  .Select(v => v.ATTACHED_FILENAME)
                                                  .FirstOrDefault())
                                                  .ConfigureAwait(false);

            return result;
        }

        public async Task<bool> DeleteImageFileName(string transactionNumber, string fileName)
        {
            var result = await Task.Run(() => _ctx.EAMIS_ATTACHED_FILES
                                                  .Where(s => s.TRANID == transactionNumber && s.ATTACHED_FILENAME == fileName)
                                                  .AsNoTracking()
                                                  .Select(v => v.ID)
                                                  .FirstOrDefault())
                                                  .ConfigureAwait(false);

            if (result != 0)
            {
                try
                {
                    EamisAttachedFilesDTO file = new EamisAttachedFilesDTO
                    {
                        Id = result
                    };
                    EAMISATTACHEDFILES data = MapToEntity(file);
                    _ctx.Entry(data).State = EntityState.Deleted;
                    await _ctx.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    return false;
                    throw ex;
                }
            }
            return true;
        }
        public async Task<bool> Delete(EamisAttachedFilesDTO file)
        {
            try
            {

                EAMISATTACHEDFILES data = MapToEntity(file);
                _ctx.Entry(data).State = EntityState.Deleted;
                await _ctx.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }
        private IQueryable<EAMISATTACHEDFILES> PagedQuery(IQueryable<EAMISATTACHEDFILES> query, int resolved_size, int resolved_index)
        {
            return query.OrderByDescending(x => x.ID).Skip((resolved_index - 1) * resolved_size).Take(resolved_size);
        }
        private IQueryable<EAMISATTACHEDFILES> FilteredEntities(EamisAttachedFilesDTO filter, IQueryable<EAMISATTACHEDFILES> custom_query = null)
        {
            var predicate = PredicateBuilder.New<EAMISATTACHEDFILES>(true);
            if (filter.Id != null && filter.Id != 0)
                predicate = predicate.And(x => x.ID == filter.Id);

            if (filter.FileName != null && filter.FileName != null)
                predicate = predicate.And(x => x.ATTACHED_FILENAME == filter.FileName);

            if (filter.ModuleName != null && filter.ModuleName != null)
                predicate = predicate.And(x => x.MODULE_NAME == filter.ModuleName);

            if (filter.TransactionNumber != null && filter.TransactionNumber != null)
                predicate = predicate.And(x => x.TRANID == filter.TransactionNumber);

            if (filter.UserStamp != null && filter.UserStamp != null)
                predicate = predicate.And(x => x.USERSTAMP == filter.UserStamp);

            if (filter.TimeStamp != null && filter.TimeStamp != DateTime.MinValue)
                predicate = predicate.And(x => x.TIIMESTAMP == filter.TimeStamp);

            var query = custom_query ?? _ctx.EAMIS_ATTACHED_FILES;
            return query.Where(predicate);
        }
        public async Task<bool> Insert(List<EamisAttachedFilesDTO> files)
        {
            try
            {
                foreach (var file in files)
                {
                    EAMISATTACHEDFILES data = MapToEntity(file);
                    _ctx.Entry(data).State = EntityState.Added;
                    await _ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
            return true;
        }

    }
}