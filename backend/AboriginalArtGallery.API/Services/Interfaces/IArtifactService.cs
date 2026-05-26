using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Services.Interfaces;

public interface IArtifactService
{
    Task<(int total, int page, int pageSize, int totalPages, IEnumerable<ArtifactResponseDto> items)> GetPagedArtifactsAsync(
        int page, int pageSize, string search, string artType, HttpRequest request);
        
    Task<ArtifactResponseDto> GetArtifactByIdAsync(int id, HttpRequest request);
    Task<ArtifactResponseDto> CreateArtifactAsync(CreateArtifactDto dto, HttpRequest request);
    Task<ArtifactResponseDto> UpdateArtifactAsync(int id, UpdateArtifactDto dto, HttpRequest request);
    Task<object> UpdateSaleStatusAsync(int id, SaleStatusDto dto);
    Task DeleteArtifactAsync(int id);
    
    Task<CommentResponseDto> AddCommentAsync(CreateCommentDto dto);
    Task<Tag> AddTagAsync(Tag tag);
}
