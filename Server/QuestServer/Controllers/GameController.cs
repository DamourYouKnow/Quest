using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Quest.Utils.Networking;

namespace Quest.Core {
    public class GameController {
        private QuestMessageHandler message_handler;
        private Dictionary<string, WebSocket> user_socket;
        private Dictionary<WebSocket, string> socket_user;

        //Eventhandler
        public event EventHandler<OnPlayerJoinedEventArgs> PlayerJoined;

        public GameController(QuestMessageHandler message_handler){
            this.message_handler = message_handler;
            this.user_socket = new Dictionary<string, WebSocket>();
            this.socket_user = new Dictionary<WebSocket, string>();

            //Register addPlayerSocket function to PlayerJoined EventHandler
            this.PlayerJoined += addPlayerSocket;
        }

        //Handles QuestEvents received from QuestMessageHandler
        public void handle_event(WebSocket socket, QuestEvent qevent){
            switch(qevent.name){
                case "player_join":
                    Console.WriteLine("Event: PlayerJoined");
                    OnPlayerJoined(new OnPlayerJoinedEventArgs(socket, qevent.data));
                    break;
                default:
                    break;
            }
        }

        //Specific event handler that passes arguments to all registered
        //methods.
        protected virtual void OnPlayerJoined(OnPlayerJoinedEventArgs e){
            EventHandler<OnPlayerJoinedEventArgs> handler = PlayerJoined;
            if(handler!=null){
                handler(this, e);
            }
        }

        public void addPlayerSocket(object sender, OnPlayerJoinedEventArgs e){
            if(!this.user_socket.ContainsKey(e.username)){
                Console.WriteLine("Adding player.");
                this.user_socket[e.username] = e.socket;
                this.socket_user[e.socket] = e.username;
            }
        }
    }

    //Event arguments specific to the PlayerJoined event.
    public class OnPlayerJoinedEventArgs : EventArgs{
        public WebSocket socket;
        public string username;

        public OnPlayerJoinedEventArgs(WebSocket socket, string username){
            this.socket = socket;
            this.username = username;
        }
    }
}
