# Assessment Report: Newtonsoft.Json removal analysis

**Date**: 2026-02-05  
**Repository**: QuattoAPIClient (workspace root)  
**Analysis Mode**: Generic  
**Analyzer**: Modernization Analyzer Agent (GitHub Copilot)

---

## Executive Summary

This assessment evaluates the impact of removing the `Newtonsoft.Json` NuGet package from the repository and provides validation steps required to safely remove the package reference. A project-level `PackageReference` to `Newtonsoft.Json` was found only in `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`. A repository-wide code search found no usages of `Newtonsoft.Json` APIs (no `using Newtonsoft.Json`, `JsonConvert`, `JObject`, `JToken`, `Newtonsoft.Json.Linq`, `JsonSerializerSettings`, or `JsonProperty` attributes) in the scanned source files.

Based on the evidence, removing the package reference is low-risk, but standard validation (build and runtime testing) is required.

---

## Scenario Context

**Scenario Objective**: Remove `Newtonsoft.Json` package dependency when it is unused, after migrating to `System.Text.Json`.

**Analysis Scope**: All projects in the solution (three projects):
- `src/01_Source/QuattoAPIClient.Source.csproj`  
- `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`  
- `src/03_UI/QuattoAPIClient.UI.csproj`

Scanned files included all code files returned by project file listings and representative source files in each project.

**Methodology**: Inspect project files for `PackageReference` to `Newtonsoft.Json`. Perform repo-wide search for API usages of `Newtonsoft.Json` and its common types and helpers. Inspect config files for binding redirects.

---

## Current State Analysis

### Package references (project-level)

- `src/01_Source/QuattoAPIClient.Source.csproj`
  - `System.Data.SqlClient` 4.8.6
  - `System.Text.Json` 10.0.2
  - No `Newtonsoft.Json`

- `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`
  - `System.Text.Json` 10.0.2
  - `Newtonsoft.Json` 13.0.3  <-- only project-level reference found

- `src/03_UI/QuattoAPIClient.UI.csproj`
  - No `<PackageReference>` entries

### Code usage of Newtonsoft.Json APIs

- Searched tokens: `Newtonsoft.Json`, `JsonConvert`, `JObject`, `JToken`, `Newtonsoft.Json.Linq`, `JsonSerializerSettings`, `JsonProperty`, `JsonPropertyAttribute`.
- Result: No matches found in the scanned source files (samples inspected: `CorporateApiSource.cs`, `HttpHelper.cs`, `SchemaMapper.cs`, `ApiConnectionManager.cs`, `OAuth2TokenManager.cs`, `TokenRefreshHandler.cs`, UI files).

### Config & binding redirects

- No `app.config`, `web.config`, `packages.config`, or `<bindingRedirect>` / `dependentAssembly` entries were found in the repository.

---

## Issues and Concerns

### Potential Issues

1. False negatives from incomplete scan
   - **Description**: The automated search covered all files listed by the projects and common token patterns, but if there are generated files, build artifacts, or files outside the scanned set that use `Newtonsoft.Json`, removal could break runtime behavior.
   - **Impact**: Build or runtime failures (FileNotFoundException, TypeLoadException) if code expects `Newtonsoft.Json` at runtime.
   - **Evidence**: No matches in source files; one project-level reference present.
   - **Severity**: Medium

2. Third-party dependencies using Newtonsoft.Json internally
   - **Description**: A package referenced by the project might depend on `Newtonsoft.Json` (transitive dependency). Removing the explicit PackageReference does not remove transitive dependency but could affect version resolution.
   - **Impact**: Version binding differences; likely low but should be validated.
   - **Evidence**: No direct evidence of transitive dependency in analysis; recommend restoring and building to confirm.
   - **Severity**: Low

---

## Risks and Considerations

- Likelihood of runtime failure: Low based on code scan, but non-zero due to potential unscanned/generated files or runtime configuration that expects the assembly.
- Build servers/CI: absolute `HintPath`s for SSIS assemblies may not exist on CI agents; ensure build agents have required SDKs or use conditional references.
- Backward compatibility: If any external integration expects `Newtonsoft.Json`-specific behavior, tests must validate contracts.

---

## Opportunities and Strengths

