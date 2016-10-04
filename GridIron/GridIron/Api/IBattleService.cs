using System.Threading.Tasks;
using Refit;

namespace GridIron.Api
{
    public interface IBattleService
    {
        [Post ("/battle/getstatus")]
        Task<Battle> GetStatus ([Body] StatusRequest request);

        [Post ("/battle/vote")]
        Task Vote ([Body] VoteRequest request);
    }
}
