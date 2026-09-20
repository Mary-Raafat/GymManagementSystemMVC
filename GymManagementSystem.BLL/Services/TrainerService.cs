using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.BLL.ViewModels.Trainer;
using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services
{
    public class TrainerService(ITrainerRepo trainerRepo) : ITrainerService
    {
        private readonly ITrainerRepo _trainerRepo = trainerRepo;


        public async Task<IEnumerable<TrainerViewModel>>  GetAllAsync(CancellationToken ct=default)
        {
            var trainers=await _trainerRepo.GetAllAsync(cancellationToken: ct);
            var trainerViewModels = trainers.Select(t => new TrainerViewModel
            {
                Id = t.ID,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialization = t.Speciality.ToString()
            });
            return trainerViewModels;
        }

        public async Task<Result> CreateAsync(CreateTrainerViewModel viewModel, CancellationToken ct = default)
        {
            var email = viewModel.Email.Trim().ToLower();
            var phone = viewModel.Phone.Trim().ToLower();
            var name = viewModel.Name.Trim().ToLower();
            if (await _trainerRepo.IsEmailTakenAsync(email, ct: ct))
            {
                return Result.Failure("Email already exists.", nameof(CreateTrainerViewModel.Email));
            }

            if (await _trainerRepo.IsPhoneTakenAsync(phone, ct: ct))
            {
                return Result.Failure("Phone number already exists.", nameof(CreateTrainerViewModel.Phone));
            }
            var trainer = new Trainer // ربط ال trainer بال view model
            {
                Name = name,
                Email = email,
                Phone = phone,
                Gender = viewModel.Gender,
                Address = new DAL.Models.ValueOfObjects.Address
                {
                    City = viewModel.City,
                    Street = viewModel.Street,
                    BuildingNumber = viewModel.BuildingNumber,
                },

                Speciality = viewModel.Specialization
            };
            await _trainerRepo.AddAsync(trainer, ct);
            var rowsAffected = await _trainerRepo.SaveChangesAsync(ct);
            if (rowsAffected == 0)
            {
                return Result.Failure("Failed to add trainer.");
            }

            return Result.Success();
        }

        public async Task<TrainerDetailsViewModel?> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id: id, cancellationToken: ct);
            if (trainer == null)
            {
                return null;
            }

            return new TrainerDetailsViewModel
            {
                Id = trainer.ID,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialization = trainer.Speciality.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToString("yyyy-MM-dd"),
                Address = $"{trainer.Address.Street}, {trainer.Address.City}, {trainer.Address.BuildingNumber}"
            };
        }

        public async Task<EditTrainerViewModel?> GetForEditAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id: id, cancellationToken: ct);
            if (trainer == null)
            {
                return null;
            }

            return new EditTrainerViewModel
            {
                Id = trainer.ID,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Gender = trainer.Gender.ToString(),
                Specialization = trainer.Speciality,
                DateOfBirth = trainer.DateOfBirth.ToString("yyyy-MM-dd"),
                City = trainer.Address.City,
                Street = trainer.Address.Street,
                BuildingNumber = trainer.Address.BuildingNumber
            };
        }

        public async Task<Result> UpdateAsync(EditTrainerViewModel viewModel, CancellationToken cancellationToken = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id: viewModel.Id, cancellationToken: cancellationToken);
            if (trainer == null)
            {
                return Result.Failure("Trainer not found.", nameof(viewModel.Id));
            }

            var normalizedEmail = viewModel.Email.Trim().ToLowerInvariant();
            var normalizedPhone = viewModel.Phone.Trim();
            var normalizedCity = viewModel.City.Trim();
            var normalizedStreet = viewModel.Street.Trim();

            bool isEmailChanged = trainer.Email != normalizedEmail;
            bool isPhoneChanged = trainer.Phone != normalizedPhone;
            bool isSpecializationChanged = trainer.Speciality != viewModel.Specialization;
            bool isAddressChanged = trainer.Address.City != normalizedCity ||
                                   trainer.Address.Street != normalizedStreet ||
                                   trainer.Address.BuildingNumber != viewModel.BuildingNumber;

            if (!isEmailChanged && !isPhoneChanged && !isAddressChanged && !isSpecializationChanged)
            {
                return Result.Failure("No changes were made.");
            }

            if (isEmailChanged && await _trainerRepo.IsEmailTakenAsync(normalizedEmail, excludeId: viewModel.Id, ct: cancellationToken))
            {
                return Result.Failure("Email already exists.", nameof(viewModel.Email));
            }

            if (isPhoneChanged && await _trainerRepo.IsPhoneTakenAsync(normalizedPhone, excludeId: viewModel.Id, ct: cancellationToken))
            {
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

            _trainerRepo.Update(trainer);
            await _trainerRepo.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var trainer = await _trainerRepo.GetByIdAsync(
              id,
              include: query => query.Include(t=>t.Sessions),
              cancellationToken: cancellationToken);

            if (trainer == null)
            {
                return Result.Failure("Trainer not found.", nameof(id));
            }

            bool HasActiveBookings = trainer.Sessions.Any(s => s.EndDate >= DateTime.UtcNow);
            if (HasActiveBookings)
            {
                return Result.Failure("Cannot delete trainer with active bookings.");
            }

            
            await _trainerRepo.SoftDeleteAsync(trainer, cancellationToken);
            await _trainerRepo.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }


    }

}

