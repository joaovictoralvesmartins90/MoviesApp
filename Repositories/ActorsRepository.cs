using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.DTOs;
using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public class ActorsRepository(ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor) : IActorsRepository
    {

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

        public async Task<List<Actor>> GetByName(string name)
        {
            return await context.Actors.Where(a => a.Name.Contains(name)).OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Actors.AnyAsync(x => x.Id == id);
        }

        public async Task<List<Actor>> GetAll(PaginationDTO pagination)
        {
            var queryable = context.Actors.AsQueryable();
            await httpContextAccessor.HttpContext!.InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync();
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
