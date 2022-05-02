namespace AsmStoreBook.Models
{
    public class Category
    {
        public int? Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<Book>? Books { get; set; } = null!;
    }
}
