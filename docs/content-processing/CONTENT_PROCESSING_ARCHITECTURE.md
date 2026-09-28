# Content Processing Architecture

## 1. Purpose

This document defines the architecture for CRC's online content-processing subsystem.

The subsystem converts uploaded academic materials into structured, reviewable, reusable academic content.

Its primary V1 purpose is to support the ingestion of:

* PDF documents;
* DOCX documents;
* images;
* scanned academic materials;
* question papers;
* question-bank source materials;
* structured academic documents.

The processing system MUST preserve the distinction between:

```text
Original Source Material
        ↓
Machine-Extracted Content
        ↓
Normalized Structured Content
        ↓
Teacher Review
        ↓
Teacher Approval
        ↓
Published Academic Content
```

Machine processing MUST NOT automatically make extracted questions available for student assessment.

Teacher review and approval remain mandatory before question-bank content becomes usable in assessments.

---

# 2. Architectural Position

Content processing is an **online-only** subsystem.

It is not part of the local school server's offline runtime.

```text
                         INTERNET
                            │
                            ▼
                  ┌──────────────────┐
                  │   ONLINE MASTER  │
                  └────────┬─────────┘
                           │
                  Content Processing
                           │
       ┌───────────────────┼───────────────────┐
       ▼                   ▼                   ▼
    Storage              OCR              Processing
       │                   │                   │
       └───────────────────┼───────────────────┘
                           ▼
                   Structured Content
                           │
                           ▼
                     Review Queue
                           │
                           ▼
                  Teacher Approval
                           │
                           ▼
                    Published Content
                           │
                           ▼
                 Academic / Assessment
                        Systems
```

The local server does not depend on content-processing services for normal offline CBT or practice operation.

Approved content must be synchronized to the local server through the synchronization subsystem.

---

# 3. Processing Objectives

The subsystem MUST:

1. accept supported academic source files;
2. validate uploaded files;
3. securely store original files;
4. detect document characteristics;
5. extract text and structure;
6. perform OCR when required;
7. identify questions and answer options;
8. preserve mathematical notation where possible;
9. preserve tables and structured content;
10. extract images and diagrams where required;
11. normalize extracted content;
12. create reviewable structured records;
13. track processing status;
14. expose processing failures;
15. support teacher review and editing;
16. support teacher approval;
17. publish only approved content;
18. preserve source provenance;
19. maintain content versions;
20. support reprocessing without destroying historical data;
21. provide processing observability;
22. prevent malicious uploads from compromising the application.

---

# 4. Non-Goals

The content-processing subsystem MUST NOT:

* automatically publish extracted questions;
* determine final academic correctness without human approval;
* modify student grades;
* score CBT attempts;
* replace the assessment engine;
* perform local offline processing;
* use Ollama as the primary document-normalization engine;
* bypass teacher review;
* silently overwrite previously approved content.

---

# 5. Processing Technology

The V1 processing pipeline uses:

* PaddleOCR / PP-OCR for OCR;
* PP-Structure where layout analysis is required;
* deterministic parsing and normalization;
* dedicated document-processing workers;
* object/file storage for source artifacts;
* PostgreSQL metadata storage.

Ollama is **not** the primary content-processing engine.

AI-assisted interpretation may be introduced as a separately controlled processing stage in the future, but deterministic extraction and normalization remain the architectural baseline.

---

# 6. Supported Input Formats

V1 MUST support:

```text
PDF
DOCX
PNG
JPG/JPEG
```

The architecture SHOULD be extensible to additional formats later.

Potential future formats include:

* PPTX;
* XLSX;
* HTML;
* EPUB;
* other educational document formats.

Unsupported formats MUST be rejected before entering the processing pipeline.

---

# 7. Upload Architecture

The upload flow is:

```text
User
 │
 ▼
Upload API
 │
 ├── Authentication
 ├── Authorization
 ├── File Validation
 ├── Size Validation
 ├── MIME Validation
 ├── Malware/Security Checks
 └── Metadata Creation
 │
 ▼
Original File Storage
 │
 ▼
Processing Job
```

Only authorized users may upload academic materials.

Teacher upload permissions MUST be controlled according to the teacher's assigned academic scope where applicable.

Super Admin may have broader upload and management permissions.

---

# 8. Upload Security

Uploaded files are untrusted input.

The system MUST NOT trust:

* filename extensions;
* browser-provided MIME type;
* file metadata;
* embedded document metadata;
* document contents;
* macros;
* scripts;
* external references.

