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

-- Bật IDENTITY_INSERT cho bảng Allergy
SET IDENTITY_INSERT [dbo].[Allergy] ON;
GO

-- Chèn dữ liệu vào bảng Allergy với IDENTITY_INSERT được bật
INSERT INTO Allergy (AllergyID, AllergyName, Notes, CreatedAt, UpdatedAt) VALUES
(1, N'Dị ứng đậu phộng', N'Dễ gây phản ứng nghiêm trọng như sốc phản vệ', GETDATE(), GETDATE()),
(2, N'Dị ứng hải sản', N'Bao gồm tôm, cua, sò, ốc, hến', GETDATE(), GETDATE()),
(3, N'Dị ứng sữa', N'Không dung nạp lactose hoặc dị ứng protein sữa', GETDATE(), GETDATE()),
(4, N'Dị ứng trứng', N'Thường gặp ở trẻ nhỏ, đặc biệt là lòng trắng trứng', GETDATE(), GETDATE()),
(5, N'Dị ứng gluten', N'Liên quan đến bệnh Celiac hoặc không dung nạp gluten', GETDATE(), GETDATE()),
(6, N'Dị ứng đậu nành', N'Phản ứng với protein trong đậu nành', GETDATE(), GETDATE()),
(7, N'Dị ứng hạt cây', N'Bao gồm hạt điều, hạnh nhân, óc chó', GETDATE(), GETDATE()),
(8, N'Dị ứng mè (vừng)', N'Gây phát ban, khó thở hoặc sốc phản vệ', GETDATE(), GETDATE()),
(9, N'Dị ứng lúa mì', N'Không giống như không dung nạp gluten, có thể gây sốc phản vệ', GETDATE(), GETDATE()),
(10, N'Dị ứng động vật có vỏ', N'Tôm, cua, sò thường gây dị ứng mạnh', GETDATE(), GETDATE()),
(11, N'Dị ứng cá', N'Dị ứng với các loại cá như cá hồi, cá ngừ, cá thu', GETDATE(), GETDATE()),
(12, N'Dị ứng mốc', N'Gây các vấn đề về hô hấp và da', GETDATE(), GETDATE()),
(13, N'Dị ứng bụi nhà', N'Gây viêm mũi dị ứng và hen suyễn', GETDATE(), GETDATE()),
(14, N'Dị ứng phấn hoa', N'Nguyên nhân phổ biến gây hắt hơi, chảy nước mũi', GETDATE(), GETDATE()),
(15, N'Dị ứng thuốc kháng sinh', N'Penicillin là tác nhân phổ biến nhất', GETDATE(), GETDATE()),
(16, N'Dị ứng côn trùng chích', N'Ong, kiến lửa có thể gây sốc phản vệ', GETDATE(), GETDATE()),
(17, N'Dị ứng nấm men', N'Liên quan đến thực phẩm lên men như bia, rượu, bánh mì', GETDATE(), GETDATE()),
(18, N'Dị ứng trái cây có múi', N'Cam, chanh, bưởi có thể gây kích ứng da hoặc miệng', GETDATE(), GETDATE()),
(19, N'Dị ứng socola', N'Dị ứng với cacao hoặc các chất phụ gia trong socola', GETDATE(), GETDATE()),
(20, N'Dị ứng sulfite', N'Chất bảo quản trong rượu vang, trái cây sấy khô', GETDATE(), GETDATE());

-- Tắt IDENTITY_INSERT cho bảng Allergy
SET IDENTITY_INSERT [dbo].[Allergy] OFF;
GO

-- Bật IDENTITY_INSERT cho bảng Disease
SET IDENTITY_INSERT [dbo].[Disease] ON;
GO

-- Chèn dữ liệu vào bảng Disease với IDENTITY_INSERT được bật
INSERT INTO Disease (DiseaseID, DiseaseName, Description, CreatedAt, UpdatedAt) VALUES
(1, N'Bệnh tiểu đường', N'Yêu cầu chế độ ăn ít đường và tinh bột', GETDATE(), GETDATE()),
(2, N'Bệnh tim mạch', N'Chế độ ăn ít cholesterol và muối', GETDATE(), GETDATE()),
(3, N'Bệnh cao huyết áp', N'Giảm muối, hạn chế thực phẩm chế biến sẵn', GETDATE(), GETDATE()),
(4, N'Bệnh thận', N'Hạn chế protein và muối', GETDATE(), GETDATE()),
(5, N'Bệnh gout', N'Giảm thực phẩm giàu purine như hải sản, thịt đỏ', GETDATE(), GETDATE()),
(6, N'Không dung nạp lactose', N'Tránh sữa và sản phẩm từ sữa', GETDATE(), GETDATE()),
(7, N'Hội chứng ruột kích thích', N'Tránh thực phẩm gây kích thích như cafein, đồ cay', GETDATE(), GETDATE()),
(8, N'Béo phì', N'Cần kiểm soát calo, tăng cường thực phẩm lành mạnh', GETDATE(), GETDATE()),
(9, N'Suy dinh dưỡng', N'Cần bổ sung vitamin và khoáng chất', GETDATE(), GETDATE()),
(10, N'Dị ứng thực phẩm', N'Tổng hợp nhiều loại dị ứng với thực phẩm', GETDATE(), GETDATE());

