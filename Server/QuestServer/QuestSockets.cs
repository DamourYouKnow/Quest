using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Utils.Networking {
    public class QuestMessageHandler : WebSocketHandler {
        public QuestMessageHandler(WebSocketConnectionManager connectionManager) : base(connectionManager) {

        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer) {
            throw new NotImplementedException();
        }
    }
}
