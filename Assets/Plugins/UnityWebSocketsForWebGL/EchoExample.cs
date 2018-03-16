using System;
using System.Text;
using UnityEngine;

public class EchoExample : MonoBehaviour {

	UnityWebSocket WebSocket;

	// Use this for initialization
	void Start () {

		// This example service echoes back any messages sent to it
		WebSocket = new UnityWebSocket("ws://echo.websocket.org");
		WebSocket.OnClose += WebSocket_OnClose;
		WebSocket.OnOpen += Websocket_OnOpen;
		WebSocket.OnMessage += WebSocket_OnMessage;
		WebSocket.OnError += WebSocket_OnError;
	}

	private void WebSocket_OnClose(UnityWebSocket sender, int code, string reason) {

		Debug.Log("Connection closed: " + reason);
	}

	private void Websocket_OnOpen(UnityWebSocket accepted) {

		Debug.Log("Connection established.");

		string message = "Hello, friend!";

		Debug.Log("Sending message: " + message);

		byte[] data = Encoding.UTF8.GetBytes(message);

		WebSocket.SendAsync(data);
	}

	private void WebSocket_OnMessage(UnityWebSocket sender, byte[] data) {

		string message = Encoding.UTF8.GetString(data);

		Debug.Log("Message received: " + message);
	}

	private void WebSocket_OnError(UnityWebSocket sender, string message) {
		Debug.Log("Error: " + message);
	}
}
