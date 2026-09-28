# CRC Product Design System
# Canonical Product Design System

**Design Language:** Academic Operations  
**Canonical Path:** `design-system/DESIGN_SYSTEM.md`  
**Version:** 2.0.0  
**Status:** Canonical / Active  
**Owner:** NAKAMA Product & Engineering  
**Platform:** CRC LMS / Management System  
**Last Revised:** September 2026

---

# Table of Contents

0. Document Authority & AI Usage  
1. Design-System Audit  
2. Design Principles  
3. Brand Foundation  
4. Token Architecture  
5. Color System  
6. Typography  
7. Spacing & Sizing  
8. Layout, Grid & Responsive System  
9. Radius, Borders & Elevation  
10. Icons, Imagery & Illustration  
11. Motion & Interaction  
12. Accessibility  
13. Component Architecture  
14. Interaction State System  
15. Core Component Specifications  
16. Forms & Validation  
17. Tables & Data-Dense Interfaces  
18. Dashboard Composition System  
19. Data Visualization  
20. Academic Domain Patterns  
21. LMS Experience  
22. CBT & Examination Experience  
23. Authentication, Permissions & Security  
24. Offline & Synchronization Experience  
25. AI Experience  
26. Role-Based Experience Guidance  
27. Local Server / Operations Manager  
28. Feedback, Notifications & System States  
29. Content & UX Writing  
30. Website Scope Boundary  
31. Print & Reporting  
32. Representative Screen Blueprints  
33. Design-to-Code Architecture  
34. Figma Architecture & Handoff  
35. Testing & Consistency Audit  
36. Governance, Versioning & Contribution  
37. Non-Negotiable Rules  
38. Acceptance Criteria  
39. Changelog

---

# 00 — Document Authority & AI Usage

## 00.1 Purpose

This file is the single visual and interaction source of truth for the CRC product deployed at Christian Royal College.

NAKAMA is the technology company developing/providing CRC. Product attribution may be **CRC powered by NAKAMA**. This is the CRC Product Design System; the NAKAMA corporate website and its design system are separate future projects outside this repository, along with unrelated company products and services.

It governs:

- Authentication
- Student Portal
- Parent Portal
- Teacher Portal
- Principal Portal
- Super Admin Portal
- LMS
- CBT
- Assessments
- Academic Administration
- Results
- Analytics
- Offline Application
- Synchronization
- Local AI
- System Administration
- Local Server Management
- Security Interfaces
- Audit Interfaces
- Reports
- Print Outputs
- Responsive Interfaces

No product domain owns an independent visual system.

---

## 00.2 Authority Order

When rules appear to conflict:

```text
Security requirements
        ↓
Business/product requirements
        ↓
DESIGN_SYSTEM.md
        ↓
Shared design tokens
        ↓
Shared components
        ↓
Domain patterns
        ↓
Role-specific composition
        ↓
Individual screen implementation
```

Lower-level UI MUST NOT silently contradict higher-level rules.

---

## 00.3 Normative Language

**MUST** — mandatory.

**MUST NOT** — prohibited.

**SHOULD** — expected unless there is a documented reason to deviate.

**SHOULD NOT** — strongly discouraged.

**MAY** — optional.

---

## 00.4 AI Agent Reading Strategy

AI coding agents SHOULD NOT read this entire file for every task when targeted retrieval is available.

Use the smallest relevant section set.

### Standard component

```text
§04 Tokens
§05 Color
§06 Typography
§07 Spacing
§09 Geometry
§12 Accessibility
§13 Component architecture
§14 States
§15 Component specifications
```

### Form

```text
§05
§06
§07
§12
§14
§15
§16
§28
```

### Data table

```text
§05
§06
§07
§08
§12
§14
§17
§28
```

### Dashboard

```text
§08
§18
§19
§26
```

### LMS

```text
§08
§12
§20
§21
§26
```

### CBT

```text
§08
§12
§14
§22
§24
§28
```

### Offline UI

```text
§14
§24
§28
```

### AI UI

```text
§12
§14
§25
§28
```

### Authentication

```text
§12
§15
§16
§23
§28
```

### Super Admin

```text
§17
§18
§23
§24
§26
§27
```

---

## 00.5 AI Implementation Rules

AI agents MUST:

1. inspect existing shared components before creating new components;
2. use semantic tokens;
3. preserve accessibility;
4. implement relevant states;
5. follow responsive transformation rules;
6. account for offline behavior where applicable;
7. avoid permanent one-off values;
8. avoid copying inconsistent legacy UI;
9. avoid new component variants without clear product meaning;
10. flag missing design-system rules rather than silently inventing permanent standards.

---

## 00.6 Root Agent Instruction

`AGENTS.md` SHOULD contain:

```md
## UI / Design System

For frontend, UI, UX, accessibility, responsive, LMS, CBT,
offline, AI-interface, analytics, or component work:

Read `design-system/DESIGN_SYSTEM.md`.

Start with §00 and retrieve only the relevant sections.

This file is authoritative for product UI and interaction decisions.
```

---

# 01 — Design-System Audit

Before revision, the previous design system was assessed against scalability, implementation precision, consistency, accessibility, and long-term governance.

---

## 01.1 KEEP

The following decisions were already strong and remain foundational.

### Academic Operations direction

The product should feel:

- professional;
- modern;
- calm;
- institutional;
- trustworthy;
- information-focused.

### Neutral-first visual language

Operational UI should be primarily neutral with restrained brand emphasis.

### Semantic token philosophy

The architecture:

```text
Primitive
→ Semantic
→ Component
```

remains correct.

### Inter as UI typeface

Inter remains appropriate for:

- tables;
- forms;
- dashboards;
- academic content;
- low-resolution displays;
- offline usage.

### 4px geometry foundation

The base spacing system remains 4px.

### Restrained radius and shadows

Borders remain preferable to unnecessary floating-card shadows.

### Role consistency

Student, Parent, Teacher, Principal, and Super Admin remain one product.

### Offline-first UX

Offline and synchronization remain first-class product states.

### AI integration

AI remains part of the core product rather than a visually separate experience.

### WCAG 2.2 AA target

Accessibility remains a system requirement.

---

## 01.2 REFINE

The following were directionally correct but insufficiently specified.

### Color

Needed:

- formal brand roles;
- secondary/accent definitions;
- interactive states;
- chart roles;
- dark-mode position;
- public-site versus application restrictions.

### Typography

Needed:

- stronger semantic usage rules;
- responsive behavior;
- reporting;
- low-resolution requirements;
- metric hierarchy.

### Responsive system

Needed exact transformations rather than only breakpoints.

### Components

Needed a consistent specification model:

```text
Purpose
Anatomy
Variants
Sizes
States
Behavior
Responsive
Accessibility
Content
Do not use when
```

### Dashboards

Needed composition rules rather than role-level content lists only.

### Academic UI

Needed reusable patterns for:

- subjects;
- topics;
- question banks;
- learning gaps;
- assessment status;
- results;
- score summaries.

### Design-to-code

Needed stronger mapping between tokens, components, Figma, and application code.

---

## 01.3 REPLACE

The following weaker ideas are replaced.

### Informal component definitions

Components are no longer documented merely as inventory lists.

Major components now have enforceable specifications.

### Generic responsive statements

“Make it responsive” is replaced with explicit transformation rules.

### Dashboard-as-card-grid thinking

Dashboards now follow attention hierarchy and composition zones.

### Screen-specific styling

Feature-specific visual conventions are explicitly prohibited.

### AI-only context file

Because this project intentionally uses a single design-system file, AI routing now lives in §00 instead of requiring another design-system document.

---

## 01.4 MISSING — NOW ADDED

