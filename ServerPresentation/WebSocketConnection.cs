﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
                    Console.WriteLine("[INFO] Client closed the connection.");
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"[DEBUG] Received message: {json}");

                if (OnMessageReceived != null)
                {
                    await OnMessageReceived.Invoke(json);
                }
            }
        }

        public async Task SendMessageAsync(object messageObj)
        {
            try
            {
                string json = JsonSerializer.Serialize(messageObj);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"[DEBUG] Sent message: {json}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Failed to send message: " + ex.Message);
            }
        }
    }
}