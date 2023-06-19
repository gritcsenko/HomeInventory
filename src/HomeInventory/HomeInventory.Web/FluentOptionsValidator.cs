using FluentValidation;
using FluentValidation.Internal;
using Microsoft.Extensions.Options;

namespace HomeInventory.Web;

internal sealed class FluentOptionsValidator<TOptions> : IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly Action<ValidationStrategy<TOptions>>? _validationOptions;

        public FluentOptionsValidator(string? name, IValidator<TOptions> validator, Action<ValidationStrategy<TOptions>>? validationOptions)
        {
            Name = name;
            Validator = validator;
            _validationOptions = validationOptions;
        }

        public string? Name { get; }

        public IValidator<TOptions> Validator { get; }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            // null name is used to configure all named options
            if (Name != null && name != Name)
            {
                // ignored if not validating this instance
                return ValidateOptionsResult.Skip;
            }

            var result = _validationOptions is null
                ? Validator.Validate(options)
                : Validator.Validate(options, _validationOptions);


            if (result.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            return ValidateOptionsResult.Fail(result.ToString());
        }
    }
