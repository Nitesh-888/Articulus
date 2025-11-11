using Articulus.BLL.Exceptions;
using Articulus.BLL.Exceptions.UserExceptions;
using Articulus.BLL.Users.Interfaces;
using Articulus.Data;
using Articulus.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Articulus.BLL.Users
{
    public class UserActionsService : IUserActionsService
    {
        private readonly AppDbContext _dbContext;
        public UserActionsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GiveFeedbackAsync(Guid userId, string text, int rating)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException("User not found.");
            }
            var newFeedback = new Feedback
            {
                Text = text,
                Rating = rating,
                UserId = userId,
                User = user
            };

            _dbContext.Feedbacks.Add(newFeedback);
            await _dbContext.SaveChangesAsync();

            return;
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _dbContext.Users
                .Include(u => u.UserCred)
                .Include(u => u.Articles)
                .Include(u => u.Comments)
                .Include(u => u.Reactions)
                .Include(u => u.Bookmarks)
                .Include(u => u.Feedbacks)
                .Include(u => u.Followers)
                .Include(u => u.Followees)
                .Include(u => u.Reports)
                .Include(u => u.Views)
                .SingleOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new UserNotFoundException("User not found.");
            }

            // Remove related entities first due to foreign key constraints
            _dbContext.Comments.RemoveRange(user.Comments);
            _dbContext.Reactions.RemoveRange(user.Reactions);
            _dbContext.Bookmarks.RemoveRange(user.Bookmarks);
            _dbContext.Feedbacks.RemoveRange(user.Feedbacks);
            _dbContext.UserFollows.RemoveRange(user.Followers);
            _dbContext.UserFollows.RemoveRange(user.Followees);
            _dbContext.Reports.RemoveRange(user.Reports);
            _dbContext.Articles.RemoveRange(user.Articles);
            _dbContext.UserCreds.Remove(user.UserCred);

            // Finally, remove the user
            _dbContext.Users.Remove(user);

            await _dbContext.SaveChangesAsync();

            return;
        }

        //Follow another user
        public async Task FollowUserAsync(Guid userId, Guid followeeId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            var followee = await _dbContext.Users.FindAsync(followeeId);
            if (user == null || followee == null)
            {
                throw new UserNotFoundException("User not found.");
            }
            if (userId == followeeId)
            {
                throw new CannotFollowSelfException();
            }
            if (await _dbContext.UserFollows.AnyAsync(uf => uf.FollowerId == userId && uf.FolloweeId == followeeId))
            {
                throw new AlreadyFollowingException();
            }

            var userFollow = new UserFollow
            {
                FollowerId = userId,
                Follower = user,
                FolloweeId = followeeId,
                Followee = followee
            };

            await _dbContext.UserFollows.AddAsync(userFollow);
            await _dbContext.SaveChangesAsync();

            return;
        }    

        //Unfollow another user
        public async Task UnfollowUserAsync(Guid userId, Guid followeeId)
        {
            var userFollow = await _dbContext.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == userId && uf.FolloweeId == followeeId);
            if (userFollow == null)
            {
                throw new NotFollowingException();
            }

            _dbContext.UserFollows.Remove(userFollow);
            await _dbContext.SaveChangesAsync();

            return;
        }
    }
}
