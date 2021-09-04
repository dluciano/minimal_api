using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Models.Requests
{
    record UpsertPostRequest([Required, MaxLength(512)] string Title, string? Content, byte[]? RowVersion);
}