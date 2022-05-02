using Microsoft.AspNetCore.Mvc.Rendering;
using AsmStoreBook.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsmStoreBook.Models

{
    public class Book
    {
        [Key]
        public string Isbn { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int? Pages { get; set; } = null!;
        public string Author { get; set; } = null!;
        public double? Price { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public string ImgUrl { get; set; } = null!;
        public int? StoreId { get; set; } = null!;
        public Store Store { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = null!;
        public int? CategoryId { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public virtual ICollection<Cart> Carts { get; set; } = null!;
    }
}
