# Deployment Architecture

## 1. Purpose

This document defines the deployment architecture for CRC across its online/cloud environment and the school's local Windows server environment.

The existing deployment path and installer filename retain the NAKAMA provider name. These identifiers refer to CRC deployment artifacts and are unchanged by this product-identity correction.

The deployment architecture must support:

* reliable production deployment;
* secure configuration;
* repeatable installation;
* Windows local-server deployment;
* online service deployment;
* database migrations;
* versioned releases;
* safe updates;
* rollback;
* backups;
* recovery;
* health monitoring;
* operational diagnostics.

Deployment must preserve the architectural separation between:

```text
Online Master System
        ↕
Synchronization
        ↕
School Local Server
        ↓
School LAN Clients
```

---

# 2. Deployment Principles

CRC deployment follows these principles:

1. Production deployments must be repeatable.
2. Configuration must be externalized from application code.
3. Secrets must never be committed to source control.
4. Database migrations must be versioned.
5. Releases must be identifiable and reproducible.
6. Local-server updates must preserve school data.
7. Updates must support rollback or recovery.
8. Core academic operation must not depend on AI availability.
9. The local server must not be publicly exposed.
10. Deployment must include health verification.
11. Backup must precede destructive migrations.
12. Production configuration must be validated before startup.
13. Deployment artifacts must be integrity-checked.
14. Environment-specific configuration must never be hardcoded.

---

# 3. Deployment Environments

The system should distinguish at minimum:

```text
Development
    ↓
Test / CI
    ↓
Staging
    ↓
Production
```

The local school installation is a production environment.

The online system is also a production environment but operates independently from individual school-local installations.

---

# 4. Production Topology

The production architecture is:

```text
                         INTERNET
                            │
                            ▼
                 ┌─────────────────────┐
                 │ ONLINE MASTER SYSTEM │
                 │ Application Services │
                 │ PostgreSQL           │
                 │ Content Processing   │
                 │ Online Dashboards    │
                 └──────────┬──────────┘
                            │
                     Secure Sync
                            │
                            ▼
                 ┌─────────────────────┐
                 │ SCHOOL LOCAL SERVER │
                 │ Windows             │
                 │ LMSServer.exe       │
                 │ SQLite              │
                 │ Sync Engine         │
                 │ Ollama              │
                 └──────────┬──────────┘
                            │
                         SCHOOL LAN
                            │
              ┌─────────────┼─────────────┐
              ▼             ▼             ▼
          Students       Teachers      Admins
```

---

# 5. Online Production Environment

The online production environment hosts:

* online application;
* online API;
* PostgreSQL;
* content-processing services;
* online dashboards;
* synchronization services;
* analytics;
* administrative services required by the architecture.

The online environment must be isolated from direct school-local database access.

School local servers communicate through supported APIs and synchronization protocols.

---

# 6. Local Production Environment

The school-local environment runs on a Windows server PC.

The primary application package is:

```text
LMSServer.exe
```

`LMSServer.exe` acts as the local server manager/orchestrator.

It controls or supervises the local services required for school operation.

---

# 7. Local Server Components

The local deployment may include:

```text
LMSServer.exe
    │
    ├── Local LMS Backend
    ├── SQLite Database
    ├── Sync Worker
    ├── AI Gateway
    ├── Ollama
    ├── Background Jobs
    ├── Watchdog
    ├── Logging
    └── Health Monitoring
```

Components should remain logically separated even when distributed through a single installer. ADR-0002 establishes only the Module 00 self-contained CRC.Api Windows publishing/service-host foundation. LMSServer.exe remains the Module 05 manager; installer, service registration, production secrets and commissioning are not delivered by Module 00.

---

# 8. Windows Installation

The local server installer should provide a controlled installation process.

The installer should:

1. verify operating-system compatibility;
2. verify required runtime dependencies;
3. verify available disk space;
4. verify required ports;
5. create application directories;
6. create protected data directories;
7. install application components;
8. initialize local configuration;
9. initialize the database;
10. configure firewall rules;
11. register required services;
12. generate or register installation identity;
13. perform health checks;
14. present the local server management interface.

