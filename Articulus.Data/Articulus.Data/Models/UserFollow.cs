using System.ComponentModel.DataAnnotations;

namespace Articulus.Data.Models
{
    public class UserFollow
    {
        [Required]
        public Guid FollowerId { get; set; }
        public User Follower { get; set; } = null!;

        [Required]
        public Guid FolloweeId { get; set; }
        public User Followee { get; set; } = null!;

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}
