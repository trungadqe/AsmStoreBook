using AsmStoreBook.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsmStoreBook.Models

{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; } = null!;
        public string? UId { get; set; } = null!;
        public DateTime? OrderDate { get; set; } = null!;
        public double? Total { get; set; } = null!;
        public AsmStoreBookUser? User { get; set; } = null!;
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; } = null!;

    }
}
