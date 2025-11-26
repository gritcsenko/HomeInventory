# PR Template DOD Design Philosophy

## The Problem with Technology-Specific Checklists

**Original approach had:**
- Specific technologies: "BCrypt", "FluentValidation", "EF parameterization"
- Specific patterns: "Option<T>", "Either<Error, T>", ".BeSome()"
- Specific conventions: "file-scoped namespaces", "primary constructors"
- Specific tools: "Mapperly", "LanguageExt", "Ardalis.Specification"

**The problem:**
- ❌ Requires updates every time the stack changes
- ❌ Becomes outdated when patterns evolve
- ❌ Doesn't apply to all types of changes
- ❌ Creates maintenance burden

## The Solution: Principle-Based Checklist

### Research: How Major OSS Projects Handle This

I analyzed PR templates from:
- **ASP.NET Core** - Focus on general quality principles
- **Roslyn** - Emphasizes testing and documentation
- **Kubernetes** - Architecture boundaries and security
- **React** - Breaking changes and migration paths

**Common pattern:** They ask "WHAT did you check?" not "HOW did you implement it?"

### Our Approach: Timeless Principles

The updated DOD focuses on **6 unchanging categories**:

#### 1. **Code Quality** (6 items)
- Self-review performed
- Follows coding standards (whatever they are)
- Clear naming (general principle)
- No debug code
- No duplication
- No warnings

**Why timeless:** These principles apply whether you use C#, Java, Python, or any language.

#### 2. **Design & Architecture** (6 items)
- Respects architectural boundaries (whatever they are)
- Follows design patterns (whatever they are)
- Dependencies flow correctly (general direction principle)
- No circular dependencies (architectural smell)
- DI used appropriately (general pattern)
- No breaking changes without documentation

**Why timeless:** Clean Architecture, Hexagonal, Onion, or any architecture - these principles apply.

#### 3. **Testing** (6 items)
- Tests prove changes work
- Follows testing patterns (whatever they are)
- Existing tests pass
- Independent tests
- Edge cases tested
- Integration tests for infrastructure

**Why timeless:** Whether you use xUnit, NUnit, Jest, JUnit - these principles don't change.

#### 4. **Security & Performance** (7 items)
- Input validation
- Authorization checks
- No sensitive data exposure
- Injection attack prevention (SQL, NoSQL, any database)
- Async for I/O (language-agnostic concept)
- Performance anti-patterns avoided
- Pagination for large datasets

**Why timeless:** Security principles and performance fundamentals don't change with technology.

#### 5. **Documentation** (5 items)
- Complex logic commented (WHY not WHAT)
- Public API documented
- Feature docs updated
- Setup/deployment docs updated
- Clear commit messages

**Why timeless:** Good documentation is always good documentation.

#### 6. **Database & Migrations** (5 items - if applicable)
- Migrations tested
- Follow naming convention (whatever it is)
- Pagination implemented
- Indexes on queried columns
- Rollback tested

**Why timeless:** Database best practices transcend specific ORMs or databases.

## Key Design Decisions

### ✅ What We Include
- **General principles** ("follow coding standards" not "use file-scoped namespaces")
- **Universal patterns** ("use DI appropriately" not "inject IScopeAccessor")
- **Technology-agnostic concepts** ("prevent injection attacks" not "use EF parameterization")
- **Outcome-focused items** ("tests prove changes work" not "use Given-When-Then")

### ❌ What We Exclude
- Specific libraries or frameworks
- Specific coding conventions
- Specific test patterns
- Specific architectural styles
- Version-specific features

### 🎯 The Golden Rule
**Ask "Will this still be true in 5 years?"**
- ✅ "I validated user input" → Yes (always important)
- ❌ "I used FluentValidation" → No (might switch libraries)

## Benefits of This Approach

### For Contributors
- ✅ **Checklist never becomes outdated** - can use today, next year, after major refactors
- ✅ **Applies to any change** - feature, bugfix, refactor, docs, CI/CD
- ✅ **Focus on outcomes** - what matters is that input is validated, not how
- ✅ **Less cognitive load** - don't need to know every specific tool to understand requirements

### For Maintainers
- ✅ **Zero maintenance** - no updates needed when:
  - Changing validation library (FluentValidation → something else)
  - Adopting new patterns (Option<T> → something else)
  - Updating frameworks (.NET 10 → .NET 11)
  - Refactoring architecture (Clean → Hexagonal)
- ✅ **Consistent standards** - principles don't drift over time
- ✅ **Easier reviews** - reviewers check principles, not implementation details

### For the Project
- ✅ **Long-term stability** - template good for years
- ✅ **Flexibility** - can evolve tech stack without updating template
- ✅ **Documentation separation** - specifics go in copilot-instructions.md, principles in PR template
- ✅ **Lower maintenance burden** - one less thing to keep in sync

