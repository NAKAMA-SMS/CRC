# Analytics Architecture

## 1. Purpose

This document defines the architecture for CRC's analytics and learning-intelligence subsystem.

The analytics subsystem transforms authoritative academic activity into reliable, explainable, role-scoped insights.

Analytics supports:

* student progress tracking;
* practice performance;
* assessment performance;
* subject performance;
* topic performance;
* learning-gap identification;
* teacher class intelligence;
* parent progress visibility;
* principal school-level intelligence;
* academic trend analysis;
* intervention support.

Analytics is a **derived intelligence layer**.

It does not replace authoritative academic records.

---

# 2. Scope

This document covers:

* analytics architecture;
* source data;
* analytical processing;
* metrics;
* aggregations;
* student analytics;
* teacher analytics;
* parent analytics;
* principal analytics;
* learning-gap detection;
* performance trends;
* assessment analytics;
* practice analytics;
* topic analytics;
* class analytics;
* subject analytics;
* role-based access;
* privacy;
* derived data;
* historical snapshots;
* dashboard data;
* analytics refresh;
* offline analytics;
* synchronization;
* performance;
* testing;
* reliability.

It does not define:

* deterministic assessment scoring;
* authentication;
* synchronization protocol details;
* AI model architecture;
* content processing;
* question-generation logic.

---

# 3. Analytics Principle

The analytics system follows:

```text id="5s6m4x"
Authoritative Academic Data
          ↓
     Analytics Pipeline
          ↓
      Derived Metrics
          ↓
    Learning Intelligence
          ↓
     Role-Specific Views
```

The analytical layer must never overwrite authoritative academic records.

---

# 4. Authoritative vs Derived Data

Authoritative data includes:

* students;
* classes;
* arms;
* subjects;
* topics;
* questions;
* assessments;
* attempts;
* answers;
* scores;
* results;
* practice submissions;
* teacher assignments.

Derived analytics may include:

* averages;
* percentages;
* trends;
* mastery indicators;
* learning gaps;
* rankings where explicitly permitted;
* class summaries;
* subject summaries;
* intervention indicators.

If derived analytics are deleted, they must be reproducible from authoritative data.

---

# 5. Analytics Sources

Analytics may consume:

```text id="y8x2s7"
Student Activity
Assessment Attempts
Practice Attempts
Question Results
Topic Mapping
Subject Mapping
Class / Arm Placement
Teacher Assignments
Academic Sessions
Academic Terms
Promotion History
```

The system must preserve the relationship between analytical results and their source records.

---

# 6. Event and Record Model

The analytics pipeline should consume stable academic records rather than depending exclusively on UI events.

For example:

```text id="2m9q1k"
Assessment Attempt
       ↓
Finalized Result
       ↓
Analytics Processing
```

An incomplete attempt should not automatically become an official performance result.

---

# 7. Assessment Analytics

Assessment analytics should support:

* score;
* percentage;
* completion;
* attempt status;
* question accuracy;
* subject performance;
* topic performance;
* time-related metrics where available;
* attempt history;
* assessment trends.

The analytics system must use the authoritative assessment result produced by the assessment engine.

---

# 8. Practice Analytics

Practice analytics should support:

* practice attempts;
* questions attempted;
* questions answered correctly;
* accuracy;
* topics practiced;
* improvement over time;
* repeated mistakes;
* practice frequency;
* practice completion.

Practice analytics must remain distinct from official examination results.

---

# 9. Student Progress

Student progress should be calculated across appropriate academic dimensions.

Examples:

```text id="h4f2w8"
Student
 ├── Subject
 │     ├── Topic
 │     │     ├── Practice
 │     │     └── Assessment
 │     └── Overall Performance
 └── Historical Trend
```

The system should allow users to understand both current performance and historical development.

---

# 10. Student Analytics

The student dashboard may provide:

* current term performance;
* subject performance;
* topic performance;
* practice history;
* assessment history;
* weak areas;
* improvement areas;
* recent activity;
* learning recommendations.

Students must only see their own authorized data.

---

# 11. Parent Analytics

Parent analytics may provide:

* child's current performance;
* subject progress;
* topic gaps;
* practice activity;
* assessment history;
* trend information.

