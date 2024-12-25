-- Healthy Diet Recommendation System Database Schema
-- SQL Server Design

CREATE DATABASE NutriDiet;
GO

USE NutriDiet;
GO

-- 1. User Management
CREATE TABLE Role (
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Age INT CHECK (Age > 0),
    Gender NVARCHAR(10),
    Avatar NVARCHAR(255),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active', -- e.g., 'Active', 'Inactive', 'Suspended'
    RoleID INT,
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);

-- 2. Health Profile
CREATE TABLE HealthProfile (
    ProfileID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    MedicalConditions NVARCHAR(255), -- e.g., 'Diabetes', 'Hypertension',
    HeightCm FLOAT CHECK (HeightCm > 0),
    WeightKg FLOAT CHECK (WeightKg > 0),
    ActivityLevel NVARCHAR(50),
    Goal NVARCHAR(50), -- e.g., 'Lose Weight', 'Maintain Weight', 'Gain Weight',
    TargetWeight FLOAT, 
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- 3. Allergy Tracking
CREATE TABLE Allergy (
    AllergyID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    FoodName NVARCHAR(100) NOT NULL, -- Name of the food causing allergy
    Notes NVARCHAR(255), -- Additional details about the allergy
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- 4. Food Database
CREATE TABLE Food (
    FoodID INT IDENTITY(1,1) PRIMARY KEY,
    FoodName NVARCHAR(100) NOT NULL,
    FoodType NVARCHAR(100), -- e.g., 'Vegetable', 'Meat', 'Dairy'
    Description NVARCHAR(255),
    ServingSize NVARCHAR(50) -- e.g., '100g', '1 cup'
);

CREATE TABLE FoodDetail (
    FoodDetailID INT IDENTITY(1,1) PRIMARY KEY,
    FoodID INT,
    FoodDetailName NVARCHAR(100), -- e.g., 'Calories', 'Protein', 'Carbs', 'Vitamin C'
    Unit NVARCHAR(20) NOT NULL, -- e.g., 'g', 'mg', 'kcal'
    Amount FLOAT CHECK (Amount >= 0),
    Description NVARCHAR(255), -- Optional, e.g., 'Energy value per serving'
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- 5. Meal Plans
CREATE TABLE MealPlans (
    MealPlanID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    PlanName NVARCHAR(100),
    TotalCalories FLOAT CHECK (TotalCalories >= 0),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

CREATE TABLE MealPlanDetail (
    DetailID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT,
    FoodID INT,
    Quantity FLOAT CHECK (Quantity > 0), -- e.g., servings or grams
    MealType NVARCHAR(50), -- e.g., 'Breakfast', 'Lunch', 'Dinner', 'Snack'
    FOREIGN KEY (MealPlanID) REFERENCES MealPlans(MealPlanID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- 6. Feedbacks
-- 6. Feedbacks
CREATE TABLE Feedback (
    FeedbackID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT,
    UserID INT,
    Message NVARCHAR(255),
    Rating INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MealPlanID) REFERENCES MealPlans(MealPlanID) ON DELETE NO ACTION,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION
);

-- 7. AI Recommendations
CREATE TABLE Recommendation (
    RecommendationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    RecommendationText NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION
);

-- 8. Food Substitution Suggestions
CREATE TABLE FoodSubstitution (
    SubstitutionID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    OriginalFoodID INT,
    SubstituteFoodID INT,
    Reason NVARCHAR(255), -- e.g., 'Allergy', 'Unavailable', 'Healthier Option'
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION,
    FOREIGN KEY (OriginalFoodID) REFERENCES Food(FoodID) ON DELETE NO ACTION,
    FOREIGN KEY (SubstituteFoodID) REFERENCES Food(FoodID) ON DELETE NO ACTION
);