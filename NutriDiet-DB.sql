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
    fcmToken NVARCHAR(MAX) NULL,
    Status NVARCHAR(50) CHECK (Status IN ('Active', 'Inactive')) DEFAULT 'Active',
    RoleID INT NOT NULL,
    RefreshToken NVARCHAR(MAX) NULL,
	RefreshTokenExpiryTime DATETIME NULL,
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
CREATE TABLE GeneralHealthProfile (
    ProfileID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Height FLOAT CHECK (Height > 0),
    Weight FLOAT CHECK (Weight > 0),
    ActivityLevel NVARCHAR(50),
    HealthGoal NVARCHAR(50),
	AISuggestion NVARCHAR(255),
	Status NVARCHAR(50) CHECK (Status IN ('Active', 'Expired')), 
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng Allergy (danh mục các loại dị ứng)
CREATE TABLE Allergy (
    AllergyID INT IDENTITY(1,1) PRIMARY KEY,
    AllergyName NVARCHAR(255) NOT NULL,
    Notes NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian UserAllergy (quan hệ n-n giữa User và Allergy)
CREATE TABLE UserAllergy (
    UserID INT NOT NULL,
    AllergyID INT NOT NULL,
    PRIMARY KEY (UserID, AllergyID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
    FOREIGN KEY (AllergyID) REFERENCES Allergy(AllergyID) ON DELETE CASCADE
);

-- Bảng Food
CREATE TABLE Food (
    FoodID INT IDENTITY(1,1) PRIMARY KEY,
    FoodName NVARCHAR(100) UNIQUE NOT NULL,
    MealType NVARCHAR(100), -- VD: bữa chính, bữa phụ, ăn vặt...
    ImageUrl NVARCHAR(MAX) NULL,
    FoodType NVARCHAR(100), -- ví dụ: rau củ, thịt
    Description NVARCHAR(255) NULL,
    ServingSize NVARCHAR(50), -- ví dụ: 100g, 1 thìa
    Calories FLOAT CHECK (Calories >= 0),
    Protein FLOAT CHECK (Protein >= 0),
    Carbs FLOAT CHECK (Carbs >= 0), -- tinh bột
    Fat FLOAT CHECK (Fat >= 0),
    Glucid FLOAT CHECK (Glucid >= 0),
    Fiber FLOAT CHECK (Fiber >= 0),
    Others NVARCHAR(255) NULL
);

-- Bảng CuisineType
CREATE TABLE CuisineType (
    CuisineID INT IDENTITY(1,1) PRIMARY KEY,
    CuisineName NVARCHAR(50) UNIQUE NOT NULL -- miền nam, miền trung, trung hoa...
);

-- Bảng RecipeSuggestion
CREATE TABLE RecipeSuggestion (
    RecipeID INT IDENTITY(1,1) PRIMARY KEY, -- ID của công thức
    UserID INT NOT NULL,
    FoodID INT NOT NULL,                    -- Món ăn được gợi ý công thức
	CuisineID INT NOT NULL,    
    AIRequest NVARCHAR(MAX),       -- Đầu vào cho AI xử lý
	AIResponse NVARCHAR(MAX),		--Đầu ra của AI
    AIModel NVARCHAR(255) NOT NULL, -- call api dùng model nào của AI
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
	FOREIGN KEY (CuisineID) REFERENCES CuisineType(CuisineID) ON DELETE CASCADE
);

-- Bảng UserFoodPreferences
CREATE TABLE UserFoodPreference (
    UserFoodPreferenceID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    FoodID INT NOT NULL,
    Preference NVARCHAR(50), -- ví dụ: like/dislike
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- Bảng MealPlan
CREATE TABLE MealPlan (
    MealPlanID INT IDENTITY(1,1) PRIMARY KEY,
    PlanName NVARCHAR(100) UNIQUE NOT NULL,
    HealthGoal NVARCHAR(50),
    Duration INT CHECK (Duration > 0),
    Status NVARCHAR(20) DEFAULT 'Active',
    CreatedBy NVARCHAR(50), --Admin, System, UserName, AI
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedBy NVARCHAR(50),
    UpdatedAt DATETIME DEFAULT GETDATE(),
);

-- Bảng MealPlanDetail
CREATE TABLE MealPlanDetail (
    MealPlanDetailID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT NOT NULL,
    FoodID INT NULL,
    FoodName NVARCHAR(255) NULL,
    Quantity FLOAT CHECK (Quantity > 0),
    MealType NVARCHAR(50), -- ví dụ: bữa sáng, bữa trưa
    DayNumber INT NOT NULL, -- ngày 1,2...
    TotalCalories FLOAT CHECK (TotalCalories >= 0) DEFAULT 0, -- tổng calo theo bữa
    TotalCarbs FLOAT CHECK (TotalCarbs >= 0) DEFAULT 0, -- theo bữa
    TotalFat FLOAT CHECK (TotalFat >= 0) DEFAULT 0, -- theo bữa
	TotalProtein FLOAT CHECK (TotalProtein >= 0) DEFAULT 0, -- theo bữa
    FOREIGN KEY (MealPlanID) REFERENCES MealPlan(MealPlanID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- Bảng UserMealPlan
-- Nếu User áp dụng kế hoạch của người khác, hệ thống chỉ cần thêm một dòng trong UserMealPlan
CREATE TABLE UserMealPlan (
    UserMealPlanID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    MealPlanID INT NOT NULL,
    AppliedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
    FOREIGN KEY (MealPlanID) REFERENCES MealPlan(MealPlanID) ON DELETE CASCADE
);

-- Bảng Feedback
CREATE TABLE FeedbackMealPlan (
    FeedbackID INT IDENTITY(1,1) PRIMARY KEY,
    MealPlanID INT NOT NULL,
    UserID INT NOT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Message NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MealPlanID) REFERENCES MealPlan(MealPlanID) ON DELETE CASCADE,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE NO ACTION
);

-- Bảng FoodSubstitution
CREATE TABLE FoodSubstitution (
    SubstitutionID INT IDENTITY(1,1) PRIMARY KEY,
    OriginalFoodID INT NOT NULL,
    SubstituteFoodID INT NOT NULL,
    Reason NVARCHAR(255),
    FOREIGN KEY (OriginalFoodID) REFERENCES Food(FoodID) ON DELETE NO ACTION,
    FOREIGN KEY (SubstituteFoodID) REFERENCES Food(FoodID) ON DELETE NO ACTION
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

-- Bảng HealthcareIndicator/UserParameter (các tham số người dùng)
CREATE TABLE HealthcareIndicator (
    HealthcareIndicatorID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Code NVARCHAR(50) NOT NULL, -- BMI,TDEE,..
    Name NVARCHAR(255) NOT NULL, -- Tên đầy đủ của tham số
    Type NVARCHAR(50) NOT NULL, -- Loại dữ liệu (Calorie, Health, Nutrient, Hydration,...)
	CurrentValue FLOAT NULL, -- giá trị hiện tại của khách
    MinValue FLOAT NULL, 
    MediumValue FLOAT NULL,
    MaxValue FLOAT NULL,
    Active BIT DEFAULT 1,
    AISuggestion NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng AIRecommendation
CREATE TABLE AIRecommendation (
    RecommendationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    RecommendedAt DATETIME DEFAULT GETDATE(),
    RecommendationText NVARCHAR(MAX),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng MealLog
CREATE TABLE MealLog (
    MealLogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    MealType NVARCHAR(50),
    LogDate DATETIME DEFAULT GETDATE(),
    TotalCalories FLOAT CHECK (TotalCalories >= 0),
    TotalProtein FLOAT CHECK (TotalProtein >= 0) DEFAULT 0,
    TotalCarbs FLOAT CHECK (TotalCarbs >= 0) DEFAULT 0,
    TotalFat FLOAT CHECK (TotalFat >= 0) DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng MealLogDetail
CREATE TABLE MealLogDetail (
    DetailID INT IDENTITY(1,1) PRIMARY KEY,
    MealLogID INT NOT NULL,
    FoodID INT NOT NULL,
    Quantity FLOAT CHECK (Quantity > 0),
    Calories FLOAT CHECK (Calories >= 0),
    ServingSize NVARCHAR(50),
    Protein FLOAT CHECK (Protein >= 0),
    Carbs FLOAT CHECK (Carbs >= 0),
    Fat FLOAT CHECK (Fat >= 0),
    FOREIGN KEY (MealLogID) REFERENCES MealLog(MealLogID) ON DELETE CASCADE,
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE
);

-- Bảng PersonalGoal
CREATE TABLE PersonalGoal (
    GoalID INT IDENTITY(1,1) PRIMARY KEY,               
    UserID INT NOT NULL,                                
    GoalType NVARCHAR(50) NOT NULL,                     -- Loại mục tiêu (ví dụ: Sức khỏe, Thể hình, Dinh dưỡng)
	TargetWeight FLOAT CHECK (TargetWeight > 0),
    GoalDescription NVARCHAR(255) NOT NULL,             -- Mô tả chi tiết về mục tiêu
    StartDate DATETIME DEFAULT GETDATE(),               -- Ngày bắt đầu
    TargetDate DATETIME NOT NULL,                       -- Ngày hoàn thành dự kiến
    ProgressRate INT DEFAULT 0,							-- tốc độ tăng giảm tuần/tháng
	Status NVARCHAR(20) DEFAULT 'Active' CHECK (Status IN ('Active', 'Completed', 'Failed')),
    ProgressPercentage FLOAT CHECK (ProgressPercentage >= 0 AND ProgressPercentage <= 100) DEFAULT 0, -- phần trăm hoàn thành
    Notes NVARCHAR(MAX) NULL,
	DailyCalories FLOAT,
	DailyCarb FLOAT,
	DailyFat FLOAT,
	DailyProtein FLOAT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);

-- Bảng Disease (danh mục các loại bệnh)
CREATE TABLE Disease (
    DiseaseID INT IDENTITY(1,1) PRIMARY KEY,
    DiseaseName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng trung gian UserDisease (quan hệ n-n giữa User và Disease)
CREATE TABLE UserDisease (
    UserID INT NOT NULL,
    DiseaseID INT NOT NULL,
    PRIMARY KEY (UserID, DiseaseID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE,
    FOREIGN KEY (DiseaseID) REFERENCES Disease(DiseaseID) ON DELETE CASCADE
);

-- Bảng FoodAllergy (liên kết món ăn với dị ứng)
CREATE TABLE FoodAllergy (
    FoodID INT NOT NULL,
    AllergyID INT NOT NULL,
    PRIMARY KEY (FoodID, AllergyID),
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE,
    FOREIGN KEY (AllergyID) REFERENCES Allergy(AllergyID) ON DELETE CASCADE
);

-- Bảng FoodDisease (liên kết món ăn với bệnh lý)
CREATE TABLE FoodDisease (
    FoodID INT NOT NULL,
    DiseaseID INT NOT NULL,
    PRIMARY KEY (FoodID, DiseaseID),
    FOREIGN KEY (FoodID) REFERENCES Food(FoodID) ON DELETE CASCADE,
    FOREIGN KEY (DiseaseID) REFERENCES Disease(DiseaseID) ON DELETE CASCADE
);

-- Bảng MyFood (Món ăn do người dùng tự thêm)
CREATE TABLE MyFood (
    MyFoodID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,  -- Người dùng tạo món ăn
    FoodName NVARCHAR(100) NOT NULL,
    ServingSize NVARCHAR(50), -- Đơn vị khẩu phần ví dụ: 100g, 1 thìa
    Calories FLOAT CHECK (Calories >= 0),
    Protein FLOAT CHECK (Protein >= 0),
	Carbs FLOAT CHECK (Carbs >= 0),
    Fat FLOAT CHECK (Fat >= 0),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES [User](UserID) ON DELETE CASCADE
);