---

# 9. Installation Directories

The implementation should separate:

```text
Application Files
Configuration
Database
Backups
Logs
AI Models
Temporary Files
Uploaded/Local Content
```

Application binaries must not be stored in the same directory as mutable production data.

This allows application updates without overwriting school data.

---

# 10. Example Local Storage Layout

A conceptual layout is:

```text
C:\NAKAMA\
    app\
    config\
    data\
    backups\
    logs\
    ai\
    uploads\
    temp\
```

The final path must be configurable.

The application must not assume that the system drive is the only available storage location.

---

# 11. Local Database Deployment

The local database is SQLite using WAL mode.

The database file must reside in the protected application data area.

The deployment must configure:

* WAL mode;
* appropriate busy timeout;
* integrity checks;
* backup procedures;
* migration version;
* database permissions.

The application must not place the SQLite database in a temporary directory.

---

# 12. Online Database Deployment

The online production database is PostgreSQL.

Production PostgreSQL deployment must provide:

* secure credentials;
* restricted network access;
* automated backups;
* migration management;
* monitoring;
* storage monitoring;
* connection management;
* recovery procedures.

The application must not rely on manually editing production database tables.

---

# 13. Database Migrations

Database schema changes must use versioned migrations.

A migration must be:

* uniquely identified;
* ordered;
* repeatable or safely one-time;
* tested;
* recorded as applied.

Production deployments must verify migration state before starting the application.

---

# 14. Migration Strategy

Prefer:

```text id="1p5w9d"
Expand
  ↓
Deploy Compatible Code
  ↓
Migrate Data
  ↓
Adopt New Schema
  ↓
Remove Legacy Structure
```

Avoid destructive schema changes in the same deployment where they are not necessary.

---

# 15. Local Migration Safety

Before applying a local production migration:

1. verify database health;
2. verify sufficient disk space;
3. create a backup/snapshot;
4. verify backup integrity where practical;
5. stop conflicting background jobs;
6. apply migration;
7. run integrity checks;
8. start application;
9. run health checks.

---

# 16. Online Migration Safety

Online migrations must be tested against a representative production-sized dataset before release.

Long-running migrations must be assessed for:

* locks;
* downtime;
* storage growth;
* transaction duration;
* rollback complexity.

---

# 17. Configuration Management

Configuration must be externalized.

Configuration categories include:

```text
Server
Network
Database
Authentication
Synchronization
AI
Storage
Logging
Backups
Security
Performance
```

Application binaries must not contain production secrets.

---

# 18. Configuration Validation

At startup, the application must validate required configuration.

Examples:

* database path;
* server port;
* network binding;
* online endpoint;
* synchronization credentials;
* backup location;
* AI configuration.

Invalid configuration should prevent unsafe startup and provide a clear diagnostic.

---

# 19. Secrets

Secrets include:

* database credentials;
* synchronization credentials;
* installation credentials;
* signing keys;
* encryption keys;
* API secrets.

Secrets must be:

* protected at rest;
* protected in transit;
* excluded from source control;
* excluded from ordinary logs;
* inaccessible to unauthorized users.

The server manager must never display secret values in plaintext.

---

# 20. Environment Separation

Development credentials must never be reused in production.

Each environment must have separate:

* databases;
* credentials;
* API keys;
* signing material;
* configuration;
* synchronization identities.

---

# 21. Release Versioning

Every production release must have a unique version.

A release should identify:

```text
Application Version
Database Schema Version
API Version
AI Configuration Version where applicable
```

A local server installation must be able to report its installed version.

---

# 22. Release Artifact

A production release should contain a traceable artifact such as:

```text
NAKAMA-LocalServer-<version>.exe
```

The artifact should be accompanied by:

* release metadata;
* checksum;
* release notes;
* migration information;
* compatibility requirements.

---

# 23. Artifact Integrity

Production deployment artifacts should be integrity-checked.

The deployment process SHOULD use:

* cryptographic checksums;
* signed artifacts where supported;
* trusted release channels.

The local server should reject or warn on invalid update packages.