A parent account may access only explicitly related students.

The system must not infer parent relationships from names, classes, or other attributes.

---

# 12. Teacher Analytics

Teacher analytics may provide information for assigned classes and subjects.

Examples:

* class performance;
* subject performance;
* topic gaps;
* student performance;
* assessment outcomes;
* practice activity;
* students requiring attention;
* improvement trends.

Teacher scope is controlled by teacher assignments.

---

# 13. Principal Analytics

Principal analytics operate at management level.

Examples:

* school performance;
* class performance comparison;
* subject performance;
* topic-level learning gaps;
* academic trends;
* assessment outcomes;
* intervention areas;
* term/session summaries.

Principal access must respect the school's configured organizational scope.

---

# 14. Super Admin Analytics

Super Admin analytics should focus primarily on system-wide operational and academic administration.

Examples:

* data processing health;
* analytics pipeline health;
* synchronization state;
* school configuration;
* aggregate academic information where authorized.

The Super Admin role does not automatically gain unrestricted access to private user conversations or unrelated personal data.

---

# 15. Academic Dimensions

Analytics should support these dimensions:

```text id="2z7p4d"
Session
   ↓
Term
   ↓
Class
   ↓
Arm
   ↓
Subject
   ↓
Topic
```

Student-level analysis may be added beneath the academic hierarchy.

All analytical records must retain sufficient dimensional information to reproduce the applicable scope.

---

# 16. Time Dimensions

Analytics should support:

* session;
* term;
* date;
* week;
* month;
* assessment date;
* practice date.

The system should avoid mixing different academic sessions without explicitly identifying the period.

Historical results must remain associated with their original academic session and term.

---

# 17. Metric Definitions

Every important analytical metric must have a documented definition.

A metric should specify:

* name;
* purpose;
* formula;
* source data;
* population;
* exclusions;
* time window;
* refresh behavior;
* authorization scope.

This prevents different dashboards from calculating the same metric differently.

---

# 18. Accuracy

Where appropriate:

```text
Accuracy =
Correct Answers / Answered Questions × 100
```

The exact metric must use the assessment engine's authoritative answer and scoring records.

Invalid, cancelled, or excluded questions must follow the assessment rules rather than being silently included.

---

# 19. Completion Rate

Where applicable:

```text id="k0x5j6"
Completion Rate =
Completed Activities / Eligible Activities × 100
```

The definition of "eligible" must be explicitly determined by the relevant academic workflow.

---

# 20. Performance Aggregation

Aggregations must identify their population.

For example:

```text id="n7k4e2"
Student Average
Class Average
Arm Average
Subject Average
Topic Average
School Average
```

These values must not be treated as interchangeable.

---

# 21. Weighted vs Unweighted Metrics

Where multiple assessments are combined, the system must explicitly define whether aggregation is:

* weighted;
* unweighted;
* activity-count based;
* question-count based.

Analytics must not accidentally give disproportionate influence to assessments simply because they contain more questions unless that is the intended rule.

---

# 22. Historical Integrity

Historical analytics must remain reproducible.

If a student moves from:

```text
JSS1 A → JSS2 B
```

previous results remain associated with JSS1 A.

Historical analytics must not be rewritten based on the student's current placement.

---

# 23. Promotion and Analytics

Promotion changes a student's future academic placement.

It must not rewrite historical analytics.

The analytics layer must retain:

* original class;
* original arm;
* session;
* term;
* subject;
* topic;
* assessment context.

---

# 24. Learning Gaps

Learning gaps identify areas where available evidence indicates that additional learning support may be useful.

A learning gap should be based on multiple relevant signals where possible.

Potential signals include:

* low assessment accuracy;
* repeated incorrect answers;
* low topic performance;
* repeated practice errors;
* insufficient practice;
* persistent weakness over time.

A single incorrect answer should not automatically create a major learning-gap classification.

---

# 25. Learning-Gap Model

Conceptually:

```text id="x7d3k9"
Question Results
      ↓
Topic Aggregation
      ↓
Subject Aggregation
      ↓
Trend Analysis
      ↓
Learning-Gap Detection
```

