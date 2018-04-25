using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Identity;
using DutchTreat.Data.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DutchTreat.Controllers
{
  public class AccountController : Controller
  {
    
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<StoreUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly UserManager<StoreUser> _userManager;

    public AccountController(ILogger<AccountController> logger,
     SignInManager<StoreUser> signInManager,
      UserManager<StoreUser> userManager,
      IConfiguration configuration
      )
    {
      this._logger = logger;
      this._signInManager = signInManager;
      this._configuration = configuration;
      _userManager = userManager;
    }
    public IActionResult Login()
    {
      if (this.User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "App");
      }
      return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
      if (ModelState.IsValid)
      {
        var result = await _signInManager.PasswordSignInAsync(
          model.UserName,
          model.Password,
          model.RememberMe,
          false
          );
        if (result.Succeeded)
        {
          if (Request.Query.Keys.Contains("ReturnUrl"))
          {
            return Redirect(Request.Query["ReturnUrl"].First());
          }
          else
          {
            return RedirectToAction("Shop", "App");
          }

        }
      }
      ModelState.AddModelError("", "Failed to login");
      return View();

    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index", "App");
    }

    [HttpPost]
    public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
    {
      try
      {
        if (ModelState.IsValid)
        {
          var user = await _userManager.FindByEmailAsync(model.UserName);

          if (user != null)
          {
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
              //create the token
              var claims = new[] {
              new Claim(JwtRegisteredClaimNames.Sub,user.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
            };
              var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
              var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

              var token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds

                );

              var results = new
              {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
              };

              return Created("", results);
            }

          }


        }
      }
      catch (Exception ex)
      {
        throw;
      }

      return BadRequest();
    }
  }
}