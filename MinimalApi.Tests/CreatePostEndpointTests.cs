using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MinimalApi.DataAccess;
using MinimalApi.DataAccess.Context;
using MinimalApi.Models.Requests;
using MinimalApi.Models.Responses;
using NSubstitute;
using Shouldly;
using Xunit;

namespace MinimalApi.Tests;

public sealed class CreatePostEndpointTests
{
    [Theory]
    [AutoDomainData(typeof(ApiCustomizations))]
    internal async Task WhenCreateAValidPost(
        UpsertPostRequest request,
        IValidator<UpsertPostRequest> validator,
        IPostRepository postRepository,
        IUserSubProvider userSubProvider,
        Guid expectedId,
        HttpRequest httpRequest)
    {
        // Arrange
        validator.ValidateAsync(request, CancellationToken.None).Returns(Task.FromResult(new ValidationResult() { }));
        httpRequest.Path = $"/posts/{expectedId}";
        var expectedUri = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}{httpRequest.Path}";
        var expectedResult = new PostCreatedResponse(new Uri(expectedUri), new(expectedId, new Uri(expectedUri)));
        postRepository.UpsertAsync(default, CancellationToken.None).ReturnsForAnyArgs(Task.FromResult(expectedId));

        // Act
        var (IsValid, ValidationProblem, ResponseValue) = await Endpoints.CreatePost(expectedId, request, validator, postRepository, userSubProvider, httpRequest, CancellationToken.None);

        // Assert
        IsValid.ShouldBeTrue();
        ResponseValue.ShouldBeEquivalentTo(expectedResult);
        ValidationProblem.ShouldBeNull();
    }
}