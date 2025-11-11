using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Articulus.BLL.Users.Interfaces
{
    public interface IUserActionsService
    {
        public Task GiveFeedbackAsync(Guid userId, string text, int rating);
        public Task DeleteUserAsync(Guid userId);
        public Task FollowUserAsync(Guid userId, Guid followeeId);
        public Task UnfollowUserAsync(Guid userId, Guid followeeId);
    }
}
