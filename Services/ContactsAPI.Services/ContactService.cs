using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Services
{
    public class ContactService : BaseService<Contact>, IContactService
    {
        public ContactService(ContactsApiDbContext contactsApiDbContext) : base(contactsApiDbContext) { }

        public override async Task<Contact> ReadAsync(int id)
        {
            var query = _contactsApiDbContext.Set<Contact>().AsQueryable();
            var contact = await query.Include(x => x.ContactSkills).ThenInclude(y => y.Skill).FirstOrDefaultAsync(entity => entity.Id == id);
            return contact;
        }



    }
}
