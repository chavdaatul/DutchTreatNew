using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
  public class DutchRepository : IDutchRepository
  {
    private readonly DutchContext _ctx;
    private readonly ILogger<DutchRepository> _logger;

    public DutchRepository(DutchContext ctx, ILogger<DutchRepository> logger)
    {

      this._ctx = ctx;
      this._logger = logger;
    }

    #region Products
    public IEnumerable<Product> GetAllProducts()
    {
      try
      {
        _logger.LogInformation("GetAllProducts was called");
        return _ctx.Products
          .OrderBy(p => p.Title)
          .ToList();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to get all products:{ex}");
        return null;
      }

    }

    public IEnumerable<Product> GetProductByCategory(string Category)
    {
      return _ctx.Products
        .Where(p => p.Category == Category)
        .ToList();
    }
    #endregion

    #region Orders
    public IEnumerable<Order> GetAllOrders(bool includeItems)
    {
      try
      {
        if (includeItems)
        {
          return _ctx.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
        }
        else
        {
          return _ctx.Orders
                    .ToList();
        }

      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to get all orders:{ex}");
        return null;
      }

    }

    public Order GetOrderById(string username, int id)
    {
      try
      {
        return _ctx.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .Where(p => p.Id == id && p.User.UserName == username)
                    .FirstOrDefault();
      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to get order:{ex}");
        return null;
      }
    }

    public void AddEntity(object model)
    {
      _ctx.Add(model);
    }

    #endregion

    public bool SaveAll()
    {
      return _ctx.SaveChanges() > 0;
    }

    public IEnumerable<Order> GetAllOrdersByUser(string userName, bool includeItems)
    {
      try
      {
        if (includeItems)
        {
          return _ctx.Orders
            .Where(o => o.User.UserName == userName)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ToList();
        }
        else
        {
          return _ctx.Orders
            .Where(o => o.User.UserName == userName)
                    .ToList();
        }

      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed to get all orders:{ex}");
        return null;
      }
    }
  }
}