The exact algorithm must be deterministic and versioned.

---

# 26. Learning-Gap Evidence

Each learning-gap result should be traceable to evidence.

Example:

```text id="m1p8q5"
Learning Gap
 ├── Subject
 ├── Topic
 ├── Time Window
 ├── Evidence Count
 ├── Accuracy
 ├── Trend
 └── Algorithm Version
```

This makes analytical conclusions explainable.

---

# 27. Learning-Gap Confidence

Where a confidence or strength indicator is used, it must be based on defined evidence.

For example, a topic with:

```text
2 attempts / 1 wrong
```

should not necessarily carry the same analytical confidence as:

```text
30 attempts / 18 wrong
```

The system should distinguish insufficient evidence from persistent weakness.

---

# 28. Learning-Gap States

A practical model may include:

```text id="a6j3v8"
Insufficient Evidence
      ↓
Emerging Concern
      ↓
Persistent Gap
      ↓
Improving
```

The exact thresholds must be configuration-driven and documented.

These are analytical classifications, not permanent labels attached to students.

---

# 29. Improvement Detection

The system should detect positive changes where sufficient evidence exists.

Examples:

* increasing topic accuracy;
* reduced repeated errors;
* improved assessment performance;
* increased practice consistency.

Improvement should be based on comparable time windows or defined trend logic.

---

# 30. Trend Analysis

Trend calculations must define:

* baseline;
* comparison period;
* minimum sample size;
* aggregation method;
* missing-data behavior.

The system must not create misleading trends from insufficient data.

---

# 31. Missing Data

Missing activity does not automatically mean poor performance.

The analytics system must distinguish:

```text id="v3c8p2"
No Evidence
       ≠
Poor Performance
```

For example, a student who has never practiced a topic should not automatically be classified as failing that topic.

---

# 32. Question-Level Analytics

Question analytics may include:

* attempts;
* correct rate;
* incorrect rate;
* skip rate;
* average response behavior where available;
* topic;
* subject;
* assessment context.

Question analytics must respect question versioning.

If a question changes substantially, analytics should not silently combine unrelated versions.

---

# 33. Question Versioning

Question analytics should identify the relevant question version.

```text id="b6n9r1"
Question
  ↓
Question Version
  ↓
Assessment Usage
  ↓
Result
```

Historical results must remain linked to the version used during the attempt.

---

# 34. Subject Analytics

Subject analytics should aggregate relevant topic and assessment data.

Possible views:

* average performance;
* assessment performance;
* practice activity;
* topic gaps;
* improvement;
* student distribution.

Subject analytics must respect the applicable academic session and term.

---

# 35. Topic Analytics

Topic analytics provide more granular academic intelligence.

They should support:

* accuracy;
* attempts;
* assessment performance;
* practice performance;
* learning-gap state;
* trend;
* evidence count.

Topic identifiers must remain stable enough for historical analysis.

---

# 36. Class Analytics

Class analytics should aggregate students assigned to a specific class/arm within a defined academic period.

For example:

```text id="k7r4x0"
JSS1
  └── Arm A
       ├── Mathematics
       ├── English
       └── Basic Science
```

Class analytics must not accidentally combine students from different arms.

---

# 37. Arm Analytics

Arms must be treated as configurable academic entities.

Analytics cannot hardcode:

```text
A
B
C
```

as the only possible values.

Any configured arm must be analyzable.

---

# 38. School Analytics

School-level analytics aggregate authorized academic data across classes and arms.

School analytics should support:

* overall performance;
* subject performance;
* topic gaps;
* term trends;
* class-level comparisons;
* intervention indicators.

Raw student-level information should not be exposed unnecessarily in management views.

---

# 39. Comparison Analytics

Comparison views may include:

* class vs class;
* arm vs arm;
* subject vs subject;
* current period vs previous period.

Comparisons must clearly identify:

* population;
* period;
* metric;
* sample size.

---

# 40. Analytics Privacy

Analytics can reveal sensitive information even when individual records are not displayed.

Therefore:

* authorization is mandatory;
* aggregation must respect role scope;
* unnecessary personal data should be excluded;
* exports must be controlled;
* API responses must use least privilege.