---

# 24. CI/CD

The development pipeline should automate:

```text
Commit
  ↓
Build
  ↓
Unit Tests
  ↓
Static Checks
  ↓
Security Checks
  ↓
Integration Tests
  ↓
Artifact Creation
  ↓
Staging Deployment
  ↓
Acceptance Tests
  ↓
Production Release
```

A failed mandatory validation must prevent promotion to the next environment.

---

# 25. Build Reproducibility

Production artifacts should be generated from a known source revision.

The release must record:

* source revision;
* build timestamp;
* version;
* dependency state;
* build environment where required.

---

# 26. Dependency Management

Dependencies must be:

* explicitly versioned;
* vulnerability-checked;
* reviewed before major upgrades;
* reproducibly installed.

Uncontrolled production dependency installation is prohibited.

---

# 27. Local Server Startup

On Windows startup:

```text id="e7q3k8"
Windows
   ↓
LMSServer
   ↓
Dependency Check
   ↓
Database Check
   ↓
Configuration Check
   ↓
Core Services
   ↓
AI / Optional Services
   ↓
Health Check
   ↓
Ready
```

Core LMS services should become available independently of optional AI readiness.

---

# 28. Startup Failure Handling

If a critical dependency fails:

* startup must report the failure;
* logs must identify the cause;
* the system must enter a safe state;
* watchdog behavior must be applied where appropriate.

If an optional subsystem such as AI fails, the LMS should continue operating.

---

# 29. Local Server Manager

The server manager should provide:

* server status;
* start/stop controls;
* restart controls;
* LAN address;
* configured port;
* connected-device information;
* database health;
* synchronization status;
* AI health;
* backup controls;
* configuration;
* logs;
* diagnostics;
* installed version;
* update status.

Administrative actions must require authentication.

---

# 30. Windows Service Model

Core local services SHOULD run as controlled Windows services or managed background processes rather than relying on a manually opened console window.

The deployment should support:

* automatic startup;
* controlled shutdown;
* restart on failure;
* service status;
* restricted service account privileges.

---

# 31. Least-Privilege Service Accounts

Local services should operate using the minimum Windows privileges required.

The LMS application must not require unrestricted administrator privileges during normal operation.

Administrative privileges should be reserved for installation and controlled maintenance operations.

---

# 32. Firewall

The local server deployment must configure Windows Firewall to permit only required traffic.

The LMS port should be accessible only from the intended school LAN.

No automatic public exposure should be created.

---

# 33. Network Binding

The local application must bind to the intended LAN interface.

It should not automatically bind to every available interface unless explicitly configured.

Public or unintended network interfaces must not expose the LMS.

---

# 34. Local Hostname

The school-local deployment should use a private LAN hostname rather than a public internet hostname.

Example:

```text
lms.school.local
```

The exact hostname is configurable.

Resolution may use:

* local DNS;
* controlled hosts configuration;
* another private LAN name-resolution mechanism.

---

# 35. No Port Forwarding

The local server must not require internet router port forwarding.

Internet communication should originate through the configured secure synchronization mechanism.

This keeps the local LMS outside direct public exposure.

---

# 36. TLS

Where practical, LAN traffic should use HTTPS/TLS.

TLS configuration should be:

* modern;
* certificate-validated;
* centrally managed;
* renewable.

If a controlled local deployment uses another secure transport architecture, the security decision must be documented.

---

# 37. Online Deployment Security

The online production environment must use:

* HTTPS;
* secure headers;
* restricted administrative access;
* network segmentation where appropriate;
* protected databases;
* centralized logging;
* backup;
* monitoring.

Production administrative endpoints must not be publicly accessible without appropriate authentication and authorization.

---

# 38. Backup Architecture

Backups must exist for:

### Online

* PostgreSQL;
* required application configuration;
* required authoritative storage.

### Local

* SQLite database;
* required local configuration;
* audit data;
* locally retained academic data;
* required synchronization state.

AI-derived vector indexes may be rebuilt and therefore do not necessarily need to be included in every backup.

---

# 39. Backup Schedule

