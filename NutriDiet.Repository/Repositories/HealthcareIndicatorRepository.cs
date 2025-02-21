using NutriDiet.Repository.Interface;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Repository.Repositories
{
    public class HealthcareIndicatorRepository : GenericRepository<HealthcareIndicator>, IHealthcareIndicatorRepository
    {
        public HealthcareIndicatorRepository(NutriDietContext context) : base(context)
        {
        }
        public double CalculateTDEE(double weightKg, double heightCm, int age, string gender, double activityLevel)
        {
            double bmr;

            if (gender.ToLower() == "male")
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * age + 5;
            }
            else if (gender.ToLower() == "female")
            {
                bmr = 10 * weightKg + 6.25 * heightCm - 5 * age - 161;
            }
            else
            {
                throw new ArgumentException("Invalid gender. Please use 'Male' or 'Female'.");
            }

            return bmr * activityLevel;
        }

        public double CalculateBMI(double weightKg, double heightCm)
        {
            if (heightCm <= 0 || weightKg <= 0)
            {
                throw new ArgumentException("Weight and height must be greater than zero.");
            }

            double heightM = heightCm / 100.0;
            return weightKg / (heightM * heightM);
        }
    }
}
