# VR Object Detection

A Unity-based VR project for real-time object detection using a YOLO model. The Unity client streams webcam frames to a Python backend via WebSocket, receives processed frames, and displays the results in VR.

## Features

- Streams webcam frames from Unity to a Python server.
- Python backend runs YOLO object detection and returns annotated frames.
- Real-time display of both live and processed frames in Unity.
- Supports remote access via ngrok tunneling.

## Setup Instructions

### 1. Clone the Repository

Clone or download this repository to your local machine.

### 2. Requirements

- [Unity](https://unity.com/) (recommended version: 2020.3 or later)
- Python 3.8+
- Python dependencies: `ultralytics`, `opencv-python`, `websockets`, `numpy`
- [ngrok](https://ngrok.com/) (for remote WebSocket tunneling)

### 3. Python Backend Setup

Install Python dependencies:

```sh
pip install ultralytics opencv-python websockets numpy
```

Run the Python WebSocket server:

```sh
python Assets/Python/process_frame.py
```

This will start the server at `ws://127.0.0.1:8080`.

### 4. Unity Setup

1. Open the project folder in Unity.
2. Open the main scene.
3. Press Play to start streaming webcam frames.

### 5. Ngrok (Optional: For Remote Access)

To expose your local WebSocket server to the internet, run:

```sh
ngrok http 8080
```

Update the WebSocket URL in [`WebcamStreamManager.cs`](Assets/Scripts/WebcamStreamManager.cs) if you want Unity to connect to the ngrok public URL.

## File Structure

- `Assets/Scripts/WebcamStreamManager.cs` — Unity script for webcam streaming and WebSocket communication.
- `Assets/Python/process_frame.py` — Python backend for YOLO inference and frame processing.
- `yolo11n-seg.pt` — YOLO model weights.

## Usage

- Start the Python server.
- Start Unity Play mode.
- (Optional) Start ngrok for remote access.
- The Unity client will send webcam frames to the Python backend, which processes and returns annotated frames for display.

## License

See [LICENSE](LICENSE) for details.

---

**Note:** For best results, ensure your webcam is connected and accessible by Unity. The Python server must be running before starting the Unity scene.
