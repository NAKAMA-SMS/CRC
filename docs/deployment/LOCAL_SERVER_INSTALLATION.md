# Local Server Installation & Commissioning

## 1. Purpose

This document defines the standard procedure for installing, configuring, securing, validating, and commissioning the CRC local school server.

The existing deployment path and installer filename retain the NAKAMA provider name. These identifiers refer to CRC deployment artifacts and are unchanged by this product-identity correction.

The local server provides the school's offline-capable LMS environment.

The production local-server installation consists primarily of:

```text id="a8v2kq"
Windows Server PC
        ↓
LMSServer.exe
        ↓
Local LMS Services
        ├── Local API
        ├── SQLite Database
        ├── Sync Engine
        ├── AI Gateway
        ├── Ollama
        ├── Background Jobs
        ├── Watchdog
        └── Monitoring
```

The installation process must result in a secure and operational school-local LMS environment.

---

# 2. Scope

This document covers:

* server hardware preparation;
* Windows preparation;
* application installation;
* directory setup;
* `LMSServer.exe` installation;
* local database initialization;
* network configuration;
* private LAN hostname;
* firewall configuration;
* TLS configuration;
* installation registration;
* online connection configuration;
* synchronization setup;
* Ollama setup;
* AI configuration;
* backup configuration;
* health checks;
* first administrator configuration;
* LAN client verification;
* offline verification;
* commissioning;
* handover;
* troubleshooting;
* uninstall/replacement procedures.

This document does not define the complete application deployment architecture.

That is covered by:

`docs/deployment/DEPLOYMENT_ARCHITECTURE.md`

---

# 3. Installation Principle

The installation process must follow:

```text id="j7c1m4"
Prepare
  ↓
Install
  ↓
Configure
  ↓
Secure
  ↓
Initialize
  ↓
Register
  ↓
Validate
  ↓
Test Offline
  ↓
Commission
  ↓
Handover
```

A server must not be considered production-ready simply because `LMSServer.exe` launches successfully.

---

# 4. Production Installation Requirements

Before installation, confirm that the school has:

* dedicated Windows server PC;
* stable local power;
* preferably UPS protection;
* wired Ethernet connection;
* school LAN;
* administrator access to the Windows machine;
* approved server location;
* sufficient storage;
* sufficient RAM;
* appropriate CPU;
* GPU/VRAM if local AI requirements require it;
* network equipment capable of supporting the expected number of clients;
* approved school network addressing;
* access to the online CRC environment for registration and synchronization.

---

# 5. Server Hardware Verification

Before software installation, record the server specifications.

At minimum:

```text id="c6q8n0"
CPU
RAM
Storage
Operating System
Network Interface
GPU
GPU VRAM
```

Where local AI is enabled, GPU and VRAM information is especially important.

The hardware configuration should be recorded as part of the deployment record.

---

# 6. Windows Requirements

The server must use a supported Windows version defined by the project's deployment compatibility matrix.

Windows must be:

* properly activated;
* updated;
* free from known critical vulnerabilities;
* configured with a dedicated administrative account;
* protected by Windows security controls;
* configured with the correct timezone;
* configured with reliable system time.

The exact supported Windows version must be pinned before production deployment.

---

# 7. Windows Account Model

The server should use separate Windows accounts for:

### Installation Administrator

Used for:

* installation;
* Windows configuration;
* service installation;
* firewall configuration;
* maintenance.

### Runtime Service Identity

Used by application services where practical.

The runtime identity should have only the permissions required to operate the LMS.

---

# 8. Windows Updates

Before production installation:

1. install required Windows updates;
2. reboot;
3. verify the system is healthy;
4. verify no critical update remains pending;
5. confirm system time;
6. confirm network connectivity.

Do not begin application commissioning while Windows is in an unstable update/reboot state.

---

# 9. Windows Security Preparation

Before installing CRC:

* enable Windows Firewall;
* keep Microsoft Defender or approved endpoint protection enabled;
* disable unnecessary services;
* remove unnecessary software;
* remove unused user accounts;
* enable automatic screen locking;
* restrict physical access;
* avoid installing unrelated applications.

The server should be dedicated to its production role wherever practical.

---

# 10. Server Time

Accurate system time is required for:

* authentication;
* sessions;
* CBT timing;
* audit records;
* synchronization;
* backups;
* logs;
* TLS.

The server must use a reliable time source.

