using AutoMapper;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Exceptions;
using AboriginalArtGallery.API.Helpers;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Services.Implementations;

public class ArtifactService : IArtifactService
{
    private readonly IArtifactRepository _artifactRepository;
    private readonly IArtistRepository _artistRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ArtifactService(
        IArtifactRepository artifactRepository,
        IArtistRepository artistRepository,
        ICommentRepository commentRepository,
        ITagRepository tagRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _artifactRepository = artifactRepository;
        _artistRepository = artistRepository;
        _commentRepository = commentRepository;
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<(int total, int page, int pageSize, int totalPages, IEnumerable<ArtifactResponseDto> items)> GetPagedArtifactsAsync(
        int page, int pageSize, string search, string artType, HttpRequest request)
    {
        var (total, items) = await _artifactRepository.GetPagedArtifactsAsync(page, pageSize, search, artType);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);

        var dtos = _mapper.Map<IEnumerable<ArtifactResponseDto>>(items);
        foreach (var dto in dtos)
        {
            dto.ImageUrl = ImageUrlHelper.Resolve(request, dto.ImageUrl);
        }

        return (total, page, pageSize, totalPages, dtos);
    }

    public async Task<ArtifactResponseDto> GetArtifactByIdAsync(int id, HttpRequest request)
    {
        var artifact = await _artifactRepository.GetArtifactWithDetailsAsync(id);
        if (artifact == null)
            throw new NotFoundException("Artifact not found.");

        var dto = _mapper.Map<ArtifactResponseDto>(artifact);
        dto.ImageUrl = ImageUrlHelper.Resolve(request, dto.ImageUrl);
        dto.Tags = await _artifactRepository.GetTagsForArtifactAsync(id);

        return dto;
    }

    public async Task<ArtifactResponseDto> CreateArtifactAsync(CreateArtifactDto dto, HttpRequest request)
    {
        var artist = await _artistRepository.GetByIdAsync(dto.ArtistId);
        if (artist == null)
            throw new ValidationException("Invalid ArtistId");

        var artifact = new Artifact
        {
            Title = dto.Title,
            Description = dto.Description,
            ArtType = dto.ArtType,
            YearCreated = dto.YearCreated,
            ImageUrl = dto.ImageUrl,
            Price = dto.Price,
            IsAvailableForPurchase = dto.IsAvailableForPurchase,
            StockQuantity = dto.StockQuantity,
            ArtistId = dto.ArtistId,
            CreatedAt = DateTime.UtcNow
        };

        await _artifactRepository.AddAsync(artifact);
        await _artifactRepository.SaveChangesAsync();

        var responseDto = _mapper.Map<ArtifactResponseDto>(artifact);
        responseDto.ArtistName = artist.Name;
        responseDto.ImageUrl = ImageUrlHelper.Resolve(request, responseDto.ImageUrl);

        return responseDto;
    }

    public async Task<ArtifactResponseDto> UpdateArtifactAsync(int id, UpdateArtifactDto dto, HttpRequest request)
    {
        var artifact = await _artifactRepository.GetByIdAsync(id);
        if (artifact == null)
            throw new NotFoundException("Artifact not found.");

        var artist = await _artistRepository.GetByIdAsync(dto.ArtistId);
        if (artist == null)
            throw new ValidationException("Invalid ArtistId");

        artifact.Title = dto.Title;
        artifact.Description = dto.Description;
        artifact.ArtType = dto.ArtType;
        artifact.YearCreated = dto.YearCreated;
        artifact.ImageUrl = dto.ImageUrl;
        artifact.Price = dto.Price;
        artifact.IsAvailableForPurchase = dto.IsAvailableForPurchase;
        artifact.StockQuantity = dto.StockQuantity;
        artifact.ArtistId = dto.ArtistId;

        _artifactRepository.Update(artifact);
        await _artifactRepository.SaveChangesAsync();

        var responseDto = _mapper.Map<ArtifactResponseDto>(artifact);
        responseDto.ArtistName = artist.Name;
        responseDto.ImageUrl = ImageUrlHelper.Resolve(request, responseDto.ImageUrl);

        return responseDto;
    }

    public async Task<object> UpdateSaleStatusAsync(int id, SaleStatusDto dto)
    {
        var artifact = await _artifactRepository.GetByIdAsync(id);
        if (artifact == null)
            throw new NotFoundException("Artifact not found.");

        artifact.IsAvailableForPurchase = dto.IsAvailableForPurchase;
        artifact.Price = dto.Price;
        artifact.StockQuantity = dto.StockQuantity;

        _artifactRepository.Update(artifact);
        await _artifactRepository.SaveChangesAsync();

        return new
        {
            id = artifact.Id,
            isAvailableForPurchase = artifact.IsAvailableForPurchase,
            price = artifact.Price,
            stockQuantity = artifact.StockQuantity,
            message = $"'{artifact.Title}' is now {(dto.IsAvailableForPurchase ? "for sale at $" + dto.Price : "not for sale")} with stock {artifact.StockQuantity}."
        };
    }

    public async Task DeleteArtifactAsync(int id)
    {
        var artifact = await _artifactRepository.GetByIdAsync(id);
        if (artifact == null)
            throw new NotFoundException("Artifact not found.");

        _artifactRepository.Remove(artifact);
        await _artifactRepository.SaveChangesAsync();
    }

    public async Task<CommentResponseDto> AddCommentAsync(CreateCommentDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        var artifact = await _artifactRepository.GetByIdAsync(dto.ArtifactId);
        if (user == null || artifact == null)
            throw new ValidationException("Invalid UserId or ArtifactId");

        var comment = new Comment
        {
            Content = dto.Content,
            UserId = dto.UserId,
            ArtifactId = dto.ArtifactId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var commentResponse = _mapper.Map<CommentResponseDto>(comment);
        commentResponse.Username = user.Username;
        return commentResponse;
    }

    public async Task<Tag> AddTagAsync(Tag tag)
    {
        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync();
        return tag;
    }
}
