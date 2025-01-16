USE NutriDiet;
GO

-- Clean existing data
DELETE FROM FeedbackReply;
DELETE FROM Feedback;
DELETE FROM MealPlanDetail;
DELETE FROM MealPlan;
DELETE FROM UserFoodPreferences;
DELETE FROM Allergy;
DELETE FROM HealthProfile;
DELETE FROM Progress;
DELETE FROM AIRecommendations;
DELETE FROM MealLog;
DELETE FROM FoodSubstitution;
DELETE FROM Notification;
DELETE FROM Users;
DELETE FROM Role;
DELETE FROM Food;

-- Insert Roles with explicit IDs
INSERT INTO Role (RoleID, RoleName) VALUES
(1, 'Admin'),
(2, 'User');

-- Insert Users with explicit IDs
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserID, FullName, Email, Password, Phone, Age, Gender, Status, RoleID, DailyCalorieTarget)
VALUES 
(1, 'John Smith', 'john@example.com', 'hashedpassword123', '1234567890', 30, 'Male', 'Active', 2, 2200),
(2, 'Sarah Johnson', 'sarah@example.com', 'hashedpassword456', '0987654321', 28, 'Female', 'Active', 2, 1800),
(3, 'Mike Williams', 'mike@example.com', 'hashedpassword789', '5556667777', 35, 'Male', 'Active', 2, 2500);
SET IDENTITY_INSERT Users OFF;

-- Insert Health Profiles
INSERT INTO HealthProfile (UserID, MedicalCondition, HeightCm, WeightKg, ActivityLevel, HealthGoal, TargetWeight)
VALUES
(1, 'None', 175, 80, 'Moderate', 'Weight Loss', 75),
(2, 'None', 165, 65, 'Light', 'Maintain Weight', 65),
(3, 'Type 2 Diabetes', 180, 90, 'Moderate', 'Weight Loss', 82);

-- Insert Allergies
INSERT INTO Allergy (UserID, AllergyName, Notes)
VALUES
(1, 'Peanuts', 'Severe allergy - avoid all nuts'),
(2, 'Lactose', 'Moderate intolerance'),
(3, 'Shellfish', 'Mild allergy');

-- Insert Foods with explicit IDs
SET IDENTITY_INSERT Food ON;
INSERT INTO Food (FoodID, FoodName, MealType, FoodType, Description, ServingSize, Calories, Protein, Carbs, Fat)
VALUES
-- Breakfast options
(1, 'Oatmeal with Berries', 'Breakfast', 'Grain', 'Healthy whole grain oatmeal with mixed berries', '1 bowl', 289, 6, 58, 4),
(2, 'Greek Yogurt Parfait', 'Breakfast', 'Dairy', 'Greek yogurt with granola and honey', '1 cup', 220, 15, 28, 8),
(3, 'Scrambled Eggs', 'Breakfast', 'Protein', 'Fresh scrambled eggs', '2 eggs', 180, 12, 2, 12),
(4, 'Whole Grain Toast with Avocado', 'Breakfast', 'Grain', 'Toasted bread with mashed avocado', '2 slices', 260, 8, 30, 14),

-- Lunch options
(5, 'Grilled Chicken Salad', 'Lunch', 'Protein', 'Grilled chicken breast with mixed greens', '1 plate', 320, 28, 12, 18),
(6, 'Quinoa Buddha Bowl', 'Lunch', 'Grain', 'Quinoa with roasted vegetables', '1 bowl', 380, 12, 68, 8),
(7, 'Tuna Sandwich', 'Lunch', 'Protein', 'Whole grain bread with tuna and vegetables', '1 sandwich', 340, 22, 38, 12),
(8, 'Vegetable Stir Fry', 'Lunch', 'Vegetarian', 'Mixed vegetables with tofu', '1 plate', 300, 15, 40, 10),

-- Dinner options
(9, 'Salmon Fillet', 'Dinner', 'Protein', 'Grilled salmon with herbs', '200g', 412, 46, 0, 23),
(10, 'Lean Beef Steak', 'Dinner', 'Protein', 'Grilled lean beef steak', '200g', 386, 58, 0, 16),
(11, 'Chickpea Curry', 'Dinner', 'Vegetarian', 'Spiced chickpeas with vegetables', '1 bowl', 320, 15, 45, 12),
(12, 'Turkey Breast', 'Dinner', 'Protein', 'Roasted turkey breast', '200g', 280, 48, 0, 8);
SET IDENTITY_INSERT Food OFF;

-- Insert User Food Preferences
INSERT INTO UserFoodPreferences (UserID, FoodID, Preference)
VALUES
(1, 1, 'Like'),
(1, 5, 'Like'),
(1, 9, 'Like'),
(2, 2, 'Like'),
(2, 6, 'Like'),
(2, 11, 'Like'),
(3, 3, 'Like'),
(3, 7, 'Like'),
(3, 10, 'Like');

