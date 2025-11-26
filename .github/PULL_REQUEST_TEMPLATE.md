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

### Self-Review Checklist

#### Code Quality
- [ ] I have reviewed my own code before requesting review
- [ ] My code follows the established coding standards and patterns
- [ ] I have chosen clear, descriptive names for new code elements
- [ ] I have removed commented-out code and debug statements
- [ ] I have eliminated code duplication
- [ ] My code builds without warnings

#### Design & Architecture  
- [ ] My changes respect the project's architectural boundaries
- [ ] I have followed established design patterns and conventions
- [ ] Dependencies flow in the correct direction
- [ ] I have not introduced circular dependencies
- [ ] I have used dependency injection appropriately
- [ ] My changes do not break existing functionality OR I have documented the breaking changes below

#### Testing
- [ ] I have added tests that prove my changes work
- [ ] My tests follow the project's testing patterns
- [ ] Existing tests still pass
- [ ] My tests are independent and can run in any order
- [ ] I have tested error/edge cases
- [ ] I have added integration tests for infrastructure changes (database, external APIs, etc.)

#### Security & Performance
- [ ] I have validated all user inputs
- [ ] I have applied appropriate authorization checks
- [ ] I have not exposed sensitive data (passwords, tokens, keys) in logs or responses  
- [ ] I have used parameterized queries or ORMs to prevent injection attacks
- [ ] I have used asynchronous operations for I/O-bound work
- [ ] I have avoided performance anti-patterns (N+1 queries, unbounded results, etc.)
- [ ] I have implemented pagination for operations that could return large datasets

#### Documentation
- [ ] I have commented complex business logic (focusing on WHY, not WHAT)
- [ ] I have updated public API documentation
- [ ] I have updated relevant feature documentation (if this changes user-facing behavior)
- [ ] I have updated setup/deployment documentation (if this changes requirements or deployment)
- [ ] My commit messages clearly describe what and why

### 🗄️ Database & Migrations (if applicable)
- [ ] Database migrations added and tested locally
- [ ] Migrations follow naming convention: `YYYYMMDDHHMMSS_DescriptiveName`
- [ ] No unbounded queries (pagination implemented where needed)
- [ ] Indexes added for frequently queried columns
- [ ] Migration rollback tested

### 🔗 Dependencies
- [ ] NuGet packages updated in `Directory.Packages.props` (if applicable)

**What breaks:**
- 

**Migration steps:**
1. 
2. 

**Deprecation timeline:**
- 