Time drift must be monitored.

---

# 11. Network Preparation

Connect the server to the school's LAN using a reliable wired connection where possible.

The server should have a stable network address.

The deployment should prefer a DHCP reservation or another controlled addressing mechanism rather than relying on an arbitrary dynamically changing address.

---

# 12. LAN Network Requirements

The local server must be reachable by authorized school devices on the LAN.

The following must be known before commissioning:

```text id="g7d1p4"
Server IP
Subnet
Gateway
DNS Configuration
LMS Port
Local Hostname
```

The final values must be recorded in the deployment record.

---

# 13. Local Hostname

The local LMS should be accessible through a private LAN hostname.

Example:

```text id="p9x2c8"
lms.school.local
```

The actual hostname must be configured according to the school's local DNS/name-resolution strategy.

The hostname must not imply public internet exposure.

---

# 14. Public Exposure Prohibition

The school-local LMS server must not be exposed directly to the public internet.

Do not:

* configure router port forwarding;
* expose the LMS port publicly;
* publish the local server's private address;
* create unnecessary public DNS records.

Online communication should occur through the approved synchronization architecture.

---

# 15. Firewall Configuration

Windows Firewall must allow only required inbound connections.

At minimum, access should be limited to:

```text id="v7k3m1"
School LAN
     ↓
LMS Port
     ↓
LMSServer
```

Unnecessary inbound ports must remain blocked.

The installer should create only documented firewall rules.

---

# 16. Application Directory

Create a dedicated application root.

Example:

```text id="u1c7p9"
C:\NAKAMA\
```

The final location may be changed during installation.

The following directories should be separated:

```text id="y8j2q0"
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

Application binaries must not be stored alongside mutable database files.

---

# 17. Installation Package

The production installation package should contain the approved release artifact.

Example:

```text id="r5n8k3"
NAKAMA-LocalServer-<version>.exe
```

Before installation:

1. verify release version;
2. verify checksum/signature where available;
3. confirm release compatibility;
4. confirm database migration requirements;
5. confirm minimum Windows requirements.

Do not install unverified builds.

---

# 18. Installing LMSServer.exe

Run the installer using the authorized Windows installation account.

The installer should:

1. display the release version;
2. validate prerequisites;
3. request the installation directory;
4. create required directories;
5. install application files;
6. install required runtime components;
7. register required services;
8. create firewall rules;
9. create protected data directories;
10. initialize configuration;
11. provide the initial server-management interface.

---

# 19. Installation Permissions

Installation may require Windows administrative privileges.

Once installation is complete, normal LMS operation should not require users to run the application as Windows Administrator.

The application must use least privilege during runtime.

---

# 20. Initial Configuration

After installation, open the Local Server Manager.

The first configuration should establish:

* server identity;
* local hostname;
* LAN binding;
* port;
* database path;
* backup path;
* online server URL;
* synchronization configuration;
* logging;
* AI configuration;
* storage configuration.

---

# 21. Configuration Validation

The Local Server Manager must validate configuration before enabling production operation.

Invalid configuration should identify:

* setting;
* reason;
* severity;
* corrective action.

The server must not silently fall back to unsafe defaults.

---

# 22. Database Initialization

The first installation must initialize the local SQLite database.

The process should:

```text id="h6c2w4"
Create Database
      ↓
Enable WAL
      ↓
Apply Schema
      ↓
Apply Seed Configuration
      ↓
Run Integrity Check
      ↓
Ready
```

The database must not be manually created by editing SQLite files.

---

# 23. SQLite Configuration

The local database must use the approved SQLite configuration.

Required considerations include:

* WAL mode;
* transaction safety;
* busy timeout;
* foreign-key enforcement;
* integrity checks;
* backup compatibility.

The final production configuration must be tested under realistic school workloads.

---

# 24. Database Migration

If the installer detects an existing database:

```text id="p4r8x2"
Existing Database
       ↓
Read Schema Version
       ↓
Check Compatibility
       ↓
Backup
       ↓
Apply Migration
       ↓
Verify Integrity
```

Never overwrite an existing production database without an explicit migration or restoration procedure.

---

# 25. Installation Identity

Every school installation must have a unique installation identity.

The identity should be generated or securely provisioned during installation.

It must not depend solely on:

* hostname;
* IP address;
* Windows computer name.

The installation identity is used by synchronization and deployment management.

---

# 26. School Registration

The local server must be associated with the correct school.

Registration should establish:

```text id="f1z8w3"
School
   ↓
