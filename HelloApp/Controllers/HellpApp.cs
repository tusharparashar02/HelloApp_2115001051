    using Microsoft.AspNetCore.Mvc;
    using BusinessLayer.Interface;
    using ModelLayer.DTO;
    using RepositoryLayer.Entity;
    using BusinessLayer.Service;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Configuration;
    using RabbitMCQProducer.Service;
    using BusinessLayer.Service;

    namespace HelloApp.Controllers
    //RabbitMqConsumer
    {
        [ApiController]
        [Route("[controller]")]
        public class HelloAppController : ControllerBase
        {
            private readonly IRegisterHelloBL _registerHelloBL;
            private readonly TokenService _jwtService;
            private readonly RabbitMqProducer _rabbitMQProducer;

            // Constructor with dependencies
            public HelloAppController(IRegisterHelloBL registerHelloBL, TokenService jwtService, RabbitMqProducer rabbitMQProducer)
            {
                _registerHelloBL = registerHelloBL;
                _jwtService = jwtService;
                _rabbitMQProducer = rabbitMQProducer;
            }

            // Default GET route
            [HttpGet]
            public string Get()
            {
                return _registerHelloBL.registration("value from controller");
            }

            // Login method
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

                    // Generate JWT Token for login
                    string token = _jwtService.GenerateToken(loginDTO.Email);

                    return Ok(new { token });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "An error occurred", error = ex.Message });
                }
            }

            // Register method
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
                    return BadRequest(new { message = "Registration failed", error = ex.Message });
                }
            }

            // Get users method (protected by Authorization)
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
                    return BadRequest(new { message = "Failed to retrieve users", error = ex.Message });
                }
            }

            // Forgot Password method (generates reset token and sends email via RabbitMQ)
            [HttpPost("forgot-password")]
            public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
            {
                try
                {
                    // Validate the email
                    if (string.IsNullOrEmpty(request.Email))
                    {
                        return BadRequest("Email is required.");
                    }

                   

                    // Create the email payload
                    var message = new
                    {
                        To = request.Email,
                        Subject = "Reset Your Password",
                        Body = $"Click the link to reset your password: https://yourdomain.com/reset-password?token={"resetToken"}"
                    };

                    // Publish the message to RabbitMQ for email sending
                    _rabbitMQProducer.PublishMessage(message);

                    return Ok("Password reset email has been sent.");
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Error occurred while processing the password reset", error = ex.Message });
                }
            }
        }

        // ForgotPasswordRequest model for capturing the email for reset
        public class ForgotPasswordRequest
        {
            public string Email { get; set; }
        }
    }
    