Backup frequency must be configured according to data criticality.

At minimum, the system should support:

* scheduled backups;
* manual backup;
* backup status;
* backup history;
* retention;
* integrity verification.

---

# 40. Local Backup Storage

Local backups must not exist only on the same physical disk as the live database.

Where practical, backups should be copied to separate storage.

A backup that disappears when the server disk fails is not a sufficient disaster-recovery strategy.

---

# 41. Backup Encryption

Backups containing sensitive school data should be encrypted.

Encryption keys must be protected separately from the backup data.

---

# 42. Restore Testing

Backups are not considered reliable until restoration is tested.

Restore tests should verify:

* database integrity;
* application startup;
* academic records;
* authentication;
* synchronization state;
* audit data.

---

# 43. Update Strategy

Local server updates should be controlled through the Local Server Manager.

Update flow:

```text id="4c8x7k"
Update Available
      ↓
Compatibility Check
      ↓
Backup
      ↓
Download / Import Artifact
      ↓
Integrity Verification
      ↓
Maintenance Mode
      ↓
Database Migration
      ↓
Application Update
      ↓
Health Checks
      ↓
Ready
```

---

# 44. Maintenance Mode

Updates that require service interruption should place the local server into maintenance mode.

Users should receive a clear status message rather than seeing unexplained connection failures.

---

# 45. Update Ordering

Updates must respect dependency order.

Recommended order:

```text
Backup
  ↓
Database Migration
  ↓
Core Services
  ↓
Background Workers
  ↓
Optional Services
  ↓
Health Checks
```

The exact order may differ for a particular release but must be defined and tested.

---

# 46. Rollback

Every production update must have a documented recovery path.

Rollback may involve:

* restoring previous application binaries;
* reverting compatible migrations;
* restoring database backup;
* restoring configuration;
* restarting services.

Destructive irreversible migrations must be avoided unless a tested recovery strategy exists.

---

# 47. Failed Update

If an update fails:

1. stop further deployment;
2. preserve diagnostic logs;
3. prevent partial startup where unsafe;
4. verify database state;
5. attempt controlled recovery;
6. restore backup if required;
7. verify core LMS operation;
8. record the incident.

---

# 48. Version Compatibility

The local server must verify compatibility between:

* application version;
* database schema;
* synchronization protocol;
* online server version;
* local AI integration where applicable.

Incompatible combinations must be rejected or explicitly handled.

---

# 49. API Compatibility

API changes should follow the API versioning architecture.

Breaking changes require:

* new API version;
* migration period;
* compatibility strategy;
* client update strategy.

---

# 50. Synchronization Compatibility

Before enabling synchronization after an update, the local server should perform a compatibility handshake.

The handshake should verify:

* protocol version;
* installation identity;
* supported capabilities;
* schema compatibility;
* authentication credentials.

---

# 51. Local Server Registration

Each production local installation must have a unique installation identity.

Registration should associate the installation with the intended school deployment.

The identity must not be based solely on:

* hostname;
* IP address;
* Windows computer name.

---

# 52. Installation Replacement

If a school replaces its server PC:

```text id="x2v8q6"
New Windows Server
       ↓
Install LMSServer
       ↓
Register Replacement Installation
       ↓
Restore / Initialize Data
       ↓
Verify Identity
       ↓
Sync / Reconcile
       ↓
Return to Service
```

The old installation identity must be retired or revoked.

---

# 53. Configuration Backup

Important local configuration should be recoverable.

This includes:

* network settings;
* synchronization configuration;
* backup settings;
* server configuration;
* AI configuration;
* logging configuration.

Secrets must remain protected and must not be exported as plaintext.

---

# 54. Health Checks

The local server must expose internal health checks for:

* application;
* database;
* storage;
* synchronization;
* AI;
* background jobs;
* configuration;
* disk capacity.

Health status should distinguish:

```text
Healthy
Degraded
Failed
Unknown
```

---

# 55. Readiness vs Liveness

The deployment should distinguish:

### Liveness

Is the service running?

### Readiness

Is the service capable of safely serving requests?

