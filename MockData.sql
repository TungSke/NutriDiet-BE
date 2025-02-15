USE NutriDiet;
GO

-- Tắt ràng buộc khóa ngoại tạm thời
ALTER TABLE UserAllergy NOCHECK CONSTRAINT ALL;
ALTER TABLE UserDisease NOCHECK CONSTRAINT ALL;
ALTER TABLE UserPackage NOCHECK CONSTRAINT ALL;
ALTER TABLE HealthProfile NOCHECK CONSTRAINT ALL;
ALTER TABLE Ingredient NOCHECK CONSTRAINT ALL;
ALTER TABLE RecipeSuggestion NOCHECK CONSTRAINT ALL;
ALTER TABLE UserFoodPreference NOCHECK CONSTRAINT ALL;
ALTER TABLE MealPlanDetail NOCHECK CONSTRAINT ALL;
ALTER TABLE Feedback NOCHECK CONSTRAINT ALL;
ALTER TABLE FeedbackReply NOCHECK CONSTRAINT ALL;
ALTER TABLE FoodSubstitution NOCHECK CONSTRAINT ALL;
ALTER TABLE Notification NOCHECK CONSTRAINT ALL;
ALTER TABLE UserParameter NOCHECK CONSTRAINT ALL;
ALTER TABLE AIRecommendation NOCHECK CONSTRAINT ALL;
ALTER TABLE MealLogDetail NOCHECK CONSTRAINT ALL;
ALTER TABLE MealLog NOCHECK CONSTRAINT ALL;
ALTER TABLE PersonalGoal NOCHECK CONSTRAINT ALL;

-- Xóa toàn bộ dữ liệu trong các bảng
DELETE FROM UserAllergy;
DELETE FROM UserDisease;
DELETE FROM UserPackage;
DELETE FROM HealthProfile;
DELETE FROM Ingredient;
DELETE FROM RecipeSuggestion;
DELETE FROM UserFoodPreference;
DELETE FROM MealPlanDetail;
DELETE FROM MealPlan;
DELETE FROM FeedbackReply;
DELETE FROM Feedback;
DELETE FROM FoodSubstitution;
DELETE FROM Notification;
DELETE FROM UserParameter;
DELETE FROM AIRecommendation;
DELETE FROM MealLogDetail;
DELETE FROM MealLog;
DELETE FROM PersonalGoal;

DELETE FROM Disease;
DELETE FROM Allergy;
DELETE FROM Food;
DELETE FROM Package;
DELETE FROM [User];
DELETE FROM Role;

-- Bật lại ràng buộc khóa ngoại
ALTER TABLE UserAllergy CHECK CONSTRAINT ALL;
ALTER TABLE UserDisease CHECK CONSTRAINT ALL;
ALTER TABLE UserPackage CHECK CONSTRAINT ALL;
ALTER TABLE HealthProfile CHECK CONSTRAINT ALL;
ALTER TABLE Ingredient CHECK CONSTRAINT ALL;
ALTER TABLE RecipeSuggestion CHECK CONSTRAINT ALL;
ALTER TABLE UserFoodPreference CHECK CONSTRAINT ALL;
ALTER TABLE MealPlanDetail CHECK CONSTRAINT ALL;
ALTER TABLE Feedback CHECK CONSTRAINT ALL;
ALTER TABLE FeedbackReply CHECK CONSTRAINT ALL;
ALTER TABLE FoodSubstitution CHECK CONSTRAINT ALL;
ALTER TABLE Notification CHECK CONSTRAINT ALL;
ALTER TABLE UserParameter CHECK CONSTRAINT ALL;
ALTER TABLE AIRecommendation CHECK CONSTRAINT ALL;
ALTER TABLE MealLogDetail CHECK CONSTRAINT ALL;
ALTER TABLE MealLog CHECK CONSTRAINT ALL;
ALTER TABLE PersonalGoal CHECK CONSTRAINT ALL;

DBCC CHECKIDENT ('User', RESEED, 0);
DBCC CHECKIDENT ('Disease', RESEED, 0);
DBCC CHECKIDENT ('Allergy', RESEED, 0);
DBCC CHECKIDENT ('Food', RESEED, 0);
DBCC CHECKIDENT ('Package', RESEED, 0);
DBCC CHECKIDENT ('MealPlan', RESEED, 0);
DBCC CHECKIDENT ('MealLog', RESEED, 0);
DBCC CHECKIDENT ('MealLogDetail', RESEED, 0);
DBCC CHECKIDENT ('PersonalGoal', RESEED, 0);

GO

