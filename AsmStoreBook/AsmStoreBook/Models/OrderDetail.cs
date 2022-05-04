
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsmStoreBook.Models

{
    public class OrderDetail
    {
        public int? OrderId { get; set; } = null!;
        public string? BookIsbn { get; set; } = null!;
        public double? Quantity { get; set; } = null!;
        public Order? Order { get; set; } = null!;

        public Book? Book { get; set; } = null!;

    }
}
