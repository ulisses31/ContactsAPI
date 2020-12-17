using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ContactsAPI.Data;
using ContactsAPI.Services;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.JsonPatch;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ContactsAPI.Services.Validations;

namespace ContactsAPI.Web.Controllers
{
    public abstract class BaseController<TEntity> : Controller
        where TEntity : class, IBase
    {
        protected readonly IBaseService<TEntity> _service;
        protected readonly IBaseValidation<TEntity> _validation;
        protected readonly ILogger _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly string _friendlyTableName;

        protected BaseController([NotNull] IBaseService<TEntity> service, 
                                 IBaseValidation<TEntity> validation,
                                 ILogger<TEntity> logger,
                                 IHttpContextAccessor httpContextAccessor, 
                                 string friendlyTableName = "record")
        {
            _service = service;
            _validation = validation;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _friendlyTableName = friendlyTableName;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(TEntity entity)
        {
            var identity = _httpContextAccessor.HttpContext.User.Identity;
            var errorMessage = await _validation.ValidateEntity(entity, identity.Name);
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            entity = await _service.CreateAsync(entity);
            return Ok(entity);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ReadAsync(int id)
        {
            var entity = await _service.ReadAsync(id);
            if (entity == null)
            {
                var message = string.Format("No {0} with ID = {1} found", _friendlyTableName, id);
                _logger.LogError(message);
                return NotFound(message);
            }

            return Ok(entity);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(TEntity entity)
        {
            var tmpEntity = await _service.ReadAsync(entity.Id);
            if (tmpEntity == null)
            {
                var message = string.Format("No {0} with ID = {1} found", _friendlyTableName, entity.Id);
                _logger.LogError(message);
                return NotFound(message);
            }

            var identity = _httpContextAccessor.HttpContext.User.Identity;
            var errorMessage = await _validation.ValidateCanUpdate(entity.Id, identity.Name);
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            errorMessage = await _validation.ValidateEntity(entity, identity.Name);
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            entity = await _service.UpdateAsync(entity.Id, entity);
            return Ok(entity);
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartialAsync(int id, [FromBody] JsonPatchDocument<TEntity> patchEntity)
        {
            var entity = await _service.ReadAsync(id);
            if (entity == null)
            {
                var message = string.Format("No {0} with ID = {1} found", _friendlyTableName, id);
                _logger.LogError(message);
                return NotFound(message);
            }

            patchEntity.ApplyTo(entity, ModelState);

            var identity = _httpContextAccessor.HttpContext.User.Identity;
            var errorMessage = await _validation.ValidateCanUpdate(entity.Id, identity.Name);
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            errorMessage = await _validation.ValidateEntity(entity, identity.Name);
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            entity = await _service.UpdateAsync(id, entity);
            return Ok(entity);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var entity = await _service.ReadAsync(id);
            if (entity == null)
            {
                var message = string.Format("No {0} with ID = {1} found", _friendlyTableName, id);
                _logger.LogError(message);
                return NotFound(message);
            }

            var identity = _httpContextAccessor.HttpContext.User.Identity;
            var errorMessage = await _validation.ValidateCanDelete(entity.Id, identity.Name);
            if (errorMessage != Constants.NO_ERROR)
            {
                _logger.LogError(errorMessage);
                return Problem(errorMessage);
            }

            await _service.DeleteAsync(id);

            return Ok(entity);
        }
    }
}
