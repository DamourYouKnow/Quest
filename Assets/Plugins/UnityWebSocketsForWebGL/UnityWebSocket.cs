using System;
using System.Collections.Generic;
using AOT;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE
using WebSocketSharp;
#endif

public class UnityWebSocket {

	private static Dictionary<int, UnityWebSocket> webSocketInstances =
		new Dictionary<int, UnityWebSocket>();

#if UNITY_EDITOR || UNITY_STANDALONE
	private WebSocket WebSocket;
#elif UNITY_WEBGL
	private NativeWebSocket NativeWebSocket;
#endif

	public UnityWebSocket(string url) {

		Debug.Log("[UnityWebSocket] Instantiating new websocket: " + url);

#if UNITY_EDITOR || UNITY_STANDALONE
		WebSocket = new WebSocket(url);
		WebSocket.ConnectAsync();
		WebSocket.AcceptAsync();
		WebSocket.OnOpen += WebSocket_OnOpen;
		WebSocket.OnMessage += WebSocket_OnMessage;
		WebSocket.OnError += WebSocket_OnError;
		WebSocket.OnClose += WebSocket_OnClose;
#elif UNITY_WEBGL
		NativeWebSocket = new NativeWebSocket(url);
		NativeWebSocket.SetOnOpen(NativeSocket_OnOpen);
		NativeWebSocket.SetOnMessage(NativeSocket_OnMessage);
		NativeWebSocket.SetOnError(NativeSocket_OnError);
		NativeWebSocket.SetOnClose(NativeSocket_OnClose);
		webSocketInstances.Add(NativeWebSocket.Id, this);
#endif
	}

	public void Close() {

		Debug.Log("[UnityWebSocket] Closing web socket connection.");

#if UNITY_EDITOR || UNITY_STANDALONE
		if (WebSocket == null)
			return;
		WebSocket.CloseAsync();
		WebSocket.OnOpen -= WebSocket_OnOpen;
		WebSocket.OnMessage -= WebSocket_OnMessage;
		WebSocket.OnError -= WebSocket_OnError;
		WebSocket.OnClose -= WebSocket_OnClose;
		WebSocket = null;
#elif UNITY_WEBGL
		if (NativeWebSocket == null)
			return;
		NativeWebSocket.CloseAsync();
		NativeWebSocket.SetOnOpen(null);
		NativeWebSocket.SetOnMessage(null);
		NativeWebSocket.SetOnError(null);
		NativeWebSocket.SetOnClose(null);
		NativeWebSocket = null;
#endif
	}

#if UNITY_EDITOR || UNITY_STANDALONE
	private void WebSocket_OnOpen(object sender, EventArgs e) {

		Debug.Log("[UnityWebSocket] Web socket on open.");

		if (OnOpen != null)
			OnOpen(this);
	}

	private void WebSocket_OnMessage(object sender, MessageEventArgs e) {

		Debug.Log("[UnityWebSocket] Web socket on message.");

		if (OnMessage != null)
			OnMessage(this, e.RawData);
	}

	private void WebSocket_OnError(object sender, ErrorEventArgs e) {

		Debug.Log("[UnityWebSocket] Web socket on error.");

		if (OnError != null)
			OnError(this, e.Message);
	}

	private void WebSocket_OnClose(object sender, CloseEventArgs e) {

		Debug.Log("[UnityWebSocket] Web socket on close.");

		if (OnClose != null)
			OnClose(this, e.Code, e.Reason);
	}
#elif UNITY_WEBGL
	[MonoPInvokeCallback(typeof(Action<int>))]
	public static void NativeSocket_OnOpen(int id) {
	
		Debug.Log("[UnityWebSocket] Web socket on open.");

		if (webSocketInstances[id].OnOpen != null)
			webSocketInstances[id].OnOpen(webSocketInstances[id]);
	}

	[MonoPInvokeCallback(typeof(Action<int>))]
	public static void NativeSocket_OnMessage(int id) {

		Debug.Log("[UnityWebSocket] Web socket on message.");

		byte[] data = webSocketInstances[id].NativeWebSocket.Receive();
		if (webSocketInstances[id].OnMessage != null)
			webSocketInstances[id].OnMessage(webSocketInstances[id], data);
	}

	[MonoPInvokeCallback(typeof(Action<int>))]
	public static void NativeSocket_OnError(int id) {
		
		Debug.Log("[UnityWebSocket] Web socket on error.");

		if (webSocketInstances[id].OnError != null)
			webSocketInstances[id].OnError(webSocketInstances[id], webSocketInstances[id].NativeWebSocket.Error);
	}

	[MonoPInvokeCallback(typeof(Action<int, int>))]
	public static void NativeSocket_OnClose(int code, int id) {

		Debug.Log("[UnityWebSocket] Web socket on close.");

		CloseError errorInfo = CloseError.Get(code);
		if (webSocketInstances[id].OnClose != null)
			webSocketInstances[id].OnClose(webSocketInstances[id], errorInfo.Code, errorInfo.Message);
	}
#endif

	public void SendAsync(byte[] packet) {

		Debug.Log("[UnityWebSocket] Sending message.");

#if UNITY_EDITOR || UNITY_STANDALONE
		WebSocket.SendAsync(packet, null);
#elif UNITY_WEBGL
		NativeWebSocket.SendAsync(packet);
#endif
	}

	public delegate void OnOpenHandler(UnityWebSocket accepted);
	public delegate void OnMessageHandler(UnityWebSocket sender, byte[] data);
	public delegate void OnErrorHandler(UnityWebSocket sender, string message);
	public delegate void OnCloseHandler(UnityWebSocket sender, int code, string reason);

	public event OnOpenHandler OnOpen;
	public event OnMessageHandler OnMessage;
	public event OnErrorHandler OnError;
	public event OnCloseHandler OnClose;
}
