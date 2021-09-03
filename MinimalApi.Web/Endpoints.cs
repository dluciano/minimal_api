using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
        Task<(bool IsValid, ValidationProblem? ValidationProblem, PostCreatedResponse? ResponseValue)>> CreatePost =[Authorize] async (
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
              var createPostDto = new UpsertPostDto(postId, request.Title, request.Content, userSub);

              // Execute action
              await postRepository.UpsertAsync(createPostDto, cancellationToken).ConfigureAwait(false);

              // Map Response Dto to Api Response
              var uri = new Uri($"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}{httpRequest.Path}");
              var response = new PostCreatedResponse(uri, new(postId, uri));

              return (true, default, response);
          };
}