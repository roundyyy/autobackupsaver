using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class AutoSaveScene
{
    private static bool autoSaveEnabled;
    private static bool remindersEnabled;
    private static int saveFrequency;
    private static float remindFrequency;
    private static string folderName = "BackupScenes";
    private static int backupCycleCount;
    private static int currentBackupIndex = 1;
    private static float nextSaveTime;
    private static float nextRemindTime;
    private static bool compressBackups = true;
    private static bool autoSaveOnPlay = true;
    private static bool showNotifications = true;

    static AutoSaveScene()
    {
        LoadPreferences();
        EditorApplication.update += Update;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void LoadPreferences()
    {
        autoSaveEnabled = EditorPrefs.GetBool("AutoSaveScene_Enabled", false);
        remindersEnabled = EditorPrefs.GetBool("AutoSaveScene_RemindersEnabled", false);
        saveFrequency = EditorPrefs.GetInt("AutoSaveScene_SaveFrequency", 5);
        remindFrequency = EditorPrefs.GetFloat("AutoSaveScene_RemindFrequency", 15f);
        backupCycleCount = EditorPrefs.GetInt("AutoSaveScene_BackupCycleCount", 5);
        folderName = EditorPrefs.GetString("AutoSaveScene_FolderName", "BackupScenes");
        compressBackups = EditorPrefs.GetBool("AutoSaveScene_CompressBackups", true);
        autoSaveOnPlay = EditorPrefs.GetBool("AutoSaveScene_AutoSaveOnPlay", true);
        showNotifications = EditorPrefs.GetBool("AutoSaveScene_ShowNotifications", true);
    }

    private static void SavePreferences()
    {
        EditorPrefs.SetBool("AutoSaveScene_Enabled", autoSaveEnabled);
        EditorPrefs.SetBool("AutoSaveScene_RemindersEnabled", remindersEnabled);
        EditorPrefs.SetInt("AutoSaveScene_SaveFrequency", saveFrequency);
        EditorPrefs.SetFloat("AutoSaveScene_RemindFrequency", remindFrequency);
        EditorPrefs.SetInt("AutoSaveScene_BackupCycleCount", backupCycleCount);
        EditorPrefs.SetString("AutoSaveScene_FolderName", folderName);
        EditorPrefs.SetBool("AutoSaveScene_CompressBackups", compressBackups);
        EditorPrefs.SetBool("AutoSaveScene_AutoSaveOnPlay", autoSaveOnPlay);
        EditorPrefs.SetBool("AutoSaveScene_ShowNotifications", showNotifications);
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (autoSaveOnPlay && state == PlayModeStateChange.ExitingEditMode)
        {
            SaveSceneBackup();
        }
    }

    [MenuItem("Tools/Roundy/Auto Save Scene %#s")]
    public static void ShowWindow()
    {
        AutoSaveSceneSettings window = EditorWindow.GetWindow<AutoSaveSceneSettings>("Auto Save Scene");
        window.minSize = new Vector2(400, 550);
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
                if (showNotifications)
                {
                    EditorUtility.DisplayDialog("Save Reminder", "Don't forget to save your scene!", "OK");
                }
                nextRemindTime = Time.realtimeSinceStartup + remindFrequency * 60;
            }
        }
    }
    private static void SaveSceneBackup()
    {
        if (!EditorSceneManager.GetActiveScene().isDirty)
        {
            if (showNotifications)
            {
                Debug.Log("Auto Save: Scene has no unsaved changes.");
            }
            return;
        }

        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        if (string.IsNullOrEmpty(currentScenePath))
        {
            if (showNotifications)
            {
                Debug.LogWarning("Auto Save: Cannot backup unsaved scene. Please save the scene first.");
            }
            return;
        }

        string backupFolderPath = folderName;
        string sceneName = Path.GetFileNameWithoutExtension(currentScenePath);
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupScenePath = Path.Combine(backupFolderPath,
            $"{sceneName}_backup{currentBackupIndex}_{timestamp}.unity");

        if (!Directory.Exists(backupFolderPath))
        {
            Directory.CreateDirectory(backupFolderPath);
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), backupScenePath, compressBackups);

        if (showNotifications)
        {
            Debug.Log($"Scene backup saved to: {backupScenePath}");
        }

        currentBackupIndex++;
        if (currentBackupIndex > backupCycleCount)
        {
            currentBackupIndex = 1;
            CleanupOldBackups(backupFolderPath, sceneName);
        }
    }

    private static void CleanupOldBackups(string folderPath, string sceneName)
    {
        var directory = new DirectoryInfo(folderPath);
        var files = directory.GetFiles($"{sceneName}_backup*");
        System.Array.Sort(files, (x, y) => y.CreationTime.CompareTo(x.CreationTime));

        // Keep only the most recent backupCycleCount files
        for (int i = backupCycleCount; i < files.Length; i++)
        {
            files[i].Delete();
        }
    }

    public class AutoSaveSceneSettings : EditorWindow
    {
        private Vector2 scrollPosition;
        private GUIStyle titleStyle;
        private GUIStyle sectionStyle;
        private GUIStyle tooltipStyle;

        private void OnEnable()
        {
            titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(10, 10, 10, 10)
            };

            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };

            tooltipStyle = new GUIStyle(EditorStyles.helpBox)
            {
                wordWrap = true,
                fontSize = 12,
                padding = new RectOffset(8, 8, 6, 6),
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = CreateBackgroundTexture(new Color(0.2f, 0.2f, 0.2f, 0.95f))
                },
                richText = true,
                alignment = TextAnchor.MiddleLeft
            };
        }

        private Texture2D CreateBackgroundTexture(Color color)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Auto Save Scene", titleStyle);
            EditorGUILayout.Space(10);

            // Auto Save Section
            EditorGUILayout.BeginVertical(sectionStyle);
            autoSaveEnabled = ToggleField("Enable Auto Save",
                autoSaveEnabled,
                "Automatically save scene backups at specified intervals");

            if (autoSaveEnabled)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Save Frequency (Minutes)", GUILayout.Width(150));  // Fixed width for label
                saveFrequency = (int)EditorGUILayout.Slider(saveFrequency, 1, 60, GUILayout.ExpandWidth(true));
                HandleTooltipForLastRect("How often should the scene be automatically saved (in minutes)");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                autoSaveOnPlay = ToggleField("Auto-Save Before Play Mode",
                    autoSaveOnPlay,
                    "Automatically save the scene before entering play mode");
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            // Reminders Section
            EditorGUILayout.BeginVertical(sectionStyle);
            remindersEnabled = ToggleField("Enable Save Reminders",
                remindersEnabled,
                "Show periodic reminders to save your scene");

            if (remindersEnabled)
            {
                EditorGUI.indentLevel++;
                remindFrequency = SliderField("Remind Frequency (Minutes)",
                    remindFrequency, 1, 60,
                    "How often should save reminders appear (in minutes)");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            // Backup Settings Section
            EditorGUILayout.BeginVertical(sectionStyle);
            EditorGUILayout.LabelField("Backup Settings", EditorStyles.boldLabel);

            backupCycleCount = (int)SliderField("Backup Cycle Count",
                backupCycleCount, 1, 10,
                "Number of backup files to keep before starting to overwrite old ones");

            compressBackups = ToggleField("Compress Backups",
                compressBackups,
                "Compress backup files to save disk space (slightly slower)");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Backup Folder"));

            if (GUILayout.Button(folderName, EditorStyles.textField))
            {
                SelectBackupFolder();
            }
            HandleTooltipForLastRect("Location where backup files will be stored");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            // Notifications Section
            EditorGUILayout.BeginVertical(sectionStyle);
            showNotifications = ToggleField("Show Notifications",
                showNotifications,
                "Display console messages when backups are created");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Action Buttons
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save Backup Now", GUILayout.Width(150)))
            {
                SaveSceneBackup();
            }
            HandleTooltipForLastRect("Create a backup of the current scene immediately");

            GUILayout.Space(10);

            if (GUILayout.Button("Open Backup Folder", GUILayout.Width(150)))
            {
                OpenBackupFolder();
            }
            HandleTooltipForLastRect("Open the folder containing backup files");

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            HandleTooltips();
            EditorGUILayout.EndScrollView();

            if (GUI.changed)
            {
                SavePreferences();
            }
        }

        private bool ToggleField(string label, bool value, string tooltip)
        {
            bool result = EditorGUILayout.Toggle(label, value);
            HandleTooltipForLastRect(tooltip);
            return result;
        }

        private float SliderField(string label, float value, float min, float max, string tooltip)
        {
            float result = EditorGUILayout.Slider(label, value, min, max);
            HandleTooltipForLastRect(tooltip);
            return result;
        }

        private string tooltipToShow;
        private Rect tooltipRect;
        private bool shouldShowTooltip;

        private void HandleTooltipForLastRect(string tooltip)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition))
            {
                tooltipToShow = tooltip;
                tooltipRect = new Rect(
                    Event.current.mousePosition.x + 20,
                    Event.current.mousePosition.y,
                    300, // Initial width - will be recalculated in HandleTooltips
                    30   // Initial height - will be recalculated in HandleTooltips
                );
                shouldShowTooltip = true;
                Repaint();
            }
        }

        private void HandleTooltips()
        {
            if (shouldShowTooltip && !string.IsNullOrEmpty(tooltipToShow))
            {
                // Calculate the required size for the tooltip content
                GUIContent content = new GUIContent(tooltipToShow);
                float maxWidth = 300f;
                float minHeight = 30f;

                // Calculate how much space the text will need
                Vector2 contentSize = tooltipStyle.CalcSize(content);
                float heightForWidth = tooltipStyle.CalcHeight(content, maxWidth);

                // Set the tooltip rectangle size
                tooltipRect.width = Mathf.Min(maxWidth, contentSize.x + 20);  // Add padding
                tooltipRect.height = Mathf.Max(minHeight, heightForWidth + 10);  // Add padding

                // Ensure tooltip stays within window bounds
                tooltipRect.x = Mathf.Min(tooltipRect.x, position.width - tooltipRect.width - 5);
                tooltipRect.y = Mathf.Min(tooltipRect.y, position.height - tooltipRect.height - 5);

                // Draw tooltip background and text
                GUI.Label(tooltipRect, tooltipToShow, tooltipStyle);
            }

            // Reset tooltip if mouse moves away
            if (!new Rect(0, 0, position.width, position.height).Contains(Event.current.mousePosition))
            {
                shouldShowTooltip = false;
                Repaint();
            }
        }

        private void SelectBackupFolder()
        {
            string defaultPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, folderName);
            string selectedPath = EditorUtility.OpenFolderPanel("Select Backup Folder", defaultPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                folderName = selectedPath;
                SavePreferences();
            }
        }

        private void OpenBackupFolder()
        {
            string fullPath = Path.GetFullPath(folderName);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            EditorUtility.RevealInFinder(fullPath);
        }

        private void OnDestroy()
        {
            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (var texture in textures)
            {
                if (texture != null && texture.name == "")
                {
                    DestroyImmediate(texture);
                }
            }
        }
    }
}
