using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eCom.Data.Repositories;
using eCom.Models;
using eCom.Helpers;
using System;

namespace eCom.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;

        public AccountController(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Register(Registration registration)
        {
            bool isSuccess = _accountRepository.Register(registration);

            if (isSuccess)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest("Registration failed");
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(Login login)
        {
            bool isValidUser = _accountRepository.Login(login);
            var fullName = _accountRepository.GetFullNameByEmail(login.Email);

            if (isValidUser)
            {
                // Generate JWT token
                var token = JwtHelper.GenerateJwtToken(_configuration, login.Email);

                // Set session cookie
                HttpContext.Session.SetString("IsAuthenticated", "true");

                // Return the token along with the response
                return Ok(new { Token = token, Message = "Valid user", FullName = fullName });
            }
            else
            {
                return BadRequest("Invalid user");
            }
        }
    

        [HttpGet]
        [Route("FullName")]
        public IActionResult GetFullName()
        {
            // Get the user's email from the authenticated user's claims
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email not found.");
            }

            // Call the method in the repository to fetch the full name from the database
            var fullName = _accountRepository.GetFullNameByEmail(email);

            if (string.IsNullOrEmpty(fullName))
            {
                return NotFound("Full name not found.");
            }

            return Ok(new { FullName = fullName });
        }
    }
}
