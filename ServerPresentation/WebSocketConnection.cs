using Commons;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace ServerPresentation
{
    public class WebSocketConnection
    {
        private readonly WebSocket _socket;
        public event Func<string, Task>? OnMessageReceived;
        
        public WebSocketConnection(WebSocket socket)
        {
            _socket = socket;
        }

        public async Task ProcessMessagesAsync()
        {
            var buffer = new byte[4096];

            while (_socket.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Logger.Debug($"Received message: {json}");

                if (OnMessageReceived != null)
                {
                    await OnMessageReceived.Invoke(json);
                }
            }
        }

        public async Task SendMessageAsync(string json)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await _socket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
                Logger.Debug($"Sent message: {json}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to send message: {ex.Message}");
            }
        }

        public Task SendMessageAsync(object messageObj)
            => SendMessageAsync(JsonConvert.SerializeObject(messageObj));
    }
}