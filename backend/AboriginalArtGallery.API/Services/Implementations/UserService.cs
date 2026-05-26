using AutoMapper;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Exceptions;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync(1, 1000);
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var validRoles = new[] { "Admin", "User" };
        if (!validRoles.Contains(dto.Role))
            throw new ValidationException("Invalid role. Allowed values: \"Admin\" or \"User\"");

        if (await _userRepository.ExistsByEmailAsync(dto.Email))
            throw new ValidationException($"A user with email '{dto.Email}' already exists.");

        if (await _userRepository.ExistsByUsernameAsync(dto.Username))
            throw new ValidationException($"Username '{dto.Username}' is already taken.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        var validRoles = new[] { "Admin", "User" };
        if (!validRoles.Contains(dto.Role))
            throw new ValidationException("Invalid role. Allowed values: \"Admin\" or \"User\"");

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User not found.");

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<object> PromoteUserAsync(PromoteUserDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new NotFoundException($"No user found with email '{dto.Email}'");

        var validRoles = new[] { "Admin", "User" };
        if (!validRoles.Contains(dto.Role))
            throw new ValidationException("Invalid role. Allowed values: Admin, User");

        user.Role = dto.Role;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new { message = $"{user.Username} is now a '{dto.Role}'.", userId = user.Id, role = user.Role };
    }
}