Installation
   ↓
Local Server Identity
```

An installation must not be able to impersonate another school's local server.

---

# 27. Registration Credentials

Registration credentials must be protected.

They must not be:

* hardcoded;
* written to plain-text logs;
* displayed after provisioning;
* included in diagnostic bundles.

After registration, the server should use its assigned secure machine/installation credentials.

---

# 28. Online Server Configuration

Configure the approved online/master server endpoint.

The configuration should include:

* online base URL;
* API version;
* synchronization endpoint;
* connection timeout;
* retry policy;
* synchronization credentials;
* TLS verification configuration.

The exact production endpoint must come from deployment configuration.

---

# 29. Online Connectivity Test

Run the server's online connectivity test.

The test should verify:

* DNS resolution;
* TLS connection;
* endpoint reachability;
* authentication;
* installation authorization;
* protocol compatibility.

A successful network ping alone is not sufficient.

---

# 30. Synchronization Initialization

After registration:

```text id="b4k7x1"
Local Server
      ↓
Sync Handshake
      ↓
Compatibility Check
      ↓
Initial Data Assessment
      ↓
Initial Synchronization
      ↓
Integrity Verification
```

Initial synchronization must not blindly overwrite local data.

---

# 31. Initial Synchronization Rules

The synchronization system must use the defined authority model.

It must respect:

* global IDs;
* revisions;
* tombstones;
* idempotency;
* conflict detection;
* dependency order.

The installer must never implement synchronization through direct database copying.

---

# 32. Initial Academic Data

After successful registration and synchronization, verify that expected academic structures are available:

* academic sessions;
* terms;
* classes;
* arms;
* subjects;
* topics;
* students;
* teachers;
* parent relationships;
* teacher assignments;
* approved question data where applicable.

---

# 33. Administrator Access

The Super Admin account must be provisioned through the approved identity process.

The local installation must not create an undocumented universal default administrator account.

If a bootstrap account is required, it must:

* be securely generated;
* require immediate credential setup;
* be auditable;
* be disabled or rotated after provisioning where applicable.

---

# 34. First Login

Any newly provisioned user with an initial password must be required to change that password during first login.

This applies to:

* students;
* teachers;
* parents;
* principals;
* administrators where applicable.

Initial passwords must never be reused across multiple accounts.

---

# 35. Authentication Verification

Test:

* valid login;
* invalid password;
* first-login password change;
* logout;
* session expiration;
* account deactivation;
* unauthorized access;
* role enforcement.

Authentication must work locally when the school is operating offline.

---

# 36. Local AI Installation

If local AI is enabled for the school deployment, install the approved Ollama runtime.

The AI runtime must be installed on the local server rather than on student client devices unless a future architecture explicitly requires otherwise.

---

# 37. Ollama Configuration

Configure:

* model storage location;
* service startup;
* resource limits;
* model availability;
* AI Gateway connection;
* logging.

The Ollama endpoint must not be directly exposed to LAN clients.

---

# 38. AI Model Installation

Install only approved models.

For each model record:

```text id="x8r1c5"
Model Name
Model Version
Size
Context Capability
Resource Requirement
Installation Date
```

The exact production model must be defined through the AI architecture/configuration rather than assumed by the installer.

---

# 39. AI Health Verification

Verify:

```text id="d3m7p0"
Ollama Running
      ↓
Model Available
      ↓
AI Gateway Healthy
      ↓
Test Inference
      ↓
Response Returned
```

AI health failure must not prevent the core LMS from operating.

---

# 40. RAG Index Initialization

If approved content is available:

```text id="q9v4h2"
Approved Content
      ↓
Embedding Generation
      ↓
Vector Index
      ↓
Retrieval Test
```

The vector index is derived data.

It may be rebuilt if necessary.

---

# 41. RAG Authorization Test

Perform a test confirming that:

* authorized users retrieve authorized content;
* unauthorized users do not retrieve restricted content;
* student scope is respected;
* teacher scope is respected;
* principal scope is respected.

This test is mandatory before AI commissioning.

---

# 42. AI CBT Restriction Test

Start or simulate an active CBT session.

Attempt an AI request.

Expected result:

```text id="s4h7k1"
AI Request
    ↓
CBT Active
    ↓
