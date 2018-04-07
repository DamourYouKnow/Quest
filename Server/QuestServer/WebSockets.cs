using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

// Code taken and modified from the following source:
// https://radu-matei.com/blog/aspnet-core-websockets-middleware/
namespace Utils.Networking {
    public class WebSocketConnectionManager {
        private static ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocket(string id) {
            return sockets[id];
        }

        public ConcurrentDictionary<string, WebSocket> GetAllSockets() {
            return sockets;
        }

        public string GetSocketId(WebSocket socket) {
            return sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AddSocket(WebSocket socket) {
            sockets.TryAdd(Guid.NewGuid().ToString(), socket);
        }

        public async Task RemoveSocket(string id) {
            WebSocket removedSocket;
            sockets.TryRemove(id, out removedSocket);
            await removedSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                           "WebSocket closed",
                                           CancellationToken.None);
        }
    }


    public abstract class WebSocketHandler {
        protected WebSocketConnectionManager connectionManager;

        public WebSocketHandler(WebSocketConnectionManager connectionManager) {
            this.connectionManager = connectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket) {
            connectionManager.AddSocket(socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket) {
            await connectionManager.RemoveSocket(connectionManager.GetSocketId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message) {
            if (socket.State != WebSocketState.Open) return;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                                   WebSocketMessageType.Text,
                                   true,
                                   CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message) {
            await this.SendMessageAsync(connectionManager.GetSocket(socketId), message);
        }

        public async Task SendAllAsync(string message) {
            foreach (WebSocket socket in connectionManager.GetAllSockets().Values) {
                await this.SendMessageAsync(socket, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }

    public class WebSocketMiddleware {
        private readonly RequestDelegate next;
        private WebSocketHandler socketHandler;

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler socketHandler) {
            this.next = next;
            this.socketHandler = socketHandler;
        }

        public async Task Invoke(HttpContext context) {
            if (!context.WebSockets.IsWebSocketRequest) return;

            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            await socketHandler.OnConnected(socket);

            await Receive(socket, async(result, buffer) => {
                if (result.MessageType == WebSocketMessageType.Binary) {
                    await socketHandler.ReceiveAsync(socket, result, buffer);
                    Array.Clear(buffer, 0, buffer.Length);
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close) {
                    await socketHandler.OnDisconnected(socket);
                    Array.Clear(buffer, 0, buffer.Length);
                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage) {
            byte[] buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open) {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }
    }
}
