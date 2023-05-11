using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class AutoSaveScene
{
    private static bool autoSaveEnabled;
    private static bool remindersEnabled;
    private static float saveFrequency;
    private static float remindFrequency;
    private static string folderName = "BackupScenes";
    private static int backupCycleCount;
    private static int currentBackupIndex = 1;
    private static float nextSaveTime;
    private static float nextRemindTime;

    static AutoSaveScene()
    {
        EditorApplication.update += Update;
    }

    [MenuItem("Tools/Auto Save Scene")]
    public static void ShowWindow()
    {
        AutoSaveSceneSettings window = EditorWindow.GetWindow<AutoSaveSceneSettings>("Auto Save Scene");
        window.Show();
    }

    private static void Update()
    {
        if (autoSaveEnabled && !EditorApplication.isPlaying && saveFrequency > 0)
        {
            if (Time.realtimeSinceStartup >= nextSaveTime)
            {
                SaveSceneBackup();
                nextSaveTime = Time.realtimeSinceStartup + saveFrequency * 60;
            }
        }

        if (remindersEnabled && !EditorApplication.isPlaying && remindFrequency > 0)
        {
            if (Time.realtimeSinceStartup >= nextRemindTime)
            {
                EditorUtility.DisplayDialog("Reminder", "Don't forget to save your scene!", "OK");
                nextRemindTime = Time.realtimeSinceStartup + remindFrequency * 60;
            }
        }
    }

    private static void SaveSceneBackup()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        string backupFolderPath = folderName;
        string sceneName = Path.GetFileNameWithoutExtension(currentScenePath);
        string backupScenePath = Path.Combine(backupFolderPath, sceneName + $"_backup{currentBackupIndex}.unity");

        if (!Directory.Exists(backupFolderPath))
        {
            Directory.CreateDirectory(backupFolderPath);
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), backupScenePath, true);
        Debug.Log($"Scene backup saved to: {backupScenePath}");

        currentBackupIndex++;
        if (currentBackupIndex > backupCycleCount)
        {
            currentBackupIndex = 1;
        }
    }

    public class AutoSaveSceneSettings : EditorWindow
    {
        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Auto Save Settings", EditorStyles.largeLabel);
            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            autoSaveEnabled = EditorGUILayout.BeginToggleGroup("Enable Auto Save", autoSaveEnabled);
            saveFrequency = EditorGUILayout.FloatField("Save Frequency (Minutes)", saveFrequency);
            backupCycleCount = EditorGUILayout.IntSlider("Backup Cycle Count", backupCycleCount, 1, 10);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            remindersEnabled = EditorGUILayout.BeginToggleGroup("Enable Reminders", remindersEnabled);
            remindFrequency = EditorGUILayout.Slider("Remind Frequency (Minutes)", remindFrequency, 1, 60);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Backup Folder", folderName);
            if (GUILayout.Button("Select Backup Folder"))
            {
                SelectBackupFolder();
            }
            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save Backup Now", GUILayout.Width(150), GUILayout.Height(30)))
            {
                SaveSceneBackup();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }

        private void SelectBackupFolder()
        {
            string defaultPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, folderName);
            string selectedPath = EditorUtility.OpenFolderPanel("Select Backup Folder", defaultPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                folderName = selectedPath;
            }
        }
    }
}