Request Rejected
```

This must be enforced by the server.

---

# 43. Backup Configuration

Configure the local backup system.

At minimum:

* backup location;
* schedule;
* retention;
* encryption;
* backup status;
* failure handling.

The backup destination should not be the same physical storage location as the live database where avoidable.

---

# 44. First Backup

After initial configuration:

1. run a manual backup;
2. verify completion;
3. verify backup integrity;
4. record backup location;
5. confirm the backup is restorable.

Do not commission a server without a verified initial backup.

---

# 45. Restore Test

Perform a controlled restore test before final handover.

Verify:

* database opens;
* schema is valid;
* users exist;
* academic records exist;
* assessment records exist;
* configuration is recoverable;
* application starts.

The restored test environment must not accidentally become the production instance.

---

# 46. Logging Configuration

Verify that production logs are:

* enabled;
* structured;
* timestamped;
* rotated;
* protected;
* free of secrets.

Verify that logs capture:

* startup;
* shutdown;
* authentication;
* database;
* synchronization;
* security events;
* errors;
* AI events.

---

# 47. Server Manager Verification

Verify that the Local Server Manager correctly reports:

```text id="g8k3r6"
Server Status
LAN Address
Port
Database Health
Sync Status
AI Status
Storage
Version
Logs
```

The reported values must correspond to the actual runtime state.

---

# 48. Endpoint Verification

The server's endpoint diagnostics should verify expected services.

The endpoint explorer must require administrative authentication.

Dangerous administrative endpoints must not be exposed as unrestricted public diagnostics.

---

# 49. LAN Client Test

Connect an authorized test device to the school LAN.

Open the configured private hostname.

Verify:

* DNS/name resolution;
* TLS if enabled;
* LMS page;
* login;
* dashboard;
* API communication.

The test device must not require internet connectivity.

---

# 50. Multi-Client Test

Test several devices simultaneously.

At minimum, simulate:

* student client;
* teacher client;
* administrator client.

Verify that concurrent access does not produce:

* database corruption;
* session collisions;
* incorrect authorization;
* severe performance degradation.

---

# 51. Student Offline Test

Disconnect the school network from the internet while maintaining LAN connectivity.

Verify that the student can:

* log in;
* access permitted learning content;
* practice;
* start an assessment;
* navigate questions;
* skip questions;
* change answers;
* have answers auto-saved;
* submit;
* receive a score;
* view progress.

---

# 52. CBT Recovery Test

During an active CBT:

1. start an attempt;
2. answer several questions;
3. close/crash the browser;
4. reopen the LMS;
5. reconnect to the attempt.

Expected behavior:

```text id="q5v9x1"
Existing Attempt
      ↓
Recover Session
      ↓
Restore Authoritative State
      ↓
Continue Attempt
```

The student's existing attempt must not be silently replaced with a new attempt.

---

# 53. Network Interruption Test

During a CBT, interrupt connectivity between a client and the local server.

Verify:

* client detects interruption;
* unsent local answer state is retained where applicable;
* reconnection occurs;
* authoritative state is reconciled;
* no duplicate answer events are created;
* timer behavior remains server-authoritative.

---

# 54. Internet Loss Test

Disconnect the local server from the internet while keeping the school LAN operational.

Verify that:

* student login works;
* practice works;
* CBT works;
* scoring works;
* local progress records;
* local dashboards required for operation work;
* synchronization reports disconnected state;
* no core workflow becomes blocked.

---

# 55. Internet Restoration Test

Restore internet access.

Verify:

```text id="p1k6s3"
Internet Restored
      ↓
Connectivity Detection
      ↓
Sync Handshake
      ↓
Queued Changes
      ↓
Synchronization
      ↓
Acknowledgement
      ↓
