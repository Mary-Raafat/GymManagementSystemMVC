using GymManagementSystem.DAL.Enums;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueOfObjects;
using GymManagementSystem.Dbcontexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data
{
    public static class UserSeeder
    {
        public static async Task SeedUsers(GymContext context)
        {
            var members = await context.Users.OfType<Member>().ToListAsync();

            Console.WriteLine(members.Count);
            if (await context.Users.AnyAsync())
                return;

            var users = new List<User>
        {
            new Member
            {
                Name = "Ahmed Mohamed",
                Email = "ahmed@gmail.com",
                Phone = "01012345678",
                DateOfBirth = new DateOnly(2002, 5, 10),
                Gender = Gender.Male,
                JoinDate = DateTime.Now,

                Photo = "member1.jpg",

                Address = new Address
                {
                    City = "Mansoura",
                    Street = "El Gomhoria",
                    BuildingNumber = 15
                }
            },

            new Member
            {
                Name = "Sara Ali",
                Email = "sara@gmail.com",
                Phone = "01123456789",
                DateOfBirth = new DateOnly(2001, 8, 20),
                Gender = Gender.Female,
                JoinDate = DateTime.Now,

                Photo = "member2.jpg",

                Address = new Address
                {
                    City = "Cairo",
                    Street = "Nasr City",
                    BuildingNumber = 20
                }
            },

            new Member
            {
                Name = "Omar Hassan",
                Email = "omar@gmail.com",
                Phone = "01234567890",
                DateOfBirth = new DateOnly(2000, 12, 5),
                Gender = Gender.Male,
                JoinDate = DateTime.Now,

                Photo = "member3.jpg",

                Address = new Address
                {
                    City = "Alexandria",
                    Street = "Smouha",
                    BuildingNumber = 9
                }
            },

            new Trainer
            {
                Name = "Mohamed Adel",
                Email = "trainer1@gmail.com",
                Phone = "01512345678",
                DateOfBirth = new DateOnly(1994, 3, 15),
                Gender = Gender.Male,
                HireDate = DateTime.Now,

                Speciality = Speciality.Bodybuilding,

                Address = new Address
                {
                    City = "Mansoura",
                    Street = "Toriel",
                    BuildingNumber = 30
                }
            },

            new Trainer
            {
                Name = "Mona Samy",
                Email = "trainer2@gmail.com",
                Phone = "01087654321",
                DateOfBirth = new DateOnly(1996, 11, 8),
                Gender = Gender.Female,
                HireDate = DateTime.Now,

                Speciality = Speciality.Bodybuilding,

                Address = new Address
                {
                    City = "Cairo",
                    Street = "Heliopolis",
                    BuildingNumber = 12
                }
            }
        };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}