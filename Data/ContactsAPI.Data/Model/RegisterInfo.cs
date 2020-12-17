using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsAPI.Data.Model
{
    public class RegisterInfo: Contact
    {
        public string Username { get; set; }
        public string Password { get; set; }

        const int MIN_PASS_LEN = 8;

        public string ValidateUserFields()
        {
            if (String.IsNullOrEmpty(Username))
                return "The Username is required";
            if (String.IsNullOrEmpty(Password))
                return "The Password is required";
            if (String.IsNullOrEmpty(Firstname))
                return "The Firstname is required";
            if (String.IsNullOrEmpty(Lastname))
                return "The Lastname is required";

            if (Password.Length < MIN_PASS_LEN)
                return String.Format("The password must have a minimum of {0} characters", MIN_PASS_LEN);

            if (Username.Length > User.UsernameMaxLengh)
                return String.Format(Constants.MaxLengthError, "Username", User.UsernameMaxLengh);

            return Constants.NO_ERROR;
        }
    }
}
