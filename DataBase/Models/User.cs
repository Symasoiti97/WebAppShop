using System;
using System.Collections.Generic;

namespace DataBase.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        
        public virtual ICollection<Order> Order { get; set; }
    }
}