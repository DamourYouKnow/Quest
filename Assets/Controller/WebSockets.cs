using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class WebSockets : MonoBehaviour {
    private UnityWebSocket socket;
    private Dictionary<string, Action<JToken>> eventHandlers;

    private void Start() {
        this.socket = new UnityWebSocket("ws://localhost:3004/quest");
        this.socket.OnClose += this.OnClose;
        this.socket.OnOpen += this.OnOpen;
        this.socket.OnMessage += this.OnMessage;
        this.socket.OnError += this.OnError;

        this.eventHandlers = new Dictionary<string, Action<JToken>>();
        this.InitEventHandlers();
    }

    private void InitEventHandlers() {
        this.On("test", (data) => {
            Debug.Log(data.ToString());
        });
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

        JObject eventObj = JObject.Parse(message);
        string eventName = (string)eventObj["event"];
        if (this.eventHandlers.ContainsKey(eventName)) {
            this.eventHandlers[eventName](eventObj["data"]);
        }
    }

    public void On(string eventName, Action<JToken> handler) {
        this.eventHandlers.Add(eventName, handler);
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

