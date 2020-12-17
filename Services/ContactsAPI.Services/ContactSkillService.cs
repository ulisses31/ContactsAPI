using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace ContactsAPI.Services
{
    public class ContactSkillService : BaseService<ContactSkill>, IContactSkillService
    {
        public ContactSkillService(ContactsApiDbContext contactsApiDbContext) : 
            base(contactsApiDbContext) 
        {
        }

        public async Task<bool> SkillInUse(int skillId)
        {
            var contactSkillsQuery = _contactsApiDbContext.Set<ContactSkill>().AsQueryable();
            var contactSkill = await contactSkillsQuery.FirstOrDefaultAsync(x => x.SkillId == skillId);
            return contactSkill != null;
        }

    }
}
