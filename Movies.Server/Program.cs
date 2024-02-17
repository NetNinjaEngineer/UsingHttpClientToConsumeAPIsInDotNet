using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Movies.API.Contexts;
using Movies.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());

})
    .AddNewtonsoftJson()
    .ConfigureApiBehaviorOptions(setupAction =>
    {
        setupAction.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MoviesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ITrailersRepository, TrailersRepository>();
builder.Services.AddScoped<IPostersRepository, PostersRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
