namespace GridIron.Api
{
    public class StatusRequest : PassCode
    {
        public static StatusRequest From (string code)
        {
            return new StatusRequest { Code = code };
        }
    }
}
