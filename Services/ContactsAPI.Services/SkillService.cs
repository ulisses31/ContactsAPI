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
    public class SkillService : BaseService<Skill>, ISkillService
    {
        public SkillService(ContactsApiDbContext contactsApiDbContext) : base(contactsApiDbContext) { }
    }
}
