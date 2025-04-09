using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientData
{
    internal class ElectionClientConnection
    {
        private readonly string url;
        private WebSocketConnection? wsConnection;
        public event Func<string, Task>? OnMessageReceived;

        public ElectionClientConnection(string url)
        {
            this.url = url;
        }

        public async Task ConnectAsync()
        {
            try
            {
                var clientSocket = new ClientWebSocket();
                await clientSocket.ConnectAsync(new Uri(url), CancellationToken.None);
                wsConnection = new WebSocketConnection(clientSocket);

                wsConnection.OnMessageReceived += async (message) =>
                {
                    if (OnMessageReceived != null)
                        await OnMessageReceived.Invoke(message);
                };

                _ = wsConnection.ProcessMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Error during connection: {ex.Message}");
            }
        }

        public async Task SendVoteAsync(Guid candidateId, string code)
        {
            if (wsConnection == null)
            {
                return;
            }

            var messageObj = new
            {
                Action = "CastVote",
                CandidateId = candidateId.ToString(),
                AuthCode = code
            };

            await wsConnection.SendMessageAsync(messageObj);
        }
    }

}
