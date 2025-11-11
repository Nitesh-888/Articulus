using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;
using Articulus.Data;
using Articulus.Data.Models;

namespace Articulus.BLL.Articles
{
    public class ArticleCommentVoteService : IArticleCommentVoteService
    {
        private readonly AppDbContext _dbContext;
        public ArticleCommentVoteService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task UpvoteAsync(Guid userId, Guid articleId, Guid commentId)
        {
            var comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment == null || comment.ArticleId != articleId)
            {
                throw new VoteNotFoundException();
            }
            var existingVote = await _dbContext.CommentVotes.FindAsync(userId, commentId);
            if (existingVote != null)
            {
                if (existingVote.Value == 1)
                {
                    _dbContext.CommentVotes.Remove(existingVote);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                else
                {
                    existingVote.Value = 1;
                    existingVote.CreatedAt = DateTime.UtcNow;
                    _dbContext.CommentVotes.Update(existingVote);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
            }

            var vote = new CommentVote
            {
                UserId = userId,
                CommentId = commentId,
                Value = 1,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.CommentVotes.Add(vote);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DownvoteAsync(Guid userId, Guid articleId, Guid commentId)
        {
            var comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment == null || comment.ArticleId != articleId)
            {
                throw new VoteNotFoundException();
            }

            var existingVote = await _dbContext.CommentVotes.FindAsync(userId, commentId);
            if (existingVote != null)
            {
                if(existingVote.Value == 1)
                {
                    //change the upvote to downvote
                    existingVote.Value = -1;
                    existingVote.CreatedAt = DateTime.UtcNow;
                    _dbContext.CommentVotes.Update(existingVote);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
                else
                {
                    // remove the downvote
                    _dbContext.CommentVotes.Remove(existingVote);
                    await _dbContext.SaveChangesAsync();
                    return;
                }
            }

            var vote = new CommentVote
            {
                UserId = userId,
                CommentId = commentId,
                Value = -1,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.CommentVotes.Add(vote);
            await _dbContext.SaveChangesAsync();
        }
    }
}
