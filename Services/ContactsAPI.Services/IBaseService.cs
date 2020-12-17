using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;

namespace ContactsAPI.Services
{
    public interface IBaseService<TEntity>
            where TEntity : class, IBase
    {
        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> ReadAsync(int id);

        Task<TEntity> UpdateAsync(int id, TEntity updateEntity);

        Task DeleteAsync(int id);
    }
}