-- Insert AI Recommendations
INSERT INTO AIRecommendations (UserID, RecommendationText, IsAccepted)
VALUES
(1, 'Based on your weight loss goal of 5kg and moderate activity level, we recommend a daily calorie intake of 2200 calories. Your personalized meal plan includes: Breakfast: Oatmeal with berries (289 cal) or Scrambled eggs (180 cal), Lunch: Grilled chicken salad (320 cal) or Tuna sandwich (340 cal), Dinner: Salmon fillet (412 cal) or Turkey breast (280 cal). Focus on high-protein foods and complex carbohydrates.', 1),
(2, 'To maintain your current weight of 65kg with light activity level, we suggest a balanced meal plan of 1800 calories per day. Your plan includes: Breakfast: Greek yogurt parfait (220 cal) or Whole grain toast with avocado (260 cal), Lunch: Quinoa Buddha bowl (380 cal) or Vegetable stir fry (300 cal), Dinner: Chickpea curry (320 cal) or Salmon fillet (412 cal).', 1),
(3, 'Considering your Type 2 Diabetes and weight loss goal, we''ve created a low-glycemic meal plan with 2500 calories daily. Your plan includes: Breakfast: Scrambled eggs (180 cal) or Greek yogurt parfait (220 cal), Lunch: Grilled chicken salad (320 cal) or Tuna sandwich (340 cal), Dinner: Lean beef steak (386 cal) or Turkey breast (280 cal).', 1);

-- Insert Meal Plans with explicit IDs
SET IDENTITY_INSERT MealPlan ON;
INSERT INTO MealPlan (MealPlanID, UserID, PlanName, HealthGoal, Duration, Status, CreatedBy)
VALUES
(1, 1, 'Weight Loss Plan - Week 1', 'Weight Loss', 7, 'Active', 'AI'),
(2, 2, 'Maintenance Plan - Week 1', 'Maintain Weight', 7, 'Active', 'AI'),
(3, 3, 'Diabetic-Friendly Weight Loss Plan', 'Weight Loss', 7, 'Active', 'AI');
SET IDENTITY_INSERT MealPlan OFF;

-- Insert Meal Plan Details
INSERT INTO MealPlanDetail (MealPlanID, FoodID, Quantity, MealType, DayNumber, TotalCalories)
VALUES
-- User 1 (Weight Loss Plan)
(1, 1, 1, 'Breakfast', 1, 289),
(1, 5, 1, 'Lunch', 1, 320),
(1, 9, 1, 'Dinner', 1, 412),
(1, 3, 1, 'Breakfast', 2, 180),
(1, 7, 1, 'Lunch', 2, 340),
(1, 12, 1, 'Dinner', 2, 280),

-- User 2 (Maintenance Plan)
(2, 2, 1, 'Breakfast', 1, 220),
(2, 6, 1, 'Lunch', 1, 380),
(2, 11, 1, 'Dinner', 1, 320),
(2, 4, 1, 'Breakfast', 2, 260),
(2, 8, 1, 'Lunch', 2, 300),
(2, 9, 1, 'Dinner', 2, 412),

-- User 3 (Diabetic-Friendly Plan)
(3, 3, 1, 'Breakfast', 1, 180),
(3, 5, 1, 'Lunch', 1, 320),
(3, 10, 1, 'Dinner', 1, 386),
(3, 2, 1, 'Breakfast', 2, 220),
(3, 7, 1, 'Lunch', 2, 340),
(3, 12, 1, 'Dinner', 2, 280);

-- Insert Progress Records
INSERT INTO Progress (UserID, WeightKg, BMI)
VALUES
(1, 80, 26.1),
(2, 65, 23.8),
(3, 90, 27.8);

-- Insert Meal Logs
INSERT INTO MealLog (UserID, FoodID, MealType, Quantity, TotalCalories, LogDate)
VALUES
(1, 1, 'Breakfast', 1, 289, GETDATE()),
(2, 2, 'Breakfast', 1, 220, GETDATE()),
(3, 3, 'Breakfast', 1, 180, GETDATE());

-- Insert Ingredients
SET IDENTITY_INSERT Ingredients ON;
INSERT INTO Ingredients (IngredientID, IngredientName, Category, Unit, CaloriesPer100g, ProteinPer100g, CarbsPer100g, FatPer100g, Description)
VALUES
-- Grains & Cereals
(1, 'Oats', 'Grain', 'g', 389, 16.9, 66.3, 6.9, 'Whole grain rolled oats'),
(2, 'Quinoa', 'Grain', 'g', 368, 14.1, 64.2, 6.1, 'Whole grain quinoa'),
(3, 'Whole Grain Bread', 'Grain', 'slice', 247, 13.0, 41.0, 3.4, 'Whole wheat bread'),