The previous version lacked sufficient detail for:

- brand foundation;
- component dependency hierarchy;
- complete dashboard framework;
- local server manager;
- approval workflows;
- academic reusable components;
- mobile CBT transformations;
- chart conventions;
- real representative screen blueprints;
- consistency audit process;
- low-end hardware guidance;
- component content rules;
- design-system maturity criteria.

These are now included.

---

# 02 — Design Principles

Eight principles govern future UI decisions.

---

## 02.1 Clarity Before Decoration

Every visual decision must improve:

- comprehension;
- hierarchy;
- action recognition;
- grouping;
- state communication.

Decoration alone is insufficient justification.

---

## 02.2 Dense Information Must Remain Scannable

The platform contains significant academic and administrative data.

Density is acceptable.

Confusion is not.

Data-heavy UI must use:

- alignment;
- whitespace;
- consistent columns;
- controlled color;
- predictable hierarchy.

---

## 02.3 One Product, Multiple Roles

Roles change:

- information;
- permissions;
- workflows;
- density;
- navigation priorities.

Roles do not change the underlying visual language.

---

## 02.4 Progressive Disclosure

Expose complexity when users need it.

Do not place:

- every filter;
- every setting;
- every advanced control

at the same hierarchy level.

---

## 02.5 Familiar Patterns Beat Clever Patterns

Use established conventions for:

- navigation;
- forms;
- tabs;
- filters;
- dialogs;
- tables;
- search;
- pagination.

Novelty requires a measurable usability reason.

---

## 02.6 Accessibility Is a Definition of Done

A component that cannot be used effectively through keyboard navigation, zoom, readable contrast, or assistive technology is incomplete.

---

## 02.7 State Must Always Be Understandable

Users should know:

- what is happening;
- whether their work is saved;
- whether action succeeded;
- whether data is current;
- whether the system is offline;
- whether action is required.

---

## 02.8 Design for the Real School Environment

The UI must remain usable with:

- older PCs;
- lower-resolution displays;
- touch devices;
- variable network quality;
- offline operation;
- large student datasets;
- long names;
- non-ideal hardware.

Do not design only for high-end developer machines.

---

# 03 — Brand Foundation

## 03.1 Brand Character

The digital product should express:

- academic seriousness;
- trust;
- discipline;
- intelligence;
- progress;
- technological capability.

Avoid:

- childish education tropes;
- gaming aesthetics;
- luxury styling;
- fintech mimicry;
- neon “AI” branding.

---

## 03.2 Brand Color Roles

Until official institutional brand assets are finalized, the following digital brand structure is provisional.

### Primary

`brand.primary`

HEX: `#1D4ED8`

RGB: `29, 78, 216`

Purpose:

- primary actions;
- active navigation;
- key links;
- selection;
- focus-associated brand emphasis.

---

### Secondary

`brand.secondary`

HEX: `#172554`

RGB: `23, 37, 84`

Purpose:

- institutional depth;
- public-site sections;
- dark brand surfaces;
- formal branded layouts.

Operational UI SHOULD use this sparingly.

---

### Accent

`brand.accent`

HEX: `#0EA5E9`

RGB: `14, 165, 233`

Purpose:

- public-site highlights;
- limited branded visual accents;
- diagrams.

It MUST NOT replace semantic information color when meaning could become ambiguous.

---

### Supporting Neutral

`brand.supporting`

HEX: `#334155`

RGB: `51, 65, 85`

Purpose:

- secondary institutional surfaces;
- formal typography;
- supporting brand composition.

---

## 03.3 Logo Usage

The final asset library MUST include:

- primary logo;
- crest/icon;
- horizontal logo;
- monochrome dark;
- monochrome light;
- transparent-background assets.

Do not:

- stretch;
- recolor arbitrarily;
- place low-contrast logo variants;
- recreate the crest from icons.

---

## 03.4 Product Attribution and Operational Branding

CRC may use **CRC powered by NAKAMA** attribution.

Brand color is restrained.

Neutral surfaces dominate.

---

# 04 — Token Architecture

## 04.1 Hierarchy

```text
Primitive Tokens
      ↓
Semantic Tokens
      ↓
Component Tokens
      ↓
Components
      ↓
Patterns
      ↓
Screens
```

---

## 04.2 Primitive Tokens

Describe raw values.

Examples:

```text
color.blue.700
space.4
radius.md
font.size.16
```

---

## 04.3 Semantic Tokens

Describe meaning.

Examples:

```text
color.text.primary
color.surface.page
color.action.primary
space.page.inline
```

---

## 04.4 Component Tokens

Used only when a component needs specific semantics.

Example:

```text
button.primary.background.default
button.primary.background.hover
input.border.focus
```

---

## 04.5 Naming Rules

Preferred:

```text
category.role.property.state
```

Examples:

```text
color.text.primary
color.status.success.background
button.primary.background.hover
```

Avoid:

```text
niceBlue
darkGray
homepageBlue
teacherGreen
```

---

# 05 — Color System

## 05.1 Primitive Neutral Scale

| Token | HEX |
|---|---|
| neutral.0 | `#FFFFFF` |
| neutral.50 | `#F8FAFC` |
| neutral.100 | `#F1F5F9` |
| neutral.200 | `#E2E8F0` |
| neutral.300 | `#CBD5E1` |
| neutral.400 | `#94A3B8` |
| neutral.500 | `#64748B` |
| neutral.600 | `#475569` |
| neutral.700 | `#334155` |
| neutral.800 | `#1E293B` |
| neutral.900 | `#0F172A` |
| neutral.950 | `#020617` |

---

## 05.2 Primary Blue Scale

| Token | HEX |
|---|---|
| blue.50 | `#EFF6FF` |
| blue.100 | `#DBEAFE` |
| blue.200 | `#BFDBFE` |
| blue.300 | `#93C5FD` |
| blue.400 | `#60A5FA` |
| blue.500 | `#3B82F6` |
| blue.600 | `#2563EB` |
| blue.700 | `#1D4ED8` |
| blue.800 | `#1E40AF` |
| blue.900 | `#1E3A8A` |
| blue.950 | `#172554` |

---

## 05.3 Success Scale

Use green scale:

```text
50  #F0FDF4
100 #DCFCE7
200 #BBF7D0
300 #86EFAC
400 #4ADE80
500 #22C55E
600 #16A34A
700 #15803D
800 #166534
900 #14532D
950 #052E16
```

---

## 05.4 Warning Scale

Use amber:

```text
50  #FFFBEB
100 #FEF3C7
200 #FDE68A
300 #FCD34D
400 #FBBF24
500 #F59E0B
600 #D97706
700 #B45309
800 #92400E
900 #78350F
950 #451A03
```

---

## 05.5 Error Scale

Use red:

```text
50  #FEF2F2
100 #FEE2E2
200 #FECACA
300 #FCA5A5
400 #F87171
500 #EF4444
600 #DC2626
700 #B91C1C
800 #991B1B
900 #7F1D1D
950 #450A0A
```

---

## 05.6 Information Scale

Use sky.

---

## 05.7 AI Identifier

Use violet only as a restrained identifier.

AI MUST NOT become an independent purple visual system.

---

## 05.8 Text Tokens

```text
text.primary
text.secondary
text.tertiary
text.disabled
text.inverse
text.link
text.linkHover
text.success
text.warning
text.error
text.info
```

Light mappings:

```text
primary     neutral.900
secondary   neutral.600
tertiary    neutral.500
disabled    neutral.400
inverse     neutral.0
link        blue.700
linkHover   blue.800
```

---

## 05.9 Surface Tokens