Validation MUST use actual file characteristics where practical.

The system MUST enforce:

* maximum file size;
* supported format validation;
* upload rate limits;
* storage quotas where appropriate;
* filename normalization;
* safe storage paths;
* malware/security scanning where available.

---

# 9. File Naming

Original filenames are metadata only.

They MUST NOT be used directly as filesystem paths.

The system SHOULD generate internal storage identifiers.

Example:

```text
Original filename:
Mathematics_JSS2_Revision.pdf

Internal object:
content-source/{global-file-id}/original
```

This prevents:

* path traversal;
* filename collisions;
* unsafe characters;
* accidental overwrites.

---

# 10. Original Source Preservation

The original uploaded file SHOULD be preserved.

The original file is the authoritative source artifact for the processing operation.

The system SHOULD preserve:

* source file;
* checksum;
* file size;
* MIME type;
* original filename;
* uploader;
* upload timestamp;
* processing version;
* source revision.

A processed representation MUST NOT replace the original source.

---

# 11. File Integrity

Every uploaded source file SHOULD receive a cryptographic checksum.

Example:

```text id="x9s3ju"
Source File
    │
    ▼
SHA-256
    │
    ▼
Content Hash
```

The checksum can be used to:

* detect duplicate uploads;
* verify file integrity;
* identify unchanged files;
* validate processing artifacts;
* support audit investigations.

---

# 12. Duplicate Detection

The system SHOULD detect identical source files using content hashes.

Duplicate detection MUST NOT automatically delete or merge records.

Instead, the system should identify:

```text
Same Content
Different Upload Event
```

as separate upload events that may reference the same source artifact.

This preserves auditability.

---

# 13. Processing Job Architecture

Processing MUST be asynchronous for non-trivial documents.

```text
Upload
  │
  ▼
Job Created
  │
  ▼
Queue
  │
  ▼
Processing Worker
  │
  ├── Validate
  ├── Inspect
  ├── OCR
  ├── Layout
  ├── Extract
  ├── Normalize
  ├── Classify
  └── Persist
  │
  ▼
Reviewable Output
```

The upload request MUST NOT remain blocked for the entire processing operation.

---

# 14. Processing Job States

A processing job MUST have an explicit state.

Recommended lifecycle:

```text id="b6egz2"
QUEUED
   ↓
VALIDATING
   ↓
EXTRACTING
   ↓
OCR_PROCESSING
   ↓
STRUCTURE_PROCESSING
   ↓
NORMALIZING
   ↓
QUESTION_EXTRACTION
   ↓
REVIEW_REQUIRED
   ↓
COMPLETED
```

Failure states:

```text id="91j1k7"
FAILED
CANCELLED
RETRY_PENDING
```

The exact state sequence may differ by document type.

---

# 15. Processing Idempotency

Processing jobs SHOULD be idempotent.

If a worker crashes during processing, retrying the job MUST NOT create uncontrolled duplicate content.

Each processing job SHOULD have:

* unique job ID;
* source file ID;
* processing version;
* operation ID;
* attempt count;
* status;
* timestamps.

Intermediate artifacts SHOULD be associated with the processing job.

---

# 16. Processing Versions

Processing output MUST record the processing pipeline version.

Example:

```text id="d9t6d7"
processing_version = "content-pipeline-v1"
ocr_engine = "PP-OCR"
ocr_version = "..."
structure_engine = "PP-Structure"
```

This allows content to be reprocessed when:

* OCR improves;
* parsing improves;
* extraction rules change;
* bugs are fixed;
* new content types are supported.

Historical outputs should not be silently rewritten.

---

# 17. Document Inspection

Before OCR or extraction, the processor SHOULD inspect the source document.

Inspection may determine:

* file type;
* page count;
* image density;
* text density;
* page dimensions;
* embedded images;
* table presence;
* text layer presence;
* scan quality;
* orientation;
* encoding;
* document structure.

The result determines which processing stages are necessary.

---

# 18. Text-Based PDF Processing

For PDFs with a reliable text layer:

```text id="yp2j0u"
PDF
 │
 ▼
Text Extraction
 │
 ▼
Layout Analysis
 │
 ▼
Structured Representation
```

OCR SHOULD NOT be unnecessarily applied to already reliable text.

This reduces:

* processing time;
* compute usage;
* extraction noise;
* duplicate text.

---

# 19. Scanned PDF Processing

For scanned PDFs:

