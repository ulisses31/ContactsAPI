using ContactsAPI.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactsAPI.Data;
using System.Text.RegularExpressions;

namespace ContactsAPI.Services.Validations
{
    public class ContactValidation : BaseValidation<Contact>, IContactValidation
    {
        IUserService _userService;

        public ContactValidation(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<string> ValidateEntity(Contact updateEntity, string userId)
        {
            if (String.IsNullOrEmpty(updateEntity.Firstname))
                return "FirstName field is required";
            if (String.IsNullOrEmpty(updateEntity.Firstname))
                return "LastName field is required";

            if (!String.IsNullOrEmpty(updateEntity.Email) && !EmailIsValid(updateEntity.Email))
                return "Invalid EMail format";

            var maxLengthErrors = GetMaxLengthErrors(updateEntity);
            if (!String.IsNullOrEmpty(maxLengthErrors))
                return maxLengthErrors;

            return Constants.NO_ERROR;
        }

        private string GetMaxLengthErrors(Contact updateEntity)
        {
            var sb = new StringBuilder("");
            if (updateEntity.Firstname.Length > Constants.FirstNameMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "FirstName", Constants.FirstNameMaxLength));

            if (updateEntity.Lastname.Length > Constants.LastNameMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "LastName", Constants.LastNameMaxLength));

            if (updateEntity.AddressLine1 != null && updateEntity.AddressLine1.Length > Contact.AddressLine1MaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "AddressLine1", Contact.AddressLine1MaxLength));

            if (updateEntity.AddressLine2 != null && updateEntity.AddressLine2.Length > Contact.AddressLine2MaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "AddressLine2", Contact.AddressLine2MaxLength));

            if (updateEntity.City != null && updateEntity.City.Length > Contact.CityMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "City", Contact.CityMaxLength));

            if (updateEntity.PostalCode != null && updateEntity.PostalCode.Length > Contact.PostalCodeMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "PostalCode", Contact.PostalCodeMaxLength));

            if (updateEntity.Country != null && updateEntity.Country.Length > Contact.CountryMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "Country", Contact.CountryMaxLength));

            if (updateEntity.Email != null && updateEntity.Email.Length > Contact.EmailMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "Email", Contact.EmailMaxLength));

            if (updateEntity.MobilePhoneNumber != null && updateEntity.MobilePhoneNumber.Length > Contact.MobilePhoneNumberMaxLength)
                sb.AppendLine(String.Format(Constants.MaxLengthError, "MobilePhoneNumber", Contact.MobilePhoneNumberMaxLength));

            return sb.ToString();
        }

        public static bool EmailIsValid(string email)
        {
            return Regex.IsMatch(email, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9_\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.ECMAScript);
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
            var contactId = await _userService.GetContactId(Convert.ToInt32(userId));
            if (contactId != id)
                return String.Format("Only the contact can edit or delete himself");

            return Constants.NO_ERROR;
        }

    }
}
