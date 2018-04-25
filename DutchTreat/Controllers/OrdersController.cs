using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using Microsoft.Extensions.Logging;
using DutchTreat.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace DutchTreat.Controllers
{
  [Route("api/[Controller]")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  public class OrdersController : Controller
  {
    private readonly IDutchRepository _repository;
    private readonly ILogger<OrdersController> _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<StoreUser> _userManager;

    public OrdersController(IDutchRepository repository,
      ILogger<OrdersController> logger,
      IMapper mapper,
      UserManager<StoreUser> userManager
      )
    {
      this._repository = repository;
      this._logger = logger;
      this._mapper = mapper;
      this._userManager = userManager;
    }
    [HttpGet]
    public IActionResult Get(bool includeItems = true)
    {
      try
      {
        var userName = User.Identity.Name;

        var results = _repository.GetAllOrdersByUser(userName, includeItems);
        return Ok(_mapper.Map<IEnumerable<Order>, List<OrderViewModel>>(results));
      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to get orders:{ex}");
        return BadRequest($"Failed to get orders");
      }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
      try
      {
        var order = _repository.GetOrderById(User.Identity.Name, id);
        if (order != null)
        {
          return Ok(_mapper.Map<Order, OrderViewModel>(order));
        }
        else
        {
          return NotFound();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to get orders:{ex}");
        return BadRequest($"Failed to get orders");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody]OrderViewModel model)
    {
      try
      {
        if (ModelState.IsValid)
        {
          var newOrder = _mapper.Map<OrderViewModel, Order>(model);

          //var newOrder = new Order()
          //{
          //  OrderDate = model.OrderDate,
          //  OrderNumber = model.OrderNumber,
          //  Id = model.OrderId
          //};

          if (newOrder.OrderDate == DateTime.MaxValue)
          {
            newOrder.OrderDate = DateTime.Now;
          }

          var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
          newOrder.User = currentUser;

          _repository.AddEntity(newOrder);
          if (_repository.SaveAll())
          {

            //var vm = new OrderViewModel()
            //{
            //  OrderDate = newOrder.OrderDate,
            //  OrderId = newOrder.Id,
            //  OrderNumber = newOrder.OrderNumber
            //};

            return Created($"/api/orders/{newOrder.Id}", _mapper.Map<Order, OrderViewModel>(newOrder));
          }
        }
        else
        {
          return BadRequest(ModelState);
        }

      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to save new order:{ex}");
      }
      return BadRequest("Failed to save new order");
    }
  }
}