```text id="d5i0oa"
PDF Pages
   │
   ▼
Page Rendering
   │
   ▼
Image Preprocessing
   │
   ▼
PP-OCR
   │
   ▼
Text + Bounding Boxes
```

OCR output SHOULD retain positional information where available.

---

# 20. Image Processing

Images may require:

* orientation detection;
* rotation correction;
* resizing;
* denoising;
* contrast normalization;
* cropping;
* OCR;
* layout detection.

Preprocessing MUST avoid unnecessary destruction of diagrams, mathematical notation, or important visual content.

---

# 21. OCR Output

OCR output should not be stored as only a plain text blob.

Where available, the system SHOULD preserve:

* extracted text;
* page;
* block;
* line;
* word;
* bounding box;
* confidence;
* reading order.

Conceptually:

```text id="l7d5kv"
Page
 ├── Block
 │    ├── Line
 │    │    ├── Word
 │    │    └── Confidence
 │    └── Bounding Box
 └── Image/Diagram
```

This enables later reconstruction and review.

---

# 22. OCR Confidence

OCR confidence SHOULD be captured where the OCR engine provides it.

Low-confidence regions SHOULD be flagged for review.

Example:

```text id="4uzfne"
High Confidence
      ↓
Normal Processing

Low Confidence
      ↓
Review Flag
```

Confidence is a processing signal, not an academic correctness judgment.

---

# 23. Layout Processing

PP-Structure or equivalent layout processing SHOULD identify structures such as:

* headings;
* paragraphs;
* tables;
* lists;
* images;
* figures;
* question blocks;
* answer-option blocks.

The layout model SHOULD retain source coordinates where useful.

---

# 24. Reading Order

The system MUST attempt to reconstruct logical reading order.

This is particularly important for:

* multi-column documents;
* examination papers;
* tables;
* side-by-side options;
* diagrams;
* headers and footers.

The original page representation MUST remain available for review when reconstruction is uncertain.

---

# 25. Headers and Footers

Repeated headers and footers SHOULD be identified separately from academic body content.

They MUST NOT accidentally become part of every question.

Examples include:

* school name;
* examination title;
* page number;
* document date;
* repeated instructions.

---

# 26. Table Extraction

Tables MUST be treated as structured content where practical.

The processor SHOULD preserve:

* rows;
* columns;
* cell text;
* cell ordering;
* merged-cell information where available;
* source position.

Tables MAY be rendered into rich content for question review.

---

# 27. Images and Diagrams

Academic questions may depend on images or diagrams.

The processing pipeline MUST support retaining these assets.

Examples:

* geometry diagrams;
* maps;
* charts;
* science diagrams;
* circuit diagrams;
* illustrations;
* graphs.

The system MUST NOT discard an image merely because OCR cannot interpret it.

---

# 28. Mathematical Content

Mathematical notation is a first-class content requirement.

The processing pipeline SHOULD preserve mathematical expressions in a format suitable for rendering.

Preferred normalized representation:

```text
LaTeX / MathML-compatible representation
```

Examples of content requiring special handling include:

* fractions;
* indices;
* exponents;
* roots;
* equations;
* matrices;
* symbols;
* mathematical operators.

The pipeline MUST avoid reducing mathematical expressions to ambiguous plain text whenever structured representation can be preserved.

---

# 29. Rich Text Representation

Processed academic content SHOULD use a structured rich-content representation rather than storing only plain text.

The representation SHOULD support:

* paragraphs;
* headings;
* emphasis;
* lists;
* tables;
* images;
* diagrams;
* links where appropriate;
* mathematical expressions.

A structured JSON-based rich-content format or editor-compatible document model may be used.

The selected representation MUST be:

* serializable;
* versionable;
* renderable;
* safe;
* searchable.

---

# 30. Content Blocks

A document SHOULD be represented internally as ordered content blocks.

Example:

```text id="crb1on"
Document
 ├── Heading
 ├── Paragraph
 ├── Image
 ├── Question
 ├── Option
 ├── Table
 └── Question
```

This allows downstream processors to operate on meaningful units instead of raw text.

---

# 31. Question Detection

For question-bank ingestion, the system SHOULD identify candidate question blocks.

Signals may include:

* numbering;
* question marks;
* option patterns;
* section headings;
* answer structures;
* layout boundaries;
* repeated examination patterns.

Question detection is an extraction task, not an approval task.

---

# 32. Question Extraction

The extraction pipeline SHOULD identify:

