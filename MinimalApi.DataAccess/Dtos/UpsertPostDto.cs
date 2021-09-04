﻿namespace MinimalApi.DataAccess.Dtos
{
    public record UpsertPostDto(Guid Id, string Title, string? Content, byte[] RowVersion, string UserSub);
}
