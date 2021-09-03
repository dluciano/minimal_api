using FluentValidation;
using MinimalApi.Models.Requests;

namespace MinimalApi.Models.Requests.Validators
{
    internal sealed class CreatePostRequestValidator : AbstractValidator<UpsertPostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(p => p.Content).NotEmpty().MaximumLength(1000).MinimumLength(1);
            RuleFor(p => p.Title).NotEmpty().MaximumLength(200).MinimumLength(1);
        }
    }
}
