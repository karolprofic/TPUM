namespace ServerPresentation
{
    internal class Presentation
    {
        static async Task Main(string[] args)
        {
            ElectionServer server = new ElectionServer();
            await server.StartAsync();
        }
    }
}