```text
surface.page
surface.primary
surface.secondary
surface.elevated
surface.sunken
surface.interactive
surface.interactiveHover
surface.selected
surface.disabled
surface.inverse
```

Recommended light mappings:

```text
page              neutral.50
primary           neutral.0
secondary         neutral.50
elevated          neutral.0
sunken            neutral.100
interactive       neutral.0
interactiveHover  neutral.50
selected          blue.50
disabled          neutral.100
inverse           neutral.900
```

---

## 05.10 Border Tokens

```text
border.subtle
border.default
border.strong
border.focus
border.error
border.success
border.warning
```

Mappings:

```text
subtle   neutral.100
default  neutral.200
strong   neutral.300
focus    blue.600
error    red.600
success  green.600
warning  amber.600
```

---

## 05.11 Semantic Status Structure

Every status family MUST provide:

```text
background
foreground
border
icon
strongBackground
onStrong
```

---

## 05.12 Interaction Color States

Primary action:

```text
default   blue.700
hover     blue.800
active    blue.900
disabled  neutral.200
```

---

## 05.13 Selection

```text
background  blue.50
border      blue.300
foreground  blue.900
icon        blue.700
```

---

## 05.14 Status Mapping

### Success

- successful save;
- healthy service;
- synced;
- passed.

### Warning

- pending;
- delayed;
- degraded;
- late;
- attention needed.

### Error

- failed;
- blocked;
- destructive;
- critical.

### Information

- explanatory system state.

---

## 05.15 Dark Mode

Dark mode is **future-compatible but not required for initial release**.

Implementation MUST use semantic tokens so dark mode can be added without component rewrites.

Do not create dark mode merely by inversion.

---

## 05.16 Contrast

Target WCAG 2.2 AA.

Minimum targets:

```text
Normal text       4.5:1
Large text        3:1
Meaningful UI     3:1 where applicable
```

---

# 06 — Typography

## 06.1 Typeface

Primary:

> Inter Variable

No secondary UI font in v2.

---

## 06.2 Font Delivery

Inter MUST be self-hosted/local.

Offline applications MUST NOT depend on external font services.

---

## 06.3 Fallback Stack

```css
"Inter",
"Inter Variable",
ui-sans-serif,
system-ui,
-apple-system,
BlinkMacSystemFont,
"Segoe UI",
sans-serif
```

---

## 06.4 Monospace

Use system monospace for:

- IDs;
- logs;
- hashes;
- diagnostic values;
- code.

---

## 06.5 Weights

```text
400 Regular
500 Medium
600 Semibold
700 Bold
```

No routine arbitrary weights.

---

## 06.6 Type Scale

| Role | Size | Line Height | Weight |
|---|---:|---:|---:|
| Display | 36 | 44 | 700 |
| H1 | 30 | 38 | 700 |
| H2 | 24 | 32 | 600 |
| H3 | 20 | 28 | 600 |
| H4 | 18 | 26 | 600 |
| Body Large | 18 | 28 | 400 |
| Body | 16 | 24 | 400 |
| Body Small | 14 | 20 | 400 |
| Label | 14 | 20 | 500 |
| Label Small | 12 | 16 | 500 |
| Caption | 12 | 16 | 400 |
| Overline | 12 | 16 | 600 |

---

## 06.7 Metric Scale

```text
metric.xl  36 / 40 / 700
metric.lg  30 / 36 / 700
metric.md  24 / 32 / 600
metric.sm  20 / 28 / 600
```

---

## 06.8 Usage

### Dashboard

Metrics use metric tokens.

Labels remain restrained.

### Tables

14px default.

12px only for metadata.

### Forms

Input values default to 16px.

### CBT

Question text 16–18px.

### Reports

Readable print sizing takes priority over application density.

---

## 06.9 Numerals

Use tabular numerals for:

- scores;
- percentages;
- timers;
- counts;
- aligned analytics.

---

## 06.10 Capitalization

Use sentence case.

Preferred:

```text
Create assessment
Student records
System settings
```

---

## 06.11 Reading Width

Long learning content:

```text
45–75 characters
ideal around 60–70
```

Typical:

```css
max-width: 65ch;
```

---

# 07 — Spacing & Sizing

## 07.1 Base Unit

4px.

---

## 07.2 Primitive Scale

```text
0
4
8
12
16
20
24
32
40
48
64
80
96
```

---

## 07.3 Semantic Spacing

```text
space.inline.xs
space.inline.sm
space.inline.md

space.stack.xs
space.stack.sm
space.stack.md
space.stack.lg

space.section.sm
space.section.md
space.section.lg

space.page.inline
space.page.block
```

---

## 07.4 Page Padding

### Large desktop

32–40px.

### Desktop

24–32px.

### Tablet

20–24px.

### Mobile

16px.

---

## 07.5 Control Heights

```text
xs       28
compact  32
default  40
large    48
xl       56
```

Most controls use:

```text
32
40
48
```

---

## 07.6 Touch Targets

Touch interaction SHOULD target approximately:

```text
44 × 44px
```

or larger.

---

## 07.7 Card Padding

Comfortable:

24px.

Standard:

16–24px.

Compact:

12–16px.

---

## 07.8 Form Vertical Rhythm

Typical:

```text
Label → input       8px
Input → helper      6–8px
Field → field       16–20px
Group → group       24–32px
Section → section   32–48px
```

---

# 08 — Layout, Grid & Responsive System

## 08.1 Breakpoints

```text
sm    640
md    768
lg    1024
xl    1280
2xl   1536
```

---

## 08.2 Grid

### Desktop

12 columns.

Gutter:

24px.

### Tablet

8 columns.

Gutter:

20px.

### Mobile

4 columns.

Gutter:

16px.

---

## 08.3 Maximum Width

General application:

```text
1440px recommended
1600px maximum for wide workspaces
```

Data-heavy screens MAY use full available width.

Reading surfaces remain constrained.

---

## 08.4 Sidebar

Desktop expanded:

248px.

Desktop collapsed:

72px.

---

## 08.5 Header

Default:

64px.

Compact operational mode:

56px.

---

## 08.6 Responsive Navigation Transformation

```text
≥1280
Expanded sidebar

1024–1279
Collapsed sidebar by default; expandable

768–1023
Collapsed rail or overlay navigation

<768
Mobile header + drawer
```

Student/Parent mobile MAY use bottom navigation for 3–5 highest-frequency destinations.

---

## 08.7 Card Transformation

Desktop:

multi-column where content supports it.

Tablet:

reduce columns.

Mobile:

single-column.

Do not preserve tiny multi-column cards on mobile.

---

## 08.8 Form Transformation

Desktop:

multi-column only where fields are logically related.

Mobile:

single-column unless tiny controls logically pair.

---

## 08.9 Table Transformation

Priority order:

1. preserve table with horizontal scrolling;
2. hide low-priority columns when justified;
3. offer expandable row details;
4. convert to structured list only when table relationships remain understandable.

---

## 08.10 Filter Transformation

Desktop:

inline toolbar or side filter.

Mobile:

drawer or sheet.

Active filters remain visible after closing the filter surface.

---

## 08.11 Modal Transformation

Desktop:

centered modal.

Mobile:

full-width dialog or bottom/full-height sheet depending on workflow.

Long multi-step flows SHOULD become pages rather than enormous mobile dialogs.

---

## 08.12 Chart Transformation

Mobile charts:

- reduce nonessential labels;
- preserve values;
- maintain minimum readable plotting height;
- move legends below where appropriate.

---

## 08.13 CBT Transformation

Desktop:

question navigation may remain visible alongside question area.

Mobile:

question navigator becomes drawer/sheet or compact overview.

Timer and progress remain persistently discoverable.