For example, a process may be alive while the database is unavailable.

It should therefore be:

```text
Liveness: Healthy
Readiness: Failed
```

---

# 56. Logging

Production deployment must preserve structured logs for:

* startup;
* shutdown;
* deployment;
* migration;
* authentication;
* synchronization;
* database;
* AI;
* errors;
* security events.

Logs must not contain secrets.

---

# 57. Log Retention

Log retention must be configurable.

The system should support:

* rotation;
* size limits;
* age limits;
* severity filtering;
* export;
* secure deletion.

This prevents logs from consuming all available disk space.

---

# 58. Monitoring

Production monitoring should track:

* service availability;
* database health;
* disk space;
* CPU;
* memory;
* sync health;
* AI health;
* backup status;
* error rates;
* request latency.

---

# 59. Alerting

Important conditions should generate administrative alerts.

Examples:

* repeated service crashes;
* backup failure;
* low disk space;
* database integrity failure;
* synchronization failure;
* authentication attack indicators;
* failed migration;
* certificate expiry;
* AI resource exhaustion.

---

# 60. Deployment Diagnostics

The Local Server Manager should provide a diagnostic bundle capability.

A diagnostic bundle may include:

* application version;
* configuration summary;
* service states;
* health results;
* recent logs;
* database status;
* sync status.

It must exclude:

* passwords;
* tokens;
* private keys;
* credential hashes;
* unnecessary personal data.

---

# 61. Production Smoke Tests

After deployment, automated or guided smoke tests should verify:

* login;
* role authorization;
* database access;
* student dashboard;
* teacher dashboard;
* assessment access;
* practice access;
* scoring;
* local hostname access;
* synchronization;
* backup;
* AI health where enabled.

---

# 62. Local Offline Smoke Test

A local deployment is not accepted until internet connectivity can be intentionally disabled and the following remain functional:

* student login;
* teacher login where applicable;
* practice;
* CBT;
* answer persistence;
* scoring;
* progress recording;
* local dashboards required for operation.

---

# 63. Deployment Security Testing

Production deployment must verify:

* firewall rules;
* exposed ports;
* TLS;
* service privileges;
* filesystem permissions;
* secret protection;
* update integrity;
* backup protection;
* administrative access.

External exposure scanning should confirm that the local server is not unintentionally reachable from the public internet.

---

# 64. Penetration Testing

Before production acceptance, security testing should include the deployed architecture.

Testing should cover:

* online application;
* APIs;
* authentication;
* authorization;
* local LAN server;
* synchronization;
* file uploads;
* administrative interfaces;
* update mechanisms;
* exposed services.

Penetration testing forms part of the project's security acceptance process.

---

# 65. Disaster Recovery

The deployment architecture must define recovery procedures for:

* server disk failure;
* Windows corruption;
* database corruption;
* application corruption;
* failed update;
* lost network;
* prolonged internet outage;
* online database failure;
* local server replacement.

---

# 66. Local Server Disaster Recovery

For local server failure:

```text id="j5v0t4"
Replace / Repair Windows PC
          ↓
Install LMSServer
          ↓
Restore Backup
          ↓
Verify Database
          ↓
Verify Configuration
          ↓
Verify Installation Identity
          ↓
Run Sync Reconciliation
          ↓
Health Check
          ↓
Return to LAN
```

---

# 67. Data Recovery Priority

Recovery priority should be:

1. authoritative academic data;
2. identity/access data;
3. assessment state/results;
4. synchronization state;
5. audit records;
6. configuration;
7. derived analytics;
8. AI-derived indexes/caches.

Derived data may be rebuilt after core data recovery.

---

# 68. Recovery Point Objective

The final production configuration must define an acceptable maximum amount of data that may be lost after a catastrophic failure.

The RPO should be configured based on:

* local backup frequency;
* synchronization frequency;
* assessment criticality.

---

# 69. Recovery Time Objective

The deployment plan must define an acceptable recovery duration for:

* local school operation;
* online services.

Targets must be validated through practical recovery exercises rather than assumed.

---

# 70. Deployment Documentation

Every production release should document:

