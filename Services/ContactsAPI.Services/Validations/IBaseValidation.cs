using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;

namespace ContactsAPI.Services.Validations
{
    public interface IBaseValidation<TEntity>
            where TEntity : class, IBase
    {
        Task<string> ValidateEntity(TEntity updateEntity, string userId);

        Task<string> ValidateCanUpdate(int id, string userId);

        Task<string> ValidateCanDelete(int id, string userId);
    }
}
