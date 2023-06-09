using System.Globalization;
using System.Reflection;
using Asp.Versioning;
using HomeInventory.Web.OpenApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Tests.Systems.Modules;

[UnitTest]
public class SwaggerDefaultValuesTests : BaseTest
{
    private readonly OperationFilterContext _context;
    private readonly IReadOnlyCollection<ISwaggerOperationFilter> _childFilters;
    private readonly IOpenApiValueConverter _converter = Substitute.For<IOpenApiValueConverter>();

    public SwaggerDefaultValuesTests()
    {
        var apiDescription = new ApiDescription
        {
            ActionDescriptor = new ActionDescriptor()
        };
        var schemaRegistry = Substitute.For<ISchemaGenerator>();
        var schemaRepository = new SchemaRepository();
        var methodInfo = Substitute.For<MethodInfo>();
        _context = new OperationFilterContext(apiDescription, schemaRegistry, schemaRepository, methodInfo);
        _childFilters = new ISwaggerOperationFilter[]
        {
            new DeprecatedSwaggerOperationFilter(),
            new ResponsesSwaggerOperationFilter(),
            new ParametersSwaggerOperationFilter(_converter),
        };
    }

    [Fact]
    public void Apply_Should_SetDeprecated()
    {
        var operation = new OpenApiOperation
        {
            Deprecated = false
        };
        var sut = CreateSut();
        var supported = new[] { new ApiVersion(2, 0) };
        var deprecated = new[] { new ApiVersion(1, 0) };
        _context.ApiDescription.ActionDescriptor.EndpointMetadata = new object[] { new ApiVersionMetadata(ApiVersionModel.Empty, new ApiVersionModel(supported, deprecated)) };
        _context.ApiDescription.Properties[typeof(ApiVersion)] = new ApiVersion(1, 0);

        sut.Apply(operation, _context);

        operation.Deprecated.Should().BeTrue();
    }

    [Fact]
    public void Apply_Should_NotUnsetDeprecated()
    {
        var operation = new OpenApiOperation
        {
            Deprecated = true
        };
        var sut = CreateSut();
        var supported = new[] { new ApiVersion(2, 0) };
        var deprecated = new[] { new ApiVersion(1, 0) };
        _context.ApiDescription.ActionDescriptor.EndpointMetadata = new object[] { new ApiVersionMetadata(ApiVersionModel.Empty, new ApiVersionModel(supported, deprecated)) };
        _context.ApiDescription.Properties[typeof(ApiVersion)] = new ApiVersion(2, 0);

        sut.Apply(operation, _context);

        operation.Deprecated.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(StatusCodes.Status200OK)]
    public void Apply_Should_RemoveNotSupportedContentTypes(int? statusCode)
    {
        var supportedMediaType = Fixture.Create<string>();
        var notSupportedMediaType = Fixture.Create<string>();
        var responseKey = statusCode?.ToString(CultureInfo.InvariantCulture) ?? "default";
        var supported = new ApiResponseType
        {
            IsDefaultResponse = statusCode is null,
            StatusCode = statusCode.GetValueOrDefault(),
            ApiResponseFormats =
            {
                new ApiResponseFormat
                {
                    MediaType = supportedMediaType,
                }
            },
        };
        var operation = new OpenApiOperation
        {
            Responses =
            {
                [responseKey] = new OpenApiResponse
                {
                    Content =
                    {
                        [notSupportedMediaType] = new OpenApiMediaType(),
                    }
                },
            }
        };
        var sut = CreateSut();
        _context.ApiDescription.SupportedResponseTypes.Add(supported);

        sut.Apply(operation, _context);

        operation.Responses[responseKey].Content.Should().NotContainKey(notSupportedMediaType);
    }

