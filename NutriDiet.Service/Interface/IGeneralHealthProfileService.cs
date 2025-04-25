using NutriDiet.Common.BusinessResult;
using NutriDiet.Common.Enums;
using NutriDiet.Service.ModelDTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriDiet.Service.Interface
{
    public interface IGeneralHealthProfileService
    {
        Task CreateHealthProfileRecord(HealthProfileRequest request);
        Task<IBusinessResult> GetHealthProfile();
        Task<IBusinessResult> TrackingHealthProfile(HealProfileFields field);
        Task<IBusinessResult> GetSuggestionImproveFromAI();
        Task<IBusinessResult> CreateAISuggestion(CategoryAdvice adviceCategory = CategoryAdvice.All);
        Task<IBusinessResult> GetHealthProfiles();
        Task<IBusinessResult> DeleteProfileById(int profileId);
        Task<IBusinessResult> AddImageToHealthProfile(int profileId, AddImageRequest request);
        Task<IBusinessResult> DeleteImageFromHealthProfile(int profileId);
        Task<bool> HasCreatedHealthProfileToday();
    }
}
