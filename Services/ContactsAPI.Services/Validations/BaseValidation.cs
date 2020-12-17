using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;

namespace ContactsAPI.Services.Validations
{
    public abstract class BaseValidation<TEntity>
            where TEntity : class, IBase    
    {
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