* question stem;
* answer options;
* option ordering;
* candidate correct answer where explicitly represented;
* images;
* diagrams;
* mathematical content;
* tables;
* question numbering;
* source location;
* extraction confidence.

The output is a candidate question.

It is not yet a published question.

---

# 33. Correct Answer Extraction

Where the source explicitly contains an answer key, the processor MAY extract the candidate correct answer.

Examples:

```text id="e5p9l4"
Answer Key
A
```

or:

```text id="0nq3n8"
1. C
2. A
3. D
```

The extracted answer MUST be marked as:

```text id="j2f6i5"
candidate_correct_answer
```

until reviewed.

The system MUST NOT treat OCR-extracted answer keys as automatically authoritative.

---

# 34. Ambiguous Extraction

If the processor cannot confidently determine:

* question boundaries;
* options;
* correct answer;
* mathematical structure;
* table structure;
* image association;

it MUST preserve the ambiguity for review rather than inventing content.

The system MUST NOT fabricate missing text.

---

# 35. Normalization

Normalization converts extracted content into the application's canonical structure.

Normalization MAY include:

* whitespace normalization;
* Unicode normalization;
* option ordering normalization;
* numbering removal;
* line-break cleanup;
* entity normalization;
* mathematical representation normalization;
* rich-content conversion.

Normalization MUST NOT change academic meaning intentionally.

---

# 36. Meaning Preservation

Normalization must distinguish between formatting correction and semantic alteration.

For example:

```text id="4j9k8w"
"2x + 3 = 7"
```

MAY be normalized to structured mathematical content.

But changing:

```text
"greater than"
```

to:

```text
"greater than or equal to"
```

would be a semantic modification and MUST NOT happen automatically.

---

# 37. Candidate Question Model

A candidate question SHOULD contain:

```text id="k5o9k4"
CandidateQuestion
├── candidate_id
├── source_document_id
├── source_page
├── source_location
├── stem
├── options[]
├── candidate_correct_option
├── media[]
├── mathematical_content[]
├── tables[]
├── extraction_confidence
├── processing_job_id
└── review_status
```

The exact database model is defined in:

`docs/database/DATABASE_ARCHITECTURE.md`

---

# 38. Question Review Lifecycle

The required lifecycle is:

```text id="p8m8ez"
Imported
   ↓
Processing
   ↓
Review Required
   ↓
Teacher Review/Edit
   ↓
Teacher Approval
   ↓
Published
   ↓
Available for Assessment
```

The system MUST NOT skip:

```text
Teacher Review/Edit
Teacher Approval
```

for machine-extracted questions.

---

# 39. Review Queue

Teachers with appropriate permissions MUST receive a review queue.

The queue SHOULD expose:

* processing status;
* candidate count;
* low-confidence items;
* source document;
* source page;
* extraction warnings;
* missing content warnings;
* answer-key uncertainty;
* review status.

---

# 40. Teacher Review Interface

Teacher review MUST allow authorized teachers to:

* inspect original source;
* inspect extracted content;
* edit question text;
* edit options;
* set the correct answer;
* edit rich text;
* edit mathematical content;
* associate images;
* correct extraction errors;
* reject candidate questions;
* approve valid questions.

The interface SHOULD make comparison between source and extracted content easy.

---

# 41. Source-to-Output Traceability

Every extracted question MUST be traceable to its source.

At minimum:

```text id="a1k2q4"
Question
  ↓
Processing Job
  ↓
Source Document
  ↓
Page / Region
```

This allows a teacher to verify extraction against the original material.

---

# 42. Rejection

A teacher MAY reject a candidate question.

Reasons may include:

* extraction error;
* duplicate;
* incomplete source;
* poor scan;
* invalid question;
* incorrect answer information;
* inappropriate content;
* unusable formatting.

Rejected candidates SHOULD remain auditable rather than being silently deleted.

---

# 43. Approval

Teacher approval is an explicit state transition.

Approval MUST record:

* reviewer;
* timestamp;
* question version;
* source processing version;
* approval event.

Only approved question versions may become eligible for publication.

---

# 44. Publication

Publication is separate from approval.

The system MUST distinguish:

```text id="u8n8gi"
Approved
   ≠
Published
```

This permits controlled content release.

Publication MUST verify that:

* required fields exist;
* at least one valid option exists;
* the correct answer is defined;
* content is renderable;
* referenced media exists;
* academic classification is valid;
* question version is internally consistent.

---

# 45. Assessment Eligibility

A question is eligible for use only when:

