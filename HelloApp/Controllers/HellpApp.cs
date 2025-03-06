using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Authorization;

namespace HelloApp.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloAppController : ControllerBase
{
    private readonly IRegisterHelloBL _registerHelloBL;
    ResponseModel<LoginDTO> response;


    public HelloAppController(IRegisterHelloBL registerHelloBL)
    {
        _registerHelloBL = registerHelloBL;
    }

    [HttpGet]
    public string Get()
    {
        return _registerHelloBL.registration("value from controller");
    }

    //[HttpPost("login")]
    //public IActionResult PostData(LoginDTO loginDTO) //ResponseModel<LoginDTO>
    //{
    //    try
    //    {
    //        response = new ResponseModel<LoginDTO>();
    //        bool result = _registerHelloBL.loginuser(loginDTO);

    //        if (result)
    //        {
    //            response.Success = true;
    //            response.Message = "Login successfully";
    //            response.Data = loginDTO;
    //            return Ok(response);
    //        }

    //        response.Success = false;
    //        response.Message = "Login Failed";
    //        response.Data = loginDTO;
    //        return NotFound();
    //    }
    //    catch (Exception ex)
    //    {
    //        response.Success = false;
    //        response.Message = "Login Failed";
    //        response.Data = loginDTO;
    //        return BadRequest();

    //}
    //}

    [HttpPost("login")]
    public IActionResult Login(LoginDTO loginDTO)
    {
        try
        {
            bool result = _registerHelloBL.loginuser(loginDTO);

            if (!result)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var tokenService = HttpContext.RequestServices.GetRequiredService<TokenService>();
            string token = tokenService.GenerateToken(loginDTO.Email);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "An error occurred", error = ex.Message });
        }
    }


    [HttpPost("register")]
    public IActionResult RegisterUser(RegisterDTO newUser)
    {
        try
        {
            UserEntity user = _registerHelloBL.RegisterUser(newUser);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [Authorize]
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        try
        {
            List<AllUsersDTO> users = _registerHelloBL.GetAllUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }
}