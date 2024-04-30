namespace MoviesApp.DTOs
{
    public class CreateActorDTO
    {      
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public IFormFile? Picture { get; set; }
    }
}
