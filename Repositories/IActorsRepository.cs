﻿using MoviesApp.Entities;

namespace MoviesApp.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
        Task<bool> Exists(int id);
        Task Update(Actor actor);
        Task Delete(int id);
    }
}
