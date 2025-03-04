﻿using AppCore.DTO;
using AppCore.Entities;
using AppCore.Interface.Services;
using AppCore.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/User")]
    [ApiController]   
    public class UserController : Controller
    {
        private readonly IUserService _user;
        private readonly IMapper _mapper;

        public UserController(IUserService user, IMapper mapper)
        {
            _user = user;
            _mapper = mapper;
        }

   

        [HttpGet]
        [Route("getall")]
        public async Task<ActionResult<dynamic>> GetAll()
        {
            // Recupera o usuário
            var users = await _user.GetAll();

            // Verifica se o usuário existe
            if (users.Value == null)
                return NotFound(new { message = "Usuários não localizados" });

            return new
            {
                users = _mapper.Map<List<UserViewModel>>(users.Value)
            };
        }

        //[HttpPost("{id}")]
        [HttpGet]
        [Route("get")]
        public async Task<ActionResult<dynamic>> Get(string id)
        {
            try
            {
                var res = await _user.Get(id);

                return new
                {
                    users = res
                };
            }
            catch (Exception)
            {
                return NotFound(new { message = "Usuário não localizado" });
            }

        }

        [HttpPost]
        [Route("auth")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] AuthUserModel model)
        {
            // Recupera o usuário
            var result = await _user.Auth(model.Email, model.Password);

            // Verifica se o usuário existe
            if (result.Value == null)
                return NotFound(result.Message);
            if (result.Status == false)
                return NotFound(result.Message);
            // Gera o Token
            var token = TokenConfig.GenerateToken(result.Value);

            // Retorna os dados
            return new
            {
                user = _mapper.Map<UserViewModel>(result.Value),
                token
            };
        }
        [HttpPost]
        [Route("insert")]
        public async Task<ActionResult<dynamic>> Insert([FromBody] CreateUserViewModel model)
        {
            var result = await _user.InsertAsync(_mapper.Map<UserEntity>(model));
                       
            if (!result.Status)            
                return NotFound( result.Message );            

            return Ok("Great, registration success");
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<dynamic>> Put([FromBody] UserViewModel model)
        {
            var result = await _user.UpdateAsync(_mapper.Map<UserEntity>(model));

            if (!result.Status)
                return NotFound(new { message = "Usuários não pode ser atualizado" });

            return new
            {  users = model };
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<dynamic>> Delete( string id)
        {
            var result = await _user.DeleteAsync(id);

            if (!result.Status)
                return NotFound(result.Message);

            return Ok("Great, remove registration success");
        }
    }
}
