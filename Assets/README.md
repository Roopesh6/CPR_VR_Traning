# CPR Training VR Starter (desktop first)

This starter builds a four-scene Unity prototype that can be explored without a VR headset. It uses a regular camera, mouse, and keyboard now, so the same learning flow can be validated before adding XR hardware.

## Create the scenes

1. Create a Unity 3D project (Unity 2022.3 LTS or newer).
2. Copy the `Assets` folder from this starter into the project root, merging if asked.
3. In Unity, choose **Tools > CPR Training > Generate Four Scenes**.
4. The generator creates `Assets/Scenes/MainMenu.unity`, `Tutorial.unity`, `Testing.unity`, and `Progress.unity`, and adds them to Build Settings.
5. Press Play from the MainMenu scene. Use the on-screen buttons. In Tutorial/Testing, click **Start compressions**, then press **Space** at a steady rate. Press Escape to release mouse capture if enabled by your Unity version.

## Add your patient model

Copy the model file (FBX/OBJ/GLB, as supported by your Unity import setup) into `Assets/Models`. Drag the imported prefab into Tutorial and Testing scenes, position it supine, and make sure its chest is visible from the camera. The starter has a simple placeholder patient until then. Keep the patient non-graphic and use a clearly marked chest target.

## Scenes and controls

- **MainMenu**: opens Tutorial, Testing, or Progress.
- **Tutorial**: guided steps with a floating dashboard, prompts, and desktop compression input.
- **Testing**: timed practice and a simple compression-rate/consistency score.
- **Progress**: locally saved best score and completed practice count.
- **Desktop input**: click the start button, then press Space to register compressions. Optional mouse wheel can reposition the chest target only if you wire it into your own interaction setup.

## Notes

This is an educational prototype, not a substitute for certified CPR instruction. The scoring is intentionally a basic interaction demo, not a clinical assessment of depth, recoil, hand placement, or real-world competence. Follow current local resuscitation guidance when authoring instructional content. The project is structured so XR Interaction Toolkit input can replace keyboard input later.
