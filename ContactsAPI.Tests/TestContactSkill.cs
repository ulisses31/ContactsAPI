using ContactsAPI.Data.Model;
using ContactsAPI.Services;
using ContactsAPI.Services.Validations;
using ContactsAPI.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsAPI.Tests
{
    [TestClass]
    public class TestContactSkill
    {
        const int ContactIdTest = 5;
        const string UserIdStr = "1";
        const int SkillIdTest = 2;
        const int ContactSkillIdTest = 7;

        Mock<IContactSkillService> contactSkillService;
        Mock<IContactService> contactService;
        Mock<ISkillService> skillService;
        Mock<IUserService> userService;
        ContactSkillValidation contactSkillValidation;
        Mock<ILogger<ContactSkill>> logger;
        Mock<IHttpContextAccessor> httpContextAccessor;


        [TestInitialize]
        public void Initialize()
        {
            contactSkillService = new Mock<IContactSkillService>();
            contactService = new Mock<IContactService>();
            skillService = new Mock<ISkillService>();
            userService = new Mock<IUserService>();
            contactSkillValidation = new ContactSkillValidation(contactService.Object, 
                                    skillService.Object, contactSkillService.Object, userService.Object);
            logger = new Mock<ILogger<ContactSkill>>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();

            contactService.Setup(entity => entity.ReadAsync(ContactIdTest)).Returns(Task.FromResult(new Contact() { Id = ContactIdTest }));

            skillService.Setup(entity => entity.ReadAsync(SkillIdTest)).Returns(Task.FromResult(new Skill() { Id = SkillIdTest }));

            userService.Setup(entity => entity.ReadAsync(Convert.ToInt32(UserIdStr))).Returns(Task.FromResult(new User() { ContactId = ContactIdTest }));
            userService.Setup(entity => entity.GetContactId(Convert.ToInt32(UserIdStr))).Returns(Task.FromResult((int?)ContactIdTest));
        }

        [TestMethod]
        public async Task TestCreate()
        {
            var contactSkill = new ContactSkill() { ContactId = ContactIdTest, SkillId = SkillIdTest };
            var resultingContactSkill = new ContactSkill() { ContactId = contactSkill.ContactId, SkillId = contactSkill.SkillId, Id = ContactSkillIdTest };
            contactSkillService.Setup(entity => entity.CreateAsync(contactSkill)).Returns(Task.FromResult(resultingContactSkill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var contactSkillController = new ContactSkillController(contactSkillService.Object, contactSkillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactSkillController.CreateAsync(contactSkill);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((ContactSkill)((OkObjectResult)result).Value).Id == ContactSkillIdTest);
        }

        [TestMethod]
        public async Task TestUpdateOkWhenSameUser()
        {
            var contactSkill = new ContactSkill() { Id = ContactSkillIdTest, ContactId = ContactIdTest, SkillId = SkillIdTest };
            contactSkillService.Setup(entity => entity.UpdateAsync(ContactSkillIdTest, contactSkill)).Returns(Task.FromResult(contactSkill));
            contactSkillService.Setup(entity => entity.ReadAsync(ContactSkillIdTest)).Returns(Task.FromResult(contactSkill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var contactSkillController = new ContactSkillController(contactSkillService.Object, contactSkillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactSkillController.UpdateAsync(contactSkill);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((ContactSkill)((OkObjectResult)result).Value).Id == ContactSkillIdTest);
        }

        [TestMethod]
        public async Task TestUpdateNotOkIfOtherUser()
        {
            var contactSkill = new ContactSkill() { Id = ContactSkillIdTest, ContactId = ContactIdTest, SkillId = SkillIdTest };
            contactSkillService.Setup(entity => entity.UpdateAsync(ContactSkillIdTest, contactSkill)).Returns(Task.FromResult(contactSkill));
            contactSkillService.Setup(entity => entity.ReadAsync(ContactSkillIdTest)).Returns(Task.FromResult(contactSkill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns("999");

            var contactSkillController = new ContactSkillController(contactSkillService.Object, contactSkillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactSkillController.UpdateAsync(contactSkill);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsTrue(((ObjectResult)result).StatusCode == Constants.InternalServerError);
        }


        [TestMethod]
        public async Task TestDeleteOkWhenSameUser()
        {
            var contactSkill = new ContactSkill() { Id = ContactSkillIdTest, ContactId = ContactIdTest, SkillId = SkillIdTest };
            contactSkillService.Setup(entity => entity.DeleteAsync(ContactSkillIdTest));
            contactSkillService.Setup(entity => entity.ReadAsync(ContactSkillIdTest)).Returns(Task.FromResult(contactSkill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var contactSkillController = new ContactSkillController(contactSkillService.Object, contactSkillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactSkillController.DeleteAsync(ContactSkillIdTest);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((OkObjectResult)result).StatusCode == Constants.StatusCodeOK);
        }

        [TestMethod]
        public async Task TestDeleteNotOkIfOtherUser()
        {
            var contactSkill = new ContactSkill() { Id = ContactSkillIdTest, ContactId = ContactIdTest, SkillId = SkillIdTest };
            contactSkillService.Setup(entity => entity.DeleteAsync(ContactSkillIdTest));
            contactSkillService.Setup(entity => entity.ReadAsync(ContactSkillIdTest)).Returns(Task.FromResult(contactSkill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns("999");

            var contactSkillController = new ContactSkillController(contactSkillService.Object, contactSkillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactSkillController.DeleteAsync(ContactSkillIdTest);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsTrue(((ObjectResult)result).StatusCode == Constants.InternalServerError);
        }
    }

}
