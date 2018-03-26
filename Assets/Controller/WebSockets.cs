using System.Text;
using UnityEngine;

public class WebSockets : MonoBehaviour {
    private UnityWebSocket socket;

    private void Start() {
        this.socket = new UnityWebSocket("ws://localhost:3004/quest");
        this.socket.OnClose += this.OnClose;
        this.socket.OnOpen += this.OnOpen;
        this.socket.OnMessage += this.OnMessage;
        this.socket.OnError += this.OnError;
    }

    public void OnClose(UnityWebSocket sender, int code, string reason) {
        Debug.Log("Connection closed: " + reason);
    }

    public void OnOpen(UnityWebSocket accepted) {
        Debug.Log("Connetion established");
        this.SendMessage(accepted, "{\"event\":\"test\",\"data\":{}}");
    }

    public void OnMessage(UnityWebSocket sender, byte[] data) {
        string message = Encoding.UTF8.GetString(data);
        Debug.Log("Message received: " + message);
    }

    public void SendMessage(UnityWebSocket socket, string message) {
        Debug.Log("Sending message: " + message);
        byte[] data = Encoding.UTF8.GetBytes(message);
        this.socket.SendAsync(data);
    }

    private void OnError(UnityWebSocket sender, string message) {
        Debug.Log("Error: " + message);
    }
}

