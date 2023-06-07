﻿using Xunit.Sdk;

namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Applies a category trait to a test.
/// </summary>
[TraitDiscoverer(CategoryTraitDiscoverer._fullyQualifiedName, CategoryTraitDiscoverer._assemblyName)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class CategoryTraitAttribute : Attribute, ITraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryTraitAttribute"/> class.
    /// </summary>
    /// <param name="category">The category of the test (e.g. Unit or Integration)</param>
    protected CategoryTraitAttribute(string category)
    {
        Category = category;
    }

    /// <summary>
    /// Gets the value of the Category trait.
    /// </summary>
    public string Category { get; private set; }
}