    [Fact]
    public void Apply_Should_SetRequiredForParameter()
    {
        var name = Fixture.Create<string>();
        var parameter = new OpenApiParameter
        {
            Name = name,
            Required = false,
        };
        var operation = new OpenApiOperation
        {
            Parameters = { parameter, },
        };
        var sut = CreateSut();
        _context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Name = name, IsRequired = true });

        sut.Apply(operation, _context);

        parameter.Required.Should().BeTrue();
    }

    [Fact]
    public void Apply_Should_NotUnSetRequiredForParameter()
    {
        var name = Fixture.Create<string>();
        var parameter = new OpenApiParameter
        {
            Name = name,
            Required = true,
        };
        var operation = new OpenApiOperation
        {
            Parameters = { parameter, },
        };
        var sut = CreateSut();
        _context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Name = name, IsRequired = false });

        sut.Apply(operation, _context);

        parameter.Required.Should().BeTrue();
    }

    [Fact]
    public void Apply_Should_SetDescriptionForParameter()
    {
        var name = Fixture.Create<string>();
        var description = Fixture.Create<string>();
        var parameter = new OpenApiParameter
        {
            Name = name,
            Description = null,
        };
        var operation = new OpenApiOperation
        {
            Parameters = { parameter, },
        };
        var sut = CreateSut();
        var metadataProvider = Substitute.For<IModelMetadataProvider>();
        var detailsProvider = Substitute.For<ICompositeMetadataDetailsProvider>();
        var identity = ModelMetadataIdentity.ForType(GetType());
        var attributes = ModelAttributes.GetAttributesForType(GetType());
        var details = new DefaultMetadataDetails(identity, attributes)
        {
            DisplayMetadata = new DisplayMetadata
            {
                Description = () => description,
            },
        };
        var metadata = new DefaultModelMetadata(metadataProvider, detailsProvider, details);
        _context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Name = name, ModelMetadata = metadata });

        sut.Apply(operation, _context);

        parameter.Description.Should().Be(description);
    }

    [Fact]
    public void Apply_Should_NotOverrideDescriptionForParameter()
    {
        var name = Fixture.Create<string>();
        var description = Fixture.Create<string>();
        var parameter = new OpenApiParameter
        {
            Name = name,
            Description = description,
        };
        var operation = new OpenApiOperation
        {
            Parameters = { parameter, },
        };
        var sut = CreateSut();
        var metadataProvider = Substitute.For<IModelMetadataProvider>();
        var detailsProvider = Substitute.For<ICompositeMetadataDetailsProvider>();
        var identity = ModelMetadataIdentity.ForType(GetType());
        var attributes = ModelAttributes.GetAttributesForType(GetType());
        var details = new DefaultMetadataDetails(identity, attributes)
        {
            DisplayMetadata = new DisplayMetadata
            {
                Description = () => Fixture.Create<string>(),
            },
        };
        var metadata = new DefaultModelMetadata(metadataProvider, detailsProvider, details);
        _context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Name = name, ModelMetadata = metadata });

        sut.Apply(operation, _context);

        parameter.Description.Should().Be(description);
    }

    [Fact]
    public void Apply_Should_SetDefaultForParameter()
    {
        var name = Fixture.Create<string>();
        var description = Fixture.Create<string>();
        var defautValue = Fixture.Create<object>();
        var expected = Substitute.For<IOpenApiAny>();
        var parameter = new OpenApiParameter
        {
            Name = name,
            Schema = new OpenApiSchema
            {
                Default = null,
            }
        };
        var operation = new OpenApiOperation
        {
            Parameters = { parameter, },
        };
        var sut = CreateSut();
        _converter.Convert(defautValue, Arg.Any<Type>()).Returns(expected);
        var metadataProvider = Substitute.For<IModelMetadataProvider>();
        var detailsProvider = Substitute.For<ICompositeMetadataDetailsProvider>();
        var identity = ModelMetadataIdentity.ForType(GetType());
        var attributes = ModelAttributes.GetAttributesForType(GetType());
        var details = new DefaultMetadataDetails(identity, attributes)
        {
            DisplayMetadata = new DisplayMetadata
            {
                Description = () => description,
            },
        };
        var metadata = new DefaultModelMetadata(metadataProvider, detailsProvider, details);
        _context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Name = name, ModelMetadata = metadata, DefaultValue = defautValue });

        sut.Apply(operation, _context);

        parameter.Schema.Default.Should().BeSameAs(expected);
    }

    [Fact]
    public void Apply_Should_NotOverrideDefaultForParameter()
    {
        var name = Fixture.Create<string>();
        var description = Fixture.Create<string>();
        var defautValue = Fixture.Create<object>();
        var expected = Substitute.For<IOpenApiAny>();
        var parameter = new OpenApiParameter
        {
            Name = name,
            Schema = new OpenApiSchema
            {
                Default = expected,
            }
        };
        var operation = new OpenApiOperation
        {
            Parameters = { parameter, },
        };
        var sut = CreateSut();
        _converter.Convert(defautValue, Arg.Any<Type>()).Returns(Substitute.For<IOpenApiAny>());
        var metadataProvider = Substitute.For<IModelMetadataProvider>();
        var detailsProvider = Substitute.For<ICompositeMetadataDetailsProvider>();
        var identity = ModelMetadataIdentity.ForType(GetType());
        var attributes = ModelAttributes.GetAttributesForType(GetType());
        var details = new DefaultMetadataDetails(identity, attributes)
        {
            DisplayMetadata = new DisplayMetadata
            {
                Description = () => description,
            },
        };
        var metadata = new DefaultModelMetadata(metadataProvider, detailsProvider, details);
        _context.ApiDescription.ParameterDescriptions.Add(new ApiParameterDescription { Name = name, ModelMetadata = metadata, DefaultValue = defautValue });

        sut.Apply(operation, _context);

        parameter.Schema.Default.Should().BeSameAs(expected);
    }

    private SwaggerDefaultValues CreateSut() => new(_childFilters);
}
