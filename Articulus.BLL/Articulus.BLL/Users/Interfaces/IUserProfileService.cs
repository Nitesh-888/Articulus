using Articulus.DTOs.Users;

namespace Articulus.BLL.Users.Interfaces
{
    public interface IUserProfileService
    {
        public Task<GetProfileResponseDTO> GetUserProfileAsync(Guid userId);
        public Task<GetProfileResponseDTO> UpdateUserProfileAsync(Guid userId, UpdateProfileRequestDTO updateDto);
    }
}
