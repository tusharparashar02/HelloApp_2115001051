using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Service
{
    public class RegisterHelloRL : IRegisterHelloRL
    {
        private readonly HelloAppContext _context;
        public RegisterHelloRL(HelloAppContext context)
        {
            _context = context;
        }

        public string GetHello(string name)
        {
            return "From repository layer: " + name;
        }

        public LoginDTO getUserPassword(LoginDTO loginDTO)
        {
            var result = _context.UserEntities.FirstOrDefault<UserEntity>(user => user.Email == loginDTO.Email);
            if(result == null)
            {
                return null;
            }

            loginDTO.Email = result.Email;
            loginDTO.Password = result.Password;

            return loginDTO;
        }

        public UserEntity RegisterUser(RegisterDTO newUser)
        {
            var result = _context.UserEntities.FirstOrDefault<UserEntity>(user => user.Email == newUser.Email);
            if (result == null)
            {
                var user = new UserEntity
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = newUser.Email,
                    Password = newUser.Password
                };
                _context.UserEntities.Add(user);
                _context.SaveChanges();
                return user;
            }
            return result;
        }

        public List<AllUsersDTO> GetAllUsers()
        {
            var users = _context.UserEntities.Select(user => new AllUsersDTO
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }).ToList();
            return users;
        }
    }
}
