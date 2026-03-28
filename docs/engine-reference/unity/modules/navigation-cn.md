# Unity 6.3 - 导航模块参考

**最后核验：** 2026-02-13  
**知识缺口：** Unity 6 的 NavMesh 改进

---

## 概览

Unity 6 的导航系统：
- **NavMesh**：内置 AI 寻路系统
- **NavMeshComponents**：支持运行时构建 NavMesh 的包

---

## NavMesh 基础

### 烘焙导航网格

1. 标记可行走表面：
   - 选中 GameObject（地面/地形）
   - Inspector > Navigation > Object
   - 勾选 `"Navigation Static"`

2. 烘焙 NavMesh：
   - `Window > AI > Navigation`
   - Bake 标签页
   - 点击 `"Bake"`

3. 配置参数：
   - **Agent Radius**：代理宽度（默认 0.5m）
   - **Agent Height**：代理高度（默认 2m）
   - **Max Slope**：最大可行走坡度（默认 45 度）
   - **Step Height**：最大可攀爬台阶高度（默认 0.4m）

---

## NavMeshAgent（AI 移动）

### 基础代理设置

```csharp
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
    private NavMeshAgent agent;
    public Transform target;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        // 正确：移动到目标位置
        agent.SetDestination(target.position);
    }
}
```

---

### NavMeshAgent 属性

```csharp
NavMeshAgent agent = GetComponent<NavMeshAgent>();

// 速度
agent.speed = 3.5f;

// 加速度
agent.acceleration = 8f;

// 停止距离
agent.stoppingDistance = 2f; // 在目标前 2 米停下

// 自动刹车（到达目标时减速）
agent.autoBraking = true;

// 转向速度
agent.angularSpeed = 120f; // 每秒旋转角度

// 避障
agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
```

---

### 检查路径状态

```csharp
void Update() {
    agent.SetDestination(target.position);

    // 检查代理是否已有路径
    if (agent.hasPath) {
        // 检查路径是否完整
        if (agent.pathStatus == NavMeshPathStatus.PathComplete) {
            Debug.Log("Valid path");
        } else if (agent.pathStatus == NavMeshPathStatus.PathPartial) {
            Debug.Log("Partial path (destination unreachable)");
        } else {
            Debug.Log("Invalid path");
        }
    }

    // 检查代理是否已到达
    if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
        Debug.Log("Reached destination");
    }
}
```

---

### 先计算路径（暂不移动）

```csharp
NavMeshPath path = new NavMeshPath();
agent.CalculatePath(targetPosition, path);

if (path.status == NavMeshPathStatus.PathComplete) {
    // 存在有效路径
    agent.SetPath(path); // 应用路径
}
```

---

## NavMesh 区域（行走代价）

### 定义区域
`Window > AI > Navigation > Areas`
- **Walkable**：代价 1（默认）
- **Not Walkable**：不可行走
- **Jump**：代价 2（尽量走别的路线）
- **Custom**：自定义

### 指定区域代价

```csharp
// 可走所有区域
agent.areaMask = NavMesh.AllAreas;

// 只走 "Walkable" 区域（避开 "Jump"）
agent.areaMask = 1 << NavMesh.GetAreaFromName("Walkable");
```

---

## NavMeshObstacle（动态障碍）

### NavMeshObstacle 组件

```csharp
// 添加：GameObject > Add Component > NavMesh Obstacle

// Carve：在 NavMesh 上挖洞（代理会绕开）
// Don't Carve：代理靠本地避障推开通过
```

### 动态 Carving（移动障碍）

```csharp
NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
obstacle.carving = true; // 在 NavMesh 上创建动态空洞
```

---

## Off-Mesh Links（跳跃、传送）

### 创建 Off-Mesh Link

1. `GameObject > Create Empty`（放在跳跃起点）
2. 添加 `Off Mesh Link` 组件
3. 设置起点/终点 Transform
4. 配置：
   - **Bi-Directional**：能否双向通行
   - **Cost Override**：该链接的路径代价

### 检测 Off-Mesh Link 穿越

```csharp
void Update() {
    // 检查代理是否处于 off-mesh link 上
    if (agent.isOnOffMeshLink) {
        // 手动穿越（例如播放跳跃动画）
        StartCoroutine(TraverseOffMeshLink());
    }
}

IEnumerator TraverseOffMeshLink() {
    OffMeshLinkData data = agent.currentOffMeshLinkData;
    Vector3 startPos = agent.transform.position;
    Vector3 endPos = data.endPos;

    float duration = 0.5f;
    float elapsed = 0f;

    while (elapsed < duration) {
        agent.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    agent.CompleteOffMeshLink(); // 恢复正常寻路
}
```

---

## NavMeshComponents 包（运行时烘焙）

### 安装
1. `Window > Package Manager`
2. 使用 Git URL 添加：`com.unity.ai.navigation`

### 运行时烘焙 NavMesh

```csharp
using Unity.AI.Navigation;

public class NavMeshBuilder : MonoBehaviour {
    public NavMeshSurface surface;

    void Start() {
        // 在运行时烘焙 NavMesh
        surface.BuildNavMesh();
    }

    void UpdateNavMesh() {
        // 地形变化后更新 NavMesh
        surface.UpdateNavMesh(surface.navMeshData);
    }
}
```

---

## 常见模式

### 在巡逻点之间移动

```csharp
public Transform[] waypoints;
private int currentWaypoint = 0;

void Update() {
    if (!agent.pathPending && agent.remainingDistance < 0.5f) {
        // 到达巡逻点，前往下一个
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentWaypoint].position);
    }
}
```

### 追逐玩家

```csharp
public Transform player;
public float chaseRange = 10f;

void Update() {
    float distance = Vector3.Distance(transform.position, player.position);

    if (distance <= chaseRange) {
        agent.SetDestination(player.position);
    } else {
        agent.ResetPath(); // 停止移动
    }
}
```

### 逃离玩家

```csharp
public Transform player;
public float fleeRange = 5f;

void Update() {
    float distance = Vector3.Distance(transform.position, player.position);

    if (distance <= fleeRange) {
        // 远离玩家
        Vector3 fleeDirection = transform.position - player.position;
        Vector3 fleeTarget = transform.position + fleeDirection.normalized * 10f;

        agent.SetDestination(fleeTarget);
    }
}
```

---

## 调试

### NavMesh 可视化
- `Window > AI > Navigation > Bake`
- 勾选 `"Show NavMesh"` 来查看可行走区域

### Agent 路径 Gizmos

```csharp
void OnDrawGizmos() {
    if (agent != null && agent.hasPath) {
        Gizmos.color = Color.green;
        Vector3[] corners = agent.path.corners;

        for (int i = 0; i < corners.Length - 1; i++) {
            Gizmos.DrawLine(corners[i], corners[i + 1]);
        }
    }
}
```

---

## 性能建议

- **限制避障质量**：远处代理使用 `LowQualityObstacleAvoidance`
- **更新频率**：如果目标没移动，不要每帧都调用 `SetDestination()`
- **Area Masks**：限制可走区域可减少寻路搜索空间
- **NavMesh Tiles**：大型世界使用分块 NavMesh（NavMeshComponents 包）

---

## 来源
- https://docs.unity3d.com/6000.0/Documentation/Manual/Navigation.html
- https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/index.html
