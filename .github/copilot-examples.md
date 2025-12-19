# Copilot Concrete Examples Reference

```toon
@meta
  updated: 2025-12
  purpose: Historical failure examples for LLM reference
  usage: Reference when facing similar issues to understand full context
```

## Quick Lookup Index

```toon
@index
  testing:
    - SubstituteFor scope → #substitutefor-scope
    - Identity lambda → #identity-lambda
    - CallerArgumentExpression → #caller-argument-expression
    - GWT violations → #gwt-violations
    - Assertions → #assertion-patterns
  languageext:
    - Option creation → #option-creation
  github-actions:
    - SHA verification → #sha-verification
    - Scorecard restrictions → #scorecard-restrictions
    - PR vs main → #pr-vs-main
  async:
    - CollectionsMarshal → #collectionsmarshal-async
  ai-behavior:
    - Tool verification → #tool-verification
```

---

## Testing Framework Examples

### SubstituteFor Scope {#substitutefor-scope}

```toon
@failure
  id: SUBST-001
  discovered: 2024-11
  category: testing/mocking
  
  symptom: "No overload for method 'SubstituteFor' takes 0 arguments"
  
  wrong_code: |
    New(out var contextVar, () => {
        var substitute = SubstituteFor<IConfiguration>();  // ❌
        return new Context(substitute);
    });
  
  root_cause: SubstituteFor() is instance method on GivenContext, not available in lambda scope
  
  correct_code: |
    New(out var contextVar, () => {
        var substitute = Substitute.For<IConfiguration>();  // ✅
        return new Context(substitute);
    });
  
  rule: Use static API (Substitute.For<T>) inside lambdas, helper methods only at context level
  
  prevention:
    - SubstituteFor() helper → only at GivenContext level
    - Inside lambdas → use Substitute.For<T>() directly
    - Remember: lambdas execute in isolated scope
```

### Identity Lambda {#identity-lambda}

```toon
@failure
  id: TEST-002
  discovered: 2024-11
  category: testing/structure
  
  symptom: Test passes but verifies nothing
  
  wrong_code: |
    var then = When
        .Invoked(firstVar, secondVar, static (first, second) => (first, second));  // ❌
  
  root_cause: Identity lambdas (x) => x return inputs unchanged, exercise no behavior
  
  correct_code: |
    // Call actual method under test
    var then = When
        .Invoked(objVar, paramVar, static (obj, param) => obj.Method(param));  // ✅
    
    // Or for property getter
    var then = When
        .Invoked(static () => HealthCheckTags.Ready);  // ✅
  
  rule: Invoked must call actual method/property being tested
  
  prevention:
    - Ask "What method am I testing?" and call it explicitly
    - Never use identity lambdas
    - Result should come from production code, not input transformation
```

### CallerArgumentExpression Collision {#caller-argument-expression}

```toon
@failure
  id: TEST-003
  discovered: 2024-11
  category: testing/variables
  
  symptom: Variables collide in VariablesContainer
  
  wrong_code: |
    public ModuleTestsGivenContext Module(out IVariable<IModule> moduleVar) { ... }
    public ModuleTestsGivenContext DependentModule(out IVariable<IModule> moduleVar) { ... }
    // Both use "moduleVar" → collision!
  
  root_cause: CallerArgumentExpression captures PARAMETER name from method signature, not call-site variable
  
  correct_code: |
    public ModuleTestsGivenContext Module(out IVariable<IModule> baseModule) { ... }
    public ModuleTestsGivenContext DependentModule(out IVariable<IModule> dependentModule) { ... }
    // "baseModule" vs "dependentModule" → no collision
  
  rule: Each helper method must have unique parameter names
  
  prevention:
    - Use descriptive parameter names: baseModule, dependentModule
    - Never use generic names like moduleVar across methods
```

### Given-When-Then Violations {#gwt-violations}

```toon
@failure
  id: TEST-004
  discovered: 2024-11
  category: testing/structure
  
  symptom: Confusing test structure, hard to maintain
  
  wrong_code: |
    [Fact]
    public void Test()
    {
        var baseModule = new SubjectModule();  // ❌ Imperative setup
        var dependentModule = new SubjectDependentModule();
        dependentModule.Dependencies.Count.Should().Be(1);  // ❌ Assertion in setup
        var metadata = new ModuleMetadata(dependentModule);
        var container = new[] { new ModuleMetadata(baseModule) };
        var dependencies = metadata.GetDependencies(container).ToList();
        dependencies.Should().HaveCount(1);  // ❌ No Then context
    }
  
  root_cause: Mixed imperative setup with assertions, no clear GWT separation
  
  correct_code: |
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
                dependency.Should().BeSome().Which.Module.Should().BeSameAs(expectedModule);
            });
    }
  
  rule: Clear separation - Given (setup), When (action), Then (assertion)
```

### Assertion Patterns {#assertion-patterns}

