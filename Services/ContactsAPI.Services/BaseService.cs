using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Services
{

    public abstract class BaseService<TEntity> : IBaseService<TEntity>
            where TEntity : class, IBase
    {
        protected ContactsApiDbContext _contactsApiDbContext;

        protected BaseService([NotNull] ContactsApiDbContext crudApiDbContext)
        {
            _contactsApiDbContext = crudApiDbContext;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            
            await _contactsApiDbContext.Set<TEntity>().AddAsync(entity);
            await _contactsApiDbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<TEntity> ReadAsync(int id)
        {
            var query = _contactsApiDbContext.Set<TEntity>().AsQueryable();

            return await query.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual async Task<TEntity> UpdateAsync(int id, TEntity updateEntity)
        {
            // Check that the record exists.
            var entity = await ReadAsync(id);
            if (entity == null)
                throw new Exception("Unable to find record with id '" + id + "'.");

            // Update changes if any of the properties have been modified.
            _contactsApiDbContext.Entry(entity).CurrentValues.SetValues(updateEntity);
            _contactsApiDbContext.Entry(entity).State = EntityState.Modified;

            if (_contactsApiDbContext.Entry(entity).Properties.Any(property => property.IsModified))
            {
                await _contactsApiDbContext.SaveChangesAsync();
            }
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            // Check that the record exists.
            var entity = await ReadAsync(id);
            if (entity == null)
                throw new Exception("Unable to find record with id '" + id + "'.");

            _contactsApiDbContext.Remove(entity);

            // Save changes to the Db Context.
            await _contactsApiDbContext.SaveChangesAsync();
        }

        public virtual async Task<string> ValidateEntity(TEntity updateEntity, string userId)
        {
            return Constants.NO_ERROR;
        }

        public virtual async Task<string> ValidateCanUpdate(int id, string userId)
        {
            return Constants.NO_ERROR;
        }

        public virtual async Task<string> ValidateCanDelete(int id, string userId)
        {
            return Constants.NO_ERROR;
        }
    }
}
