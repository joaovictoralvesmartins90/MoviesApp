using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);
    }
}
