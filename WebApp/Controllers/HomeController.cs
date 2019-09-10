using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using BusinessLogic.DataManager;
using DataBase.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApp.Models;
using Z.EntityFramework.Plus;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOperationDb _operationDb;
        
        public HomeController(IOperationDb operationDb)
        {
            _operationDb = operationDb;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var items = _operationDb.GetModels<Item>().Include(u => u.User);
            
            var indexVM = new ItemsViewModel
            {
                Items = items
            };

            return View(indexVM);
        }
        
        [HttpGet]
        public IActionResult Items()
        {
            var items = _operationDb.GetModels<Item>(u => u.UserId == new Guid(User.FindFirstValue("UserId")));
            
            var indexVM = new ItemsViewModel
            {
                Items = items
            };

            return View(indexVM);
        }
        
        [HttpGet]
        public IActionResult AddItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddItem(AddItemViewModel addItemVM)
        {
            var newItem = new Item
            {
                Name = addItemVM.NewItem.Name,
                Quantity = addItemVM.NewItem.Quantity,
                Price = addItemVM.NewItem.Price,
                UserId = new Guid(User.FindFirstValue("UserId"))
            };
            
            _operationDb.CreateModel(newItem);
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Orders()
        {
            var orders = _operationDb.GetModels<Order>().Include(u => u.User).Include(i => i.Item);

            var ordersVM = new OrdersViewModel
            {
                Orders = orders
            };
            
            return View(ordersVM);
        }
        
        [HttpGet]
        public IActionResult DeleleOrder(Guid orderId)
        {
            _operationDb.GetModels<Order>(i => i.Id == orderId).Delete();
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EditItem(Guid itemId)
        {
            var editItemVM = new EditItemViewModel
            {
                Item = _operationDb.GetModels<Item>(i => i.Id == itemId).First()
            };
            
            return View(editItemVM);
        }
        
        [HttpPost]
        public IActionResult EditItem(EditItemViewModel editItemVM)
        {
            _operationDb.GetModels<Item>(i => i.Id == editItemVM.Item.Id).Update(i => new Item
            {
                Name = editItemVM.Item.Name,
                Quantity = editItemVM.Item.Quantity,
                Price = editItemVM.Item.Price    
            });
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> BuyItem(Guid itemId, int count)
        {
            var transactionOptions = new TransactionOptions() {IsolationLevel = IsolationLevel.RepeatableRead};
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var items = _operationDb.GetModels<Item>(i => i.Id == itemId);
                var item = await items.FirstOrDefaultAsync();

                if (item.Quantity >= count)
                {
                    var newOrder = new Order
                    {
                        UserId = new Guid(User.FindFirstValue("UserId")),
                        ItemId = itemId,
                        Quantity = count,
                        TotalPrice = count * item.Price
                    };

                    items.Update(i => new Item
                    {
                        Quantity = i.Quantity - count
                    });
                
                    _operationDb.CreateModel(newOrder);
                }
                
                transaction.Complete();
            
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult LogIn(string name)
        {
            var user = _operationDb.GetModels<User>(u => u.Name == name).FirstOrDefault();

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                    new Claim("UserId", user.Id.ToString())
                };
                
                var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

    }
}