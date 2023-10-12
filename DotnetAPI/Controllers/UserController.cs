using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    //This will set up /User/GetUsers
    [HttpGet("GetUsers")]
    //public IEnumerable<User> GetUsers()
    public IEnumerable<User> GetUsers()
    {   
        string sql = @"
        SELECT  [UserId]
            , [FirstName]
            , [LastName]
            , [Email]
            , [Gender]
            , [Active]
         FROM  TutorialAppSchema.Users";

         IEnumerable<User> users = _dapper.LoadData<User>(sql);

        return users;

        // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        // {
        //     Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //     TemperatureC = Random.Shared.Next(-20, 55),
        //     Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        // })
        // .ToArray();
    }

    [HttpGet("GetSingleUser/{userId}")]

    public User GetSingleUser(int userId){
        return _dapper.LoadDataSingle<User>("SELECT * FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString());
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName + 
            "', [LastName] = '" + user.LastName + 
            "', [Email] = '" + user.Email +
            "', [Gender] = '"  + user.Gender +
            "', [Active] = '" + user.Active + 
            "' WHERE UserId = " + user.UserId;
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed To Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.Users(
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
        ) 
        VALUES 
        (" +
            "'" + user.FirstName + 
            "', '" + user.LastName + 
            "', '" + user.Email +
            "', '"  + user.Gender +
            "', '" + user.Active + 
        "')";

        Console.WriteLine(sql);

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    
    public IActionResult DeleteUser(int userId)
    {   
        string sql = "DELETE FROM TutorialAppSchema.Users WHERE UserId = " +  userId.ToString();

        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }


}