- `System.Text.Json` is already referenced and used in many source files, indicating the codebase primarily relies on `System.Text.Json` APIs.
- Removing unused `Newtonsoft.Json` reduces package surface and potential vulnerabilities.

---

## Remediation Validation Checklist (steps to safely remove `Newtonsoft.Json`)

1. Pre-check (recommended)
   - Run the repo-wide search locally to ensure no missed usages:
     - PowerShell: `Select-String -Path . -Pattern "Newtonsoft.Json|JsonConvert|JObject|JToken|Newtonsoft.Json.Linq|JsonSerializerSettings|JsonProperty" -SimpleMatch -Recursive`
   - Confirm no `app.config` binding redirects mention `Newtonsoft.Json`.

2. Remove package reference (one of):
   - CLI: `dotnet remove src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj package Newtonsoft.Json`
   - Visual Studio: `Uninstall-Package Newtonsoft.Json -ProjectName QuattoAPIClient.ConnectionManager`
   - Manual: remove `<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />` from `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`.

3. Restore and build
   - `dotnet restore` (or Restore NuGet Packages in Visual Studio)
   - Rebuild solution (Visual Studio: Rebuild Solution or CLI: `msbuild /t:Rebuild`)
   - Verify no compile errors referencing `Newtonsoft.Json` types.

4. Runtime validation
   - Execute all unit/integration tests. If no automated tests exist, perform manual functional flows that exercise: OAuth token acquisition, HTTP calls, serialization/deserialization, schema mapping, and UI interactions.
   - Validate that SSIS components load in the target SSIS/SQL Server environment without assembly load errors.

5. CI validation
   - Ensure build agent has the SSIS/SQL Server SDK assemblies expected by `HintPath`s or revise project references for CI.

6. Post-removal checks
   - Confirm `packages.lock.json` (if present) and package graph no longer require `Newtonsoft.Json` explicitly.
   - Search repository again for `Newtonsoft.Json` tokens to confirm removal.

---

## Data for Planning Stage

### Inventory
- Project with explicit `Newtonsoft.Json` PackageReference:
  - `src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`

### Key Commands
- Search: `Select-String -Path . -Pattern "Newtonsoft.Json|JsonConvert|JObject|JToken" -SimpleMatch -Recursive`
- Remove package (CLI): `dotnet remove src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj package Newtonsoft.Json`
- Restore: `dotnet restore`
- Build: `msbuild /t:Rebuild` or Visual Studio Rebuild

---

## Analysis Artifacts

### Tools Used
- Project analysis tools: `upgrade_get_projects_info`, `upgrade_get_project_dependencies`
- File and code operations: `get_files_in_project`, `get_file`, `code_search`, `file_search`

### Files Analyzed (representative)
- `src/01_Source/Components/CorporateApiSource.cs`
- `src/01_Source/Helpers/HttpHelper.cs`
- `src/01_Source/Helpers/SchemaMapper.cs`
- `src/02_ConnectionManager/ApiConnectionManager.cs`
- `src/02_ConnectionManager/OAuth2TokenManager.cs`
- `src/02_ConnectionManager/TokenRefreshHandler.cs`
- `src/03_UI/Forms/ApiSourceWizard.cs`
- Project files: `QuattoAPIClient.Source.csproj`, `QuattoAPIClient.ConnectionManager.csproj`, `QuattoAPIClient.UI.csproj`

### Analysis Duration
- **Start Time**: 2026-02-05
- **End Time**: 2026-02-05
- **Duration**: ~short interactive session (~minutes)

---

## Conclusion

The repository contains a single explicit project-level reference to `Newtonsoft.Json` in the ConnectionManager project and no detectable code usages of the library in the scanned source. Removing the PackageReference is low-risk but should be validated by performing the checklist steps: search, remove reference, restore, rebuild, run tests, and validate in the SSIS runtime.

Next: This assessment documents the current-state analysis for `Newtonsoft.Json` removal. I can (choose one):
- generate the exact patch to remove the `<PackageReference>` line for you to apply, or
- produce the CLI commands and a short PowerShell script to run the validation checklist.

---

*This assessment was generated by the Analyzer Agent to support Planning and Execution stages. I did not modify source code except for read-only analysis and the assessment document itself.*
