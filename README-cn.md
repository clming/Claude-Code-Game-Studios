<p align="center">
  <h1 align="center">Claude Code Game Studios</h1>
  <p align="center">
    把一次单独的 Claude Code 会话，变成一个完整的游戏开发工作室。
    <br />
    48 个代理。37 套工作流。一个协同运作的 AI 团队。
  </p>
</p>

<p align="center">
  <a href="LICENSE"><img src="https://img.shields.io/badge/license-MIT-blue.svg" alt="MIT License"></a>
  <a href=".claude/agents"><img src="https://img.shields.io/badge/agents-48-blueviolet" alt="48 Agents"></a>
  <a href=".claude/skills"><img src="https://img.shields.io/badge/skills-37-green" alt="37 Skills"></a>
  <a href=".claude/hooks"><img src="https://img.shields.io/badge/hooks-8-orange" alt="8 Hooks"></a>
  <a href=".claude/rules"><img src="https://img.shields.io/badge/rules-11-red" alt="11 Rules"></a>
  <a href="https://docs.anthropic.com/en/docs/claude-code"><img src="https://img.shields.io/badge/built%20for-Claude%20Code-f5f5f5?logo=anthropic" alt="Built for Claude Code"></a>
  <a href="https://ko-fi.com/donchitos"><img src="https://img.shields.io/badge/Ko--fi-Support%20this%20project-ff5e5b?logo=ko-fi&logoColor=white" alt="Ko-fi"></a>
</p>

---

## 为什么会有这个项目

独自使用 AI 做游戏开发很强大，但单纯的一次聊天会话本身没有结构。没有人会阻止你把魔法数字硬编码进代码、跳过设计文档、或者把项目写成一团乱麻。也没有 QA 审查、没有设计评审、没有人追问：“这真的符合游戏的整体愿景吗？”

**Claude Code Game Studios** 的目标，就是给你的 AI 会话加上一套真正工作室级别的结构。你不再只有一个通用助手，而是得到 48 个专门代理，按真实工作室的层级组织起来：有把关愿景的总负责人，有负责各自领域的部门负责人，也有负责具体执行的专家。每个代理都有明确职责、升级路径和质量门槛。

结果就是：决策权依然在你手里，但现在你有了一个会主动问对问题、尽早发现错误，并且能把项目从第一次头脑风暴一直组织到正式上线的团队。

---

## 目录

