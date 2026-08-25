# Scene setup

Scenes are saved in the repo with everything wired in the Hierarchy. You should not need a runtime bootstrap script.

## MainMenu scene hierarchy

- **Main Camera** / **Directional Light** (default)
- **AudioManager** — `AudioSource` + `AudioManager`
- **GameManager** — `GameManager`
- **MainMenuUI** — `UI Document` + `MainMenuView` + `MainMenuController`
  - Panel Settings: `Assets/Content/UI/MenuPanelSettings.asset`
  - Source Asset: `Assets/Content/UI/UXML/MainMenu.uxml`

## SampleGameplay scene hierarchy

- **Main Camera** / **Directional Light**
- **PauseMenuUI** — `UI Document` + `PauseMenuView` + `PauseMenuController` + `InputHandler`
  - Panel Settings: `Assets/Content/UI/MenuPanelSettings.asset`
  - Source Asset: `Assets/Content/UI/UXML/PauseMenu.uxml`

`AudioManager` and `GameManager` live only in **MainMenu**. They use `DontDestroyOnLoad`, so they persist when you load gameplay. No duplicate managers, no singleton warnings.

## UI Document vs Panel Renderer

Use **UI Document** with a **Panel Settings** asset. That is the standard UI Toolkit screen-space setup.

Unity may show a banner suggesting **Panel Renderer**. Ignore it for this template. Panel Renderer is a newer component aimed at different workflows and can conflict if you add it alongside UI Document.

## Regenerate scenes (optional)

If you delete scenes or want a clean rebuild:

```powershell
unity command setup_menus --overwrite true
```

Requires the project open in the Editor with Pipeline connected.

## Build settings

Both scenes should already be listed under **File > Build Settings**:

1. `MainMenu`
2. `SampleGameplay`

## Input System

**Edit > Project Settings > Player > Active Input Handling** should be **Input System Package** (or run `unity command setup_project_settings`).
