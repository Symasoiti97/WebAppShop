using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        
        
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }

        [ForeignKey(nameof(User))]
        public virtual User User { get; set; }
        [ForeignKey(nameof(Item))]
        public virtual Item Item { get; set; }
    }
}