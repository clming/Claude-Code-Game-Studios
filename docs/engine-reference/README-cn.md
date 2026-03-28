# 引擎参考文档

本目录包含当前项目所用游戏引擎的精选、版本锁定的文档快照。
之所以需要这些文件，是因为 **LLM 的知识有截止日期**，而游戏引擎更新非常频繁。

## 为什么需要这些内容

Claude 的训练数据有知识截止日期（目前为 2025 年 5 月）。像 Godot、Unity 和 Unreal 这样的游戏引擎会持续发布更新，其中可能包含破坏性 API 变更、新特性以及被弃用的旧模式。如果没有这些参考文件，代理就可能给出已经过时的代码建议。

## 目录结构

每个引擎都有自己的目录：

```text
<engine>/
├── VERSION.md                 # 锁定版本、验证日期、知识缺口时间窗
├── breaking-changes.md        # 不同版本间的 API 变更，按风险等级组织
├── deprecated-apis.md         # “不要用 X -> 改用 Y”的查询表
├── current-best-practices.md  # 模型训练数据中尚未包含的新实践
└── modules/                   # 按子系统拆分的快速参考（每份最多约 150 行）
    ├── rendering.md
    ├── physics.md
    └── ...
```

## 代理如何使用这些文件

引擎专长代理会被要求：

1. 读取 `VERSION.md` 以确认当前引擎版本
2. 在建议任何引擎 API 前先检查 `deprecated-apis.md`
3. 查阅 `breaking-changes.md` 了解版本相关注意事项
4. 针对子系统工作读取相关的 `modules/*.md`

## 维护方式

### 何时更新

- 升级引擎版本之后
- LLM 模型更新之后（知识截止日期变化）
- 运行 `/refresh-docs` 之后（如果可用）
- 当你发现模型对某个 API 的理解有误时

### 如何更新

1. 在 `VERSION.md` 中更新新的引擎版本和日期
2. 在 `breaking-changes.md` 中添加本次版本迁移的新条目
3. 把新近弃用的 API 移入 `deprecated-apis.md`
4. 在 `current-best-practices.md` 中补充新的实践模式
5. 在相关 `modules/*.md` 中更新 API 变化
6. 为所有修改过的文件设置 “Last verified” 日期

### 质量规则

- 每个文件都必须包含 `Last verified: YYYY-MM-DD` 日期
- 模块文件应保持在 150 行以内（节省上下文预算）
- 包含展示正确/错误模式的代码示例
- 链接到官方文档 URL 以便核验
- 只记录那些与模型训练数据不同的内容