Healthy
```

No duplicate academic records should be created.

---

# 56. Synchronization Verification

After synchronization, verify:

* local changes reached the online system where required;
* online changes reached the local server where required;
* conflicts are detected correctly;
* failed changes are visible;
* retries work;
* audit records exist.

---

# 57. Promotion Safety Test

If promotion functionality is enabled for the deployment:

1. verify end-of-session data;
2. verify promotion candidates;
3. confirm Super Admin activation requirement;
4. verify no automatic promotion occurs before activation;
5. execute a controlled promotion test;
6. verify historical records remain unchanged.

---

# 58. Security Verification

Before commissioning, verify:

* firewall;
* network binding;
* private hostname;
* TLS;
* authentication;
* authorization;
* session security;
* service privileges;
* filesystem permissions;
* secret protection;
* endpoint exposure;
* logs;
* backups.

---

# 59. Port Verification

Check the Windows host for listening ports.

Only expected production services should be listening.

Unexpected listeners must be investigated before commissioning.

---

# 60. Local Network Exposure Verification

From an authorized LAN test device:

* verify expected LMS port;
* verify expected hostname;
* verify expected services.

From an external network:

* verify the local LMS is not publicly reachable.

The external test must be performed without exposing credentials or sensitive information.

---

# 61. Performance Baseline

Before handover, record baseline measurements for:

* login latency;
* dashboard response;
* question loading;
* answer saving;
* CBT navigation;
* assessment submission;
* database operations;
* synchronization;
* AI inference where enabled.

These measurements provide a baseline for future troubleshooting.

---

# 62. Capacity Baseline

Record:

```text id="j3n8x6"
CPU
RAM
Storage Used
Storage Available
GPU
GPU VRAM
Active Services
Connected Devices
```

This establishes the initial operating baseline.

---

# 63. Monitoring Baseline

Confirm that monitoring can detect:

* service failure;
* database failure;
* low disk space;
* sync failure;
* backup failure;
* AI failure;
* repeated authentication errors.

---

# 64. Update Test

Before production handover, verify the update process in a non-production environment.

Test:

* update package verification;
* backup;
* migration;
* service restart;
* health check;
* rollback/recovery.

Do not experiment with untested updates on the live school installation.

---

# 65. Commissioning Checklist

The server is ready for commissioning only when all required checks pass.

### Infrastructure

* [ ] Windows supported
* [ ] Windows updated
* [ ] Server hardware recorded
* [ ] UPS/power protection verified where available
* [ ] Wired LAN verified
* [ ] System time verified

### Application

* [ ] `LMSServer.exe` installed
* [ ] Correct release version
* [ ] Configuration validated
* [ ] Local database initialized
* [ ] Database integrity verified
* [ ] Services running

### Network

* [ ] Stable IP/addressing
* [ ] Private hostname configured
* [ ] Firewall configured
* [ ] Required port verified
* [ ] No public exposure
* [ ] TLS verified where configured

### Identity

* [ ] Installation identity created
* [ ] School registration completed
* [ ] Authentication verified
* [ ] Role authorization verified

### Synchronization

* [ ] Online connectivity verified
* [ ] Sync handshake successful
* [ ] Initial synchronization completed
* [ ] Retry behavior verified
* [ ] Conflict handling verified

### AI

* [ ] Ollama installed where enabled
* [ ] Approved model installed
* [ ] AI Gateway healthy
* [ ] RAG index available where required
* [ ] Authorization tested
* [ ] CBT AI blocking tested

### Backup

* [ ] Backup configured
* [ ] Initial backup completed
* [ ] Backup integrity verified
* [ ] Restore test completed

### Offline

* [ ] Student offline login tested
* [ ] Practice tested
* [ ] CBT tested
* [ ] Scoring tested
* [ ] Browser recovery tested
* [ ] LAN interruption tested
* [ ] Internet loss tested
* [ ] Internet restoration tested

### Security

* [ ] Firewall verified
* [ ] Service privileges verified
* [ ] Secrets protected
* [ ] Ports verified
* [ ] Administrative access protected
* [ ] Security checks completed

---

# 66. Commissioning Status

The final installation should have one of these states:

```text id="r0c6m8"
Not Installed
    ↓
Installed
    ↓
Configured
    ↓
Registered
    ↓
Testing
    ↓
Ready for Commissioning
    ↓
Production
```

A server must not be marked `Production` until all mandatory acceptance checks pass.

---

# 67. Handover

The handover record should contain:

* server name;
* installation identity;
* application version;
* database schema version;
* LAN hostname;
* server address;
* configured port;
* deployment date;
* backup status;
* synchronization status;
* AI status;
* responsible technical contact;
* operational notes.

Passwords and secrets must not be written into the handover document.

---

# 68. Administrator Training

The responsible school technical administrator should be shown how to:

* open the Local Server Manager;
* inspect server health;
* check synchronization;
* check backups;
* inspect logs;
* restart services;
* perform approved backups;
* identify common failures;
* contact technical support.

The administrator must not be given unnecessary access to protected secrets.

---

# 69. Routine Operations

After commissioning, routine operations should include:

### Daily

* server health;
* synchronization status;
* backup status;
* available storage.

### Periodically

* restore testing;
* Windows security updates;
* application updates;
* log review;
* security review;
* hardware health review.

The exact schedule must be documented in the operational maintenance plan.

---

# 70. Troubleshooting Priority

When the local LMS is unavailable, troubleshoot in this order:

```text id="w4j7c2"
Power
  ↓
