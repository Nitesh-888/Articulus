using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Articulus.BLL.Exceptions
{
    public class VoteNotFoundException : Exception
    {
        public VoteNotFoundException() : base("The specified vote was not found.")
        {
        }
    }
}
