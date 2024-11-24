# AutoSaveScene Tool User Manual

AutoSaveScene is a Unity Editor tool that provides automatic scene backup functionality with configurable settings and notifications. It helps prevent work loss and maintains a history of scene changes.

## Features

### Auto-Save Functionality
- Automatic scene backup at configurable intervals (1-60 minutes)
- Auto-save before entering Play Mode option
- Saves only when changes are detected
- Timestamps in backup file names for better version tracking

### Backup Management
- Configurable backup cycle count (1-10 backups)
- Automatic cleanup of old backups
- Optional backup compression to save disk space
- Customizable backup folder location
- Quick access to backup folder
- Organized backup naming with timestamps

### Save Reminders
- Configurable reminder intervals (1-60 minutes)
- Reminder popups to save scene manually
- Helps maintain good saving habits

### Notifications
- Toggleable notification system
- Console messages for backup operations
- Status updates for:
  - Successful backups
  - No changes detected
  - Unsaved scene warnings
  - Backup folder operations

### Quality of Life
- Quick access through keyboard shortcut (Ctrl/Cmd + Alt + S)
- Manual backup button
- Direct backup folder access
- Persistent settings between Unity sessions
- Tooltip help for all options

## Installation

Download here: [AutoBackupSaver](https://github.com/roundyyy/autobackupsaver/releases)

1. Copy the provided AutoSaveScene script into your Unity project's `Editor` folder. If the folder doesn't exist, create it.
2. The tool will automatically become available in Unity's Tools menu.

## Usage

1. Access the tool via:
   - Menu: `Tools > Roundy > Auto Save Scene`
   - Keyboard shortcut: `Ctrl/Cmd + Alt + S`

2. Configure Auto-Save Settings:
   - Enable Auto Save: Activates automatic backup system
   - Save Frequency: Set interval between backups (1-60 minutes)
   - Auto-Save Before Play: Automatically creates backup before entering Play Mode

3. Configure Backup Settings:
   - Backup Cycle Count: Number of backup files to maintain (1-10)
   - Compress Backups: Enable to reduce backup file size
   - Backup Folder: Choose custom location for backups (default: "BackupScenes" in project root)

4. Configure Reminders (Optional):
   - Enable Save Reminders: Activates reminder system
   - Remind Frequency: Set interval between reminders (1-60 minutes)

5. Notification Settings:
   - Show Notifications: Toggle console messages and notifications

6. Quick Actions:
   - Save Backup Now: Create immediate backup
   - Open Backup Folder: Quick access to backup location

## Backup File Structure

Backup files are named using the following format:
`SceneName_backupN_YYYYMMDD_HHMMSS.unity`
- SceneName: Original scene name
- N: Backup number in cycle
- YYYYMMDD_HHMMSS: Timestamp of backup

## Troubleshooting

- If menu item is not visible: Verify script is in Editor folder
- If backups aren't creating: Check if scene has unsaved changes
- If reminders don't show: Verify Unity is not in Play Mode
- For backup folder access issues: Check write permissions

## Notes

- Scene must be saved at least once before backups can begin
- Backups are only created when changes are detected
- Compressed backups are slightly slower but save disk space
- Settings persist between Unity sessions
