# Unity template repo

A reusable Unity 6 starter for game jams and small projects. It ships with a simple UI Toolkit menu stack (View, Controller, Settings model), Input System pause handling, audio, and licensing aligned with my UE template.

## Menu architecture (keep it small)

Ordinary MVC-shaped UI code:

- **View:** `MainMenuView` / `PauseMenuView` query UXML and expose buttons/panels.
- **Controller:** `MainMenuController` / `PauseMenuController` handle clicks and scene flow (~265 lines of C# together with the model).
- **Model:** `SettingsModel` is a plain C# class that holds volume and PlayerPrefs load/save.

Panel swapping is a private method on each controller. No Application hub. No extra Component class.

## Unity version policy

This template does **not** pin a specific editor install. `ProjectSettings/ProjectVersion.txt` is a placeholder. When you first open the project, Unity rewrites it to your installed editor version.

## Quick start

1. Create a repo from this template (GitHub **Use this template**).
2. Clone it and open the folder in Unity 6.
3. Let Unity upgrade the project version file if prompted.
4. Open `MainMenu` and press Play.

Scenes are **committed with Hierarchy objects already wired** (managers, UI Document, Panel Settings). No runtime bootstrap script.

To regenerate scenes from scratch:

```powershell
unity command setup_menus --overwrite true
```

## Folder map

```
Assets/Content/
  Scripts/
    Core/            Singleton, SceneNames
    Editor/          setup_menus + setup_project_settings CLI commands
    Input/           InputHandler + Input System asset
    Managers/        AudioManager, GameManager
    UI/
      Controllers/
      Models/
      Views/
  UI/
    UXML/
    USS/
```

`MenuPanelSettings.asset` is assigned on each UI Document. Styles come from the UXML `Style` reference to `MenuStyles.uss`.

Do **not** add Panel Renderer. Use UI Document + Panel Settings only (see `docs/SCENE_SETUP.md`).

## Menus included

| Screen | Panels |
|--------|--------|
| Main menu | Start, Settings (master volume), Controls, Quit |
| Pause menu | Resume, Settings, Controls, Main menu, Quit |

## Render pipeline

Uses **URP** (`com.unity.render-pipelines.universal`) with:

- `Assets/Settings/URP_Pipeline.asset`
- `Assets/Settings/URP_Renderer.asset`

Assigned as the default Graphics + Quality pipeline. Works for WebGL, PC, and mobile.

## WebGL notes

UI Toolkit menus work on WebGL, PC, and mobile targets.

## Branch protection (optional)

GitHub template repos do not copy rulesets into child repos automatically. Apply under **Settings > Rules > Rulesets** if you want:

- Block branch deletion
- Block non-fast-forward updates

Target all branches. Admin bypass optional.

## Licenses

| Content | License |
|---------|---------|
| Source code | PolyForm Noncommercial 1.0.0 |
| Art / audio / UI art | CC BY-NC 4.0 |

Full details: `LICENSE.md` and `LICENSES/`.

## Unity CLI notes

```powershell
# If `unity` is missing in an old terminal:
$env:Path = [Environment]::GetEnvironmentVariable('Path','Machine') + ';' + [Environment]::GetEnvironmentVariable('Path','User')

unity --version
unity pipeline install
unity pipeline list
unity command setup_menus
```

Pipeline package is already listed in this template's `Packages/manifest.json`.

### Console messages you can ignore

| Message | Meaning |
|---------|---------|
| `Editor is not in automated mode` | Normal when you open Unity from Hub. Only needed for headless CI. Safe to ignore for jam work. |
| `URP Global Settings Asset has been created` | Unity auto-created `Assets/UniversalRenderPipelineGlobalSettings.asset`. Commit it. |
| `type is not a supported int value` | Caused by one-off CLI `eval` scripts treating bool batching fields as ints. Fixed in repo settings. Clear Console after reimport. |

Use proper CLI commands instead of raw `eval` when possible:

```powershell
unity command setup_project_settings
unity command setup_menus
```

## What is intentionally empty

- Pre-baked `.unity` scene files (create once via CLI or by hand)
- Gameplay systems, card AI, combat
- URP / CI workflows