---

# 09 — Radius, Borders & Elevation

## 09.1 Radius

```text
none  0
sm    6
md    8
lg    12
xl    16
full  999
```

Usage:

```text
Inputs/buttons   md
Cards            lg
Dialogs          lg/xl
Badges           full
Avatars          full
```

---

## 09.2 Borders

Standard:

1px.

Strong/focus:

2px only when necessary.

---

## 09.3 Elevation

### elevation.none

No shadow.

### elevation.sm

Raised card/sticky surface.

### elevation.md

Popover/dropdown.

### elevation.lg

Modal/drawer.

### elevation.xl

Exceptional floating layer.

---

## 09.4 Shadow Principle

Normal cards SHOULD rely primarily on:

```text
surface + border
```

not shadows.

---

# 10 — Icons, Imagery & Illustration

## 10.1 Icon Library

Primary:

> Lucide

Do not mix arbitrary icon libraries.

---

## 10.2 Icon Sizes

```text
small    16
default  20
large    24
```

Exceptional:

32px.

---

## 10.3 Icon Usage

Use when icons improve:

- scanning;
- recognition;
- compact control use.

Do not attach decorative icons to every heading.

---

## 10.4 Icon Buttons

Require:

- accessible label;
- focus state;
- tooltip for unfamiliar action.

---

## 10.5 Photography

Preferred:

- real school environments;
- facilities;
- authentic learning activities.

Avoid unrelated international stock photography where school-specific imagery is available.

---

## 10.6 Illustration

Use for:

- onboarding;
- empty states;
- public communications.

Do not dominate operational screens.

---

# 11 — Motion & Interaction

## 11.1 Motion Tokens

```text
fast        120ms
standard    180ms
slow        240ms
deliberate  300ms
```

---

## 11.2 Motion Purpose

Motion communicates:

- entry;
- exit;
- selection;
- state;
- hierarchy;
- progress.

---

## 11.3 Prohibited Motion

Avoid:

- bouncing controls;
- animated backgrounds;
- decorative looping motion;
- excessive parallax;
- long transitions.

---

## 11.4 Reduced Motion

Honor user preference.

Replace movement with simpler transitions.

---

# 12 — Accessibility

## 12.1 Target

WCAG 2.2 AA.

---

## 12.2 Keyboard

Every applicable interactive element MUST be keyboard-operable.

---

## 12.3 Focus

Visible focus is mandatory.

---

## 12.4 Semantic HTML

Prefer native semantic elements before ARIA.

---

## 12.5 Screen Readers

Components must expose:

- name;
- role;
- state;
- relationships.

---

## 12.6 Color

Never communicate meaning through color only.

---

## 12.7 Forms

Labels must be persistent and programmatically associated.

Errors must be associated with the affected control.

---

## 12.8 Modals

Must:

- move focus;
- trap focus;
- expose title;
- restore focus.

---

## 12.9 Tables

Must expose:

- header relationships;
- sort states;
- accessible actions.

---

## 12.10 Low-Resolution & Older Hardware

Avoid interfaces that depend on:

- huge canvases;
- heavy blur;
- GPU-intensive animation;
- large image backgrounds;
- complex visual effects.

Core functionality must remain usable on modest school hardware.

---

# 13 — Component Architecture

## 13.1 Dependency Hierarchy

```text
Tokens
  ↓
Primitives
  ↓
Core Components
  ↓
Composite Components
  ↓
Domain Components
  ↓
Patterns
  ↓
Page Templates
  ↓
Product Screens
```

---

## 13.2 Primitives

Examples:

- Text
- Icon
- Surface
- Divider
- Stack
- Grid

---

## 13.3 Core Components

Examples:

- Button
- Input
- Badge
- Dialog
- Tabs

---

## 13.4 Composite Components

Examples:

- Search field;
- filter toolbar;
- user selector;
- date range control.

---

## 13.5 Domain Components

Examples:

- QuestionCard;
- ScoreSummary;
- SyncIndicator;
- LearningGapIndicator.

---

## 13.6 Pattern

Examples:

- CRUD directory;
- exam shell;
- dashboard KPI area;
- account danger zone.

---

## 13.7 Page Template

Examples:

- directory;
- detail;
- dashboard;
- configuration;
- report.

---

# 14 — Interaction State System

Every applicable interactive component MUST define:

```text
default
hover
focus
active
selected
disabled
read-only
loading
error
success
```

---

## 14.1 Hover

Should indicate interactability without causing layout movement.

---

## 14.2 Focus

Must be more visible than hover.

---

## 14.3 Active

Indicates press or current interaction.

---

## 14.4 Disabled

Disabled controls must remain understandable.

Do not hide required contextual information through excessive opacity.

---

## 14.5 Read-Only

Read-only is distinct from disabled.

Users should still be able to read and often copy the value.

---

## 14.6 Loading

Loading controls prevent duplicate action while communicating progress.

---

# 15 — Core Component Specifications

Every reusable component follows:

```text
Purpose
Anatomy
Variants
Sizes
States
Behavior
Responsive behavior
Accessibility
Content rules
Do not use when
```

---

## 15.1 Button

### Purpose

Triggers an action.

### Anatomy

```text
Optional leading icon
Label
Optional trailing icon
Loading indicator
```

### Variants

```text
Primary
Secondary
Tertiary
Ghost
Danger
Link
```

### Sizes

```text
Compact 32
Default 40
Large 48
```

### States

All applicable interaction states.

### Behavior

Primary buttons represent the dominant action.

Only one primary action SHOULD exist inside a single action group.

### Responsive

Buttons may become full width on narrow screens when the action is primary and context benefits.

### Accessibility

Must expose accessible name.

### Content

Use action verbs.

### Do not use when

Navigation should be a link.

---

## 15.2 Icon Button

### Purpose

Compact familiar action.

### Anatomy

Icon only.

### Sizes

32 / 40 / 48.

### Accessibility

Accessible label mandatory.

Tooltip for unfamiliar meaning.

### Do not use when

The action is unfamiliar or requires explanatory text.

---

## 15.3 Link

### Purpose

Navigation or text-based action.

### States

Default, hover, focus, visited where relevant.

### Content

Use descriptive link labels.

Avoid “click here”.

---

## 15.4 Input

### Purpose

Single-line text entry.

### Anatomy

Label, control, optional icon, helper/error.

### Sizes

Default 40px.

Large 48px.

Compact 32px only in dense desktop UI.

### States

Default, hover, focus, disabled, read-only, error, success.

### Accessibility

Persistent label mandatory.

Placeholder is supplemental.

---

## 15.5 Password Input

Extends Input.

Supports:

- show/hide;
- password manager;
- requirement guidance;
- error feedback.

Do not block paste without explicit security justification.

---

## 15.6 Textarea

Default minimum height:

96px.

May auto-grow for content-writing workflows.

---

## 15.7 Select

Use for small known option sets.

Do not use for very large searchable datasets.

---

## 15.8 Combobox

Use for:

- student search;
- class search;
- large datasets;
- searchable options.

Must support keyboard navigation.

---

## 15.9 Checkbox

Use for independent selections.

Tri-state MAY be used for group selection.

---

## 15.10 Radio

Use when exactly one option is selected from a visible short set.

---

## 15.11 Switch

Use for immediate binary setting changes.

Do not use for actions requiring form submission unless the semantics remain clear.

---

## 15.12 Search

Anatomy:

```text
Search icon
Input
Clear action
Loading state
```

Must define:

- empty query;
- searching;
- results;
- no results;
- error.

---

## 15.13 Date Picker

Use calendar UI where visual date selection helps.

Allow direct keyboard entry where practical.

