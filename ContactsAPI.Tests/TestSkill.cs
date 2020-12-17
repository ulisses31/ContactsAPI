using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ContactsAPI.Web.Controllers;
using ContactsAPI.Services;
using Moq;
using ContactsAPI.Services.Validations;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using ContactsAPI.Data.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ContactsAPI.Tests
{
    [TestClass]
    public class TestSkill
    {
        const int SkillIdTest = 5;
        const string UserIdStr = "1";

        Mock<ISkillService> skillService;
        Mock<IContactSkillService> contactSkillService;
        SkillValidation skillValidation;
        Mock<ILogger<Skill>> logger;
        Mock<IHttpContextAccessor> httpContextAccessor;


        [TestInitialize]
        public void Initialize()
        {
            skillService = new Mock<ISkillService>();
            contactSkillService = new Mock<IContactSkillService>();
            skillValidation = new SkillValidation(contactSkillService.Object);
            logger = new Mock<ILogger<Skill>>();
            httpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [TestMethod]
        public async Task TestCreate()
        {
            var skill = new Skill() { Name = "Test Skill" };
            skillService.Setup(entity => entity.CreateAsync(skill)).Returns(Task.FromResult(new Skill() { Name = skill.Name, Id = SkillIdTest }));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns(UserIdStr);

            var skillController = new SkillController(skillService.Object, skillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await skillController.CreateAsync(skill);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((Skill)((OkObjectResult)result).Value).Id == SkillIdTest);
        }

        [TestMethod]
        public async Task TestUpdate()
        {
            var skill = new Skill() { Id = SkillIdTest, Name = "Test Skill" };
            skillService.Setup(entity => entity.UpdateAsync(SkillIdTest, skill)).Returns(Task.FromResult(skill));
            skillService.Setup(entity => entity.ReadAsync(SkillIdTest)).Returns(Task.FromResult(skill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns((string)null);

            var skillController = new SkillController(skillService.Object, skillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await skillController.UpdateAsync(skill);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((Skill)((OkObjectResult)result).Value).Id == SkillIdTest);
        }

        [TestMethod]
        public async Task TestDeleteNotInUseElsewhere()
        {
            var skill = new Skill() { Id = SkillIdTest, Name = "Test Skill" };
            skillService.Setup(entity => entity.DeleteAsync(SkillIdTest));  
            skillService.Setup(entity => entity.ReadAsync(SkillIdTest)).Returns(Task.FromResult(skill));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns((string)null);

            var skillController = new SkillController(skillService.Object, skillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await skillController.DeleteAsync(SkillIdTest);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsTrue(((OkObjectResult)result).StatusCode == Constants.StatusCodeOK);
        }

        [TestMethod]
        public async Task TestCantDeleteIfInUseElsewhere()
        {
            var skill = new Skill() { Id = SkillIdTest, Name = "Test Skill" };
            skillService.Setup(entity => entity.DeleteAsync(SkillIdTest));
            skillService.Setup(entity => entity.ReadAsync(SkillIdTest)).Returns(Task.FromResult(skill));
            contactSkillService.Setup(x => x.SkillInUse(SkillIdTest)).Returns(Task.FromResult(true));
            httpContextAccessor.Setup(acc => acc.HttpContext.User.Identity.Name).Returns((string)null);

            var skillController = new SkillController(skillService.Object, skillValidation,
                                                logger.Object, httpContextAccessor.Object);
            var result = await skillController.DeleteAsync(SkillIdTest);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsTrue(((ObjectResult)result).StatusCode == Constants.InternalServerError);
        }
    }
}