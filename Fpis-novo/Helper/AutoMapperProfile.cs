using AutoMapper;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Models;
using FpisNovoAPI.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FpisNovoAPI.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            
            CreateMap<Concert, ConcertDto>();
            CreateMap<ConcertDto, Concert>();
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();
            CreateMap<Promocode, PromocodeDto>();
            CreateMap<PromocodeDto, Promocode>();
            CreateMap<Reservation, ReservationDto>();
            CreateMap<ReservationDto, Reservation>();
            CreateMap<ReservationTicket, ReservationTicketDto>();
            CreateMap<ReservationTicketDto, ReservationTicket>();
            CreateMap<Zone, ZoneDto>();
            CreateMap<ZoneDto, Zone>();
            CreateMap<AvailableSeats, AvailableSeatsDto>();
            CreateMap<AvailableSeatsDto, AvailableSeats>();
            CreateMap<CreateReservationRequest, Reservation>();
        }
    }
}

