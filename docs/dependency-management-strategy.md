# Dependency Management Strategy

**Last Updated:** December 18, 2025

## Overview

This document explains the multi-layered approach to preventing NuGet dependency conflicts in the HomeInventory project.

## Dependency Management Tool

The project uses **Renovate** for all dependency updates.

| Tool | Handles | Configuration File | Why |
|------|---------|-------------------|-----|
| **Renovate** | NuGet packages, Docker images, GitHub Actions | `renovate.json` | Supports sophisticated package grouping to prevent transitive conflicts |

**Note:** Dependabot was previously configured but has been removed to avoid duplicate PRs and leverage Renovate's superior grouping capabilities.

## The Problem

Automated dependency bots create individual PRs for each package update. When packages share transitive dependencies with incompatible version requirements, both PRs fail with `NU1107` or `NU1608` errors, and neither can merge independently.

### Example Conflict (December 2025)

Two separate PRs:
- Update `Reqnroll.xUnit` 3.2.1 → 3.3.0 (requires Reqnroll 3.3.0)
- Update `Expressium.LivingDoc.ReqnrollPlugin` 1.1.4 → 1.1.6 (requires Reqnroll.CustomPlugin 3.3.0)

**Error:** `NU1107: Version conflict detected for Reqnroll`

**Root cause:** Each update individually creates incompatible transitive dependency requirements.

## Why Renovate?

**Advantages over other tools:**
- ✅ Sophisticated regex-based package grouping
- ✅ Supports grouping by patterns, dependencies, update types
- ✅ Handles NuGet, Docker, and GitHub Actions in one tool
- ✅ More flexible configuration options
- ✅ Better handling of monorepos and transitive dependencies
- ✅ Can group packages with completely different names based on shared dependencies

**Example:** Renovate can group `Reqnroll.xUnit` with `Expressium.LivingDoc.ReqnrollPlugin` (different package names, shared transitive dependency) - other tools cannot.

## Multi-Layered Solution

### Layer 1: Renovate Package Grouping (Primary Prevention)

**File:** `renovate.json`

**Purpose:** Automatically group related packages so Renovate creates a single PR updating all of them together.

**Configuration:**

```json
{
  "extends": ["config:recommended", ":dependencyDashboardApproval"],
  "dockerfile": { "enabled": true },
  "docker-compose": { "enabled": true },
  "github-actions": { "enabled": true },
  "packageRules": [
    {
      "groupName": "GitHub Actions",
      "matchManagers": ["github-actions"],
      "description": "Group GitHub Actions updates together"
    },
    {
      "groupName": "Reqnroll ecosystem",
      "matchPackagePatterns": ["^Reqnroll", "^Expressium\\.LivingDoc\\.ReqnrollPlugin$"],
      "description": "Group Reqnroll packages to prevent transitive dependency conflicts"
    },
    {
      "groupName": "Entity Framework Core",
      "matchPackagePatterns": ["^Microsoft\\.EntityFrameworkCore"],
      "description": "Group EF Core packages - must stay on same version"
    },
    {
      "groupName": "LanguageExt",
      "matchPackagePatterns": ["^LanguageExt\\.(Core|Rx|Sys|CodeGen|AutoFixture|UnitTesting)$"],
      "description": "Group LanguageExt packages - must stay on same version"
    }
    // + 5 more NuGet package groups (see renovate.json for complete list)
  ]
}
```

**Benefits:**
- ✅ Prevents conflicts proactively
- ✅ Automated - no manual intervention needed
- ✅ Scales to any number of related packages

### Layer 2: Explicit Transitive Dependency Pinning (Fallback)

**File:** `src/HomeInventory/Directory.Packages.props`

**Purpose:** Override transitive dependency versions by explicitly listing them in Central Package Management.

**Configuration:**

```xml
<ItemGroup Label="Testing packages">
  <!-- Explicitly pin transitive dependency -->
  <PackageVersion Include="Reqnroll" Version="3.3.0" />
  <!-- Direct dependencies -->
  <PackageVersion Include="Reqnroll.xUnit" Version="3.3.0" />
  <PackageVersion Include="Expressium.LivingDoc.ReqnrollPlugin" Version="1.1.6" />
</ItemGroup>
```

**How it works:** When you explicitly list a transitive dependency in `Directory.Packages.props`, Central Package Management uses that version and overrides any conflicting transitive requirements.

**Benefits:**
- ✅ Provides a safety net if Renovate grouping is misconfigured
- ✅ Makes version dependencies explicit and visible
- ✅ Prevents individual package updates from breaking transitive resolution

### Layer 3: Central Package Management (Foundation)

**File:** `src/HomeInventory/Directory.Packages.props`

**Purpose:** Centralize all package versions in one file, ensuring consistency across all projects.

**Configuration:**

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup Label="Testing packages">
    <PackageVersion Include="Reqnroll.xUnit" Version="3.3.0" />
    <!-- All package versions defined here -->
  </ItemGroup>
