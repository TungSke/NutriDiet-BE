USE master;
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'NutriDiet')
BEGIN
    ALTER DATABASE NutriDiet SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
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
CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Age INT CHECK (Age > 0),
    Gender NVARCHAR(10) CHECK (Gender IN ('Male', 'Female', 'Other')),
	Location NVARCHAR(20),
    Avatar NVARCHAR(255) NULL,
	fcmToken NVARCHAR(max) NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Active', 'Inactive')) DEFAULT 'Active',
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);
-- Bảng Package
CREATE TABLE Package (
    PackageID INT IDENTITY(1,1) PRIMARY KEY,
    PackageName NVARCHAR(100) UNIQUE NOT NULL,
    Price FLOAT CHECK (Price >= 0),
    Duration INT CHECK (Duration > 0),
    Description NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE UserPackage (
    UserPackageID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    PackageID INT NOT NULL,
    StartDate DATETIME DEFAULT GETDATE(),
    ExpiryDate DATETIME NOT NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Active', 'Expired')),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
    FOREIGN KEY (PackageID) REFERENCES Package(PackageID) ON DELETE CASCADE
);
-- Bảng HealthProfile
CREATE TABLE HealthProfile (
    ProfileID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    MedicalCondition NVARCHAR(255) NULL,
    Height FLOAT CHECK (Height > 0),
    Weight FLOAT CHECK (Weight > 0),
    ActivityLevel NVARCHAR(50),
    HealthGoal NVARCHAR(50),
    TargetWeight FLOAT CHECK (TargetWeight > 0),
	DurationTarget DateTime,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng Allergy
CREATE TABLE Allergy (
    AllergyID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    AllergyName NVARCHAR(255) NOT NULL,
    Notes NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);
-- Bảng Food
CREATE TABLE Food (
    FoodID INT IDENTITY(1,1) PRIMARY KEY,
    FoodName NVARCHAR(100) UNIQUE NOT NULL,
    MealType NVARCHAR(100),
    ImageUrl NVARCHAR(MAX) NULL,
    FoodType NVARCHAR(100), -- rau củ, thịt
    Description NVARCHAR(255) NULL,
    ServingSize NVARCHAR(50), -- 100g, 1 thìa
    Calories FLOAT CHECK (Calories >= 0),
    Protein FLOAT CHECK (Protein >= 0),
    Carbs FLOAT CHECK (Carbs >= 0), --tinh bột
    Fat FLOAT CHECK (Fat >= 0),
	Glucid FLOAT CHECK (Glucid >= 0),
	Fiber FLOAT CHECK (Fiber >= 0),
	Others NVARCHAR(255) NULL
);
-- Bảng nguyên liệu
CREATE TABLE Ingredient (
    IngredientID INT IDENTITY(1,1) PRIMARY KEY,
    IngredientName NVARCHAR(100) UNIQUE NOT NULL,
    Category NVARCHAR(50), -- Loại nguyên liệu (ví dụ: Rau củ, Thịt, Gia vị...)
    Unit NVARCHAR(20) NOT NULL, -- Đơn vị tính (gram, ml, piece...)
	Calories FLOAT NULL,
	FoodID INT NOT NULL,
	FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);


-- Bảng RecipeSuggestion
CREATE TABLE RecipeSuggestion (
    RecipeID INT IDENTITY(1,1) PRIMARY KEY, -- ID của công thức
	UserID INT NOT NULL,
    FoodID INT NOT NULL,                    -- Món ăn được gợi ý công thức
    Description NVARCHAR(MAX) NOT NULL, -- Mô tả từng bước thực hiện
	CreatedBy NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE,-- Ràng buộc khóa ngoại
	FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng UserFoodPreferences
CREATE TABLE UserFoodPreference (
    UserFoodPreferenceID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    FoodID INT NOT NULL,
    Preference NVARCHAR(50), -- like/dislike
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- Bảng MealPlan
CREATE TABLE MealPlan (
    MealPlanID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    PlanName NVARCHAR(100) UNIQUE NOT NULL,
    HealthGoal NVARCHAR(50),
    Duration INT CHECK (Duration > 0),
    Status NVARCHAR(20) Default 'Active',
    CreatedBy NVARCHAR(50),
    CreatedAt DATETIME DEFAULT GETDATE(),
	UpdatedBy NVARCHAR(50),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng MealPlanDetail
CREATE TABLE MealPlanDetail (
    MealPlanDetailID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT NOT NULL,
    FoodID INT NULL,
	FoodName NVARCHAR(255) NULL,
    Quantity FLOAT CHECK (Quantity > 0),
    MealType NVARCHAR(50), -- bữa sáng, bữa trưa
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
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE NO ACTION -- Changed to avoid cycles
);

-- Bảng FeedbackReply
CREATE TABLE FeedbackReply (
    ReplyID INT IDENTITY(1,1) PRIMARY KEY,
    FeedbackID INT NOT NULL,
    UserID INT NOT NULL,
    ReplyMessage NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FeedbackID) REFERENCES Feedback(FeedbackID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE NO ACTION -- Changed to avoid cycles
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
    FOREIGN KEY (UserId) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng UserParameter
CREATE TABLE UserParameter (
    UserParameterID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Code NVARCHAR(50),
    Name NVARCHAR(255),
    Type NVARCHAR(50),
    MinValue FLOAT NULL,
    MaxValue FLOAT NULL,
    Active BIT DEFAULT 1,
    AISuggestion NVARCHAR(255) NULL,
    Date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng AIRecommendation
CREATE TABLE AIRecommendation (
    RecommendationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    RecommendedAt DATETIME DEFAULT GETDATE(),
    RecommendationText NVARCHAR(MAX),
    IsAccepted BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
);

-- Bảng MealLog
-- Bảng chung về bữa ăn
CREATE TABLE MealLog (
    MealLogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    MealType NVARCHAR(50), -- Bữa sáng, bữa trưa, bữa tối
    LogDate DATETIME DEFAULT GETDATE(),
    TotalCalories FLOAT CHECK (TotalCalories >= 0),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng chi tiết món ăn trong mỗi bữa
CREATE TABLE MealLogDetail (
    DetailID INT IDENTITY(1,1) PRIMARY KEY,
    MealLogID INT NOT NULL,
    FoodID INT NOT NULL,
    Quantity FLOAT CHECK (Quantity > 0),
    Calories FLOAT CHECK (Calories >= 0),
    FOREIGN KEY (MealLogID) REFERENCES MealLog(MealLogID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);


CREATE TABLE PersonalGoal (
    GoalID INT IDENTITY(1,1) PRIMARY KEY,               
    UserID INT NOT NULL,                                
    GoalType NVARCHAR(50) NOT NULL,                    -- Loại mục tiêu (ví dụ: Sức khỏe, Thể hình, Dinh dưỡng)
    GoalDescription NVARCHAR(255) NOT NULL,            -- Mô tả chi tiết về mục tiêu
    StartDate DATETIME DEFAULT GETDATE(),              -- Ngày bắt đầu
    TargetDate DATETIME NOT NULL,                      -- Ngày hoàn thành dự kiến
    Status NVARCHAR(20) DEFAULT 'Active' CHECK (Status IN ('Active', 'Completed', 'Failed')), -- Trạng thái mục tiêu
    ProgressPercentage FLOAT CHECK (ProgressPercentage >= 0 AND ProgressPercentage <= 100) DEFAULT 0, -- Tiến độ hoàn thành
    Notes NVARCHAR(MAX) NULL,                          -- Ghi chú thêm của người dùng
    CreatedAt DATETIME DEFAULT GETDATE(),              -- Ngày tạo mục tiêu
    UpdatedAt DATETIME DEFAULT GETDATE(),              -- Ngày cập nhật mục tiêu
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE -- Ràng buộc quan hệ với User
);

CREATE TABLE UserEatingHabit (
    HabitID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    HabitType NVARCHAR(50) NOT NULL, -- Loại thói quen (ăn chay, ăn kiêng, low-carb,...)
    Description NVARCHAR(255) NULL, -- Mô tả chi tiết
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);