* release version;
* deployment date;
* changes;
* migrations;
* configuration changes;
* compatibility;
* known issues;
* rollback procedure;
* validation results.

---

# 71. Change Management

Production changes must be traceable.

Changes should identify:

* requester;
* reason;
* affected component;
* implementation;
* validation;
* rollback;
* completion status.

Emergency changes should be documented retrospectively if immediate action is required.

---

# 72. Production Access

Production administrative access must follow least privilege.

Access should be limited to authorized personnel.

Administrative actions should be auditable.

Shared administrator credentials should not be used.

---

# 73. Local Physical Security

The local server PC is part of the security boundary.

The deployment environment should provide:

* restricted physical access;
* controlled Windows accounts;
* automatic screen locking;
* protected storage;
* reliable power;
* UPS where practical.

Physical access can bypass many software controls and must therefore be treated as a security concern.

---

# 74. Power Protection

The local server should ideally operate behind a UPS.

The deployment should support graceful shutdown during prolonged power failure where hardware/software integration permits.

SQLite WAL and transactional operations must minimize corruption risk during unexpected shutdown.

---

# 75. Capacity Planning

Before deployment, the school server should be evaluated for:

* CPU;
* RAM;
* storage;
* network interface;
* available disk space;
* GPU/VRAM where local AI is enabled.

AI requirements must be considered separately from core LMS requirements.

---

# 76. Storage Monitoring

The local server must monitor storage capacity.

Warnings should occur before the disk becomes critically full.

The system must prioritize protecting:

* database;
* logs;
* synchronization queue;
* backups.

AI caches and other rebuildable data may be cleaned according to controlled policies.

---

# 77. Deployment Acceptance

A production deployment is accepted only after:

* application health checks pass;
* database health passes;
* security checks pass;
* firewall is verified;
* backups are verified;
* migration status is correct;
* synchronization works;
* offline operation works;
* core workflows pass;
* logging works;
* monitoring works;
* rollback/recovery procedure is documented.

---

# 78. Definition of Done

Deployment architecture is complete when:

* online deployment is defined;
* local Windows deployment is defined;
* `LMSServer.exe` packaging is defined;
* configuration management exists;
* secret management exists;
* migrations are versioned;
* release versioning exists;
* artifact integrity is checked;
* local firewall configuration is defined;
* private LAN access is defined;
* TLS is addressed;
* backup and restore are defined;
* update and rollback are defined;
* health checks exist;
* logging and monitoring exist;
* disaster recovery is documented;
* production smoke testing exists;
* security deployment testing exists;
* penetration testing is included;
* offline acceptance testing exists.

---

# 79. Non-Negotiable Rules

1. Production configuration must never be hardcoded.
2. Secrets must never be committed to source control.
3. Local server data must survive application updates.
4. Database migrations must be versioned.
5. Production migrations must be tested.
6. Backups must exist before risky migrations.
7. Backups must be tested through restoration.
8. The local server must not be publicly exposed.
9. No automatic router port forwarding may be required.
10. Firewall rules must restrict local server access.
11. Services must use least privilege.
12. Application binaries and mutable production data must be separated.
13. Every release must be versioned.
14. Production artifacts must be integrity-checked.
15. Failed deployments must have a recovery path.
16. Health checks must distinguish liveness from readiness.
17. AI failure must not prevent core LMS startup.
18. Local operation must survive internet loss.
19. Synchronization compatibility must be checked before sync resumes.
20. Deployment must not be considered complete until security, backup, recovery, and offline tests pass.

---

# 80. Final Deployment Principle

CRC deployment must make the production system predictable, recoverable, secure, and maintainable.

The deployment lifecycle is:

```text
Build
  ↓
Validate
  ↓
Package
  ↓
Backup
  ↓
Deploy
  ↓
Migrate
  ↓
Health Check
  ↓
Smoke Test
  ↓
Monitor
  ↓
Recover / Roll Back if Required
```

The fundamental principle is:

> **A production deployment is not complete when the software starts; it is complete when the system has been verified, secured, monitored, backed up, and proven recoverable.**
