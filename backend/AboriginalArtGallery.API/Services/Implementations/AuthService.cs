using BCrypt.Net;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Exceptions;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthService(
        IUserRepository userRepository,
        IOtpCodeRepository otpCodeRepository,
        IEmailService emailService,
        ITokenService tokenService,
        IConfiguration config)
    {
        _userRepository = userRepository;
        _otpCodeRepository = otpCodeRepository;
        _emailService = emailService;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<string> SendOtpAsync(SendOtpDto dto)
    {
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
            throw new ValidationException("An account with this email already exists.");

        if (await _userRepository.ExistsByUsernameAsync(dto.Username))
            throw new ValidationException("This username is already taken.");

        await _otpCodeRepository.InvalidatePreviousOtpsAsync(dto.Email);

        var code = new Random().Next(100000, 999999).ToString();
        await _otpCodeRepository.AddAsync(new OtpCode
        {
            Email = dto.Email,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        });

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        await _otpCodeRepository.AddAsync(new OtpCode
        {
            Email = $"__payload__{dto.Email}",
            Code = $"{dto.Username}||{passwordHash}",
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false
        });

        await _otpCodeRepository.SaveChangesAsync();

        await _emailService.SendOtpEmailAsync(dto.Email, code);

        return "OTP sent to your email. It expires in 10 minutes.";
    }

    public async Task<string> VerifyOtpAsync(VerifyOtpDto dto)
    {
        var otpRecord = await _otpCodeRepository.GetActiveOtpAsync(dto.Email, dto.Otp);
        if (otpRecord == null)
            throw new ValidationException("Invalid OTP code.");

        if (otpRecord.ExpiresAt < DateTime.UtcNow)
        {
            otpRecord.IsUsed = true;
            await _otpCodeRepository.SaveChangesAsync();
            throw new ValidationException("OTP has expired. Please request a new one.");
        }

        var payload = await _otpCodeRepository.GetActivePayloadAsync(dto.Email);
        if (payload == null)
            throw new ValidationException("Registration session expired. Please start again.");

        var parts = payload.Code.Split("||", 2);
        if (parts.Length != 2)
            throw new ValidationException("Invalid registration session.");

        var username = parts[0];
        var passwordHash = parts[1];

        var user = new User
        {
            Username = username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        await _userRepository.AddAsync(user);

        otpRecord.IsUsed = true;
        payload.IsUsed = true;

        await _userRepository.SaveChangesAsync();

        return "Account created successfully! Please log in.";
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid credentials.");

        var token = _tokenService.GenerateToken(user);
        var expiryHours = Convert.ToDouble(_config["JwtSettings:ExpiryHours"] ?? "24");

        return new TokenResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(expiryHours)
        };
    }
}
