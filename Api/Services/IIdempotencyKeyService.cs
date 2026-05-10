using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;

namespace Api.Services;

public class IdempotencyBody
{
    public ObjectResult Response { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IIdempotencyKeyService
{
    IdempotencyBody? Get(string idempotencyKey);
    bool Set(string idempotencyKey, IdempotencyBody idempotencyBody);
}