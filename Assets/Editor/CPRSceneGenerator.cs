using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class CPRSceneGenerator
{
    private static readonly string[] Names = { "MainMenu", "Tutorial", "Testing", "Progress" };

    [MenuItem("Tools/CPR Training/Generate Four Scenes")]
    public static void Generate()
    {
        EnsureFolder("Assets/Scenes");
        var settings = new List<EditorBuildSettingsScene>();
        foreach (var name in Names)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = name;
            var camera = new GameObject("Desktop Camera", typeof(Camera), typeof(AudioListener));
            camera.tag = "MainCamera"; camera.transform.position = new Vector3(0, 1.65f, -3.5f); camera.transform.rotation = Quaternion.Euler(8, 0, 0);
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = new Color(0.16f, 0.22f, 0.27f);
            if (name == "Tutorial" || name == "Testing") CreatePatientPlaceholder();
            var manager = new GameObject("CPR Session"); manager.AddComponent<CPRSession>();
            var ui = new GameObject("Scene UI"); ui.AddComponent<CPRSceneUI>();
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            string path = $"Assets/Scenes/{name}.unity";
            EditorSceneManager.SaveScene(scene, path);
            settings.Add(new EditorBuildSettingsScene(path, true));
        }
        EditorBuildSettings.scenes = settings.ToArray();
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        AssetDatabase.Refresh();
        Debug.Log("Generated CPR Training scenes under Assets/Scenes.");
    }

    [MenuItem("Tools/CPR Training/Add XR Player to Tutorial and Testing")]
    public static void AddXRPlayerToPracticeScenes()
    {
        foreach (var name in new[] { "Tutorial", "Testing" })
        {
            string path = $"Assets/Scenes/{name}.unity";
            if (!System.IO.File.Exists(path))
            {
                Debug.LogError($"Could not find {path}. Generate the four scenes first.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            if (GameObject.Find("CPR XR Player") == null)
            {
                var player = new GameObject("CPR XR Player");
                player.AddComponent<CPRXRPlayer>();
            }
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);
        Debug.Log("Added CPR XR Player to Tutorial and Testing. Configure OpenXR before using a headset.");
    }

    private static void CreatePatientPlaceholder()
    {
        var patient = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        patient.name = "Patient Placeholder - replace with your human model";
        patient.transform.position = new Vector3(0, 0.38f, 0.4f);
        patient.transform.rotation = Quaternion.Euler(0, 0, 90);
        patient.transform.localScale = new Vector3(0.5f, 1.1f, 0.5f);
        var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        target.name = "Chest Target - demo marker";
        target.transform.position = new Vector3(0, 0.58f, 0.55f);
        target.transform.localScale = Vector3.one * 0.18f;
        target.GetComponent<Renderer>().sharedMaterial.color = new Color(0.85f, 0.2f, 0.14f);
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor"; floor.transform.position = new Vector3(0, -0.2f, 0.4f); floor.transform.localScale = new Vector3(4, 0.2f, 4);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        var parent = System.IO.Path.GetDirectoryName(path).Replace("\\", "/");
        var leaf = System.IO.Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, leaf);
    }
}