---

# 41. Small-Population Protection

Where aggregated analytics could reveal an individual student's result indirectly, the system SHOULD apply appropriate disclosure controls.

This is particularly important for:

* very small groups;
* filtered dashboards;
* teacher/private datasets;
* principal reports.

The exact threshold should be configurable if such protection is required.

---

# 42. Student Data Access

Students can access:

* their own performance;
* their own practice history;
* their own learning gaps;
* their own progress.

They cannot access another student's analytics.

---

# 43. Parent Data Access

Parents can access only the analytics belonging to their explicitly related students.

A parent with multiple children should have separate child contexts.

The system must not merge unrelated students into a single inferred family record.

---

# 44. Teacher Data Access

Teachers can access analytics for students and academic areas covered by their assignments.

Teacher access should be evaluated server-side.

A modified frontend request must not expand teacher analytics scope.

---

# 45. Principal Data Access

Principal analytics should provide management-level visibility across the school.

The principal may access aggregated academic information across classes and subjects according to school policy and system permissions.

---

# 46. Analytics API

Analytics APIs should use explicit resources.

Examples:

```text
GET /api/v1/students/{studentId}/analytics
GET /api/v1/students/{studentId}/progress
GET /api/v1/students/{studentId}/learning-gaps

GET /api/v1/classes/{classId}/analytics
GET /api/v1/subjects/{subjectId}/analytics
GET /api/v1/topics/{topicId}/analytics

GET /api/v1/teachers/{teacherId}/analytics
GET /api/v1/principal/analytics
```

All endpoints require authorization.

---

# 47. Dashboard Read Models

Dashboards should not repeatedly execute expensive analytical queries against transactional tables.

The system may use dedicated analytical read models.

```text id="y4c8n2"
Transactional Data
       ↓
Analytics Processor
       ↓
Analytics Read Models
       ↓
Dashboards
```

This improves performance and protects core LMS operations.

---

# 48. Derived Analytics Storage

Derived analytics may be stored in:

* relational summary tables;
* materialized views;
* analytical read models;
* cached aggregates.

The chosen mechanism must remain rebuildable from authoritative data.

---

# 49. Incremental Processing

The analytics pipeline SHOULD support incremental processing.

When a new finalized result arrives:

```text id="z5p3m7"
New Result
   ↓
Identify Affected Dimensions
   ↓
Update Derived Metrics
   ↓
Invalidate Relevant Caches
```

This avoids rebuilding all analytics after every activity.

---

# 50. Full Rebuild

The system must support a full analytics rebuild.

Use cases include:

* algorithm changes;
* migration;
* corruption recovery;
* data correction;
* new metric introduction.

A rebuild must be versioned and observable.

---

# 51. Analytics Versioning

Analytical algorithms must be versioned.

For example:

```text id="p8x1k6"
Learning Gap Algorithm v1
Learning Gap Algorithm v2
```

Historical derived values should be identifiable by algorithm version where required.

---

# 52. Data Corrections

When authoritative academic data is corrected:

```text id="e7v2c9"
Corrected Source Data
       ↓
Affected Analytics Identified
       ↓
Derived Metrics Recomputed
```

The system must not manually patch derived metrics without updating their source relationship.

---

# 53. Offline Analytics

The local school system should provide sufficient analytics for offline academic operation.

Local analytics may include:

* student progress;
* practice history;
* assessment results;
* topic performance;
* teacher class performance;
* local learning gaps.

The exact local analytics scope should be determined by operational requirements and available local data.

---

# 54. Online Analytics

The online/master system provides broader synchronized analytics.

It may aggregate:

* school-level data;
* synchronized assessment results;
* historical data;
* management dashboards.

Online analytics must respect synchronization state.

Unresolved or incomplete synchronization must not be silently presented as complete data.

---

# 55. Analytics and Synchronization

Analytics data is generally derived.

The preferred synchronization pattern is:

```text id="j2f8w4"
Authoritative Source Data
       ↓
Synchronization
       ↓
Analytics Recalculation
```

Rather than treating every derived metric as independently authoritative.

This reduces conflict risk.

---

# 56. Analytics Sync State

