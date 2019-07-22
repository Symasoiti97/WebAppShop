using System;
using System.Collections.Generic;

namespace DataBase.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        
        public virtual ICollection<Order> Order { get; set; }
    }
}