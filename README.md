# CPR Training VR

A Unity CPR practice prototype with a guided tutorial, a timed practice test, and
a local progress screen. It supports desktop practice and includes an OpenXR
controller interaction prototype.

> **Educational prototype only:** This project is not a substitute for certified
> CPR training or current local resuscitation guidance. Its score measures only
> the timing between registered compressions; it does not assess compression
> depth, recoil, hand placement, or real-world competence.

## Requirements

- Unity **6000.3.6f1** (see `ProjectSettings/ProjectVersion.txt`)
- Git, to clone the project
- For headset practice: an OpenXR-compatible headset and controllers, with the
  appropriate runtime configured on your system

## Open the project

1. Clone this repository:

   ```bash
   git clone https://github.com/Roopesh6/CPR_VR_Traning.git
   ```

2. In Unity Hub, choose **Add** or **Open**, and select the cloned project
   folder (the folder containing `Assets`, `Packages`, and `ProjectSettings`).
3. Open `Assets/Scenes/MainMenu.unity` and press **Play**.

Unity-generated folders and IDE solution files are excluded from Git. Unity
recreates them when the project is opened.

## Scenes and controls

The four scenes are included in the project's build settings:

- **MainMenu** — Open the tutorial, test, or progress screen.
- **Tutorial** — Step through CPR practice guidance, then start a practice
  session.
- **Testing** — Run a 60-second practice test.
- **Progress** — View the number of completed tests and your best rhythm score.

In desktop mode, click the on-screen controls and press **Space** for each
compression while a session is active. The tutorial practice and test each end
after 60 seconds. Progress is stored locally with Unity `PlayerPrefs`.

When an XR device is active, `CPRXRPlayer` tracks the headset and controllers.
During an active session, squeeze a controller trigger near the demo chest
marker to register a compression. This is a prototype interaction; headset
support also depends on your Unity XR/OpenXR project and runtime configuration.

## Project layout

- `Assets/Scenes/` — Main menu, tutorial, testing, and progress scenes.
- `Assets/Scripts/` — Session tracking, scene UI, and XR controller prototype.
- `Assets/Settings/`, `Assets/XR/`, `Assets/XRI/` — Unity and XR configuration.
- `Packages/manifest.json` — Unity package dependencies.
- `ProjectSettings/` — Editor version and project/build settings.

The starter notes in [`Assets/README.md`](Assets/README.md) also describe the
scene generator and the original desktop prototype workflow.