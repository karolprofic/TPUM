namespace ServerPresentation
{
    internal class Presentation
    {
        static async Task Main(string[] args)
        {
            ElectionServer server = new ElectionServer("http://localhost:5000/");
            await server.StartAsync();
        }
    }
}
