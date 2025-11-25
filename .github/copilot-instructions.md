# GitHub Copilot Instructions for HomeInventory

## Table of Contents

- [Meta-Instructions for AI Assistants](#meta-instructions-for-ai-assistants)
- [Failed Code Edits - Investigation & Prevention](#failed-code-edits---investigation--prevention)
- [Terminal Commands Reference](#terminal-commands-reference)
  - [GitHub Actions Version Management](#github-actions-version-management)
- [Project Overview](#project-overview)
- [Architecture & Structure](#architecture--structure)
- [Technology Stack](#technology-stack)
- [Coding Standards & Conventions](#coding-standards--conventions)
  - [LanguageExt v5 Patterns](#languageext-v5-patterns)
  - [CollectionsMarshal and Async Patterns](#collectionsmarshal-and-async-patterns)
  - [Endpoint Development (Carter)](#endpoint-development-carter)
  - [Domain Development](#domain-development)
  - [Persistence & Data Access](#persistence--data-access)
  - [Application Layer (CQRS)](#application-layer-cqrs)
  - [Application Services Pattern](#application-services-pattern)
- [Testing Guidelines](#testing-guidelines)
  - [General Testing Principles](#general-testing-principles)
  - [Coverage Improvement Strategy](#coverage-improvement-strategy)
  - [Test Structure Pattern](#test-structure-pattern)
  - [Test Design Principles](#test-design-principles)
  - [Critical Test Guidelines](#critical-test-guidelines)
  - [AutoFixture Usage Guidelines](#autofixture-usage-guidelines)
  - [Common Test Anti-Patterns Reference](#common-test-anti-patterns-reference)
  - [Common Assertions](#common-assertions)
  - [Code Quality Warnings to Avoid](#code-quality-warnings-to-avoid)
- [Module Development Workflow](#module-development-workflow)
- [Documentation](#documentation)
- [Development Commands](#development-commands)
- [Additional Guidelines](#additional-guidelines)
- [Code Quality & Analyzers](#code-quality--analyzers)
- [Common Patterns to Follow](#common-patterns-to-follow)
- [Questions to Ask When Developing](#questions-to-ask-when-developing)
- [Code Review Guidelines](#code-review-guidelines)

## Meta-Instructions for AI Assistants

**IMPORTANT**: When a user provides:
- Requests about code patterns or practices
- Advice on how to write better code
- Hints about avoiding specific issues
- Rules that can prevent undesired results
- Corrections to mistakes you've made
- **Terminal commands that fail and their working alternatives**
- **Code edits that fail to compile or have logical errors**
- **Hints about failed approaches during investigation**
- **Corrections about test structure or assertion patterns**

**YOU MUST UPDATE THESE INSTRUCTIONS** to incorporate that guidance so future conversations benefit from the learning. Add the guidance to the appropriate section (Critical Guidelines, Examples, Patterns, Terminal Commands, etc.) with clear examples of what to do and what to avoid.

**Process for Updating Instructions:**
1. **Immediately document** the user's guidance in the relevant section
2. **Add to Failed Code Edits** section if it's a mistake that should be prevented
3. **Update DO/DON'T list** with concrete before/after examples
4. **Reference the investigation process** used to identify and fix issues
5. **Ensure future AI assistants learn from the mistake** by making the guidance explicit and searchable

## Failed Code Edits - Investigation & Prevention

When a code edit fails (compilation error, test failure, logical error):

1. **Document the failure** in these instructions with:
   - What you attempted to do
   - Why it failed
   - The correct solution
   - How to prevent it in the future

2. **Investigate thoroughly**:
   - Read the actual error message carefully
   - Check the framework/library API documentation
   - Look at existing working examples in the codebase
   - Understand WHY it failed, not just HOW to fix it

3. **Prevent recurrence**:
   - Add the failure pattern to the "DON'T" list in [Critical Test Guidelines](#critical-test-guidelines)
   - Add the correct pattern to the "DO" list
   - Include before/after examples
   - Explain the reasoning

> **See also:** [Critical Test Guidelines](#critical-test-guidelines) for the complete DO/DON'T list of testing patterns.

**Example Documentation:**

```markdown
**❌ FAILED: Using SubstituteFor() inside a lambda**

Attempted:
```csharp
New(out var contextVar, () => {
    var substitute = SubstituteFor<IConfiguration>();  // ❌ FAILED
    return new Context(substitute);
});
```

**Error**: `No overload for method 'SubstituteFor' takes 0 arguments`

**Why it failed**: `SubstituteFor()` is a helper method on GivenContext, not available inside lambda scope.

**✅ SOLUTION: Use Substitute.For<T>() directly inside lambdas**

```csharp
New(out var contextVar, () => {
    var substitute = Substitute.For<IConfiguration>();  // ✅ Works
    return new Context(substitute);
});
```

**Prevention**: SubstituteFor helper is only for GivenContext level, use NSubstitute directly inside lambdas.
```

**❌ FAILED: Identity lambda in Invoked (doesn't test behavior)**

Attempted:
```csharp
var then = When
    .Invoked(firstAccessVar, secondAccessVar, static (first, second) => (first, second));
```

**Error**: The Invoked lambda returns a tuple of the inputs without calling any method. This tests nothing and violates the principle that Invoked should invoke the method under test.

**Why it failed**: Identity lambdas `(x) => x` or `(a, b) => (a, b)` don't exercise any behavior—they simply return their inputs unchanged.

**✅ SOLUTION: Invoke actual method under test**

```csharp
// ❌ BAD: Identity lambda (returns inputs unchanged, does not test behavior)
var then = When
    .Invoked(firstAccessVar, secondAccessVar, static (first, second) => (first, second));

// ✅ GOOD: Actually invokes the method under test with the parameters
var then = When
    .Invoked(objVar, paramVar, static (obj, param) => obj.Method(param));

// ✅ GOOD: For parameterless property getter, just call the property
var then = When
    .Invoked(static () => HealthCheckTags.Ready);
```

**Prevention**: Invoked must call actual method under test - never use identity lambdas like `(x) => x` or `(a, b) => (a, b)`.

## Terminal Commands Reference

This section documents working terminal commands and common failures encountered in this project.

### Version Management

**When updating/setting external component versions (NuGet packages, GitHub Actions, etc.):**
- **Always list at least 3 previous versions in your response/explanation**
- This helps maintainers understand the version history and progression
- Format: "Updating from vX.Y.Z (previous: vA.B.C, vD.E.F, vG.H.I) to vN.M.P"
- Example: "Updating actions/checkout from v3 (previous versions: v1, v2) to v4"

**Verifying GitHub Actions Versions:**

> ⚠️ **CRITICAL:** NEVER update a GitHub Action without verifying the SHA first! See the comprehensive [GitHub Actions Version Management](#github-actions-version-management) section for detailed step-by-step instructions.

Before updating GitHub Actions in workflow files, **ALWAYS use `git ls-remote` to verify the latest version and get the correct commit SHA**:

```powershell
# ALWAYS run this BEFORE updating any GitHub Action:
git ls-remote --tags https://github.com/[owner]/[action].git | Select-String "v[major]" | Select-Object -Last 10

# Example for actions/cache:
git ls-remote --tags https://github.com/actions/cache.git | Select-String "v4" | Select-Object -Last 10

# Example output shows SHA and tag:
# 0057852bfaa89a56745cba8c7296529d2fc39830        refs/tags/v4.3.0
# 0400d5f644dc74513175e3cd8d07132dd4860809        refs/tags/v4.2.4
```

**Why this is critical:**
- ❌ **Using incorrect/assumed SHAs causes "action could not be found" errors**
- ❌ **Invalid SHAs waste time debugging workflow failures**
- ✅ **Verification takes 5 seconds and prevents all these issues**
- ✅ **The SHA from `git ls-remote` is guaranteed to be correct**

**Process for Updating GitHub Actions:**

1. **Check current version** in workflow file
2. **Use `git ls-remote`** to list available versions:
   ```powershell
   git ls-remote --tags https://github.com/[owner]/[action].git | Select-String "v[major]"
   ```
3. **Identify latest stable release** from the output
4. **Document version history** - list at least 3 previous versions in your explanation
5. **Update workflow file** with the SHA and version comment:
   ```yaml
   uses: actions/cache@0057852bfaa89a56745cba8c7296529d2fc39830 # v4.3.0
   ```
6. **Verify update** - check that all instances are consistent

**Example Workflow:**

```powershell
# 1. Check the actions/cache repository for latest v4 versions
git ls-remote --tags https://github.com/actions/cache.git | Select-String "v4" | Select-Object -Last 10

# Output shows:
# 6849a6489940f00c2f30c0fb92c6274307ccb58a        refs/tags/v4.1.2
# 1bd1e32a3bdc45362d1e726936510720a7c30a57        refs/tags/v4.2.0
# 0057852bfaa89a56745cba8c7296529d2fc39830        refs/tags/v4.3.0  ← Latest

# 2. Update workflow with the latest SHA and version comment
# uses: actions/cache@0057852bfaa89a56745cba8c7296529d2fc39830 # v4.3.0

# 3. Document in your response:
# "Updating actions/cache to v4.3.0 (previous versions: v4.0.0, v4.1.0, v4.2.0)"
```

**Common GitHub Actions to Update:**

| Action                  | Repository URL                                 | Current Major |
|-------------------------|------------------------------------------------|---------------|
| actions/checkout        | https://github.com/actions/checkout.git        | v4, v6        |
| actions/setup-dotnet    | https://github.com/actions/setup-dotnet.git    | v4, v5        |
| actions/cache           | https://github.com/actions/cache.git           | v4            |
| actions/upload-artifact | https://github.com/actions/upload-artifact.git | v4, v5        |
| ossf/scorecard-action   | https://github.com/ossf/scorecard-action.git   | v2            |
| github/codeql-action    | https://github.com/github/codeql-action.git    | v3            |

**Why Use SHA Pinning:**

- ✅ **OpenSSF Scorecard requirement** - Pinned dependencies for security
- ✅ **Immutable reference** - SHA cannot be changed, unlike tags
- ✅ **Supply chain security** - Prevents tag manipulation attacks
- ⚠️ **Maintenance overhead** - Must update when GitHub deprecates old SHAs

**Handling GitHub Action Deprecations:**

If you encounter a deprecation error like:
```
Error: This request has been automatically failed because it uses a deprecated version of `actions/cache: [SHA]`
```

**Solution:**
1. Use `git ls-remote` to find the **latest** version SHA
2. Update all instances to the new SHA
3. Document the change with previous versions
4. Verify the new SHA is from an official release tag, not an intermediate commit

**Common GitHub Actions Mistakes & How to Avoid Them:**

Based on actual failures encountered (November 2024):

**❌ MISTAKE 1: Using assumed/guessed SHAs without verification**
```yaml
# ❌ WRONG - This SHA was assumed, not verified
uses: actions/checkout@db14d8b7fea37acffdd656bd35b81b8f8b3bb8ad # v6.0.0
# Error: "An action could not be found at the URI"
```

**✅ CORRECT - Always verify first:**
```powershell
# 1. Verify the correct SHA
git ls-remote --tags https://github.com/actions/checkout.git | Select-String "v6" | Select-Object -Last 5
# Output: 1af3b93b6815bc44a9784bd300feb67ff0d1eeb3        refs/tags/v6.0.0

# 2. Use the verified SHA
uses: actions/checkout@1af3b93b6815bc44a9784bd300feb67ff0d1eeb3 # v6.0.0
```

**❌ MISTAKE 2: Updating one action at a time reactively**

Instead of verifying and updating ALL actions systematically, we updated one at a time as errors appeared, resulting in:
- 11 different actions with invalid SHAs
- 27 total instances that needed correction
- Multiple workflow failures

**✅ CORRECT - Proactive systematic verification:**
```powershell
# Verify ALL actions in your workflow BEFORE updating:
git ls-remote --tags https://github.com/actions/checkout.git | Select-String "v6" | Select-Object -Last 5
git ls-remote --tags https://github.com/actions/setup-dotnet.git | Select-String "v5" | Select-Object -Last 5
git ls-remote --tags https://github.com/actions/cache.git | Select-String "v4" | Select-Object -Last 10
git ls-remote --tags https://github.com/actions/upload-artifact.git | Select-String "v5" | Select-Object -Last 5
# ... verify ALL actions used in workflow
```

**❌ MISTAKE 3: Using outdated/deprecated SHAs**

GitHub deprecates old commit SHAs, even for stable releases:
```
Error: This request has been automatically failed because it uses a deprecated version of `actions/cache: 6849a6489940f00c2f30c0fb92c6274307ccb58a`
```

**✅ CORRECT - Use the LATEST stable release SHA:**
```powershell
# Always get the LATEST version in the major version series
git ls-remote --tags https://github.com/actions/cache.git | Select-String "v4" | Select-Object -Last 10
# Use the most recent: v4.3.0, not v4.1.2 or v4.2.0
```

**Actions verified and corrected in this project (as of November 2024):**

| Action | Correct SHA | Version | Previous Issues |
|--------|-------------|---------|-----------------|
| actions/checkout | `1af3b93b6815bc44a9784bd300feb67ff0d1eeb3` | v6.0.0 | Used invalid SHA |
| actions/setup-dotnet | `2016bd2012dba4e32de620c46fe006a3ac9f0602` | v5.0.1 | Used invalid SHA |
| actions/cache | `0057852bfaa89a56745cba8c7296529d2fc39830` | v4.3.0 | Multiple deprecated SHAs |
| actions/upload-artifact | `330a01c490aca151604b8cf639adc76d48f6c5d4` | v5.0.0 | Used invalid SHA |
| ossf/scorecard-action | `99c09fe975337306107572b4fdf4db224cf8e2f2` | v2.4.3 | Was using v2.4.0 |
| github/codeql-action/upload-sarif | `5ad83d3202da6e473f763d732b591299ae4e380c` | v3 | Used invalid SHA |
| dorny/test-reporter | `894765a932a426ee30919ffd3b5fd3b53c0e26b8` | v2.2.0 | Used invalid SHA |
| EnricoMi/publish-unit-test-result-action | `6e8f8c55b476f977d1c58cfbd7e337cbf86d917f` | v2 | Used invalid SHA |
| irongut/CodeCoverageSummary | `51cc3a756ddcd398d447c044c02cb6aa83fdae95` | v1.3.0 | Was correct |
| marocchino/sticky-pull-request-comment | `773744901bac0e8cbb5a0dc842800d45e9b2b405` | v2 | Used invalid SHA |
| danielpalme/ReportGenerator-GitHub-Action | `dcdfb6e704e87df6b2ed0cf123a6c9f69e364869` | v5.5.0 | Used invalid SHA |

**Key Lesson:** If you're updating GitHub Actions, verify EVERY action's SHA with `git ls-remote` before making changes. This takes a few minutes but prevents hours of debugging workflow failures.

---

## GitHub Actions Version Management

> **CRITICAL:** Always verify the SHA for every GitHub Action you use. Using an incorrect SHA can cause workflow failures, security issues, or silent breakage. Never trust the SHA in documentation or release notes—always check it yourself.

### Step-by-step: How to verify a GitHub Action SHA

1. **Find the action and version you want to use.**
   - Example: `actions/checkout@v4`

2. **Get the repository URL.**
   - Example: `https://github.com/actions/checkout`

3. **Run `git ls-remote` to list all refs and SHAs:**
   ```sh
   git ls-remote https://github.com/actions/checkout.git
   ```

4. **Find the SHA for the tag or release you want.**
   - For example, to use v4, look for the line ending with `refs/tags/v4`.

5. **Update your workflow to use the full SHA:**
   ```yaml
   uses: actions/checkout@<SHA>
   ```
   Example:
   ```yaml
   uses: actions/checkout@a9b1234567890abcdef1234567890abcdef123456
   ```

6. **Double-check the SHA matches the intended tag.**
   - If the tag is moved or deleted, your workflow may break.

### Example: Real workflow failure due to incorrect SHA

In November 2024, several workflows failed with errors like:

```
Error: The workflow is not valid. .github/workflows/ci.yml (Line 23): The workflow uses an action 'actions/checkout@v4' with an invalid SHA. Please verify the SHA and try again.
```

**Root Cause:** The SHA for `actions/checkout@v4` was copied from a blog post and did not match the actual tag in the repository.

**Resolution:** The workflow failed to run until the correct SHA was verified with `git ls-remote` and updated.

**Lesson:** Always verify SHAs directly from the source repository, never copy from documentation or blog posts.

---

**Security Scanning Best Practices:**

The security-scan job implements two levels of security validation:

1. **Vulnerable Packages Check** (Blocking):
   - Fails workflow if any NuGet packages have known CVEs
   - Uses `dotnet list package --vulnerable`
   - Blocks merge until vulnerabilities are resolved

2. **OpenSSF Scorecard Check** (Blocking on Critical Issues):
   - Runs Scorecard to analyze supply chain security
   - Publishes results to GitHub Security tab for visibility
   - **Fails workflow if critical security issues detected** (error/warning level with security tags)
   - Does NOT fail on low scores alone (scores are informational)
   - Parses SARIF results to detect actionable security problems

**Why This Approach:**
- ✅ Blocks on **actionable security issues** (vulnerable packages, critical Scorecard findings)
- ✅ Provides **visibility** on security posture (Scorecard scores, SARIF uploads)
- ✅ Allows **incremental improvements** (low scores don't block, but critical issues do)
- ✅ Follows **OpenSSF best practices** (Scorecard for visibility + blocking on critical findings)

### PowerShell Commands (Windows)

**Shell**: `powershell.exe` (Windows PowerShell v7.5.4, as of time of writing. Use ` $PSVersionTable.PSVersion.ToString()` to check version, if needed)

#### Working Commands

**Build:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet build HomeInventory.Tests\HomeInventory.Tests.csproj --no-restore
```

**Test:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet test HomeInventory.Tests\HomeInventory.Tests.csproj --no-build
```

**Test with Filter:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet test HomeInventory.Tests\HomeInventory.Tests.csproj --filter "FullyQualifiedName~TestClassName" --no-build
```

**Format Verification:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet format --verify-no-changes --severity error --verbosity diag
```

**Coverage (Local):**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage"
```

**Build with Warnings as Errors:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet build HomeInventory.Tests\HomeInventory.Tests.csproj --no-restore /p:TreatWarningsAsErrors=true
```

**Filter Output with Select-String:**
```powershell
dotnet build 2>&1 | Select-String -Pattern "warning|error" | Select-Object -First 20
```

#### Command Failures and Solutions

**❌ FAILED: Commands hanging indefinitely**

Some commands appear to hang with no output, especially when run in background mode or with complex pipelines:

```powershell
# ❌ These may hang or produce no output:
dotnet build HomeInventory.Tests\HomeInventory.Tests.csproj
dotnet test HomeInventory.Tests\HomeInventory.Tests.csproj --no-build
dotnet format --verify-no-changes --severity error --verbosity diag
```

**✅ SOLUTION: Use explicit working directory and output redirection**

```powershell
# ✅ Always use explicit cd and simpler commands:
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet build HomeInventory.Tests\HomeInventory.Tests.csproj --no-restore

# ✅ For checking output, use Select-String:
dotnet build 2>&1 | Select-String -Pattern "succeeded|failed"
```

**Why it works**: Explicit `cd` ensures correct context, and separating commands avoids complex pipeline issues.

---

**❌ FAILED: grep doesn't exist in PowerShell**

```powershell
# ❌ This fails:
dotnet build | grep "error"
```

**Error**: `grep : The term 'grep' is not recognized`

**✅ SOLUTION: Use Select-String instead**

```powershell
# ✅ PowerShell equivalent:
dotnet build 2>&1 | Select-String -Pattern "error"
```

**Why**: PowerShell uses `Select-String`, not `grep`. Always use PowerShell cmdlets.

---

**❌ FAILED: Piping with complex filter expressions**

```powershell
# ❌ This may fail with parsing errors:
dotnet test --filter "FullyQualifiedName~Test1|FullyQualifiedName~Test2"
```

**✅ SOLUTION: Use proper escaping or quotes**

```powershell
# ✅ Proper escaping:
dotnet test --filter "FullyQualifiedName~Test1|FullyQualifiedName~Test2"
# OR separate filters:
dotnet test --filter "Category=Unit"
```

---

### When Commands Fail

**If a terminal command fails:**

1. **Record the failure** in this section
2. **Document the error message**
3. **Document the working solution**
4. **Explain why it works**

**Template for new failures:**

```markdown
**❌ FAILED: [Brief description]**

[Failed command]

**Error**: [Error message]

**✅ SOLUTION: [Working alternative]**

[Working command]

**Why**: [Explanation of why the solution works]
```

### Command Best Practices

1. **Always use explicit `cd`** before running commands
2. **Use `--no-restore`** and `--no-build`** when appropriate to speed up builds
3. **Use `2>&1`** to capture both stdout and stderr in PowerShell
4. **Use `Select-String`** instead of `grep`
5. **Avoid complex pipelines** - break into multiple commands if needed
6. **Use `--filter`** for running specific tests
7. **Check command output** with `Select-String` patterns

### Common Patterns

**Check if build succeeded:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet build 2>&1 | Select-String -Pattern "Build succeeded|Build failed"
```

**Run specific test category:**
```powershell
cd C:\GitHub\HomeInventory\src\HomeInventory
dotnet test --filter "Category=Unit" --no-build
```

**Check for specific errors:**
```powershell
dotnet build 2>&1 | Select-String -Pattern "IDE0053|CS\d+"
```

## Project Overview

HomeInventory is a home inventory management system built with .NET 10.0, following Clean Architecture principles with a modular design pattern. The application uses ASP.NET Core for the API layer and implements Domain-Driven Design (DDD) patterns.

## Architecture & Structure

### Project Organization

The solution follows a **vertical slice/modular architecture** with clear separation of concerns:

- **HomeInventory.Api** - Entry point and API configuration
- **HomeInventory.Web[.Module]** - Carter-based HTTP endpoints (minimal APIs)
- **HomeInventory.Application[.Module]** - Application logic and use cases
- **HomeInventory.Application[.Module].Interfaces** - Public contracts for commands/queries
- **HomeInventory.Contracts[.Module]** - DTOs and request/response models
- **HomeInventory.Contracts[.Module].Validators** - FluentValidation validators for contracts
- **HomeInventory.Domain[.Module]** - Domain entities, aggregates, value objects, and domain events
- **HomeInventory.Infrastructure[.Module]** - Data access and external services
- **HomeInventory.Infrastructure.Framework** - Shared infrastructure concerns
- **HomeInventory.Core** - Shared primitives and utilities
- **HomeInventory.Modules** - Module registration and orchestration
- **HomeInventory.Tests[.Type]** - Testing projects (Unit, Integration, Acceptance)

### Key Architectural Patterns

1. **Modular Monolith**: Each feature module (e.g., UserManagement) has its own Application, Domain, Contracts, Infrastructure, and Web layers
2. **CQRS**: Command Query Responsibility Segregation for operations
3. **Domain-Driven Design**: Aggregates, Entities, Value Objects, Domain Events
4. **Vertical Slice Architecture**: Features are organized by business capability
5. **Clean Architecture**: Dependency rule - dependencies point inward toward domain

## Technology Stack

### Core Technologies
- **.NET 10.0** (see `global.json` for specific SDK version)
- **ASP.NET** for Web API
- **Carter** for minimal API endpoints
- **Entity Framework Core** for data access
- **Serilog** for structured logging

### Key Libraries
- **LanguageExt** - Functional programming primitives
- **FluentValidation** - Request validation
- **Riok.Mapperly** - Compile-time object mapping
- **Ardalis.Specification** - Repository pattern with specifications
- **BCrypt.Net-Next** - Password hashing
- **Swashbuckle** - OpenAPI/Swagger documentation
- **Scrutor** - Assembly scanning and decoration
- **Ulid** - Unique identifiers
- **System.IdentityModel.Tokens.Jwt** - JWT authentication
- **Microsoft.Extensions.Options.DataAnnotations** - Options validation
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **Microsoft.Extensions.Diagnostics.HealthChecks** - Health check infrastructure

### Testing
- **xUnit** - Testing framework
- **Reqnroll** - BDD/Acceptance testing (SpecFlow successor)
- **Expressium.LivingDoc.ReqnrollPlugin** - Living documentation
- **NSubstitute** - Mocking framework
- **AutoFixture** - Test data generation
- **AwesomeAssertions** - Fluent assertions
- **LanguageExt.UnitTesting** - Testing helpers for LanguageExt types
- **TngTech.ArchUnitNET** & **NetArchTest.Rules** - Architecture testing

## Coding Standards & Conventions

### Naming Conventions

1. **Namespaces**: Follow folder structure - `HomeInventory.[Layer][.Module][.SubFolder]`
2. **Files**: One type per file, file name matches type name
3. **Projects**:
   - Feature modules: `HomeInventory.[Layer].[ModuleName]`
   - Framework/shared: `HomeInventory.[Layer].Framework`
4. **Private/Internal Fields**: Use underscore prefix with camelCase - `_fieldName`
5. **Interfaces**: Start with 'I' prefix - `IUserRepository`
6. **Type Parameters**: Use 'T' prefix - `TEntity`, `TIdentity`

### Code Style

1. **Use file-scoped namespaces**
2. **Use primary constructors** where appropriate
3. **Prefer `using` declarations** over `using` statements
4. **Use implicit usings** - defined in `ImplicitUsings.cs` files
5. **Functional programming**: Leverage LanguageExt for functional patterns (Option, Either, Try, etc.)
6. **Immutability**: Prefer immutable data structures and records
7. **Prefer `extension` keyword** for static helper methods
8. **Expression-bodied members** for simple getters and methods
9. **Pattern matching** over traditional type checks and casts
10. **Static local functions** and **static anonymous functions** where possible

### LanguageExt v5 Patterns

**The project uses LanguageExt.Core v5.** Follow these patterns for Option usage:

**Creating Option Values:**

```csharp
// ✅ CORRECT - Use Prelude.Some() for non-null values
return value is not null
    ? Prelude.Some(value)
    : Option<T>.None;

// ✅ CORRECT - Use pattern matching with Prelude.Some()
return await query.FirstOrDefaultAsync(ct) is { } entity
    ? Prelude.Some(entity)
    : Option<T>.None;

// ✅ CORRECT - Use .NoneIfNull() extension for nullable references
return nullableValue.NoneIfNull();

// ✅ CORRECT - Use .ToOption() extension (LanguageExt built-in)
return collection.Where(predicate).ToOption();

// ❌ WRONG - Don't use Prelude.Optional() for non-null values
return Prelude.Optional(entity);  // ❌ This is for nullable types only

// ❌ WRONG - Don't use new Option<T>()
return new Option<T>(value);  // ❌ Not the v5 API
```

**When to use what:**
- **`Prelude.Some(value)`**: When you have a guaranteed non-null value
- **`Option<T>.None`**: To represent absence of value
- **`.NoneIfNull()`**: For nullable reference types (defined in `HomeInventory.Core.OptionExtensions`)
- **`.ToOption()`**: For collections or LanguageExt built-in conversions
- **`Prelude.Optional(value)`**: ONLY for nullable value types (e.g., `int?`, `DateTime?`)

### CollectionsMarshal and Async Patterns

**Using `CollectionsMarshal.GetValueRefOrAddDefault` correctly:**

```csharp
// ✅ CORRECT - Synchronous: Use ref to avoid second dictionary lookup
public TResult GetOrAdd<TResult>(TKey key, Func<TKey, TResult> createValueFunc)
{
    ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
    if (!exists)
    {
        val = createValueFunc(key);  // Set via ref - no second lookup!
    }
    return (TResult)val!;
}

// ✅ CORRECT - Async: Cannot use ref across await, use TryGetValue pattern
public async ValueTask<TResult> GetOrAddAsync<TResult>(TKey key, Func<TKey, Task<TResult>> createValueFunc)
{
    if (dictionary.TryGetValue(key, out var existingValue))
    {
        return (TResult)existingValue!;
    }

    var newValue = await createValueFunc(key);
    dictionary[key] = newValue;
    return newValue;
}

// ❌ WRONG - Using CollectionsMarshal with async
public async ValueTask<TResult> GetOrAddAsync<TResult>(TKey key, Func<TKey, Task<TResult>> createValueFunc)
{
    ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
    // ❌ Cannot use 'val' after await - ref becomes invalid!
    var newValue = await createValueFunc(key);
    val = newValue;  // ❌ COMPILER ERROR: Cannot use ref variable across await boundary
    return (TResult)val!;
}

// ❌ WRONG - Not using the ref properly (defeats the optimization)
public TResult GetOrAdd<TResult>(TKey key, Func<TKey, TResult> createValueFunc)
{
    ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
    if (!exists)
    {
        var newValue = createValueFunc(key);
        dictionary[key] = newValue;  // ❌ Second dictionary lookup - defeats the purpose of ref!
    }
    return (TResult)val!;
}
```

**Key Rules:**
- **Refs cannot cross await boundaries** - C# compiler enforces this
- **Use `CollectionsMarshal.GetValueRefOrAddDefault` only in synchronous code** for performance
- **Assign through the ref variable (`val = ...`)** to avoid second dictionary lookup
- **For async operations**, use standard `TryGetValue` pattern - the ref optimization isn't applicable
- **`Dictionary<TKey, TValue>` is NOT thread-safe** - use `ConcurrentDictionary` for multithreaded scenarios

### Architecture Rules

1. **Modules are independent**: Each module should be self-contained
2. **Module registration**: Use `[Module]Module.cs` classes implementing module interfaces
3. **Dependency injection**: Register services via module configuration
4. **No circular dependencies** between modules
5. **Domain layer has no external dependencies** (except LanguageExt)
6. **Application layer depends only on Domain**
7. **Infrastructure implements interfaces from Application**

### Endpoint Development (Carter)

When creating new endpoints, inherit from `ApiCarterModule` (not the base `CarterModule`):

```csharp
namespace HomeInventory.Web.[ModuleName];

public sealed class [Feature]CarterModule(
    IScopeAccessor scopeAccessor,
    IProblemDetailsFactory problemDetailsFactory,
    ContractsMapper mapper)
    : ApiCarterModule
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IProblemDetailsFactory _problemDetailsFactory = problemDetailsFactory;
    private readonly ContractsMapper _mapper = mapper;

    protected override string PathPrefix => "/api/[resource]";

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("[OperationName]")
            .WithValidationOf<[RequestType]>()
            .Produces<[ResponseType]>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<Results<Ok<[Response]>, ProblemHttpResult>> HandleAsync(
        [FromBody] [RequestType] request,
        [FromServices] I[Service] service,
        [FromServices] I[Repository] repository,
        [FromServices] IUnitOfWork unitOfWork,
        HttpContext context,
        CancellationToken cancellationToken = default)
    {
        // Set scoped context for use in service methods
        using var scopes = new CompositeDisposable(
            _scopeAccessor.GetScope<I[Repository]>().Set(repository),
            _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

        // Map request to command and call service
        var command = _mapper.ToCommand(request);
        var result = await service.[MethodName]Async(command, cancellationToken);

        // Handle result with pattern matching
        return result.Match(
            error => _problemDetailsFactory.CreateProblemResult(error, context.TraceIdentifier),
            () => TypedResults.Ok(new [Response]()));
    }
}
```

**Important Endpoint Conventions:**
- Use `ApiCarterModule` base class for automatic API versioning
- Override `PathPrefix` property to specify the base path (required)
- Inject `IScopeAccessor` for scoped dependency management
- Inject `IProblemDetailsFactory` for standardized error responses
- Inject `ContractsMapper` for mapping between DTOs and commands/queries
- Inject application services (e.g., `IUserService`) directly via `[FromServices]`
- Use `WithValidationOf<T>()` (not `WithValidation<T>()`) for validation
- Return typed results: `Results<Ok<TResponse>, ProblemHttpResult>`
- Always include `CancellationToken` parameter with `default` value
- Set scoped context before calling service methods
- Use mapper to convert requests to commands/queries

### Domain Development

1. **Entities**: Inherit from base entity classes in `HomeInventory.Domain.Primitives`
2. **Value Objects**: Use records or LanguageExt value types
3. **Domain Events**: Implement domain event notifications via `IDomainEventNotification`
4. **Aggregates**: Define aggregate roots in `Aggregates/` folder
5. **Domain Errors**: Define errors in `Errors/` folder

### Persistence & Data Access

1. **Entity Framework Core**: Use EF Core 10.0 for data access
2. **Repository Pattern**: Use Ardalis.Specification for repository pattern
   - Define specifications in `HomeInventory.Infrastructure.[Module]`
   - Inherit from `IRepository<TEntity>` or `IReadOnlyRepository<TEntity>`
3. **Unit of Work**:
   - `IUnitOfWork` is implemented by `DatabaseContext`
   - Injected via `IScopeAccessor` at endpoint level
4. **Database Context**:
   - One `DatabaseContext` per application
   - Module-specific configurations via `IDatabaseConfigurationApplier`
5. **Interceptors**: Domain events published via `PublishDomainEventsInterceptor`
6. **Auditing**:
   - Use `ICreationAuditableEntity` for creation tracking
   - Use `IModificationAuditableEntity` for modification tracking
   - Timestamp from injected `TimeProvider`

### Application Layer (CQRS)

The application layer implements CQRS with the following messaging patterns:

**Commands** (write operations):
```csharp
public record RegisterCommand(
    string Email,
    string Name,
    string Password) : ICommand; // Returns Option<Error>
```

**Queries** (read operations):
```csharp
public record GetUserByEmailQuery(string Email) : IQuery<User>; // Returns Option<User>
```

**Key CQRS Conventions:**
- Commands return `Option<Error>` (success = None, failure = Some(error))
- Queries return `IQueryResult<T>` which wraps validation results
- Commands and queries are immutable records
- Define in `HomeInventory.Application.[Module].Interfaces`
- Service implementations live in `HomeInventory.Application.[Module]`

### Application Services Pattern

The solution uses a **direct service pattern** (not MediatR) where application services implement command and query methods:

**Service Interface:**
```csharp
namespace HomeInventory.Application.[Module].Interfaces;

public interface I[Module]Service
{
    // Commands return Option<Error>
    Task<Option<Error>> [CommandName]Async([Command] command, CancellationToken cancellationToken = default);

    // Queries return IQueryResult<T>
    Task<IQueryResult<[Result]>> [QueryName]Async([Query] query, CancellationToken cancellationToken = default);
}
```

**Service Implementation:**
```csharp
namespace HomeInventory.Application.[Module];

internal sealed class [Module]Service(
    IScopeAccessor scopeAccessor,
    // ... other dependencies
    ) : I[Module]Service
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task<Option<Error>> [CommandName]Async([Command] command, CancellationToken cancellationToken = default)
    {
        // Retrieve scoped dependencies set in endpoint
        var repository = _scopeAccessor.GetRequiredContext<I[Repository]>();
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();

        // Implementation - business logic
        // ...

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Option<Error>.None; // Success
    }

    public async Task<IQueryResult<[Result]>> [QueryName]Async([Query] query, CancellationToken cancellationToken = default)
    {
        // Retrieve scoped dependencies set in endpoint
        var repository = _scopeAccessor.GetRequiredContext<I[Repository]>();

        // Implementation
        var result = await repository.GetByIdAsync(query.Id, cancellationToken);
        var validationResult = result
            .Map(entity => /* map to result */)
            .ErrorIfNone(() => new NotFoundError("Not found"));

        return QueryResult.From(validationResult);
    }
}
```

**Service Pattern Conventions:**
- Services are `internal sealed` classes implementing public interfaces
- Service interfaces defined in `HomeInventory.Application.[Module].Interfaces`
- Service implementations in `HomeInventory.Application.[Module]`
- Use `IScopeAccessor` to retrieve scoped dependencies (set in endpoints)
- Commands return `Option<Error>` - None for success, Some(error) for failure
- Queries return `IQueryResult<T>` wrapping `Validation<Error, T>`
- Service methods named as `[OperationName]Async` with async suffix
- Always include `CancellationToken` parameter with `default` value
- Use LanguageExt functional patterns (Option, Validation, Map, Bind, etc.)

### Validation

1. **Contract validation**: Use FluentValidation in `HomeInventory.Contracts.[Module].Validators`
2. **Validation registration**: Automatically registered via Scrutor scanning
3. **Endpoint validation**: Apply via `.WithValidationOf<T>()` extension method

### Response Mapping (Mapperly)

Use Mapperly for compile-time DTO mapping:

```csharp
namespace HomeInventory.Contracts.[Module];

[Mapper]
public static partial class [Module]Mapper
{
    // Domain to DTO
    public static partial [Response] ToResponse(this [Entity] entity);

    // DTO to Domain (if needed)
    public static partial [Entity] ToDomain(this [Request] request);

    // Collection mapping
    public static partial IEnumerable<[Response]> ToResponses(
        this IEnumerable<[Entity]> entities);

    // With custom mapping logic
    [MapProperty(nameof([Entity].Property), nameof([Response].MappedProperty))]
    public static partial [Response] ToResponseWithCustomMapping(this [Entity] entity);
}
```

**Mapper Conventions:**
- Place mappers in `HomeInventory.Contracts.[Module]` namespace
- Use `[Mapper]` attribute on static partial classes
- Methods must be `public static partial`
- Use extension methods for better fluent API
- Name mapper classes as `[Module]Mapper` (e.g., `UserManagementMapper`)
- Mapperly generates implementation at compile-time

### API Versioning

API versioning is built into the `ApiCarterModule` base class:

```csharp
public sealed class MyCarterModule : ApiCarterModule
{
    protected override string PathPrefix => "/api/resource";

    // Constructor - call MapToApiVersion to use a different version than v1 (default)
    public MyCarterModule()
    {
        MapToApiVersion(new ApiVersion(2)); // Use v2 instead of default v1
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        // Define your routes here
    }
}
```

**Versioning Conventions:**
- Default API version is 1.0
- Version is specified in query string: `/api/resource?api-version=1`
- All endpoints automatically versioned through `ApiCarterModule`
- Use `protected override string PathPrefix` to specify the base path (required)
- Call `MapToApiVersion(new ApiVersion(x))` in constructor to use a different version
- If no version is specified, defaults to v1
- Version set configured in `WebSwaggerModule`

### Logging

1. **Use Serilog** with structured logging
2. **Log context enrichment**: Thread, demystified stack traces
3. **Bootstrap logger**: Created in `LoggingModule.CreateBootstrapLogger()`
4. **Configuration**: See `appsettings.json` for Serilog settings

### Security & Authentication

1. **Password Hashing**: Use BCrypt.Net-Next for secure password storage
2. **JWT Tokens**: Use `System.IdentityModel.Tokens.Jwt` for authentication
3. **Authorization**:
   - Use `.AllowAnonymous()` for public endpoints
   - Use `.RequireAuthorization()` for protected endpoints
   - Custom dynamic authorization in `DynamicWebAuthorizationModule`
4. **Token Generation**: Implement `IAuthenticationTokenGenerator` interface
5. **Identity**: Use ULID-based identity generation via `IIdSupplier<Ulid>`

### Error Handling

1. **Use LanguageExt types**: `Either<Error, Result>`, `Option<T>`, `Try<T>`
2. **Domain errors**: Define typed errors in domain layer
3. **Exception handling**: Use `Execute.AndCatchAsync` for top-level handling
4. **ProblemDetails**: Return RFC 7807 problem details for API errors

### Scope Accessor Pattern

The project uses a custom **Scope Accessor** pattern for managing scoped dependencies across layers:

**Setting Scope (in Endpoints):**
```csharp
// In Carter module endpoint handler
private async Task<Results<Ok<Response>, ProblemHttpResult>> HandleAsync(
    [FromBody] Request request,
    [FromServices] IUserService userService,
    [FromServices] IUserRepository repository,
    [FromServices] IUnitOfWork unitOfWork,
    HttpContext context,
    CancellationToken cancellationToken = default)
{
    // Set scoped context before calling service
    using var scopes = new CompositeDisposable(
        _scopeAccessor.GetScope<IUserRepository>().Set(repository),
        _scopeAccessor.GetScope<IUnitOfWork>().Set(unitOfWork));

    var command = _mapper.ToCommand(request);
    var result = await userService.RegisterAsync(command, cancellationToken);

    // Handle result...
}
```

**Retrieving Scope (in Services):**
```csharp
// In application service implementation
internal sealed class UserService(
    IScopeAccessor scopeAccessor,
    // ... other dependencies
    ) : IUserService
{
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;

    public async Task<Option<Error>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        // Retrieve scoped dependencies that were set in endpoint
        var repository = _scopeAccessor.GetRequiredContext<IUserRepository>();
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();

        // Use dependencies for business logic...
    }
}
```

**Why use Scope Accessor?**
- Allows passing request-scoped dependencies from Web layer to Application layer
- Avoids parameter pollution in service methods
- Maintains Clean Architecture dependency rules
- Repositories and UnitOfWork are injected at the endpoint level and accessed in services
- **Important:** Always dispose scopes using `using` or `CompositeDisposable`

## Module Development Workflow

When creating a new feature module (e.g., "Inventory"):

1. **Create project structure**:
   ```
   HomeInventory.Domain.Inventory/
   HomeInventory.Application.Inventory/
   HomeInventory.Application.Inventory.Interfaces/
   HomeInventory.Contracts.Inventory/
   HomeInventory.Contracts.Inventory.Validators/
   HomeInventory.Infrastructure.Inventory/
   HomeInventory.Web.Inventory/
   ```

2. **Create module registration class** in each layer (e.g., `InventoryModule.cs`)

3. **Register module** in `ApplicationModules.cs`:
   ```csharp
   public static class ApplicationModules
   {
       public static IReadOnlyCollection<Type> GetModuleTypes() =>
       [
           typeof(CoreModule),
           typeof(LoggingModule),
           typeof(DatabaseModule),
           typeof(UserManagementModule),
           typeof(InventoryModule), // Add new module here
           // Modules are loaded in dependency order
       ];
   }
   ```

4. **Define domain models** (Aggregates, Entities, Value Objects, Events)

5. **Define contracts** (Request/Response DTOs)

6. **Create validators** for contracts

7. **Implement application services** (service interfaces and implementations)

8. **Implement infrastructure** (repositories, external services)

9. **Create web endpoints** using Carter

10. **Write tests** (unit, integration, acceptance)

## Testing Guidelines

### General Testing Principles

1. **Unit Tests**: Test domain logic and business rules in isolation
2. **Integration Tests**: Test with real dependencies (database, external services, middleware pipelines)
3. **Acceptance Tests**: BDD scenarios using Reqnroll
4. **Test project structure**: Mirror source project structure
5. **Test naming**: `[MethodName]_[Scenario]_[ExpectedResult]`
6. **Coverage targets**:
   - Domain/Application layers: Aim for 80%+ (business logic)
   - Infrastructure layer: 60-70% acceptable (database access, specifications)
   - Web layer: 50-60% acceptable (mostly framework wiring)
   - Don't chase 100% - focus on business-critical paths

### When to Use Unit vs Integration Tests

**Unit Tests (BaseModuleTest):**
- ✅ Service registration (`AddServicesAsync`)
- ✅ Domain logic and business rules
- ✅ Pure functions and transformations
- ✅ Error handling and validation
- ✅ Data structure operations

**Integration Tests (WebApplicationFactory):**
- ✅ Middleware pipeline configuration (`BuildAppAsync`)
- ✅ Endpoint routing and handlers
- ✅ Authentication/Authorization flow
- ✅ Database queries with real DB
- ✅ End-to-end scenarios

**Don't Unit Test:**
- ❌ Framework generated code (marked with `CompilerGenerated` or `GeneratedCode` attributes)
- ❌ Simple property getters/setters
- ❌ Thin wrappers over framework methods
- ❌ Code that only calls framework APIs

**Code Coverage Exclusions:**

The following are automatically excluded from code coverage (see `coverlet.runsettings` and CI workflow):
- Classes in `Microsoft.AspNetCore.OpenApi.Generated.*` namespace (OpenAPI generated code)
- Classes in `System.Runtime.CompilerServices.*` namespace (compiler-generated helpers)
- Files matching `**/*.g.cs` pattern (generated files)
- Classes marked with `[CompilerGenerated]`, `[GeneratedCode]`, or `[ExcludeFromCodeCoverage]` attributes
- Auto-implemented properties
- Test assemblies themselves

To run tests with coverage locally:
```cmd
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage"
```

### Coverage Improvement Strategy

**Priority Modules for Coverage Improvement:**

1. **HomeInventory.Application** (currently 0% coverage)
   - **Focus Areas:**
     - `BaseHealthCheck` and derived health checks
     - `HealthCheckStatus` and `HealthCheckTags` utilities
   - **Recommended Tests:**
     - Unit tests for health check logic (status determination, failure conditions)
     - Tests for health check tag constants and status properties
     - Mock dependencies to test in isolation
   - **Target:** 80%+ (business logic)

2. **HomeInventory.Modules** (currently 44.4% coverage)
   - **Focus Areas:**
     - `ModulesCollection` - add, enumerate, duplicate handling
     - `ModuleMetadata` - dependency resolution, module wrapping
     - `ModuleServicesContext` - property access, construction
     - `IModule` implementations - lifecycle methods
   - **Recommended Tests:**
     - Unit tests for collection operations
     - Module dependency resolution scenarios
     - Module registration and initialization flows
   - **Target:** 80%+ (core module infrastructure)

**Testing Approach:**
- Use `BaseTest<TGivenContext>` with Given-When-Then pattern
- Focus on public API and business logic, not framework plumbing
- Test edge cases: empty collections, circular dependencies, missing dependencies
- Verify error handling and validation logic
- Use AutoFixture for test data generation
- Mock external dependencies with NSubstitute

### Test Structure Pattern

All tests follow the **Given-When-Then** pattern using `BaseTest<TGivenContext>`:

```csharp
[UnitTest]
public class MyFeatureTests() : BaseTest<MyFeatureTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void MethodName_Scenario_ExpectedResult()
    {
        Given
            .New<SomeType>(out var variable1)
            .New<string>(out var inputVar);  // ✅ AutoFixture generates value

        var then = When
            .Invoked(variable1, inputVar, static (v1, input) => v1.Method(input));

        then
            .Result(inputVar, static (result, expectedInput) =>
            {
                result.Should().BeOfType<ResultType>();
                result.ProcessedValue.Should().Be(expectedInput);
            });
    }
}

public sealed class MyFeatureTestsGivenContext(BaseTest test) : GivenContext<MyFeatureTestsGivenContext>(test);
```

### Test Design Principles

**Single Responsibility:**
- Each test should verify **one behavior**
- One Given-When-Then chain per test method
- If you need multiple assertions, use a single `Result()` call with multiple variables
- If you need multiple scenarios, create separate test methods

**Clarity and Explicitness:**
- Always define the **system under test (SUT)** explicitly
- Use meaningful variable names that indicate their role
- Test method names follow pattern: `MethodName_Scenario_ExpectedResult`
- No comments needed - the test structure should be self-documenting

**Framework Extension:**
- **GivenContext is designed to be extended** - don't assume limitations
- Add new overloads when you need more than 3 IVariable parameters
- Create semantic helper methods for complex setup
- Use builder pattern for multi-step initialization

**Assertion Quality:**
- Use the most specific assertion available
- Avoid redundant checks (e.g., `.NotBeNull()` before `.BeOfType<T>()`)
  - `.BeOfType<T>()` and `.BeAssignableTo<T>()` already check for null
  - Null checks ARE appropriate when followed by property access or other operations that don't verify nullability
  - Example: `.NotBeNull()` is redundant before `.BeOfType<T>()` but may be useful before `.Property.Should().Be(...)`
- Use compile-time type references instead of string matching
- Prefer `ContainSingle()` over `HaveCount(1)`
- **`.Subject` vs `.Which` pattern:**
  - Use `.Subject` when you need to assert **multiple properties** - save subject to a local variable and assert properties
  - Use `.Which` when you need to assert the **value itself or a single property** - chain directly with assertion
  - Example: `.ContainSingle().Which.Should().BeSameAs(expected)` for single assertion
  - Example: `var item = result.Should().ContainSingle().Subject; item.Property1.Should()...; item.Property2.Should()...;` for multiple properties
- Use method chaining for related assertions (e.g., `.ContainKey(...).WhoseValue.Should()...`)
- **For Option<T> assertions**: Use `.BeSome()` and `.BeNone()` extension methods, not `.IsSome.Should().BeTrue()`
  - Example with collections: `result.Should().ContainSingle().Which.Should().BeSome().Which.Should().BeOfType<ModuleMetadata>()`
  - Chain `.BeSome()` or `.BeNone()` directly after accessing the Option value

**Test Setup Order:**
- **Create SUT last in Given section** - this eliminates excessive helper methods that modify SUT after creation
- Create test data first, then create SUT with that data if needed
- Example: `.New<Module>(out var moduleVar).SutWithModule(out var sutVar, moduleVar)` instead of `.Sut(out var sutVar).New<Module>(out var moduleVar).AddToSut(sutVar, moduleVar)`

**GivenContext Method Naming:**
- Methods that create values should answer the question: "Given what value?"
- Use descriptive names like `Module()`, `EmptyContainer()`, `ContainerWith()`
- **DON'T** use "Given" prefix - it causes repetition: `Given.GivenAModule()` reads poorly
- The method name combined with `Given.` should read naturally: `Given.Module(...)` reads better than `Given.GivenAModule(...)`
- Be concise but clear - the context already indicates it's part of Given section
- **CRITICAL: Use unique parameter names in helper methods** - `CallerArgumentExpression` captures the parameter name (e.g., `out IVariable<IModule> moduleVar`), not the calling variable. If `Module(out var x)` and `DependentModule(out var y)` both have parameters named `moduleVar`, they'll collide in `VariablesContainer`. Use unique names like `baseModule` and `dependentModule`.

**Module Dependency Test Setup:**
When testing module dependencies:
1. Create base module variable (the dependency)
2. Create dependent module variable (depends on base)
3. Ensure dependent module declares dependency (e.g., `DependsOn<BaseModule>()`)
4. Create SUT (usually `ModuleMetadata` wrapping dependent module)
5. Create container with base module metadata
6. Invoke `GetDependencies(container)` on SUT
7. Assert result contains exactly one dependency using `.ContainSingle().Which.Should().BeSome()`
8. Verify the resolved dependency is the base module

**Test Data:**
- Use AutoFixture to generate test data - avoid hardcoded literals
- Only create variables (with `New`) for values that need to be asserted
- Use `Create<T>()` for intermediate values that don't need assertions
- Use `SubstituteFor()` helper instead of direct `Substitute.For<T>()` calls

**Result Parameters Semantics:**
- **IVariable/IIndexedVariable parameters in `.Result()` represent EXPECTED values or KEYS to get actual values**
- The lambda's first parameter is always the RESULT (actual value returned from When)
- Subsequent parameters are expected values to compare against OR values to check for side effects
- Example: `.Result(expectedVar, static (result, expected) => result.Should().Be(expected))`
- `result` = actual value from When
- `expected` = value from expectedVar to compare against
- **For side effect testing**: Pass the modified variable and check both result AND side effect
- Example: `.Result(servicesVar, static (result, services) => { result.Should().BeOfType<X>(); services.Should().Contain(...); })`
- **DON'T** use result variable name in IVariable parameters
- **DO** use descriptive names like `expectedVar`, `keyVar`, `valueVar`, or the actual variable name for side effects
- **ALWAYS use the `result` parameter** - if you're not using it, you're likely testing the wrong thing

**Testing Static Properties:**
- **DO** verify the actual value using specific assertions (e.g., `.Be("ExpectedValue")` or `.Be(nameof(PropertyName))`)
- **DON'T** use vague assertions like `.NotBeNullOrEmpty()` when you know the expected value
- **DON'T** test instance equality for strings (`.BeSameAs()`) - strings are interned, this tests .NET behavior, not your code
- **DON'T** test that static string properties return the same instance multiple times - this is excessive for immutable types
- Example:
  ```csharp
  // ✅ GOOD - Tests actual value
  .Result(static result => result.Should().Be(nameof(HealthCheckTags.Ready)));

  // ❌ BAD - Vague assertion
  .Result(static result => result.Should().NotBeNullOrEmpty());

  // ❌ BAD - Tests .NET string interning, not your code
  .Result(expectedVar, static (result, expected) => result.Should().BeSameAs(expected));
  ```

### Critical Test Guidelines

> **Note:** When you encounter test failures or make mistakes, document them in the [Failed Code Edits - Investigation & Prevention](#failed-code-edits---investigation--prevention) section so future AI assistants can learn from them.

**DO:**
- ✅ **Use expression-bodied lambdas for single statements** - `static x => x.Method()` not `static x => { x.Method(); }`
- ✅ **Use `Create<T>()` for values that don't need to be in test context** - avoids polluting context with unnecessary variables
- ✅ **Update these instructions when user provides requests, advice, hints, or rules** that can prevent undesired results
- ✅ **Add temporary assertions during investigation** - use assertions to verify test setup, data values, and assumptions
- ✅ **Remove them before committing** - once you've identified the issue, remove investigation assertions and keep only the assertions that verify the actual test behavior
- ❌ **Don't commit debugging assertions** - investigation assertions are for local debugging only
- ✅ **Container should include ALL modules** - in module dependency tests, the container needs both the dependency module AND the dependent module
- ✅ **Use unique PARAMETER names in GivenContext helper methods** - `CallerArgumentExpression` captures the PARAMETER name (e.g., `out var moduleVar`), NOT the calling variable name. If two methods both use `out IVariable<IModule> moduleVar`, they'll collide in VariablesContainer. Use `out IVariable<IModule> baseModule` and `out IVariable<IModule> dependentModule` instead
- ✅ Use `var then = When.Invoked(...);` followed by `then.Result(...)` on separate lines
- ✅ Define all test data in `Given` section using `.New<T>(out var variable)`
- ✅ Use AutoFixture to generate test data - avoid hardcoded literals/constants
- ✅ **Avoid hardcoded literals in tests** - use AutoFixture-generated values or variables
- ✅ Use `static` lambdas wherever possible for performance
- ✅ Use AwesomeAssertions fluent syntax (`.Should()`)
- ✅ Use `ContainSingleSingleton<T>()`, `ContainTransient<T>()`, `ContainScoped<T>()` for service collection assertions
- ✅ Use `.ContainKey(...).WhoseValue.Should()...` for dictionary assertions
- ✅ Create a separate `GivenContext` class for each test class
- ✅ Keep test methods focused on a single behavior
- ✅ Provide meaningful assertions in module tests - verify specific services are registered
- ✅ **Use ONE Given-When-Then chain per test** - don't repeat Given or When within a test
- ✅ **Use `SubstituteFor()` helper method in GivenContext** instead of `Substitute.For<T>()` in `New()`
- ✅ **Define system under test explicitly using `Sut(out var sutVar)` method** - never use `.New<SutType>(out var sut, ...)` for SUT
- ✅ **Prefer `.New<T>(out var variable)` without factory method** - factory is for corner cases only
- ✅ **Keep `Invoked` lambdas simple** - invoke only the testing method, move setup to Given section
- ✅ **Invoked must call actual method under test** - always invoke the actual method/property being tested, not identity transformations
- ✅ **DON'T add comments explaining why factory methods are used** - the code should be self-evident
- ✅ Use `ContainSingle()` instead of `HaveCount(1)`
- ✅ **Use `Result()` overload with multiple IVariable parameters** instead of multiple `.Result()` calls
- ✅ **Expected values in Result come from Given variables** - never create expected values from actual method outputs
- ✅ **Static immutable properties need only one value test** - don't test instance equality or multiple accesses for strings/immutable types
- ✅ **Skip `.NotBeNull()` checks before type checks** - `.BeOfType<T>()` and `.BeAssignableTo<T>()` already check for null
- ✅ **Use direct type references** in assertions (e.g., `typeof(FeatureManager)`) instead of string matching on type names
- ✅ **Add new overloads to GivenContext when needed** - if you need more than 3 IVariable parameters, create custom helper methods or new overloads
- ✅ **You can add overloads to `GivenContext<TContext>` in `HomeInventory.Tests.Framework`** for current and future use

**DON'T:**
- ❌ **Use block body `{ }` for lambdas with single statement** - triggers IDE0053 warning
- ❌ **Create variables only to create other variables** - pollutes test context; use `Create<T>()` instead
- ❌ **Use hardcoded literals in tests** - use AutoFixture or variables instead of `"test"`, `false`, `0`, etc.
- ❌ Create local variables in test methods (except for `then`)
- ❌ Capture local variables in `Invoked` or `Result` lambdas
- ❌ Chain `When.Invoked(...).Result(...)` without the `then` variable
- ❌ Use `Contain(d => d.ServiceType == typeof(T) && d.Lifetime == ...)` - use `ContainTransient<T>()` instead
- ❌ Add comments explaining what test does - test name should be self-documenting
- ❌ Duplicate literal values across tests - use AutoFixture or define in `Given` section
- ❌ Hardcode string literals, numbers, or constants in factory functions - use `Fixture.Create<T>()` or parameters
- ❌ Use `ContainKey(...)` then access dictionary - use `.ContainKey(...).WhoseValue.Should()...` pattern
- ❌ Write module tests with only `.NotBeNullOrEmpty()` - verify specific services
- ❌ **Repeat Given-When-Then chains in a single test** - restructure test or split into multiple tests
- ❌ **Use `Substitute.For<T>()` directly in `New()`** - use `.SubstituteFor()` helper instead
- ❌ **Use `.New<T>(out var sut, static () => new())` for SUT** - use `Sut(out var sutVar)` method instead
- ❌ **Use factory method in New when not needed** - prefer `.New<T>(out var variable)` for simple AutoFixture generation
- ❌ **Put test setup in `Invoked` lambdas** - keep them simple, only invoke the method under test
- ❌ **Use identity lambdas like `(x) => x` in Invoked** - these don't test behavior, only return inputs unchanged
- ❌ **Add comments about corner cases in factory methods** - code should be self-documenting
- ❌ **Fall out of Given-When-Then pattern** - avoid imperative setup and assertions mixed together
- ❌ **Leave system under test implicit** - always define it clearly with `Sut(out var sutVar)`
- ❌ **Use `HaveCount(1)` for single items** - use `ContainSingle()` instead
- ❌ **Call `.Result()` multiple times** - use the overload that accepts multiple variables
- ❌ **Add `.NotBeNull()` before `.BeOfType<T>()` or `.BeAssignableTo<T>()`** - redundant check
- ❌ **Use string matching on type names** - use compile-time type references
- ❌ **Assume GivenContext limitations** - you can extend it with new overloads or helper methods
- ❌ **Ignore the `result` parameter in Result lambda** - if you're not asserting against `result`, you're testing the wrong thing
- ❌ **Create expected values from actual method calls** - expected must come from Given, not from invoking the same method being tested
- ❌ **Test static immutable properties multiple times** - one value assertion is enough for strings/immutable types

### AutoFixture Usage Guidelines

**Using AutoFixture for Test Data:**

The `New` method in `GivenContext` is designed to infer types automatically in most cases, eliminating the need to specify generic type parameters explicitly:

```csharp
// ✅ GOOD - Type is inferred from out parameter with explicit type
Given
    .New(out IVariable<string> fieldVar)      // Type inferred from IVariable<string>
    .New(out IVariable<int> statusVar);       // Type inferred from IVariable<int>

// ✅ GOOD - AutoFixture generates values with out var (requires type argument)
Given
    .New<string>(out var fieldVar)    // Type argument required with 'out var'
    .New<int>(out var statusVar);      // Type argument required with 'out var'

// ✅ GOOD - Type is inferred from lambda return type
Given
    .New(out var fieldVar, static () => "some value")  // Infers string from lambda
    .New(out var statusVar, static () => 400);         // Infers int from lambda

// ❌ BAD - Redundant type argument when out parameter has explicit type
Given
    .New<string>(out IVariable<string> fieldVar);  // ❌ Compiler warning: redundant type argument

// ✅ GOOD - Type is inferred from lambda with IVariable parameters
Given
    .New(out var errorVar, errorMsgVar, static msg => new NotFoundError(msg));

// ✅ GOOD - Use up to 3 IVariable parameters + lambda
Given
    .New(out var resultVar, arg1Var, arg2Var, arg3Var, static (a1, a2, a3) =>
        new MyClass { Arg1 = a1, Arg2 = a2, Arg3 = a3 });

// ❌ BAD - Hardcoded literals
Given
    .New<MyClass>(out var objectVar, static () =>
        new MyClass { Field = "hardcoded", Status = 400 });

// ✅ GOOD - For complex setups, create helper method in GivenContext
public sealed class MyTestsGivenContext(BaseTest test) : GivenContext<MyTestsGivenContext>(test)
{
    public MyTestsGivenContext ComplexSetup(
        out IVariable<ResultType> resultVar,
        out IVariable<string> field1Var,
        out IVariable<string> field2Var)
    {
        // Omit type argument - type inferred from explicit IVariable<string>
        New(out field1Var);
        New(out field2Var);
        // Use Create<T>() for values that don't need to be variables
        // Remove 'static' when using Create<T>() from base class
        New(out resultVar, field1Var, field2Var, (f1, f2) =>
            new ResultType
            {
                Field1 = f1,
                Field2 = f2,
                Temp = Create<string>()  // ✅ Don't create tempVar just to use it here
            });
        return This;
    }
}

// Usage in test
Given
    .ComplexSetup(out var resultVar, out var field1Var, out var field2Var);
```

**Key Principles:**
- The `New` method supports up to 3 `IVariable<T>` parameters followed by a factory lambda
- Type inference works when the lambda return type is explicit
- **Avoid creating variables only to create other variables** - use `Create<T>()` instead to avoid context pollution
- **Every `New` call adds a variable to test context** - only create variables that need to be referenced in assertions
- Use `.New<T>(out var)` when AutoFixture can generate the type directly AND you need to reference it
- **You can add new overloads to `GivenContext<T>` if you need more than 3 parameters** - extend the framework as needed
- Remove `static` from lambda when using `Create<T>()` method from base class

### Extending GivenContext for Complex Scenarios

When you encounter limitations with the built-in `New` method (e.g., needing more than 3 IVariable parameters), **add new overloads or helper methods** to the test's GivenContext:

```csharp
public sealed class MyTestsGivenContext(BaseTest test) : GivenContext<MyTestsGivenContext>(test)
{
    // ✅ OPTION 1: Add a new overload supporting 5 parameters
    public MyTestsGivenContext New<T, TArg1, TArg2, TArg3, TArg4, TArg5>(
        out IVariable<T> variable,
        IVariable<TArg1> arg1,
        IVariable<TArg2> arg2,
        IVariable<TArg3> arg3,
        IVariable<TArg4> arg4,
        IVariable<TArg5> arg5,
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, T> create,
        int count = 1,
        [CallerArgumentExpression(nameof(variable))] string? name = null)
    {
        New(out variable, _ => create(
            GetValue(arg1),
            GetValue(arg2),
            GetValue(arg3),
            GetValue(arg4),
            GetValue(arg5)), count, name);
        return This;
    }

    // ✅ OPTION 2: Create a semantic helper method
    public MyTestsGivenContext CreateModuleContext(
        out IVariable<ModuleServicesContext> contextVar,
        IVariable<IServiceCollection> servicesVar,
        IVariable<IConfiguration> configVar,
        IVariable<IMetricsBuilder> metricsVar,
        IVariable<IFeatureManager> featureManagerVar,
        IVariable<IReadOnlyCollection<IModule>> modulesVar)
    {
        New(out contextVar, servicesVar, configVar, metricsVar,
            (services, config, metrics) =>
            {
                // Access remaining variables via GetValue
                var fm = GetValue(featureManagerVar);
                var modules = GetValue(modulesVar);
                return new ModuleServicesContext(services, config, metrics, fm, modules);
            });
        return This;
    }

    // ✅ OPTION 3: Use builder pattern for very complex setup
    public MyTestsGivenContext ComplexSetup(
        out IVariable<ResultType> resultVar,
        out IVariable<string> field1Var,
        out IVariable<string> field2Var)
    {
        New(out field1Var);
        New(out field2Var);
        New(out resultVar, field1Var, field2Var, (f1, f2) =>
            new ResultType
            {
                Field1 = f1,
                Field2 = f2,
                Temp = Create<string>()
            });
        return This;
    }
}

// Usage in test
Given
    .CreateModuleContext(out var contextVar, servicesVar, configVar, metricsVar, fmVar, modulesVar);
```

**When to Extend:**
- ❌ **DON'T** assume you can't extend GivenContext - it's designed to be extended!
- ✅ **DO** add new overloads when you need more than 3 IVariable parameters
- ✅ **DO** create semantic helper methods for complex object creation
- ✅ **DO** use builder pattern for multistep setup
- ❌ **DON'T** work around limitations by using non-static lambdas that access variables directly
- ❌ **DON'T** simplify tests by removing necessary assertions just to fit the 3-parameter limit

**Understanding CallerArgumentExpression and Variable Names:**

The `New` method uses `CallerArgumentExpression` to capture variable names for storage in `VariablesContainer`. This mechanism captures the **parameter name** from the helper method definition, NOT the variable name at the call site.

```csharp
// In GivenContext helper methods:
public MyTestsGivenContext Module(out IVariable<IModule> baseModule)  // ← "baseModule" is captured
{
    New(out baseModule, static () => new SubjectModule());
    return This;
}

public MyTestsGivenContext DependentModule(out IVariable<IModule> dependentModule)  // ← "dependentModule" is captured
{
    New(out dependentModule, static () => new SubjectDependentModule());
    return This;
}

// At call site:
Given
    .Module(out var moduleVar)          // ← CallerArgumentExpression sees "baseModule" from helper definition
    .DependentModule(out var depVar);   // ← CallerArgumentExpression sees "dependentModule" from helper definition

// ❌ COLLISION EXAMPLE - Both use same parameter name:
public MyTestsGivenContext Module(out IVariable<IModule> moduleVar)      // ← "moduleVar"
{ ... }
public MyTestsGivenContext DependentModule(out IVariable<IModule> moduleVar)  // ← Same "moduleVar" → COLLISION!
{ ... }
```

**Key insight:** The variable name is determined by `CreateVariable<T>(name)` where `name` comes from the parameter name in the helper method signature, not from `out var x` at the call site.

**When to Use `Create<T>()` vs `New`:**

```csharp
// ❌ BAD - Creating variables only to use them once in factory
New<string>(out var error1Var);
New<string>(out var error2Var);
New(out var modelStateVar, field1Var, field2Var, error1Var, error2Var, static (f1, f2, e1, e2) =>
{
    var ms = new ModelStateDictionary();
    ms.AddModelError(f1, e1);  // e1 and e2 are only used here
    ms.AddModelError(f2, e2);
    return ms;
});

// ✅ GOOD - Use Create<T>() for values that don't need to be variables
New(out var modelStateVar, field1Var, field2Var, (f1, f2) =>  // Note: not 'static' when using Create<T>()
{
    var ms = new ModelStateDictionary();
    ms.AddModelError(f1, Create<string>());  // ✅ Create value directly
    ms.AddModelError(f2, Create<string>());
    return ms;
});

// Rule: Create a variable with New ONLY if:
// 1. You need to reference it in test assertions
// 2. You need to pass it to multiple method calls
// 3. You need to verify its value in .Result()

// Otherwise, use Create<T>() directly in the factory function
```

**Type Argument Guidelines:**
- **Omit type argument** when the out parameter has an explicit type: `.New(out IVariable<string> myVar)`
- **Include type argument** when using `out var`: `.New<string>(out var myVar)`
- The compiler warns about "Type argument specification is redundant" when you use `.New<T>(out IVariable<T> var)`
- Type inference relies on the declared type of the out parameter or the lambda return type

**Why AutoFixture?**
- Reduces possibility of "cheating" in production code
- Ensures code works with any valid input, not just known test values
- Makes tests more robust and less brittle
- Reveals hidden dependencies on specific values

### Examples

**❌ BAD - Local variables, chaining, and hardcoded literals:**
```csharp
[Fact]
public void MyTest()
{
    Given.New<MyClass>(out var sut);

    var localData = "test data";  // ❌ Local variable
    var status = 400;              // ❌ Hardcoded literal

    When
        .Invoked(sut, s => s.Method(localData, status))  // ❌ Capturing local variables
        .Result(result => result.Should().BeTrue());      // ❌ Chained, no 'then' variable
}
```

**✅ GOOD - Proper pattern with AutoFixture:**
```csharp
[Fact]
public void Method_WithValidData_ReturnsTrue()
{
    Given
        .New<MyClass>(out var sut)
        .New<string>(out var dataVar)      // ✅ AutoFixture generates string
        .New<int>(out var statusVar);      // ✅ AutoFixture generates int

    var then = When
        .Invoked(sut, dataVar, statusVar, static (s, data, status) => s.Method(data, status));  // ✅ No capturing

    then
        .Result(static result => result.Should().BeTrue());  // ✅ Separate 'then' variable
}
```

**❌ BAD - Multiple Result() calls:**
```csharp
then
    .Result(servicesVar, static (result, expected) => result.Services.Should().BeSameAs(expected))
    .Result(configVar, static (result, expected) => result.Configuration.Should().BeSameAs(expected))
    .Result(metricsVar, static (result, expected) => result.Metrics.Should().BeSameAs(expected));
```

**✅ GOOD - Single Result() call with multiple variables:**
```csharp
then
    .Result(servicesVar, configVar, metricsVar, static (result, services, config, metrics) =>
    {
        result.Services.Should().BeSameAs(services);
        result.Configuration.Should().BeSameAs(config);
        result.Metrics.Should().BeSameAs(metrics);
    });
```

**❌ BAD - Same parameter names in GivenContext helper methods:**
```csharp
public sealed class ModuleTestsGivenContext(BaseTest test) : GivenContext<ModuleTestsGivenContext>(test)
{
    public ModuleTestsGivenContext Module(out IVariable<IModule> moduleVar)
    {
        New(out moduleVar, static () => new SubjectModule());  // ❌ Parameter named "moduleVar"
        return This;
    }

    public ModuleTestsGivenContext DependentModule(out IVariable<IModule> moduleVar)
    {
        New(out moduleVar, static () => new SubjectDependentModule());  // ❌ ALSO named "moduleVar"!
        return This;
    }
}

// CallerArgumentExpression captures "moduleVar" for BOTH methods!
// Both store values under the SAME key, causing collision!
```

**✅ GOOD - Unique parameter names in GivenContext helper methods:**
```csharp
public sealed class ModuleTestsGivenContext(BaseTest test) : GivenContext<ModuleTestsGivenContext>(test)
{
    public ModuleTestsGivenContext Module(out IVariable<IModule> baseModule)
    {
        New(out baseModule, static () => new SubjectModule());  // ✅ Parameter named "baseModule"
        return This;
    }

    public ModuleTestsGivenContext DependentModule(out IVariable<IModule> dependentModule)
    {
        New(out dependentModule, static () => new SubjectDependentModule());  // ✅ Named "dependentModule"
        return This;
    }
}

// CallerArgumentExpression captures different names: "baseModule" vs "dependentModule"
// Each stores values under unique keys - no collision!
```

**❌ BAD - Multiple Given-When-Then chains:**
```csharp
[Fact]
public void Test()
{
    Given.New<MyClass>(out var sut);
    
    var then1 = When.Invoked(sut, static s => s.Property);
    then1.Result(static r => r.Should().Be(1));
    
    // ❌ Second Given-When-Then chain in same test
    Given.New<string>(out var dataVar);
    var then2 = When.Invoked(sut, dataVar, static (s, d) => s.Process(d));
    then2.Result(static r => r.Should().BeTrue());
}
```

**✅ GOOD - Single Given-When-Then chain (or split into separate tests):**
```csharp
[Fact]
public void Process_WithData_ReturnsTrue()
{
    Given
        .New<MyClass>(out var sut)
        .New<string>(out var dataVar);

    var then = When
        .Invoked(sut, dataVar, static (s, d) => s.Process(d));

    then
        .Result(static r => r.Should().BeTrue());
}
```

**❌ BAD - Redundant null checks:**
```csharp
then
    .Result(static result => result.Should().NotBeNull())
    .Result(static result => result.Should().BeOfType<MyType>());
```

**✅ GOOD - BeOfType already checks for null:**
```csharp
then
    .Result(static result => result.Should().BeOfType<MyType>());
```

**❌ BAD - Multiple assertions for single item:**
```csharp
then
    .Result(expectedVar, static (result, expected) =>
    {
        result.Should().ContainSingle();
        result.Should().Contain(expected);
    });
```

**✅ GOOD - ContainSingle().Which pattern:**
```csharp
then
    .Result(expectedVar, static (result, expected) =>
        result.Should().ContainSingle().Which.Should().BeSameAs(expected));
```

**❌ BAD - Creating SUT first, then modifying it:**
```csharp
Given
    .Sut(out var sutVar)
    .New<Module>(out var moduleVar)
    .AddModuleToSut(sutVar, moduleVar);  // ❌ Modifying SUT after creation
```

**✅ GOOD - Create test data first, then SUT:**
```csharp
Given
    .New<Module>(out var moduleVar)
    .SutWithModule(out var sutVar, moduleVar);  // ✅ SUT created with data
```

**❌ BAD - String matching on type names:**
```csharp
services.Should().Contain(d => d.ServiceType.Name.Contains("FeatureManager"));
```

**✅ GOOD - Direct type references:**
```csharp
services.Should().Contain(d => d.ServiceType == typeof(FeatureManager));
// Or even better:
services.Should().ContainSingleton<FeatureManager>();
```

**❌ BAD - Substitute.For in New:**
```csharp
Given
    .New<IMyService>(out var serviceVar, static () => Substitute.For<IMyService>());
```

**✅ GOOD - Use SubstituteFor helper:**
```csharp
Given
    .SubstituteFor<IMyService>(out var serviceVar);
```

**❌ BAD - Implicit system under test:**
```csharp
[Fact]
public void Test()
{
    Given
        .New<ModulesCollection>(out var collectionVar, static () => new())
        .New<SubjectModule>(out var moduleVar, static () => new());

    var then = When
        .Invoked(collectionVar, moduleVar, static (collection, module) =>
        {
            collection.Add(module);
            return collection;
        });
    // What is the system under test? The collection or the module?
}
```

**✅ GOOD - Explicit system under test:**
```csharp
[Fact]
public void Add_ShouldAddModule()
{
    Given
        .Sut(out var sutVar)  // ✅ Clear: collection is SUT, uses dedicated Sut() helper
        .New<SubjectModule>(out var moduleVar, static () => new());

    var then = When
        .Invoked(sutVar, moduleVar, static (sut, module) => sut.Add(module));  // ✅ Only invokes method

    then
        .Result(sutVar, moduleVar, static (result, sut, expectedModule) =>
        {
            // ✅ Testing side effects: Assert on SUT (not result) for mutation operations
            // This pattern is correct when the method modifies state (Add, Remove, etc.)
            // The 'result' parameter contains the return value, but we verify the side effect
            sut.Should().ContainSingle().Which.Should().BeSome()
                .Which.Module.Should().BeSameAs(expectedModule);
        });
}
```

**Note on Testing Side Effects:**
- When testing **mutation methods** (Add, Remove, Update, etc.), the `Invoked` block should only call the method
- The `Result` block can assert on the **SUT's state changes** by passing `sutVar` as a parameter
- This is different from testing **pure functions**, where you assert on the returned `result`
- Both patterns are correct - choose based on what you're testing (side effects vs. return value)

**✅ GOOD - ContainSingle():**
```csharp
result.Should().ContainSingle();
```

**❌ BAD - Hardcoded literals in tests:**
```csharp
Given
    .New(out IVariable<HealthCheckStatus> statusVar, static () => new HealthCheckStatus
    {
        IsFailed = false,  // ❌ Hardcoded literal
        Description = "test"  // ❌ Hardcoded literal
    });
```

**✅ GOOD - Use AutoFixture-generated values:**
```csharp
Given
    .New<bool>(out var isFailedVar)
    .New<string>(out var descriptionVar);

var then = When
    .Invoked(isFailedVar, descriptionVar, static (isFailed, description) => new HealthCheckStatus
    {
        IsFailed = isFailed,
        Description = description
    });
```

**❌ BAD - Testing string instance equality for static properties:**
```csharp
[Fact]
public void Ready_WhenAccessedMultipleTimes_ReturnsSameInstance()
{
    Given
        .New(out var expectedInstanceVar, static () => HealthCheckTags.Ready);

    var then = When
        .Invoked(static () => HealthCheckTags.Ready);

    then
        .Result(expectedInstanceVar, static (result, expected) =>
            result.Should().BeSameAs(expected));  // ❌ Tests .NET string interning, not your code
}
```

**✅ GOOD - Testing static property actual value:**
```csharp
[Fact]
public void Ready_WhenAccessed_ReturnsExpectedValue()
{
    Given
        .New(out var tagVar, static () => HealthCheckTags.Ready);

    var then = When
        .Invoked(tagVar, static tag => tag);

    then
        .Result(static result => result.Should().Be(nameof(HealthCheckTags.Ready)));  // ✅ Tests actual value
}
```

**❌ BAD - Using New with factory for SUT:**
```csharp
Given
    .New<ModulesCollection>(out var sutVar, static () => new());
```

**✅ GOOD - Use Sut() method:**
```csharp
Given
    .Sut(out var sutVar);  // GivenContext provides Sut() method

// In GivenContext:
public ModulesCollectionTestsGivenContext Sut(out IVariable<ModulesCollection> sutVar)
{
    New(out sutVar, static () => new ModulesCollection());
    return This;
}
```

**❌ BAD - Using New with factory when not needed:**
```csharp
Given
    .New<string>(out var nameVar, static () => "John Doe");  // ❌ Hardcoded in factory
```

**✅ GOOD - Let AutoFixture generate:**
```csharp
Given
    .New<string>(out var nameVar);  // ✅ AutoFixture generates random string
```

**❌ BAD - Test setup in Invoked lambda:**
```csharp
var then = await When
    .InvokedAsync(sutVar, servicesVar, configurationVar, metricsVar,
        async static (sut, services, config, metrics, ct) =>
        {
            await sut.AddServicesAsync(services, config, metrics, ct);  // ❌ Setup
            return services;  // ❌ Return different object
        });
```

**✅ GOOD - Invoke only the method under test:**
```csharp
var then = await When
    .InvokedAsync(sutVar, servicesVar, configurationVar, metricsVar,
        static (sut, services, config, metrics, ct) =>
            sut.AddServicesAsync(services, config, metrics, ct));  // ✅ Just invoke the method

// Then assert on the result
then
    .Result(static result => result.Should().BeAssignableTo<IRegisteredModules>());
```

**❌ BAD - Identity lambda in Invoked (doesn't test behavior):**
```csharp
var then = When
    .Invoked(firstAccessVar, secondAccessVar, static (first, second) => (first, second));  // ❌ Returns tuple of inputs, no method called
```

**✅ GOOD - Invoke actual method under test:**
```csharp
var then = When
    .Invoked(static () => HealthCheckTags.Ready);  // ✅ Calls the actual property getter
```

**❌ BAD - Falling out of Given-When-Then pattern:**
```csharp
[Fact]
public void Test()
{
    // ❌ Imperative setup mixed with assertions
    var baseModule = new SubjectModule();
    var dependentModule = new SubjectDependentModule();
    dependentModule.DependsOn<SubjectModule>();

    dependentModule.Dependencies.Count.Should().Be(1);  // ❌ Assertion in setup

    var metadata = new ModuleMetadata(dependentModule);
    var container = new[] { new ModuleMetadata(baseModule) };
    var dependencies = metadata.GetDependencies(container).ToList();

    dependencies.Should().HaveCount(1);  // ❌ No Then context
}
```

**✅ GOOD - Proper Given-When-Then structure:**
```csharp
[Fact]
public void GetDependencies_WithOneDependency_ReturnsSingleDependency()
{
    Given
        .NewModule(out var baseModuleVar)
        .NewDependentModule(out var dependentModuleVar)
        .Sut(out var sutVar, dependentModuleVar)
        .Container(out var containerVar, baseModuleVar);

    var then = When
        .Invoked(sutVar, containerVar, static (sut, container) => sut.GetDependencies(container));

    then
        .Result(baseModuleVar, static (result, expectedModule) =>
        {
            var dependency = result.Should().ContainSingle().Subject;
            dependency.Should().BeSome()
                .Which.Module.Should().BeSameAs(expectedModule);
        });
}
```

**❌ BAD - HaveCount(1):**
```csharp
result.Should().HaveCount(1);
```

**✅ GOOD - ContainSingle():**
```csharp
result.Should().ContainSingle();
```

**❌ BAD - Dictionary assertions:**
```csharp
result.Extensions.Should().ContainKey("errorCodes");
var errorCodes = result.Extensions["errorCodes"] as string[];  // ❌ Separate access
errorCodes.Should().Contain("InvalidCredentialsError");
```

**✅ GOOD - Dictionary assertions:**
```csharp
result.Extensions.Should().ContainKey("errorCodes")
    .WhoseValue.Should().BeAssignableTo<string[]>()  // ✅ Chained assertion
    .Which.Should().Contain("InvalidCredentialsError");
```

**❌ BAD - Service assertions:**
```csharp
services.Should()
    .Contain(d => d.ServiceType == typeof(IMyService) && d.Lifetime == ServiceLifetime.Transient);
```

**✅ GOOD - Service assertions:**
```csharp
services.Should()
    .ContainTransient<IMyService>();
```

**❌ BAD - Module test with no meaningful assertions:**
```csharp
protected override void EnsureRegistered(IServiceCollection services) =>
    services.Should().NotBeNullOrEmpty();  // ❌ Too generic
```

**✅ GOOD - Module test with specific assertions:**
```csharp
protected override void EnsureRegistered(IServiceCollection services) =>
    services.Should()
        .Contain(d => d.ServiceType == typeof(HealthCheckService))
        .And.Contain(d => d.ServiceType == typeof(IHealthCheckPublisher));
```

### Common Test Anti-Patterns Reference

This section consolidates frequently encountered mistakes across test patterns:

| Anti-Pattern                           | Why It's Wrong          | Correct Pattern                         | Warning/Error Code |
|----------------------------------------|-------------------------|-----------------------------------------|--------------------|
| `.HaveCount(1)`                        | Not specific enough     | `.ContainSingle()`                      | FAA0001            |
| `.IsSome.Should().BeTrue()`            | Verbose, less readable  | `.Should().BeSome()`                    | -                  |
| `.NotBeNull()` before `.BeOfType<T>()` | Redundant check         | Just `.BeOfType<T>()`                   | -                  |
| String matching on types               | Brittle, error-prone    | `typeof(T)` direct reference            | -                  |
| `Substitute.For<T>()` in `New()`       | Inconsistent pattern    | `.SubstituteFor<T>()` helper            | -                  |
| Identity lambda `(x) => x`             | Tests nothing           | Call actual method: `(obj, param) => obj.Method(param)` | -                  |
| Hardcoded literals in tests            | Reduces test robustness | Use AutoFixture                         | -                  |
| Multiple `.Result()` with shared vars  | Verbose, inefficient    | Single `.Result()` with multiple params | -                  |
| Implicit SUT                           | Unclear test intent     | Explicit `Sut(out var sutVar)`          | -                  |
| Multi-line `Invoked` lambda            | Mixes setup with action | Move setup to `Given`                   | IDE0053            |
| Block body `{ }` for single expression | Unnecessary verbosity   | Use expression body `x => x.Method()`   | IDE0053            |
| Type arg when out param has type       | Redundant specification | Omit type: `.New(out IVariable<T> var)` | CS8597             |

**Quick Reference Links:**
- Single item assertions → Use `ContainSingle()` not `HaveCount(1)` (FAA0001)
- Option<T> assertions → Use `.BeSome()` / `.BeNone()` not `.IsSome.Should().BeTrue()`
- Type assertions → Use `typeof(T)` not `.Name.Contains("T")`
- Dictionary assertions → Chain `.WhoseValue.Should()...` not separate access
- Service assertions → Use `.ContainTransient<T>()` not manual lifetime checks
- SUT creation → Use `Sut(out var sutVar)` not `.New<T>(out var sut, ...)`
- Lambda expressions → Use expression body for single statements (IDE0053)

**Note on `.Which` Pattern**:
- Use `.Which` after `.BeSome()` to access the inner value: `.Should().BeSome().Which.Should().Be(...)`
- `.BeNone()` has no `.Which` since there's no value to access
- Chain `.Which` as many times as needed: `.ContainSingle().Which.Should().BeSome().Which.Should().BeOfType<T>()`


### Integration Test Pattern

For tests requiring full application context or multiple modules:

```csharp
[IntegrationTest]
public class MyIntegrationTests() : BaseTest<MyIntegrationTestsGivenContext>(static t => new(t))
{
    [Fact]
    public async Task Integration_Scenario_ExpectedBehavior()
    {
        await Given
            .HttpContext(out var contextVar)
            .New<MyRequest>(out var requestVar)
            // Note: This uses an advanced SubstituteFor overload with NSubstitute setup
            .SubstituteFor(out IVariable<IMyService> serviceVar, requestVar, (s, r) =>
                s.ProcessAsync(r, Cancellation.Token).Returns(Task.FromResult(true)))
            .InitializeHostAsync();

        var then = await When
            .InvokedAsync(serviceVar, requestVar, static (svc, req, ct) => svc.ProcessAsync(req, ct));

        then
            .Result(static result => result.Should().BeTrue());
    }
}

public sealed class MyIntegrationTestsGivenContext(BaseTest test) : GivenContext<MyIntegrationTestsGivenContext>(test);
```

**Note**: For simple mocking without setup, use `.SubstituteFor<IMyService>(out var serviceVar)`. The pattern above shows advanced usage where you configure NSubstitute returns inline.

### Test Data Builder Pattern

For complex test data, use factory methods in `Given`:

```csharp
// ❌ BAD - Hardcoded values in test data builder
Given
    .New<ComplexObject>(out var objectVar, static () => new ComplexObject
    {
        Property1 = "hardcoded",  // ❌ Hardcoded literal
        Property2 = 42,  // ❌ Hardcoded literal
        NestedObject = new NestedObject { NestedProperty = "nested" }
    });

// ✅ GOOD - Use Create<T>() for values that don't affect test logic
Given
    .New<ComplexObject>(out var objectVar, () => new ComplexObject
    {
        Property1 = Create<string>(),  // ✅ Value doesn't matter for test
        Property2 = 35,  // ✅ Specific value needed for validation test
        NestedObject = Create<NestedObject>()  // ✅ Value doesn't matter for test
    });
### Async Test Pattern

```csharp
[Fact]
public async Task AsyncMethod_Scenario_ExpectedResult()
{
    await Given
        .New<MyClass>(out var sutVar)
        .New<string>(out var dataVar)  // ✅ AutoFixture generates value
        .InitializeAsync();  // If async setup needed

    var then = await When
        .InvokedAsync(sutVar, dataVar, static (s, data, ct) => s.MethodAsync(data, ct));

    then
        .Result(dataVar, static (result, expectedData) => 
            result.Should().Contain(expectedData));  // ✅ Assert against expected from Given
}
```

### Testing Error Scenarios

```csharp
[Fact]
public void Method_WithInvalidInput_ReturnsError()
{
    Given
        .New<MyService>(out var sut)
        .New<InvalidInput>(out var inputVar);

    var then = When
        .Invoked(sut, inputVar, static (s, input) => s.Process(input));

    then
        .Result(static result =>
        {
            result.IsFaulted.Should().BeTrue();
            result.Error.Should().BeOfType<ValidationError>();
        });
}
```

### Test Organization

1. Group related tests in the same file
2. Use nested classes for organizing related scenarios (when appropriate)
3. Use `[Theory]` with `[InlineData]` or `[ClassData]` for parameterized tests
4. Use `[UnitTest]`, `[IntegrationTest]`, or `[AcceptanceTest]` attributes for categorization

### Common Assertions

```csharp
// AwesomeAssertions patterns
result.Should().BeTrue();
result.Should().Be(expected);
result.Should().BeOfType<ExpectedType>();  // Already checks for null
result.Should().BeAssignableTo<ExpectedType>();  // Already checks for null
collection.Should().HaveCount(3);
collection.Should().ContainSingle();  // ✅ Use instead of HaveCount(1)
collection.Should().Contain(item => item.Id == expectedId);
collection.Should().NotBeNullOrEmpty();

// Service collection assertions
services.Should().ContainSingleSingleton<IService>();
services.Should().ContainScoped<IService>();
services.Should().ContainTransient<IService>();

// Type assertions
services.Should().Contain(d => d.ServiceType == typeof(MyService));  // ✅ Use typeof()
services.Should().Contain(d => d.ServiceType.Name.Contains("MyService"));  // ❌ Avoid string matching

// Option<T> assertions
optionValue.Should().BeSome().Which.Should().Be(expected);
optionValue.Should().BeNone();
// With collections
result.Should().ContainSingle().Which.Should().BeSome()
    .Which.Should().BeOfType<ModuleMetadata>();

// Dictionary assertions
dict.Should().ContainKey(key).WhoseValue.Should().Be(value);  // ✅ Chained assertion

// Null checking
result.Should().BeOfType<MyType>();  // ✅ Already checks for null, no need for NotBeNull() first
result.Should().BeAssignableTo<IMyInterface>();  // ✅ Already checks for null
```

### Test Result Patterns

```csharp
// ✅ GOOD - Single Result() call with multiple related variables
then
    .Result(var1, var2, var3, static (result, v1, v2, v3) =>
    {
        result.Property1.Should().Be(v1);
        result.Property2.Should().Be(v2);
        result.Property3.Should().Be(v3);
    });

// ❌ BAD - Multiple Result() calls with related variables
then
    .Result(var1, static (result, v1) => result.Property1.Should().Be(v1))
    .Result(var2, static (result, v2) => result.Property2.Should().Be(v2))
    .Result(var3, static (result, v3) => result.Property3.Should().Be(v3));

// ✅ ACCEPTABLE - Multiple Result() for independent assertions (no shared variables)
then
    .Result(static result => result.Should().BeTrue())
    .Result(static result => result.Count.Should().Be(5));
// Note: Even these can often be combined into a single Result() call
```

### Code Quality Warnings to Avoid

When writing tests, watch for and fix these common warnings:

1. **IDE0053: Use expression body for lambda expression**
   - Use expression-bodied lambdas when the body is a single expression
   - ✅ Good: `static x => x.Property`
   - ❌ Bad: `static x => { return x.Property; }`

2. **Type argument specification is redundant**
   - Omit type arguments when the compiler can infer from the out parameter type
   - ✅ Good: `.New(out IVariable<string> myVar)`
   - ❌ Bad: `.New<string>(out IVariable<string> myVar)`

3. **Variable is assigned but its value is never used**
   - Don't create variables that are only used to create other variables
   - Use helper methods in GivenContext instead

4. **Async method lacks 'await' operators**
   - Ensure async test methods actually await async operations
   - Use `await When.InvokedAsync(...)` not `When.Invoked(...)`

5. **FAA0001: Use .Should().ContainSingle()**
   - Use ContainSingle() instead of HaveCount(1) for asserting single items
   - ✅ Good: `result.Should().ContainSingle()`
   - ❌ Bad: `result.Should().HaveCount(1)`

## Documentation

1. **Feature documentation**: Document features in `docs/features/[feature-name].md`
2. **API documentation**: Use XML comments for Swagger/OpenAPI generation
3. **Architecture decisions**: Document significant decisions
4. **README updates**: Keep README.md current with badges and instructions

## Development Commands

### Build
```cmd
dotnet build
```

### Test
```cmd
dotnet test
```

### Run API
```cmd
dotnet run --project src\HomeInventory\HomeInventory.Api
```

### Docker
```cmd
docker-compose up
```

## Additional Guidelines

1. **Central Package Management**: All package versions in `Directory.Packages.props`
2. **Configuration**: Use strongly-typed options pattern with validation
3. **Health Checks**: Implement health checks for all critical dependencies
4. **API Versioning**: Use `Asp.Versioning.Mvc.ApiExplorer` for versioning
5. **Authentication**: JWT bearer token authentication for secured endpoints
6. **OpenAPI**: All endpoints documented and accessible via Swagger UI
7. **Docker**: Application is containerized and can run in Docker/Kubernetes
8. **Feature Flags**: Use `Microsoft.FeatureManagement` for toggling features

## Code Quality & Analyzers

The project uses multiple analyzers to enforce code quality:

1. **SonarAnalyzer.CSharp** - Code quality and security analysis
2. **Microsoft.EntityFrameworkCore.Analyzers** - EF Core best practices
3. **NSubstitute.Analyzers.CSharp** - Proper mocking patterns
4. **AwesomeAssertions.Analyzers** - Test assertion quality
5. **EditorConfig** - Enforces consistent coding style (see `.editorconfig`)

**Key EditorConfig Rules:**
- File-scoped namespaces required
- UTF-8 encoding with CRLF line endings
- Expression-bodied members preferred
- Pattern matching preferred over traditional checks
- Static local functions/anonymous functions preferred when possible

## Common Patterns to Follow

### Options Pattern
```csharp
public sealed class [Feature]Options : IOptions
{
    public static SectionPath SectionPath => new("[Feature]");
    // Properties
}

// Validator
public sealed class [Feature]OptionsValidator : FluentOptionsValidator<[Feature]Options>
{
    // Validation rules
}
```

### Mapper Pattern
```csharp
[Mapper]
public static partial class [Feature]Mapper
{
    public static partial [Destination] Map([Source] source);
}
```

### Module Pattern
```csharp
public sealed class [Module]Module : IModule
{
    public IReadOnlyCollection<Type> Dependencies { get; } = [];

    public IFeatureFlag Flag => FeatureFlags.Enabled; // Or create custom flag

    public async Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken)
    {
        // Service registration
        context.Services.AddScoped<IMyService, MyService>();
    }

    public async Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken)
    {
        // Middleware and endpoint configuration
    }
}
```

**Module System:**
- Modules define `Dependencies` to ensure correct load order
- Use `IFeatureFlag` to enable/disable modules
- Register in `ApplicationModules.cs` in proper order
- Modules can access `IServiceCollection`, `IConfiguration`, and `IMetricsBuilder` in `AddServicesAsync`
- Modules can access `WebApplication` in `BuildAppAsync`

### Test Patterns Quick Reference

| Pattern                 | ❌ Avoid                                    | ✅ Prefer                              |
|-------------------------|--------------------------------------------|---------------------------------------|
| **Null checks**         | `.NotBeNull()` then `.BeOfType<T>()`       | Just `.BeOfType<T>()` (checks null)   |
| **Single item**         | `.HaveCount(1)`                            | `.ContainSingle()`                    |
| **Type checking**       | `d.ServiceType.Name.Contains("MyService")` | `d.ServiceType == typeof(MyService)`  |
| **Mocking**             | `Substitute.For<T>()` in `New()`           | `.SubstituteFor<T>()` helper          |
| **Multiple assertions** | Multiple `.Result()` calls                 | Single `.Result()` with multiple vars |
| **GWT chains**          | Multiple Given-When-Then in one test       | One chain per test                    |
| **Lambda body**         | `{ return x; }`                            | Expression-bodied `x`                 |
| **Test variables**      | Local variables outside Given              | All in `Given.New()`                  |
| **Intermediate values** | `New()` for every value                    | `Create<T>()` for non-asserted values |
| **SUT clarity**         | Implicit sut in variable names             | Explicit `sut` variable               |

### Common Test Warnings

| Warning                | Cause                                     | Solution                                |
|------------------------|-------------------------------------------|-----------------------------------------|
| **IDE0053**            | Block-bodied lambda for single expression | Use expression body: `x => x.Property`  |
| **FAA0001**            | Using `.HaveCount(1)`                     | Use `.ContainSingle()` instead          |
| **Type arg redundant** | `.New<T>(out IVariable<T> var)`           | Omit type: `.New(out IVariable<T> var)` |
| **CA2008**             | Task created without TaskScheduler        | Use proper async pattern with `await`   |

## Questions to Ask When Developing

1. Does this belong in Domain, Application, or Infrastructure?
2. Is this a new module or part of an existing one?
3. What validation rules apply to this contract?
4. What domain events should be raised?
5. What error cases need handling?
6. Does this need authentication/authorization?
7. What are the testable scenarios?

## Code Review Guidelines

When reviewing code or providing feedback on pull requests, check for these critical aspects:

### Architecture & Design Review

**✅ Check For:**
- Correct layer placement (Domain/Application/Infrastructure/Web)
- No circular dependencies between modules
- Domain layer has no infrastructure dependencies
- Dependency injection used correctly (not service locator)
- CQRS pattern followed (Commands vs Queries)
- Functional patterns used (Option, Either, Validation)

**❌ Red Flags:**
- Business logic in controllers/endpoints
- Infrastructure code in domain layer
- Direct database calls in application layer
- Tight coupling between modules
- Static dependencies or singletons for stateful services

### Code Quality Review

**✅ Check For:**
- Expression-bodied lambdas for single statements
- `static` lambdas where possible
- File-scoped namespaces
- Primary constructors used appropriately
- Immutable data structures (records, readonly)
- Pattern matching over type checks/casts
- No hardcoded literals (use configuration/constants)

**❌ Red Flags:**
- Block-bodied lambdas `{ }` for single statements (IDE0053)
- Mutable state in domain entities
- String literals scattered throughout code
- Large methods (>20 lines)
- Deep nesting (>3 levels)
- Magic numbers

### Test Review

**✅ Check For:**
- All new code has tests
- Tests follow Given-When-Then pattern
- Expression-bodied lambdas in test assertions
- AutoFixture used (no hardcoded test data)
- `.WhoseValue` pattern for dictionary assertions
- `ContainTransient/Scoped/Singleton<T>()` for service assertions
- No local variables in tests (except `then`)
- No intermediate variables (use `Create<T>()`)
- Meaningful test names: `Method_Scenario_ExpectedResult`

**❌ Red Flags:**
- Block-bodied lambdas in assertions
- Hardcoded strings/numbers in tests
- Variables created only to create other variables
- Chained `When.Invoked().Result()` without `then` variable
- Tests with only `.NotBeNullOrEmpty()` assertions
- Missing tests for critical paths
- Tests that test framework behavior, not business logic

### Security Review

**✅ Check For:**
- Passwords hashed with BCrypt
- JWT tokens validated correctly
- Authorization checks on protected endpoints
- Input validation with FluentValidation
- SQL injection prevention (EF parameterization)
- XSS prevention (proper encoding)
- CSRF protection where needed
- Sensitive data not logged

**❌ Red Flags:**
- Passwords in plain text or weak hashing
- Missing authorization checks
- User input directly in queries
- Sensitive data in logs or exceptions
- Hardcoded secrets or connection strings
- Missing validation on endpoints

### Performance Review

**✅ Check For:**
- Async/await used correctly
- Database queries optimized (no N+1)
- Proper use of `IAsyncEnumerable<T>`
- Specifications for complex queries
- `CancellationToken` passed through
- Proper disposal of resources (`using`, `IDisposable`)
- Lazy loading where appropriate

**❌ Red Flags:**
- Blocking calls in async code (`.Result`, `.Wait()`)
- N+1 query problems
- Missing `CancellationToken` parameters
- Large objects kept in memory unnecessarily
- Circular references causing memory leaks
- Missing `using` statements for disposables

### Error Handling Review

**✅ Check For:**
- LanguageExt types used (`Option<T>`, `Either<Error, T>`)
- Domain errors defined and used
- ProblemDetails returned from APIs
- Proper exception handling (no swallowed exceptions)
- Validation errors mapped to ProblemDetails
- Error messages are user-friendly
- Errors logged with context

**❌ Red Flags:**
- Exceptions used for control flow
- Empty catch blocks
- Generic `Exception` caught
- Errors not logged
- Technical error messages exposed to users
- Missing error handling on critical paths

### API/Endpoint Review

**✅ Check For:**
- Inherits from `ApiCarterModule`
- `PathPrefix` property defined
- Validation with `.WithValidationOf<T>()`
- Proper status codes (200, 201, 400, 404, 500)
- OpenAPI documentation attributes
- Authentication/authorization applied correctly
- Cancellation tokens in signatures
- Scope accessor used for dependencies
- DTOs mapped with Mapperly

**❌ Red Flags:**
- Direct use of `CarterModule` (should use `ApiCarterModule`)
- Missing validation
- Wrong HTTP methods (POST for queries, GET for commands)
- Missing API versioning
- Business logic in endpoint handler
- Direct repository access (should use services)

### Database/EF Core Review

**✅ Check For:**
- Entities in `Domain.[Module]` namespace
- Configurations in `Infrastructure.[Module]`
- Specifications for complex queries
- Proper indexes on frequently queried columns
- Navigation properties configured correctly
- Migrations named descriptively
- Audit fields (`ICreationAuditableEntity`, `IModificationAuditableEntity`)

**❌ Red Flags:**
- EF entities in application or web layer
- Missing or incorrect relationships
- N+1 queries (missing includes)
- Missing indexes on foreign keys
- Migrations modifying production data
- Unbounded queries (no pagination)

### Code Review Process

When providing review feedback:

1. **Start with positives** - What's done well
2. **Prioritize issues** - Critical > Important > Nice-to-have
3. **Be specific** - Point to exact lines and suggest fixes
4. **Provide examples** - Show correct pattern vs incorrect
5. **Explain why** - Don't just say "don't do X", explain the reason
6. **Suggest alternatives** - If rejecting an approach, propose better one
7. **Ask questions** - If unclear, ask for clarification before assuming

### Review Comment Templates

**Architecture Concern:**
```
❌ **Architecture Issue**: This business logic should be in the application layer, not in the endpoint handler.

**Why**: Endpoints should be thin wrappers that delegate to services. This keeps business logic testable and reusable.

**Suggestion**: Move this logic to `I[Module]Service` and call it from the endpoint.
```

**Test Issue:**
```
❌ **Test Issue**: This test uses hardcoded strings instead of AutoFixture.

**Why**: Hardcoded test data can lead to tests that pass with specific values but fail in production.

**Suggestion**:
- Use `.New<string>(out var emailVar)` to generate random data
- This ensures the code works with any valid input
```

**Code Quality:**
```
⚠️ **Code Quality**: This lambda has a block body for a single statement (IDE0053).

**Before**:
```csharp
.Result(static result => { result.Should().BeTrue(); })
```

**After**:
```csharp
.Result(static result => result.Should().BeTrue())
```
```

**Security Concern:**
```
🔒 **Security Issue**: Password is not being hashed before storage.

**Why**: Storing plain text passwords is a critical security vulnerability.

**Fix**: Use `BCrypt.Net.HashPassword()` before saving to database.
```

**Performance Issue:**
```
⚡ **Performance Concern**: This query may cause N+1 problem.

**Issue**: Loading `User.Orders` in a loop will execute N queries.

**Solution**: Use `.Include(u => u.Orders)` in the original query.
```

### Review Checklist

Before approving a PR, verify:

- [ ] Code follows project architecture (layers, modules, patterns)
- [ ] All new code has tests with good coverage
- [ ] No code quality warnings (IDE0053, redundant type args, etc.)
- [ ] Security best practices followed
- [ ] Performance considerations addressed
- [ ] Error handling implemented correctly
- [ ] API endpoints follow conventions
- [ ] Database changes have migrations
- [ ] Documentation updated if needed
- [ ] No breaking changes without migration path
- [ ] CI/CD pipeline passes (build, tests, format, coverage)

### When to Request Changes vs Approve

**Request Changes:**
- Security vulnerabilities
- Architecture violations
- Missing critical tests
- Breaking changes without discussion
- Performance issues that will cause problems
- Code that doesn't compile or breaks tests

**Approve with Comments:**
- Minor code quality issues
- Non-critical performance suggestions
- Style preferences (if not covered by editorconfig)
- Suggestions for future improvements
- Questions for clarification

**Approve:**
- All critical checks pass
- Code follows project standards
- Tests cover new functionality
- No security or performance concerns
- Minor comments don't block merge

---

**Remember**: Always favor immutability, functional patterns, and explicit error handling. Keep modules cohesive and loosely coupled.
