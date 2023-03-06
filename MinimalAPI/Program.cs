using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.DBContext;
using MinimalAPI.Dto;
using MinimalAPI.IRepository;
using MinimalAPI.Models;
using MinimalAPI.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Configure the HTTP request pipeline.
var appServiceConnString = "Endpoint=https://myappconfiguration003.azconfig.io;Id=ufzu-li-s0:TXbEqy6g5DkdIJ0FxS6U;Secret=TY7FSk68ryrzePo/ppSgMMl9GNTllG9P3EUAuvqITbQ=";
builder.Host.ConfigureAppConfiguration(builder =>
{
    builder.AddAzureAppConfiguration(appServiceConnString);
});

var sqlconBuilder = new SqlConnectionStringBuilder();
sqlconBuilder.ConnectionString = builder.Configuration["DevConnection"];



builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlconBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/v1/commands", async (ICommandRepository repo, IMapper mapper) =>
{
    var commands = await repo.GetAllCommands();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
}).WithDisplayName("CommandNames");
//app.MapGet("api/v2/commands", async (ICommandRepository repo, IMapper mapper) =>
//{
//    var commands = await repo.GetAllCommands();
//    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
//});
app.MapGet("api/v1/commands/{id}", async (ICommandRepository repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandById(id);
    if (command != null)
    {
        return Results.Ok(mapper.Map<CommandReadDto>(command));
    }
    return Results.NotFound();
});
app.MapPost("api/v1/commands", async (ICommandRepository repo, IMapper mapper, CommandCreateDto cmdCreateDto) =>
{
    
    var commandModel = mapper.Map<Command>(cmdCreateDto);

    await repo.CreateCommand(commandModel);
    await repo.SaveChanges();

    var cmdReadDto = mapper.Map<CommandReadDto>(commandModel);

    return Results.Created($"api/v1/commands/{cmdReadDto.Id}", cmdReadDto);

});
app.MapPut("api/v1/commands", async (ICommandRepository repo, IMapper mapper, int id, CommandUpdateDto cmdUpdateDto) =>
{ 
    var command = await repo.GetCommandById(id);
    if (command == null)
    {
            return Results.NotFound();
    }
    mapper.Map(cmdUpdateDto, command);
    await repo.SaveChanges();
    return Results.NoContent();
});
app.MapDelete("api/v1/commands/{id}", async (ICommandRepository repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandById(id);
    if (command == null)
    {
        return Results.NotFound();
    }

    repo.DeleteCommand(command);

    await repo.SaveChanges();

    return Results.NoContent();

});

app.Run();

