using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly ApplicationDbContext context;

        public GenresRepository(ApplicationDbContext context) {
            this.context = context;
        }
        public async Task<int> Create(Genre genre)
        {
            context.Add(genre);//marca para ser inserido posteriormente
            await context.SaveChangesAsync();//de fato insere na base
            return genre.Id;//após inserir no banco, insere o id no objeto
        }

        public async Task<List<Genre>> GetAll()
        {
            return await context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetById(int id)
        {
            return await context.Genres.FirstOrDefaultAsync(g => g.Id.Equals(id));
        }
    }
}
