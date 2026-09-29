using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.ViewModels.Membership;
using GymManagementSystem.BLL.ViewModels.SessionSchedule;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class SessionScheduleService(
        IUnitOfWork unitOfWork,
        ILogger<SessionScheduleService> logger) : ISessionScheduleService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<SessionScheduleService> _logger = logger;

        public async Task<IEnumerable<SessionScheduleViewModel>> GetUpcomingScheduleAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.Sessions.GetAllAsync(
                include: query => query.Include(s => s.Category)
                                       .Include(s => s.Trainer)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            return sessions
                .OrderBy(s => s.StartDate)
                .Select(s => s.ToScheduleViewModel());
        }

        public async Task<BookSessionViewModel?> GetForBookingAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(
                id: sessionId,
                include: query => query.Include(s => s.Category)
                                       .Include(s => s.Trainer)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            if (session == null)
            {
                _logger.LogWarning("Session ID {SessionId} not found for booking.", sessionId);
                return null;
            }

            return session.ToBookViewModel();
        }

        public async Task<Result> BookMemberAsync(BookSessionViewModel viewModel, CancellationToken ct = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(
                id: viewModel.SessionId,
                include: query => query.Include(s => s.Bookings),
                cancellationToken: ct);

            if (session == null)
            {
                _logger.LogWarning("Booking failed: Session ID {SessionId} not found.", viewModel.SessionId);
                return Result.Failure("Session not found.", nameof(viewModel.SessionId));
            }

            if (session.EndDate <= DateTime.UtcNow)
            {
                _logger.LogWarning("Booking failed: Session ID {SessionId} has already ended.", viewModel.SessionId);
                return Result.Failure("Cannot book into a completed session.");
            }

            if (session.Bookings.Count >= session.Capacity)
            {
                _logger.LogWarning("Booking failed: Session ID {SessionId} has reached maximum capacity of {Capacity}.", viewModel.SessionId, session.Capacity);
                return Result.Failure("This session has reached maximum capacity.");
            }

            var member = await _unitOfWork.Members.GetByIdAsync(viewModel.MemberId, cancellationToken: ct);
            if (member == null)
            {
                _logger.LogWarning("Booking failed: Member ID {MemberId} not found.", viewModel.MemberId);
                return Result.Failure("Member not found.", nameof(viewModel.MemberId));
            }

            if (await _unitOfWork.Bookings.IsMemberBookedInSessionAsync(viewModel.MemberId, viewModel.SessionId, ct))
            {
                _logger.LogWarning("Booking failed: Member ID {MemberId} is already booked in session ID {SessionId}.", viewModel.MemberId, viewModel.SessionId);
                return Result.Failure("Member is already booked in this session.", nameof(viewModel.MemberId));
            }

            if (await _unitOfWork.Bookings .HasMemberConflictBookingAsync(viewModel.MemberId, session.StartDate, session.EndDate, ct))
            {
                _logger.LogWarning("Booking failed: Member ID {MemberId} has a schedule conflict between {StartDate} and {EndDate}.", viewModel.MemberId, session.StartDate, session.EndDate);
                return Result.Failure("Member already has another booking during this time.", nameof(viewModel.MemberId));
            }

            var booking = viewModel.ToBookingEntity();

            await _unitOfWork.Bookings.AddAsync(booking, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist booking for member ID {MemberId} in session ID {SessionId}.", viewModel.MemberId, viewModel.SessionId);
                return Result.Failure("Failed to complete booking.");
            }

            _logger.LogInformation("Member ID {MemberId} successfully booked into session ID {SessionId}.", viewModel.MemberId, viewModel.SessionId);
            return Result.Success();
        }

        public async Task<SessionAttendanceViewModel?> GetSessionBookingsAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(
                id: sessionId,
                include: query => query.Include(s => s.Category)
                                       .Include(s => s.Trainer)
                                       .Include(s => s.Bookings).ThenInclude(b => b.Member),
                cancellationToken: ct);

            if (session == null)
            {
                _logger.LogWarning("Session ID {SessionId} not found for attendance retrieval.", sessionId);
                return null;
            }

            return session.ToAttendanceViewModel();
        }

        public async Task<Result> ToggleAttendanceAsync(int bookingId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId, cancellationToken: ct);
            if (booking == null)
            {
                _logger.LogWarning("Attendance toggle failed: Booking ID {BookingId} not found.", bookingId);
                return Result.Failure("Booking not found.", nameof(bookingId));
            }

            booking.IsAttended = !booking.IsAttended;
            _unitOfWork.Bookings.Update(booking);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to update attendance status for booking ID: {BookingId}", bookingId);
                return Result.Failure("Failed to update attendance.");
            }

            _logger.LogInformation("Attendance status for booking ID: {BookingId} toggled to {IsAttended}.", bookingId, booking.IsAttended);
            return Result.Success();
        }

        public async Task<Result> CancelBookingAsync(int bookingId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId, cancellationToken: ct);
            if (booking == null)
            {
                _logger.LogWarning("Booking cancellation failed: Booking ID {BookingId} not found.", bookingId);
                return Result.Failure("Booking not found.", nameof(bookingId));
            }

            await _unitOfWork.Bookings.SoftDeleteAsync(booking, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to cancel booking ID: {BookingId} in the database.", bookingId);
                return Result.Failure("Failed to cancel booking.");
            }

            _logger.LogInformation("Booking ID: {BookingId} cancelled successfully.", bookingId);
            return Result.Success();
        }

        public async Task<IEnumerable<MembershipLookupViewModel>> GetEligibleMembersLookupAsync(int sessionId, CancellationToken ct = default)
        {
            var allMembers = await _unitOfWork.Members.GetAllAsync(cancellationToken: ct);
            var session = await _unitOfWork.Sessions.GetByIdAsync(
                id: sessionId,
                include: query => query.Include(s => s.Bookings),
                cancellationToken: ct);

            var bookedMemberIds = session?.Bookings.Select(b => b.MemberId).ToHashSet() ?? [];

            return allMembers
                .Where(m => !bookedMemberIds.Contains(m.ID))
                .Select(m => m.ToEligibleMemberLookup());
        }
    }
}