Windows
  ↓
Network
  ↓
LMSServer Process
  ↓
Database
  ↓
Authentication
  ↓
Application
  ↓
Synchronization
  ↓
AI
```

AI should not be investigated first when core LMS services are unavailable.

---

# 71. Server Restart

A controlled restart should follow:

```text id="m9x3d7"
Notify Users
      ↓
Stop / Complete Active Operations
      ↓
Stop Application Services
      ↓
Windows Restart
      ↓
LMSServer Startup
      ↓
Database Check
      ↓
Service Health
      ↓
Sync Recovery
      ↓
Ready
```

Active CBT sessions must be protected by the recovery architecture.

---

# 72. Uninstall

Normal uninstallation must not automatically delete:

* production database;
* backups;
* audit records;
* synchronization state;
* required configuration.

Uninstallation should clearly distinguish:

```text id="z8f4m1"
Remove Application
```

from:

```text id="b5c2k9"
Destroy Production Data
```

Data destruction must require an explicit, controlled procedure.

---

# 73. Server Replacement

When replacing the physical server:

1. identify the existing installation;
2. verify current synchronization;
3. create a current backup;
4. install supported Windows;
5. install the approved CRC release;
6. restore required data;
7. register replacement installation;
8. retire the old installation identity;
9. perform reconciliation;
10. complete commissioning tests.

---

# 74. Lost or Compromised Server

If the local server is suspected to be compromised:

1. isolate it from the network;
2. preserve relevant logs;
3. notify the responsible technical/security personnel;
4. revoke or disable affected installation credentials;
5. determine whether data integrity is affected;
6. rebuild from a trusted installation;
7. restore verified data;
8. reconcile synchronization;
9. perform security validation;
10. return to production only after acceptance.

Do not simply reinstall over a suspected compromise without preserving required evidence and evaluating the incident.

---

# 75. Installation Failure

If installation fails:

* preserve installer logs;
* record the failure stage;
* do not repeatedly retry destructive operations;
* verify prerequisites;
* verify disk space;
* verify Windows permissions;
* verify network configuration;
* verify installer integrity.

Production data must never be destroyed as a troubleshooting shortcut.

---

# 76. Local Database Failure

If the local database reports corruption:

1. stop write operations;
2. preserve the database and logs;
3. run approved integrity diagnostics;
4. determine whether recovery is possible;
5. restore from the latest verified backup if required;
6. reconcile with the online system;
7. verify academic integrity;
8. resume service.

Do not manually modify SQLite files to "fix" records.

---

# 77. Synchronization Failure

If synchronization fails:

* verify internet connectivity;
* inspect sync status;
* inspect authentication;
* inspect protocol compatibility;
* inspect failed operations;
* allow configured retries;
* resolve conflicts through the sync system.

Do not bypass the sync engine by copying database files between environments.

---

# 78. AI Failure

If local AI fails:

* verify Ollama;
* verify model availability;
* verify AI Gateway;
* verify resource usage;
* verify storage;
* inspect logs.

Core LMS operation must continue.

---

# 79. Security Incident During Installation

If suspicious activity is detected during installation:

* stop the installation;
* isolate the server if necessary;
* preserve logs;
* do not continue until the issue is understood;
* rotate affected credentials;
* reinstall from a trusted artifact if required;
* repeat security validation.

---

# 80. Final Installation Principle

The local server is a production academic infrastructure component, not simply a desktop application.

A successful installation must establish:

```text id="e3r8v5"
Trusted Windows Environment
          ↓
Secure LMSServer Installation
          ↓
Protected Local Database
          ↓
Controlled LAN Access
          ↓
Registered School Installation
          ↓
Verified Synchronization
          ↓
Optional Local AI
          ↓
Verified Backup
          ↓
Offline Acceptance
          ↓
Production Commissioning
```

The fundamental principle is:

> **Do not hand over a local server because it is installed; hand it over only after its security, data integrity, offline operation, synchronization, backup, recovery, and production workflows have been verified.**
