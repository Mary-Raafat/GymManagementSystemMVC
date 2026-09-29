using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.ViewModels.Session;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class SessionService(IUnitOfWork unitOfWork, ILogger<SessionService>
        logger) : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<SessionService> _logger = logger;

        public async Task<IEnumerable<SessionViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.Sessions.GetAllAsync(
                include: query => query.Include(s => s.Trainer)
                                       .Include(s => s.Category)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            return sessions.Select(s => s.ToViewModel());
        }

        public async Task<Result> CreateAsync(CreateSessionViewModel viewModel, CancellationToken ct = default)
        {
            if (viewModel.EndDate <= viewModel.StartDate)
            {
                _logger.LogWarning("Session creation failed: End date ({EndDate}) must be after start date ({StartDate}).", viewModel.EndDate, viewModel.StartDate);
                return Result.Failure("End date must be after start date.", nameof(CreateSessionViewModel.EndDate));
            }

            if (viewModel.StartDate < DateTime.Now.AddMinutes(-5))
            {
                _logger.LogWarning("Session creation failed: Start date ({StartDate}) cannot be in the past.", viewModel.StartDate);
                return Result.Failure("Start date cannot be in the past.", nameof(CreateSessionViewModel.StartDate));
            }

            var trainerExists = await _unitOfWork.Trainers.ExistAsync(t => t.ID == viewModel.TrainerId, ct);
            if (!trainerExists)
            {
                _logger.LogWarning("Session creation failed: Selected Trainer ID {TrainerId} does not exist.", viewModel.TrainerId);
                return Result.Failure("Selected trainer does not exist.", nameof(CreateSessionViewModel.TrainerId));
            }

            var categoryExists = await _unitOfWork.Categories.ExistAsync(c => c.ID == viewModel.CategoryId, ct);
            if (!categoryExists)
            {
                _logger.LogWarning("Session creation failed: Selected Category ID {CategoryId} does not exist.", viewModel.CategoryId);
                return Result.Failure("Selected category does not exist.", nameof(CreateSessionViewModel.CategoryId));
            }

            if (await _unitOfWork.Sessions.HasTrainerConflictAsync(viewModel.TrainerId, viewModel.StartDate, viewModel.EndDate, ct: ct))
            {
                _logger.LogWarning("Session creation failed: Trainer ID {TrainerId} has a schedule conflict between {StartDate} and {EndDate}.", viewModel.TrainerId, viewModel.StartDate, viewModel.EndDate);
                return Result.Failure("Trainer already has another session scheduled during this time period.", nameof(CreateSessionViewModel.TrainerId));
            }

            var session = viewModel.ToEntity();

            await _unitOfWork.Sessions.AddAsync(session, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist session in the database.");
                return Result.Failure("Failed to add session.");
            }

            _logger.LogInformation("Session created successfully with ID: {SessionId}.", session.ID);
            return Result.Success();
        }

        public async Task<SessionDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync( 
                id: id,
                include: query => query.Include(s => s.Trainer)
                                       .Include(s => s.Category)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            if (session == null)
            {
                _logger.LogWarning("Session with ID {SessionId} not found.", id);
                return null;
            }

            return session.ToDetailsViewModel();
        }

        public async Task<EditSessionViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id: id, cancellationToken: ct);
            if (session == null)
            {
                _logger.LogWarning("Session with ID {SessionId} not found for editing.", id);
                return null;
            }

            return session.ToEditViewModel();
        }

        public async Task<Result> UpdateAsync(EditSessionViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id: viewModel.Id, cancellationToken: cancellationToken);
            if (session == null)
            {
                _logger.LogWarning("Session update failed: Session ID {SessionId} not found.", viewModel.Id);
                return Result.Failure("Session not found.", nameof(viewModel.Id));
            }

            if (viewModel.EndDate <= viewModel.StartDate)
            {
                _logger.LogWarning("Session update failed: End date ({EndDate}) must be after start date ({StartDate}).", viewModel.EndDate, viewModel.StartDate);
                return Result.Failure("End date must be after start date.", nameof(EditSessionViewModel.EndDate));
            }

            var normalizedDescription = viewModel.Description.Trim();
            bool isDescriptionChanged = session.Description != normalizedDescription;
            bool isCapacityChanged = session.Capacity != viewModel.Capacity;
            bool isStartDateChanged = session.StartDate != viewModel.StartDate;
            bool isEndDateChanged = session.EndDate != viewModel.EndDate;
            bool isTrainerChanged = session.TrainerId != viewModel.TrainerId;
            bool isCategoryChanged = session.CategoryId != viewModel.CategoryId;

            if (!isDescriptionChanged && !isCapacityChanged && !isStartDateChanged && !isEndDateChanged && !isTrainerChanged && !isCategoryChanged)
            {
                _logger.LogInformation("No changes detected for session ID: {SessionId}", viewModel.Id);
                return Result.Failure("No changes were made.");
            }

            if (isTrainerChanged)
            {
                var trainerExists = await _unitOfWork.Trainers.ExistAsync(t => t.ID == viewModel.TrainerId, cancellationToken);
                if (!trainerExists)
                {
                    _logger.LogWarning("Session update failed: Trainer ID {TrainerId} does not exist.", viewModel.TrainerId);
                    return Result.Failure("Selected trainer does not exist.", nameof(EditSessionViewModel.TrainerId));
                }
            }

            if (isCategoryChanged)
            {
                var categoryExists = await _unitOfWork.Categories.ExistAsync(c => c.ID == viewModel.CategoryId, cancellationToken);
                if (!categoryExists)
                {
                    _logger.LogWarning("Session update failed: Category ID {CategoryId} does not exist.", viewModel.CategoryId);
                    return Result.Failure("Selected category does not exist.", nameof(EditSessionViewModel.CategoryId));
                }
            }

            if (isTrainerChanged || isStartDateChanged || isEndDateChanged)
            {
                if (await _unitOfWork.Sessions.HasTrainerConflictAsync(viewModel.TrainerId, viewModel.StartDate, viewModel.EndDate, excludeSessionId: viewModel.Id, ct: cancellationToken))
                {
                    _logger.LogWarning("Session update failed: Trainer ID {TrainerId} has a schedule conflict between {StartDate} and {EndDate}.", viewModel.TrainerId, viewModel.StartDate, viewModel.EndDate);
                    return Result.Failure("Trainer already has another session scheduled during this time period.", nameof(EditSessionViewModel.TrainerId));
                }
            }

            session.Description = normalizedDescription;
            session.Capacity = viewModel.Capacity;
            session.StartDate = viewModel.StartDate;
            session.EndDate = viewModel.EndDate;
            session.TrainerId = viewModel.TrainerId;
            session.CategoryId = viewModel.CategoryId;

            _unitOfWork.Sessions.Update(session);
            var rowsAffected = await _unitOfWork.CompleteAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist updates for session ID: {SessionId}", viewModel.Id);
                return Result.Failure("Failed to update session.");
            }

            _logger.LogInformation("Session ID: {SessionId} updated successfully.", viewModel.Id);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(
                id: id,
                include: query => query.Include(s => s.Bookings),
                cancellationToken: cancellationToken);

            if (session == null)
            {
                _logger.LogWarning("Session deletion failed: Session ID {SessionId} not found.", id);
                return Result.Failure("Session not found.", nameof(id));
            }

            bool hasActiveBookings = session.EndDate >= DateTime.UtcNow && session.Bookings.Any();
            if (hasActiveBookings)
            {
                _logger.LogWarning("Session deletion rejected: Session ID {SessionId} has active bookings.", id);
                return Result.Failure("Cannot delete a session that has active bookings.");
            }

            await _unitOfWork.Sessions.SoftDeleteAsync(session, cancellationToken);
            var rowsAffected = await _unitOfWork.CompleteAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to soft delete session ID: {SessionId}", id);
                return Result.Failure("Failed to delete session.");
            }

            _logger.LogInformation("Session ID: {SessionId} soft-deleted successfully.", id);
            return Result.Success();
        }

        public async Task<IEnumerable<SessionLookupViewModel>> GetTrainersLookupAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.Trainers.GetAllAsync(cancellationToken: ct);
            return trainers.Select(t => new SessionLookupViewModel
            {
                Id = t.ID,
                Name = t.Name
            });
        }

        public async Task<IEnumerable<SessionLookupViewModel>> GetCategoriesLookupAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken: ct);
            return categories.Select(c => new SessionLookupViewModel
            {
                Id = c.ID,
                Name = c.Name
            });
        }
    }
}