```text id="1h7h2u"
Question Version
      │
      ├── Valid
      ├── Reviewed
      ├── Approved
      ├── Published
      └── Assessment-Eligible
```

Draft, rejected, processing, or unapproved questions MUST NOT enter an assessment.

---

# 46. Question Versioning

Questions MUST be versioned.

A change to an approved/published question SHOULD create a new question version rather than mutating historical content.

Example:

```text id="7a0l8v"
Question Q123
   │
   ├── Version 1 — Published
   │
   └── Version 2 — Draft/Edit
```

Historical assessments must retain the exact question version presented to the student.

---

# 47. Published Content Immutability

Published content used by an assessment SHOULD be treated as immutable for historical purposes.

If a teacher identifies an error:

```text id="9k6x6k"
Published Version
       │
       ▼
Correction
       │
       ▼
New Version
```

Existing attempts remain associated with the original version.

---

# 48. Duplicate Question Detection

The system SHOULD detect likely duplicate questions.

Detection may use:

* normalized text;
* option similarity;
* source metadata;
* content fingerprints;
* semantic similarity where introduced.

Duplicate detection is advisory.

It MUST NOT automatically delete or merge questions without an explicit review process.

---

# 49. Content Classification

Approved questions SHOULD be associated with the academic hierarchy:

```text id="c4a8nd"
Academic Session
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
    ↓
Question
```

The processor MAY suggest classification based on source metadata.

Final classification MUST remain subject to authorized review/configuration.

---

# 50. Content Processing and Academic Scope

A teacher MUST only be able to approve or publish content they are authorized to manage.

Authorization MUST consider:

* teacher identity;
* subject assignment;
* class/arm assignment;
* content ownership;
* publication permission.

Super Admin retains broader administrative control.

---

# 51. Processing Failures

A processing failure MUST produce an explicit failed state.

Example:

```text id="2p7m2u"
Processing
   │
   ▼
Failure
   │
   ├── Error Recorded
   ├── Job Marked Failed
   ├── Source Preserved
   └── Retry Available
```

The system MUST NOT delete the original upload because processing failed.

---

# 52. Retry Strategy

Retryable failures SHOULD be automatically retried.

Examples:

* temporary worker failure;
* transient storage failure;
* service timeout;
* temporary resource exhaustion.

Non-retryable failures SHOULD be surfaced for review.

Examples:

* corrupted file;
* unsupported format;
* invalid document structure;
* persistent parser failure.

Retries MUST be bounded.

---

# 53. Worker Isolation

Content-processing workers SHOULD be isolated from the main API process.

This reduces the impact of:

* parser crashes;
* malformed files;
* memory exhaustion;
* OCR failures;
* third-party library vulnerabilities.

Conceptually:

```text id="xw1l5e"
API
 │
 ▼
Job Queue
 │
 ├── OCR Worker
 ├── Structure Worker
 ├── Extraction Worker
 └── Normalization Worker
```

---

# 54. Resource Limits

Processing workers MUST enforce resource limits.

Controls SHOULD include:

* maximum upload size;
* maximum page count where appropriate;
* maximum processing duration;
* CPU limits;
* memory limits;
* output-size limits;
* queue limits;
* concurrency limits.

This protects the online system against resource-exhaustion attacks.

---

# 55. Malicious Document Protection

Documents can contain malicious or intentionally crafted content.

The system MUST consider:

* parser vulnerabilities;
* decompression bombs;
* malicious macros;
* embedded scripts;
* malformed XML;
* malicious images;
* oversized content;
* archive-based attacks;
* external entity attacks.

Document parsers MUST be configured securely.

Macros and executable content MUST NOT be executed as part of normal content processing.

---

# 56. DOCX Security

DOCX files are package-based document formats.

Processing MUST:

* safely unpack documents;
* restrict extraction paths;
* prevent path traversal;
* avoid executing embedded content;
* inspect embedded media safely;
* reject dangerous structures where appropriate.

The processor MUST treat DOCX as untrusted input.

---

# 57. PDF Security

PDF processing MUST account for:

* malformed objects;
* embedded JavaScript;
* external references;
* embedded files;
* parser vulnerabilities.

The system SHOULD use hardened, sandboxed processing where practical.

---

# 58. Image Security

Image processing MUST protect against:

* malformed image files;
* decompression bombs;
* excessive dimensions;
* memory exhaustion;
* parser vulnerabilities.

Images SHOULD be normalized into safe internal representations before broad application use.

---

# 59. Content Storage Architecture

