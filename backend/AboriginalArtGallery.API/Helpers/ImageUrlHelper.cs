namespace AboriginalArtGallery.API.Helpers;

public static class ImageUrlHelper
{
    public static string? Resolve(HttpRequest? request, string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return imageUrl;
        if (imageUrl.StartsWith("http://") || imageUrl.StartsWith("https://")) return imageUrl;
        
        if (request != null)
        {
            var scheme = request.Headers["X-Forwarded-Proto"].ToString();
            if (string.IsNullOrEmpty(scheme))
            {
                scheme = request.Scheme;
            }
            var baseUri = $"{scheme}://{request.Host}{request.PathBase}";
            return $"{baseUri}/{imageUrl.TrimStart('/')}";
        }
        
        return imageUrl;
    }
}
