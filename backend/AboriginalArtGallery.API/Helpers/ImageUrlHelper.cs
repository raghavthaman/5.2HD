namespace AboriginalArtGallery.API.Helpers;

public static class ImageUrlHelper
{
    public static string? Resolve(HttpRequest? request, string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return imageUrl;
        if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")) return imageUrl;
        
        if (request != null)
        {
            var baseUri = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return $"{baseUri}/{imageUrl.TrimStart('/')}";
        }
        
        return imageUrl;
    }
}
