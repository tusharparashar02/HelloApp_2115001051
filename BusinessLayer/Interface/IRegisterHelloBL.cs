using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IRegisterHelloBL
    {
        public string registration(string name);
        public bool loginuser(LoginDTO loginDTO);
        public UserEntity RegisterUser(RegisterDTO newUser);
        public List<AllUsersDTO> GetAllUsers();
    }
}
