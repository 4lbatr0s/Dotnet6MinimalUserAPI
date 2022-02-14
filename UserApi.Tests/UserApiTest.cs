using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using UserApi.Properties;
using Xunit;

namespace UserApi.UnitTest
{

public class UserApiTest
{
    [Fact]
    public async Task GetUsers()
    {
        await using var application = new UserApplication();
        var client = application.CreateClient();
        var users = await client.GetFromJsonAsync<List<User>>("/users");
        Assert.Empty(users);
    }

     [Fact]
    public async Task GetUserById()
    {
        await using var application = new UserApplication();
        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync("/users/create", new User{Id=1, Name = "serhat", Surname = "oner", Gender = "male", BirthDate= "15.12.1998",CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")});
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var users = await client.GetFromJsonAsync<List<User>>("/users");
        var user = Assert.Single(users);
        Assert.Equal(1,user.Id);
        Assert.Equal("serhat",user.Name);
        Assert.Equal("oner",user.Surname);
        Assert.Equal("male",user.Gender);
        Assert.Equal("15.12.1998", user.BirthDate);
        Assert.Equal(DateTime.Now.ToString("MM.dd.yyyy"), user.CreatedAt);

        response = await client.GetAsync($"/users/{user.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    }

    //create multiple user's been used here.
    [Fact] 
    public async Task GetUsersByDate()
    {

        await using var application = new UserApplication();
        var client = application.CreateClient();
        List<User> userList = new List<User>(){
            new User{Id=1, Name = "serhat", Surname = "oner", Gender = "male", BirthDate= "15.12.1998",CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")},
            new User{Id=2, Name = "leonard", Surname = "cohen", Gender = "male", BirthDate= "20.12.1948",CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")}
        };
        var response = await client.PostAsJsonAsync("/users/createmultiple", userList);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var currentDate = DateTime.Now.ToString("MM.dd.yyyy");
        var users = await client.GetFromJsonAsync<List<User>>($"/users/date?date={currentDate}");


    }


    //be able to create a user.
    [Fact]
    public async Task PostUsers()
    {
        await using var application = new UserApplication();
        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync("/users/create", new User{Id=1, Name = "serhat", Surname = "oner", Gender = "male", BirthDate= "15.12.1998",CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")});
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var users = await client.GetFromJsonAsync<List<User>>("/users");
        var user = Assert.Single(users);
        Assert.Equal(1,user.Id);
        Assert.Equal("serhat",user.Name);
        Assert.Equal("oner",user.Surname);
        Assert.Equal("male",user.Gender);
        Assert.Equal("15.12.1998", user.BirthDate);
        Assert.Equal(DateTime.Now.ToString("MM.dd.yyyy"), user.CreatedAt);

    }

    [Fact]
    public async Task UpdateUsers()
    {
        await using var application = new UserApplication();
        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync("/users/create", new User{Id=1, Name = "serhat", Surname = "oner", Gender = "male", BirthDate= "15.12.1998",CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")});
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var users = await client.GetFromJsonAsync<List<User>>("/users");
        var user = Assert.Single(users);
        Assert.Equal(1,user.Id);
        Assert.Equal("serhat",user.Name);
        Assert.Equal("oner",user.Surname);
        Assert.Equal("male",user.Gender);
        Assert.Equal("15.12.1998", user.BirthDate);
        Assert.Equal(DateTime.Now.ToString("MM.dd.yyyy"), user.CreatedAt);
        var currentId = user.Id;
        var responseUpdate = await client.PutAsJsonAsync($"users/{currentId}", new UserDtoForUpdate{Name = "Leonard", Surname = "Cohen", Gender = "Supermale", BirthDate = "12.01.1938", CreatedAt = "12.02.2022"});
        var updatedUser = await client.GetFromJsonAsync<User>($"/users/{currentId}");
        Assert.Equal("Leonard",updatedUser.Name);
        Assert.Equal("Cohen",updatedUser.Surname);
        Assert.Equal("Supermale",updatedUser.Gender);
        Assert.Equal("12.01.1938", updatedUser.BirthDate);
        Assert.Equal("12.02.2022", updatedUser.CreatedAt);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    //be able to delete a user.
    [Fact]
    public async Task DeleteUsers()
    {
        await using var application = new UserApplication();
        var client = application.CreateClient();
        var response = await client.PostAsJsonAsync("/users/create", new User{Id=1, Name = "serhat", Surname = "oner", Gender = "male", BirthDate= "15.12.1998",CreatedAt = DateTime.Now.ToString("MM.dd.yyyy")});
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var users = await client.GetFromJsonAsync<List<User>>("/users");
        var user = Assert.Single(users);
        Assert.Equal(1,user.Id);
        Assert.Equal("serhat",user.Name);
        Assert.Equal("oner",user.Surname);
        Assert.Equal("male",user.Gender);
        Assert.Equal("15.12.1998", user.BirthDate);
        Assert.Equal(DateTime.Now.ToString("MM.dd.yyyy"), user.CreatedAt);

        response = await client.DeleteAsync($"/users/{user.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response = await client.GetAsync($"/users/{user.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
    
    class UserApplication : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<UserDbContext>));
                services.AddDbContext<UserDbContext>(options =>
                    options.UseInMemoryDatabase("Testing", root));
            });
            return base.CreateHost(builder);
        }
    }
}

