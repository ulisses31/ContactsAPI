using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data.Model;

namespace ContactsAPI.Services.Validations
{
    public interface IContactValidation : IBaseValidation<Contact>
    {
    }
}
