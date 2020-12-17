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
    // To get the JWT token necessary to invoke ContactSkill web methods call api/user/authenticate web method
    [Authorize]
    [Route("api/contactSkill")]
    [ApiController]
    public class ContactSkillController : BaseController<ContactSkill>
    {
        public ContactSkillController(IContactSkillService contactSkillService, 
                                      IContactSkillValidation contactSkillValidation, 
                                      ILogger<ContactSkill> logger, 
                                      IHttpContextAccessor httpContextAccessor) : 
                base(contactSkillService, contactSkillValidation, logger, httpContextAccessor, "contact/skill entry")
        {
        }


    }
}

