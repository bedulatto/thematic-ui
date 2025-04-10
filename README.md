# ThematicUI

**ThematicUI** is a theme management system for Unity that allows you to configure, apply, and switch UI visuals dynamically — such as colors, fonts, and sprites. It's ideal for games or apps that support visual customization, dark/light modes, or skin systems.

---

## What it does

- Defines global keys for visual properties (e.g. `BackgroundColor`, `TitleFont`, `ButtonIcon`)
- Lets you create multiple themes with custom values for each key
- Apply themes at runtime with a simple API
- Intuitive custom inspector to create, edit, and preview themes
- Supports `Color`, `Font`, and `Sprite` types (extensible)

---

## How to use

1. Create a new `Theme Asset`  
   → Menu: **Assets > Create > ThematicUI > Theme Asset**

2. Add global key references (`Name` + `Type`): `Color`, `Font`, or `Sprite`

3. Create one or more `Theme` assets inside the Theme Asset

4. Set individual values for each key in every theme

5. At runtime, apply a theme using:

```csharp
ThemeAsset.ChangeTheme("DarkMode");
var bgColor = ThemeAsset.CurrentTheme.Get<ColorKey>("BackgroundColor").Value;
```

6. Use custom scripts or components to apply values to your UI

---

## How to install

### Install via Unity Package Manager

1. Open Unity
2. Go to **Window > Package Manager**
3. Click the **+** button in the top-left corner
4. Select **Add package from Git URL...**
5. Paste the following URL:

```
https://github.com/bedulatto/thematic-ui.git
```

6. Click **Add**  
   The package will be downloaded and installed automatically.

---

## License

MIT License – free for personal and commercial use.