## How This Works With copilot-instructions.md

### Division of Responsibilities

**PR Template (this file):**
- ✅ Timeless principles
- ✅ What outcomes to achieve
- ✅ Quality gates
- ✅ General patterns

**copilot-instructions.md:**
- ✅ Current technology stack
- ✅ Specific implementation patterns
- ✅ Detailed coding conventions
- ✅ Tool-specific guidance

**Example:**

**PR Template says:** "My code follows the established coding standards"
**Copilot Instructions detail:** "Use file-scoped namespaces, primary constructors, expression-bodied members"

**PR Template says:** "I have validated all user inputs"
**Copilot Instructions detail:** "Use FluentValidation in HomeInventory.Contracts.[Module].Validators"

**PR Template says:** "My tests follow the project's testing patterns"
**Copilot Instructions detail:** "Use BaseTest<TGivenContext> with Given-When-Then, AutoFixture for data, .BeSome() for Option<T>"

### Why This Separation Matters

1. **PR template = contract** - What must be done (stable)
2. **Copilot instructions = implementation guide** - How to do it (evolves)

When stack changes:
- ❌ Don't update PR template (principles unchanged)
- ✅ Update copilot-instructions.md (implementation changed)

## Real-World Example: Technology Migration

### Scenario: Switching from FluentValidation to Another Library

**With technology-specific DOD (OLD approach):**
```markdown
- [ ] FluentValidation used for input validation ❌ BREAKS
- [ ] Validators in HomeInventory.Contracts.[Module].Validators ❌ BREAKS
- [ ] Use AbstractValidator<T> base class ❌ BREAKS
```
**Result:** Template becomes outdated, requires update, contributors confused

**With principle-based DOD (NEW approach):**
```markdown
- [ ] I have validated all user inputs ✅ STILL VALID
```
**Result:** Template stays relevant, only copilot-instructions.md updated

### Scenario: Changing Test Patterns

**With pattern-specific DOD (OLD approach):**
```markdown
- [ ] Tests use BaseTest<TGivenContext> ❌ BREAKS
- [ ] Given-When-Then pattern used ❌ BREAKS  
- [ ] .BeSome() used for Option<T> assertions ❌ BREAKS
```
**Result:** Template outdated after test framework evolution

**With principle-based DOD (NEW approach):**
```markdown
- [ ] My tests follow the project's testing patterns ✅ STILL VALID
- [ ] I have tested error/edge cases ✅ STILL VALID
```
**Result:** Template survives test framework changes

## Comparison: Before vs After

### Before (Technology-Specific)
```markdown
✅ Code Quality & Standards
- [ ] Code follows project style guidelines (file-scoped namespaces, primary constructors)
- [ ] No Helper, Manager, Utility classes
- [ ] No code analysis warnings (IDE0053, FAA0001)

✅ Functional Patterns & Error Handling
- [ ] Option<T> for nullability
- [ ] Either<Error, T> for operations that fail
- [ ] .BeSome() / .BeNone() for assertions

✅ Testing
- [ ] AutoFixture used for test data
- [ ] Sut(out var sutVar) pattern
- [ ] .ContainSingle() not .HaveCount(1)
```
**Maintenance:** ❌ High - update when patterns/tools change

### After (Principle-Based)
```markdown
#### Code Quality
- [ ] My code follows the established coding standards and patterns
- [ ] I have chosen clear, descriptive names
- [ ] My code builds without warnings

#### Testing
- [ ] My tests follow the project's testing patterns
- [ ] I have tested error/edge cases
- [ ] My tests are independent and can run in any order
```
**Maintenance:** ✅ Zero - principles never change

## Total Checklist Items

**Main Checklist:** 30 items across 5 categories
- Code Quality: 6 items
- Design & Architecture: 6 items
- Testing: 6 items
- Security & Performance: 7 items
- Documentation: 5 items

**Optional Sections:**
- Database & Migrations: 5 items (only for DB changes)
- Dependencies & Breaking Changes: 3 items (always check)

**Total:** 38 items maximum (30 core + 8 optional)

**Down from:** 50+ technology-specific items in previous version

## Conclusion

This PR template will serve the project for **years without updates** because it focuses on:
- ✅ Timeless engineering principles
- ✅ Outcome-based requirements
- ✅ Technology-agnostic concepts
- ✅ Universal best practices

**The specifics live in copilot-instructions.md where they can evolve freely.**

**The principles live in PR template where they provide stable quality gates.**

This is how mature open-source projects maintain their PR templates - and now HomeInventory follows the same pattern. 🎯

