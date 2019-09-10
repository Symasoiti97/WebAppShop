using System.Collections.Generic;
using System.Linq;
using DataBase.Models;

namespace WebApp.Models
{
    public class ItemsViewModel
    {
        public IEnumerable<Item> Items { get; set; }
    }
}