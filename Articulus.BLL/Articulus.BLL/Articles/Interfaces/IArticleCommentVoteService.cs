using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Articulus.BLL.Articles.Interfaces
{
    public interface IArticleCommentVoteService
    {
        public Task UpvoteAsync(Guid userId, Guid articleId, Guid commentId);
        public Task DownvoteAsync(Guid userId, Guid articleId, Guid commentId);
    }
}
