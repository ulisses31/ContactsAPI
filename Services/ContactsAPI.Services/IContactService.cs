using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;

namespace ContactsAPI.Services
{
    public interface IContactService : IBaseService<Contact>
    {
    }
}
