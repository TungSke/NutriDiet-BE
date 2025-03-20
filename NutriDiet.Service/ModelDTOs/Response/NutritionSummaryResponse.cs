public class NutritionSummaryResponse
{
    // ... Các trường cũ ...
    public double TotalCalories { get; set; }
    public double NetCalories { get; set; }
    public double Goal { get; set; } // Daily Calorie Goal

    public List<MealTypeBreakdownResponse> MealBreakdown { get; set; } = new();

    // Highest in Calories
    public List<FoodHighResponse> HighestInCalories { get; set; } = new();

    // MACROS thực tế
    public MacrosResponse Macros { get; set; } = new();

    // Bổ sung: MACROS Goal
    public MacroGoalResponse MacroGoal { get; set; } = new();

    // Highest in Carbs / Fat / Protein
    public List<FoodHighResponse> HighestInCarbs { get; set; } = new();
    public List<FoodHighResponse> HighestInFat { get; set; } = new();
    public List<FoodHighResponse> HighestInProtein { get; set; } = new();
}

// Mô tả macro thực tế
public class MacrosResponse
{
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Protein { get; set; }
}

// Mô tả macro goal (VD: 50% Carbs, 20% Fat, 30% Protein)
public class MacroGoalResponse
{
    public double CarbsRatio { get; set; }
    public double FatRatio { get; set; }
    public double ProteinRatio { get; set; }
}

// Meal breakdown
public class MealTypeBreakdownResponse
{
    public string MealType { get; set; }
    public double Calories { get; set; }
    public double Percentage { get; set; }
}

// Dùng cho HighestInCalories, HighestInCarbs, etc.
public class FoodHighResponse
{
    public string FoodName { get; set; }
    public double Value { get; set; }
}
