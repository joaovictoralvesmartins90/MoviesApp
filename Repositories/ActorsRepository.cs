using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public class ActorsRepository : IActorsRepository
    {
        private readonly ApplicationDbContext context;

        public ActorsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task Delete(int id)
        {
            await context.Actors.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Actors.AnyAsync(x => x.Id == id);
        }

        public async Task<List<Actor>> GetAll()
        {
            return await context.Actors.OrderBy(x => x.Name).ToListAsync();
        }

        public Task<Actor?> GetById(int id)
        {
            return context.Actors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Update(Actor actor)
        {
            context.Actors.Update(actor);
            await context.SaveChangesAsync();
        }
    }
}
