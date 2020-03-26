#if UNITY_EDITOR

using Extensions;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MapPreviewGenerator : IPreprocessBuildWithReport
{
    private const int PREVIEW_WIDTH = 800;
    private const int PREVIEW_HEIGHT = 450;

    public static readonly string MAP_PREFABS_PATH = "Assets/Decontainment/Maps/Resources/MapPrefabs/";
    public static readonly string MAP_PREVIEWS_PATH = "Assets/Decontainment/Maps/Resources/MapPreviews/";

    private static readonly string ARENA_SCENE_PATH = "Assets/Decontainment/Scenes/Arena.unity";

    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        GenerateMapPreviews();
    }

    [MenuItem("Generators/GenerateMapPreview")]
    private static void GenerateMapPreviews()
    {
        Debug.Log("Generating map previews");

        string oldScenePath = EditorSceneManager.GetActiveScene().path;
        GameObject[] mapPrefabs = AssetDatabaseExtensions.LoadAllAssetsInDir<GameObject>(MAP_PREFABS_PATH);

        foreach (GameObject mapPrefab in mapPrefabs) {
            EditorSceneManager.OpenScene(ARENA_SCENE_PATH, OpenSceneMode.Single);

            // Setup the scene for the screenshot
            GameObject map = GameObject.Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
            map.GetComponent<Map>().SpawnPreviewLabels();

            // Disable canvas
            GameObject.FindGameObjectWithTag("MainCanvas").SetActive(false);

            // Grab the preview shot
            RenderTexture oldTargetTex = Camera.main.targetTexture;
            RenderTexture oldActiveTex = RenderTexture.active;

            Camera.main.targetTexture = new RenderTexture(PREVIEW_WIDTH, PREVIEW_HEIGHT, 24, RenderTextureFormat.ARGB32);
            RenderTexture.active = Camera.main.targetTexture;
            Camera.main.Render();
            Texture2D tex = new Texture2D(PREVIEW_WIDTH, PREVIEW_HEIGHT, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, PREVIEW_WIDTH, PREVIEW_HEIGHT), 0, 0);
            tex.Apply();

            Camera.main.targetTexture = oldTargetTex;
            RenderTexture.active = oldActiveTex;

            // Save it
            byte[] bytes = tex.EncodeToPNG();
            string previewPath = MAP_PREVIEWS_PATH + mapPrefab.name + ".png";
            File.WriteAllBytes(previewPath, bytes);
            AssetDatabase.ImportAsset(previewPath);

            Debug.Log("Successfully generated " + previewPath);
        }

        EditorSceneManager.OpenScene(oldScenePath);
    }
}

#endif