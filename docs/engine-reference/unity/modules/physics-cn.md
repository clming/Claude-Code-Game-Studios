# Unity 6.3 - 物理模块参考

**最后核验：** 2026-02-13  
**知识缺口：** Unity 6 的物理改进、求解器变化

---

## 概览

Unity 6.3 使用 **PhysX 5.1**（相较于 2022 LTS 的 PhysX 4.x 有提升）：
- 更好的求解器稳定性
- 更佳性能
- 更强的碰撞检测

---

## 相比 2022 LTS 的关键变化

### 默认 Solver Iterations 增加
Unity 6 提高了默认求解器迭代次数，以获得更好的稳定性：

```csharp
// 默认值从 6 提高到 8 次迭代
Physics.defaultSolverIterations = 8; // 如果依赖旧行为，请检查
```

### 增强的碰撞检测

```csharp
// 正确：Unity 6 改进了连续碰撞检测（CCD）
rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
// 对高速运动物体的处理更好
```

---

## 核心物理组件

### Rigidbody

```csharp
// 正确实践：使用 AddForce，而不是直接写 velocity
Rigidbody rb = GetComponent<Rigidbody>();
rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);

// 错误：避免直接赋值 velocity（可能导致不稳定）
rb.velocity = new Vector3(0, 10, 0); // 仅在确有必要时使用
```

### Colliders

```csharp
// 基础碰撞体：Box、Sphere、Capsule（最便宜）
// Mesh Collider：开销高，仅用于静态几何体

// 正确：复合碰撞体（多个基础碰撞体）优于单个 Mesh Collider
```

---

## 射线检测

### 高效 Raycast（避免分配）

```csharp
if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance)) {
    Debug.Log($"Hit: {hit.collider.name}");
}

RaycastHit[] results = new RaycastHit[10];
int hitCount = Physics.RaycastNonAlloc(origin, direction, results, maxDistance);
for (int i = 0; i < hitCount; i++) {
    Debug.Log($"Hit {i}: {results[i].collider.name}");
}

// 错误：避免 RaycastAll（每次调用都会分配数组）
RaycastHit[] hits = Physics.RaycastAll(origin, direction);
```

### 使用 LayerMask 进行选择性检测

```csharp
int layerMask = 1 << LayerMask.NameToLayer("Enemy");
Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask);
```

---

## 物理查询

### OverlapSphere（检查附近对象）

```csharp
Collider[] results = new Collider[10];
int count = Physics.OverlapSphereNonAlloc(center, radius, results);
for (int i = 0; i < count; i++) {
    // 处理 results[i]
}
```

### SphereCast（带厚度的射线）

```csharp
if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, maxDistance)) {
    // 用球形射线命中了某物
}
```

---

## 碰撞事件

### OnCollisionEnter / Stay / Exit

```csharp
void OnCollisionEnter(Collision collision) {
    Debug.Log($"Collided with {collision.gameObject.name}");

    foreach (ContactPoint contact in collision.contacts) {
        Debug.DrawRay(contact.point, contact.normal, Color.red, 2f);
    }
}
```

### OnTriggerEnter / Stay / Exit

```csharp
void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Pickup")) {
        Destroy(other.gameObject);
    }
}
```

---

## 角色控制器

### CharacterController 组件

```csharp
CharacterController controller = GetComponent<CharacterController>();

Vector3 move = transform.forward * speed * Time.deltaTime;
controller.Move(move);

if (!controller.isGrounded) {
    velocity.y += Physics.gravity.y * Time.deltaTime;
}
controller.Move(velocity * Time.deltaTime);
```

---

## 物理材质

### 摩擦与弹性

```csharp
// 创建：Assets > Create > Physic Material
// 指定给碰撞体：Collider > Material
```

---

## 关节

### Fixed Joint（连接两个刚体）

```csharp
FixedJoint joint = gameObject.AddComponent<FixedJoint>();
joint.connectedBody = otherRigidbody;
```

### Hinge Joint（门、车轮）

```csharp
HingeJoint hinge = gameObject.AddComponent<HingeJoint>();
hinge.axis = Vector3.up;
hinge.useLimits = true;
hinge.limits = new JointLimits { min = -90, max = 90 };
```

---

## 性能优化

### Physics Layer Collision Matrix
`Edit > Project Settings > Physics > Layer Collision Matrix`
- 关闭不必要的层间碰撞检测
- 性能提升非常明显

### Fixed Timestep
`Edit > Project Settings > Time > Fixed Timestep`
- 默认：0.02（50 FPS 物理帧率）
- 值越低越精确，但 CPU 开销更高
- 尽量与游戏目标帧率匹配

### 简化碰撞几何
- 优先使用基础碰撞体而不是 mesh collider
- 在构建时烘焙 mesh collider，不要在运行时生成

---

## 常见模式

### 地面检测

```csharp
bool IsGrounded() {
    float rayLength = 0.1f;
    return Physics.Raycast(transform.position, Vector3.down, rayLength);
}
```

### 应用爆炸力

```csharp
void ApplyExplosion(Vector3 explosionPos, float radius, float force) {
    Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
    foreach (Collider hit in colliders) {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.AddExplosionForce(force, explosionPos, radius);
        }
    }
}
```

---

## 调试

### Physics Debugger（Unity 6+）
- `Window > Analysis > Physics Debugger`
- 可视化碰撞体、接触点、查询

### Gizmos

```csharp
void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);
}
```

---

## 来源
- https://docs.unity3d.com/6000.0/Documentation/Manual/PhysicsOverview.html
- https://docs.unity3d.com/ScriptReference/Physics.html
