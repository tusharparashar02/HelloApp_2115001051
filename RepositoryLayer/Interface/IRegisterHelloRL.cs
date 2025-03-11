using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IRegisterHelloRL
    {
        public string GetHello(string name);
        public LoginDTO getUserPassword(LoginDTO loginDTO);
        public UserEntity RegisterUser(RegisterDTO newUser);
        public List<AllUsersDTO> GetAllUsers();
    }
}
