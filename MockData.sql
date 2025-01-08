USE NutriDiet;
GO
-- Xóa dữ liệu từ các bảng có khóa ngoại trước
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

-- Xóa dữ liệu từ bảng chính Users và Role sau cùng
DELETE FROM Users;
DELETE FROM Role;

-- Chèn dữ liệu vào bảng Role
INSERT INTO Role (RoleID,RoleName) VALUES
(1,'Admin'),
(2,'Customer'),
(3,'Nutritionist');
GO

-- Chèn dữ liệu vào bảng Users
INSERT INTO Users (FullName, Email, Password, Phone, Age, Gender, Avatar, Status, RoleID, DailyCalorieTarget) VALUES
('Alice Smith', 'alice@example.com', 'password123', '1234567890', 28, 'Female', NULL, 'Active', 2, 2000),
('Bob Johnson', 'bob@example.com', 'password123', '0987654321', 35, 'Male', NULL, 'Active', 2, 2500),
('Charlie Brown', 'charlie@example.com', 'password123', '1122334455', 22, 'Other', NULL, 'Inactive', 2, 1800);
GO

-- Chèn dữ liệu vào bảng HealthProfile
INSERT INTO HealthProfile (UserID, MedicalCondition, HeightCm, WeightKg, ActivityLevel, HealthGoal, TargetWeight) VALUES
(1, 'No', 165.0, 60.0, 'Moderate', 'Weight Loss', 55.0),
(2, 'Diabetes', 178.0, 85.0, 'Active', 'Weight Maintenance', 82.0),
(3, 'No', 170.0, 70.0, 'Light', 'Weight Gain', 75.0);
GO

-- Chèn dữ liệu vào bảng Allergy
INSERT INTO Allergy (UserID, AllergyName, Notes) VALUES
(1, 'Peanuts', 'Severe allergy'),
(2, 'Lactose', 'Avoid dairy products'),
(3, 'None', NULL);
GO

-- Chèn dữ liệu vào bảng Food
INSERT INTO Food (FoodName, MealType, FoodImageUrl, FoodType, Description, ServingSize, Calories, Protein, Carbs, Fat) VALUES
('Grilled Chicken', 'Dinner', NULL, 'Protein', 'Juicy grilled chicken breast', '150g', 200, 30, 0, 7),
('Brown Rice', 'Dinner', NULL, 'Carbohydrate', 'Healthy brown rice', '100g', 111, 3, 23, 1),
('Broccoli', 'Dinner', NULL, 'Vegetable', 'Fresh steamed broccoli', '100g', 55, 4, 11, 1),
('Apple', 'Snack', NULL, 'Fruit', 'Crispy red apple', '1 medium', 95, 0, 25, 0),
('Greek Yogurt', 'Snack', NULL, 'Dairy', 'Creamy Greek yogurt', '150g', 120, 10, 5, 5),
('Salmon', 'Dinner', NULL, 'Protein', 'Rich in omega-3', '150g', 367, 39, 0, 22),
('Quinoa', 'Lunch', NULL, 'Grain', 'Nutritious quinoa', '100g', 120, 4, 21, 2),
('Almonds', 'Snack', NULL, 'Nut', 'Healthy almonds', '30g', 174, 6, 6, 15),
('Spinach', 'Salad', NULL, 'Vegetable', 'Fresh spinach leaves', '100g', 23, 3, 4, 0),
('Oatmeal', 'Breakfast', NULL, 'Grain', 'Warm oatmeal', '100g', 68, 2, 12, 1);
GO

-- Chèn dữ liệu vào bảng UserFoodPreferences
INSERT INTO UserFoodPreferences (UserID, FoodID, Preference) VALUES
(1, 1, 'Like'),
(1, 2, 'Like'),
(2, 3, 'Dislike'),
(2, 4, 'Like'),
(3, 5, 'Like');
GO

-- Chèn dữ liệu vào bảng MealPlan
INSERT INTO MealPlan (UserID, PlanName, HealthGoal, Duration, Status, CreatedBy) VALUES
(1, 'Weight Loss Plan', 'Weight Loss', 30, 'Active', 'Alice'),
(2, 'Muscle Gain Plan', 'Weight Gain', 60, 'Active', 'Bob'),
(3, 'Maintenance Plan', 'Weight Maintenance', 45, 'Draft', 'Charlie');
GO

-- Chèn dữ liệu vào bảng MealPlanDetail
INSERT INTO MealPlanDetail (MealPlanID, FoodID, Quantity, MealType, DayNumber, TotalCalories) VALUES
(1, 1, 1, 'Dinner', 1, 200),
(1, 2, 1, 'Dinner', 1, 111),
(2, 3, 1, 'Lunch', 1, 55),
(2, 4, 1, 'Snack', 1, 95),
(3, 5, 1, 'Breakfast', 1, 120);
GO

-- Chèn dữ liệu vào bảng Feedback
INSERT INTO Feedback (MealPlanID, UserID, Rating, Message) VALUES
(1, 1, 5, 'Great plan!'),
(2, 2, 4, 'Good suggestions.'),
(3, 3, 3, 'Okay, but could be better.');
GO

-- Chèn dữ liệu vào bảng FeedbackReply
INSERT INTO FeedbackReply (FeedbackID, UserID, ReplyMessage) VALUES
(1, 2, 'Thanks for the feedback!'),
(2, 3, 'Glad you liked it!'),
(3, 1, 'We appreciate your input.');
GO

-- Chèn dữ liệu vào bảng FoodSubstitution
INSERT INTO FoodSubstitution (OriginalFoodID, SubstituteFoodID, Reason) VALUES
(1, 2, 'Lower calories option'),
(2, 3, 'High fiber alternative'),
(3, 4, 'Fresh snack option');
GO

-- Chèn dữ liệu vào bảng Notification
INSERT INTO Notification (UserId, Title, Description, Status) VALUES
(1, 'Plan Update', 'Your meal plan has been updated.', 'Unread'),
(2, 'Feedback Received', 'You have a new feedback on your meal plan.', 'Read'),
(3, 'Reminder', 'Dont forget to log your meals!', 'Unread');
GO

-- Chèn dữ liệu vào bảng Progress
INSERT INTO Progress (UserID, WeightKg, BMI) VALUES
(1, 58.0, 21.3),
(2, 85.0, 26.8),
(3, 70.0, 24.2);
GO

-- Chèn dữ liệu vào bảng AIRecommendations
INSERT INTO AIRecommendations (UserID, MealPlanID, Feedback, IsAccepted) VALUES
(1, 1, 'Consider adding more protein.', 1),
(2, 2, 'Try to include more vegetables.', 0),
(3, 3, 'Focus on portion control.', 1);
GO

-- Chèn dữ liệu vào bảng MealLog
INSERT INTO MealLog (UserID, FoodID, MealType, Quantity, TotalCalories) VALUES
(1, 1, 'Dinner', 1, 200),
(2, 2, 'Lunch', 1, 111),
(3, 3, 'Snack', 1, 55);
GO