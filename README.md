<img src="https://beeimg.com/images/j01302805902.png" alt="Unity Toolbox Logo" width="400"/>

# AutoSaveScene Tool User Manual

AutoSaveScene is a Unity tool that provides automatic scene saving and reminder functionalities.

## Features

- Auto-save backup scenes at a specified interval
- Configurable backup cycle count
- Reminders to save scenes manually
- Customizable backup folder

## Installation
Download here : [AutoBackupSaver](https://github.com/roundyyy/autobackupsaver/releases)

1. Copy the provided AutoSaveScene script into your Unity project's `Editor` folder. If the folder doesn't exist, create it.

## Usage

1. In the Unity Editor, go to the menu bar and click on `Tools` > `Auto Save Scene` to open the Auto Save Scene settings window.
2. Configure the auto-save settings:

    - Enable Auto Save: Toggle the auto-save functionality on or off.
    - Save Frequency (Minutes): Set the interval (in minutes) for backup scene auto-saving.
    - Backup Cycle Count: Choose the number of backups to cycle through before overwriting the oldest backup.

3. Configure the reminder settings:

    - Enable Reminders: Toggle the reminder functionality on or off.
    - Remind Frequency (Minutes): Set the interval (in minutes) for showing reminders to save your scene manually.

4. Set the backup folder (optionally):

    - Click the "Select Backup Folder" button to choose a folder for storing scene backups.
    - Default : BackupScenes in root of Project Files

5. (optionally) Click the "Save Backup Now" button to manually create a backup of the current scene.

## Troubleshooting

- If the Auto Save Scene menu item is not visible in the `Tools` menu, make sure the script is in the `Editor` folder.
