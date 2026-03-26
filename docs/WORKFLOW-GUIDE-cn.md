# Claude Code Game Studios -- 完整工作流指南

> **如何借助 Agent 架构，从零开始一路做到游戏正式发布。**
>
> 本指南会带你走完使用 48 个代理、37 个斜杠命令以及自动化 hooks 的整个游戏开发流程。它默认你已经安装好 Claude Code，并且正位于项目根目录中工作。

---

## 目录

1. [阶段 0：安装与配置](#阶段-0安装与配置)
2. [阶段 1：创意构思与概念定义](#阶段-1创意构思与概念定义)
3. [阶段 2：前期制作与设计](#阶段-2前期制作与设计)
4. [阶段 3：原型制作与验证](#阶段-3原型制作与验证)
5. [阶段 4：生产冲刺工作流](#阶段-4生产冲刺工作流)
6. [阶段 5：实现阶段深度说明](#阶段-5实现阶段深度说明)
7. [阶段 6：测试与质量保证](#阶段-6测试与质量保证)
8. [阶段 7：打磨与优化](#阶段-7打磨与优化)
9. [阶段 8：本地化与无障碍](#阶段-8本地化与无障碍)
10. [阶段 9：发布与上线](#阶段-9发布与上线)
11. [阶段 10：上线后运营与 Live Ops](#阶段-10上线后运营与-live-ops)
12. [附录 A：代理速查表](#附录-a代理速查表)
13. [附录 B：斜杠命令速查表](#附录-b斜杠命令速查表)
14. [附录 C：常见工作流](#附录-c常见工作流)

---

## 阶段 0：安装与配置

### 你需要准备什么

开始之前，请确认你已经具备：

- **Claude Code** 已安装并可正常运行
- **Git**，Windows 上建议配合 Git Bash，Mac/Linux 用标准终端即可
- **jq**（可选但推荐，缺失时 hooks 会退回使用 `grep`）
- **Python 3**（可选，部分 hooks 会用它做 JSON 校验）

### 步骤 0.1：克隆并配置项目

克隆仓库，并在编辑器中打开：

```bash
git clone <repo-url> my-game
cd my-game
```

### 步骤 0.2：运行 `/start`（推荐新用户使用）

如果你刚接触这个项目，或者还不知道自己要做什么游戏：

```text
/start
```

这个引导式入口会询问你当前所处的阶段（完全没想法、只有模糊想法、已有明确概念、已经有现成工作），然后把你导向合适的开发阶段。如果你已经明确了游戏概念和引擎选择，可以跳过这一步。

### 步骤 0.3：选择你的引擎

在 Claude Code 中运行 `/setup-engine`。这是最重要的一项配置，因为它会告诉整个系统你正在使用哪种引擎、语言和工具链：

```bash
/setup-engine godot 4.6
```

你也可以不带参数直接运行 `/setup-engine`，系统会根据你的游戏需求（2D/3D、目标平台、团队规模、语言偏好）给出交互式推荐。

**`/setup-engine` 会做什么：**

- 在 `CLAUDE.md` 中固定引擎与版本
- 填充 `.claude/docs/technical-preferences.md`，写入命名约定、性能预算以及引擎专属默认值
- 识别知识空缺（例如引擎版本比模型训练数据更新），并从网络拉取最新文档，确保代理建议的是正确 API
- 在 `docs/engine-reference/` 中生成带版本锁定的参考文档

**这为什么重要：** 一旦引擎被设定，系统就知道优先调用哪些引擎专家代理。如果你选的是 Godot，那么 `godot-specialist`、`godot-gdscript-specialist`、`godot-shader-specialist` 就会成为首选专家。Unity 和 Unreal 的专家仍然可用，但不再是主线路径。

> **手动替代方式：** 如果你更喜欢自己改配置，也可以直接编辑 `CLAUDE.md` 里的 Technology Stack 部分。

### 步骤 0.3：确认 Hooks 正常工作

启动一个新的 Claude Code 会话。你应该能看到 `session-start.sh` hook 输出类似内容：

```text
=== Claude Code Game Studios -- Session Context ===
Branch: main
Recent commits:
  abc1234 Initial commit
===================================
```

如果你看到了，说明 hooks 正在正常运行。如果没看到，请检查 `.claude/settings.json`，确认 hook 路径和你的操作系统匹配。

### 步骤 0.4：创建目录结构

`CLAUDE.md` 中列出的目录起初未必都存在。按需创建即可，但系统默认你最终会遵循如下结构：

```text
src/                  # 游戏源代码
  core/               # 引擎/框架层代码
  gameplay/           # 游戏玩法系统
  ai/                 # AI 系统
  networking/         # 多人联机代码
  ui/                 # UI 代码
  tools/              # 开发工具
assets/               # 游戏资源
  art/                # 精灵、模型、贴图
  audio/              # 音乐、音效
  vfx/                # 粒子特效
  shaders/            # Shader 文件
  data/               # JSON 配置/平衡数据
design/               # 设计文档
  gdd/                # 游戏设计文档
  narrative/          # 剧情、设定、对白
  levels/             # 关卡设计文档
  balance/            # 平衡表格和数据
docs/                 # 技术文档
  architecture/       # 架构决策记录
  api/                # API 文档
  postmortems/        # 复盘文档
tests/                # 测试集
prototypes/           # 可抛弃原型
production/           # Sprint 计划、里程碑、发布内容
  sprints/
  milestones/
  releases/
```

> **提示：** 第一天不需要把这些目录全部建完。什么时候进入对应阶段，什么时候再创建即可。关键是创建时要遵循这个结构，因为**规则系统**会根据文件路径自动套用标准。比如 `src/gameplay/` 下的代码会触发 gameplay 规则，`src/ai/` 下的代码会触发 AI 规则，以此类推。

---

## 阶段 1：创意构思与概念定义

### 这个阶段会发生什么

你会从“完全没想法”或“只有一点模糊想法”，推进到一份结构化的游戏概念文档。这里要搞清楚的是：**你到底要做什么游戏**。

> **提示：** 如果你在阶段 0 中运行了 `/start`，并选择了 Path A 或 B，那么你其实已经进入这个阶段了。`/start` 会自动把你导向 `/brainstorm`。

### 步骤 1.1：使用 `/brainstorm` 进行头脑风暴

如果你跳过了 `/start`，这里就是起点。运行 brainstorm 技能：

```text
/brainstorm
```

或者给一个类型提示：

```text
/brainstorm roguelike deckbuilder
```

**会发生什么：** brainstorm 技能会使用专业工作室常见的方法，引导你完成一个协作式的 6 阶段创意流程：

Agent: "我们来探索游戏概念。你对什么类型或主题感兴趣？（可选，不填也可以做开放式探索）"

User: "Roguelike deckbuilder"

Agent: "我生成了 10 个把 roguelike 和 deckbuilder 结合起来的概念种子。这里是前 3 个：

       **概念 A：Spell Compiler** - 代码主题的卡组构筑，卡牌是函数
       **概念 B：Memory Palace** - 心理惊悚主题，卡牌是记忆
       **概念 C：Ecosystem Builder** - 自然生态主题，卡牌是物种

       你希望我对哪个概念继续做 MDA 分析？"

User: "Concept B sounds unique"

Agent: [对概念 B 执行 MDA 分析、玩家动机映射、目标受众分析、Verb-First Design，并在每一步展示思路与征求反馈]

       "完整分析已经完成。要不要我把它整理成一份结构化概念文档？"

User: "Yes"

Agent: [创建结构化的概念文档，包含最佳创意、目标受众、核心机制和情绪目标]

### 步骤 1.2：把最终概念文档化

将 brainstorm 的输出整理成正式文档。使用**游戏概念模板**：

```text
Ask Claude to create a game concept document using the template at
.claude/docs/templates/game-concept.md
```

该模板通常包含：

- 一句话电梯陈述（Elevator Pitch）
- 核心幻想（玩家会想象自己在做什么）
- MDA 拆解
- 目标受众（Bartle 类型、人口画像）
- 核心循环图
- 独特卖点
- 对标作品与差异化说明

### 步骤 1.3：定义你的游戏支柱

继续往下之前，请先锁定你的**游戏支柱（Game Pillars）**，也就是 3 到 5 条不可妥协的设计价值观，它们会指导几乎所有设计决策。使用这个模板：

```text
Ask Claude to create a game pillars document using the template at
.claude/docs/templates/game-pillars.md
```

示例支柱：

- **有意义的选择**：每个决策都会永久改变某些东西
- **涌现式叙事**：故事由系统自然生成，而不是纯脚本推动
- **易上手但有深度**：入门简单，精通困难

同时也建议定义**反支柱（Anti-Pillars）**，也就是你的游戏有意不追求的方向：

- 我们**不**追求：照片级写实、竞技性平衡、线性叙事

> **当你对概念与支柱都满意后，就可以进入前期制作阶段。**

---

## 阶段 2：前期制作与设计

### 这个阶段会发生什么

你要产出所有用于定义游戏如何运作的设计文档。这个阶段**还不写代码**，重点是纯设计和技术架构。

### 步骤 2.1：将概念拆分成系统

在编写各个 GDD 之前，先罗列你的游戏需要哪些系统：

```text
/map-systems
```

这会创建 `design/gdd/systems-index.md`，它是一份总控文档，用于：

- 列出游戏所需的全部系统（战斗、移动、UI 等）
- 标明系统之间的依赖关系
- 分配优先级层级（MVP、Vertical Slice、Alpha、Full Vision）
- 决定设计顺序（基础 -> 核心 -> 功能 -> 表现 -> 打磨）

然后按依赖顺序逐个设计系统：

```text
/map-systems next
```

这会选出当前最高优先级、但尚未设计的系统，并把流程交给 `/design-system`。它会逐节引导你编写该系统的 GDD。每完成一份 GDD，都会先经过 `/design-review`，再进入下一份。

你也可以直接指定某个系统来写：

```text
/design-system combat-system
```

### 步骤 2.2：创建游戏设计文档（GDD）

为游戏中的每个主要系统，在 `design/gdd/` 下创建设计文档。使用这个模板：

```text
Ask Claude to create a GDD using the template at
.claude/docs/templates/game-design-document.md
```

**`design-docs` 规则**（`.claude/rules/design-docs.md`）要求每份 GDD 必须包含以下 8 个部分：

| # | 章节 | 内容说明 |
|---|---|---|
| 1 | **概览** | 用一段话总结这个系统 |
| 2 | **玩家幻想** | 玩家在使用这个系统时，会想象/感受到什么 |
| 3 | **详细规则** | 规则必须无歧义，两个设计师看完都应做出同样结果 |
| 4 | **公式** | 所有计算公式、变量定义与取值范围 |
| 5 | **边界情况** | 异常或极端情况下如何处理，必须明确写清 |
| 6 | **依赖项** | 该系统和哪些系统有关联，且要双向说明 |
| 7 | **可调参数** | 设计师可安全修改哪些值，以及推荐安全区间 |
| 8 | **验收标准** | 如何验证它可用，必须具体且可衡量 |

**你可能会创建的 GDD 示例：**

- `design/gdd/core-loop.md`：核心玩法循环
- `design/gdd/combat-system.md`：战斗系统
- `design/gdd/progression-system.md`：成长与推进系统
- `design/gdd/inventory-system.md`：背包与物品系统
- `design/gdd/ability-system.md`：技能/能力系统

### 步骤 2.2：复杂机制请使用 Game Designer 代理

对于需要较深机制思考的系统，建议委托给 `game-designer`：

```text
Ask the game-designer agent to design a crafting system that supports
emergent recipes (players discover combinations) with a skill progression
tied to experimentation. The system should feel rewarding without being
grindy. Target audience is Explorers and Achievers.
```

**会发生什么：** `game-designer` 会和你一起把设计拆清楚：

Agent: "我来帮你设计这个 crafting system。先问几个问题：
       - 配方是通过实验自然发现，还是明确教学？
       - 制作失败应该多大惩罚？
       - 目标玩家更偏 Explorers 还是 Achievers？"

User: "通过实验发现。失败中等惩罚，消耗材料但给部分技能经验。主要面向 Explorers。"

Agent: "基于这些前提，我给出三种机制方向：

       **方案 A：纯发现式** - 没有提示，完全自由
       优点：探索奖励感最强
       缺点：挫败风险高，可能逼玩家查 wiki

       **方案 B：提示系统** - 材料兼容时给视觉提示
       优点：探索有引导，挫败感更少
       缺点：顿悟感会弱一些

       **方案 C：混合式** - 自由实验 + 技能解锁后的可选日志提示
       优点：兼容两类玩家，支持渐进式引导
       缺点：实现更复杂

       我推荐方案 C，因为它既尊重 Explorer，也为玩家提供安全网，而且日志提示还能和技能成长挂钩，照顾 Achiever。
       哪个方案更符合你的愿景？"

User: "Option C sounds perfect"

Agent: [逐节起草 GDD：概览、玩家幻想、详细规则、公式、边界情况，并不断征求反馈]

       "成功率和技能成长曲线需要更细的公式设计，我去请 systems-designer 一起做数学部分。"

Agent: [咨询后] "完整草稿已准备好，公式也经过 systems-designer 验证。
       economy-designer 提醒材料成本需要平衡，我已经把建议合并进去了。

       我可以把它写入 `design/gdd/crafting-system.md` 吗？"

User: "Yes"

Agent: [写入文件]

### 步骤 2.3：设计评审

任何设计文档交给程序实现之前，都要跑一次评审：

```text
/design-review design/gdd/crafting-system.md
```

**会发生什么：** 这个技能会按 8 个章节标准检查文档：

- 章节是否齐全且完整
- 公式是否清晰且变量有定义
- 边界情况是否明确写死，而不是写成 “TBD”
- 依赖是否双向列出
- 验收标准是否可测试

把问题修完再继续。

### 步骤 2.4：创建叙事设计（如果适用）

如果你的游戏有剧情、世界观或对话，这就是搭建它们的阶段：

1. **世界构建**：用 `world-builder` 定义阵营、历史、地理和世界规则
2. **故事结构**：用 `narrative-director` 设计剧情弧、角色弧和叙事节点
3. **角色档案**：每个主要角色使用 `narrative-character-sheet.md` 模板

`narrative` 规则（`.claude/rules/narrative.md`）会强制保证：

- 各文档设定一致
- 正典分级（Core Canon、Flavor、Apocrypha）
- 角色语气档案
- 世界规则文档化

### 步骤 2.5：技术架构

编码开始之前，要先做关键技术决策：

```text
/architecture-decision "State Machine vs Behavior Tree for NPC AI"
```

**会发生什么：** 这个技能会引导你产出一份 ADR（Architecture Decision Record，架构决策记录）：

Agent: "我来帮你记录这个架构决策。先让我理解一下背景。你当前为什么要做这个选择？"

User: "我们要在 NPC AI 中选择状态机还是行为树。预计会有 50+ 种敌人。"

Agent: "考虑到有 50+ 敌人类型，这是我的分析：

       **方案 A：有限状态机**
       优点：简单、易调试、开销较小
       缺点：行为复杂时会迅速失控，逻辑复用困难

       **方案 B：行为树**
       优点：复用性高、扩展性好、可视化调试友好
       缺点：学习曲线更陡，开销略高

       **方案 C：效用型 AI**
       优点：行为更涌现、更动态
       缺点：调试困难，调参要求高

       对于 50+ 种敌人，我建议行为树，可扩展性和复用性带来的收益超过学习成本。
       要不要我帮你创建一份 ADR？"

User: "Yes, go with Behavior Trees"

Agent: [在 `docs/architecture/` 中创建 ADR，记录背景、决策驱动、候选方案、最终选择与后果]

**更复杂的架构问题，请交给 `technical-director`：**

```text
Ask the technical-director agent to evaluate whether we should use an
ECS architecture or a traditional component system for our gameplay
entities, considering our team size (solo) and engine choice (Godot 4).
```

Agent: "我来结合你的上下文分析 ECS 和传统组件系统：

       [围绕 Godot 4 场景系统、单人开发工作流、学习曲线、调试工具和性能需求做详细分析]

       **建议：采用传统的 Godot 节点式组件方案**

       理由：[详细解释]

       要不要我把这份完整分析写成 ADR？"

User: "Yes"

Agent: [创建完整 ADR]

### 步骤 2.6：里程碑规划

使用下列模板定义你的项目里程碑：

```text
Ask Claude to create a milestone definition using the template at
.claude/docs/templates/milestone-definition.md
```

典型独立游戏里程碑：

1. **Vertical Slice**：一个完整可玩的关卡，质量足以代表最终游戏
2. **Alpha**：所有核心系统都能运行，允许临时美术
3. **Beta**：内容完整，全部功能上线，开始集中打磨
4. **Release Candidate**：只修 Bug，准备发售
5. **Gold**：正式出货

> **当所有设计文档都审完、关键架构也确定后，你就可以开始真正构建游戏了。**

---

## 阶段 3：原型制作与验证

### 这个阶段会发生什么

在正式投入完整制作前，你要先通过可抛弃原型验证那些高风险或不确定的机制。这样能节省大量返工时间。

### 步骤 3.1：识别哪些内容需要原型

不是所有东西都要做原型。适合原型验证的情况包括：

- 机制很新，你不确定它是否真的好玩
- 技术方案风险高，你不确定是否可行
- 有两个设计方向都看起来不错，但必须亲手体验差异

### 步骤 3.2：运行 Prototype 技能

```text
/prototype "grappling hook movement with momentum"
```

**会发生什么：** 这个技能会和你一起搭一个最小原型：

Agent: "我们来做抓钩移动原型。先确认这次原型要验证的假设：

       - 是要验证基于动量的抓钩手感是否好？
       - 还是验证底层物理是否技术可行？
       - 还是比较不同控制方案？

       这个原型最核心要回答的问题是什么？"

User: "Testing if momentum transfer feels satisfying and creates skill expression"

Agent: "明白。这里是我建议的原型设置：

       **假设**：带动量传递的抓钩会有令人满足的手感，并通过基于物理的操作带来技巧表达和涌现式移动方式

       **成功标准**：
       - 玩家能流畅串联 3 个以上抓点而不失去动量
       - 动量既足够可预测、便于规划，也足够有表现力、带来惊喜
       - 玩家在 5 分钟内能自然发现一些非预期的移动技巧

       **原型范围**（最多 1 到 2 小时）：
       - 基础抓点附着
       - 动量计算与传递
       - 带多种抓点组合的简易测试关卡
       - 不做美术、UI、音效，只做机制验证

       这和你想验证的内容一致吗？"

User: "Perfect, but add one more criteria: can we swing around corners"

Agent: "已加入‘是否能绕角摆荡’这一成功标准。要不要我创建 `prototypes/grappling-hook/` 并开始实现？"

User: "Yes"

Agent: [创建目录结构、README（记录假设/标准/状态），并把最小实现交给 prototyper 代理]

**关键规则：** `prototype-code` 规则（`.claude/rules/prototype-code.md`）会有意放宽原型阶段的编码标准。允许硬编码、不强制测试，但必须写 README，把假设与结论记清楚。

### 步骤 3.3：评估原型

试玩或测试原型后，回答这些问题：

- 假设成立了吗？它是真的好玩、可行、或更优吗？
- 有什么出乎意料的发现？
- 真正投入生产时需要改什么？

然后把结论写回原型 README，并给出**判定结果**：Ship It、Rework 或 Kill It。

### 步骤 3.4：生成试玩报告

如果你让其他人也试玩了原型：

```text
/playtest-report
```

它会生成一份结构化报告模板，覆盖：

- 测试者画像与测试环境
- 任务完成率
- 困惑点与摩擦点
- 愉悦时刻
- 定量评分
- 优先级排序后的结论

> **当你用原型验证或否定了最高风险假设后，就可以进入正式生产了。**

---

## 阶段 4：生产冲刺工作流

### 这个阶段会发生什么

这是核心生产循环。你会以 Sprint（通常 1 到 2 周）为单位推进功能、修复问题并跟踪进度。这个阶段会不断重复，直到游戏内容完整。

### 步骤 4.1：创建第一个 Sprint

```text
/sprint-plan new
```

**会发生什么：** 这个技能会委托 `producer` 和你一起完成 Sprint 规划：

Agent: "我们来规划这次 Sprint。先说一句话：这个 Sprint 的首要目标是什么？"

User: "Implement core combat system with basic attacks and health"

Agent: "这次 Sprint 你大概有多少可用时间？按小时或天都行。"

User: "20 hours over 2 weeks"

Agent: "基于‘核心战斗系统’，我拆出了这些任务：

       **必须完成（Must Have）**：
       - 伤害计算系统（对应 `design/gdd/combat-system.md` 中的公式）
       - 生命值组件与受伤处理
       - 基础攻击输入和执行
       - 伤害计算的单元测试

       **应该完成（Should Have）**：
       - 命中反馈和 VFX
       - 死亡状态处理
       - 战斗属性数据文件

       **可选（Nice to Have）**：
       - 连击系统
       - 飘字伤害 UI

       **我看到的风险**：
       - 目前还没有战斗设计文档，这是阻塞项
       - VFX 需要 technical-artist 参与

       这样的任务拆分符合你的预期吗？有没有要增删或调整优先级的？"

User: "Move combo system to Should Have - it's important for feel"

Agent: "已更新。要不要我创建 `production/sprints/sprint-01.md`？"

User: "Yes"

Agent: [创建 Sprint 计划，包含目标、任务分类、20 小时容量估算、风险登记和依赖项]

### 步骤 4.2：日常工作循环

每个工作会话通常按这个模式进行：

```text
1. 启动 Claude Code 会话
   （session-start hook 会显示分支、Sprint 状态、最近提交）

2. 从 Sprint 计划中选择一个任务
   （例如：“我现在在做战斗伤害计算系统”）

3. Claude 判断应该调用哪些代理
   - 设计不清时找 game-designer
   - 写实现找 gameplay-programmer
   - 公式细节找 systems-designer

4. 实现该功能（详见阶段 5）

5. 编写测试（验证驱动开发）

6. 提交代码
   （validate-commit hook 会检查是否引用了设计文档、
    JSON 数据是否合法、是否存在硬编码数值）

7. 重复，直到当前上下文接近上限（约 65% 到 70%）

8. 使用 /clear 开新上下文，或 compact 后继续
```

### 步骤 4.3：Sprint 跟踪

随时检查 Sprint 进度：

```text
/sprint-plan status
```

如果范围开始膨胀：

```text
/scope-check production/sprints/sprint-03.md
```

它会对比当前范围和最初计划，并标出：

- Sprint 开始后新增了哪些任务
- 范围增长了多少百分比
- 建议删掉哪些内容

### 步骤 4.4：Sprint 回顾与复盘

Sprint 结束时：

```text
/retrospective
```

它会分析：

- 原计划与实际完成对比
- 速度（完成了多少任务/点数）
- 做得好的地方
- 做得不好的地方
- 遇到的阻塞
- 下个 Sprint 可执行的改进措施

然后再规划下一个 Sprint：

```text
/sprint-plan new
```

新 Sprint 会吸收未完成任务、复盘经验和更新后的优先级。

### 步骤 4.5：里程碑评审

在里程碑节点执行：

```text
/milestone-review "alpha"
```

它会产出一份综合报告，包含：

- 功能完成度百分比
- 质量指标（Bug 数、测试覆盖率）
- 风险评估
- Go / No-Go 建议
- 为达成里程碑还需完成的行动项

---

## 阶段 5：实现阶段深度说明

### 这个阶段会发生什么

真正写代码的阶段到了。系统会通过路径级规则、自动化 hooks 和结构化委派，持续保证代码质量。

### 步骤 5.1：单系统实现

如果你要实现单个玩法系统，例如“实现伤害计算”：

```text
Ask the gameplay-programmer agent to implement the damage calculation
system as specified in design/gdd/combat-system.md. The formulas are in
section 4. Use the project's data-driven approach with config files in
assets/data/.
```

**会发生什么：** `gameplay-programmer` 会先读取设计文档，再和你确认关键细节：

Agent: "我已经读了 `design/gdd/combat-system.md`。开始实现前，我需要澄清几点：

       第 4 节的伤害公式是：
       `base_damage * (1 + attack_stat * 0.1) * type_modifier`

       问题：
       - `attack_stat` 是否要限制最大值？
       - `type_modifiers` 应该挂在武器上，还是挂在伤害类型上？
       - 伤害要怎么取整，向下取整 / 向上 / 四舍五入？

       另外我还没看到生命值组件的设计文档。你希望我先补一个最小版本，还是你想单独设计？"

User: "Clamp attack_stat to 0-100. Type modifiers per damage type. Round down.
      Create minimal health doc - just current/max HP and damage handling."

Agent: "明白。我的实现方案如下：

       **要创建的文件**：
       - `src/gameplay/combat/damage_calculator.gd`（核心公式逻辑）
       - `src/gameplay/combat/health_component.gd`（生命值管理）
       - `assets/data/combat_damage.json`（基础伤害值、类型修正）
       - `design/gdd/health-system.md`（最小设计文档）

       **架构**：
       - `DamageCalculator` 作为无状态工具类，便于测试
       - `HealthComponent` 作为节点组件，使用 signal 广播受伤/死亡
       - 所有数值都来自配置文件（符合 gameplay-code 规则）

       我可以开始实现了吗？"

User: "Yes"

Agent: [开始实现，自动遵守规则：数据驱动数值、与 delta time 解耦、基于 signal 通讯、通过依赖注入保证可测性]

       "实现完成。已创建：
       - `src/gameplay/combat/damage_calculator.gd`
       - `src/gameplay/combat/health_component.gd`
       - `assets/data/combat_damage.json`（JSON 结构已校验）
       - `design/gdd/health-system.md`

       接下来可以测试。要不要我顺手起草单元测试？"

User: "Yes"

Agent: [编写测试]

### 步骤 5.2：多系统功能（团队编排）

如果一个功能横跨多个领域，请使用 team 技能：

```text
/team-combat "healing ability with HoT (heal-over-time) and cleanse"
```

**会发生什么：** team 技能会编排一个 6 阶段协作式开发流程：

**阶段 1：设计**

Agent（game-designer）: "我来设计这个治疗技能。有几个问题：
       - HoT 应该可叠加，还是只刷新持续时间？
       - 净化移除所有减益，还是只移除指定类别？
       - 法力消耗是否随治疗量缩放？"

User: [回答问题]

Agent: [创建设计文档并展示草稿] "设计已经就绪。要不要进入架构阶段？"

User: "Approved"

**阶段 2：架构**

Agent（gameplay-programmer）: "这是我的代码结构建议：
       - `HealAbility` 继承 `BaseAbility`
       - HoT 做成独立 buff 组件，方便复用
       - Cleanse 做成通用工具函数，方便在别处调用

       这样可以吗？"

User: "Approved"

**阶段 3：并行实现**

Agent（协调者）: "开始并行实现：
       - gameplay-programmer：治疗技能和 HoT buff 代码
       - ai-programmer：NPC 何时使用治疗技能的 AI 逻辑
       - technical-artist：HoT 特效规范
       - sound-designer：治疗施放和 HoT 生效音效规范

       全部完成后我再回来汇报。"

[各专家会分别提问、展示工作并最终收口]

**阶段 4：集成**

Agent（gameplay-programmer）: "所有组件都准备好了，现在开始集成……"
       [展示集成结果] "测试场景中工作正常。可以开始验证了吗？"

**阶段 5：验证**

Agent（qa-tester）: "正在根据设计文档中的验收标准测试……"
       [汇报结果] "发现 2 个边界问题：[具体说明]。要修还是先延后？"

User: "Fix the critical one, defer the other"

**阶段 6：汇报**

Agent（协调者）: "功能完成。状态如下：
       - 设计、架构、实现、集成已完成
       - 严重问题已修复
       - 1 个轻微边界问题已放入待办
       可以合并。"

其他 team 技能类似：

- `/team-narrative`：从故事结构一路做到对白内容
- `/team-ui`：从线框稿一路做到完整 UI 实现
- `/team-level`：从关卡布局做到遭遇、摆件和细化
- `/team-audio`：从声音方向做到事件接入

### 步骤 5.3：AI 系统

如果你要做 NPC 行为、寻路或决策系统：

```text
Ask the ai-programmer agent to implement a patrol/investigate/chase
behavior tree for enemy NPCs as specified in design/gdd/enemy-ai.md.
```

**会发生什么：** `ai-programmer` 在实现前会先确认行为细节：

Agent: "我已经读了 `design/gdd/enemy-ai.md`。开始构建行为树前，我需要确认几项行为：

       - 巡逻时 NPC 是在路径点停留，还是持续循环移动？
       - 调查状态持续多久，之后才回到巡逻？
       - 发现玩家后要立刻追击，还是要有‘确认时间’？
       - 追击中丢失视线后怎么办？

       另外，我看到性能预算写的是所有 AI 总共 2ms/frame。最坏情况下同时会有多少个 NPC 激活？"

User: "Stop at waypoints for 2 seconds. Search for 8 seconds. Immediate chase on sight.
      Break LOS = return to last known position, search there, then investigate.
      Max 15 NPCs active."

Agent: "明白。这意味着每个 NPC 大约只有 0.13ms 的预算。我会使用分层行为树并共享寻路查询，以确保不超预算。

       我的方案：
       - 根选择器：追击 > 调查 > 巡逻
       - 所有时间与范围都来自 `assets/data/enemy_ai.json`（符合 ai-code 的数据驱动规则）
       - 提供调试可视化，展示当前状态与侦测半径
       - 攻击前保留 0.5 秒前摇，给玩家可读预警

       这个方案可以吗？"

User: "Yes"

Agent: [开始实现，自动遵守规则：2ms 性能预算、参数数据驱动、调试可视化、对玩家公平的 AI 表达]

       "实现完成。已在 20 个 NPC 的场景下测试，AI 总耗时 1.8ms/frame。
       调试可视化中，绿色=巡逻，黄色=调查，红色=追击。可以评审了。"

`ai-code` 规则会自动强制：性能预算、参数数据驱动、调试可视化以及对玩家公平的 AI 表达。

### 步骤 5.4：引擎特定工作

当你需要引擎专项建议时：

```text
# Godot:
Ask the godot-specialist agent to review our scene tree architecture
and suggest improvements for performance and maintainability.

# 特定子系统：
Ask the godot-gdscript-specialist to refactor our signal architecture
to reduce coupling between the combat and UI systems.
```

引擎专家通常擅长：

- 该引擎的最佳实践和常见反模式
- 该引擎的性能优化方式
- 正确使用引擎子系统（Godot signals、Unity Addressables、UE GAS 等）
- 判断什么时候该上原生扩展，什么时候脚本就够

### 步骤 5.5：架构决策记录

如果在实现过程中遇到重要技术抉择：

```text
/architecture-decision "Event Bus vs Direct Signals for System Communication"
```

这样就能把决策记录下来，方便未来的你或新加入的协作者理解**当时为什么这样设计**。

### 步骤 5.6：代码评审

实现完一个功能后：

```text
/code-review src/gameplay/combat/damage_calculator.gd
```

**会发生什么：** 这个技能会做一次协作式架构评审：

Agent: "正在评审 `src/gameplay/combat/damage_calculator.gd`……

       **架构一致性**：
       - 符合 ADR-003（无状态工具模式）
       - 伤害数值采用数据驱动（符合 gameplay-code 规则）
       - 问题：直接引用 `HealthComponent`，破坏了依赖注入

       **SOLID 原则**：
       - 单一职责：只负责伤害计算
       - 依赖倒置：和 `HealthComponent` 类型耦合过深

       **可测试性**：
       - `HealthComponent` 难以 mock

       **性能**：
       - 热路径无额外分配
       - 警告：每次调用都在字典中查伤害类型，建议缓存

       **建议**：
       1. 高优先级：为 `HealthComponent` 增加接口，便于 mock
       2. 中优先级：缓存类型修正查找
       3. 低优先级：补上边界情况的空值检查

       需要我演示如何修复第 1 个问题（依赖注入）吗？"

User: "Yes, show me"

Agent: [展示通过接口解耦后的重构代码]

       "这样一来，只要对象实现了 `take_damage()`，都可以注入进来，测试会更方便。要不要应用这个重构？"

User: "Approved"

Agent: [应用修改] "代码评审完成，所有高优先级问题已解决。"

---

## 阶段 6：测试与质量保证

### 这个阶段会发生什么

测试会贯穿整个生产过程，而不是只在最后做。系统默认支持测试驱动或验证驱动开发，并有自动规则帮助你维持质量。

### 步骤 6.1：先写测试（验证驱动开发）

项目强调**验证驱动开发（Verification-Driven Development）**：测试应在实现之前或与实现同步完成，而不是功能做完后才补。

`test-standards` 规则（`.claude/rules/test-standards.md`）会强制：

- 测试命名：`test_[system]_[scenario]_[expected_result]`
- 测试结构：Arrange / Act / Assert
- 不能依赖外部状态
- 测试需要自行清理
- 测试文件路径应和被测源文件对应

```text
Ask the qa-tester agent to write test cases for the damage calculation
system based on the acceptance criteria in design/gdd/combat-system.md.
```

**会发生什么：** `qa-tester` 会先根据设计文档梳理测试范围：

Agent: "我读了 `design/gdd/combat-system.md` 中的验收标准。写测试前先确认一下范围：

       目前我看到需要覆盖：
       - 基础伤害公式结果正确
       - 伤害类型修正生效正确
       - 边界情况：`attack_stat` 限制在 0 到 100
       - 边界情况：负伤害应归零

       问题：
       - 现在只写单元测试，还是也要和 `HealthComponent` 的集成测试？
       - 需要性能测试吗？比如 1000 次计算低于某个耗时？
       - 文档之外还有没有特别要覆盖的边界情况？"

User: "Unit tests only for now. Add performance test - target 10,000 calcs < 1ms.
      Also test what happens with invalid damage types."

Agent: "明白。我的测试计划如下：

       **单元测试**：
       - `test_damage_basic_calculation`
       - `test_damage_type_modifiers`
       - `test_damage_attack_stat_clamping`
       - `test_damage_negative_becomes_zero`
       - `test_damage_invalid_type`
       - `test_damage_rounding`

       **性能测试**：
       - `test_damage_performance_10k_calculations`

       这套测试可以吗？"

User: "Yes"

Agent: [按 test-standards 规则编写测试：命名规范、AAA 结构、无外部依赖、自清理]

       "测试套件完成，共 7 项。要现在运行吗？"

### 步骤 6.2：Bug 报告

发现 Bug 时：

```text
/bug-report "Healing spell applies damage instead of healing when
target has a shield buff active"
```

这会生成一份结构化问题报告，包含：

- 严重性（Critical / Major / Minor / Cosmetic）
- 优先级（P0-P4）
- 复现步骤
- 预期行为 vs 实际行为
- 受影响系统/组件
- 关联设计文档

### 步骤 6.3：测试计划

为里程碑做整体测试规划时：

```text
Ask the qa-lead agent to create a test plan for the Alpha milestone,
covering combat, progression, and UI systems.
```

**会发生什么：** `qa-lead` 会协助你做一份完整测试计划：

Agent: "我来为 Alpha 里程碑创建测试计划。先确认范围：

       根据里程碑定义，我看到以下系统在本次范围内：
       - 战斗系统
       - 成长系统
       - UI 系统

       问题：
       - 要测哪些平台？Windows、Linux、Mac、移动端？
       - 目标性能是多少？60fps？30fps？
       - 有没有已知高风险区域需要加强覆盖？
       - 截止时间是什么时候？"

User: "Windows and Linux. Target 60fps. Combat AI is risky - lots of edge cases.
      Testing deadline is end of next week."

Agent: "明白。我建议的测试计划结构如下：

       **功能测试**（3 天）：
       - 战斗：伤害计算、状态效果、死亡状态、AI 行为
       - 成长：经验获取、升级、技能解锁、属性增长
       - UI：菜单、HUD、背包、设置

       **边界测试**（1 天）：
       - 战斗 AI：视线中断、寻路失败、卡死状态
       - 边界条件：满级、背包满、负伤害

       **回归测试**（1 天）：
       - 复跑此前里程碑的全部测试

       **性能基准**（1 天）：
       - 15 个敌人 + 复杂 VFX 场景下保持 60fps
       - 内存低于 2GB
       - 加载时间低于 5 秒

       **平台测试**（1 天）：
       - Windows 10/11
       - Ubuntu 22.04 LTS

       这能满足你的时间线。要确认还是调整？"

User: "Approved"

Agent: [使用 test-plan.md 模板创建详细测试计划]

### 步骤 6.4：平衡性测试

当你实现了经济或成长系统后：

```text
/balance-check assets/data/combat_damage.json
```

它会分析你的平衡数据中是否存在：

- 统计异常值
- 成长曲线断裂
- 退化策略（某个选项在所有场景都碾压其他选项）
- 经济失衡（通胀、通缩、可利用漏洞）

---

## 阶段 7：打磨与优化

### 这个阶段会发生什么

你的游戏现在已经功能完整。接下来要做的是把它变得**真正好玩、好看、好用**。这一阶段重点是性能、视觉、音频和手感。

### 步骤 7.1：性能分析

```text
/perf-profile
```

**会发生什么：** 这个技能会引导你进行结构化性能分析：

Agent: "开始性能分析。先确认目标：
       - 目标帧率是多少？
       - 目标内存上限是多少？
       - 目标平台有哪些？"

User: "60fps on mid-range PC. Under 2GB RAM. Windows/Linux."

Agent: "正在对当前构建进行 profiler 分析……

       **当前性能**：
       - 平均 FPS：48
       - 帧时间：20.8ms（60fps 目标对应 16.6ms）
       - 内存：2.4GB（超目标 400MB）

       **已识别瓶颈**（按影响排序）：
       1. Combat AI：8.2ms/frame（占帧时间 49%）- 高优先级
       2. 粒子特效：3.1ms/frame（18%）- 中优先级
       3. UI 渲染：2.8ms/frame（17%）- 中优先级
       4. 物理：1.9ms/frame（11%）- 低优先级

       **内存问题**：
       1. 贴图图集未压缩：+280MB - 高优先级
       2. 音频缓冲未流式加载：+120MB - 高优先级

       **建议**：
       1. Combat AI：改成分层行为树，减少逐帧检查
       2. 贴图：启用 GPU 压缩
       3. 音频：长音频改为流式播放，不要全部常驻内存

       要不要我把前三个问题拆成可执行的优化任务？"

User: "Yes"

Agent: [生成可执行优化任务，附带代码位置和预期收益]

**更深入的分析请用 `performance-analyst`：**

```text
Ask the performance-analyst agent to analyze our combat system for
frame time spikes. We're seeing hitches when 10+ enemies are active.
```

Agent: "我来分析战斗系统的性能问题。先按不同敌人数做 profile……

       **分析结果**：
       - 5 个敌人：58fps 稳定
       - 10 个敌人：52fps，偶尔掉到 45fps
       - 15 个敌人：38fps，尖峰跌到 22fps

       **根因**：行为树每帧都为每个 NPC 完整重算一次。15 个 NPC 就是 15 次完整树遍历，总耗时 8.2ms。

       **可选方案**：
       1. 错峰更新：每帧只更新 5 个 NPC，轮询
       2. 缓存树结果：只有状态变化时才重算
       3. 简化行为树：把节点数从 12 个降到 6 个

       我推荐方案 2 + 3：缓存结果并简化树结构。
       预期收益：8.2ms -> 1.5ms（节省 6.7ms，大约可回收 40fps）

       要不要我展示优化后的行为树实现？"

User: "Yes"

Agent: [展示优化实现]

### 步骤 7.2：视觉打磨

使用 team polish 技能进行完整打磨：

```text
/team-polish "combat system"
```

**会发生什么：** team polish 会组织一轮多角色打磨流程：

Agent（协调者）: "开始为战斗系统做 polish pass。我先评估当前状态，并识别打磨优先级……

       **当前状态分析**：
       - 性能：48fps（需要优化）
       - 视觉反馈：命中特效有，但不够有冲击力
       - 音频：基础音效已接入，但混音还不理想
       - 手感：输入响应不错，但打击缺少重量感

       我会并行协调 4 位专家。先请你给优先级排序：
       - 性能（冲到 60fps）
       - 视觉冲击（VFX、shader、反馈）
       - 音频打磨（混音、空间感、力度）
       - 手感/juice（屏幕震动、hitstop、粒子）"

User: "Performance #1, then visual impact, then feel, then audio"

**阶段 1：性能**（performance-analyst）

Agent: "正在优化战斗系统性能……"
       [应用前述性能分析中的优化]
       "现在稳定在 62fps，性能目标达成。"

**阶段 2：视觉冲击**（technical-artist）

Agent: "我在看视觉反馈。命中 VFX 有了，但比较泛。几个问题：
       - 不同伤害类型要不要有不同视觉反馈？
       - 暴击是否要更明显？
       - 命中时屏幕震动要轻微还是明显？"

User: "Distinct per damage type. Crits should pop. Subtle screen shake."

Agent: [制作更精细的 VFX 规范与 shader 改进]
       "视觉打磨完成。要不要看前后对比视频？"

**阶段 3：手感**（technical-artist + gameplay-programmer）

Agent: "正在加入 hitstop（短暂停帧）、暴击粒子爆发和轻微相机震动，测试手感中……"
       "普通命中 100ms、暴击 200ms 的 hitstop 体验很好。确认吗？"

User: "Approved"

**阶段 4：验证**（qa-tester）

Agent: "正在对所有战斗打磨修改进行回归测试……"
       "全部通过，没有引入新 Bug。"

Agent（协调者）: "本次打磨完成：
       - 性能：48fps -> 62fps
       - 视觉：增强 VFX、伤害类型区分、暴击反馈
       - 手感：hitstop、粒子、镜头震动
       - 测试：全部通过
       可以合并。"

### 步骤 7.3：音频实现

```text
/team-audio "combat audio"
```

**会发生什么：** team audio 技能会组织完整的音频协作流水线：

**阶段 1：声音方向**（audio-director）

Agent: "我们先定义战斗音频方向。你希望战斗给人的情绪感受是：
       - 残酷、血腥、强烈？
       - 战术化、精准？
       - 魔法感、奇幻感？
       - 快速、街机感？"

User: "Tactical and precise with some weight to impacts"

Agent: "明白。我的战斗音频方向如下：

       **音频支柱**：
       - 清晰：每个动作都应该有明确可辨识的声音
       - 空间感：仅靠声音也能判断敌人方位
       - 反馈：成功操作要有满足感
       - 氛围：维持紧张感，但不让人听觉疲劳

       **优先级**：
       1. 武器音效要有力度，但不能刺耳疲劳
       2. 脚步声对空间感至关重要
       3. 敌人攻击应有声音前摇提示
       4. 命中确认要足够爽

       这个方向确认吗？"

User: "Approved"

**阶段 2：音效规范**（sound-designer）

Agent: "根据音频方向，我列了以下音效事件：

       **玩家动作**：
       - `weapon_swing_light`、`weapon_swing_heavy`
       - `footstep_walk`、`footstep_run`（按材质区分）
       - `ability_cast_start`、`ability_cast_release`

       **命中**：
       - `hit_flesh_light`、`hit_flesh_heavy`（带音高变化）
       - `hit_armor_deflect`（金属偏转音）
       - `critical_hit`（叠层设计的特殊暴击音）

       **敌人音频**：
       - `enemy_attack_windup`（0.5 秒前摇）
       - `enemy_footstep`（比玩家更明显，帮助定位）

       每个事件都准备 3 到 5 个变体避免重复。
       还有漏掉的重要音效吗？"

User: "Add dodge/roll sound"

Agent: "已加入 `dodge_roll`。音效规范已完整。"

**阶段 3：音频实现**（technical-artist）

Agent: "正在接入音频系统：
       - 基于事件触发，不在代码中硬编码音频调用
       - 3D 空间音频与距离衰减
       - 优先级系统（关键声音不会被截断）
       - 动态混音（战斗音乐层 + 音效平衡）

       两个问题：
       - 最大同时播放音数多少？我建议 32
       - 距离衰减曲线用线性还是对数？"

User: "32 sounds max. Logarithmic attenuation."

Agent: [实现音频中间件接入]

**阶段 4：集成**（gameplay-programmer）

Agent: "正在把音频事件接到玩法代码中……
       - 武器挥击：接到动画事件
       - 脚步声：和移动速度及地面材质联动
       - 命中音：接到伤害事件
       - 敌人攻击前摇音：在真正攻击前 0.5 秒触发

       可以开始游戏内试听。"

Agent（协调者）: "战斗音频流程完成。请在游戏里试听并反馈平衡、清晰度和手感。"

### 步骤 7.4：技术债

发售前，记得评估一次技术债：

```text
/tech-debt
```

它会扫描代码库中的：

- TODO / FIXME / HACK 注释
- 重复代码
- 过于复杂的函数
- 缺失的测试
- 过期依赖

每一项技术债都会被分类并排序。你再决定哪些要在发售前解决，哪些可以延后。

---

## 阶段 8：本地化与无障碍

### 步骤 8.1：本地化扫描

```text
/localize src/
```

它会检查：

- 应该抽离却仍然硬编码的字符串
- 会破坏翻译的字符串拼接
- 没有考虑文本膨胀的问题（例如德语通常长 30% 左右）
- 缺失的 locale 文件

### 步骤 8.2：无障碍评审

```text
Ask the accessibility-specialist agent to audit our UI system for
WCAG compliance, colorblind support, and input remapping.
```

`ui-code` 规则其实已经默认覆盖了一部分无障碍要求：

- 文本支持本地化，不允许硬编码
- 支持键盘与手柄输入
- 支持文本缩放
- 提倡色盲友好设计

而 `accessibility-specialist` 会进一步深入检查：

- 屏幕阅读器兼容性
- 按键重映射是否完整
- 字幕与说明文字支持
- 动效敏感选项
- 难度/辅助选项

---

## 阶段 9：发布与上线

### 这个阶段会发生什么

你的游戏已经打磨完毕、测试完成，准备上线。现在是正式出货的时候。

### 步骤 9.1：发布检查清单

```text
/release-checklist v1.0.0
```

它会生成一份发布前综合清单，覆盖：

- 构建验证（所有平台都能编译和运行）
- 认证需求（平台相关）
- 商店元数据（描述、截图、预告片）
- 法务合规（EULA、隐私政策、分级）
- 存档兼容性
- 分析与遥测验证

### 步骤 9.2：上线准备度检查（全量验证）

```text
/launch-checklist
```

这是“核选项”，意味着做一次全部门交叉验证：

| 部门 | 检查内容 |
|---|---|
| **工程** | 构建稳定性、崩溃率、内存泄漏、加载时间 |
| **设计** | 功能完成度、教程流程、难度曲线 |
| **美术** | 资源质量、缺失贴图、LOD 层级 |
| **音频** | 缺失音效、混音水平、空间音频 |
| **QA** | 按严重程度统计的未解决 Bug 数、回归通过率 |
| **叙事** | 对话完整性、设定一致性、错别字 |
| **本地化** | 所有文本是否翻译完成、是否截断、各语言实机测试 |
| **无障碍** | 合规清单、辅助功能测试 |
| **商店** | 元数据是否完整、截图是否批准、定价是否设置 |
| **市场** | 媒体包、上线预告片、社交媒体排期 |
| **社区** | Patch Notes 草稿、FAQ、客服渠道 |
| **基础设施** | 服务器扩容、CDN 配置、监控是否激活 |
| **法务** | EULA、隐私政策、COPPA/GDPR 合规 |

每一项都会给出 **Go / No-Go** 状态。全部为 Go 才能上线。

### 步骤 9.3：生成面向玩家的内容

```text
/patch-notes v1.0.0
```

它会根据 git 历史和内部数据，生成玩家能看懂的更新说明。会把开发者语言，比如 “refactored state machine”，翻译成玩家语言，比如 “优化了敌人的行为响应”。

```text
/changelog v1.0.0
```

它会生成一份更偏技术、供团队内部使用的变更日志。

### 步骤 9.4：协调发布

```text
/team-release
```

**会发生什么：** team release 技能会协调所有部门完成上线：

**阶段 1：发布前验证**（release-manager）

Agent: "开始为 v1.0.0 做发布协调。先汇总各部门状态……

       我在复查 launch checklist 结果：
       - 工程：全部绿色
       - QA：仍有 2 个轻微 Bug 未关闭（不阻塞）
       - 美术/音频：完成
       - 本地化：完成
       - 商店：截图等待审核
       - 市场：上线预告片尚未最终完成

       目前识别出 2 个阻塞项。它们会影响时间线吗？"

User: "Screenshots approved this morning. Trailer done by end of day."

Agent: "明白。时间线已更新：如果预告片今天结束前完成，就计划明天发售。
       要不要继续准备构建包？"

User: "Yes"

**阶段 2：构建管理**（release-manager）

Agent: "正在创建 v1.0.0 发布构建……
       - Windows 构建：完成，已测试，2.1GB
       - Linux 构建：完成，已测试，2.3GB
       - 已生成校验和
       - 已创建版本清单
       - 已配置更新服务器

       所有构建都已准备好部署。"

**阶段 3：最终 QA 签核**（qa-lead）

Agent: "正在对发布构建跑最终回归……
       - 所有关键测试：通过
       - 所有高优先级测试：通过
       - 平台专项测试：通过

       QA 已签核，无阻塞问题。"

**阶段 4：部署准备**（devops-engineer）

Agent: "CI/CD 流水线已就绪。部署计划如下：
       1. 12:00 UTC 上传构建到 CDN
       2. 12:30 UTC 更新商店条目
       3. 13:00 UTC 开放下载（正式上线时间）
       4. 持续监控服务器负载与错误率

       如果出现严重问题，回滚方案也已准备好。
       这个部署排期确认吗？"

User: "Approved"

**阶段 5：Go / No-Go 决策**（producer）

Agent: "最终 go/no-go 检查：
       - 所有构建已验证
       - QA 已签核
       - 商店元数据已批准
       - 上线预告片已完成
       - 市场素材已就绪
       - 支持团队已同步
       - 部署计划已批准

       所有系统均为 GO。建议明天 13:00 UTC 正式上线。
       最终批准吗？"

User: "GO for launch"

Agent: "上线批准。明天 12:00 UTC 开始部署。我会持续监控并回报状态更新。"

### 步骤 9.5：发货

`validate-push` hook 会在你向 `main` 或 `develop` 推送时发出提醒。这是故意设计的，因为发布推送必须谨慎：

```bash
git tag v1.0.0
git push origin main --tags
```

---

## 阶段 10：上线后运营与 Live Ops

### 步骤 10.1：热修流程

当线上出现严重问题时：

```text
/hotfix "Players losing save data when inventory exceeds 99 items"
```

这会绕过常规 Sprint 流程，但保留完整审计链：

1. 创建 hotfix 分支
2. 跟踪审批记录
3. 实现修复
4. 确保修复回灌到开发分支
5. 记录事故文档

### 步骤 10.2：项目复盘

上线尘埃落定后：

```text
Ask Claude to create a post-mortem using the template at
.claude/docs/templates/post-mortem.md
```

通常会覆盖：

- 哪些地方做得好
- 哪些地方做得不好
- 哪些事情出乎意料
- 关键指标（销量、评价、崩溃率）
- 对下个项目的经验教训

### 步骤 10.3：Live Operations（如果适用）

如果你的游戏会持续更新内容：

```text
Ask the live-ops-designer agent to design a seasonal content cadence
with battle pass, weekly events, and monthly content drops. Target
retention for the 30-60-90 day cohorts.
```

`live-ops-designer` 通常负责：

- 赛季/活动日历
- Battle Pass 设计
- 内容投放节奏规划
- 留存机制
- 实时经济调优
- 参与度数据分析

### 步骤 10.4：社区管理

```text
Ask the community-manager agent to draft patch notes for v1.0.1,
respond to the top 5 community complaints from the feedback channel,
and prepare a "known issues" post.
```

---

## 附录 A：代理速查表

### “我想做 X，应该找哪个代理？”

| 我想做…… | 代理 | 层级 |
|---|---|---|
| 想一个游戏点子 | `/brainstorm` 技能 | -- |
| 设计游戏机制 | `game-designer` | 2 |
| 设计公式/数值 | `systems-designer` | 3 |
| 设计关卡 | `level-designer` | 3 |
| 设计掉落表 / 经济系统 | `economy-designer` | 3 |
| 构建世界观设定 | `world-builder` | 3 |
| 写对白 | `writer` | 3 |
| 规划故事 | `narrative-director` | 2 |
| 规划一个 Sprint | `producer` | 1 |
| 做创意方向决策 | `creative-director` | 1 |
| 做技术方向决策 | `technical-director` | 1 |
| 实现玩法代码 | `gameplay-programmer` | 3 |
| 实现引擎核心系统 | `engine-programmer` | 3 |
| 实现 AI 行为 | `ai-programmer` | 3 |
| 实现多人联机 | `network-programmer` | 3 |
| 实现 UI | `ui-programmer` | 3 |
| 制作开发工具 | `tools-programmer` | 3 |
| 评审代码架构 | `lead-programmer` | 2 |
| 编写 shader / VFX | `technical-artist` | 3 |
| 定义视觉风格 | `art-director` | 2 |
| 定义音频风格 | `audio-director` | 2 |
| 设计音效 | `sound-designer` | 3 |
| 设计 UX 流程 | `ux-designer` | 3 |
| 编写测试用例 | `qa-tester` | 3 |
| 制定测试策略 | `qa-lead` | 2 |
| 分析性能 | `performance-analyst` | 3 |
| 搭建 CI/CD | `devops-engineer` | 3 |
| 设计分析埋点 | `analytics-engineer` | 3 |
| 检查无障碍 | `accessibility-specialist` | 3 |
| 规划长期运营 | `live-ops-designer` | 3 |
| 管理一次发布 | `release-manager` | 2 |
| 管理本地化 | `localization-lead` | 2 |
| 快速做原型 | `prototyper` | 3 |
| 做安全审计 | `security-engineer` | 3 |
| 与玩家沟通 | `community-manager` | 3 |
| Godot 专项帮助 | `godot-specialist` | 3 |
| Unity 专项帮助 | `unity-specialist` | 3 |
| Unreal 专项帮助 | `unreal-specialist` | 3 |

### 代理层级关系

```text
                    creative-director / technical-director / producer
                                         |
          ---------------------------------------------------------------
          |            |           |           |          |        |
    game-designer  lead-prog  art-dir  audio-dir  narr-dir  qa-lead  release-mgr
          |            |           |           |          |        |
     specialists  programmers  tech-art  snd-design  writer   qa-tester  devops
```

**升级规则：** 如果两个代理意见相左，就往上一级。设计冲突交给 `creative-director`，技术冲突交给 `technical-director`，范围冲突交给 `producer`。

---

## 附录 B：斜杠命令速查表

### 按工作流阶段分类

| 阶段 | 命令 |
|---|---|
| **入门** | `/start` |
| **创意构思** | `/brainstorm` |
| **设计** | `/map-systems`、`/design-system`、`/design-review`、`/architecture-decision` |
| **Sprint** | `/sprint-plan`、`/estimate`、`/scope-check`、`/retrospective` |
| **实现** | `/code-review`、`/prototype`、`/tech-debt` |
| **测试** | `/balance-check`、`/playtest-report`、`/perf-profile` |
| **资源** | `/asset-audit`、`/localize` |
| **发布** | `/release-checklist`、`/launch-checklist`、`/changelog`、`/patch-notes`、`/hotfix` |
| **生产** | `/milestone-review`、`/onboard` |
| **团队协作** | `/team-combat`、`/team-narrative`、`/team-ui`、`/team-release`、`/team-polish`、`/team-audio`、`/team-level` |

---

## 附录 C：常见工作流

### 工作流 1：“我刚开始，还没有任何游戏点子”

```text
1. /start（询问你当前阶段，并把你导向合适流程）
   或者如果你想直接开始构思，也可以直接 /brainstorm
2. 从 brainstorm 结果里挑出最好的概念
3. 创建游戏概念文档（templates/game-concept.md）
4. 定义游戏支柱（templates/game-pillars.md）
5. 对概念文档运行 /design-review
6. 用 /map-systems 把概念拆成系统，并建立依赖与优先级
7. 用 /design-system 逐个编写系统级 GDD（分节引导）
```

### 工作流 2：“我已经有设计了，想开始写代码”

```text
1. 对每份 GDD 先跑 /design-review，确认足够扎实
2. 用 /architecture-decision 处理第一个关键技术选择
3. /sprint-plan new，规划第一个 Sprint
4. 让 gameplay-programmer / engine-programmer 开始实现
5. 每完成一个大功能就跑 /code-review
6. 写代码时同步写测试
7. 高频提交（hooks 会自动校验）
```

### 工作流 3：“我要加一个复杂功能”

```text
1. 在 design/gdd/ 中创建或更新该功能的 GDD
2. 跑 /design-review 验证设计
3. 跑 /estimate 评估工作量和风险
4. 使用合适的 /team-* 技能：
   - /team-combat 用于战斗相关功能
   - /team-narrative 用于剧情内容
   - /team-ui 用于 UI 功能
   - /team-level 用于新关卡/新区域
   - /team-audio 用于音频工作
5. 对实现结果跑 /code-review
6. 如果影响平衡，再跑 /balance-check
```

### 工作流 4：“线上出问题了”

```text
1. /hotfix "问题描述"
2. 在 hotfix 分支上完成修复
3. 对修复结果跑 /code-review
4. 跑测试
5. 对热修构建跑 /release-checklist
6. 部署并回灌到开发分支
```

### 工作流 5：“我快到一个里程碑了”

```text
1. /milestone-review 检查当前进度
2. /scope-check 看是否发生了范围蔓延
3. /tech-debt 评估里程碑前的技术债
4. /perf-profile 检查性能目标
5. /team-polish 做最终打磨
6. 准备发货时跑 /release-checklist
```

### 工作流 6：“我要开始一个新的 Sprint”

```text
1. /retrospective 回顾上一个 Sprint
2. /sprint-plan new 创建新 Sprint
3. /scope-check 确保范围合理
4. 按任务推进 Sprint 工作
5. 中途可用 /sprint-plan status 查看状态
```

### 工作流 7：“我要发售游戏了”

```text
1. /milestone-review 检查最终里程碑
2. /tech-debt 判断哪些问题在发售时仍可接受
3. /localize 做最后一轮本地化检查
4. 让 accessibility-specialist 做无障碍审计
5. /launch-checklist 做全部门上线验证
6. /team-release 统一协调发布
7. /patch-notes 和 /changelog 生成对外/对内说明
8. 发售
9. 上线后若有问题，用 /hotfix
10. 上线稳定后做 Post-mortem
```

---

## 如何更好地用好这套系统

1. **永远先设计，再实现。** 整套 Agent 系统默认前提就是：代码编写前，设计文档已经存在。代理会频繁引用 GDD。

2. **跨领域功能优先用 team 技能。** 不要手动协调 4 个代理，让 `/team-combat`、`/team-narrative` 之类的技能接管编排。

3. **相信规则系统。** 当规则指出代码有问题时，尽量修掉。那些规则是很多游戏开发经验沉淀出来的，比如数据驱动、delta time、无障碍等。

4. **主动压缩上下文。** 当上下文使用量接近 65% 到 70% 时，主动 compact 或 `/clear`。pre-compact hook 会帮你保存进度，不要等到快爆了再处理。

5. **用对层级的代理。** 不要让 `creative-director` 去写 shader，也不要让 `qa-tester` 去做设计决策。层级划分是有意义的。

6. **把设计交给程序前先跑 `/design-review`。** 这能更早发现规格不完整，减少返工。

7. **每个大功能完成后都跑 `/code-review`。** 尽早发现架构问题，别让它扩散。

8. **高风险机制先做原型。** 一天原型验证，可能帮你省下一整周在错误方向上的生产成本。

9. **让 Sprint 计划保持诚实。** 经常用 `/scope-check`。范围失控是独立游戏最常见的失败原因之一。

10. **用 ADR 记录决策。** 未来的你一定会感谢现在的你，把“为什么这样做”写了下来。