---

## 15.14 File Upload

Must expose:

- allowed types;
- size limit;
- selected file;
- progress;
- error;
- retry;
- remove.

---

## 15.15 Badge

Variants:

```text
Neutral
Brand
Info
Success
Warning
Danger
AI
```

Badge color always accompanies readable text.

---

## 15.16 Avatar

Variants:

- image;
- initials;
- fallback icon.

Sizes must be standardized.

---

## 15.17 Card

### Purpose

Represents one meaningful content grouping.

### Variants

```text
Default
Interactive
Selected
Metric
Status
```

### Do not use

As an automatic wrapper for every section.

---

## 15.18 Statistic

Anatomy:

```text
Label
Value
Optional delta
Optional description
```

Metrics should answer a useful question.

---

## 15.19 Alert

Variants:

```text
Info
Success
Warning
Error
```

Alerts remain inline to their context.

---

## 15.20 Toast

Use for transient feedback.

Do not use for information users must remember.

---

## 15.21 Banner

Use for persistent page/system-level conditions.

Examples:

- offline;
- service degradation;
- account action required.

---

## 15.22 Tooltip

Supplementary only.

Never hide critical information exclusively inside tooltips.

---

## 15.23 Dialog

Use for focused short workflows.

Long workflows should become pages or sheets.

---

## 15.24 Confirmation Dialog

Must state consequence.

Never use only:

```text
Are you sure?
```

---

## 15.25 Drawer / Sheet

Use for:

- filters;
- secondary details;
- mobile navigation;
- contextual tools.

---

## 15.26 Dropdown Menu

Use for secondary/overflow actions.

Destructive options appear separated when appropriate.

---

## 15.27 Tabs

Use for peer sections of one context.

Do not use tabs to hide sequential workflow stages.

---

## 15.28 Breadcrumbs

Use for meaningful hierarchy.

Do not show on shallow screens merely because the component exists.

---

## 15.29 Pagination

Must expose:

- current range;
- next/previous;
- total where known.

---

## 15.30 Stepper

Use for sequential multi-step processes.

Examples:

- assessment creation;
- promotion configuration.

---

## 15.31 Progress

Variants:

- determinate;
- indeterminate;
- multi-stage.

Status must remain understandable without color.

---

## 15.32 Skeleton

Should approximate actual layout.

---

## 15.33 Empty State

Anatomy:

```text
Optional visual
Title
Explanation
Primary next action
Optional secondary action
```

---

# 16 — Forms & Validation

## 16.1 Form Layout

Default:

labels above controls.

---

## 16.2 Required Indicator

Use a consistent convention globally.

---

## 16.3 Validation

### Inline

Use for field-level issues.

### Form summary

Use when multiple errors need attention after submission.

---

## 16.4 Validation Timing

Avoid aggressive errors while users are still typing.

Validate:

- after blur;
- after meaningful completion;
- after submit.

---

## 16.5 Async Validation

Show pending state.

Example:

```text
Checking admission number…
```

---

## 16.6 Disabled vs Read-Only

Disabled:

cannot interact.

Read-only:

visible, understandable, often copyable.

---

## 16.7 Form Sections

Long administrative forms must be divided into semantic groups.

---

## 16.8 Action Placement

Primary save action should remain predictable.

Avoid moving the main save action to inconsistent locations between modules.

---

## 16.9 Unsaved Changes

Warn when meaningful work could be lost.

---

## 16.10 Forced Password Change

Flow:

```text
Temporary credentials
→ Required password change
→ New password
→ Confirm password
→ Success
→ Dashboard
```

---

# 17 — Tables & Data-Dense Interfaces

## 17.1 Table System

Tables must support serious administration workflows.

Potential capabilities:

- sort;
- filter;
- search;
- select;
- bulk action;
- paginate;
- resize;
- pin;
- hide/show columns;
- export;
- virtualize.

---

## 17.2 Alignment

Text:

left.

Numbers:

right where comparison benefits.

Status:

consistent within table.

---

## 17.3 Column Width

Define sensible minimum and preferred widths.

Long-content columns may flex.

Identifiers should not consume excessive width.

---

## 17.4 Row Heights

```text
Comfortable 52
Default     44
Compact     36
```

---

## 17.5 Row Actions

Frequent action MAY be visible.

Secondary actions go into overflow.

---

## 17.6 Bulk Actions

Appear after selection.

The interface must communicate:

```text
7 students selected
```

---

## 17.7 Sticky Header

Use for long tables.

---

## 17.8 Long Content

Use:

- wrapping;
- truncation;
- details;
- tooltips only for supplemental access.

---

## 17.9 Empty

Differentiate:

```text
No data exists.
```

from:

```text
No records match your filters.
```

---

## 17.10 Loading

Use table skeleton rows.

Avoid page-level spinners when only the table is refreshing.

---

## 17.11 Mobile

Use deliberate transformation from §08.

---

# 18 — Dashboard Composition System

## 18.1 Dashboard Purpose

Dashboard answers:

> What needs my attention now?

---

## 18.2 Standard Composition Zones

```text
Page header
Critical/important alerts
Primary action / next tasks
Primary KPI group
Operational content
Trend/analysis
Recent activity
Secondary information
```

---

## 18.3 KPI Limits

Do not present 12 equal KPI cards.

Recommended top-level KPI count:

```text
3–5
```

depending on role.

---

## 18.4 KPI Hierarchy

Primary KPI:

larger.

Secondary metrics:

more compact.

---

## 18.5 Dashboard Filters

Global dashboard filters such as:

- academic session;
- term;
- date range;
- class

must clearly communicate their scope.

---

## 18.6 Student Dashboard

Comfortable density.

Focus:

- today;
- next assignment;
- exam;
- progress;
- recent result.

---

## 18.7 Teacher Dashboard

Default density.

Focus:

- today's classes;
- attendance;
- grading queue;
- upcoming assessments.

---

## 18.8 Parent Dashboard

Comfortable.

Focus:

- child;
- attendance;
- performance;
- alerts.

---

## 18.9 Principal Dashboard

Analytical.

Focus:

- trends;
- school-level exceptions;
- class comparison;
- attendance.

---

## 18.10 Super Admin Dashboard

Compact/default.

Focus:

- system health;
- users;
- sync;
- security;
- failures.

---

# 19 — Data Visualization

## 19.1 Approved Chart Types

- line;
- area;
- bar;
- stacked bar;
- donut;
- heatmap;
- sparkline;
- progress.

---

## 19.2 Line

Use for trends over time.

---

## 19.3 Bar

Use for categorical comparisons.

---

## 19.4 Donut

Use for simple proportions with few categories.

---

## 19.5 Prohibited

Avoid:

- 3D charts;
- decorative gauges;
- meaningless pies;
- rainbow charts.

---

## 19.6 Axis

Axes should be readable but visually restrained.

---

## 19.7 Grid Lines

Use subtle grid lines only where they aid comparison.

---

## 19.8 Tooltip

Chart tooltips show:

- category;
- value;
- context.

---

## 19.9 Empty Chart

Do not render empty axes.

Show a proper no-data state.

---

## 19.10 Accessibility

Provide textual summaries for critical charts.

---

# 20 — Academic Domain Patterns

## 20.1 Subject Identity

Subject components may include:

```text
Subject name
Class
Teacher
Status
Relevant progress
```

Do not depend on arbitrary permanent subject colors.

---

## 20.2 Topic

Topic pattern:

```text
Title
Subject
Sequence
Completion
Content count
Assessment link
```

---

## 20.3 Class

Class identity:

```text
Class name
Arm
Academic session
Student count
Class teacher
Status
```

---

