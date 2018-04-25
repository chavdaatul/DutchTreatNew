using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DutchTreat.ViewModels;
using DutchTreat.Services;
using DutchTreat.Data;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DutchTreat.Controllers
{
  public class AppController : Controller
  {
    private readonly IMailService _mailService;
    private readonly IDutchRepository _repository;

    public AppController(IMailService mailService, IDutchRepository repository)
    {
      this._mailService = mailService;
      this._repository = repository;      
    }
    public IActionResult Index()
    {
      return View();
    }

    [HttpGet("Contact")]
    public IActionResult Contact()
    {
      ViewBag.Title = "Contact Us";
      return View();
    }

    [HttpPost("Contact")]
    public IActionResult Contact(ContactViewModel model)
    {
      ViewBag.Title = "Contact Us";
      if (ModelState.IsValid)
      {
        //send the Email
        _mailService.SendMessage("chavda.atul@nexuslinkservices.in", model.Subject, $"From:{ model.Name} - { model.Email}, Message : { model.Message} ");
        ViewBag.Message = "Mail Sent";
        ModelState.Clear();
      }
      else
      {
        //show the error
      }

      return View();
    }
    public IActionResult About()
    {
      ViewBag.Title = "About Us";
      return View();
    }

    [Authorize]
    public IActionResult Shop()
    {
      var result = _repository.GetAllProducts();
      return View(result.ToList());
    }


  }
}
