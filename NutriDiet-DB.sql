-- Create the NutriDiet database if it doesn't exist
use master;
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'NutriDiet')
BEGIN
    DROP DATABASE NutriDiet;
END;
GO

CREATE DATABASE NutriDiet;
GO

USE NutriDiet;
GO

-- Bảng Role
CREATE TABLE Role (
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(50) UNIQUE NOT NULL
);

-- Bảng User
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Age INT CHECK (Age > 0),
    Gender NVARCHAR(10) CHECK (Gender IN ('Male', 'Female', 'Other')),
    Avatar NVARCHAR(255) NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Active', 'Inactive')) DEFAULT 'Active',
    RoleID INT NOT NULL,
    DailyCalorieTarget FLOAT CHECK (DailyCalorieTarget > 0),
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);

-- Bảng HealthProfile
CREATE TABLE HealthProfile (
    ProfileID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    MedicalCondition NVARCHAR(255) NULL,
    HeightCm FLOAT CHECK (HeightCm > 0),
    WeightKg FLOAT CHECK (WeightKg > 0),
    ActivityLevel NVARCHAR(50),
    HealthGoal NVARCHAR(50),
    TargetWeight FLOAT CHECK (TargetWeight > 0),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Bảng Allergy
CREATE TABLE Allergy (
    AllergyID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    AllergyName NVARCHAR(100) NOT NULL,
    Notes NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Bảng Food
CREATE TABLE Food (
    FoodID INT IDENTITY(1,1) PRIMARY KEY,
    FoodName NVARCHAR(100) UNIQUE NOT NULL,
    MealType NVARCHAR(100),
    FoodImageUrl NVARCHAR(MAX) NULL,
    FoodType NVARCHAR(100),
    Description NVARCHAR(255) NULL,
    ServingSize NVARCHAR(50),
    Calories FLOAT CHECK (Calories >= 0),
    Protein FLOAT CHECK (Protein >= 0),
    Carbs FLOAT CHECK (Carbs >= 0),
    Fat FLOAT CHECK (Fat >= 0)
);

-- Bảng UserFoodPreferences
CREATE TABLE UserFoodPreferences (
    UserFoodPreferenceID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    FoodID INT NOT NULL,
    Preference NVARCHAR(50),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- Bảng MealPlan
CREATE TABLE MealPlan (
    MealPlanID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    PlanName NVARCHAR(100) UNIQUE NOT NULL,
    HealthGoal NVARCHAR(50),
    Duration INT CHECK (Duration > 0),
    Status NVARCHAR(20) DEFAULT 'Draft',
    CreatedBy NVARCHAR(50) DEFAULT 'User',
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Bảng MealPlanDetail
CREATE TABLE MealPlanDetail (
    DetailID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT NOT NULL,
    FoodID INT NOT NULL,
    Quantity FLOAT CHECK (Quantity > 0),
    MealType NVARCHAR(50),
    DayNumber INT NOT NULL,
    TotalCalories FLOAT CHECK (TotalCalories >= 0),
    FOREIGN KEY (MealPlanID) REFERENCES MealPlan(MealPlanID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- Bảng Feedback
CREATE TABLE Feedback (
    FeedbackID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT NOT NULL,
    UserID INT NOT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Message NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MealPlanID) REFERENCES MealPlan(MealPlanID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION -- Changed to avoid cycles
);

-- Bảng FeedbackReply
CREATE TABLE FeedbackReply (
    ReplyID INT IDENTITY(1,1) PRIMARY KEY,
    FeedbackID INT NOT NULL,
    UserID INT NOT NULL,
    ReplyMessage NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FeedbackID) REFERENCES Feedback(FeedbackID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE NO ACTION -- Changed to avoid cycles
);

-- Bảng FoodSubstitution
CREATE TABLE FoodSubstitution (
    SubstitutionID INT IDENTITY(1,1) PRIMARY KEY,
    OriginalFoodID INT NOT NULL,
    SubstituteFoodID INT NOT NULL,
    Reason NVARCHAR(255),
    FOREIGN KEY (OriginalFoodID) REFERENCES Food(FoodID) ON DELETE NO ACTION,  -- Changed to NO ACTION
    FOREIGN KEY (SubstituteFoodID) REFERENCES Food(FoodID) ON DELETE NO ACTION   -- Changed to NO ACTION
);

-- Bảng Notification
CREATE TABLE Notification (
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(255),
    Description NVARCHAR(255),
    Status NVARCHAR(50) CHECK (Status IN ('Unread', 'Read')),
    Date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Bảng Progress
CREATE TABLE Progress (
    ProgressID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Date DATETIME DEFAULT GETDATE(),
    WeightKg FLOAT CHECK (WeightKg > 0),
    BMI FLOAT CHECK (BMI > 0),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- Bảng AIRecommendations
CREATE TABLE AIRecommendations (
    RecommendationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    MealPlanID INT NOT NULL,
    RecommendedAt DATETIME DEFAULT GETDATE(),
    Feedback NVARCHAR(MAX),
    IsAccepted BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (MealPlanID) REFERENCES MealPlan(MealPlanID) ON UPDATE NO ACTION
);

-- Bảng MealLog
CREATE TABLE MealLog (
    MealLogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    FoodID INT NOT NULL,
    MealType NVARCHAR(50),
    Quantity FLOAT CHECK (Quantity > 0),
    TotalCalories FLOAT CHECK (TotalCalories >= 0),
    LogDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);