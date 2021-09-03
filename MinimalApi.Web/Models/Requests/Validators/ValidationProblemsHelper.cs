using FluentValidation.Results;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using FluentValidation;

namespace MinimalApi.Models.Requests.Validators
{
    internal static class ValidationProblemKeys
    {
        public const string InvalidCreatePostRequest = "InvalidCreatePostRequest ";
    }

    internal static class ValidationProblemsHelper
    {
        public static async ValueTask<(bool IsValid, ValidationProblem? ProblemResult)> ValidateAsync<TValidator>(
            this TValidator request,
            IValidator<TValidator> requestValidator,
            string problemResultKey,
            CancellationToken cancellationToken)
        {
            var validationResult = await requestValidator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validationResult.IsValid)
            {
                var result = validationResult.CreateValidationProblemResult(problemResultKey);
                return (false, result);
            }

            return (true, default);
        }

        public static IResult ToValidationProblemResult(this ValidationProblem validationProblem)
        {
            var result = Results.ValidationProblem(
                validationProblem.ModelState,
                statusCode: validationProblem.StatusCode,
                title: validationProblem.Title,
                type: validationProblem.Type); ;

            return result;
        }

        private static ValidationProblem CreateValidationProblemResult(this ValidationResult validationResult, string validationKey)
        {
            if (validationResult.IsValid)
                throw new InvalidOperationException("The validation result does not contain any validation errors");

            var modelState = new ModelStateDictionary();
            validationResult.AddToModelState(modelState, null);

            var dict = modelState.ToDictionary(m => m.Key, m => m.Value.Errors.Select(m => m.ErrorMessage).ToArray());

            var details = validationProblemDictionary[validationKey];

            var result = new ValidationProblem(
                dict,
                StatusCodes.Status422UnprocessableEntity,
                 details.Title,
                details.Type);

            return result;
        }

        private static readonly IReadOnlyDictionary<string, ValidationProblemDetail> validationProblemDictionary = new Dictionary<string, ValidationProblemDetail>()
        {
            { ValidationProblemKeys.InvalidCreatePostRequest, new("Invalid create post request", "https://example.com/probs/InvalidCreatePostRequest")}
        };

        private record ValidationProblemDetail(string Title, string Type);
    }
    public record ValidationProblem(Dictionary<string, string[]> ModelState, int StatusCode, string Title, string Type);
}
