using System.ComponentModel.DataAnnotations;
using Articulus.Data.Models;

namespace Articulus.DTOs.Reactions
{
    public class CreateReactionRequestDTO
    {
        [EnumDataType(typeof(ReactionType))]
        public ReactionType Type { get; set; }
    }
}
