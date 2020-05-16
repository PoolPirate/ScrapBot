namespace ScrapTDWrapper.Entities
{
    public class SocketEntity
    {
        protected ScrapClient Client { get; }

        protected SocketEntity(ScrapClient client)
        {
            Client = client;
        }
    }
}
