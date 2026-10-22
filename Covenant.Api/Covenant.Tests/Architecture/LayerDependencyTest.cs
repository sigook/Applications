using System.Reflection;
using Covenant.Api.Controllers.Shared;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Services.Candidates;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NetArchTest.Rules;
using Xunit;

namespace Covenant.Tests.Architecture;

public class LayerDependencyTest
{
    private static readonly Assembly CommonAssembly = typeof(IBaseRepository).Assembly;
    private static readonly Assembly CoreAssembly = typeof(CandidateService).Assembly;
    private static readonly Assembly ApiAssembly = typeof(CatalogController).Assembly;

    private static readonly string[] PresentationNamespaces = ["Covenant.Api.Controllers", "Covenant.Api.Consumers"];

    [Fact]
    public void CoreDoesNotDependOnInfrastructureOrAspNet()
    {
        var result = Types.InAssembly(CoreAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Covenant.Infrastructure", "Covenant.Api", "Microsoft.AspNetCore.Http", "Microsoft.AspNetCore.Mvc", "Microsoft.EntityFrameworkCore", "Azure.Messaging.ServiceBus")
            .GetResult();

        Assert.True(result.IsSuccessful, FailingTypes(result));
    }

    [Fact]
    public void CommonDoesNotDependOnOuterLayers()
    {
        var result = Types.InAssembly(CommonAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Covenant.Core.BL", "Covenant.Infrastructure", "Covenant.Api", "Covenant.Documents", "Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful, FailingTypes(result));
    }

    [Fact]
    public void ControllersAndConsumersNeverWriteThroughRepositories()
    {
        var writeMethods = typeof(IBaseRepository).GetMethods().Select(m => m.Name).ToHashSet();
        using var module = ModuleDefinition.ReadModule(ApiAssembly.Location);

        var violations = module.Types
            .SelectMany(Flatten)
            .Where(t => PresentationNamespaces.Any(ns => FullNamespace(t).StartsWith(ns)))
            .SelectMany(t => t.Methods.Where(m => m.HasBody).Select(m => (Type: t, Method: m)))
            .SelectMany(x => x.Method.Body.Instructions
                .Where(i => i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt)
                .Select(i => i.Operand)
                .OfType<MethodReference>()
                .Where(called => called.DeclaringType.FullName == typeof(IBaseRepository).FullName && writeMethods.Contains(called.Name))
                .Select(called => $"{RootType(x.Type).FullName} calls {called.Name}"))
            .Distinct()
            .ToList();

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
    }

    private static IEnumerable<TypeDefinition> Flatten(TypeDefinition type) =>
        type.NestedTypes.SelectMany(Flatten).Prepend(type);

    private static TypeDefinition RootType(TypeDefinition type) =>
        type.DeclaringType is null ? type : RootType(type.DeclaringType);

    private static string FullNamespace(TypeDefinition type) => RootType(type).Namespace;

    private static string FailingTypes(TestResult result) =>
        string.Join(Environment.NewLine, result.FailingTypeNames ?? []);
}
