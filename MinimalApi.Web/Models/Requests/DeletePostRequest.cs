using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Models.Requests
{
    record DeletePostRequest([Required] byte[] RowVersion);
}