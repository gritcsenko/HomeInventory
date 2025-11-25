<!--
Thank you for contributing to HomeInventory!

Please read the contributing guide before raising a pull request:
https://github.com/gritcsenko/HomeInventory/blob/main/.github/CONTRIBUTING.md
-->

# Description

Please include a summary of the change and which issue is fixed. Include relevant motivation, context, and any dependencies required for this change.

## Type of Change

Please delete options that are not relevant:

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Refactoring (no functional changes)
- [ ] Performance improvement
- [ ] Test improvement
- [ ] CI/CD change

## Related Issues

Fixes # (issue number)

Related to # (issue number)

Depends on # (PR number)

Blocks # (issue number)

## Testing Evidence

### Coverage Report
_Will be generated automatically by CI - review the bot comment below_

### Manual Testing
Please describe the manual tests you ran to verify your changes:

1. 
2. 

**Test Configuration**:
- OS: [e.g., Windows 11, Ubuntu 22.04]
- .NET SDK: [e.g., 10.0]
- Browser (if UI): [e.g., Chrome 120]

## Definition of Done

> **Note**: Build, tests, format, security scan, architecture validation, and coverage are automatically checked by CI. PR cannot be merged if automated checks fail. Review bot comments for detailed reports.

### Self-Review Checklist

#### Code Quality
- [ ] I have performed a thorough self-review of my code
- [ ] My code follows the style guidelines of this project
- [ ] I have chosen meaningful names for classes, methods, and variables
- [ ] I have avoided code duplication and extracted reusable logic
- [ ] I have handled errors appropriately using functional patterns (Option, Either, Validation)

#### Architecture & Design
- [ ] Changes follow Clean Architecture principles (Domain → Application → Infrastructure → Web)
- [ ] Domain layer has no external dependencies (except LanguageExt)
- [ ] Dependency injection used correctly (proper lifetimes, no service locator)
- [ ] Functional patterns used appropriately (Option for nullability, Either for errors)
- [ ] CQRS pattern followed (Commands return `Option<Error>`, Queries return `IQueryResult<T>`)
- [ ] No breaking changes OR migration path documented below

#### Testing
- [ ] I have added/updated tests that prove my changes work
- [ ] Tests follow Given-When-Then pattern using `BaseTest<TGivenContext>`
- [ ] AutoFixture used for test data (no hardcoded literals)
- [ ] Tests use `.BeSome()` / `.BeNone()` for Option<T> assertions
- [ ] Tests use `ContainSingle()` not `HaveCount(1)` (FAA0001)
- [ ] System under test explicitly defined with `Sut(out var sutVar)`
- [ ] Test coverage meets minimum thresholds for changed code

#### Documentation
- [ ] XML comments added/updated for public APIs (for Swagger/OpenAPI)
- [ ] Feature documentation updated in `docs/features/[feature-name].md` (if applicable)
- [ ] README.md updated (if public API or setup changes)
- [ ] Architecture decisions documented (for significant design changes)

### 🗄️ Database & Migrations (if applicable)
- [ ] Database migrations added and tested locally
- [ ] Migrations follow naming convention: `YYYYMMDDHHMMSS_DescriptiveName`
- [ ] No unbounded queries (pagination implemented where needed)
- [ ] Indexes added for frequently queried columns
- [ ] Migration rollback tested

### 🔗 Dependencies & Breaking Changes
- [ ] Any dependent changes have been merged and published in downstream modules
- [ ] NuGet packages updated in `Directory.Packages.props` (if applicable)
- [ ] **No breaking changes** OR **Migration path provided below**

### Breaking Changes (if applicable)
_If this PR introduces breaking changes, document the migration path:_

**What breaks:**
- 

**Migration steps:**
1. 
2. 

**Deprecation timeline:**
- 

## Additional Context

Add any other context, screenshots, or information about the pull request here.