-- Tắt IDENTITY_INSERT cho bảng Disease
SET IDENTITY_INSERT [dbo].[Disease] OFF;
GO

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

SET IDENTITY_INSERT Food ON;
INSERT INTO Food (FoodID, FoodName, MealType, FoodType, Description, ServingSize, Calories, Protein, Carbs, Fat, Glucid, Fiber)
VALUES
    (1, N'Phở bò', 'Main', 'Noodle', N'Phở truyền thống với thịt bò', '1 tô', 450, 25, 50, 10, 5, 2),
    (2, N'Bánh mì thịt', 'Main', 'Bread', N'Bánh mì kẹp thịt, chả, rau sống', '1 ổ', 350, 15, 40, 12, 3, 1),
    (3, N'Cơm tấm sườn', 'Main', 'Rice', N'Cơm tấm với sườn nướng', '1 đĩa', 600, 30, 70, 20, 10, 3),
    (4, N'Bún chả', 'Main', 'Noodle', N'Bún với thịt nướng và nước mắm', '1 tô', 400, 20, 50, 15, 5, 2),
    (5, N'Gỏi cuốn', 'Appetizer', 'Roll', N'Gỏi cuốn tôm thịt', '1 cuốn', 100, 5, 10, 3, 1, 1),
    (6, N'Bánh xèo', 'Main', 'Pancake', N'Bánh xèo nhân tôm thịt', '1 cái', 300, 10, 30, 15, 5, 2),
    (7, N'Chả giò', 'Appetizer', 'Fried', N'Chả giò chiên giòn', '1 cái', 150, 5, 10, 8, 2, 1),
    (8, N'Canh chua cá lóc', 'Soup', 'Soup', N'Canh chua với cá lóc', '1 tô', 200, 15, 10, 5, 2, 1),
    (9, N'Bún riêu', 'Main', 'Noodle', N'Bún riêu cua', '1 tô', 350, 20, 40, 10, 5, 2),
    (10, N'Cá kho tộ', 'Main', 'Fish', N'Cá kho tộ với nước dừa', '1 đĩa', 400, 25, 10, 20, 5, 1),
    (11, N'Bánh cuốn', 'Main', 'Roll', N'Bánh cuốn nhân thịt', '1 đĩa', 300, 10, 40, 8, 3, 2),
    (12, N'Chè đậu đen', 'Dessert', 'Dessert', N'Chè đậu đen nước cốt dừa', '1 chén', 200, 5, 30, 5, 2, 3),
    (13, N'Bánh bèo', 'Snack', 'Cake', N'Bánh bèo nhân tôm', '1 đĩa', 250, 8, 30, 10, 3, 1),
    (14, N'Bánh ướt', 'Snack', 'Roll', N'Bánh ướt cuốn thịt', '1 đĩa', 200, 10, 25, 5, 2, 1),
    (15, N'Bánh canh cua', 'Main', 'Noodle', N'Bánh canh với cua', '1 tô', 400, 20, 40, 15, 5, 2),
    (16, N'Bánh tét', 'Main', 'Rice', N'Bánh tét nhân đậu xanh', '1 lát', 300, 10, 40, 10, 5, 2),
    (17, N'Bánh chưng', 'Main', 'Rice', N'Bánh chưng truyền thống', '1 miếng', 350, 15, 45, 12, 5, 3),
    (18, N'Bánh đúc', 'Snack', 'Cake', N'Bánh đúc nóng', '1 đĩa', 150, 5, 20, 5, 2, 1),
    (19, N'Bánh khọt', 'Snack', 'Cake', N'Bánh khọt nhân tôm', '1 đĩa', 200, 10, 20, 10, 3, 1),
    (20, N'Bánh tráng trộn', 'Snack', 'Snack', N'Bánh tráng trộn đặc biệt', '1 đĩa', 250, 5, 30, 10, 3, 2);
