using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using FpisNovoAPI.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Services
{
    public class PromocodeService : IPromocodeService
    {
        private readonly IPromocodeRepository _promocodeRepository;
        private readonly ILogger<PromocodeService> _logger;

        public PromocodeService(IPromocodeRepository promocodeRepository, ILogger<PromocodeService> logger)
        {
            _promocodeRepository = promocodeRepository;
            _logger = logger;
        }

        public PromoCodeValidationResult ValidatePromoCode(int? promoCodeId)
        {
            try
            {
                var promocode = _promocodeRepository.GetPromocode(promoCodeId);
                if (promocode != null)
                {
                    if (!promocode.IsUsed)
                    {
                        return new PromoCodeValidationResult { IsValid = true };
                    }
                    else
                    {
                        return new PromoCodeValidationResult { IsValid = false, ErrorMessage = "Promo code has already been used." };
                    }
                }
                else
                {
                    return new PromoCodeValidationResult { IsValid = false, ErrorMessage = "Invalid promo code." };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return new PromoCodeValidationResult { IsValid = false, ErrorMessage = "An error occurred while checking the promo code." };
            }
        }
       

        public string GeneratePromoCode(Reservation reservation)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public bool MarkPromoCodeAsUsed(int promocodeId, int reservationId)
        {
            try
            {
                var promocode = _promocodeRepository.GetPromocode(promocodeId);
                if (promocode != null)
                {
                    promocode.IsUsed = true;
                    promocode.UsedReservationId = reservationId;
                    return _promocodeRepository.UpdatePromocode(promocode);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                throw ex;
            }
        }

        public int CreatePromoCode(string promoCode, Reservation reservation)
        {
            try
            {
                var promocode = new Promocode
                {
                    Code = promoCode,
                    IsUsed = false,
                    ReservationId = reservation.ReservationId
                };

                if (!_promocodeRepository.CreatePromocode(promocode))
                {
                    return -1;
                }

                return promocode.PromocodeId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return -1;
            }
        }
    }
}
