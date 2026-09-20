using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Session;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class SessionService(
        ISessionRepo sessionRepo,
        ITrainerRepo trainerRepo,
        IGenericRepo<Category> categoryRepo) : ISessionService
    {
        private readonly ISessionRepo _sessionRepo = sessionRepo;
        private readonly ITrainerRepo _trainerRepo = trainerRepo;
        private readonly IGenericRepo<Category> _categoryRepo = categoryRepo;

        public async Task<IEnumerable<SessionViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var sessions = await _sessionRepo.GetAllAsync(
                include: query => query.Include(s => s.Trainer)
                                       .Include(s => s.Category)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            return sessions.Select(s => new SessionViewModel
            {
                Id = s.ID,
                Description = s.Description,
                Capacity = s.Capacity,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                TrainerId = s.TrainerId,
                TrainerName = s.Trainer != null ? s.Trainer.Name : "N/A",
                CategoryId = s.CategoryId,
                CategoryName = s.Category != null ? s.Category.Name : "N/A",
                BookedSlots = s.Bookings?.Count ?? 0
            });
        }

        public async Task<Result> CreateAsync(CreateSessionViewModel viewModel, CancellationToken ct = default)
        {
            if (viewModel.EndDate <= viewModel.StartDate)
            {
                return Result.Failure("End date must be after start date.", nameof(CreateSessionViewModel.EndDate));
            }

            if (viewModel.StartDate < DateTime.Now.AddMinutes(-5))
            {
                return Result.Failure("Start date cannot be in the past.", nameof(CreateSessionViewModel.StartDate));
            }

            var trainerExists = await _trainerRepo.ExistAsync(t => t.ID == viewModel.TrainerId, ct);
            if (!trainerExists)
            {
                return Result.Failure("Selected trainer does not exist.", nameof(CreateSessionViewModel.TrainerId));
            }

            var categoryExists = await _categoryRepo.ExistAsync(c => c.ID == viewModel.CategoryId, ct);
            if (!categoryExists)
            {
                return Result.Failure("Selected category does not exist.", nameof(CreateSessionViewModel.CategoryId));
            }

            if (await _sessionRepo.HasTrainerConflictAsync(viewModel.TrainerId, viewModel.StartDate, viewModel.EndDate, ct: ct))
            {
                return Result.Failure("Trainer already has another session scheduled during this time period.", nameof(CreateSessionViewModel.TrainerId));
            }

            var session = new Session
            {
                Description = viewModel.Description.Trim(),
                Capacity = viewModel.Capacity,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                TrainerId = viewModel.TrainerId,
                CategoryId = viewModel.CategoryId
            };

            await _sessionRepo.AddAsync(session, ct);
            var rowsAffected = await _sessionRepo.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to add session.");
            }

            return Result.Success();
        }

        public async Task<SessionDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var session = await _sessionRepo.GetByIdAsync(
                id: id,
                include: query => query.Include(s => s.Trainer)
                                       .Include(s => s.Category)
                                       .Include(s => s.Bookings),
                cancellationToken: ct);

            if (session == null)
            {
                return null;
            }

            var bookedCount = session.Bookings?.Count ?? 0;
            var status = DateTime.UtcNow < session.StartDate
                ? "Upcoming"
                : (DateTime.UtcNow <= session.EndDate ? "Ongoing" : "Completed");

            return new SessionDetailsViewModel
            {
                Id = session.ID,
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate.ToString("yyyy-MM-dd HH:mm"),
                EndDate = session.EndDate.ToString("yyyy-MM-dd HH:mm"),
                TrainerId = session.TrainerId,
                TrainerName = session.Trainer != null ? session.Trainer.Name : "N/A",
                CategoryId = session.CategoryId,
                CategoryName = session.Category != null ? session.Category.Name : "N/A",
                BookedSlots = bookedCount,
                AvailableSlots = Math.Max(0, session.Capacity - bookedCount),
                Status = status,
                CreatedAt = session.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            };
        }

        public async Task<EditSessionViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var session = await _sessionRepo.GetByIdAsync(id: id, cancellationToken: ct);
            if (session == null)
            {
                return null;
            }

            return new EditSessionViewModel
            {
                Id = session.ID,
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                TrainerId = session.TrainerId,
                CategoryId = session.CategoryId
            };
        }

        public async Task<Result> UpdateAsync(EditSessionViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var session = await _sessionRepo.GetByIdAsync(id: viewModel.Id, cancellationToken: cancellationToken);
            if (session == null)
            {
                return Result.Failure("Session not found.", nameof(viewModel.Id));
            }

            if (viewModel.EndDate <= viewModel.StartDate)
            {
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
                return Result.Failure("No changes were made.");
            }

            if (isTrainerChanged)
            {
                var trainerExists = await _trainerRepo.ExistAsync(t => t.ID == viewModel.TrainerId, cancellationToken);
                if (!trainerExists)
                {
                    return Result.Failure("Selected trainer does not exist.", nameof(EditSessionViewModel.TrainerId));
                }
            }

            if (isCategoryChanged)
            {
                var categoryExists = await _categoryRepo.ExistAsync(c => c.ID == viewModel.CategoryId, cancellationToken);
                if (!categoryExists)
                {
                    return Result.Failure("Selected category does not exist.", nameof(EditSessionViewModel.CategoryId));
                }
            }

            if (isTrainerChanged || isStartDateChanged || isEndDateChanged)
            {
                if (await _sessionRepo.HasTrainerConflictAsync(viewModel.TrainerId, viewModel.StartDate, viewModel.EndDate, excludeSessionId: viewModel.Id, ct: cancellationToken))
                {
                    return Result.Failure("Trainer already has another session scheduled during this time period.", nameof(EditSessionViewModel.TrainerId));
                }
            }

            session.Description = normalizedDescription;
            session.Capacity = viewModel.Capacity;
            session.StartDate = viewModel.StartDate;
            session.EndDate = viewModel.EndDate;
            session.TrainerId = viewModel.TrainerId;
            session.CategoryId = viewModel.CategoryId;

            _sessionRepo.Update(session);
            await _sessionRepo.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _sessionRepo.GetByIdAsync(
                id: id,
                include: query => query.Include(s => s.Bookings),
                cancellationToken: cancellationToken);

            if (session == null)
            {
                return Result.Failure("Session not found.", nameof(id));
            }

            bool hasActiveBookings = session.EndDate >= DateTime.UtcNow && session.Bookings.Any();
            if (hasActiveBookings)
            {
                return Result.Failure("Cannot delete a session that has active bookings.");
            }

            await _sessionRepo.SoftDeleteAsync(session, cancellationToken);
            await _sessionRepo.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<IEnumerable<SessionLookupViewModel>> GetTrainersLookupAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepo.GetAllAsync(cancellationToken: ct);
            return trainers.Select(t => new SessionLookupViewModel
            {
                Id = t.ID,
                Name = t.Name
            });
        }

        public async Task<IEnumerable<SessionLookupViewModel>> GetCategoriesLookupAsync(CancellationToken ct = default)
        {
            var categories = await _categoryRepo.GetAllAsync(cancellationToken: ct);
            return categories.Select(c => new SessionLookupViewModel
            {
                Id = c.ID,
                Name = c.Name
            });
        }
    }
}
