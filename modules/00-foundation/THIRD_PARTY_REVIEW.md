# Module 00 dependency review

Reviewed 2026-10-02 against the locked package graphs and restored package metadata. Generated inventory: `artifacts/evidence/dependency-licenses.csv`. No unidentified third-party license remains in that inventory. Local CRC workspace links are not third-party packages.

The inventory includes MIT, Apache-2.0, BSD-2-Clause, BSD-3-Clause, ISC, PostgreSQL, BlueOak-1.0.0, MPL-2.0 and CC-BY-4.0 identifiers. These are recorded metadata, not a declaration that every possible redistribution has been approved.

- Backend/runtime dependencies include the Microsoft .NET/EF packages, Npgsql and SQLite provider/native dependencies. They implement the accepted hosting/provider boundary; no generic business framework was added.
- React/React DOM and their scheduler support the static client. Preserve their upstream license/copyright notices when packaging the client.
- Vite/TypeScript/lint/browser/test tools are development dependencies. The MPL-2.0 entries are Lightning CSS and its platform binaries; CC-BY-4.0 is caniuse-lite data. Those build-tool trees do not ship as a Node runtime in the school artifact. Do not redistribute those tools or modify their sources without preserving their applicable notices/obligations.
- Retain the .NET self-contained distribution notices and upstream package notices. Production distribution/signing remains outside Module 00; the release packaging gate must review the actual delivered notice bundle, including any later dependencies.

NuGet advisory inspection, npm audit, and Gitleaks history/working-tree scans were executed. See EVIDENCE.md for exact results and remaining acceptance gates; do not infer future advisory status from this dated review. No advisory suppression or license override was introduced.
