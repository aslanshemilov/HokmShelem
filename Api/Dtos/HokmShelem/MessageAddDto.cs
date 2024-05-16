namespace Api.Dtos.HokmShelem
{
    public class MessageAddDto
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(100)]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        [StringLength(5000)]
        public string Message { get; set; }
    }
}