## 20.4 Student Identity

Reusable StudentIdentity component:

```text
Avatar
Full name
Admission ID
Class
Status
```

---

## 20.5 Teacher Identity

```text
Avatar
Name
Staff ID
Subjects/classes
Status
```

---

## 20.6 Assessment Status

Canonical states:

```text
Draft
Scheduled
Available
In progress
Completed
Closed
Cancelled
```

---

## 20.7 Question Card

Displays:

```text
Question type
Question
Options/response area
Marks
Difficulty if applicable
Topic
Status
```

---

## 20.8 Question Bank Item

Includes:

- question preview;
- subject;
- topic;
- type;
- difficulty;
- marks;
- status;
- usage count;
- actions.

---

## 20.9 Score Summary

Displays:

```text
Score
Maximum
Percentage
Grade
Status
```

---

## 20.10 Performance Indicator

Should communicate:

- current value;
- trend;
- context.

Avoid simplistic red/green judgments where academic interpretation is nuanced.

---

## 20.11 Learning Gap Indicator

May contain:

```text
Topic
Evidence
Performance
Severity
Recommended action
```

Do not present AI inference as unquestionable fact.

---

## 20.12 Result Breakdown

Structure:

```text
Subject
Assessment components
Total
Grade
Comment/status
```

---

## 20.13 Approval Workflow

Reusable pattern:

```text
Draft
→ Submitted for review
→ Review required
→ Approved / Rejected
→ Published
```

Use where appropriate for:

- assessments;
- results;
- generated content;
- announcements.

---

# 21 — LMS Experience

## 21.1 Core Objects

- Course
- Subject
- Module
- Topic
- Lesson
- Resource
- Assignment
- Submission
- Feedback
- Progress

---

## 21.2 Course Page

Possible hierarchy:

```text
Course header
Overview
Modules
Assignments
Resources
Progress
Announcements
```

---

## 21.3 Learning Page

Prioritize:

- lesson;
- readable content;
- resources;
- progress;
- previous/next.

---

## 21.4 Assignment

States:

```text
Not started
Draft
Submitted
Late
Missing
Returned
Graded
```

---

## 21.5 Progress

Must include numeric/text context.

Never rely on progress-bar color alone.

---

# 22 — CBT & Examination Experience

## 22.1 Design Priorities

1. Clarity
2. Low cognitive load
3. Reliability
4. Exam status visibility
5. Submission safety
6. Interruption recovery

---

## 22.2 Desktop Exam Layout

Recommended:

```text
Header
  Exam title
  Timer
  Connection/save status

Question navigator | Question workspace

Footer/actions
```

---

## 22.3 Question States

```text
Unvisited
Visited
Answered
Unanswered
Flagged
Current
```

Use:

- color;
- icon/marker;
- shape or text where necessary.

---

## 22.4 Answer Option

States:

```text
Default
Hover
Focus
Selected
Disabled
```

Selection must be unmistakable.

---

## 22.5 Timer

Use tabular numbers.

Timer should remain calm until threshold.

Warning threshold and critical threshold should be configurable.

---

## 22.6 Save Status

```text
Saving…
Saved
Saved locally
Waiting to sync
Save failed
```

---

## 22.7 Navigation

Students must be able to:

- next;
- previous;
- jump to question;
- flag;
- review.

---

## 22.8 Mobile CBT

Header remains compact.

Question navigator moves to sheet/drawer.

Question text remains 16px minimum.

Answer targets remain large enough for touch.

---

## 22.9 Submission

Review screen shows:

```text
Total questions
Answered
Unanswered
Flagged
```

Final action is deliberately separated.

---

## 22.10 Confirmation

Example:

```text
Submit examination?

47 of 50 questions are answered.
3 questions remain unanswered.

After final submission, your answers cannot be changed.
```

---

## 22.11 Recovery

After crash/restart:

```text
Attempt detected
→ validate attempt
→ restore answers
→ show recovery confirmation
→ continue
```

---

# 23 — Authentication, Permissions & Security

## 23.1 Roles

- Student
- Parent
- Teacher
- Principal
- Super Admin

---

## 23.2 Account Provisioning

Staff and parents are provisioned administratively.

Self-registration is not part of the current account model.

---

## 23.3 Login

Keep login focused.

Contains:

- school identity;
- identifier;
- password;
- sign-in;
- support/recovery where permitted.

---

## 23.4 Account Status

```text
Active
Inactive
Pending
Suspended
Locked
Archived
```

---

## 23.5 Super Admin Actions

Must support:

- create account;
- deactivate;
- reactivate;
- reset password;
- inspect status;
- manage relationships.

---

## 23.6 Permission Matrix

Display human-readable permission meanings first.

Technical permission code may appear secondarily.

---

## 23.7 Security Events

Show:

```text
Event
Actor
Time
Outcome
Relevant context
```

---

## 23.8 Danger Zone

Separate high-risk actions.

Examples:

- deactivate;
- terminate session;
- revoke access;
- reset credentials.

---

# 24 — Offline & Synchronization Experience

## 24.1 States

```text
Online
Offline
Saved locally
Pending sync
Syncing
Synced
Stale
Conflict
Failed
```

---

## 24.2 Offline Banner

Example:

```text
You're offline. Changes will be saved on this device and synced when a connection becomes available.
```

---

## 24.3 Stale Data

Show:

```text
Last synced 2 hours ago.
```

when relevant.

---

## 24.4 Conflict Resolution

Must expose understandable differences.

Example:

```text
Local version
Server version
Last modified
Resolution
```

---

## 24.5 Sync Queue

Advanced administrative view may display:

- operation;
- record;
- status;
- attempts;
- last error;
- retry.

---

## 24.6 Watchdog

Super Admin may inspect:

- monitored services;
- last heartbeat;
- recovery actions;
- current health.

---

# 25 — AI Experience

## 25.1 Principle

AI assists.

AI does not obscure responsibility.

---

## 25.2 AI Components

- Prompt field
- Assistant panel
- Response
- Suggestion
- Generated label
- Source
- Model status
- Processing
- Retry
- Accept
- Edit
- Reject

---

## 25.3 Local AI Status

```text
Ready
Loading
Processing
Unavailable
Resource constrained
Error
```

---

## 25.4 AI-Generated Content

Generated content should remain editable before publication where applicable.

---

## 25.5 Provenance

Where possible, identify relevant sources used by AI.

---

## 25.6 Consequential Data

AI should not silently finalize:

- grades;
- disciplinary records;
- security changes;
- account changes;
- published institutional communication.

Human confirmation is required.

---

# 26 — Role-Based Experience Guidance

## 26.1 Student

Prioritize:

- learning;
- practice;
- assignments;
- CBT;
- progress;
- feedback.

Density:

comfortable.

---

## 26.2 Teacher

Prioritize:

- classes;
- attendance;
- students;
- questions;
- assessments;
- marking;
- results;
- learning gaps.

Density:

default.

---

## 26.3 Parent

Prioritize:

- selected child;
- attendance;
- performance;
- results;
- learning gaps;
- announcements.

Density:

comfortable.

---

## 26.4 Principal

Prioritize:

- performance;
- attendance;
- trends;
- comparisons;
- institutional intelligence.

Density:

default / analytical.

---

## 26.5 Super Admin

Prioritize:

- users;
- academic configuration;
- system health;
- offline sync;
- security;
- diagnostics;
- logs.

Density:

default / compact.

---

# 27 — Local Server / Operations Manager

## 27.1 Purpose

The local school server manager is a technical operations interface.

It uses the same foundation but supports higher information density.

---

## 27.2 Server Overview

Display:

