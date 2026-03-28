# Unity 6.3 - UI 模块参考

**最后核验：** 2026-02-13  
**知识缺口：** Unity 6 的 UI Toolkit 已可稳定用于运行时 UI

---

## 概览

Unity 6 的 UI 系统：
- **UI Toolkit**（推荐）：现代、高性能、类似 HTML/CSS（在 Unity 6 中已可用于生产）
- **UGUI（Canvas）**：旧版系统，仍受支持，但不推荐用于新项目
- **IMGUI**：仅编辑器用途，运行时 UI 已弃用

---

## UI Toolkit（现代 UI）

### 设置 UI Document

1. 创建 UXML（UI 结构）：
   - `Assets > Create > UI Toolkit > UI Document`
2. 创建 USS（样式）：
   - `Assets > Create > UI Toolkit > StyleSheet`
3. 添加到场景：
   - `GameObject > UI Toolkit > UI Document`
   - 将 UXML 指定到 `UIDocument > Source Asset`

---

### UXML（UI 结构）

```xml
<!-- MainMenu.uxml -->
<ui:UXML xmlns:ui="UnityEngine.UIElements">
    <ui:VisualElement class="container">
        <ui:Label text="Main Menu" class="title" />
        <ui:Button name="play-button" text="Play" />
        <ui:Button name="settings-button" text="Settings" />
        <ui:Button name="quit-button" text="Quit" />
    </ui:VisualElement>
</ui:UXML>
```

---

### USS（样式）

```css
.container {
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
    background-color: rgb(30, 30, 30);
}

.title {
    font-size: 48px;
    color: white;
    margin-bottom: 20px;
}

Button {
    width: 200px;
    height: 50px;
    margin: 10px;
    font-size: 24px;
}

Button:hover {
    background-color: rgb(100, 150, 200);
}
```

---

### C# 脚本（UI Toolkit）

```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour {
    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var playButton = root.Q<Button>("play-button");
        var settingsButton = root.Q<Button>("settings-button");
        var quitButton = root.Q<Button>("quit-button");

        playButton.clicked += OnPlayClicked;
        settingsButton.clicked += OnSettingsClicked;
        quitButton.clicked += Application.Quit;
    }

    void OnPlayClicked() { }
    void OnSettingsClicked() { }
}
```

---

### 常见 UI 元素

```csharp
var label = root.Q<Label>("score-label");
label.text = "Score: 100";

var button = root.Q<Button>("submit-button");
button.clicked += OnSubmit;

var textField = root.Q<TextField>("name-input");
string playerName = textField.value;

var toggle = root.Q<Toggle>("music-toggle");
bool isMusicEnabled = toggle.value;

var slider = root.Q<Slider>("volume-slider");
float volume = slider.value;

var dropdown = root.Q<DropdownField>("difficulty-dropdown");
dropdown.choices = new List<string> { "Easy", "Normal", "Hard" };
dropdown.value = "Normal";
```

---

### 动态创建 UI（不使用 UXML）

```csharp
void CreateUI() {
    var root = GetComponent<UIDocument>().rootVisualElement;

    var container = new VisualElement();
    container.AddToClassList("container");

    var label = new Label("Hello, UI Toolkit!");
    var button = new Button(() => Debug.Log("Clicked")) { text = "Click Me" };

    container.Add(label);
    container.Add(button);
    root.Add(container);
}
```

---

### USS Flexbox 布局

```css
.horizontal {
    flex-direction: row;
}

.vertical {
    flex-direction: column;
}

.centered {
    align-items: center;
    justify-content: center;
}

.spaced {
    justify-content: space-between;
}
```

---

## UGUI（旧版 Canvas UI）

### 基础设置

```csharp
// GameObject > UI > Canvas
// Text 建议改用 TextMeshPro
```

### UGUI 脚本

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LegacyUI : MonoBehaviour {
    public TextMeshProUGUI scoreText;
    public Button playButton;
    public Slider volumeSlider;

    void Start() {
        scoreText.text = "Score: 100";
        playButton.onClick.AddListener(OnPlayClicked);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnPlayClicked() { }

    void OnVolumeChanged(float value) {
        AudioListener.volume = value;
    }
}
```

### TextMeshPro

```csharp
using TMPro;

public TextMeshProUGUI tmpText;
tmpText.text = "High Quality Text";
tmpText.fontSize = 24;
tmpText.color = Color.white;
```

---

## Canvas 设置（UGUI）

### 渲染模式

```csharp
// Screen Space - Overlay
// Screen Space - Camera
// World Space
```

### Canvas Scaler

```csharp
// 推荐：Scale With Screen Size
// Reference Resolution：1920x1080
```

---

## Layout Groups（UGUI）

### Horizontal Layout Group

```csharp
// 自动水平排列子元素
```

### Vertical Layout Group

```csharp
// 自动垂直排列子元素
```

### Grid Layout Group

```csharp
// 以网格方式排列子元素
```

---

## 性能（UI Toolkit vs UGUI）

### UI Toolkit 的优势
- 更快的渲染
- 更适合复杂 UI
- 样式管理更容易
- 更适合动态 UI

### UGUI 的优势
- 更成熟
- 资料更多
- 对初学者更直观

---

## 常见模式

### 血条（UI Toolkit）

```csharp
var healthBar = root.Q<VisualElement>("health-bar");
healthBar.style.width = new StyleLength(new Length(healthPercent, LengthUnit.Percent));
```

### 血条（UGUI）

```csharp
public Image healthBarImage;

void UpdateHealth(float percent) {
    healthBarImage.fillAmount = percent;
}
```

### 淡入/淡出（UI Toolkit）

```csharp
IEnumerator FadeIn(VisualElement element, float duration) {
    float elapsed = 0f;
    while (elapsed < duration) {
        elapsed += Time.deltaTime;
        element.style.opacity = Mathf.Lerp(0f, 1f, elapsed / duration);
        yield return null;
    }
}
```

---

## 调试

### UI Toolkit Debugger
- `Window > UI Toolkit > Debugger`

### UGUI Event System Debugger
- 在 Hierarchy 中选中 EventSystem

---

## 来源
- https://docs.unity3d.com/6000.0/Documentation/Manual/UIElements.html
- https://docs.unity3d.com/Packages/com.unity.ui@2.0/manual/index.html
- https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html
