using System.Net;
using AscentService.Application.Abstractions;
using AscentService.Infrastructure.ExternalServices;
using CloudinaryDotNet;
using Common.Domain.Results;
using Common.Infrastructure.Observability;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace AscentService.IntegrationTests.ExternalServices;

public sealed class CloudinaryPhotoStorageTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(503)]
    public void IsStorageUnavailable_WithProviderOrCredentialFailures_IsTrue(int statusCode) =>
        CloudinaryPhotoStorage.IsStorageUnavailable((HttpStatusCode)statusCode).Should().BeTrue();

    [Theory]
    [InlineData(400)]
    [InlineData(413)]
    [InlineData(422)]
    public void IsStorageUnavailable_WithContentRejections_IsFalse(int statusCode) =>
        CloudinaryPhotoStorage.IsStorageUnavailable((HttpStatusCode)statusCode).Should().BeFalse();

    [Fact]
    public async Task UploadAsync_WithoutConfiguration_ReturnsStorageUnavailableWithoutNetwork()
    {
        CloudinaryPhotoStorage storage = CreateStorage(new CloudinaryOptions { CloudName = "ascenda-test" });

        Result<StoredPhoto> result = await storage.UploadAsync(
            new PhotoFile(new byte[] { 1, 2, 3 }, "image/png", "photo.png"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ascent.PhotoStorageUnavailable");
        result.Error.Type.Should().Be(Common.Domain.Results.ErrorType.Unavailable);
    }

    private static CloudinaryPhotoStorage CreateStorage(CloudinaryOptions options) =>
        new(
            new CloudinaryFactory(Options.Create(options)),
            new CompensationMetrics(),
            NullLogger<CloudinaryPhotoStorage>.Instance);
}
