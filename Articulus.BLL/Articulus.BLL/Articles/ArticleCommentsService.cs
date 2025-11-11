using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Articulus.BLL.Articles.Interfaces;
using Articulus.Data;
using Articulus.Data.Models;
using Microsoft.EntityFrameworkCore;
using Articulus.DTOs.Comments;
using Articulus.BLL.Exceptions;

namespace Articulus.BLL.Articles
{
    public class ArticleCommentsService : IArticleCommentsService
    {
        private readonly AppDbContext _dbContext;
        public ArticleCommentsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<GetAllCommentsDTO>> GetAllCommentsAsync(Guid articleId)
        {
            var article = await _dbContext.Articles
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(a => a.ArticleId == articleId) ?? throw new ArticleNotFoundException(articleId);
            
            var comments = article.Comments.Select(c => new GetAllCommentsDTO
            {
                CommentId = c.CommentId,
                Text = c.Text,
                AuthourId = c.UserId.ToString(),
                AuthorName = c.User.FirstName + " " + c.User.LastName,
                CreatedAt = c.CreatedAt,
            });

            return comments;
        }
        public async Task<GetCommentByIDResponseDTO> GetCommentByIdAsync(Guid userId, Guid articleId, Guid commentId)
        {
            var user = await _dbContext.Users.FindAsync(userId) ?? throw new UserNotFoundException(userId);

            var article = await _dbContext.Articles
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                    .ThenInclude(c => c.Votes)
                .SingleOrDefaultAsync(a => a.ArticleId == articleId) ?? throw new ArticleNotFoundException(articleId);


            var comment = article.Comments.SingleOrDefault(c => c.CommentId == commentId) ?? throw new CommentNotFoundException(commentId);
            var response = new GetCommentByIDResponseDTO
            {
                CommentId = comment.CommentId,
                Text = comment.Text,
                AuthourId = comment.UserId.ToString(),
                AuthorName = comment.User.FirstName + " " + comment.User.LastName,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                VoteCount = comment.Votes.Count,
                UpvoteCount = comment.Votes.Count(v => v.Value == 1),
                DownvoteCount = comment.Votes.Count(v => v.Value == -1),
                IsVotedByCurrentUser = comment.Votes.Any(v => v.UserId == userId),
                IsUpvotedByCurrentUser = comment.Votes.Any(v => v.UserId == userId && v.Value == 1),
            };

            return response;
        }
        public async Task AddCommentAsync(Guid userId, Guid articleId, string content)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            // extract article from db
            var article = await _dbContext.Articles
                .SingleOrDefaultAsync(a => a.ArticleId == articleId) ?? throw new ArticleNotFoundException(articleId);
            var comment = new Comment
            {
                Text = content,
                ArticleId = articleId,
                UserId = user.UserId,
                User = user,
                Article = article
            };

            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditCommentAsync(Guid userId, Guid articleId, Guid commentId, string content)
        {
            var user = await _dbContext.Users
                .Include(u => u.Comments)
                .SingleOrDefaultAsync(u => u.UserId == userId) ?? throw new UserNotFoundException(userId);

            // Check if the comment belongs to the user
            if (!user.Comments.Any(c => c.CommentId == commentId && c.ArticleId == articleId))
            {
                throw new ForbiddenException();
            }

            var comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment == null || comment.ArticleId != articleId)
            {
                throw new CommentNotFoundException(commentId);
            }

            comment.Text = content;
            comment.UpdatedAt = DateTime.UtcNow;

            _dbContext.Comments.Update(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCommentAsync(Guid userId, Guid articleId, Guid commentId)
        {
            var comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment == null || comment.ArticleId != articleId)
            {
                throw new CommentNotFoundException(commentId);
            }

            // Check if the comment belongs to the user
            if (comment.UserId != userId)
            {
                throw new ForbiddenException();
            }

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