```toon
@patterns
  id: ASSERT-PATTERNS
  category: testing/assertions
  
  patterns:
    single_item:
      wrong: ".HaveCount(1)"
      correct: ".ContainSingle()"
      reason: More specific, better error messages
    
    null_before_type:
      wrong: ".NotBeNull() then .BeOfType<T>()"
      correct: ".BeOfType<T>() only"
      reason: BeOfType already checks null
    
    type_matching:
      wrong: ".Name.Contains(\"FeatureManager\")"
      correct: "typeof(FeatureManager)"
      reason: Compile-time safety, not brittle strings
    
    dictionary:
      wrong: ".ContainKey(k); dict[k].Should()..."
      correct: ".ContainKey(k).WhoseValue.Should()..."
      reason: Chained assertions are cleaner
    
    services:
      wrong: ".Contain(d => d.ServiceType == typeof(T) && d.Lifetime == ...)"
      correct: ".ContainTransient<T>() / .ContainScoped<T>() / .ContainSingleton<T>()"
      reason: Purpose-built helpers
    
    multiple_results:
      wrong: "Multiple .Result() calls with shared variables"
      correct: "Single .Result() with multiple params"
      reason: More efficient, clearer intent
```

---

## LanguageExt v5 Examples

### Option Creation {#option-creation}

```toon
@failure
  id: LANG-001
  discovered: 2024-11
  category: languageext/option
  
  symptom: "Cannot convert from 'Entity' to 'Entity?'"
  
  wrong_code: |
    return Prelude.Optional(entity);  // ❌ Wrong for non-nullable
  
  root_cause: |
    LanguageExt v5 API:
    - Optional(T?) → for nullable VALUE types (int?, DateTime?)
    - Some(T) → for guaranteed non-null REFERENCE types
    - .NoneIfNull() → for nullable reference types
  
  correct_code: |
    // Non-null guaranteed
    return entity is not null
        ? Prelude.Some(entity)
        : Option<Entity>.None;
    
    // Pattern matching
    return await query.FirstOrDefaultAsync(ct) is { } entity
        ? Prelude.Some(entity)
        : Option<Entity>.None;
    
    // Nullable reference
    return nullableValue.NoneIfNull();
  
  rule: Match API to nullability - Some() for non-null, Optional() for nullable value types
```

---

## GitHub Actions Examples

### SHA Verification {#sha-verification}

```toon
@failures
  id: GHA-001
  discovered: 2024-11
  category: github-actions/sha
  
  mistakes:
    guessed_sha:
      symptom: "An action could not be found at the URI"
      wrong: "uses: actions/checkout@db14d8b7fea37acffdd656bd35b81b8f8b3bb8ad"
      correct: |
        # Verify first
        git ls-remote --tags https://github.com/actions/checkout.git | Select-String "v6"
        # Use verified SHA
        uses: actions/checkout@1af3b93b6815bc44a9784bd300feb67ff0d1eeb3
    
    reactive_updates:
      symptom: Multiple workflow failures, 11 actions needed fixing
      wrong: "Update one action at a time as errors appear"
      correct: "Verify ALL actions systematically BEFORE any update"
    
    deprecated_sha:
      symptom: "deprecated version of actions/cache"
      wrong: "Using old SHA like v4.1.2"
      correct: "Always use LATEST in major series (v4.3.0)"
  
  verification_command: |
    git ls-remote --tags https://github.com/[owner]/[action].git | Select-String "v[major]"
  
  rule: NEVER guess SHAs - ALWAYS verify with git ls-remote
```

### Scorecard Restrictions {#scorecard-restrictions}

```toon
@failure
  id: GHA-002
  discovered: 2024-11
  category: github-actions/scorecard
  
  context: When ossf/scorecard-action has publish_results=true
  
  restrictions:
    - NO global env or defaults sections
    - NO env section in scorecard job
    - NO disallowed actions (setup-dotnet, cache, etc.)
    - NO run steps - ONLY uses allowed
  
  wrong_patterns:
    global_env: |
      env:
        CI: true
      jobs:
        security-scan: ...
    
    job_env: |
      jobs:
        security-scan:
          env:
            CI: true
          steps: ...
    
    disallowed_action: |
      jobs:
        security-scan:
          steps:
            - uses: actions/setup-dotnet@v5  # ❌
            - uses: ossf/scorecard-action@v2
    
    run_step: |
      jobs:
        scorecard:
          steps:
            - uses: ossf/scorecard-action@v2
            - run: echo "checking"  # ❌
  
  correct_pattern: |
    jobs:
      vulnerability-scan:  # Separate job for .NET stuff
        env:
          CI: true
        steps:
          - uses: actions/checkout@v6
          - uses: actions/setup-dotnet@v5
          - run: dotnet list package --vulnerable
      
      scorecard:  # Minimal scorecard job
        steps:
          - uses: actions/checkout@v6
          - uses: ossf/scorecard-action@v2
            with:
              publish_results: true
          - uses: github/codeql-action/upload-sarif@v3
  
  rule: Split into TWO jobs - vulnerability-scan (can have anything) + scorecard (minimal)
```

### PR vs Main Validation {#pr-vs-main}

