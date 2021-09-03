using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using MinimalApi.DataAccess;
using MinimalApi.DataAccess.Context;
using MinimalApi.DataAccess.Dtos;
using MinimalApi.Models.Requests;
using MinimalApi.Models.Responses;
using MinimalApi.Models.Requests.Validators;
using MinimalApi.Models;



var builder = WebApplication.CreateBuilder(args);

var postConnectionString = builder.Configuration["ConnectionStrings:PostContext"];


var services = new ServiceCollection();
services.ConfigureMinimalApiDataAccessServices(postConnectionString);
await services.EnsureDatabaseMigrated().ConfigureAwait(false);

builder.Services
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
        options.AddSecurityDefinition("oidc", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OpenIdConnect,
            Description = "OAuht2 Authorization",
            OpenIdConnectUrl = new("https://demo.identityserver.io/.well-known/openid-configuration"),
            Name = "Authorization",
            Scheme = "Bearer",
            In = ParameterLocation.Header,
            BearerFormat = "JWT"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "oidc",
                        Type = ReferenceType.SecurityScheme,
                    }
                },
                new[] { "openid ", "profile", "email", "api", "offline_access" }
            }
        });
    })
    .AddFluentValidation()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
                {
                    options.Authority = "https://demo.identityserver.io";
                    options.Audience = "api";
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.RequireSignedTokens = true;
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateActor = true;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.TokenValidationParameters.ValidateTokenReplay = true;
                    options.TokenValidationParameters.ValidAudience = "api";
                    options.TokenValidationParameters.ValidAudiences = new[] { "api" };
                    options.TokenValidationParameters.ValidIssuer = "https://demo.identityserver.io";
                    options.TokenValidationParameters.ValidIssuers = new[] { "https://demo.identityserver.io" };
                    options.Validate();
                })
    .Services
    .AddAuthorization(options =>
    {
        var apiScopePolicy = new AuthorizationPolicy(new[]{
            (IAuthorizationRequirement) new DenyAnonymousAuthorizationRequirement(),
            new ClaimsAuthorizationRequirement("scope", new []{ "api" })
            }, new[] { JwtBearerDefaults.AuthenticationScheme });
        options.AddPolicy("api", apiScopePolicy);
        options.DefaultPolicy = apiScopePolicy;
    })
    .AddHealthChecks()
    .Services
    .AddHttpContextAccessor()
    .AddScoped<IUserSubProvider, UserSubProvider>()
    .AddEndpointsApiExplorer()
    .ConfigureMinimalApiDataAccessServices(postConnectionString)
    //.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<PageModelValidator>())    
    .AddTransient<IValidator<UpsertPostRequest>, CreatePostRequestValidator>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage()
        .UseSwagger()
        .UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1");
            c.DisplayOperationId();
            c.DisplayRequestDuration();
            c.OAuthClientId("interactive.public");
            c.OAuthScopes(new[] { "openid ", "profile", "email", "api", "offline_access" });
            c.OAuthUsePkce();
            c.EnableDeepLinking();
        });

if (app.Environment.IsProduction())
    app.UseHsts();

app
    .UseHttpsRedirection()
    .UseAuthentication()
    .UseAuthorization()
    .UseHealthChecks("/health");

app.MapGet("/posts/", [Authorize] async (
    [Range(0, int.MaxValue), FromQuery] int offset,
    [Range(1, 50), FromQuery] int limit,
    IPostRepository postRepository,
    HttpRequest httpRequest,
    CancellationToken cancellationToken) =>
{
    var page = new PageModel(offset, limit);
    var dto = await postRepository.GetAllAsync(new(page.Offset, page.Limit), cancellationToken).ConfigureAwait(false);

    var host = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}";

    var link = new Uri($"{host}/posts/?{nameof(page.Offset)}={page.Offset}&{nameof(PageModel.Limit)}={page.Limit}");

    var idDataResponse = dto.Ids.Select(id => new IdResponse(id, new Uri($"{host}/posts/{id}")));

    var previousPage = page.Offset <= 0 ? default : new PageModel(page.Offset - page.Limit <= 0 ? 0 : page.Offset - page.Limit, page.Limit);
    var previousPageLink = previousPage is not null ? new Uri($"{host}/posts/?{nameof(PageModel.Offset)}={previousPage.Offset}&{nameof(PageModel.Limit)}={previousPage.Limit}") : default;

    var nextPage = dto.Ids.Any() ? new PageModel(page.Offset + page.Limit, page.Limit) : default;
    var nextPageLink = nextPage is not null ? new Uri($"{host}/posts/?{nameof(page.Offset)}={nextPage.Offset}&{nameof(PageModel.Limit)}={nextPage.Limit}") : default;

    var response = new GetAllPostIdsResponse(
        link,
        new(idDataResponse),
        previousPageLink,
        nextPageLink);

    return Results.Ok(response);
});

app.MapGet("/posts/{id}", [Authorize] async (
    Guid id,
    IPostRepository postRepository,
    HttpRequest httpRequest,
    CancellationToken cancellationToken) =>
{
    var dto = await postRepository.GetById(id, cancellationToken).ConfigureAwait(false);

    if (dto is PostDto post)
    {
        var host = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}";

        var response = new PostResponse(new Uri($"{host}/posts/{id}"), new(post.Title, post.Content, post.CreatedOn));
        return Results.Ok(response);
    }

    return Results.NotFound();
});

app.MapPost("/posts/{postId}", async (
               [FromRoute] Guid postId,
               [FromBody] UpsertPostRequest request,
               [FromServices] IValidator<UpsertPostRequest> validator,
               IPostRepository postRepository,
               IUserSubProvider userSubProvider,
               HttpRequest httpRequest,
               CancellationToken cancellationToken) =>
{
    var (isValid, validationProblem, responseValue) = await Endpoints.CreatePost(postId, request, validator, postRepository, userSubProvider, httpRequest, cancellationToken).ConfigureAwait(false);
    if (!isValid && validationProblem is not null) return validationProblem.ToValidationProblemResult();
    if (isValid && responseValue is not null) return Results.Created(responseValue.Link.ToString(), responseValue);
    throw new Exception("unknow error");
});

await app.RunAsync().ConfigureAwait(false);
