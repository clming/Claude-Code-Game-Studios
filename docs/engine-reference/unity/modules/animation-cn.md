# Unity 6.3 - 动画模块参考

**最后核验：** 2026-02-13  
**知识缺口：** Unity 6 的动画改进、Timeline 增强

---

## 概览

Unity 6.3 的动画系统：
- **Animator Controller（Mecanim）**：基于状态机（推荐）
- **Timeline**：电影化序列、过场动画
- **Animation Rigging**：运行时程序化动画
- **Legacy Animation**：已弃用，避免使用

---

## 相比 2022 LTS 的关键变化

### Animation Rigging 包（在 Unity 6 中已可用于生产）

```csharp
// 安装：Package Manager > Animation Rigging
// 运行时 IK、瞄准约束、程序化动画
```

### Timeline 改进
- 更好的性能
- 更多轨道类型
- 更完善的信号系统

---

## Animator Controller（Mecanim）

### 基础设置

```csharp
// 创建：Assets > Create > Animator Controller
// 添加到 GameObject：Add Component > Animator
// 指定控制器：Animator > Controller = YourAnimatorController
```

### 状态切换

```csharp
Animator animator = GetComponent<Animator>();

// 正确：Trigger 触发切换
animator.SetTrigger("Jump");

// 正确：Bool 参数
animator.SetBool("IsRunning", true);

// 正确：Float 参数（Blend Tree）
animator.SetFloat("Speed", currentSpeed);

// 正确：Integer 参数
animator.SetInteger("WeaponType", 2);
```

### 动画层
- **Base Layer**：默认动画（如移动）
- **Override Layers**：覆盖基础层（如切换武器）
- **Additive Layers**：叠加在基础层之上（如呼吸、瞄准偏移）

```csharp
// 设置层权重（0-1）
animator.SetLayerWeight(1, 0.5f); // 50% 混合
```

---

## Blend Tree

### 1D Blend Tree（速度混合）

```csharp
// Idle (Speed = 0) -> Walk (Speed = 0.5) -> Run (Speed = 1.0)
animator.SetFloat("Speed", moveSpeed);
```

### 2D Blend Tree（方向移动）

```csharp
// X 轴：横移（-1 到 1）
// Y 轴：前进/后退（-1 到 1）
animator.SetFloat("MoveX", input.x);
animator.SetFloat("MoveY", input.y);
```

---

## 动画事件

### 从动画片段触发事件

```csharp
// 在 Animation 窗口中：右键时间线 > Add Animation Event
// GameObject 上必须存在同名方法：

public void OnFootstep() {
    // 播放脚步声
    AudioSource.PlayClipAtPoint(footstepClip, transform.position);
}

public void OnAttackHit() {
    // 造成伤害
    DealDamageInFrontOfPlayer();
}
```

---

## Root Motion

### 通过动画驱动角色移动

```csharp
Animator animator = GetComponent<Animator>();
animator.applyRootMotion = true; // 根据动画移动角色

void OnAnimatorMove() {
    // 自定义 Root Motion 处理
    transform.position += animator.deltaPosition;
    transform.rotation *= animator.deltaRotation;
}
```

---

## Animation Rigging（Unity 6+）

### IK（反向运动学）

```csharp
// 安装：Package Manager > Animation Rigging
// 添加：Rig Builder 组件 + Rig GameObject

// Two Bone IK（手臂/腿）
// - 添加 Two Bone IK Constraint
// - 指定 Tip（手/脚）、Mid（肘/膝）、Root（肩/髋）
// - 设置 Target（手/脚要到达的位置）

// 运行时控制：
TwoBoneIKConstraint ikConstraint = rig.GetComponentInChildren<TwoBoneIKConstraint>();
ikConstraint.data.target = targetTransform;
ikConstraint.weight = 1f; // 0-1 混合
```

### Aim Constraint（朝向目标）

```csharp
// 让角色看向目标
MultiAimConstraint aimConstraint = rig.GetComponentInChildren<MultiAimConstraint>();
aimConstraint.data.sourceObjects[0] = new WeightedTransform(targetTransform, 1f);
```

---

## Timeline（过场动画）

### 基础 Timeline 设置

```csharp
// 创建：Assets > Create > Timeline
// 添加到 GameObject：Add Component > Playable Director
// 指定 Timeline：Playable Director > Playable = YourTimeline

// 从脚本播放：
PlayableDirector director = GetComponent<PlayableDirector>();
director.Play();
```

### Timeline 轨道
- **Activation Track**：启用/禁用 GameObject
- **Animation Track**：在 Animator 上播放动画
- **Audio Track**：同步播放音频
- **Cinemachine Track**：镜头移动
- **Signal Track**：在指定时间触发事件

### Signal 系统（事件）

```csharp
// 创建 Signal 资源：Assets > Create > Signals > Signal
// 在 Timeline 轨道上添加 Signal Emitter
// 给 GameObject 添加 Signal Receiver 组件

public class CutsceneEvents : MonoBehaviour {
    public void OnDialogueStart() {
        // 由 signal 触发
    }
}
```

---

## 动画播放控制

### 直接播放动画（不使用状态机）

```csharp
// 正确：CrossFade（平滑过渡）
animator.CrossFade("Attack", 0.2f); // 0.2 秒过渡

// 正确：Play（立即播放）
animator.Play("Idle");

// 错误：避免使用 Legacy Animation 组件
Animation anim = GetComponent<Animation>(); // 已弃用
```

---

## 动画曲线

### 自定义属性动画

```csharp
// 在 Animation 窗口中：Add Property > Custom Component > Your Script > Your Float

public class WeaponTrail : MonoBehaviour {
    public float trailIntensity; // 由动画片段控制

    void Update() {
        // 强度由动画曲线控制
        trailRenderer.startWidth = trailIntensity;
    }
}
```

---

## 性能优化

### Culling
- `Animator > Culling Mode`：
  - **Always Animate**：始终更新（开销大）
  - **Cull Update Transforms**：离屏时停止更新骨骼（推荐）
  - **Cull Completely**：离屏时完全停止动画

### LOD（细节层级）
- 远处角色使用更简单的动画
- 为 LOD 网格减少骨骼数量

---

## 常见模式

### 检查动画是否播放完成

```csharp
AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1.0f) {
    // 攻击动画已结束
}
```

### 覆盖动画速度

```csharp
animator.speed = 1.5f; // 150% 速度
```

### 获取当前动画名称

```csharp
AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
string currentClip = clipInfo[0].clip.name;
```

---

## 调试

### Animator 窗口
- `Window > Animation > Animator`
- 可视化状态机，查看当前激活状态

### Animation 窗口
- `Window > Animation > Animation`
- 编辑动画片段、添加事件

---

## 来源
- https://docs.unity3d.com/6000.0/Documentation/Manual/AnimationOverview.html
- https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/index.html
- https://docs.unity3d.com/Packages/com.unity.timeline@1.8/manual/index.html
