using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsAPI.Services.Validations
{
    public class ContactSkillValidation : BaseValidation<ContactSkill>, IContactSkillValidation
    {
        IContactService _contactService;
        ISkillService _skillService;
        IContactSkillService _contactSkillService;
        IUserService _userService;

        public ContactSkillValidation(IContactService contactService, ISkillService skillService, IContactSkillService contactSkillService, IUserService userService)
        {
            _contactService = contactService;
            _skillService = skillService;
            _contactSkillService = contactSkillService;
            _userService = userService;
        }

        public override async Task<string> ValidateEntity(ContactSkill updateEntity, string identityName)
        {
            if (updateEntity.Level != null &&
                ((int)updateEntity.Level < Constants.SkillLevelMin || (int)updateEntity.Level > Constants.SkillLevelMax))
                return String.Format("The level of the skill must be between {0} and {1}", Constants.SkillLevelMin, Constants.SkillLevelMax);

            var contact = await _contactService.ReadAsync(updateEntity.ContactId);
            if (contact == null)
                return String.Format("Contact with id {0} not found ", updateEntity.ContactId);

            var skill = await _skillService.ReadAsync(updateEntity.SkillId);
            if (skill == null)
                return String.Format("Skill with id {0} not found ", updateEntity.SkillId);

            var user = await _userService.ReadAsync(Convert.ToInt32(identityName));
            if (user.ContactId != updateEntity.ContactId)
                return String.Format("Only the contact can add skills to himself");

            return Constants.NO_ERROR;
        }

        public override async Task<string> ValidateCanUpdate(int id, string userId)
        {
            return await ValidateCanUpdateDelete(id, userId);
        }

        public override async Task<string> ValidateCanDelete(int id, string userId)
        {
            return await ValidateCanUpdateDelete(id, userId);
        }

        private async Task<string> ValidateCanUpdateDelete(int id, string userId)
        {
            var contactSkill = await _contactSkillService.ReadAsync(id);
            if (contactSkill == null)
                return String.Format("The contactSkill with id {0} not found", id);

            var contactId = await _userService.GetContactId(Convert.ToInt32(userId));
            if (contactId != contactSkill.ContactId)
                return String.Format("Only the contact owner of a skill can edit or delete it");

            return Constants.NO_ERROR;
        }

    }
}
