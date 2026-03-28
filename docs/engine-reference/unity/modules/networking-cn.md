# Unity 6.3 - 网络模块参考

**最后核验：** 2026-02-13  
**知识缺口：** Unity 6 使用 Netcode for GameObjects（UNet 已弃用）

---

## 概览

Unity 6 的联网方案：
- **Netcode for GameObjects**（推荐）：Unity 官方多人框架
- **Mirror**：社区维护（UNet 的继任者）
- **Photon**：第三方服务（PUN2）
- **Custom**：底层套接字方案

**UNet（旧版）**：已弃用，不要再使用。

---

## Netcode for GameObjects

### 安装
1. `Window > Package Manager`
2. 搜索 `"Netcode for GameObjects"`
3. 安装 `com.unity.netcode.gameobjects`

---

## 基础设置

### NetworkManager

```csharp
// 添加到场景：GameObject > Add Component > NetworkManager

// 或创建自定义 NetworkManager：
using Unity.Netcode;

public class CustomNetworkManager : MonoBehaviour {
    void Start() {
        NetworkManager.Singleton.StartHost(); // 服务器 + 客户端
        // 或
        NetworkManager.Singleton.StartServer(); // 专用服务器
        // 或
        NetworkManager.Singleton.StartClient(); // 仅客户端
    }
}
```

---

## NetworkObject（联网 GameObject）

### 将 GameObject 标记为联网对象

1. 给 GameObject 添加 `NetworkObject` 组件
2. 它必须位于 prefab 根节点上（不能嵌套）
3. 在 `NetworkManager > NetworkPrefabs List` 中注册该 prefab

### 生成网络对象

```csharp
using Unity.Netcode;

public class GameManager : NetworkBehaviour {
    public GameObject playerPrefab;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId) {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
}
```

---

## NetworkBehaviour（联网脚本）

### NetworkBehaviour 基类

```csharp
using Unity.Netcode;

public class Player : NetworkBehaviour {
    public override void OnNetworkSpawn() {
        if (IsOwner) {
            GetComponent<Camera>().enabled = true;
        }
    }

    void Update() {
        if (!IsOwner) return;

        if (Input.GetKey(KeyCode.W)) {
            MoveServerRpc(Vector3.forward);
        }
    }

    [ServerRpc]
    void MoveServerRpc(Vector3 direction) {
        transform.position += direction * Time.deltaTime;
    }
}
```

---

## NetworkVariable（同步状态）

### NetworkVariable<T>

```csharp
using Unity.Netcode;

public class Player : NetworkBehaviour {
    private NetworkVariable<int> health = new NetworkVariable<int>(100);

    public override void OnNetworkSpawn() {
        health.OnValueChanged += OnHealthChanged;
    }

    void OnHealthChanged(int oldValue, int newValue) {
        Debug.Log($"Health changed: {oldValue} -> {newValue}");
        UpdateHealthUI(newValue);
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int damage) {
        health.Value -= damage;
    }
}
```

### NetworkVariable 权限

```csharp
private NetworkVariable<int> score = new NetworkVariable<int>();

private NetworkVariable<int> ammo = new NetworkVariable<int>(
    default,
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Owner
);
```

---

## RPC（远程过程调用）

### ServerRpc（客户端 -> 服务器）

```csharp
[ServerRpc]
void FireWeaponServerRpc() {
    Debug.Log("Server: Weapon fired");
}

if (IsOwner && Input.GetKeyDown(KeyCode.Space)) {
    FireWeaponServerRpc();
}
```

### ClientRpc（服务器 -> 所有客户端）

```csharp
[ClientRpc]
void PlayExplosionClientRpc(Vector3 position) {
    Instantiate(explosionPrefab, position, Quaternion.identity);
}

[ServerRpc]
void ExplodeServerRpc(Vector3 position) {
    DealDamageToNearbyPlayers(position);
    PlayExplosionClientRpc(position);
}
```

### RPC 参数

```csharp
// 正确：支持基础类型、结构体、字符串、数组
[ServerRpc]
void SetNameServerRpc(string playerName) { }

[ClientRpc]
void UpdateScoresClientRpc(int[] scores) { }

// 错误：不支持 MonoBehaviour、GameObject（改用 NetworkObjectReference）
```

---

## 网络所有权

### 检查所有权

```csharp
if (IsOwner) { }
if (IsServer) { }
if (IsClient) { }
if (IsLocalPlayer) { }
```

### 转移所有权

```csharp
NetworkObject netObj = GetComponent<NetworkObject>();
netObj.ChangeOwnership(newOwnerClientId);
```

---

## NetworkObjectReference（在 RPC 中传递 GameObject）

```csharp
using Unity.Netcode;

[ServerRpc]
void AttackTargetServerRpc(NetworkObjectReference targetRef) {
    if (targetRef.TryGet(out NetworkObject target)) {
        target.GetComponent<Health>().TakeDamage(10);
    }
}

NetworkObject targetNetObj = target.GetComponent<NetworkObject>();
AttackTargetServerRpc(targetNetObj);
```

---

## 客户端-服务器架构

### 服务器权威模式（推荐）

```csharp
public class Player : NetworkBehaviour {
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();

    void Update() {
        if (IsOwner) {
            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            MoveServerRpc(input);
        }

        transform.position = position.Value;
    }

    [ServerRpc]
    void MoveServerRpc(Vector3 input) {
        position.Value += input * Time.deltaTime * moveSpeed;
    }
}
```

---

## 网络传输

### Unity Transport（默认）

```csharp
// 在 NetworkManager 中配置：
// - Transport: Unity Transport
// - Address: 127.0.0.1（本机）或服务器 IP
// - Port: 7777（默认）
```

### 连接事件

```csharp
void Start() {
    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
}

void OnClientConnected(ulong clientId) {
    Debug.Log($"Client {clientId} connected");
}

void OnClientDisconnected(ulong clientId) {
    Debug.Log($"Client {clientId} disconnected");
}
```

---

## 性能建议

### 减少网络流量
- 对变化不频繁的状态使用 `NetworkVariable`
- 在同步前合并多次变更
- 大型数据使用增量压缩

### 预测与校正
- 本地先执行移动以提升响应速度
- 再和服务器权威状态做校正
- 使用插值让移动更平滑

---

## 调试

### Network Profiler
- `Window > Analysis > Network Profiler`
- 监控带宽、RPC 调用、变量更新

### Network Simulator（测试延迟/丢包）
- `NetworkManager > Network Simulator`
- 添加人工延迟和丢包进行测试

---

## 来源
- https://docs-multiplayer.unity3d.com/netcode/current/about/
- https://docs-multiplayer.unity3d.com/netcode/current/learn/bossroom/