```text
Server status
Uptime
CPU
Memory
Storage
Database
Network
Sync
AI
Connected devices
```

---

## 27.3 Service Status

Each service exposes:

```text
Service
State
Last heartbeat
Version
Relevant action
```

Canonical states:

```text
Operational
Degraded
Offline
Failed
Maintenance
Unknown
```

---

## 27.4 Database Health

Potential data:

- connectivity;
- storage usage;
- backup status;
- last successful backup;
- error state.

---

## 27.5 AI Health

Display:

- model loaded;
- model version;
- runtime status;
- memory usage where useful;
- error.

---

## 27.6 Connected Devices

Data table may include:

- device;
- user/role;
- IP/network context where allowed;
- last seen;
- sync state.

---

## 27.7 Diagnostics

Use expandable technical detail.

Do not show raw logs as the first-level experience.

---

## 27.8 Logs

Compact monospaced view.

Support:

- severity;
- time;
- source;
- filter;
- search;
- copy;
- export where appropriate.

---

## 27.9 Backups

Display:

```text
Last successful
Next scheduled
Status
Size
Restore availability
```

Restore is a high-risk flow requiring confirmation.

---

# 28 — Feedback, Notifications & System States

## 28.1 Toast

Use when:

- outcome is transient;
- user does not need to retain message.

---

## 28.2 Inline Alert

Use for local context.

---

## 28.3 Banner

Use for persistent broad conditions.

---

## 28.4 Modal

Use when user decision is required before proceeding.

---

## 28.5 Confirmation Dialog

Use for consequential actions.

---

## 28.6 Loading States

### Skeleton

Use for structured initial loading.

### Spinner

Use for small isolated waits.

### Progress

Use for deterministic work.

### Page loading

Avoid full-page blocking unless the whole screen truly depends on it.

---

## 28.7 Empty States

Canonical categories:

```text
No data yet
No search results
No assignments
No questions
No analytics
No notifications
```

---

## 28.8 Error Categories

```text
Validation
Network
Server
Permission
Authentication
Sync
System
```

---

## 28.9 Success States

```text
Saved
Submitted
Imported
Approved
Synced
Completed
```

---

# 29 — Content & UX Writing

## 29.1 Voice

- clear;
- direct;
- respectful;
- calm;
- professional.

---

## 29.2 Buttons

Specific verbs.

Good:

```text
Save changes
Create assessment
Mark attendance
Submit examination
```

Avoid generic wording when specific wording exists.

---

## 29.3 Errors

Pattern:

```text
What happened.
What the user should do.
```

---

## 29.4 Confirmation

State consequence.

---

## 29.5 Empty States

Explain:

```text
What is missing
Why it may be missing
What the user can do
```

---

## 29.6 Tooltips

Short explanatory phrases.

Do not write paragraphs inside tooltips.

---

# 30 — Website Scope Boundary

This design system covers the CRC LMS / Management System. Public school website guidance is outside the current CRC product scope and does not establish website requirements here.

The NAKAMA corporate website is a separate future project with its own design system. Corporate homepages, service catalogs, pricing, marketing, blogs/resources, and lead-generation pages are outside this repository.

---

# 31 — Print & Reporting

## 31.1 Formats

Support:

- A4;
- grayscale;
- low-quality printers.

---

## 31.2 Print Removes

Do not print:

- navigation;
- controls;
- toolbars;
- irrelevant filters.

---

## 31.3 Report Card Structure

```text
Institution
Student
Session / term
Class
Subject breakdown
Totals / grades
Attendance
Comments
Approvals/signatures where required
Generated date
```

---

## 31.4 Long Tables

Repeat table headers across pages where technically supported.

---

# 32 — Representative Screen Blueprints

These screen blueprints demonstrate composition. They do not create new visual rules.

---

## 32.1 Student Dashboard

```text
App Shell
└── Page Header
    ├── Greeting/context
    └── Notifications

Primary Area
├── Today's timetable
├── Next assignment
└── Upcoming examination

Secondary
├── Subject progress
├── Recent results
└── Announcements
```

Density:

comfortable.

---

## 32.2 Student Practice

```text
Practice header
Topic / subject context
Question workspace
Answer controls
Feedback
Progress
Next action
```

Do not overload with analytics during the active question.

---

## 32.3 CBT Interface

```text
Exam bar
├── Exam title
├── Timer
├── Save/connection status

Question navigator
Question workspace
Answer controls
Question actions
Exam actions
```

---

## 32.4 CBT Completion

```text
Submission confirmed
Reference/time
Next instruction
```

Results appear only if examination policy permits.

---

## 32.5 Teacher Dashboard

```text
Today's classes
Attendance actions
Grading queue
Upcoming assessments
Recent class activity
```

---

## 32.6 Question Bank

```text
Page header
Create/import actions
Search/filter toolbar
Question table
Bulk actions
Pagination
```

Filters:

- subject;
- topic;
- type;
- difficulty;
- status.

---

## 32.7 Assessment Configuration

Prefer stepper:

```text
1 Details
2 Questions
3 Rules
4 Schedule
5 Review
6 Publish
```

---

## 32.8 Parent Dashboard

```text
Child selector
Performance summary
Attendance
Recent results
Learning gaps
Announcements
```

---

## 32.9 Principal Dashboard

```text
Term/session filter
Priority alerts
Performance KPIs
Trend charts
Class comparison
Attendance
Intervention areas
```

---

## 32.10 Super Admin Dashboard

```text
System alerts
Health KPIs
User/account issues
Sync status
Failed jobs
Security events
Recent audit activity
```

---

## 32.11 User Management

```text
Header
Create account
Filters/search
Data table
Bulk action
Row actions
Account detail drawer/page
```

---

## 32.12 Sync Monitoring

```text
Overall sync health
Pending
Failed
Conflict

Queue table

Service detail
Watchdog
Last successful cycle
```

---

## 32.13 Local Server Manager

```text
Server status
Resource utilization
Services
Database
Sync
AI
Connected devices
Backups
Logs
```

---

## 32.14 Authentication

```text
School identity
Sign-in title
Identifier
Password
Primary sign in
Support/recovery
Minimal footer
```

---

## 32.15 State Examples

### Loading

Skeleton matching content.

### Empty

Explanation + relevant action.

### Error

Issue + recovery action.

### Permission

Explanation without revealing restricted data.

---

# 33 — Design-to-Code Architecture

## 33.1 Target Repository

```text
CRC/
├── apps/
│   ├── web/
│   └── offline/
│
├── packages/
│   ├── design-tokens/
│   ├── ui/
│   └── icons/
│
├── design-system/
│   └── DESIGN_SYSTEM.md
│
├── docs/
└── modules/
```

---

## 33.2 Design Tokens Package

Contains:

```text
color
typography
spacing
sizing
radius
border
shadow
motion
breakpoints
z-index
```

---

## 33.3 UI Package

Suggested:

```text
ui/
├── primitives/
├── actions/
├── forms/
├── navigation/
├── feedback/
├── overlays/
├── data-display/
├── academic/
├── assessment/
├── offline/
└── system/
```

---

## 33.4 Naming

Components use semantic names.

Good:

```text
Button
DataTable
QuestionCard
QuestionNavigator
ScoreSummary
SyncIndicator
```

Bad:

```text
BlueButton
BigCard
MainActionButton
```

---

## 33.5 CSS Variables

Example:

```css
:root {
  --color-brand-primary: #1d4ed8;

  --color-text-primary: #0f172a;
  --color-text-secondary: #475569;

  --color-surface-page: #f8fafc;
  --color-surface-primary: #ffffff;

  --color-border-default: #e2e8f0;

  --space-1: 0.25rem;
  --space-2: 0.5rem;
  --space-4: 1rem;

  --radius-md: 0.5rem;
  --radius-lg: 0.75rem;
}
```

