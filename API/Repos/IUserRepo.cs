using Shared.DTOs;

namespace API.Repos
{
    public interface IUserRepo
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
    }
}