SET IDENTITY_INSERT Food OFF;

-- Thêm nguyên liệu cho các món ăn
INSERT INTO Ingredient (IngredientName, Category, Unit, Calories, FoodID)
VALUES
    -- Nguyên liệu cho Phở bò (FoodID = 1)
    (N'Bánh phở', 'Noodle', 'gram', 143, 1),
    (N'Thịt bò', 'Meat', 'gram', 118, 1),
    (N'Hành tây', 'Vegetable', 'gram', 40, 1),
    (N'Gừng', 'Spice', 'gram', 10, 1),
    (N'Nước dùng', 'Broth', 'ml', 50, 1),

    -- Nguyên liệu cho Bánh mì thịt (FoodID = 2)
    (N'Bánh mì', 'Bread', 'gram', 249, 2),
    (N'Thịt nguội', 'Meat', 'gram', 118, 2),
    (N'Chả lụa', 'Meat', 'gram', 136, 2),
    (N'Rau sống', 'Vegetable', 'gram', 20, 2),
    (N'Dưa leo', 'Vegetable', 'gram', 12, 2),

    -- Nguyên liệu cho Cơm tấm sườn (FoodID = 3)
    (N'Cơm tấm', 'Rice', 'gram', 344, 3),
    (N'Sườn heo', 'Meat', 'gram', 260, 3),
    (N'Nước mắm', 'Sauce', 'ml', 20, 3),
    (N'Dưa leo', 'Vegetable', 'gram', 12, 3),
    (N'Đồ chua', 'Vegetable', 'gram', 20, 3),

    -- Nguyên liệu cho Bún chả (FoodID = 4)
    (N'Bún', 'Noodle', 'gram', 110, 4),
    (N'Thịt heo nướng', 'Meat', 'gram', 260, 4),
    (N'Nước mắm', 'Sauce', 'ml', 20, 4),
    (N'Rau sống', 'Vegetable', 'gram', 20, 4),
    (N'Đồ chua', 'Vegetable', 'gram', 20, 4),

    -- Nguyên liệu cho Gỏi cuốn (FoodID = 5)
    (N'Bánh tráng', 'Wrapper', 'gram', 333, 5),
    (N'Tôm', 'Seafood', 'gram', 82, 5),
    (N'Thịt heo', 'Meat', 'gram', 260, 5),
    (N'Rau sống', 'Vegetable', 'gram', 20, 5),
    (N'Bún', 'Noodle', 'gram', 110, 5),

    -- Nguyên liệu cho Bánh xèo (FoodID = 6)
    (N'Bột gạo', 'Flour', 'gram', 359, 6),
    (N'Tôm', 'Seafood', 'gram', 82, 6),
    (N'Thịt heo', 'Meat', 'gram', 260, 6),
    (N'Giá đỗ', 'Vegetable', 'gram', 44, 6),
    (N'Hành lá', 'Vegetable', 'gram', 22, 6),

    -- Nguyên liệu cho Chả giò (FoodID = 7)
    (N'Bánh tráng', 'Wrapper', 'gram', 333, 7),
    (N'Thịt heo', 'Meat', 'gram', 260, 7),
    (N'Tôm', 'Seafood', 'gram', 82, 7),
    (N'Cà rốt', 'Vegetable', 'gram', 39, 7),
    (N'Nấm mèo', 'Vegetable', 'gram', 304, 7),

    -- Nguyên liệu cho Canh chua cá lóc (FoodID = 8)
    (N'Cá lóc', 'Fish', 'gram', 97, 8),
    (N'Cà chua', 'Vegetable', 'gram', 20, 8),
    (N'Dứa', 'Fruit', 'gram', 50, 8),
    (N'Giá đỗ', 'Vegetable', 'gram', 44, 8),
    (N'Hành lá', 'Vegetable', 'gram', 22, 8),

    -- Nguyên liệu cho Bún riêu (FoodID = 9)
    (N'Bún', 'Noodle', 'gram', 110, 9),
    (N'Cua đồng', 'Seafood', 'gram', 87, 9),
    (N'Cà chua', 'Vegetable', 'gram', 20, 9),
    (N'Đậu hũ', 'Legume', 'gram', 95, 9),
    (N'Hành lá', 'Vegetable', 'gram', 22, 9),

    -- Nguyên liệu cho Cá kho tộ (FoodID = 10)
    (N'Cá trê', 'Fish', 'gram', 173, 10),
    (N'Nước mắm', 'Sauce', 'ml', 20, 10),
    (N'Đường', 'Sweetener', 'gram', 397, 10),
    (N'Hành tím', 'Vegetable', 'gram', 118, 10),
    (N'Ớt', 'Spice', 'gram', 10, 10),

    -- Nguyên liệu cho Bánh cuốn (FoodID = 11)
    (N'Bột gạo', 'Flour', 'gram', 359, 11),
    (N'Thịt heo', 'Meat', 'gram', 260, 11),
    (N'Hành khô', 'Vegetable', 'gram', 118, 11),
    (N'Nấm mèo', 'Vegetable', 'gram', 304, 11),
    (N'Hành lá', 'Vegetable', 'gram', 22, 11),

    -- Nguyên liệu cho Chè đậu đen (FoodID = 12)
    (N'Đậu đen', 'Legume', 'gram', 325, 12),
    (N'Đường', 'Sweetener', 'gram', 397, 12),
    (N'Nước cốt dừa', 'Dairy', 'ml', 336, 12),
    (N'Gừng', 'Spice', 'gram', 10, 12),

    -- Nguyên liệu cho Bánh bèo (FoodID = 13)
    (N'Bột gạo', 'Flour', 'gram', 359, 13),
    (N'Tôm', 'Seafood', 'gram', 82, 13),
    (N'Hành lá', 'Vegetable', 'gram', 22, 13),
    (N'Nước mắm', 'Sauce', 'ml', 20, 13),

    -- Nguyên liệu cho Bánh ướt (FoodID = 14)
    (N'Bột gạo', 'Flour', 'gram', 359, 14),
    (N'Thịt heo', 'Meat', 'gram', 260, 14),
    (N'Hành lá', 'Vegetable', 'gram', 22, 14),
    (N'Nước mắm', 'Sauce', 'ml', 20, 14),

    -- Nguyên liệu cho Bánh canh cua (FoodID = 15)
    (N'Bánh canh', 'Noodle', 'gram', 110, 15),
    (N'Cua đồng', 'Seafood', 'gram', 87, 15),
    (N'Hành lá', 'Vegetable', 'gram', 22, 15),
    (N'Nước mắm', 'Sauce', 'ml', 20, 15),

    -- Nguyên liệu cho Bánh tét (FoodID = 16)
    (N'Gạo nếp', 'Rice', 'gram', 346, 16),
    (N'Đậu xanh', 'Legume', 'gram', 328, 16),
    (N'Thịt heo', 'Meat', 'gram', 260, 16),
    (N'Lá chuối', 'Wrapper', 'gram', 10, 16),

    -- Nguyên liệu cho Bánh chưng (FoodID = 17)
    (N'Gạo nếp', 'Rice', 'gram', 346, 17),
    (N'Đậu xanh', 'Legume', 'gram', 328, 17),
    (N'Thịt heo', 'Meat', 'gram', 260, 17),
    (N'Lá dong', 'Wrapper', 'gram', 10, 17),

    -- Nguyên liệu cho Bánh đúc (FoodID = 18)
    (N'Bột gạo', 'Flour', 'gram', 359, 18),
    (N'Nước cốt dừa', 'Dairy', 'ml', 336, 18),
    (N'Đường', 'Sweetener', 'gram', 397, 18),

    -- Nguyên liệu cho Bánh khọt (FoodID = 19)
    (N'Bột gạo', 'Flour', 'gram', 359, 19),
    (N'Tôm', 'Seafood', 'gram', 82, 19),
    (N'Hành lá', 'Vegetable', 'gram', 22, 19),
    (N'Nước mắm', 'Sauce', 'ml', 20, 19),

    -- Nguyên liệu cho Bánh tráng trộn (FoodID = 20)
    (N'Bánh tráng', 'Wrapper', 'gram', 333, 20),
    (N'Trứng gà', 'Egg', 'gram', 166, 20),
    (N'Hành lá', 'Vegetable', 'gram', 22, 20),
    (N'Nước mắm', 'Sauce', 'ml', 20, 20);