Content storage should separate:

```text id="m0v8lw"
Original Files
Processed Artifacts
Structured Content
Question Records
Media Assets
Processing Metadata
```

Large binary objects SHOULD NOT be stored directly inside ordinary relational rows when object/file storage is more appropriate.

PostgreSQL stores metadata and relationships.

---

# 60. Content Metadata

Each source document SHOULD record:

* global ID;
* original filename;
* MIME type;
* size;
* checksum;
* uploader;
* upload time;
* processing status;
* processing version;
* storage location;
* academic context where known;
* retention state;
* review state.

---

# 61. Content Retention

Original files SHOULD be retained according to project retention requirements.

Deleting an original source SHOULD NOT be allowed if active approved content still depends on it unless the system has an explicit archival policy.

Content deletion MUST consider:

* published questions;
* assessment history;
* audit records;
* provenance;
* synchronization;
* backups.

---

# 62. Content Provenance

Every structured academic item SHOULD preserve provenance.

Example:

```text id="z0q0i9"
Question
  │
  ├── Source File
  ├── Source Page
  ├── Source Region
  ├── Processing Job
  ├── Processor Version
  ├── Reviewer
  └── Approval Event
```

This enables reliable academic and technical auditing.

---

# 63. Content Search

Processed content SHOULD be searchable using indexed structured fields.

Potential search dimensions include:

* subject;
* class;
* arm;
* topic;
* question text;
* source;
* status;
* reviewer;
* processing date;
* publication state.

Search indexing MUST not expose unauthorized content.

---

# 64. Content and Synchronization

Only approved/published content intended for school-local operation should be synchronized to the local server.

Example:

```text id="g6l1pi"
Online Content
      │
      ▼
Approved + Published
      │
      ▼
Sync Eligibility
      │
      ▼
Local Server
```

Processing intermediates SHOULD remain online unless explicitly required locally.

This minimizes local storage and attack surface.

---

# 65. Synchronization of Content Versions

Content synchronization MUST preserve version identity.

The local server should know:

* question ID;
* question version;
* publication state;
* content revision;
* media references.

A local assessment MUST reference a specific question version.

---

# 66. Content Removal

If published content is withdrawn:

```text id="u6e1a1"
Published
   │
   ▼
Withdrawn
```

the synchronization system must propagate the withdrawal according to content-availability rules.

Existing completed attempts MUST remain historically valid.

The withdrawal MUST NOT rewrite historical assessment results.

---

# 67. Processing Observability

The system MUST provide operational visibility into processing.

Metrics SHOULD include:

* uploaded documents;
* queued jobs;
* active jobs;
* successful jobs;
* failed jobs;
* retry count;
* average processing time;
* OCR duration;
* extraction duration;
* review queue size;
* approval rate;
* rejection rate;
* low-confidence rate.

---

# 68. Processing Logs

Logs SHOULD contain:

* job ID;
* source ID;
* processing stage;
* worker;
* duration;
* status;
* error category;
* correlation ID.

Logs MUST NOT contain:

* passwords;
* authentication tokens;
* private secrets;
* unnecessary personal information.

Large extracted documents SHOULD not be duplicated unnecessarily into logs.

---

# 69. Processing Errors

Errors should be classified.

Recommended categories:

```text id="5m9gwl"
VALIDATION_ERROR
UNSUPPORTED_FORMAT
CORRUPT_FILE
OCR_ERROR
STRUCTURE_ERROR
EXTRACTION_ERROR
NORMALIZATION_ERROR
STORAGE_ERROR
RESOURCE_LIMIT
SECURITY_REJECTION
INTERNAL_ERROR
```

This enables useful retry and operational behavior.

---

# 70. Human Review as a Trust Boundary

Human review is a deliberate trust boundary.

The pipeline moves from:

```text id="3n1vzn"
Untrusted Source
      ↓
Machine Interpretation
      ↓
Human Review
      ↓
Approved Academic Content
```

The system MUST NOT collapse these stages merely for convenience.

---

# 71. Content Processing and AI

Ollama is not required for deterministic OCR and normalization.

If AI is used later in content processing, it MUST operate as an isolated, auditable assistance layer.

AI-generated output MUST be clearly distinguishable from deterministic extraction.

AI MUST NOT automatically:

* publish questions;
* set authoritative correct answers;
* modify approved content;
* alter student grades;
* bypass teacher approval.

---

# 72. AI-Assisted Suggestions

If AI assistance is introduced, it MAY suggest:

