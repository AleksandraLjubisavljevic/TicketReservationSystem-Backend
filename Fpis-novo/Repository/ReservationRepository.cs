using AutoMapper;
using FpisNovoAPI.Data;
using FpisNovoAPI.Dto;
using FpisNovoAPI.Interfaces;
using FpisNovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;

namespace FpisNovoAPI.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IConcertRepository _concertRepository;
        private readonly ILogger<ReservationRepository> _logger;
        public ReservationRepository(IConcertRepository concertRepository, DataContext dataContext, IMapper mapper, ILogger<ReservationRepository> logger)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _concertRepository = concertRepository;
            _logger = logger;
        }
        public bool CreateReservation(Reservation reservation)
        {
            _dataContext.Reservations.Add(reservation);
            return Save();
        }
        public IDbContextTransaction BeginTransaction()
        {
            return _dataContext.Database.BeginTransaction();
        }
        public ICollection<Reservation> GetReservationsByCustomerId(int customerId)
        {
            return _dataContext.Reservations.Where(r => r.CustomerId == customerId).ToList();
        }
        public bool DeleteReservation(Reservation reservation)
        {
            _dataContext.Reservations.Remove(reservation);
            return Save();
        }

        public Reservation GetReservation(int reservationId)
        {
            return _dataContext.Reservations.Where(c => c.ReservationId == reservationId).FirstOrDefault();
        }
        public Reservation GetReservationByEmailAndToken(string email, string token)
        {
            return _dataContext.Reservations.Where(c =>c.Customer.Email==email && c.Token == token).FirstOrDefault();
        }
        
        public ICollection<Reservation> GetReservations()
        {
            return _dataContext.Reservations.ToList();
        }

        public bool ReservationExists(int reservationId)
        {
            return _dataContext.Reservations.Any(c => c.ReservationId == reservationId);
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
            
        }

        public bool UpdateReservation(Reservation reservation)
        {
            _dataContext.Reservations.Update(reservation);
            return Save();
        }
    }
}