-- Insert data into FoodAllergy table based on ingredients and common allergies
INSERT INTO FoodAllergy (FoodID, AllergyID) VALUES
-- Phở bò (1) - Contains beef
(1, 2), -- seafood allergy (some broths may contain seafood)

-- Bánh mì thịt (2) - Contains wheat, meat
(2, 9), -- wheat allergy
(2, 3), -- dairy allergy (may contain butter)

-- Cơm tấm sườn (3) - Contains pork
(3, 2), -- seafood allergy (fish sauce)

-- Bún chả (4) - Contains pork, fish sauce
(4, 2), -- seafood allergy (fish sauce)

-- Gỏi cuốn (5) - Contains shrimp, pork
(5, 2), -- seafood allergy
(5, 10), -- shellfish allergy

-- Bánh xèo (6) - Contains shrimp, pork, wheat
(6, 2), -- seafood allergy
(6, 9), -- wheat allergy
(6, 10), -- shellfish allergy

-- Chả giò (7) - Contains pork, shrimp
(7, 2), -- seafood allergy
(7, 10), -- shellfish allergy

-- Canh chua cá lóc (8) - Contains fish
(8, 2), -- seafood allergy
(8, 11), -- fish allergy

-- Bún riêu (9) - Contains crab, shrimp
(9, 2), -- seafood allergy
(9, 10), -- shellfish allergy

