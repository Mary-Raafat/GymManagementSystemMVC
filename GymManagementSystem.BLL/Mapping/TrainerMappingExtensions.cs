using GymManagementSystem.BLL.ViewModels.Trainer;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueOfObjects;

namespace GymManagementSystem.BLL.Mapping
{
    public static class TrainerMappingExtensions
    {
        // Trainer → TrainerViewModel (قائمة المدربين)
        public static TrainerViewModel ToViewModel(this Trainer trainer)
        {
            return new TrainerViewModel
            {
                Id = trainer.ID,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialization = trainer.Speciality.ToString()
            };
        }

        // Trainer → TrainerDetailsViewModel (تفاصيل المدرب)
        public static TrainerDetailsViewModel ToDetailsViewModel(this Trainer trainer)
        {
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

        // Trainer → EditTrainerViewModel (للتعديل)
        public static EditTrainerViewModel ToEditViewModel(this Trainer trainer)
        {
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

        // CreateTrainerViewModel → Trainer (إنشاء مدرب جديد)
        public static Trainer ToEntity(this CreateTrainerViewModel viewModel, string normalizedName, string normalizedEmail, string normalizedPhone)
        {
            return new Trainer
            {
                Name = normalizedName,
                Email = normalizedEmail,
                Phone = normalizedPhone,
                Gender = viewModel.Gender,
                Address = new Address
                {
                    City = viewModel.City,
                    Street = viewModel.Street,
                    BuildingNumber = viewModel.BuildingNumber,
                },
                Speciality = viewModel.Specialization
            };
        }
    }
}
