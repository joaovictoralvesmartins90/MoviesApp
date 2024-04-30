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

        public async Task Delete(int id)
        {
            await context.Genres.Where(g => g.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Genres.AnyAsync(x => x.Id == id);
        }

        public async Task<List<Genre>> GetAll()
        {
            return await context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre?> GetById(int id)
        {
            return await context.Genres.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Update(Genre genre)
        {
            context.Genres.Update(genre);
            await context.SaveChangesAsync();
        }
    }
}
