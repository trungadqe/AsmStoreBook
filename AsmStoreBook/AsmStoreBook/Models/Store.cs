using AsmStoreBook.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsmStoreBook.Models

{
    public class Store
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; } = null!;
        public string? Name { get; set; } = null!;
        public string? Address { get; set; } = null!;
        public string? Slogan { get; set; } = null!;
        public string? UId { get; set; } = null!;
        public AsmStoreBookUser?  User { get; set; } = null!;
        public virtual ICollection<Book>? Books { get; set; } = null!;

    }
}