-- Insert Roles with explicit IDs
INSERT INTO Role (RoleID, RoleName) VALUES
(1, 'Admin'),
(2, 'Customer');

SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserID], [FullName], [Email], [Password], [Phone], [Age], [Gender], [Location], [Avatar], [fcmToken], [Status], [RoleID]) VALUES (1, N'New User', N'user@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, NULL, NULL, NULL, N'', NULL, N'Active', 2)
INSERT [dbo].[User] ([UserID], [FullName], [Email], [Password], [Phone], [Age], [Gender], [Location], [Avatar], [fcmToken], [Status], [RoleID]) VALUES (2, N'Admin', N'admin@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, NULL, NULL, NULL, N'', NULL, N'Active', 1)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO

INSERT INTO Allergy (AllergyName, Notes, CreatedAt, UpdatedAt) VALUES
(N'Dị ứng đậu phộng', N'Dễ gây phản ứng nghiêm trọng như sốc phản vệ', GETDATE(), GETDATE()),
(N'Dị ứng hải sản', N'Bao gồm tôm, cua, sò, ốc, hến', GETDATE(), GETDATE()),
(N'Dị ứng sữa', N'Không dung nạp lactose hoặc dị ứng protein sữa', GETDATE(), GETDATE()),
(N'Dị ứng trứng', N'Thường gặp ở trẻ nhỏ, đặc biệt là lòng trắng trứng', GETDATE(), GETDATE()),
(N'Dị ứng gluten', N'Liên quan đến bệnh Celiac hoặc không dung nạp gluten', GETDATE(), GETDATE()),
(N'Dị ứng đậu nành', N'Phản ứng với protein trong đậu nành', GETDATE(), GETDATE()),
(N'Dị ứng hạt cây', N'Bao gồm hạt điều, hạnh nhân, óc chó', GETDATE(), GETDATE()),
(N'Dị ứng mè (vừng)', N'Gây phát ban, khó thở hoặc sốc phản vệ', GETDATE(), GETDATE()),
(N'Dị ứng lúa mì', N'Không giống như không dung nạp gluten, có thể gây sốc phản vệ', GETDATE(), GETDATE()),
(N'Dị ứng động vật có vỏ', N'Tôm, cua, sò thường gây dị ứng mạnh', GETDATE(), GETDATE()),
(N'Dị ứng cá', N'Dị ứng với các loại cá như cá hồi, cá ngừ, cá thu', GETDATE(), GETDATE()),
(N'Dị ứng mốc', N'Gây các vấn đề về hô hấp và da', GETDATE(), GETDATE()),
(N'Dị ứng bụi nhà', N'Gây viêm mũi dị ứng và hen suyễn', GETDATE(), GETDATE()),
(N'Dị ứng phấn hoa', N'Nguyên nhân phổ biến gây hắt hơi, chảy nước mũi', GETDATE(), GETDATE()),
(N'Dị ứng thuốc kháng sinh', N'Penicillin là tác nhân phổ biến nhất', GETDATE(), GETDATE()),
(N'Dị ứng côn trùng chích', N'Ong, kiến lửa có thể gây sốc phản vệ', GETDATE(), GETDATE()),
(N'Dị ứng nấm men', N'Liên quan đến thực phẩm lên men như bia, rượu, bánh mì', GETDATE(), GETDATE()),
(N'Dị ứng trái cây có múi', N'Cam, chanh, bưởi có thể gây kích ứng da hoặc miệng', GETDATE(), GETDATE()),
(N'Dị ứng socola', N'Dị ứng với cacao hoặc các chất phụ gia trong socola', GETDATE(), GETDATE()),
(N'Dị ứng sulfite', N'Chất bảo quản trong rượu vang, trái cây sấy khô', GETDATE(), GETDATE());

INSERT INTO Disease (DiseaseName, Description, CreatedAt, UpdatedAt) VALUES
(N'Bệnh tiểu đường', N'Yêu cầu chế độ ăn ít đường và tinh bột', GETDATE(), GETDATE()),
(N'Bệnh tim mạch', N'Chế độ ăn ít cholesterol và muối', GETDATE(), GETDATE()),
(N'Bệnh cao huyết áp', N'Giảm muối, hạn chế thực phẩm chế biến sẵn', GETDATE(), GETDATE()),
(N'Bệnh thận', N'Hạn chế protein và muối', GETDATE(), GETDATE()),
(N'Bệnh gout', N'Giảm thực phẩm giàu purine như hải sản, thịt đỏ', GETDATE(), GETDATE()),
(N'Không dung nạp lactose', N'Tránh sữa và sản phẩm từ sữa', GETDATE(), GETDATE()),
(N'Hội chứng ruột kích thích', N'Tránh thực phẩm gây kích thích như cafein, đồ cay', GETDATE(), GETDATE()),
(N'Béo phì', N'Cần kiểm soát calo, tăng cường thực phẩm lành mạnh', GETDATE(), GETDATE()),
(N'Suy dinh dưỡng', N'Cần bổ sung vitamin và khoáng chất', GETDATE(), GETDATE()),
(N'Dị ứng thực phẩm', N'Tổng hợp nhiều loại dị ứng với thực phẩm', GETDATE(), GETDATE());

-- Lấy ID của bệnh "Bệnh tiểu đường"
DECLARE @DiseaseId INT;
SELECT @DiseaseId = DiseaseID FROM Disease WHERE DiseaseName = N'Bệnh tiểu đường';

-- Lấy ID của dị ứng "Dị ứng đậu phộng"
DECLARE @AllergyId1 INT;
SELECT @AllergyId1 = AllergyID FROM Allergy WHERE AllergyName = N'Dị ứng đậu phộng';

-- Lấy ID của dị ứng "Dị ứng sữa"
DECLARE @AllergyId2 INT;
SELECT @AllergyId2 = AllergyID FROM Allergy WHERE AllergyName = N'Dị ứng sữa';

-- Gán bệnh cho User 1
INSERT INTO UserDisease (UserID, DiseaseID) VALUES (1, @DiseaseId);

-- Gán hai dị ứng cho User 1
INSERT INTO UserAllergy (UserID, AllergyID) VALUES (1, @AllergyId1);
INSERT INTO UserAllergy (UserID, AllergyID) VALUES (1, @AllergyId2);

-- Thêm 20 món ăn Việt Nam vào bảng Food
SET IDENTITY_INSERT Food ON;
INSERT INTO Food (FoodID, FoodName, MealType, FoodType, Description, ServingSize, Calories, Protein, Carbs, Fat, Glucid, Fiber)
VALUES
    (1, 'Phở bò', 'Main', 'Noodle', 'Phở truyền thống với thịt bò', '1 tô', 450, 25, 50, 10, 5, 2),
    (2, 'Bánh mì thịt', 'Main', 'Bread', 'Bánh mì kẹp thịt, chả, rau sống', '1 ổ', 350, 15, 40, 12, 3, 1),
    (3, 'Cơm tấm sườn', 'Main', 'Rice', 'Cơm tấm với sườn nướng', '1 đĩa', 600, 30, 70, 20, 10, 3),
    (4, 'Bún chả', 'Main', 'Noodle', 'Bún với thịt nướng và nước mắm', '1 tô', 400, 20, 50, 15, 5, 2),
    (5, 'Gỏi cuốn', 'Appetizer', 'Roll', 'Gỏi cuốn tôm thịt', '1 cuốn', 100, 5, 10, 3, 1, 1),
    (6, 'Bánh xèo', 'Main', 'Pancake', 'Bánh xèo nhân tôm thịt', '1 cái', 300, 10, 30, 15, 5, 2),
    (7, 'Chả giò', 'Appetizer', 'Fried', 'Chả giò chiên giòn', '1 cái', 150, 5, 10, 8, 2, 1),
    (8, 'Canh chua cá lóc', 'Soup', 'Soup', 'Canh chua với cá lóc', '1 tô', 200, 15, 10, 5, 2, 1),
    (9, 'Bún riêu', 'Main', 'Noodle', 'Bún riêu cua', '1 tô', 350, 20, 40, 10, 5, 2),
    (10, 'Cá kho tộ', 'Main', 'Fish', 'Cá kho tộ với nước dừa', '1 đĩa', 400, 25, 10, 20, 5, 1),
    (11, 'Bánh cuốn', 'Main', 'Roll', 'Bánh cuốn nhân thịt', '1 đĩa', 300, 10, 40, 8, 3, 2),
    (12, 'Chè đậu đen', 'Dessert', 'Dessert', 'Chè đậu đen nước cốt dừa', '1 chén', 200, 5, 30, 5, 2, 3),
    (13, 'Bánh bèo', 'Snack', 'Cake', 'Bánh bèo nhân tôm', '1 đĩa', 250, 8, 30, 10, 3, 1),
    (14, 'Bánh ướt', 'Snack', 'Roll', 'Bánh ướt cuốn thịt', '1 đĩa', 200, 10, 25, 5, 2, 1),
    (15, 'Bánh canh cua', 'Main', 'Noodle', 'Bánh canh với cua', '1 tô', 400, 20, 40, 15, 5, 2),
    (16, 'Bánh tét', 'Main', 'Rice', 'Bánh tét nhân đậu xanh', '1 lát', 300, 10, 40, 10, 5, 2),
    (17, 'Bánh chưng', 'Main', 'Rice', 'Bánh chưng truyền thống', '1 miếng', 350, 15, 45, 12, 5, 3),
    (18, 'Bánh đúc', 'Snack', 'Cake', 'Bánh đúc nóng', '1 đĩa', 150, 5, 20, 5, 2, 1),
    (19, 'Bánh khọt', 'Snack', 'Cake', 'Bánh khọt nhân tôm', '1 đĩa', 200, 10, 20, 10, 3, 1),
    (20, 'Bánh tráng trộn', 'Snack', 'Snack', 'Bánh tráng trộn đặc biệt', '1 đĩa', 250, 5, 30, 10, 3, 2);
SET IDENTITY_INSERT Food OFF;

-- Thêm nguyên liệu cho các món ăn
INSERT INTO Ingredient (IngredientName, Category, Unit, Calories, FoodID)
VALUES
    -- Nguyên liệu cho Phở bò (FoodID = 1)
    ('Bánh phở', 'Noodle', 'gram', 143, 1),
    ('Thịt bò', 'Meat', 'gram', 118, 1),
    ('Hành tây', 'Vegetable', 'gram', 40, 1),
    ('Gừng', 'Spice', 'gram', 10, 1),
    ('Nước dùng', 'Broth', 'ml', 50, 1),

    -- Nguyên liệu cho Bánh mì thịt (FoodID = 2)
    ('Bánh mì', 'Bread', 'gram', 249, 2),
    ('Thịt nguội', 'Meat', 'gram', 118, 2),
    ('Chả lụa', 'Meat', 'gram', 136, 2),
    ('Rau sống', 'Vegetable', 'gram', 20, 2),
    ('Dưa leo', 'Vegetable', 'gram', 12, 2),

    -- Nguyên liệu cho Cơm tấm sườn (FoodID = 3)
    ('Cơm tấm', 'Rice', 'gram', 344, 3),
    ('Sườn heo', 'Meat', 'gram', 260, 3),
    ('Nước mắm', 'Sauce', 'ml', 20, 3),
    ('Dưa leo', 'Vegetable', 'gram', 12, 3),
    ('Đồ chua', 'Vegetable', 'gram', 20, 3),

    -- Nguyên liệu cho Bún chả (FoodID = 4)
    ('Bún', 'Noodle', 'gram', 110, 4),
    ('Thịt heo nướng', 'Meat', 'gram', 260, 4),
    ('Nước mắm', 'Sauce', 'ml', 20, 4),
    ('Rau sống', 'Vegetable', 'gram', 20, 4),
    ('Đồ chua', 'Vegetable', 'gram', 20, 4),

    -- Nguyên liệu cho Gỏi cuốn (FoodID = 5)
    ('Bánh tráng', 'Wrapper', 'gram', 333, 5),
    ('Tôm', 'Seafood', 'gram', 82, 5),
    ('Thịt heo', 'Meat', 'gram', 260, 5),
    ('Rau sống', 'Vegetable', 'gram', 20, 5),
    ('Bún', 'Noodle', 'gram', 110, 5),

    -- Nguyên liệu cho Bánh xèo (FoodID = 6)
    ('Bột gạo', 'Flour', 'gram', 359, 6),
    ('Tôm', 'Seafood', 'gram', 82, 6),
    ('Thịt heo', 'Meat', 'gram', 260, 6),
    ('Giá đỗ', 'Vegetable', 'gram', 44, 6),
    ('Hành lá', 'Vegetable', 'gram', 22, 6),

    -- Nguyên liệu cho Chả giò (FoodID = 7)
    ('Bánh tráng', 'Wrapper', 'gram', 333, 7),
    ('Thịt heo', 'Meat', 'gram', 260, 7),
    ('Tôm', 'Seafood', 'gram', 82, 7),
    ('Cà rốt', 'Vegetable', 'gram', 39, 7),
    ('Nấm mèo', 'Vegetable', 'gram', 304, 7),

    -- Nguyên liệu cho Canh chua cá lóc (FoodID = 8)
    ('Cá lóc', 'Fish', 'gram', 97, 8),
    ('Cà chua', 'Vegetable', 'gram', 20, 8),
    ('Dứa', 'Fruit', 'gram', 50, 8),
    ('Giá đỗ', 'Vegetable', 'gram', 44, 8),
    ('Hành lá', 'Vegetable', 'gram', 22, 8),

    -- Nguyên liệu cho Bún riêu (FoodID = 9)
    ('Bún', 'Noodle', 'gram', 110, 9),
    ('Cua đồng', 'Seafood', 'gram', 87, 9),
    ('Cà chua', 'Vegetable', 'gram', 20, 9),
    ('Đậu hũ', 'Legume', 'gram', 95, 9),
    ('Hành lá', 'Vegetable', 'gram', 22, 9),

    -- Nguyên liệu cho Cá kho tộ (FoodID = 10)
    ('Cá trê', 'Fish', 'gram', 173, 10),
    ('Nước mắm', 'Sauce', 'ml', 20, 10),
    ('Đường', 'Sweetener', 'gram', 397, 10),
    ('Hành tím', 'Vegetable', 'gram', 118, 10),
    ('Ớt', 'Spice', 'gram', 10, 10),

    -- Nguyên liệu cho Bánh cuốn (FoodID = 11)
    ('Bột gạo', 'Flour', 'gram', 359, 11),
    ('Thịt heo', 'Meat', 'gram', 260, 11),
    ('Hành khô', 'Vegetable', 'gram', 118, 11),
    ('Nấm mèo', 'Vegetable', 'gram', 304, 11),
    ('Hành lá', 'Vegetable', 'gram', 22, 11),

    -- Nguyên liệu cho Chè đậu đen (FoodID = 12)
    ('Đậu đen', 'Legume', 'gram', 325, 12),
    ('Đường', 'Sweetener', 'gram', 397, 12),
    ('Nước cốt dừa', 'Dairy', 'ml', 336, 12),
    ('Gừng', 'Spice', 'gram', 10, 12),

    -- Nguyên liệu cho Bánh bèo (FoodID = 13)
    ('Bột gạo', 'Flour', 'gram', 359, 13),
    ('Tôm', 'Seafood', 'gram', 82, 13),
    ('Hành lá', 'Vegetable', 'gram', 22, 13),
    ('Nước mắm', 'Sauce', 'ml', 20, 13),

    -- Nguyên liệu cho Bánh ướt (FoodID = 14)
    ('Bột gạo', 'Flour', 'gram', 359, 14),
    ('Thịt heo', 'Meat', 'gram', 260, 14),
    ('Hành lá', 'Vegetable', 'gram', 22, 14),
    ('Nước mắm', 'Sauce', 'ml', 20, 14),

    -- Nguyên liệu cho Bánh canh cua (FoodID = 15)
    ('Bánh canh', 'Noodle', 'gram', 110, 15),
    ('Cua đồng', 'Seafood', 'gram', 87, 15),
    ('Hành lá', 'Vegetable', 'gram', 22, 15),
    ('Nước mắm', 'Sauce', 'ml', 20, 15),

    -- Nguyên liệu cho Bánh tét (FoodID = 16)
    ('Gạo nếp', 'Rice', 'gram', 346, 16),
    ('Đậu xanh', 'Legume', 'gram', 328, 16),
    ('Thịt heo', 'Meat', 'gram', 260, 16),
    ('Lá chuối', 'Wrapper', 'gram', 10, 16),

    -- Nguyên liệu cho Bánh chưng (FoodID = 17)
    ('Gạo nếp', 'Rice', 'gram', 346, 17),
    ('Đậu xanh', 'Legume', 'gram', 328, 17),
    ('Thịt heo', 'Meat', 'gram', 260, 17),
    ('Lá dong', 'Wrapper', 'gram', 10, 17),

    -- Nguyên liệu cho Bánh đúc (FoodID = 18)
    ('Bột gạo', 'Flour', 'gram', 359, 18),
    ('Nước cốt dừa', 'Dairy', 'ml', 336, 18),
    ('Đường', 'Sweetener', 'gram', 397, 18),

    -- Nguyên liệu cho Bánh khọt (FoodID = 19)
    ('Bột gạo', 'Flour', 'gram', 359, 19),
    ('Tôm', 'Seafood', 'gram', 82, 19),
    ('Hành lá', 'Vegetable', 'gram', 22, 19),
    ('Nước mắm', 'Sauce', 'ml', 20, 19),

    -- Nguyên liệu cho Bánh tráng trộn (FoodID = 20)
    ('Bánh tráng', 'Wrapper', 'gram', 333, 20),
    ('Trứng gà', 'Egg', 'gram', 166, 20),
    ('Hành lá', 'Vegetable', 'gram', 22, 20),
    ('Nước mắm', 'Sauce', 'ml', 20, 20);
