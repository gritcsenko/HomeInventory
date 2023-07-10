﻿using System.Reflection;

namespace HomeInventory.Application;

internal class MappingAssemblySource : IMappingAssemblySource
{
    private readonly Assembly _assembly;

    public MappingAssemblySource(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        _assembly = assembly;
    }

    public Assembly GetAssembly() => _assembly;
}