import asyncio
import websockets
import numpy as np
import cv2
from ultralytics import YOLO

async def handler(websocket):
    print("✅ Client connected")
    frame_count = 0

    while True:
        try:
            # Receive JPEG bytes from Unity
            data = await websocket.recv()
            #print(f"📥 Received {len(data)} bytes")
            # with open(f"frame_{frame_count}.jpg", "wb") as f:
            #     f.write(data)

            # Decode to image
            nparr = np.frombuffer(data, np.uint8)
            frame = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

            # get results
            if frame is None:
                print("❌ Failed to decode image")
                continue
            results = model(frame)
            result_frame = results[0].plot()

            #Encode to JPEG and send back
            success, encoded = cv2.imencode(".jpg", result_frame)
            if not success:
                print("❌ Failed to encode image")
                continue

            await websocket.send(encoded.tobytes())
            #print(f"📤 Sent processed frame #{frame_count}")
            frame_count += 1

        except websockets.exceptions.ConnectionClosed:
            print("🔌 Connection closed by client")
            break

async def main():
    print("🚀 Starting WebSocket server on ws://127.0.0.1:8080")
    async with websockets.serve(handler, "127.0.0.1", 8080):
        await asyncio.Future()

if __name__ == "__main__":
    model = YOLO("yolo11n-seg.pt")  # Load your YOLO model here
    asyncio.run(main())
