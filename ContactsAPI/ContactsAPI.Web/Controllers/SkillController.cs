using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using ContactsAPI.Services;
using ContactsAPI.Services.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContactsAPI.Web.Controllers
{
    [Route("api/skill")]
    [ApiController]
    public class SkillController : BaseController<Skill>
    { 
        public SkillController(ISkillService skillService, 
                               ISkillValidation skillValidation,
                               ILogger<Skill> logger, 
                               IHttpContextAccessor httpContextAccessor) : 
                        base(skillService, skillValidation, logger, httpContextAccessor, "skill")
        {
        }
    }
}
