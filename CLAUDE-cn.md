# Claude Code Game Studios -- 游戏工作室代理架构

通过 48 个协同工作的 Claude Code 子代理来管理独立游戏开发。
每个代理负责一个特定领域，以确保关注点分离和质量控制。

## 技术栈

- **引擎**: [选择：Godot 4 / Unity / Unreal Engine 5]
- **语言**: [选择：GDScript / C# / C++ / Blueprint]
- **版本控制**: 使用主干开发（trunk-based development）的 Git
- **构建系统**: [在选择引擎后指定]
- **资源流水线**: [在选择引擎后指定]

> **注意**：针对 Godot、Unity 和 Unreal 都提供了引擎专家代理及其专门的子专家。请使用与你所选引擎相匹配的那一组。

## 项目结构

@.claude/docs/directory-structure.md

## 引擎版本参考

@docs/engine-reference/godot/VERSION.md

## 技术偏好

@.claude/docs/technical-preferences.md

## 协调规则

@.claude/docs/coordination-rules.md

## 协作协议

**以用户驱动的协作为主，而不是自主执行。**
每项任务都遵循：**提问 -> 选项 -> 决策 -> 草案 -> 批准**

- 代理在使用 Write/Edit 工具之前，必须先询问“我可以将其写入 [filepath] 吗？”
- 代理在请求批准之前，必须先展示草案或摘要
- 涉及多个文件的修改，必须对完整变更集获得明确批准
- 未经用户指示，不得提交 commit

完整协议和示例请见 `docs/COLLABORATIVE-DESIGN-PRINCIPLE.md`。

> **第一次使用？** 如果项目尚未配置引擎，也还没有游戏概念，请运行 `/start` 开始引导式入门流程。

## 编码规范

@.claude/docs/coding-standards.md

## 上下文管理

@.claude/docs/context-management.md
