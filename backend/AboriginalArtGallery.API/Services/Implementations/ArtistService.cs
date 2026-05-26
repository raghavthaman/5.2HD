using AutoMapper;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Exceptions;
using AboriginalArtGallery.API.Helpers;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Services.Implementations;

public class ArtistService : IArtistService
{
    private readonly IArtistRepository _artistRepository;
    private readonly IMapper _mapper;

    public ArtistService(IArtistRepository artistRepository, IMapper mapper)
    {
        _artistRepository = artistRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ArtistResponseDto>> GetAllArtistsAsync()
    {
        var artists = await _artistRepository.GetAllAsync(1, 1000);
        return _mapper.Map<IEnumerable<ArtistResponseDto>>(artists);
    }

    public async Task<ArtistResponseDto> GetArtistByIdAsync(int id, HttpRequest request)
    {
        var artist = await _artistRepository.GetArtistWithArtifactsAsync(id);
        if (artist == null)
            throw new NotFoundException("Artist not found.");

        var dto = _mapper.Map<ArtistResponseDto>(artist);
        foreach (var artifact in dto.Artifacts)
        {
            artifact.ImageUrl = ImageUrlHelper.Resolve(request, artifact.ImageUrl);
        }
        return dto;
    }

    public async Task<Artist> CreateArtistAsync(Artist artist)
    {
        if (string.IsNullOrWhiteSpace(artist.Name))
            throw new ValidationException("Artist name is required.");

        var trimmedName = artist.Name.Trim();
        var existing = await _artistRepository.GetByNameAsync(trimmedName);
        if (existing != null)
            throw new ValidationException("Artist with the same name already exists.");

        artist.Name = trimmedName;
        await _artistRepository.AddAsync(artist);
        await _artistRepository.SaveChangesAsync();
        return artist;
    }

    public async Task<Artist> UpdateArtistAsync(int id, Artist updatedArtist)
    {
        var artist = await _artistRepository.GetByIdAsync(id);
        if (artist == null)
            throw new NotFoundException("Artist not found.");

        if (string.IsNullOrWhiteSpace(updatedArtist.Name))
            throw new ValidationException("Artist name is required.");

        var trimmedName = updatedArtist.Name.Trim();
        var duplicate = await _artistRepository.GetByNameAsync(trimmedName);
        if (duplicate != null && duplicate.Id != id)
            throw new ValidationException("Another artist with the same name already exists.");

        artist.Name = trimmedName;
        artist.Tribe = updatedArtist.Tribe;
        artist.Biography = updatedArtist.Biography;
        artist.BirthYear = updatedArtist.BirthYear;
        artist.Country = updatedArtist.Country;

        _artistRepository.Update(artist);
        await _artistRepository.SaveChangesAsync();
        return artist;
    }

    public async Task DeleteArtistAsync(int id)
    {
        var artist = await _artistRepository.GetByIdAsync(id);
        if (artist == null)
            throw new NotFoundException("Artist not found.");

        _artistRepository.Remove(artist);
        await _artistRepository.SaveChangesAsync();
    }
}
