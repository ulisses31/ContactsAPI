using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Data.Model;
using ContactsAPI.Services;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net;
using AutoMapper;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ContactsAPI.Services.Validations;

namespace ContactsAPI.Web.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController: ControllerBase
    {
        protected readonly IUserService _userService;
        protected readonly IContactService _contactService;
        protected readonly IContactValidation _contactValidation;
        protected readonly ILogger _logger;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserController([NotNull] IUserService userService,
                              IContactService contactService, 
                              IContactValidation contactValidation,
                              ILogger<User> logger, 
                              IMapper mapper, 
                              IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _contactService = contactService;
            _contactValidation = contactValidation;
            _logger = logger;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        /// <summary>
        /// Call this to get the JWT token that will be used in other web methods
        /// </summary>
        public IActionResult Authenticate([FromBody] AuthenticateInfo model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        /// <summary>
        /// This will register both a contact and a user linked to the contact
        /// eg. call ... /api/user/register
        /// </summary>
        public async Task<IActionResult> RegisterAsync(RegisterInfo entity)
        {
            var errorMessage = entity.ValidateUserFields();
            if (errorMessage != Constants.NO_ERROR)
                return Problem(errorMessage);

            if (await _userService.UsernameAlreadyExists(entity.Username))
            {
                var message = "That username already exists";
                _logger.LogError(message);
                return Problem(message);
            }

            var contact = _mapper.Map<Contact>(entity);

            var user = new User()
            {
                Username = entity.Username,
                Firstname = entity.Firstname,
                Lastname = entity.Lastname
            };

            errorMessage = await _contactValidation.ValidateEntity(contact, "");
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            contact = await _contactService.CreateAsync(contact);
            if (contact != null)
            {
                user.ContactId = contact.Id;
                byte[] passwordHash, passwordSalt;
                UserService.CreatePasswordHash(entity.Password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                user = await _userService.CreateAsync(user);                

                return Ok(entity);
            }
            else
                return Problem();
        }

    }
}