* question classification;
* topic classification;
* wording cleanup;
* duplicate detection;
* malformed-question warnings;
* answer-key review suggestions.

Such suggestions MUST remain editable and reviewable.

---

# 73. Content Processing Security Boundary

The processing subsystem sits between:

```text
Untrusted External File
```

and:

```text
Trusted Academic Database
```

Therefore:

```text id="p9x1qk"
Untrusted Input
      │
      ▼
Validation
      │
      ▼
Sandboxed Processing
      │
      ▼
Structured Output
      │
      ▼
Human Review
      │
      ▼
Trusted Published Content
```

This boundary MUST be preserved.

---

# 74. API Integration

Content-processing APIs MUST follow:

`docs/api/API_ARCHITECTURE.md`

Typical operations include:

```text id="k4g6cw"
POST /api/v1/content/uploads
GET  /api/v1/content/uploads/{id}
GET  /api/v1/content/processing-jobs/{id}
GET  /api/v1/content/review-queue
GET  /api/v1/content/candidates/{id}
PATCH /api/v1/content/candidates/{id}
POST /api/v1/content/candidates/{id}/approve
POST /api/v1/content/candidates/{id}/reject
POST /api/v1/content/{id}/publish
POST /api/v1/content/{id}/withdraw
```

Exact routes may be refined during implementation.

All operations MUST enforce server-side authorization.

---

# 75. Transaction Boundaries

Processing metadata updates MUST use appropriate database transactions.

For example:

```text id="h9j1xw"
Create Candidate
+
Create Source Reference
+
Create Processing Metadata
```

should be committed atomically where appropriate.

Large binary processing SHOULD occur outside long-running database transactions.

---

# 76. Concurrency

The system MUST prevent conflicting review operations.

Examples:

* two teachers editing the same candidate;
* one teacher approving while another edits;
* publication occurring while content is modified.

Optimistic concurrency controls SHOULD be used.

A stale editor MUST receive a conflict rather than silently overwriting another user's changes.

---

# 77. Audit Requirements

The system MUST audit:

* upload;
* processing start;
* processing completion;
* processing failure;
* review;
* edit;
* approval;
* rejection;
* publication;
* withdrawal;
* reprocessing;
* deletion/archive;
* authorization failures.

Audit records MUST identify the responsible actor where applicable.

---

# 78. Reprocessing

Reprocessing MUST create a new processing attempt or processing version.

The system MUST preserve the relationship:

```text id="c4mtkk"
Original Source
   ├── Processing v1
   ├── Processing v2
   └── Processing v3
```

Existing approved content MUST NOT be silently replaced by a new extraction.

A new result must go through the required review process before becoming authoritative.

---

# 79. Manual Content Creation

V1's question-bank workflow is centered on imported questions.

If manual question creation is implemented later, it MUST still use the same fundamental question model and approval/publication lifecycle where applicable.

The content-processing architecture MUST therefore avoid coupling imported content to a completely separate question system.

---

# 80. Failure Recovery

If the content-processing service crashes:

1. original files remain intact;
2. processing jobs remain identifiable;
3. incomplete jobs remain recoverable;
4. retry policy determines whether the job resumes or restarts;
5. no partial candidate is silently treated as approved;
6. review state remains consistent.

The system MUST favor recoverability over silent data loss.

---

# 81. Backup Requirements

Backups SHOULD cover:

* original source metadata;
* structured content;
* question versions;
* processing metadata;
* review records;
* publication state;
* audit records.

Binary source storage MUST have an appropriate backup/retention strategy.

---

# 82. Performance Requirements

The processing system SHOULD support concurrent processing while respecting resource limits.

Performance should be measured for:

* small image;
* multi-page PDF;
* large scanned PDF;
* DOCX;
* image-heavy documents;
* question-bank documents.

The system SHOULD expose processing duration metrics by document type.

---

# 83. Scalability

The processing architecture SHOULD allow worker scaling independently from the main API.

Conceptually:

```text id="9p7qzi"
                 Processing Queue
                       │
          ┌────────────┼────────────┐
          ▼            ▼            ▼
       Worker 1     Worker 2     Worker N
```

This permits increased throughput without scaling the entire application unnecessarily.

---

# 84. Testing Strategy

Content processing MUST have layered tests.

## 84.1 Unit Tests

Test:

* MIME detection;
* file validation;
* checksum generation;
* filename sanitization;
* content normalization;
* question parsing;
* option extraction;
* mathematical-content handling;
* confidence handling;
* state transitions.

