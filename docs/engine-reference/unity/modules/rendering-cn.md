# Unity 6.3 - 渲染模块参考

**最后核验：** 2026-02-13  
**知识缺口：** LLM 训练基于 Unity 2022 LTS；Unity 6 在渲染方面有较大变化

---

## 概览

Unity 6.3 LTS 使用 **Scriptable Render Pipelines（SRP）** 作为现代渲染架构：
- **URP（Universal Render Pipeline）**：跨平台、移动端友好（推荐）
- **HDRP（High Definition Render Pipeline）**：高端 PC/主机、偏照片真实感
- **Built-in Pipeline**：已弃用，新项目应避免使用

---

## 相比 2022 LTS 的关键变化

### RenderGraph API（Unity 6+）
自定义渲染 Pass 现在使用 RenderGraph，而不是 CommandBuffer：

```csharp
public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
    using var builder = renderGraph.AddRasterRenderPass<PassData>("MyPass", out var passData);
    builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => {
        // 渲染命令
    });
}

// 旧方式：仍可用，但已弃用
public override void Execute(ScriptableRenderContext context, ref RenderingData data) { }
```

### GPU Resident Drawer（Unity 6+）

```csharp
// 在 URP Asset 设置中启用：
// Rendering > GPU Resident Drawer = Enabled
```

---

## URP 快速参考

### 创建 URP Asset
1. `Assets > Create > Rendering > URP Asset (with Universal Renderer)`
2. 指定到 `Project Settings > Graphics > Scriptable Render Pipeline Settings`

### URP Renderer Features

```csharp
using UnityEngine.Rendering.Universal;

public class OutlineRendererFeature : ScriptableRendererFeature {
    OutlineRenderPass pass;

    public override void Create() {
        pass = new OutlineRenderPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data) {
        renderer.EnqueuePass(pass);
    }
}
```

---

## 材质与着色器

### Shader Graph（可视化着色器编辑器）

```csharp
// 创建：Assets > Create > Shader Graph > URP > Lit Shader Graph
```

### HLSL 自定义着色器（URP）

```hlsl
Shader "Custom/URPLit" {
    Properties {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                return half4(1, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}
```

---

## 光照

### 烘焙光照

```csharp
// 将对象标记为静态：Inspector > Static > Contribute GI
// 烘焙：Window > Rendering > Lighting > Generate Lighting
```

### 实时灯光（URP）

```csharp
// 主光源由 URP 自动处理
int lightCount = GetAdditionalLightsCount();
```

---

## 后处理

### Volume 系统（Unity 6+）

```csharp
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

Volume volume = GetComponent<Volume>();
if (volume.profile.TryGet<Bloom>(out var bloom)) {
    bloom.intensity.value = 2.5f;
}
```

---

## 性能

### SRP Batcher

```csharp
// 启用：URP Asset > Advanced > SRP Batcher = Enabled
```

### GPU Instancing

```csharp
Graphics.RenderMeshInstanced(
    new RenderParams(material),
    mesh,
    0,
    matrices
);
```

### 遮挡剔除

```csharp
// Window > Rendering > Occlusion Culling
```

---

## 常见模式

### 自定义摄像机渲染

```csharp
var cameraData = frameData.Get<UniversalCameraData>();
var camera = cameraData.camera;
var colorTarget = cameraData.renderer.cameraColorTargetHandle;
```

### 屏幕空间效果

```csharp
// 创建 ScriptableRendererFeature
// 在指定阶段插入 pass：AfterRenderingOpaques、AfterRenderingTransparents 等
```

---

## 调试

### Frame Debugger
- `Window > Analysis > Frame Debugger`
- 逐步查看 Draw Call，检查渲染状态

### Rendering Debugger（Unity 6+）
- `Window > Analysis > Rendering Debugger`
- 实时查看 URP 设置、过绘制、光照等

---

## 来源
- https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/index.html
- https://docs.unity3d.com/6000.0/Documentation/Manual/render-pipelines.html