</Project>
```

**Benefits:**
- ✅ Single source of truth for all package versions
- ✅ Prevents version mismatches across projects in the solution
- ✅ Easier to audit and update dependencies

## Known Dependency Groups

These package groups must be updated together because they share transitive dependencies:

### 1. Reqnroll Ecosystem
- `Reqnroll` (transitive, now explicit)
- `Reqnroll.xUnit`
- `Expressium.LivingDoc.ReqnrollPlugin`

**Reason:** All depend on core Reqnroll package.

### 2. Entity Framework Core
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.Relational`
- `Microsoft.EntityFrameworkCore.Analyzers`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.EntityFrameworkCore.InMemory` (testing)

**Reason:** Must all be on the same version for API compatibility.

### 3. LanguageExt
- `LanguageExt.Core`
- `LanguageExt.Rx`
- `LanguageExt.Sys`
- `LanguageExt.CodeGen`
- `LanguageExt.AutoFixture`
- `LanguageExt.UnitTesting`

**Reason:** Tightly coupled functional programming library.

### 4. Microsoft Extensions
- All `Microsoft.Extensions.*` packages
- Grouped for major/minor updates only

**Reason:** Part of ASP.NET Core framework, coordinated releases.

### 5. xUnit
- `xunit`
- `xunit.core`
- `xunit.runner.visualstudio`
- `Microsoft.NET.Test.Sdk`

**Reason:** Test framework components must be compatible.

### 6. TngTech ArchUnit
- `TngTech.ArchUnitNET`
- `TngTech.ArchUnitNET.xUnit`

**Reason:** xUnit extension requires specific version of core package.

### 7. Ardalis Specification
- `Ardalis.Specification`
- `Ardalis.Specification.EntityFrameworkCore`

**Reason:** Tight coupling between core and EF Core implementation.

### 8. Health Checks
- All `AspNetCore.HealthChecks.*` packages

**Reason:** Related packages from same ecosystem.

### 9. Serilog
- All `Serilog*` packages (core, sinks, enrichers)

**Reason:** Logging ecosystem with shared dependencies.

## Troubleshooting

### Detecting Version Conflicts

```powershell
# Show all transitive dependencies
dotnet list package --include-transitive

# Show version conflicts
dotnet restore --verbosity detailed 2>&1 | Select-String "NU1107|NU1608"

# Show outdated packages
dotnet list package --outdated
```

### Verifying Package Alignment

After updating packages, verify all transitive versions are aligned:

```powershell
dotnet list package --include-transitive | Select-String -Pattern "PackageName"
```

Example for Reqnroll:
```powershell
dotnet list package --include-transitive | Select-String -Pattern "Reqnroll"
```

Expected output (all 3.3.0):
```
> Reqnroll.xUnit                           3.3.0
> Expressium.LivingDoc.ReqnrollPlugin      1.1.6
> Reqnroll                                 3.3.0  (transitive)
> Reqnroll.CustomPlugin                    3.3.0  (transitive)
```

### Manual Conflict Resolution

If a conflict slips through:

1. **Identify the conflicting packages:**
   ```powershell
   dotnet restore 2>&1 | Select-String "NU1107|NU1608"
   ```

2. **Check what versions are required:**
   ```powershell
   dotnet list package --include-transitive | Select-String "ConflictingPackage"
   ```

3. **Update both packages together** in `Directory.Packages.props`

4. **Consider adding explicit transitive dependency** to prevent future conflicts

5. **Update `renovate.json`** to group these packages if not already grouped

## Maintenance

### Adding New Dependencies

When adding a new package that has transitive dependencies shared with existing packages:

1. **Check transitive dependencies:**
   ```powershell
   dotnet add package NewPackage
   dotnet list package --include-transitive | Select-String "NewPackage"
   ```

2. **If it shares dependencies with existing packages:**
   - Add to appropriate group in `renovate.json`
   - Consider adding transitive dependency explicitly to `Directory.Packages.props`

3. **Document the group** in this file if it represents a new ecosystem

### Reviewing Renovate PRs

Before merging a grouped package update PR:

1. ✅ Verify all packages in the group are updated together
2. ✅ Check CI passes (build, tests, restore)
3. ✅ Review transitive dependencies are aligned
4. ✅ Check for any new transitive dependencies that might need explicit pinning

## Benefits of This Approach

1. **Automated:** Renovate handles grouping, no manual intervention needed
2. **Scalable:** Add new groups as needed, existing groups work automatically
3. **Resilient:** Multiple layers prevent conflicts even if one layer fails
4. **Explicit:** Transitive dependencies are visible in `Directory.Packages.props`
5. **Maintainable:** Clear documentation of dependency relationships

## References

- [NuGet Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management)
- [Renovate Package Rules](https://docs.renovatebot.com/configuration-options/#packagerules)
- [NU1107 Error Documentation](https://learn.microsoft.com/en-us/nuget/reference/errors-and-warnings/nu1107)

---

**Key Takeaway:** This multi-layered approach ensures dependency conflicts are prevented proactively (Layer 1), caught early (Layer 2), and easy to resolve if they slip through (Layer 3).