- [包含了什么](#包含了什么)
- [工作室层级](#工作室层级)
- [斜杠命令](#斜杠命令)
- [快速开始](#快速开始)
- [升级](#升级)
- [项目结构](#项目结构)
- [它如何运作](#它如何运作)
- [设计理念](#设计理念)
- [可定制性](#可定制性)
- [平台支持](#平台支持)
- [社区](#社区)
- [许可证](#许可证)

---

## 包含了什么

| 类别 | 数量 | 说明 |
|------|------|------|
| **Agents** | 48 | 覆盖设计、程序、美术、音频、叙事、QA、制作管理等方向的专职子代理 |
| **Skills** | 37 | 用于常见工作流的斜杠命令，如 `/start`、`/sprint-plan`、`/code-review`、`/brainstorm` 等 |
| **Hooks** | 8 | 在提交、推送、资源变动、会话生命周期、代理审计、缺口检测等阶段自动执行的校验机制 |
| **Rules** | 11 | 按路径生效的编码和文档标准，针对 gameplay、engine、AI、UI、network 等不同区域分别约束 |
| **Templates** | 29 | 用于 GDD、ADR、冲刺计划、经济模型、派系设计等内容的文档模板 |

## 工作室层级

代理按照真实工作室的组织方式分成三层：

```text
第 1 层 - 总负责人（Opus）
  creative-director    technical-director    producer

第 2 层 - 部门负责人（Sonnet）
  game-designer        lead-programmer       art-director
  audio-director       narrative-director    qa-lead
  release-manager      localization-lead

第 3 层 - 专项执行者（Sonnet/Haiku）
  gameplay-programmer  engine-programmer     ai-programmer
  network-programmer   tools-programmer      ui-programmer
  systems-designer     level-designer        economy-designer
  technical-artist     sound-designer        writer
  world-builder        ux-designer           prototyper
  performance-analyst  devops-engineer       analytics-engineer
  security-engineer    qa-tester             accessibility-specialist
  live-ops-designer    community-manager
```

### 引擎专项代理

模板内置了三大主流引擎的专用代理组。你应使用与自己项目匹配的那一组：

| 引擎 | 主代理 | 子专项 |
|------|--------|--------|
| **Godot 4** | `godot-specialist` | GDScript、Shaders、GDExtension |
| **Unity** | `unity-specialist` | DOTS/ECS、Shaders/VFX、Addressables、UI Toolkit |
| **Unreal Engine 5** | `unreal-specialist` | GAS、Blueprints、Replication、UMG/CommonUI |

## 斜杠命令

在 Claude Code 中输入 `/`，即可访问全部 37 个技能：

**评审与分析**
`/design-review` `/code-review` `/balance-check` `/asset-audit` `/scope-check` `/perf-profile` `/tech-debt`

**制作管理**
`/sprint-plan` `/milestone-review` `/estimate` `/retrospective` `/bug-report`

**项目管理**
`/start` `/project-stage-detect` `/reverse-document` `/gate-check` `/map-systems` `/design-system`

**发布**
`/release-checklist` `/launch-checklist` `/changelog` `/patch-notes` `/hotfix`

**创意**
`/brainstorm` `/playtest-report` `/prototype` `/onboard` `/localize`

**团队协同**（在一个功能上协调多个代理）
`/team-combat` `/team-narrative` `/team-ui` `/team-release` `/team-polish` `/team-audio` `/team-level`

## 快速开始

### 前置条件

- [Git](https://git-scm.com/)
- [Claude Code](https://docs.anthropic.com/en/docs/claude-code)（`npm install -g @anthropic-ai/claude-code`）
- **推荐**：安装 [jq](https://jqlang.github.io/jq/)（用于 hook 校验）以及 Python 3（用于 JSON 校验）

如果缺少这些可选工具，所有 hook 都会优雅降级，不会把系统弄坏，只是少了一些自动校验能力。

### 安装与启动

1. **克隆仓库或将其作为模板使用**：
   ```bash
   git clone https://github.com/Donchitos/Claude-Code-Game-Studios.git my-game
   cd my-game
   ```

2. **打开 Claude Code** 并开始一个会话：
   ```bash
   claude
   ```

3. **运行 `/start`**
   系统会先问你当前处于什么阶段：完全没想法、只有模糊概念、已有清晰设计，还是已经有现成项目。然后它会把你带到合适的下一条工作流，不做主观假设。

   如果你已经明确知道自己要什么，也可以直接跳到具体技能，例如：
   - `/brainstorm`：从零探索游戏创意
   - `/setup-engine godot 4.6`：如果你已经知道引擎，直接完成配置
   - `/project-stage-detect`：分析一个已有项目的状态

## 升级

如果你已经在使用这个模板的旧版本，请查看 [UPGRADING.md](UPGRADING.md)。

这个文件会告诉你：

- 如何一步步迁移到新版本
- 各版本之间变了什么
- 哪些文件可以直接覆盖
- 哪些文件必须手动合并

## 项目结构

```text
CLAUDE.md                           # 主配置文件
.claude/
  settings.json                     # Hooks、权限和安全规则
  agents/                           # 48 个代理定义（Markdown + YAML frontmatter）
  skills/                           # 37 个斜杠命令（每个技能一个子目录）
  hooks/                            # 8 个 hook 脚本（bash，跨平台）
  rules/                            # 11 条按路径生效的编码标准
  docs/
    quick-start.md                  # 详细使用指南
    agent-roster.md                 # 完整代理列表与职责范围
    agent-coordination-map.md       # 委派与升级路径图
    setup-requirements.md           # 前置环境和平台说明
    templates/                      # 28 个文档模板
src/                                # 游戏源码
assets/                             # 美术、音频、VFX、Shader、数据文件
design/                             # GDD、叙事文档、关卡设计
docs/                               # 技术文档和 ADR
tests/                              # 测试套件
tools/                              # 构建工具和流水线工具
prototypes/                         # 一次性原型（与 src 隔离）
production/                         # 冲刺计划、里程碑、发布追踪
```

## 它如何运作

### 代理协作方式

代理遵循结构化的委派模型：

1. **纵向委派**
总负责人把任务分给部门负责人，部门负责人再分给执行专家。

2. **横向咨询**
同一层级的代理可以彼此咨询，但不能替其他领域做有约束力的决定。

3. **冲突解决**
如果出现分歧，就向共同上级升级：设计问题交给 `creative-director`，技术问题交给 `technical-director`。

4. **变更传播**
跨部门变更由 `producer` 负责协调。

5. **领域边界**
代理不会越界修改不属于自己领域的文件，除非获得明确委派。

### 协作，而不是自动驾驶

这**不是**一个自动驾驶系统。每个代理都必须遵守严格的协作协议：

1. **先提问**
代理会在提出方案之前先问清楚问题。

2. **给选项**
代理会给出 2 到 4 个带有优缺点的方案。

3. **由你决定**
最终拍板的人始终是用户。

4. **先看草稿**
代理会在真正定稿前展示过程和草稿。

5. **得到批准**
没有你的同意，任何内容都不会被写入文件。

你始终掌握控制权。代理提供的是结构和专业支持，而不是取代你做决定。

### 自动安全机制

**Hooks** 会在会话生命周期中自动运行：

| Hook | 触发时机 | 作用 |
|------|----------|------|
| `validate-commit.sh` | `git commit` | 检查硬编码值、TODO 格式、JSON 有效性、设计文档章节完整性 |
| `validate-push.sh` | `git push` | 在推送到受保护分支时给出警告 |
| `validate-assets.sh` | 写入 `assets/` 时 | 校验命名规范和 JSON 结构 |
| `session-start.sh` | 会话开始 | 加载当前冲刺上下文和最近 Git 活动 |
| `detect-gaps.sh` | 会话开始 | 检测是否是新项目，并在存在代码或原型却缺少文档时提示缺口 |
| `pre-compact.sh` | 上下文压缩前 | 保留会话进度笔记 |
| `session-stop.sh` | 会话结束 | 记录本次完成的事项 |
| `log-agent.sh` | 代理被创建时 | 记录所有子代理调用的审计轨迹 |

`settings.json` 中的权限规则会自动放行安全操作，如 `git status`、测试运行等，也会阻止危险操作，如强制推送、`rm -rf`、读取 `.env` 文件等。

### 按路径生效的规则

编码和文档标准会根据文件位置自动启用：

| 路径 | 约束内容 |
|------|----------|
| `src/gameplay/**` | 强制数据驱动、要求使用 delta time、禁止直接引用 UI |
| `src/core/**` | 热路径零分配、线程安全、API 稳定性 |
| `src/ai/**` | 性能预算、可调试性、参数数据驱动 |
| `src/networking/**` | 服务端权威、消息版本化、安全要求 |
| `src/ui/**` | 不允许持有游戏状态所有权、必须支持本地化和无障碍 |
| `design/gdd/**` | 必须具备 8 个章节、公式格式标准、边界情况说明 |
| `tests/**` | 测试命名、覆盖率要求、fixture 模式 |
| `prototypes/**` | 标准更宽松，但必须有 README 和原型假设说明 |

## 设计理念

这个模板建立在专业游戏开发实践基础上：

- **MDA Framework**
用于游戏设计分析的 Mechanics、Dynamics、Aesthetics 框架

- **Self-Determination Theory**
用于分析玩家动机的自主性、能力感、关联感

- **Flow State Design**
用挑战与技能平衡来提升沉浸和参与感

- **Bartle Player Types**
用于定位目标玩家和验证受众

- **Verification-Driven Development**
先验证、先测试，再进入实现

## 可定制性

这是一套**模板**，不是死板锁定的框架。所有部分都应该根据你的项目进行调整：

- **增删代理**
删除你不需要的代理文件，也可以新增适合你项目的角色

- **修改代理提示词**
调整代理行为，加入你的项目知识

- **修改技能**
让工作流匹配你的团队方式

- **增加规则**
为你的目录结构补充新的按路径规则

- **调整 hooks**
改变校验严格程度，或新增更多检查

- **选择引擎**
可以使用 Godot、Unity、Unreal 对应的代理组，也可以一个都不用

## 平台支持

该模板已在 **Windows 10 + Git Bash** 上测试。所有 hook 都使用 POSIX 兼容写法，例如使用 `grep -E` 而不是 `grep -P`，并对缺失工具提供回退方案。它也可以在 macOS 和 Linux 上直接使用，无需修改。

## 社区

- **Discussions**
[GitHub Discussions](https://github.com/Donchitos/Claude-Code-Game-Studios/discussions)
用于提问、交流想法，以及展示你做出的内容

- **Issues**
[Bug reports and feature requests](https://github.com/Donchitos/Claude-Code-Game-Studios/issues)
用于提交 Bug 和功能需求

---

*这个项目仍在持续开发中。代理架构、技能体系和协作系统现在已经足够稳定并且可用，但后面还会继续扩展。*

## 许可证

MIT License。详情见 [LICENSE](LICENSE)。
