using Shared.DTOs;

namespace API.Repos
{
    public interface IAccountRepo
    {
        Task<Response> RegisterAsync(RegisterDTO registerDTO, string role);
        Task<Response> LoginAsync(LoginDTO loginDTO);
    }
}
