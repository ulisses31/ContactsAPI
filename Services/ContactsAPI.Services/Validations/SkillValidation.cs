using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsAPI.Services.Validations
{
    public class SkillValidation : BaseValidation<Skill>, ISkillValidation
    {
        IContactSkillService _contactSkillService;

        public SkillValidation(IContactSkillService contactSkillService)
        {
            _contactSkillService = contactSkillService;
        }

        public override async Task<string> ValidateEntity(Skill updateEntity, string userId)
        {
            if (String.IsNullOrEmpty(updateEntity.Name))
                return "The Name field is required";

            if (updateEntity.Name.Length > Skill.SkillNameMaxLengh)
                return String.Format(Constants.MaxLengthError, "Name", Skill.SkillNameMaxLengh);

            return Constants.NO_ERROR;
        }

        public override async Task<string> ValidateCanDelete(int id, string userId)
        {
            var skillInUse = await _contactSkillService.SkillInUse(id);
            if (skillInUse)
                return String.Format("The Skill {0} is in use", id);

            return Constants.NO_ERROR;
        }
    }
}
