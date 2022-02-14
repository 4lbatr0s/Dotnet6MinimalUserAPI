using System;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using UserApi.Properties;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Users") ?? "Data Source=Users.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<UserDbContext>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
}

app.MapFallback(() => Results.Redirect("/swagger"));



//bring all users
app.MapGet("/users", async (UserDbContext db) =>
    await db.Users.Select(x=> new UserDto(x)).ToListAsync());


//bring user by id
app.MapGet("/users/{id}", async (int id, UserDbContext db) =>
    await db.Users.FindAsync(id)
        is User user
            ? Results.Ok(new UserDto(user))
            : Results.NotFound(Messages.UserIsNotFound));


//bring users filtering by date:MM.dd.yyyy
app.MapGet("/users/date", async (string date, UserDbContext db) =>
    await db.Users.Where(u=> u.CreatedAt == date).ToListAsync());



//crete multiple users at once
app.MapPost("/users/createmultiple", async (UserDto[] userDtos, UserDbContext db) =>
{
    var randomGenerator = new Random();
    int randomNumber = randomGenerator.Next(5000,10000);
    List<int> userIds = new List<int>(); 


    foreach (var userDto in userDtos)
    {
         var user = new User
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Surname = userDto.Surname,
            Gender = userDto.Gender,
            BirthDate = userDto.BirthDate,
            CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")
        };
        db.Users.Add(user); //add to db.
        userIds.Add(user.Id); //save ids
        await db.SaveChangesAsync();
    }

    return Results.Created($"/users/{randomNumber}", userIds); //return a list of created users' ids at a symbolic endpoint
});


//create single user at once.
app.MapPost("/users/create", async (UserDto userDto, UserDbContext db) =>
{
    var user = new User
    {
        Id = userDto.Id,
        Name = userDto.Name,
        Surname = userDto.Surname,
        Gender = userDto.Gender,
        BirthDate = userDto.BirthDate,
        CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")
    };
    
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
});


//update user
app.MapPut("/users/{id}", async (int id, UserDtoForUpdate inputUserDto, UserDbContext db) =>
{
    var user = await db.Users.FindAsync(id);

    if (user is null) return Results.NotFound(Messages.UserIsNotFound);

    
    user.Name = inputUserDto.Name;
    user.Surname = inputUserDto.Surname;
    user.Gender = inputUserDto.Gender;
    user.BirthDate = inputUserDto.BirthDate;
    if(inputUserDto.CreatedAt is null){
        user.CreatedAt = user.CreatedAt;
    } else { //12.01.2022 => 12, 01, 2022 => 12 month, 1 day, 2022 year => 0 1 2  
        user.CreatedAt = inputUserDto.CreatedAt;
    }

    await db.SaveChangesAsync();

    return Results.Ok(Messages.UserIsUpdated(user.Id));
});


//delete user.
app.MapDelete("/users/{id}", async (int id, UserDbContext db) =>
{
    if (await db.Users.FindAsync(id) is User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.Ok(Messages.UserIsDeleted(id));
    }

    return Results.NotFound(Messages.UserIsNotFound);
});

app.Run();