---

## 33.6 Tailwind

Prefer semantic mappings.

Good:

```text
bg-surface-primary
text-primary
border-default
bg-action-primary
```

Avoid:

```text
bg-[#1D4ED8]
mt-[17px]
rounded-[11px]
```

---

## 33.7 Component Ownership

Application screens compose shared UI.

They do not rewrite primitives.

---

## 33.8 Offline App

Offline and online applications should consume the same design-system packages.

---

# 34 — Figma Architecture & Handoff

## 34.1 Pages

```text
00 Cover
01 Foundations
02 Variables
03 Primitives
04 Components
05 Patterns
06 Templates
07 Student
08 Parent
09 Teacher
10 Principal
11 Super Admin
12 LMS
13 CBT
14 AI
15 Offline
16 Local Server
17 Archive
```

---

## 34.2 Variables

Collections:

```text
Primitive Color
Semantic Color
Spacing
Sizing
Radius
Typography
Motion
```

---

## 34.3 Components

Use:

- auto layout;
- variants;
- properties;
- variables.

---

## 34.4 Naming

Figma and code SHOULD use closely aligned concepts.

---

## 34.5 Design Handoff

A completed design should identify:

- component names;
- states;
- responsive behavior;
- relevant tokens;
- interaction behavior.

---

# 35 — Testing & Consistency Audit

## 35.1 Component Testing

Shared components require:

- behavior tests;
- accessibility tests;
- state tests;
- responsive tests;
- visual regression.

---

## 35.2 Visual Regression

Cover at minimum:

- primary components;
- forms;
- table states;
- dialogs;
- navigation;
- offline banner;
- CBT;
- dashboard primitives.

---

## 35.3 Content Stress Test

Test:

- long names;
- long subject titles;
- large counts;
- empty data;
- thousands of rows;
- missing images;
- multi-line validation.

---

## 35.4 Low-End Environment Test

Test representative interfaces on:

- low-resolution viewport;
- slower CPU profile where feasible;
- weak/offline network;
- reduced motion.

---

## 35.5 Final Consistency Audit

Before a major release, verify:

### Tokens

- [ ] All production colors are tokenized.
- [ ] Typography values are tokenized.
- [ ] Spacing values are controlled.
- [ ] Radius values are controlled.
- [ ] Shadows follow elevation tokens.

### Components

- [ ] Duplicate buttons do not exist.
- [ ] Duplicate form controls do not exist.
- [ ] Badge variants are justified.
- [ ] Modal variants are justified.
- [ ] Domain components reuse foundations.

### Interaction

- [ ] Hover defined.
- [ ] Focus defined.
- [ ] Disabled defined.
- [ ] Loading defined where relevant.
- [ ] Error defined where relevant.

### Responsive

- [ ] Navigation transforms correctly.
- [ ] Tables remain usable.
- [ ] Forms reflow.
- [ ] Charts remain readable.
- [ ] CBT remains usable.

### Accessibility

- [ ] Keyboard flows work.
- [ ] Contrast passes.
- [ ] Screen-reader labels exist.
- [ ] Focus is visible.
- [ ] Color is not the only state cue.

### Product Consistency

- [ ] Student remains visually related to Teacher.
- [ ] Teacher remains visually related to Admin.
- [ ] CBT still belongs to the same product.
- [ ] Local Server UI still belongs to CRC.
- [ ] AI styling is integrated.
- [ ] No unexplained visual exceptions exist.

---

# 36 — Governance, Versioning & Contribution

## 36.1 New Component Process

```text
Need identified
→ Existing component evaluated
→ Reuse attempted
→ Existing variant considered
→ New component justified
→ Design review
→ Accessibility review
→ Documentation
→ Implementation
→ Testing
```

---

## 36.2 Variant Rule

A new variant must answer:

> What meaningful product requirement does this solve?

If the answer is only:

> It looks better on this screen.

the variant should not be added.

---

## 36.3 Token Change Process

Document:

- reason;
- affected token;
- affected components;
- compatibility impact;
- migration.

---

## 36.4 Semantic Versioning

```text
MAJOR.MINOR.PATCH
```

### MAJOR

Breaking design-system change.

### MINOR

Backward-compatible addition.

### PATCH

Correction or clarification.

---

## 36.5 Deprecation

Process:

```text
Mark deprecated
→ document replacement
→ migrate usage
→ verify no consumers
→ remove
```

---

## 36.6 One-Off Rule

A screen-specific visual solution used more than once should be reviewed for promotion into:

- component;
- variant;
- pattern.

---

## 36.7 AI Governance

AI-generated code MUST NOT silently introduce canonical UI standards.

If a missing rule is discovered:

1. reuse existing patterns where possible;
2. make the smallest reversible decision;
3. flag the missing standard;
4. update this file through review if the rule becomes canonical.

---

# 37 — Non-Negotiable Rules

1. One visual language across all CRC product experiences.
2. Semantic tokens over raw values.
3. No arbitrary production colors.
4. No arbitrary typography.
5. No arbitrary spacing.
6. No arbitrary radii.
7. One primary icon system.
8. Accessibility is mandatory.
9. Keyboard focus remains visible.
10. Status never depends only on color.
11. Components require relevant interaction states.
12. Responsive behavior must be deliberate.
13. Offline behavior must be intentional.
14. AI remains visually integrated.
15. Student interfaces must not become childish.
16. Administration interfaces must not become unreadably dense.
17. The NAKAMA corporate website and its design system remain outside CRC scope.
18. Cards represent meaningful groups.
19. Borders are preferred to unnecessary shadows.
20. Gradients are exceptional.
21. Glassmorphism is not the product language.
22. Critical information does not live exclusively inside tooltips.
23. Important actions cannot require hover discovery.
24. Error messages should be actionable.
25. Long realistic content must be tested.
26. Feature teams reuse shared components.
27. Variants require product justification.
28. Low-end hardware is a supported design constraint.
29. Figma and implementation should use aligned terminology.
30. This document remains the single design-system source of truth.

---

# 38 — Acceptance Criteria

The design system is considered implementation-ready when a new designer or engineer can create a new feature without inventing:

- colors;
- fonts;
- font sizes;
- spacing;
- radius;
- shadows;
- component states;
- layout conventions;
- responsive rules;
- status patterns.

A new contributor should be able to determine:

- which component to use;
- what its variants mean;
- how it responds;
- how it behaves offline;
- how it handles errors;
- how it remains accessible;
- how it maps into code.

If repeated clarification is necessary for fundamental visual decisions, this specification must be improved.

---

# 39 — Changelog

## 2.0.0 — September 2026

Major design-system refinement.

Added:

- formal design audit;
- stronger design principles;
- brand foundation;
- stricter semantic token model;
- responsive transformation rules;
- standardized component specification model;
- component dependency hierarchy;
- dashboard composition rules;
- richer academic components;
- learning-gap patterns;
- question-bank patterns;
- approval workflows;
- local server manager design;
- low-end hardware requirements;
- CBT mobile behavior;
- representative product screens;
- comprehensive consistency audit;
- stronger AI-agent instructions;
- stronger governance.

Refined:

- color;
- typography;
- spacing;
- layout;
- components;
- forms;
- tables;
- dashboards;
- charts;
- LMS;
- CBT;
- offline;
- AI;
- role-specific UX;
- design-to-code architecture.

---

# End of Canonical Design System

**Authoritative path**

```text
design-system/DESIGN_SYSTEM.md
```

Any product UI rule outside this file must either conform to this specification or be formally incorporated through the governance process.