using RepositoryLayer.Interface;
using ModelLayer.DTO;
using BusinessLayer.Interface;
using RepositoryLayer.Entity;

namespace BusinessLayer.Service;

public class RegisterHelloBL : IRegisterHelloBL
{
    private readonly IRegisterHelloRL _registerHelloRL;

    public RegisterHelloBL(IRegisterHelloRL registerHelloRL)
    {
        _registerHelloRL = registerHelloRL;

    }

    public string registration(string name)
    {
        return "sending from business layer: "+ _registerHelloRL.GetHello(name);
    }

    public bool loginuser(LoginDTO loginDTO)
    {
        string frontendUsername = loginDTO.Email;
        string frontendPassword = loginDTO.Password;

        LoginDTO result = _registerHelloRL.getUserPassword(loginDTO);

        bool res = checkUserPassword(frontendUsername, frontendPassword, result);

        return res;
    }

    private bool checkUserPassword(string frontendUsername, string frontendPassword, LoginDTO result)
    {
        if(result == null)
        {
            return false;
        }
        if (frontendUsername == result.Email && frontendPassword == result.Password)
        {
            return true;
        }
        return false;
    }

    public UserEntity RegisterUser(RegisterDTO newUser)
    {
        return _registerHelloRL.RegisterUser(newUser);
    }

    public List<AllUsersDTO> GetAllUsers()
    {
        return _registerHelloRL.GetAllUsers();
    }
}