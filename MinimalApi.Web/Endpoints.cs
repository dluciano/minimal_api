using FluentValidation;
using MinimalApi.DataAccess;
using MinimalApi.DataAccess.Context;
using MinimalApi.DataAccess.Dtos;
using MinimalApi.Models.Requests;
using MinimalApi.Models.Requests.Validators;
using MinimalApi.Models.Responses;

internal static class Endpoints
{
    public static readonly Func<
        Guid,
        UpsertPostRequest,
        IValidator<UpsertPostRequest>,
        IPostRepository,
        IUserSubProvider,
        HttpRequest,
        CancellationToken,
        Task<(bool IsValid, ValidationProblem? ValidationProblem, PostUpsertedResponse? ResponseValue)>> UpsertPost = async (
            Guid postId,
            UpsertPostRequest request,
            IValidator<UpsertPostRequest> validator,
            IPostRepository postRepository,
            IUserSubProvider userSubProvider,
            HttpRequest httpRequest,
            CancellationToken cancellationToken) =>
          {
              // Validation
              var (isRequestValid, problemResult) = await request.ValidateAsync(validator, ValidationProblemKeys.InvalidCreatePostRequest, cancellationToken).ConfigureAwait(false);
              if (!isRequestValid && problemResult is not null) return (false, problemResult, default);

              // Map Request to DTO
              var userSub = userSubProvider.GetCurrentUserSub();
              var upsertResponse = new UpsertPostDto(postId, request.Title, request.Content, request.RowVersion ?? Array.Empty<byte>(), userSub);

              // Execute action
              await postRepository.UpsertAsync(upsertResponse, cancellationToken).ConfigureAwait(false);

              // Map Response Dto to Api Response
              var uri = new Uri($"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}{httpRequest.Path}");
              var response = new PostUpsertedResponse(uri, new(postId, uri));

              return (true, default, response);
          };
}