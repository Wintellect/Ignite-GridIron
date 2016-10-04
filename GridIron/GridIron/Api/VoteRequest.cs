namespace GridIron.Api
{
    public class VoteRequest
    {
        public PassCode PassCode { get; private set; }
        public string UserId { get; private set; }
        public string ArtistId { get; private set; }
        public static VoteRequest From (string code, string userId, string artistId)
        {
            return new VoteRequest {
                UserId = userId,
                ArtistId = artistId,
                PassCode = new PassCode {
                    Code = code
                }
            };
        }
    }
}
