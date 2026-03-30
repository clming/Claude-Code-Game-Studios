# Claude Code Game Studios -- 游戏工作室代理工作规范

通过协作式 Claude Code 代理支持独立游戏开发。
每个代理负责特定领域，以保证分工清晰、质量稳定、交付可追踪。

## 技术栈

- 引擎: Unity 6.3 LTS
- 语言: C#
- 版本控制: Git with trunk-based development
- 构建系统: Unity Build Pipeline
- 资源流水线: Unity Asset Import Pipeline + Addressables

## 项目结构

@.claude/docs/directory-structure.md

## 引擎版本参考

@docs/engine-reference/unity/VERSION.md

## 技术偏好

@.claude/docs/technical-preferences.md

## 协调规则

@.claude/docs/coordination-rules.md

## 协作协议

**以用户驱动协作为主，而不是默认无限自主扩写。**

- 涉及写入前，应先明确将修改落到哪些文件
- 多文件改动应提供清晰摘要
- 未经用户要求，不提交 commit

完整原则见:
`docs/COLLABORATIVE-DESIGN-PRINCIPLE.md`

## UI 工作流优先级

### 硬规则

在游戏项目中，`UI 美术设计` 必须排在 `UI 代码实现` 之前。

允许顺序：

1. 项目代码地基与核心框架稳定
2. UI 美术设计启动并完成第一轮视觉方向
3. UI 组件设计稿、资源规范、动效方向确定
4. 再进入 UI 代码实现与资源接入

禁止默认流程：

- 不得在缺少 UI 美术方向时，长期用“程序化占位 UI”替代正式 UI 设计
- 不得把临时文本、纯色块、调试式布局持续扩写成事实标准
- 不得在没有视觉基准的情况下，大量推进 HUD、Board、Results 的最终界面代码

### 程序化占位 UI 的使用边界

程序化占位 UI 只允许用于：

- 验证信息层级
- 验证交互时序
- 验证会话流与表现流是否打通

一旦满足上述验证，必须切换到：

- UI 美术设计
- 视觉方向确认
- 资源规范定义
- 再做 UI 代码与资源集成

### AI 默认执行策略

当项目进入 UI 阶段时，代理默认按以下顺序工作：

1. 输出 UI 美术工作流
2. 输出 UI 视觉方向文档
3. 明确参考来源、工具链、产出物
4. 再进入 Figma/设计稿/资源规范阶段
5. 最后进入 Unity UI 实现

如果用户没有明确要求继续写 UI 代码，代理不得跳过 UI 美术设计阶段。

## UI 美术设计入口文档

@design/ui/ui-art-design-workflow.md

@design/ui/matchjoy-ui-art-direction-v1.md

## 编码规范

@.claude/docs/coding-standards.md

## 上下文管理

@.claude/docs/context-management.md
