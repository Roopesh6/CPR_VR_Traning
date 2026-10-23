using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Lightweight OpenXR player for the prototype. Tracks the HMD and both controllers
/// with Unity XR device poses and registers a compression when a controller trigger
/// is pressed near the chest marker. Does not require XR Interaction Toolkit.
/// </summary>
public class CPRXRPlayer : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 0.22f;
    [SerializeField] private float controllerScale = 0.09f;

    private Camera xrCamera;
    private Transform leftVisual;
    private Transform rightVisual;
    private InputDevice headDevice;
    private InputDevice leftDevice;
    private InputDevice rightDevice;
    private bool leftWasPressed;
    private bool rightWasPressed;
    private bool xrActive;

    private void Start()
    {
        xrActive = XRSettings.isDeviceActive;
        if (!xrActive)
        {
            // Preserve the existing monitor/desktop camera when no headset is connected.
            enabled = false;
            return;
        }

        Camera desktopCamera = Camera.main;
        if (desktopCamera != null) desktopCamera.enabled = false;

        var cameraObject = new GameObject("XR Head Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.transform.SetParent(transform, false);
        xrCamera = cameraObject.GetComponent<Camera>();
        xrCamera.tag = "MainCamera";
        xrCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        xrCamera.nearClipPlane = 0.03f;
        xrCamera.farClipPlane = 100f;

        leftVisual = CreateControllerVisual("Left XR Controller", new Color(0.15f, 0.55f, 1f));
        rightVisual = CreateControllerVisual("Right XR Controller", new Color(1f, 0.45f, 0.15f));
        RefreshDevices();
    }

    private void Update()
    {
        if (!xrActive) return;
        if (!headDevice.isValid || !leftDevice.isValid || !rightDevice.isValid) RefreshDevices();

        ApplyPose(headDevice, xrCamera != null ? xrCamera.transform : null);
        UpdateController(leftDevice, leftVisual, ref leftWasPressed);
        UpdateController(rightDevice, rightVisual, ref rightWasPressed);
    }

    private void RefreshDevices()
    {
        headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void UpdateController(InputDevice device, Transform visual, ref bool wasPressed)
    {
        bool tracked = ApplyPose(device, visual);
        if (visual != null) visual.gameObject.SetActive(tracked);

        bool pressed = device.isValid && device.TryGetFeatureValue(CommonUsages.trigger, out float trigger) && trigger > 0.75f;
        if (pressed && !wasPressed && tracked) TryCompress(visual.position);
        wasPressed = pressed;
    }

    private bool ApplyPose(InputDevice device, Transform target)
    {
        if (target == null || !device.isValid) return false;
        bool hasPosition = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        bool hasRotation = device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        if (!hasPosition) return false;
        target.localPosition = position;
        if (hasRotation) target.localRotation = rotation;
        return true;
    }

    private void TryCompress(Vector3 controllerPosition)
    {
        GameObject targetObject = GameObject.Find("Chest Target - demo marker");
        CPRSession session = CPRSession.Instance;
        if (targetObject == null || session == null || !session.IsActive) return;
        if (Vector3.Distance(controllerPosition, targetObject.transform.position) <= interactionRadius)
            session.RegisterCompression();
    }

    private Transform CreateControllerVisual(string objectName, Color color)
    {
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = objectName;
        visual.transform.SetParent(transform, false);
        visual.transform.localScale = new Vector3(controllerScale, controllerScale * 1.8f, controllerScale);
        var renderer = visual.GetComponent<Renderer>();
        renderer.material.color = color;
        Collider collider = visual.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        return visual.transform;
    }
}
