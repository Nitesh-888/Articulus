using Microsoft.EntityFrameworkCore;
using Articulus.DTOs.Users;
using Articulus.BLL.Exceptions;
using Articulus.BLL.Exceptions.UserExceptions;
using Articulus.BLL.Users.Interfaces;
using Articulus.Data;
using Articulus.Data.Models;

namespace Articulus.BLL.Users
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _dbContext;
        public UserProfileService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetProfileResponseDTO> GetUserProfileAsync(Guid userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserCred)
                .Include(u => u.Followers)
                .Include(u => u.Followees)
                .Include(u => u.Articles)
                .Include(u => u.Reactions)
                .Include(u => u.Bookmarks)
                .Include(u => u.Feedbacks)
                .Include(u => u.Comments)
                .Include(u => u.Reports)
                .SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            var response = new GetProfileResponseDTO
            {
                UserId = user.UserId,
                ProfileImageUrl = user.ProfileImageUrl,
                Bio = user.Bio,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.UserCred.Email,
                Username = user.UserCred.Username,
                FollowersCount = user.Followers.Count,
                FolloweesCount = user.Followees.Count,
                ArticlesCount = user.Articles.Count,
                CommentsCount = user.Comments.Count,
                ReactionsCount = user.Reactions.Count,
                BookmarksCount = user.Bookmarks.Count,
                ReportsCount = user.Reports.Count,
                FeedbacksCount = user.Feedbacks.Count,
                Country = user.Country,
                City = user.City,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                TimeZone = user.TimeZone
            };
            return response;
        }

        public async Task<GetProfileResponseDTO> UpdateUserProfileAsync(Guid userId, UpdateProfileRequestDTO updateDto)
        {
            var user = await _dbContext.Users.Include(u => u.UserCred).SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            //checking for unique email
            if (updateDto.Email != null && updateDto.Email != user.UserCred.Email)
            {
                var emailExists = await _dbContext.UserCreds.AnyAsync(u => u.Email == updateDto.Email);
                if (emailExists)
                {
                    throw new EmailAlreadyExistsException(updateDto.Email);
                }
            }

            //checking for unique username
            if (updateDto.Username != null && updateDto.Username != user.UserCred.Username)
            {
                var usernameExists = await _dbContext.UserCreds.AnyAsync(u => u.Username == updateDto.Username);
                if (usernameExists)
                {
                    throw new UsernameAlreadyExistsException(updateDto.Username);
                }
            }

            user.ProfileImageUrl = updateDto.ProfileImageUrl;
            user.Bio = updateDto.Bio;
            user.FirstName = updateDto.FirstName ?? user.FirstName;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.UserCred.Email = updateDto.Email ?? user.UserCred.Email;
            user.UserCred.Username = updateDto.Username ?? user.UserCred.Username;
            user.Country = updateDto.Country ?? user.Country;
            user.City = updateDto.City ?? user.City;
            user.Address = updateDto.Address ?? user.Address;
            user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = updateDto.DateOfBirth ?? user.DateOfBirth;
            user.Gender = updateDto.Gender ?? user.Gender;
            user.UpdatedAt = DateTime.UtcNow;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new GetProfileResponseDTO
            {
                UserId = user.UserId,
                    ProfileImageUrl = user.ProfileImageUrl,
                    Bio = user.Bio,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.UserCred.Email,
                    Username = user.UserCred.Username,
                    FollowersCount = user.Followers.Count,
                    FolloweesCount = user.Followees.Count,
                    ArticlesCount = user.Articles.Count,
                    CommentsCount = user.Comments.Count,
                    ReactionsCount = user.Reactions.Count,
                    BookmarksCount = user.Bookmarks.Count,
                    ReportsCount = user.Reports.Count,
                    FeedbacksCount = user.Feedbacks.Count,
                    Country = user.Country,
                    City = user.City,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    TimeZone = user.TimeZone
            };
        }
    }
}
