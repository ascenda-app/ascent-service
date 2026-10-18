using System.ComponentModel.DataAnnotations;

namespace AscentService.Infrastructure.ExternalServices;

public sealed class CloudinaryOptions
{
    public const string SectionName = "Cloudinary";

    [Required]
    public string CloudName { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string ApiSecret { get; init; } = string.Empty;

    public string AuthTokenKey { get; init; } = string.Empty;

    [Required]
    public string Folder { get; init; } = "ascenda/dev/ascents";

    /// <summary>
    /// Una configuración vacía o incompleta no permite hablar con Cloudinary: el almacén
    /// no está disponible. Se comprueba antes de emitir ninguna petición de red.
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(CloudName) &&
        !string.IsNullOrWhiteSpace(ApiKey) &&
        !string.IsNullOrWhiteSpace(ApiSecret);
}
