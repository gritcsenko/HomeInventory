﻿using Carter;
using FluentValidation;
using HomeInventory.Modules.Interfaces;

namespace HomeInventory.Web.Framework;

public abstract class BaseModuleWithCarter : BaseModule, IModuleWithCarter
{
    protected BaseModuleWithCarter() => DependsOn<WebCarterSupportModule>();

    public abstract void Configure(CarterConfigurator configurator);

    protected void AddValidatorsFromCurrentAssembly(CarterConfigurator configurator)
    {
        var validators = GetTypesDerivedFrom<IValidator>();
        configurator.WithValidators([.. validators]);
    }

    protected void AddCarterModulesFromCurrentAssembly(CarterConfigurator configurator)
    {
        var modules = GetTypesDerivedFrom<ICarterModule>()
            .Where(static t => t.IsPublic || t.IsNestedPublic);
        configurator.WithModules([.. modules]);
    }

    private IEnumerable<Type> GetTypesDerivedFrom<T>() =>
        from t in GetType().Assembly.GetTypes()
        where !t.IsAbstract && typeof(T).IsAssignableFrom(t)
        select t;
}