Dashboards should be able to distinguish:

* current;
* processing;
* partially synchronized;
* stale;
* unavailable.

For example:

```text
Last Updated:
27 Sep 2026, 14:30

Data Status:
Synchronized
```

The timestamp should represent the relevant analytical dataset rather than merely the user's page load time.

---

# 57. Analytics Freshness

Every analytical read model should have a known refresh state.

Possible states:

```text
Current
Refreshing
Stale
Failed
Unavailable
```

A stale result must not be presented as real-time data.

---

# 58. Performance

Analytics queries should use:

* indexed dimensions;
* precomputed aggregates;
* pagination;
* bounded date ranges;
* efficient filtering;
* read models.

Expensive calculations should run asynchronously.

---

# 59. Dashboard Caching

Dashboard data may be cached where appropriate.

Cache keys should include relevant scope such as:

```text
user
role
session
term
class
arm
subject
topic
```

Authorization must be evaluated independently of cache lookup.

A cached response must never bypass authorization.

---

# 60. Analytics Jobs

Background analytics jobs may include:

* incremental aggregation;
* learning-gap computation;
* trend calculation;
* full rebuild;
* cache refresh;
* historical snapshot generation.

Jobs should be:

* retryable;
* observable;
* idempotent;
* resumable.

---

# 61. Analytics Observability

The system should monitor:

* processing latency;
* queue size;
* failed jobs;
* rebuild duration;
* data freshness;
* query latency;
* cache hit rate;
* storage usage;
* calculation errors.

---

# 62. Analytics Auditability

Important analytical outputs should be traceable to:

* source data;
* algorithm version;
* calculation time;
* relevant academic period.

This allows administrators and developers to investigate unexpected results.

---

# 63. Analytics and AI

AI may interpret analytics.

The correct architecture is:

```text id="t5q9w1"
Authoritative Data
       ↓
Deterministic Analytics
       ↓
Learning Intelligence
       ↓
AI Interpretation
```

AI must not become the calculation engine for authoritative metrics.

---

# 64. AI Interpretation Boundary

AI may say:

> The available results indicate that performance in this topic has remained below the configured threshold across recent attempts.

AI must not independently invent the underlying metric.

The numeric evidence comes from analytics.

---

# 65. Recommendations

Analytics may generate deterministic recommendations such as:

* topic requires additional practice;
* student has insufficient evidence;
* teacher may review a topic;
* class may require intervention.

AI may then explain or contextualize those recommendations.

---

# 66. Ranking and Competitive Metrics

If rankings or comparative metrics are introduced, they must be explicitly configured.

The system should avoid presenting competitive rankings as the default definition of academic progress.

Where ranking exists, the metric and population must be clearly identified.

---

# 67. Data Export

Analytics exports should support authorized formats where required.

Exports should include:

* report period;
* generated timestamp;
* scope;
* metric definitions;
* data status.

Sensitive exports must be permission-controlled and audited.

---

# 68. Report Generation

Reports may be generated for:

* student progress;
* parent review;
* teacher class analysis;
* principal management;
* term summaries.

Generated reports should identify whether data is:

* current;
* stale;
* partial;
* synchronized.

---

# 69. Analytics Failure Behavior

If analytics processing fails:

* authoritative academic records remain intact;
* dashboards should display a clear stale/unavailable state;
* background jobs should retry;
* failures should be logged;
* administrators should be able to identify the affected period.

Analytics failure must not block:

* login;
* practice;
* CBT;
* scoring;
* question access;
* synchronization of authoritative records.

---

# 70. Data Integrity

Analytics must validate source records before processing.

Examples:

* invalid student reference;
* missing subject;
* invalid topic relationship;
* impossible score;
* duplicate result;
* unknown academic period.

Invalid records should be quarantined or reported rather than silently producing incorrect analytics.

---

# 71. Analytics Testing

Testing must cover:

### Unit Tests

* metric formulas;
* aggregation;
* learning-gap rules;
* trend calculations;
* threshold behavior.

### Integration Tests

* assessment → analytics;
* practice → analytics;
* promotion → historical analytics;
* synchronization → analytics.

### Authorization Tests

