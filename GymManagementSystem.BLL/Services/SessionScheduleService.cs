using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.BLL.ViewModels.SessionSchedule;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class SessionScheduleService(
        ISessionRepo sessionRepo,
        IBookingRepo bookingRepo,
        IMemberRepository memberRepo,
        IMembershipRepo membershipRepo) : ISessionScheduleService
    {
        private readonly ISessionRepo _sessionRepo = sessionRepo;
        private readonly IBookingRepo _bookingRepo = bookingRepo;
        private readonly IMemberRepository _memberRepo = memberRepo;
        private readonly IMembershipRepo _membershipRepo = membershipRepo;

        public async Task<IEnumerable<SessionScheduleViewModel>> GetUpcomingScheduleAsync(CancellationToken ct = default)
        {
            var sessions = await _sessionRepo.GetAllAsync(
                include: query => query.Include(s => s.Category)
                                       .Include(s => s.Trainer)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            return sessions
                .OrderBy(s => s.StartDate)
                .Select(s => new SessionScheduleViewModel
                {
                    SessionId = s.ID,
                    Description = s.Description,
                    CategoryName = s.Category?.Name ?? "N/A",
                    TrainerName = s.Trainer?.Name ?? "N/A",
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Capacity = s.Capacity,
                    BookedCount = s.Bookings?.Count ?? 0
                });
        }

        public async Task<BookSessionViewModel?> GetForBookingAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _sessionRepo.GetByIdAsync(
                id: sessionId,
                include: query => query.Include(s => s.Category)
                                       .Include(s => s.Trainer)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            if (session == null)
            {
                return null;
            }

            var bookedCount = session.Bookings?.Count ?? 0;

            return new BookSessionViewModel
            {
                SessionId = session.ID,
                CategoryName = session.Category?.Name ?? "N/A",
                TrainerName = session.Trainer?.Name ?? "N/A",
                DateDisplay = session.StartDate.ToString("dd MMM yyyy"),
                TimeRangeDisplay = $"{session.StartDate:hh:mm tt} - {session.EndDate:hh:mm tt}",
                AvailableSlots = Math.Max(0, session.Capacity - bookedCount)
            };
        }

        public async Task<Result> BookMemberAsync(BookSessionViewModel viewModel, CancellationToken ct = default)
        {
            var session = await _sessionRepo.GetByIdAsync(
                id: viewModel.SessionId,
                include: query => query.Include(s => s.Bookings),
                cancellationToken: ct);

            if (session == null)
            {
                return Result.Failure("Session not found.", nameof(viewModel.SessionId));
            }

            if (session.EndDate <= DateTime.UtcNow)
            {
                return Result.Failure("Cannot book into a completed session.");
            }

            if (session.Bookings.Count >= session.Capacity)
            {
                return Result.Failure("This session has reached maximum capacity.");
            }

            var member = await _memberRepo.GetByIdAsync(viewModel.MemberId, cancellationToken: ct);
            if (member == null)
            {
                return Result.Failure("Member not found.", nameof(viewModel.MemberId));
            }

            if (await _bookingRepo.IsMemberBookedInSessionAsync(viewModel.MemberId, viewModel.SessionId, ct))
            {
                return Result.Failure("Member is already booked in this session.", nameof(viewModel.MemberId));
            }

            if (await _bookingRepo.HasMemberConflictBookingAsync(viewModel.MemberId, session.StartDate, session.EndDate, ct))
            {
                return Result.Failure("Member already has another booking during this time.", nameof(viewModel.MemberId));
            }

            var booking = new Booking
            {
                SessionId = viewModel.SessionId,
                MemberId = viewModel.MemberId,
                Date = DateTime.UtcNow,
                IsAttended = false
            };

            await _bookingRepo.AddAsync(booking, ct);
            var rowsAffected = await _bookingRepo.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to complete booking.");
            }

            return Result.Success();
        }

        public async Task<SessionAttendanceViewModel?> GetSessionBookingsAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _sessionRepo.GetByIdAsync(
                id: sessionId,
                include: query => query.Include(s => s.Category)
                                       .Include(s => s.Trainer)
                                       .Include(s => s.Bookings).ThenInclude(b => b.Member),
                cancellationToken: ct);

            if (session == null)
            {
                return null;
            }

            return new SessionAttendanceViewModel
            {
                SessionId = session.ID,
                CategoryName = session.Category?.Name ?? "N/A",
                TrainerName = session.Trainer?.Name ?? "N/A",
                DateDisplay = session.StartDate.ToString("dd MMM yyyy"),
                TimeRangeDisplay = $"{session.StartDate:hh:mm tt} - {session.EndDate:hh:mm tt}",
                Capacity = session.Capacity,
                Bookings = session.Bookings.Select(b => new BookingItemViewModel
                {
                    BookingId = b.ID,
                    MemberId = b.MemberId,
                    MemberName = b.Member?.Name ?? "N/A",
                    MemberPhone = b.Member?.Phone ?? "N/A",
                    BookingDate = b.Date,
                    IsAttended = b.IsAttended
                }).ToList()
            };
        }

        public async Task<Result> ToggleAttendanceAsync(int bookingId, CancellationToken ct = default)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId, cancellationToken: ct);
            if (booking == null)
            {
                return Result.Failure("Booking not found.", nameof(bookingId));
            }

            booking.IsAttended = !booking.IsAttended;
            _bookingRepo.Update(booking);
            var rowsAffected = await _bookingRepo.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to update attendance.");
            }

            return Result.Success();
        }

        public async Task<Result> CancelBookingAsync(int bookingId, CancellationToken ct = default)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId, cancellationToken: ct);
            if (booking == null)
            {
                return Result.Failure("Booking not found.", nameof(bookingId));
            }

            await _bookingRepo.SoftDeleteAsync(booking, ct);
            var rowsAffected = await _bookingRepo.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to cancel booking.");
            }

            return Result.Success();
        }

        public async Task<IEnumerable<MembershipLookupViewModel>> GetEligibleMembersLookupAsync(int sessionId, CancellationToken ct = default)
        {
            var allMembers = await _memberRepo.GetAllAsync(cancellationToken: ct);
            var session = await _sessionRepo.GetByIdAsync(
                id: sessionId,
                include: query => query.Include(s => s.Bookings),
                cancellationToken: ct);

            var bookedMemberIds = session?.Bookings.Select(b => b.MemberId).ToHashSet() ?? [];

            return allMembers
                .Where(m => !bookedMemberIds.Contains(m.ID))
                .Select(m => new MembershipLookupViewModel
                {
                    Id = m.ID,
                    Name = $"{m.Name} ({m.Phone})"
                });
        }
    }
}
