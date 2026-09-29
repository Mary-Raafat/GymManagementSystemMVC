using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.BLL.ViewModels.Trainer;
using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class TrainerService(IUnitOfWork unitOfWork, ILogger<TrainerService> logger) : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<TrainerService> _logger = logger;


        public async Task<IEnumerable<TrainerViewModel>>  GetAllAsync(CancellationToken ct=default)
        {
            var trainers=await _unitOfWork.Trainers.GetAllAsync(cancellationToken: ct);
            return trainers.Select(t => t.ToViewModel());
        }

        public async Task<Result> CreateAsync(CreateTrainerViewModel viewModel, CancellationToken ct = default)
        {
            var email = viewModel.Email.Trim().ToLowerInvariant();
            var phone = viewModel.Phone.Trim();
            var name = viewModel.Name.Trim();


            if (await _unitOfWork.Trainers.IsEmailTakenAsync(email, ct: ct))
            {
                _logger.LogWarning("Trainer creation failed: Email {Email} already exists.", email);
                return Result.Failure("Email already exists.", nameof(CreateTrainerViewModel.Email));
            }

            if (await _unitOfWork.Trainers.IsPhoneTakenAsync(phone, ct: ct))
            {
                _logger.LogWarning("Trainer creation failed: Phone number {Phone} already exists.", phone);
                return Result.Failure("Phone number already exists.", nameof(CreateTrainerViewModel.Phone));
            }
            var trainer = viewModel.ToEntity(name, email, phone); // ربط ال trainer بال view model
            await _unitOfWork.Trainers.AddAsync(trainer, ct);
            var rowsAffected = await _unitOfWork.CompleteAsync(ct);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist trainer {Email} in the database.", email);
                return Result.Failure("Failed to add trainer.");
            }

            _logger.LogInformation("Trainer created successfully with ID: {TrainerId}", trainer.ID);
            return Result.Success();
        }

        public async Task<TrainerDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(id: id, cancellationToken: ct);
            if (trainer == null)
            {
                _logger.LogWarning("Trainer with ID {TrainerId} not found.", id);
                return null;
            }

            return trainer.ToDetailsViewModel();
        }

        public async Task<EditTrainerViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(id: id, cancellationToken: ct);
            if (trainer == null)
            {
                _logger.LogWarning("Trainer with ID {TrainerId} not found for editing.", id);
                return null;
            }

            return trainer.ToEditViewModel();
        }

        public async Task<Result> UpdateAsync(EditTrainerViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(id: viewModel.Id, cancellationToken: cancellationToken);
            if (trainer == null)
            {
                _logger.LogWarning("Trainer update failed: Trainer ID {TrainerId} not found.", viewModel.Id);
                return Result.Failure("Trainer not found.", nameof(viewModel.Id));
            }

            var normalizedEmail = viewModel.Email.Trim().ToLowerInvariant();
            var normalizedPhone = viewModel.Phone.Trim();
            var normalizedCity = viewModel.City.Trim();
            var normalizedStreet = viewModel.Street.Trim();

            bool isEmailChanged = trainer.Email != normalizedEmail;
            bool isPhoneChanged = trainer.Phone != normalizedPhone;
            bool isSpecializationChanged = trainer.Speciality != viewModel.Specialization;
            bool isAddressChanged = trainer.Address == null ||
                                   trainer.Address.City != normalizedCity ||
                                   trainer.Address.Street != normalizedStreet ||
                                   trainer.Address.BuildingNumber != viewModel.BuildingNumber;

            if (!isEmailChanged && !isPhoneChanged && !isAddressChanged && !isSpecializationChanged)
            {
                _logger.LogInformation("No changes detected for trainer ID: {TrainerId}", viewModel.Id);
                return Result.Failure("No changes were made.");
            }

            if (isEmailChanged && await _unitOfWork.Trainers.IsEmailTakenAsync(normalizedEmail, excludeId: viewModel.Id, ct: cancellationToken))
            {
                _logger.LogWarning("Trainer update failed: Email {Email} already exists.", normalizedEmail);
                return Result.Failure("Email already exists.", nameof(viewModel.Email));
            }

            if (isPhoneChanged && await _unitOfWork.Trainers.IsPhoneTakenAsync(normalizedPhone, excludeId: viewModel.Id, ct: cancellationToken))
            {
                _logger.LogWarning("Trainer update failed: Phone number {Phone} already exists.", normalizedPhone);
                return Result.Failure("Phone number already exists.", nameof(viewModel.Phone));
            }

            trainer.Email = normalizedEmail;
            trainer.Phone = normalizedPhone;
            trainer.Speciality = viewModel.Specialization;
            trainer.Address = new DAL.Models.ValueOfObjects.Address
            {
                City = normalizedCity,
                Street = normalizedStreet,
                BuildingNumber = viewModel.BuildingNumber
            };

            _unitOfWork.Trainers.Update(trainer);
            var rowsAffected = await _unitOfWork.CompleteAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to persist updates for trainer ID: {TrainerId}", viewModel.Id);
                return Result.Failure("Failed to update trainer.");
            }

            _logger.LogInformation("Trainer ID: {TrainerId} updated successfully.", viewModel.Id);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(
              id,
              include: query => query.Include(t=>t.Sessions),
              cancellationToken: cancellationToken);

            if (trainer == null)
            {
                _logger.LogWarning("Trainer deletion failed: Trainer ID {TrainerId} not found.", id);
                return Result.Failure("Trainer not found.", nameof(id));
            }

            bool HasActiveBookings = trainer.Sessions.Any(s => s.EndDate >= DateTime.UtcNow);
            if (HasActiveBookings)
            {
                _logger.LogWarning("Trainer deletion rejected: Trainer ID {TrainerId} has active sessions.", id);
                return Result.Failure("Cannot delete trainer with active bookings.");
            }


            await _unitOfWork.Trainers.SoftDeleteAsync(trainer, cancellationToken);
            var rowsAffected = await _unitOfWork.CompleteAsync(cancellationToken);
            if (rowsAffected == 0)
            {
                _logger.LogError("Failed to soft delete trainer ID: {TrainerId}", id);
                return Result.Failure("Failed to delete trainer.");
            }

            _logger.LogInformation("Trainer ID: {TrainerId} deleted successfully.", id);
            return Result.Success();
        }


    }

}


