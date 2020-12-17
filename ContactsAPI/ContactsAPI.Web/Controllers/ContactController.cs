using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using ContactsAPI.Services;
using ContactsAPI.Services.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactsAPI.Web.Controllers
{
    // To get the JWT token necessary to invoke Contact web methods call the api/user/authenticate web method
    [Authorize]
    [Route("api/contact")]
    [ApiController]
    public class ContactController : BaseController<Contact>
    {
        public ContactController(IContactService contactService, 
                                 IContactValidation contactValidation, 
                                 ILogger<Contact> logger, 
                                 IHttpContextAccessor httpContextAccessor) : 
               base(contactService, contactValidation, logger, httpContextAccessor, "contact")
        {
        }

       // Note: While the API allows to create contacts directly, this is not usually a good idea, 
       // as we can't add skills to a contact if it's not linked to a user
       // it might be still be useful in a future version, if we had a superuser that could create contacts 
        // and add skills to any user
    }
}
