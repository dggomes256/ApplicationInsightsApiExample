using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Domain.Entities
{
    public  class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string OrderDescription { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        [NotMapped]
        public decimal TotalValue { get; set; }
    }
}