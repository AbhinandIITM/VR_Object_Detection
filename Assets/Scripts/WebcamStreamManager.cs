using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using System;

public class WebcamStreamManager : MonoBehaviour
{
    [Header("Quads")]
    public GameObject liveQuad;
    public GameObject resultQuad;

    private WebCamTexture webcamTexture;
    private Texture2D resultTexture;
    private WebSocket websocket;
    private bool isRunning = false;

    async void Start()
    {
        // Start webcam
        webcamTexture = new WebCamTexture(320, 240);
        webcamTexture.Play();

        if (liveQuad)
            liveQuad.GetComponent<Renderer>().material.mainTexture = webcamTexture;

        resultTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);
        if (resultQuad)
            resultQuad.GetComponent<Renderer>().material.mainTexture = resultTexture;

        // Setup WebSocket
        websocket = new WebSocket("ws://127.0.0.1:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("✅ WebSocket connection opened");
            isRunning = true;
            _ = SendFramesLoop(); // Fire and forget
        };

        websocket.OnMessage += (bytes) =>
        {
            //Debug.Log($"📥 Received {bytes.Length} bytes from Python");

            if (resultTexture.LoadImage(bytes))
            {
                // Resize texture if necessary
                resultTexture.Apply();
                if (resultQuad)
                    resultQuad.GetComponent<Renderer>().material.mainTexture = resultTexture;
                //Debug.Log("🖼️ Updated result quad with received texture");
            }
            else
            {
                Debug.LogWarning("⚠️ Failed to load image into texture");
            }
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"❌ WebSocket error: {e}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.LogWarning($"🔌 WebSocket closed with code {e}");
            isRunning = false;
        };

        //Debug.Log("⚙️ WebSocket event handlers set up");

        try
        {
            await websocket.Connect();
            Debug.Log("🔗 WebSocket connected");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to connect to WebSocket: {ex.Message}");
        }
    }

    private async Task SendFramesLoop()
    {
        while (isRunning)
        {
            try
            {
                if (websocket != null && websocket.State == WebSocketState.Open)
                {
                    Texture2D frame = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
                    frame.SetPixels32(webcamTexture.GetPixels32());
                    frame.Apply();

                    byte[] jpeg = frame.EncodeToJPG();
                    Destroy(frame);

                    //Debug.Log($"📤 Sending frame: {jpeg.Length} bytes");
                    await websocket.Send(jpeg);
                }

                await Task.Delay(50); // ~20 FPS
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error in SendFramesLoop: {ex.Message}");
                break;
            }
        }
    }

    private async void OnApplicationQuit()
    {
        isRunning = false;

        if (webcamTexture != null && webcamTexture.isPlaying)
            webcamTexture.Stop();

        if (websocket != null)
        {
            try
            {
                await websocket.Close();
                Debug.Log("🔌 WebSocket closed cleanly");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Error closing WebSocket: {ex.Message}");
            }
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }
}
