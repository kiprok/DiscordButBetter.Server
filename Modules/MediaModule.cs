using System.Security.Claims;
using System.Security.Cryptography;
using Amazon.S3;
using Amazon.S3.Model;
using Carter;
using DiscordButBetter.Server.Contracts.Requests;
using DiscordButBetter.Server.Contracts.Responses;
using DiscordButBetter.Server.Database;
using DiscordButBetter.Server.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiscordButBetter.Server.Modules;

public class MediaModule : CarterModule
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public MediaModule() : base("/api/media")
    {
        RequireAuthorization();
        IncludeInOpenApi();
        WithTags("Media");
    }


    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/avatars", UploadAvatar);
    }

    private async Task<Results<Ok<UploadFileResponse>, BadRequest>> UploadAvatar(
        [FromBody] UploadFileRequest rq,
        IAmazonS3 s3Client,
        ClaimsPrincipal claim,
        DbbContext db)
    {
        if(rq.FileSize > 5_000_000) return TypedResults.BadRequest();
        if(!rq.FileType.StartsWith("image/")) return TypedResults.BadRequest();
        
        var userId = Guid.Parse(claim.Claims.First().Value);
        var fileExtension = Path.GetExtension(rq.FileName);
        var randomFileName = RandomNumberGenerator.GetString(Chars, 40);
        var newFileName = $"{userId}/{randomFileName}{fileExtension}";
        var preSign = new GetPreSignedUrlRequest
        {
            BucketName = "avatars",
            Key = newFileName,
            Expires = DateTime.Now.AddMinutes(5),
            Verb = HttpVerb.PUT,
            ContentType = rq.FileType,
            Headers =
            {
                ContentLength = rq.FileSize
            }
        };

        var url = await s3Client.GetPreSignedURLAsync(preSign);
        
        UploadedFile dbEntry = new()
        {
            OriginalFileName = rq.FileName,
            FileName = newFileName,
            UploaderId = userId,
            FileType = rq.FileType,
            FileSize = rq.FileSize,
            UploadedAt = DateTime.UtcNow,
            Hash = ""
        };
        
        await db.UploadedFiles.AddAsync(dbEntry);
        await db.SaveChangesAsync();
        

        return TypedResults.Ok(new UploadFileResponse
        {
            FileName = rq.FileName,
            NewFileName = newFileName,
            UploadUrl = url
        });
    }
}