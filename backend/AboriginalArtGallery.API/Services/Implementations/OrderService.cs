using System.Text.RegularExpressions;
using AutoMapper;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Exceptions;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IArtifactRepository _artifactRepository;
    private readonly IPromoCodeRepository _promoCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        IArtifactRepository artifactRepository,
        IPromoCodeRepository promoCodeRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _artifactRepository = artifactRepository;
        _promoCodeRepository = promoCodeRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedException("User not found.");

        var order = new Order
        {
            UserId = userId,
            BillingFirstName = dto.BillingFirstName,
            BillingLastName = dto.BillingLastName,
            BillingAddressLine1 = dto.BillingAddressLine1,
            BillingAddressLine2 = dto.BillingAddressLine2,
            BillingCity = dto.BillingCity,
            BillingState = dto.BillingState,
            BillingPostCode = dto.BillingPostCode,
            BillingCountry = dto.BillingCountry,
            BillingPhone = dto.BillingPhone,

            SameAsDelivery = dto.SameAsDelivery,
            DeliveryFirstName = dto.SameAsDelivery ? dto.BillingFirstName : dto.DeliveryFirstName,
            DeliveryLastName = dto.SameAsDelivery ? dto.BillingLastName : dto.DeliveryLastName,
            DeliveryAddressLine1 = dto.SameAsDelivery ? dto.BillingAddressLine1 : dto.DeliveryAddressLine1,
            DeliveryAddressLine2 = dto.SameAsDelivery ? dto.BillingAddressLine2 : dto.DeliveryAddressLine2,
            DeliveryCity = dto.SameAsDelivery ? dto.BillingCity : dto.DeliveryCity,
            DeliveryState = dto.SameAsDelivery ? dto.BillingState : dto.DeliveryState,
            DeliveryPostCode = dto.SameAsDelivery ? dto.BillingPostCode : dto.DeliveryPostCode,
            DeliveryCountry = dto.SameAsDelivery ? dto.BillingCountry : dto.DeliveryCountry,

            PreferredDeliveryDate = dto.PreferredDeliveryDate.ToUniversalTime(),
            DeliveryTimeSlot = dto.DeliveryTimeSlot,

            IsGift = dto.IsGift,
            GiftMessage = dto.GiftMessage,
            GiftWrapStyle = dto.GiftWrapStyle,

            CouponCode = dto.CouponCode,
            RedeemCode = dto.RedeemCode,

            PaymentMethod = dto.PaymentMethod,
            CardLastFour = dto.CardLastFour,
            CardBrand = dto.CardBrand
        };

        decimal subtotal = 0;

        foreach (var itemDto in dto.Items)
        {
            var artifact = await _artifactRepository.GetByIdAsync(itemDto.ArtifactId);
            if (artifact == null || !artifact.IsAvailableForPurchase)
                throw new ValidationException($"Artifact {itemDto.ArtifactId} is not available for purchase.");

            if (artifact.StockQuantity < itemDto.Quantity)
                throw new ValidationException($"Not enough stock for '{artifact.Title}'. Only {artifact.StockQuantity} remaining.");

            artifact.StockQuantity -= itemDto.Quantity;
            if (artifact.StockQuantity <= 0)
            {
                artifact.StockQuantity = 0;
            }

            _artifactRepository.Update(artifact);

            var lineTotal = artifact.Price * itemDto.Quantity;
            subtotal += lineTotal;

            order.OrderItems.Add(new OrderItem
            {
                ArtifactId = artifact.Id,
                ArtifactTitle = artifact.Title,
                UnitPrice = artifact.Price,
                Quantity = itemDto.Quantity,
                LineTotal = lineTotal
            });
        }

        order.SubTotal = subtotal;
        decimal discountAmount = 0;

        if (!string.IsNullOrEmpty(dto.CouponCode))
        {
            var promo = await _promoCodeRepository.GetByCodeAsync(dto.CouponCode);
            if (promo != null && promo.IsActive && (!promo.ExpiresAt.HasValue || promo.ExpiresAt > DateTime.UtcNow) && promo.UsedCount < promo.MaxUses)
            {
                if (promo.Type == "percentage")
                {
                    discountAmount += subtotal * (promo.Value / 100);
                }
                else if (promo.Type == "fixed")
                {
                    discountAmount += promo.Value;
                }
                order.DiscountType = promo.Type;
                promo.UsedCount++;
                _promoCodeRepository.Update(promo);
            }
        }

        if (!string.IsNullOrEmpty(dto.RedeemCode))
        {
            if (Regex.IsMatch(dto.RedeemCode, @"^GC-[A-Z0-9]{8}$"))
            {
                discountAmount += 50;
            }
            else
            {
                throw new ValidationException("Invalid gift card format. Expected GC-XXXXXXXX");
            }
        }

        if (discountAmount > subtotal) discountAmount = subtotal;
        order.DiscountAmount = discountAmount;

        var discountedSubtotal = subtotal - discountAmount;

        order.ShippingCost = discountedSubtotal > 500 ? 0 : 25;
        order.GiftWrapFee = dto.IsGift ? (dto.GiftWrapStyle == "luxury" ? 30 : dto.GiftWrapStyle == "premium" ? 15 : 5) : 0;
        order.TaxAmount = discountedSubtotal * 0.10m;
        order.TotalAmount = discountedSubtotal + order.ShippingCost + order.GiftWrapFee + order.TaxAmount;

        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        var fullOrder = await _orderRepository.GetOrderWithItemsAsync(order.Id);
        if (fullOrder != null)
        {
            _ = _emailService.SendOrderConfirmationAsync(user.Email, user.Username, fullOrder);
        }

        var res = _mapper.Map<OrderResponseDto>(fullOrder ?? order);
        res.Username = user.Username;
        return res;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await _orderRepository.GetMyOrdersWithItemsAsync(userId);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> GetOrderByIdAsync(int id, int userId, string role)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(id);
        if (order == null)
            throw new NotFoundException("Order not found.");

        if (role != "Admin" && order.UserId != userId)
            throw new UnauthorizedException("Access denied to this order.");

        var user = await _userRepository.GetByIdAsync(order.UserId);
        var res = _mapper.Map<OrderResponseDto>(order);
        res.Username = user?.Username;
        return res;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public async Task<OrderResponseDto> UpdateOrderStatusAsync(int id, string status)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(id);
        if (order == null)
            throw new NotFoundException("Order not found.");

        order.Status = status;
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync();

        var user = await _userRepository.GetByIdAsync(order.UserId);
        var res = _mapper.Map<OrderResponseDto>(order);
        res.Username = user?.Username;
        return res;
    }

    public async Task<object> ValidatePromoCodeAsync(string code)
    {
        var promo = await _promoCodeRepository.GetByCodeAsync(code);
        if (promo == null || !promo.IsActive || (promo.ExpiresAt.HasValue && promo.ExpiresAt < DateTime.UtcNow) || promo.UsedCount >= promo.MaxUses)
        {
            throw new ValidationException("Invalid or expired promo code.");
        }

        return new
        {
            code = promo.Code,
            type = promo.Type,
            value = promo.Value
        };
    }

    public async Task<IEnumerable<PromoCode>> GetPromoCodesAsync()
    {
        return await _promoCodeRepository.GetAllAsync(1, 1000);
    }

    public async Task<PromoCode> CreatePromoCodeAsync(PromoCode promoCode)
    {
        if (await _promoCodeRepository.ExistsByCodeAsync(promoCode.Code))
            throw new ValidationException("Promo code already exists.");

        await _promoCodeRepository.AddAsync(promoCode);
        await _promoCodeRepository.SaveChangesAsync();
        return promoCode;
    }
}
