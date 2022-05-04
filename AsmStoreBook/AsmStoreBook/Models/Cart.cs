using AsmStoreBook.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsmStoreBook.Models

{
    public class Cart
    {
        public string UId { get; set; } = null!;
        public string? BookIsbn { get; set; } = null!;
        public AsmStoreBookUser? User { get; set; } = null!;
        public Book? Book { get; set; } = null!;
        public double? Quantity { get; set; } = null!;
        public double? UnitPrice { get; set; } = null!;
        public double? TotalPrice { get; set; } = null!;
    }
}