* student isolation;
* parent-child scope;
* teacher assignment scope;
* principal scope;
* admin scope.

### Regression Tests

Existing metrics must remain stable unless an intentional version change occurs.

---

# 72. Analytics Failure Testing

Test scenarios should include:

* duplicate results;
* delayed synchronization;
* missing records;
* corrected scores;
* deleted content;
* changed question versions;
* incomplete attempts;
* corrupted analytical cache;
* failed processing job;
* interrupted rebuild;
* database restart;
* local server restart.

---

# 73. Rebuild Testing

A full rebuild from authoritative data must produce analytically equivalent results for the same algorithm version.

This is an important integrity property.

---

# 74. Performance Targets

The implementation should define measurable targets for:

* dashboard response time;
* analytical job latency;
* incremental processing;
* full rebuild;
* large class aggregation;
* school-level reporting.

Targets must be validated using realistic school datasets.

---

# 75. Scalability

The architecture should support growth in:

* students;
* classes;
* arms;
* subjects;
* topics;
* questions;
* assessments;
* practice activity;
* historical sessions.

Adding students should not require architectural redesign.

---

# 76. Data Retention

Analytics retention must respect authoritative data retention policies.

Derived data may be removed and rebuilt where practical.

Historical analytical snapshots may be retained when required for:

* reporting;
* audit;
* academic history;
* management analysis.

---

# 77. Analytics Security

Analytics endpoints and storage must follow the security architecture.

Required controls include:

* authentication;
* authorization;
* object-level access control;
* input validation;
* output filtering;
* audit logging;
* rate limiting;
* secure exports;
* protection against unauthorized aggregation.

---

# 78. Analytics Non-Interference

Analytics must never become a blocking dependency for core academic operations.

The system must continue operating if:

* analytics worker is down;
* analytics database is rebuilding;
* learning-gap processing is delayed;
* dashboard cache is unavailable.

Core academic workflows remain authoritative and operational.

---

# 79. Definition of Done

Analytics architecture is complete when:

* authoritative and derived data are clearly separated;
* all major metrics are formally defined;
* student analytics exist;
* teacher analytics exist;
* parent analytics exist;
* principal analytics exist;
* learning-gap logic is deterministic and versioned;
* historical session/term integrity is preserved;
* question versions are respected;
* role-based access is enforced;
* parent-child access is explicit;
* teacher scope follows assignments;
* dashboards use appropriate read models;
* analytics can refresh incrementally;
* full rebuild is supported;
* synchronization state is represented;
* stale data is identifiable;
* analytics failures do not affect core LMS operation;
* analytics are auditable;
* analytics security tests pass;
* performance is validated against realistic data.

---

# 80. Non-Negotiable Rules

1. Authoritative academic records remain the source of truth.
2. Analytics are derived data.
3. Derived analytics must be rebuildable.
4. Scores are calculated by the assessment engine, not analytics.
5. Analytics must not rewrite historical academic records.
6. Historical results retain their original academic context.
7. Learning gaps must be evidence-based.
8. Insufficient evidence must not be treated as poor performance.
9. Every important metric must have a documented definition.
10. Analytical algorithms must be versioned where changes affect interpretation.
11. Student analytics are private to the student.
12. Parent analytics are limited to explicitly related students.
13. Teacher analytics follow teacher assignments.
14. Principal analytics follow management authorization.
15. Cached analytics must never bypass authorization.
16. Stale analytics must be identifiable.
17. Analytics failure must not stop core academic workflows.
18. AI may interpret analytics but must not replace deterministic analytics.
19. Synchronization should prioritize authoritative data over derived metrics.
20. Analytical results must remain explainable and traceable to their source data.

---

# 81. Final Analytics Principle

CRC analytics exists to turn reliable academic records into useful intelligence without compromising academic integrity.

The fundamental architecture is:

```text
Authoritative Academic Activity
            ↓
     Deterministic Processing
            ↓
       Derived Metrics
            ↓
      Learning Intelligence
            ↓
 Role-Authorized Dashboards
            ↓
 Optional AI Interpretation
```

The fundamental principle is:

> **Analytics explains what the academic system knows; it does not redefine what the academic system knows.**