-- Cá kho tộ (10) - Contains fish
(10, 2), -- seafood allergy
(10, 11), -- fish allergy

-- Bánh cuốn (11) - Contains pork, wheat
(11, 9), -- wheat allergy

-- Chè đậu đen (12) - Contains beans, coconut milk
(12, 6), -- soy allergy

-- Bánh bèo (13) - Contains shrimp, rice flour
(13, 2), -- seafood allergy
(13, 10), -- shellfish allergy

-- Bánh ướt (14) - Contains rice flour, pork
(14, 2), -- seafood allergy (fish sauce)

-- Bánh canh cua (15) - Contains crab, wheat flour
(15, 2), -- seafood allergy
(15, 9), -- wheat allergy
(15, 10), -- shellfish allergy

-- Bánh tét (16) - Contains pork, mung beans
(16, 6), -- soy allergy

-- Bánh chưng (17) - Contains pork, mung beans
(17, 6), -- soy allergy

-- Bánh đúc (18) - Contains rice flour, coconut milk
(18, 3), -- dairy allergy (coconut milk)

-- Bánh khọt (19) - Contains shrimp, rice flour
(19, 2), -- seafood allergy
(19, 10), -- shellfish allergy

-- Bánh tráng trộn (20) - Contains eggs
(20, 4); -- egg allergy

-- Insert data into FoodDisease table based on nutritional content and disease restrictions
INSERT INTO FoodDisease (FoodID, DiseaseID) VALUES
-- Phở bò (1) - High in sodium, protein
(1, 2), -- heart disease
(1, 3), -- high blood pressure
(1, 4), -- kidney disease

-- Bánh mì thịt (2) - High in carbs
(2, 1), -- diabetes
(2, 8), -- obesity

-- Cơm tấm sườn (3) - High in calories, fat
(3, 1), -- diabetes
(3, 2), -- heart disease
(3, 8), -- obesity

-- Bún chả (4) - High in sodium
(4, 2), -- heart disease
(4, 3), -- high blood pressure

-- Gỏi cuốn (5) - Generally healthy but contains shrimp
(5, 5), -- gout

-- Bánh xèo (6) - High in fat, calories
(6, 1), -- diabetes
(6, 2), -- heart disease
(6, 8), -- obesity

-- Chả giò (7) - High in fat, calories
(7, 1), -- diabetes
(7, 2), -- heart disease
(7, 8), -- obesity

-- Canh chua cá lóc (8) - Generally healthy
(8, 5), -- gout (fish)

-- Bún riêu (9) - High in sodium, contains seafood
(9, 3), -- high blood pressure
(9, 5), -- gout

-- Cá kho tộ (10) - High in sodium
(10, 2), -- heart disease
(10, 3), -- high blood pressure
(10, 5), -- gout

-- Bánh cuốn (11) - High in carbs
(11, 1), -- diabetes
(11, 8), -- obesity

-- Chè đậu đen (12) - High in sugar
(12, 1), -- diabetes
(12, 8), -- obesity

-- Bánh bèo (13) - Contains shrimp
(13, 5), -- gout

-- Bánh ướt (14) - High in carbs
(14, 1), -- diabetes
(14, 8), -- obesity

-- Bánh canh cua (15) - High in sodium, contains seafood
(15, 3), -- high blood pressure
(15, 5), -- gout

-- Bánh tét (16) - High in carbs, calories
(16, 1), -- diabetes
(16, 8), -- obesity

-- Bánh chưng (17) - High in carbs, calories
(17, 1), -- diabetes
(17, 8), -- obesity

-- Bánh đúc (18) - High in carbs
(18, 1), -- diabetes

-- Bánh khọt (19) - High in fat, contains seafood
(19, 2), -- heart disease
(19, 5), -- gout
(19, 8), -- obesity

-- Bánh tráng trộn (20) - Generally lower in concerns
(20, 7); -- IBS (spicy ingredients)