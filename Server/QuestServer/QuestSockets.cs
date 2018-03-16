﻿using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Utils.Networking {
    public class QuestMessageHandler : WebSocketHandler {
        public QuestMessageHandler(WebSocketConnectionManager connectionManager) : base(connectionManager) {

        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer) {
            // FIXME: We still are not reaching here!
            string message = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("Message received: " + message);
        }

        // TODO: Send handler.
    }
}
