using System.ComponentModel.DataAnnotations;

namespace AuthorBook.Models
{
    public class Book
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        public string CoverImagePath { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}