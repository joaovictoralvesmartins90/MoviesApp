using Microsoft.EntityFrameworkCore;
using MoviesApp.DatabaseContext;
using MoviesApp.Endpoints;
using MoviesApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

//Serviços
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["AllowedOrigins"]!).AllowAnyOrigin().AllowAnyHeader();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("desenv_mysql_desktop")!);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddAutoMapper(typeof(Program));
//Serviços

//Os serviços só podem ser alterados antes da aplicação ser criada, por isso a linha abaixo vem em seguida.
var app = builder.Build();

//middleware
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseOutputCache();
//middleware

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();

app.Run();

