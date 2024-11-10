﻿using HomeInventory.Api;
using HomeInventory.Application.Framework;
using HomeInventory.Contracts.Validations;
using HomeInventory.Domain;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure;
using HomeInventory.Web.Framework;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.Architecture;

public static class AssemblyReferences
{
    public static IAssemblyReference Core { get; } = new BaseAssemblyReference(typeof(Execute));
    public static IAssemblyReference DomainPrimitives { get; } = new BaseAssemblyReference(typeof(ValueObject<>));
    public static IAssemblyReference Domain { get; } = new BaseAssemblyReference(typeof(DomainModule));
    public static IAssemblyReference Application { get; } = new BaseAssemblyReference(typeof(ApplicationMediatrSupportModule));
    public static IAssemblyReference WebUserManagement { get; } = new BaseAssemblyReference(typeof(WebUerManagementMappingModule));
    public static IAssemblyReference ContractValidations { get; } = new BaseAssemblyReference(typeof(ContractsValidationsModule));
    public static IAssemblyReference Infrastructure { get; } = new BaseAssemblyReference(typeof(InfrastructureMappingModule));
    public static IAssemblyReference Api { get; } = new BaseAssemblyReference(typeof(LoggingModule));
    public static IAssemblyReference Web { get; } = new BaseAssemblyReference(typeof(WebCarterSupportModule));
    public static IAssemblyReference WebFramework { get; } = new BaseAssemblyReference(typeof(ApiCarterModule));
}