-- Proteins
(4, 'Chicken Breast', 'Meat', 'g', 165, 31.0, 0, 3.6, 'Skinless chicken breast'),
(5, 'Salmon', 'Fish', 'g', 206, 22.0, 0, 13.0, 'Fresh Atlantic salmon'),
(6, 'Beef Steak', 'Meat', 'g', 193, 29.0, 0, 8.0, 'Lean beef cut'),
(7, 'Turkey Breast', 'Meat', 'g', 140, 24.0, 0, 4.0, 'Skinless turkey breast'),
(8, 'Tuna', 'Fish', 'g', 144, 23.6, 0, 4.9, 'Canned tuna in water'),
(9, 'Eggs', 'Protein', 'piece', 155, 12.6, 1.1, 11.3, 'Fresh whole eggs'),
(10, 'Tofu', 'Protein', 'g', 144, 15.9, 3.3, 8.7, 'Firm tofu'),

-- Vegetables
(11, 'Mixed Greens', 'Vegetable', 'g', 17, 1.2, 3.3, 0.2, 'Assorted salad greens'),
(12, 'Avocado', 'Vegetable', 'piece', 160, 2.0, 8.5, 14.7, 'Ripe avocado'),
(13, 'Mixed Vegetables', 'Vegetable', 'g', 65, 3.2, 11.8, 0.6, 'Assorted vegetables'),
(14, 'Chickpeas', 'Legume', 'g', 364, 15.0, 61.0, 6.0, 'Cooked chickpeas'),

-- Dairy & Others
(15, 'Greek Yogurt', 'Dairy', 'g', 59, 10.0, 3.6, 0.4, 'Plain Greek yogurt'),
(16, 'Berries', 'Fruit', 'g', 57, 0.7, 14.0, 0.3, 'Mixed berries'),
(17, 'Honey', 'Condiment', 'g', 304, 0.3, 82.4, 0, 'Pure honey'),
(18, 'Granola', 'Grain', 'g', 471, 10.0, 64.0, 20.0, 'Homemade granola'),
(19, 'Curry Sauce', 'Sauce', 'g', 160, 2.0, 18.0, 9.0, 'Homemade curry sauce');
SET IDENTITY_INSERT Ingredients OFF;

-- Insert FoodIngredients relationships
INSERT INTO FoodIngredients (FoodID, IngredientID, Amount, Unit, Notes)
VALUES
-- Oatmeal with Berries (FoodID: 1)
(1, 1, 60, 'g', 'Cook in water or milk'),
(1, 16, 100, 'g', 'Add after cooking'),
(1, 17, 10, 'g', 'Add to taste'),

-- Greek Yogurt Parfait (FoodID: 2)
(2, 15, 200, 'g', 'Base layer'),
(2, 18, 45, 'g', 'Sprinkle on top'),
(2, 17, 15, 'g', 'Drizzle on top'),
(2, 16, 50, 'g', 'Add as topping'),

-- Scrambled Eggs (FoodID: 3)
(3, 9, 2, 'piece', 'Whisk before cooking'),

-- Whole Grain Toast with Avocado (FoodID: 4)
(4, 3, 2, 'slice', 'Toast until golden'),
(4, 12, 1, 'piece', 'Mash and spread'),

-- Grilled Chicken Salad (FoodID: 5)
(5, 4, 150, 'g', 'Grill until cooked through'),
(5, 11, 100, 'g', 'Fresh salad base'),

-- Quinoa Buddha Bowl (FoodID: 6)
(6, 2, 75, 'g', 'Cook until fluffy'),
(6, 13, 200, 'g', 'Roast vegetables'),

-- Tuna Sandwich (FoodID: 7)
(7, 8, 100, 'g', 'Drain well'),
(7, 3, 2, 'slice', 'Toast if desired'),
(7, 11, 30, 'g', 'Add for crunch'),

-- Vegetable Stir Fry (FoodID: 8)
(8, 10, 150, 'g', 'Cube and fry until golden'),
(8, 13, 200, 'g', 'Stir fry until tender-crisp'),

-- Salmon Fillet (FoodID: 9)
(9, 5, 200, 'g', 'Season and grill'),

-- Lean Beef Steak (FoodID: 10)
(10, 6, 200, 'g', 'Grill to desired doneness'),

-- Chickpea Curry (FoodID: 11)
(11, 14, 200, 'g', 'Pre-cook chickpeas'),
(11, 13, 150, 'g', 'Add to curry'),
(11, 19, 100, 'g', 'Mix with vegetables'),

-- Turkey Breast (FoodID: 12)
(12, 7, 200, 'g', 'Roast until done');