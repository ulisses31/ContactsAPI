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
    public class TestContact
    {
        const int ContactIdTest = 5;
        const string UserIdStr = "1";

        Mock<IContactService> contactService;
        Mock<IUserService> userService;
        ContactValidation contactValidation;
        Mock<ILogger<Contact>> logger;
        Mock<IHttpContextAccessor> httpContextAccessor;


        [TestInitialize]
        public void Initialize()
        {
            contactService = new Mock<IContactService>();
            userService = new Mock<IUserService>();
            contactValidation = new ContactValidation(userService.Object);
            logger = new Mock<ILogger<Contact>>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();

            userService.Setup(entity => entity.GetContactId(Convert.ToInt32(UserIdStr))).Returns(Task.FromResult((int?)ContactIdTest));
        }

        [TestMethod]
        public async Task TestCreate()
        {
            var contact = new Contact() { Firstname = "John", Lastname = "Dennis" };
            var resultingContact = new Contact() { Firstname = contact.Firstname, Lastname = contact.Lastname, Id = ContactIdTest };
            contactService.Setup(entity => entity.CreateAsync(contact)).Returns(Task.FromResult(resultingContact));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var contactController = new ContactController(contactService.Object, contactValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactController.CreateAsync(contact);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((Contact)((OkObjectResult)result).Value).Id == ContactIdTest);
        }

        [TestMethod]
        public async Task TestUpdateOkWhenSameUser()
        {
            var contact = new Contact() { Id = ContactIdTest, Firstname = "John", Lastname = "Dennis" };
            contactService.Setup(entity => entity.UpdateAsync(ContactIdTest, contact)).Returns(Task.FromResult(contact));
            contactService.Setup(entity => entity.ReadAsync(ContactIdTest)).Returns(Task.FromResult(contact));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var contactController = new ContactController(contactService.Object, contactValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactController.UpdateAsync(contact);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((Contact)((OkObjectResult)result).Value).Id == ContactIdTest);
        }

        [TestMethod]
        public async Task TestUpdateNotOkIfOtherUser()
        {
            var contact = new Contact() { Id = ContactIdTest, Firstname = "John", Lastname = "Dennis" };
            contactService.Setup(entity => entity.UpdateAsync(ContactIdTest, contact)).Returns(Task.FromResult(contact));
            contactService.Setup(entity => entity.ReadAsync(ContactIdTest)).Returns(Task.FromResult(contact));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns("999");

            var contactController = new ContactController(contactService.Object, contactValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactController.UpdateAsync(contact);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsTrue(((ObjectResult)result).StatusCode == Constants.InternalServerError);
        }


        [TestMethod]
        public async Task TestDeleteOkWhenSameUser()
        {
            var contact = new Contact() { Id = ContactIdTest, Firstname = "John", Lastname = "Dennis" };
            contactService.Setup(entity => entity.DeleteAsync(ContactIdTest));
            contactService.Setup(entity => entity.ReadAsync(ContactIdTest)).Returns(Task.FromResult(contact));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var contactController = new ContactController(contactService.Object, contactValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactController.DeleteAsync(ContactIdTest);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((OkObjectResult)result).StatusCode == Constants.StatusCodeOK);
        }

        [TestMethod]
        public async Task TestDeleteNotOkIfOtherUser()
        {
            var contact = new Contact() { Id = ContactIdTest+1, Firstname = "John", Lastname = "Dennis" };
            contactService.Setup(entity => entity.DeleteAsync(ContactIdTest));
            contactService.Setup(entity => entity.ReadAsync(ContactIdTest)).Returns(Task.FromResult(contact));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns("999");
            
            var contactController = new ContactController(contactService.Object, contactValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await contactController.DeleteAsync(ContactIdTest);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsTrue(((ObjectResult)result).StatusCode == Constants.InternalServerError);
        }
    }

}