```toon
@failure
  id: GHA-003
  discovered: 2025-11
  category: github-actions/ci
  
  symptom: PR CI green, main CI red after merge
  
  behavior_matrix:
    pull_request:
      publishing: skipped
      validation: lenient
      result: passes even with env vars
    push_to_main:
      publishing: attempts
      validation: STRICT
      result: fails if env vars present
  
  root_cause: Scorecard validation only enforced when publishing (main branch)
  
  prevention:
    - Test workflow changes on test branch (triggers push event)
    - Verify scorecard job has NO env section
    - Check merge commits for reintroduced env
    - Run manual workflow_dispatch for push-like validation
  
  rule: PR validation is lenient - always test push event behavior
```

---

## Async Patterns

### CollectionsMarshal Async {#collectionsmarshal-async}

```toon
@failure
  id: ASYNC-001
  discovered: 2024-11
  category: async/refs
  
  symptom: "Cannot use ref variable across await boundary"
  
  wrong_code: |
    public async ValueTask<TResult> GetOrAddAsync<TResult>(...)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
        var newValue = await createValueFunc(key);  // ← await here
        val = newValue;  // ❌ ref invalid after await
        return (TResult)val!;
    }
  
  root_cause: C# compiler enforces refs cannot cross await boundaries
  
  correct_patterns:
    sync: |
      // Use ref for performance
      public TResult GetOrAdd<TResult>(...)
      {
          ref var val = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out var exists);
          if (!exists) val = createValueFunc(key);
          return (TResult)val!;
      }
    
    async: |
      // Use TryGetValue pattern
      public async ValueTask<TResult> GetOrAddAsync<TResult>(...)
      {
          if (dictionary.TryGetValue(key, out var existingValue))
              return (TResult)existingValue!;
          var newValue = await createValueFunc(key);
          dictionary[key] = newValue;
          return newValue;
      }
  
  rule: CollectionsMarshal optimization only for sync code - use TryGetValue for async
```

---

## AI Behavior Examples

### Tool Verification {#tool-verification}

```toon
@failure
  id: AI-001
  discovered: 2025-11
  category: ai/tools
  
  symptom: User discovers instructions weren't updated after AI claimed success
  
  wrong_behavior: |
    AI: "I'll update the instructions now."
    [Calls replace_string_in_file]
    [Response: "oldString not found"]
    AI: "Done! I've updated the instructions."  ← LIE
  
  correct_behavior: |
    AI: "Let me update the instructions."
    [Calls replace_string_in_file]
    [Response: "oldString not found"]
    AI: "The edit failed. Let me read the file to find the exact text."
    [Calls read_file]
    AI: "Found it - whitespace was different. Trying again."
    [Calls replace_string_in_file with correct text]
    [Response: "successfully edited"]
    AI: "Done! Successfully added to [section]."
  
  rule: |
    ALWAYS check tool response after every operation
    IF failed → read error, understand cause, retry
    NEVER claim success without seeing confirmation
```

---

## Historical Reference

### GitHub Actions SHA Table (November 2024)

```toon
@reference
  id: SHA-TABLE
  as_of: 2024-11
  note: SHAs become outdated - always verify with git ls-remote
  
  actions:
    actions/checkout:
      sha: 1af3b93b6815bc44a9784bd300feb67ff0d1eeb3
      version: v6.0.0
    actions/setup-dotnet:
      sha: 2016bd2012dba4e32de620c46fe006a3ac9f0602
      version: v5.0.1
    actions/cache:
      sha: 0057852bfaa89a56745cba8c7296529d2fc39830
      version: v4.3.0
    actions/upload-artifact:
      sha: 330a01c490aca151604b8cf639adc76d48f6c5d4
      version: v5.0.0
    ossf/scorecard-action:
      sha: 99c09fe975337306107572b4fdf4db224cf8e2f2
      version: v2.4.3
    github/codeql-action/upload-sarif:
      sha: 5ad83d3202da6e473f763d732b591299ae4e380c
      version: v3
    dorny/test-reporter:
      sha: 894765a932a426ee30919ffd3b5fd3b53c0e26b8
      version: v2.2.0
```

---

## Anti-Pattern Quick Reference

```toon
@antipatterns
  testing:
    - wrong: "SubstituteFor() inside lambda"
      correct: "Substitute.For<T>() inside lambda"
    - wrong: ".HaveCount(1)"
      correct: ".ContainSingle()"
    - wrong: ".NotBeNull() before .BeOfType<T>()"
      correct: ".BeOfType<T>() only"
    - wrong: "String matching on type names"
      correct: "typeof(T) reference"
    - wrong: "Identity lambda (x) => x"
      correct: "Call actual method"
    - wrong: "Multiple GWT chains"
      correct: "One chain per test"
    - wrong: "Same parameter names in helpers"
      correct: "Unique parameter names"
  
  languageext:
    - wrong: "Prelude.Optional() for non-nullable"
      correct: "Prelude.Some() for non-null"
  
  github_actions:
    - wrong: "Guessed SHAs"
      correct: "Verified with git ls-remote"
    - wrong: "env in Scorecard job"
      correct: "Separate jobs"
  
  async:
    - wrong: "CollectionsMarshal across await"
      correct: "TryGetValue pattern"
  
  ai:
    - wrong: "Claim success without checking"
      correct: "Always verify tool response"
```

