using System.ComponentModel.DataAnnotations;

namespace Movies.DataTransferObjects
{
    public class UserDto
    {
        [Required]
        public string Name { get; set; }

        public List<CategoryDto> InterestedCategories { get; set; }
    }
}
