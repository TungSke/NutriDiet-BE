using NutriDiet.Repository.Base;
using NutriDiet.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Repository.Interface
{
    public interface IHealthcareIndicatorRepository : IGenericRepository<HealthcareIndicator>
    {
        double CalculateTDEE(double weightKg, double heightCm, int age, string gender, double activityLevel);
        double CalculateBMI(double weightKg, double heightCm);
    }
}