## 84.2 Fixture Tests

Maintain representative fixtures for:

* text PDFs;
* scanned PDFs;
* DOCX;
* clean images;
* low-quality scans;
* multi-column documents;
* tables;
* diagrams;
* mathematical expressions;
* mixed-content documents.

Expected extraction results SHOULD be version-controlled where appropriate.

## 84.3 Integration Tests

Test:

* upload;
* queueing;
* worker execution;
* OCR;
* structure extraction;
* candidate creation;
* review;
* approval;
* publication;
* withdrawal;
* reprocessing.

## 84.4 Security Tests

Test:

* malicious files;
* path traversal;
* oversized uploads;
* decompression bombs;
* malformed documents;
* unauthorized upload;
* unauthorized review;
* unauthorized approval;
* unauthorized publication;
* parser isolation;
* resource exhaustion.

## 84.5 Regression Tests

Every processing bug that affects production behavior SHOULD result in a regression fixture or test.

---

# 85. Definition of Done

The content-processing subsystem is complete only when:

### Upload

* supported formats work;
* unsupported formats are rejected;
* size limits work;
* files are securely stored;
* source checksums are generated.

### Processing

* PDF processing works;
* DOCX processing works;
* image OCR works;
* layout extraction works;
* tables are preserved where supported;
* images/diagrams are retained;
* mathematical content is preserved where supported.

### Question Extraction

* candidate questions are extracted;
* options are extracted;
* candidate answer keys are handled safely;
* provenance is retained;
* low-confidence extraction is flagged.

### Review

* review queue works;
* teachers can edit candidates;
* teachers can reject candidates;
* teachers can approve candidates;
* unauthorized users cannot review content outside their scope.

### Publication

* only approved content can be published;
* publication is audited;
* question versions are preserved;
* historical content remains stable.

### Security

* uploaded files are treated as untrusted;
* parsers are isolated appropriately;
* malicious files are rejected or contained;
* no secrets are logged;
* authorization is enforced.

### Reliability

* processing failures are recoverable;
* retries work;
* jobs are idempotent;
* original files are preserved.

### Testing

* unit tests pass;
* fixture tests pass;
* integration tests pass;
* security tests pass;
* regression tests pass.

---

# 86. Non-Negotiable Rules

1. Content processing is online-only.
2. Original source files must be preserved according to retention policy.
3. Uploaded files are untrusted.
4. File extensions alone cannot determine file safety.
5. Processing must be isolated from the main request path.
6. Processing failures must not destroy source files.
7. Machine extraction does not equal approval.
8. Teacher review is mandatory for imported questions.
9. Teacher approval is mandatory before assessment eligibility.
10. Unapproved questions cannot enter assessments.
11. Published assessment content must be versioned.
12. Historical assessment content must remain reproducible.
13. Mathematical content must be treated as first-class content.
14. Images and diagrams must not be discarded merely because OCR cannot interpret them.
15. OCR confidence is a processing signal, not academic truth.
16. The system must never invent missing academic content.
17. Ollama must not become the deterministic content-normalization engine.
18. AI suggestions must never bypass human approval.
19. Processing must be observable and auditable.
20. Reprocessing must preserve historical processing lineage.
21. Authorization must be enforced server-side.
22. Processing must be resilient to worker failure.
23. Security controls must protect the boundary between untrusted documents and trusted academic data.

---

# 87. Final Processing Principle

CRC's content-processing architecture treats every uploaded academic document as **untrusted source material** and every published question as **reviewed academic data**.

The complete trust transition is:

```text id="g8k2qy"
                UNTRUSTED
              SOURCE FILE
                   │
                   ▼
             VALIDATION
                   │
                   ▼
          SECURE PROCESSING
                   │
                   ▼
          OCR / STRUCTURE
                   │
                   ▼
          NORMALIZED OUTPUT
                   │
                   ▼
        CANDIDATE QUESTIONS
                   │
                   ▼
           TEACHER REVIEW
                   │
                   ▼
          TEACHER APPROVAL
                   │
                   ▼
              PUBLISHED
                   │
                   ▼
          ASSESSMENT-READY
                   │
                   ▼
       ONLINE + LOCAL ACADEMIC
             OPERATIONS
```

The processing system exists to accelerate academic content preparation while preserving human academic authority, provenance, security, reproducibility, and historical integrity.

**The machine extracts. The teacher validates. The system versions. Only approved content becomes authoritative.**
