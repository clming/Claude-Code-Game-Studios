# Unity 6.3 - 音频模块参考

**最后核验：** 2026-02-13  
**知识缺口：** Unity 6 的 Audio Mixer 改进

---

## 概览

Unity 6.3 的音频系统：
- **AudioSource**：在 GameObject 上播放声音
- **Audio Mixer**：混音、效果处理、动态混音
- **Spatial Audio**：3D 定位音效

---

## 基础音频播放

### AudioSource 组件

```csharp
AudioSource audioSource = GetComponent<AudioSource>();

// 正确：播放
audioSource.Play();

// 正确：延迟播放
audioSource.PlayDelayed(0.5f); // 0.5 秒

// 正确：单次播放（不打断当前声音）
audioSource.PlayOneShot(clip);

// 正确：停止
audioSource.Stop();

// 正确：暂停/恢复
audioSource.Pause();
audioSource.UnPause();
```

### 在指定位置播放声音（静态方法）

```csharp
// 正确：快速播放 3D 音效（播放完会自动销毁）
AudioSource.PlayClipAtPoint(clip, transform.position);

// 正确：带音量
AudioSource.PlayClipAtPoint(clip, transform.position, 0.7f);
```

---

## 3D 空间音频

### AudioSource 的 3D 设置

```csharp
AudioSource source = GetComponent<AudioSource>();

// Spatial Blend: 0 = 2D, 1 = 3D
source.spatialBlend = 1.0f; // 完全 3D

// 多普勒效果（基于速度的音高变化）
source.dopplerLevel = 1.0f;

// 距离衰减
source.minDistance = 1f;   // 该距离内保持满音量
source.maxDistance = 50f;  // 超出该距离后不可听见
source.rolloffMode = AudioRolloffMode.Logarithmic; // 自然衰减
```

### 音量衰减曲线
- **Logarithmic**：自然、真实（推荐）
- **Linear**：线性下降
- **Custom**：自定义曲线

---

## Audio Mixer（高级混音）

### 设置 Audio Mixer

1. `Assets > Create > Audio Mixer`
2. 打开混音器：`Window > Audio > Audio Mixer`
3. 创建分组：Master > SFX、Music、Dialogue

### 将 AudioSource 指向 Mixer Group

```csharp
using UnityEngine.Audio;

public AudioMixerGroup sfxGroup;

void Start() {
    AudioSource source = GetComponent<AudioSource>();
    source.outputAudioMixerGroup = sfxGroup; // 路由到 SFX 组
}
```

### 在代码中控制 Mixer

```csharp
using UnityEngine.Audio;

public AudioMixer audioMixer;

// 正确：设置音量（暴露参数）
audioMixer.SetFloat("MusicVolume", -10f); // dB（-80 到 0）

// 正确：获取音量
audioMixer.GetFloat("MusicVolume", out float volume);

// 线性值（0-1）转 dB
float volumeDB = Mathf.Log10(volumeLinear) * 20f;
audioMixer.SetFloat("MusicVolume", volumeDB);
```

### 暴露 Mixer 参数
在 Audio Mixer 窗口中：
1. 右键参数（例如 Volume）
2. 选择 `"Expose 'Volume' to script"`
3. 在 `"Exposed Parameters"` 面板中重命名（例如 `"MusicVolume"`）

---

## 音频效果

### 给 Mixer Group 添加效果

在 Audio Mixer 中：
- 点击某个组（如 SFX）
- 点击 `"Add Effect"`
- 选择：Reverb、Echo、Low Pass、High Pass、Distortion 等

### 对话期间压低音乐（Sidechain / Ducking）

```csharp
// 在 Audio Mixer 中设置：
// 1. 创建 "Duck Volume" snapshot
// 2. 在该 snapshot 中降低音乐音量
// 3. 当对话播放时切换到该 snapshot

public AudioMixerSnapshot normalSnapshot;
public AudioMixerSnapshot duckedSnapshot;

public void PlayDialogue(AudioClip clip) {
    duckedSnapshot.TransitionTo(0.5f); // 0.5 秒切换
    audioSource.PlayOneShot(clip);
    Invoke(nameof(RestoreMusic), clip.length);
}

void RestoreMusic() {
    normalSnapshot.TransitionTo(1.0f); // 1 秒切回
}
```

---

## 音频性能

### 优化音频加载

```csharp
// 音频导入设置（Inspector）：
// - Load Type：
//   - Decompress On Load：小型音频（SFX），完整载入内存
//   - Compressed In Memory：中型音频，运行时解压（推荐）
//   - Streaming：大型音频（音乐），从磁盘流式读取

// Compression Format：
// - PCM：未压缩，质量最高，体积最大
// - ADPCM：约 3.5 倍压缩，适合 SFX（推荐用于 SFX）
// - Vorbis/MP3：高压缩率，适合音乐（推荐用于音乐）
```

### 预加载音频

```csharp
// 播放前先预加载音频片段（避免卡顿）
audioSource.clip.LoadAudioData();

// 检查是否已加载
if (audioSource.clip.loadState == AudioDataLoadState.Loaded) {
    audioSource.Play();
}
```

---

## 音乐系统

### 曲目间淡入淡出

```csharp
public IEnumerator CrossfadeMusic(AudioSource from, AudioSource to, float duration) {
    float elapsed = 0f;
    to.Play();

    while (elapsed < duration) {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        from.volume = Mathf.Lerp(1f, 0f, t);
        to.volume = Mathf.Lerp(0f, 1f, t);

        yield return null;
    }

    from.Stop();
}
```

### 无缝循环音乐

```csharp
// Audio Import Settings：
// - 勾选 "Loop" 实现无缝循环
audioSource.loop = true;
```

---

## 常见模式

### 随机音高变化（避免重复感）

```csharp
void PlaySoundWithVariation(AudioClip clip) {
    AudioSource source = GetComponent<AudioSource>();
    source.pitch = Random.Range(0.9f, 1.1f); // 正负 10% 音高变化
    source.PlayOneShot(clip);
}
```

### 脚步声（从数组中随机选择）

```csharp
public AudioClip[] footstepClips;

void PlayFootstep() {
    AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
    AudioSource.PlayClipAtPoint(clip, transform.position, 0.5f);
}
```

### 检查声音是否正在播放

```csharp
if (audioSource.isPlaying) {
    // 当前正在播放声音
}
```

---

## Audio Listener

### 单监听器规则
- 同一时刻只能有一个激活的 `AudioListener`
- 通常挂在 Main Camera 上

```csharp
// 禁用多余的监听器
AudioListener listener = GetComponent<AudioListener>();
listener.enabled = false;
```

---

## 调试

### Audio 窗口
- `Window > Audio > Audio Mixer`
- 可视化电平、测试快照切换

### Audio Settings
- `Edit > Project Settings > Audio`
- 全局音量、DSP 缓冲区大小、扬声器模式

---

## 来源
- https://docs.unity3d.com/6000.0/Documentation/Manual/Audio.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/AudioMixer.html
