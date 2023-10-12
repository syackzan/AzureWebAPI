using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{

    DataContextEF _entityFramework;

    IMapper _mapper;

    public UserEFController(IConfiguration config)
    {
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
        _entityFramework = new DataContextEF(config);

        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    //This will set up /User/GetUsers
    [HttpGet("GetUsers")]
    //public IEnumerable<User> GetUsers()
    public IEnumerable<User> GetUsers()
    {   

         IEnumerable<User>? users = _entityFramework.Users.ToList();

         if(users != null)
         {
            return users;
         }

        throw new Exception("Failed to get Users");
    }

    [HttpGet("GetSingleUser/{userId}")]

    public User GetSingleUser(int userId){

        User? user = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault();

        if(user != null)
        {
            return user;
        }
        
        throw new Exception("Failed to get User");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
     User? userDb = _entityFramework.Users.Where(u => u.UserId == user.UserId).FirstOrDefault();

        if(userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            userDb.Active = user.Active;

            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }
        
        throw new Exception("Failed to update user");
    }   

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        //Mapper
        User userDb = _mapper.Map<User>(user);
        
        // //Structure for changing Data if we are not using a Mapper
        // User userDb = new User();
        // userDb.FirstName = user.FirstName;
        // userDb.LastName = user.LastName;
        // userDb.Email = user.Email;
        // userDb.Gender = user.Gender;
        // userDb.Active = user.Active;

        _entityFramework.Add(userDb);
        if(_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        
        throw new Exception("Failed to Add user");
    }

    [HttpDelete("DeleteUser/{userId}")]
    
    public IActionResult DeleteUser(int userId)
    {   
        User? userDb = _entityFramework.Users.Where(u => u.UserId == userId).FirstOrDefault();

        if(userDb != null)
        {
            _entityFramework.Users.Remove(userDb);

            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
        }

        throw new Exception("Failed to Delete User");
    }


}
