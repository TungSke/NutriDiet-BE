USE NutriDiet
GO

-- Tắt ràng buộc khóa ngoại tạm thời
ALTER TABLE UserAllergy NOCHECK CONSTRAINT ALL;
ALTER TABLE UserDisease NOCHECK CONSTRAINT ALL;
ALTER TABLE UserPackage NOCHECK CONSTRAINT ALL;
ALTER TABLE GeneralHealthProfile NOCHECK CONSTRAINT ALL;
ALTER TABLE RecipeSuggestion NOCHECK CONSTRAINT ALL;
ALTER TABLE UserFoodPreference NOCHECK CONSTRAINT ALL;
ALTER TABLE MealPlanDetail NOCHECK CONSTRAINT ALL;
ALTER TABLE FoodSubstitution NOCHECK CONSTRAINT ALL;
ALTER TABLE Notification NOCHECK CONSTRAINT ALL;
ALTER TABLE HealthcareIndicator NOCHECK CONSTRAINT ALL;
ALTER TABLE AIRecommendation NOCHECK CONSTRAINT ALL;
ALTER TABLE MealLogDetail NOCHECK CONSTRAINT ALL;
ALTER TABLE MealLog NOCHECK CONSTRAINT ALL;
ALTER TABLE PersonalGoal NOCHECK CONSTRAINT ALL;

-- Xóa toàn bộ dữ liệu trong các bảng
DELETE FROM UserAllergy;
DELETE FROM UserDisease;
DELETE FROM UserPackage;
DELETE FROM GeneralHealthProfile;
DELETE FROM RecipeSuggestion;
DELETE FROM UserFoodPreference;
DELETE FROM MealPlanDetail;
DELETE FROM MealPlan;
DELETE FROM FoodSubstitution;
DELETE FROM Notification;
DELETE FROM HealthcareIndicator;
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
DELETE FROM CuisineType;

-- Bật lại ràng buộc khóa ngoại
ALTER TABLE UserAllergy CHECK CONSTRAINT ALL;
ALTER TABLE UserDisease CHECK CONSTRAINT ALL;
ALTER TABLE UserPackage CHECK CONSTRAINT ALL;
ALTER TABLE GeneralHealthProfile CHECK CONSTRAINT ALL;
ALTER TABLE RecipeSuggestion CHECK CONSTRAINT ALL;
ALTER TABLE UserFoodPreference CHECK CONSTRAINT ALL;
ALTER TABLE MealPlanDetail CHECK CONSTRAINT ALL;
ALTER TABLE FoodSubstitution CHECK CONSTRAINT ALL;
ALTER TABLE Notification CHECK CONSTRAINT ALL;
ALTER TABLE HealthcareIndicator CHECK CONSTRAINT ALL;
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
INSERT [dbo].[User] ([UserID], [FullName], [Email], [Password], [Phone], [Age], [Gender], [Location], [Avatar], [fcmToken], [Status], [RoleID]) VALUES (1, N'New User', N'user@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '21', 'Male', 'VietNam', N'', NULL, N'Active', 2)
INSERT [dbo].[User] ([UserID], [FullName], [Email], [Password], [Phone], [Age], [Gender], [Location], [Avatar], [fcmToken], [Status], [RoleID]) VALUES (2, N'Admin', N'admin@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, NULL, NULL, NULL, N'', NULL, N'Active', 1)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO

-- Bật IDENTITY_INSERT cho GeneralHealthProfile
SET IDENTITY_INSERT [dbo].[GeneralHealthProfile] ON;

INSERT INTO [dbo].[GeneralHealthProfile] ([ProfileID], [UserID], [Height], [Weight], [ActivityLevel], [Status], [CreatedAt], [UpdatedAt])  
VALUES (1, 1, 170, 70, 'ModeratelyActive', 'Active', GETDATE(), GETDATE());

-- Tắt IDENTITY_INSERT
SET IDENTITY_INSERT [dbo].[GeneralHealthProfile] OFF;


-- Bật IDENTITY_INSERT cho PersonalGoal
SET IDENTITY_INSERT [dbo].[PersonalGoal] ON;

INSERT INTO [dbo].[PersonalGoal] 
    ([GoalID], [UserID], [GoalType], [TargetWeight], [StartDate], [TargetDate], [ProgressRate], [Status], 
	[DailyCalories], [DailyCarb], [DailyFat], [DailyProtein], [CreatedAt], [UpdatedAt], GoalDescription)  
VALUES 
    (1, 1, 'LoseWeight', 65, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 0, 'Active', 1800, 150, 50, 120, GETDATE(), GETDATE(), 'LoseWeight in 3 months');
SET IDENTITY_INSERT [dbo].[PersonalGoal] OFF;

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
INSERT INTO Food (FoodID, FoodName, MealType, FoodType, Description, ServingSize, Calories, Protein, Carbs, Fat, Glucid, Fiber, ImageUrl)
VALUES
    (1, N'Phở bò', 'Main', 'Noodle', N'Phở truyền thống với thịt bò', N'1 tô', 450, 25, 50, 10, 5, 2,'https://fohlafood.vn/cdn/shop/articles/bi-quyet-nau-phi-bo-ngon-tuyet-dinh.jpg?v=1712213789'),
    (2, N'Bánh mì thịt', 'Main', 'Bread', N'Bánh mì kẹp thịt, chả, rau sống', N'1 ổ', 350, 15, 40, 12, 3, 1,'https://cdn.tgdd.vn/2021/05/CookRecipe/Avatar/banh-mi-thit-bo-nuong-thumbnail-1.jpg'),
    (3, N'Cơm tấm sườn', 'Main', 'Rice', N'Cơm tấm với sườn nướng', N'1 đĩa', 600, 30, 70, 20, 10, 3,'https://i.ytimg.com/vi/OVb5uoDWspM/maxresdefault.jpg'),
    (4, N'Bún chả', 'Main', 'Noodle', N'Bún với thịt nướng và nước mắm', N'1 tô', 400, 20, 50, 15, 5, 2,'https://cdn.tgdd.vn/2022/01/CookDishThumb/huong-dan-cach-lam-bun-cha-ha-noi-thom-ngon-nhu-ngoai-hang-thumb-620x620.jpg'),
    (5, N'Gỏi cuốn', 'Appetizer', 'Roll', N'Gỏi cuốn tôm thịt', N'1 cuốn', 100, 5, 10, 3, 1, 1,'https://upload.wikimedia.org/wikipedia/commons/thumb/0/03/Summer_roll.jpg/800px-Summer_roll.jpg'),
    (6, N'Bánh xèo', 'Main', 'Pancake', N'Bánh xèo nhân tôm thịt', N'1 cái', 300, 10, 30, 15, 5, 2,'https://i-giadinh.vnecdn.net/2023/09/19/Bc10Thnhphm11-1695107510-2493-1695107555.jpg'),
    (7, N'Chả giò', 'Appetizer', 'Fried', N'Chả giò chiên giòn', N'1 cái', 150, 5, 10, 8, 2, 1,'https://assets.unileversolutions.com/recipes-v2/157768.jpg'),
    (8, N'Canh chua cá lóc', 'Soup', 'Soup', N'Canh chua với cá lóc', N'1 tô', 200, 15, 10, 5, 2, 1,'https://i-giadinh.vnecdn.net/2023/04/25/Thanh-pham-1-1-7239-1682395675.jpg'),
    (9, N'Bún riêu', 'Main', 'Noodle', N'Bún riêu cua', N'1 tô', 350, 20, 40, 10, 5, 2,'https://cdn.tgdd.vn/2020/08/CookProduct/Untitled-1-1200x676-10.jpg'),
    (10, N'Cá kho tộ', 'Main', 'Fish', N'Cá kho tộ với nước dừa', N'1 đĩa', 400, 25, 10, 20, 5, 1,'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSEEM3RpIcckZXR_XrXEuyOqZhEXJrFzxg_kQ&s'),
    (11, N'Bánh cuốn', 'Main', 'Roll', N'Bánh cuốn nhân thịt', N'1 đĩa', 300, 10, 40, 8, 3, 2,'https://upload.wikimedia.org/wikipedia/commons/a/a7/Banh_cuon.jpg'),
    (12, N'Chè đậu đen', 'Dessert', 'Dessert', N'Chè đậu đen nước cốt dừa', N'1 chén', 200, 5, 30, 5, 2, 3,'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTZRmTbQ0DL6ZVKhlFJvsieDnVKDgT4dPoj2g&s'),
    (13, N'Bánh bèo', 'Snack', 'Cake', N'Bánh bèo nhân tôm', N'1 đĩa', 250, 8, 30, 10, 3, 1,'https://upload.wikimedia.org/wikipedia/commons/d/d3/B%C3%A1nh_b%C3%A8o.jpg'),
    (14, N'Bánh ướt', 'Snack', 'Roll', N'Bánh ướt cuốn thịt', N'1 đĩa', 200, 10, 25, 5, 2, 1,'https://upload.wikimedia.org/wikipedia/commons/thumb/2/25/Typical_serving_of_B%C3%A1nh_%C6%B0%E1%BB%9Bt.jpg/1200px-Typical_serving_of_B%C3%A1nh_%C6%B0%E1%BB%9Bt.jpg'),
    (15, N'Bánh canh cua', 'Main', 'Noodle', N'Bánh canh với cua', N'1 tô', 400, 20, 40, 15, 5, 2,'https://cdn.tgdd.vn/2021/05/CookProduct/thumbcmscn-1200x676-4.jpg'),
    (16, N'Bánh tét', 'Main', 'Rice', N'Bánh tét nhân đậu xanh', N'1 lát', 300, 10, 40, 10, 5, 2,'https://file.hstatic.net/200000868155/file/1489-post-cach-goi-banh-tet-truyen-thong-nam-bo-dep-don-gian-1.jpg'),
    (17, N'Bánh chưng', 'Main', 'Rice', N'Bánh chưng truyền thống', N'1 miếng', 350, 15, 45, 12, 5, 3,'https://upload.wikimedia.org/wikipedia/commons/thumb/e/ec/Banh_chung_vuong.jpg/1200px-Banh_chung_vuong.jpg'),
    (18, N'Bánh đúc', 'Snack', 'Cake', N'Bánh đúc nóng', N'1 đĩa', 150, 5, 20, 5, 2, 1,'https://i-giadinh.vnecdn.net/2024/11/02/Bc7Thnhphm17-1730530097-3638-1730530219.jpg'),
    (19, N'Bánh khọt', 'Snack', 'Cake', N'Bánh khọt nhân tôm', N'1 đĩa', 200, 10, 20, 10, 3, 1,'https://upload.wikimedia.org/wikipedia/commons/thumb/b/ba/B%C3%A1nh_kh%E1%BB%8Dt_tr%C3%AAn_khu%C3%B4n.jpg/240px-B%C3%A1nh_kh%E1%BB%8Dt_tr%C3%AAn_khu%C3%B4n.jpg'),
    (20, N'Bánh tráng trộn', 'Snack', 'Snack', N'Bánh tráng trộn đặc biệt', N'1 đĩa', 250, 5, 30, 10, 3, 2,'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a6/Mixed_rice_paper.jpg/1200px-Mixed_rice_paper.jpg'),
    (21, N'Salad ức gà', 'Main', 'Salad', N'Salad với ức gà, rau xanh và sốt chanh leo', N'1 dĩa', 300, 35, 20, 8, 5, 4,'https://cdn.tgdd.vn/2021/10/CookRecipe/GalleryStep/thanh-pham-975.jpg'),
    (22, N'Cháo yến mạch', 'Breakfast', 'Porridge', N'Cháo yến mạch với chuối và hạt chia', N'1 chén', 250, 8, 40, 6, 2, 6,'https://cdn.tgdd.vn/Files/2018/11/25/1133505/yen-mach-la-gi-cach-nau-chao-yen-mach-bo-duong-ngon-ngat-ngay-10.jpg'),
    (23, N'Smoothie bơ chuối', 'Snack', 'Smoothie', N'Sinh tố bơ chuối giàu chất xơ và chất béo lành mạnh', N'1 ly', 220, 5, 30, 10, 5, 5,'https://cdn.tgdd.vn/Files/2019/04/22/1162314/dang-xinh-da-dep-voi-sinh-to-bo-chuoi-thom-ngon-202201071430267959.jpg'),
    (24, N'Cá hồi áp chảo', 'Main', 'Fish', N'Cá hồi áp chảo với bông cải xanh', N'1 phần', 400, 40, 10, 20, 2, 3,'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSWX07IGQspfz6npRQhw-rjgBpCBPyf9alEzA&s'),
    (25, N'Súp lơ hấp', 'Side', 'Vegetable', N'Súp lơ hấp với dầu oliu và hạt lanh', N'1 dĩa', 180, 5, 20, 10, 3, 5,'https://giadinh.mediacdn.vn/2021/1/7/hap-rau-16099992712071268362112.jpg'),
    (26, N'Bánh pancake chuối yến mạch', 'Breakfast', 'Cake', N'Bánh pancake từ chuối và yến mạch', N'1 cái', 200, 6, 30, 5, 3, 4,'https://cdn.tgdd.vn/Files/2021/09/28/1386199/huong-dan-lam-banh-pancake-chuoi-yen-mach-thom-ngon-day-dinh-duong-202202211133148108.jpg'),
    (27, N'Trà gừng mật ong', 'Drink', 'Tea', N'Trà gừng mật ong giúp tăng cường miễn dịch', N'1 ly', 100, 0, 25, 0, 5, 1,'https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Ginger_tea.jpg/1200px-Ginger_tea.jpg'),
    (28, N'Xôi gạo lứt', 'Main', 'Rice', N'Xôi gạo lứt ăn kèm đậu phộng và mè đen', N'1 chén', 320, 8, 55, 7, 5, 6,'https://cdn.tgdd.vn/Files/2018/12/28/1141156/cach-nau-xoi-nep-cam-bang-noi-com-dien-don-gian-ma-thom-ngon-tai-nha-10-760x367.jpg'),
    (29, N'Súp bí đỏ', 'Soup', 'Soup', N'Súp bí đỏ với sữa hạnh nhân', N'1 chén', 230, 6, 35, 8, 5, 4,'https://cdn.tgdd.vn/Files/2021/09/10/1381722/huong-dan-lam-sup-bi-do-tai-nha-ngon-nhu-nha-hang-202208311606274453.jpg'),
    (30, N'Salad cá ngừ', 'Main', 'Salad', N'Salad cá ngừ với rau xanh và sốt dầu giấm', N'1 dĩa', 350, 40, 15, 10, 4, 5,'https://cdn.tgdd.vn/2020/07/CookProductThumb/Untitled-1-620x620-364.jpg'),
	(31, N'Lẩu gà lá é', 'Main', 'Hotpot', N'Lẩu gà nấu với lá é, đặc sản Đà Lạt', N'1 nồi', 600, 40, 50, 20, 5, 3, 'https://cdn.tgdd.vn/2021/05/CookRecipe/GalleryStep/thanh-pham-248.jpg'),
    (32, N'Lẩu thái hải sản', 'Main', 'Hotpot', N'Lẩu chua cay với tôm, mực, nghêu', N'1 nồi', 700, 50, 60, 25, 8, 4, 'https://upload.wikimedia.org/wikipedia/commons/thumb/b/be/Hotpot.jpg/640px-Hotpot.jpg'),
    (33, N'Lẩu bò nhúng giấm', 'Main', 'Hotpot', N'Lẩu bò nhúng giấm chấm mắm nêm', N'1 nồi', 750, 60, 40, 30, 5, 3, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR1R-koXWIGx2boTVLFJpN7RMvDVHalEP4Beg&s'),
    (34, N'Bún bò Huế', 'Main', 'Noodle', N'Bún bò đặc sản Huế với chân giò, chả cua', N'1 tô', 480, 30, 55, 12, 6, 2, 'https://upload.wikimedia.org/wikipedia/commons/0/00/Bun-Bo-Hue-from-Huong-Giang-2011.jpg'),
    (35, N'Hủ tiếu Nam Vang', 'Main', 'Noodle', N'Hủ tiếu Nam Vang với tôm, thịt bằm, trứng cút', N'1 tô', 450, 28, 50, 10, 5, 2, 'https://upload.wikimedia.org/wikipedia/commons/thumb/0/04/H%E1%BB%A7_ti%E1%BA%BFu_Nam_Vang.jpg/1280px-H%E1%BB%A7_ti%E1%BA%BFu_Nam_Vang.jpg'),
    (36, N'Bánh đúc mặn', 'Snack', 'Cake', N'Bánh đúc mặn ăn kèm nước mắm và đậu phộng', N'1 dĩa', 250, 8, 30, 8, 3, 1, 'https://upload.wikimedia.org/wikipedia/commons/thumb/0/00/Banhduc-northern.jpg/640px-Banhduc-northern.jpg'),
    (37, N'Mì Quảng', 'Main', 'Noodle', N'Mì Quảng đặc sản Quảng Nam với tôm, thịt, trứng', N'1 tô', 500, 35, 55, 15, 5, 3, 'https://upload.wikimedia.org/wikipedia/commons/d/df/Mi_Quang_1A_Danang.jpg'),
    (38, N'Bánh giò', 'Snack', 'Cake', N'Bánh giò nhân thịt, nấm mèo', N'1 cái', 320, 12, 40, 10, 5, 2, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSnLGO2_Vb49AUNjOLXV_UZOcYd_M_KQMDPIQ&s'),
    (39, N'Xôi xéo', 'Breakfast', 'Rice', N'Xôi xéo với đậu xanh, hành phi, mỡ gà', N'1 chén', 350, 10, 50, 12, 5, 3, 'https://upload.wikimedia.org/wikipedia/commons/f/f9/X%C3%B4i_x%C3%A9o.jpg'),
    (40, N'Cá nướng', 'Main', 'Fish', N'Cá nướng với gia vị thơm ngon', N'1 phần', 400, 35, 10, 20, 5, 3, 'https://upload.wikimedia.org/wikipedia/commons/e/ec/Gurame_bakar_kecap_2.JPG'),
    (41, N'Ốc hương xào bơ tỏi', 'Snack', 'Seafood', N'Ốc hương xào bơ tỏi thơm béo', N'1 dĩa', 380, 25, 15, 18, 4, 2, 'https://cdn.tgdd.vn/Files/2021/08/25/1377750/cach-lam-oc-huong-xao-bo-toi-thom-ngon-don-gian-tai-nha-202108251415105438.jpg'),
    (42, N'Bánh bột lọc', 'Snack', 'Cake', N'Bánh bột lọc nhân tôm thịt', N'1 đĩa', 270, 8, 35, 8, 3, 1, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTk8g6FBCxXxRjs3IZe-r6JIz03ewWQivoDgQ&s'),
    (43, N'Cơm gà', 'Main', 'Rice', N'Cơm gà', N'1 dĩa', 450, 35, 50, 12, 5, 2, 'https://upload.wikimedia.org/wikipedia/commons/3/39/Arroz-con-Pollo.jpg'),
    (44, N'Chè bắp', 'Dessert', 'Dessert', N'Chè bắp nước cốt dừa thơm ngon', N'1 chén', 200, 5, 35, 5, 5, 3, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT5Qqi9lImimEMclmIFQtgRAt2yTIeufoULXA&s'),
    (45, N'Gỏi bò bóp thấu', 'Appetizer', 'Salad', N'Gỏi bò bóp thấu chua ngọt hấp dẫn', N'1 dĩa', 320, 30, 15, 12, 5, 3, 'https://upload.wikimedia.org/wikipedia/commons/9/9f/G%E1%BB%8Fi_%C4%91u_%C4%91%E1%BB%A7_kh%C3%B4_b%C3%B2.jpg'),
    (46, N'Tiết canh vịt', 'Appetizer', 'Soup', N'Tiết canh vịt - món ăn truyền thống', N'1 chén', 250, 20, 5, 8, 3, 2, 'https://upload.wikimedia.org/wikipedia/commons/f/fd/Tiet.jpg'),
    (47, N'Lòng xào dưa', 'Main', 'Meat', N'Lòng non xào dưa chua', N'1 đĩa', 400, 30, 10, 20, 4, 2, 'https://cdn.tgdd.vn/2020/07/CookProduct/Screenshot_7-1200x676.jpg'),
    (48, N'Dê tái chanh', 'Main', 'Meat', N'Dê tái chanh chấm tương gừng', N'1 dĩa', 380, 40, 10, 15, 4, 2, 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/ce/T%C3%A1i_d%C3%AA.jpg/300px-T%C3%A1i_d%C3%AA.jpg'),
    (49, N'Bánh khoai mì nướng', 'Dessert', 'Cake', N'Bánh khoai mì nướng béo thơm, ăn kèm nước cốt dừa', N'1 miếng', 280, 3, 40, 12, 5, 2, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRONh1-lqvwZ1RqJazYGCSbBI1YExkD2XEodQ&s'),
    (50, N'Gỏi ngó sen tôm thịt', 'Appetizer', 'Salad', N'Gỏi ngó sen tôm thịt chua ngọt, ăn kèm bánh phồng tôm', N'1 đĩa', 220, 15, 20, 8, 4, 3, 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Ph%E1%BB%93ng_t%C3%B4m.jpg/640px-Ph%E1%BB%93ng_t%C3%B4m.jpg');

SET IDENTITY_INSERT Food OFF;

-- Insert data into FoodAllergy table based on common allergies
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

INSERT INTO CuisineType (CuisineName) VALUES 
(N'Ẩm thực miền Bắc'),
(N'Ẩm thực miền Trung'),
(N'Ẩm thực miền Nam'),
(N'Ẩm thực Tây Nguyên'),
(N'Ẩm thực Nam Bộ'),
(N'Ẩm thực Trung Hoa');

-- Insert data into MealPlan
-- Thêm 3 MealPlan
SET IDENTITY_INSERT MealPlan ON;

INSERT INTO MealPlan (MealPlanID, UserID, PlanName, HealthGoal, Duration, CreatedBy, UpdatedBy, Status)
VALUES
    (1, 2, N'Kế hoạch ăn uống giảm cân', N'Giảm cân', 7, N'Admin', N'Admin', 'Active'),
    (2, 2, N'Kế hoạch ăn uống tăng cân', N'Tăng cơ bắp', 7, N'Admin', N'Admin', 'Active'),
    (3, 2, N'Kế hoạch ăn uống cân bằng', N'Cân bằng dinh dưỡng', 7, N'Admin', N'Admin', 'Active'),
    (4, 2, N'Kế hoạch ăn uống Healthy', N'Cải thiện sức khỏe', 7, N'Admin', N'Admin', 'Active'),
    (5, 2, N'Kế hoạch ăn uống cho người tiểu đường', N'Cải thiện tiểu đường', 7, N'Admin', N'Admin', 'Active'),
    (6, 2, N'Kế hoạch ăn uống cho người gan nhiễm mỡ', N'Cải thiện gan nhiễm mỡ', 7, N'Admin', N'Admin', 'Active');

SET IDENTITY_INSERT MealPlan OFF;

-- Insert data into MealPlanDetail for 'Giảm cân nhanh'
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    (1, 5, N'Gỏi cuốn', 2, N'Bữa sáng', 1, 200, 20, 6, 10),
    (1, 8, N'Canh chua cá lóc', 1, N'Bữa trưa', 1, 200, 10, 5, 15),
    (1, 14, N'Bánh ướt', 1, N'Bữa tối', 1, 200, 25, 5, 10),
    (1, 7, N'Chả giò', 2, N'Bữa sáng', 2, 300, 20, 10, 10),
    (1, 2, N'Bánh mì thịt', 1, N'Bữa trưa', 2, 350, 40, 12, 15),
    (1, 18, N'Bánh đúc', 1, N'Bữa tối', 2, 150, 20, 5, 5),
    (1, 1, N'Phở bò', 1, N'Bữa sáng', 3, 450, 50, 10, 25),
    (1, 6, N'Bánh xèo', 1, N'Bữa trưa', 3, 300, 30, 15, 10),
    (1, 12, N'Chè đậu đen', 1, N'Bữa tối', 3, 200, 30, 5, 5),
    (1, 9, N'Bún riêu', 1, N'Bữa sáng', 4, 350, 40, 10, 20),
    (1, 10, N'Cá kho tộ', 1, N'Bữa trưa', 4, 400, 10, 20, 25),
    (1, 15, N'Bánh canh cua', 1, N'Bữa tối', 4, 400, 40, 15, 20),
    (1, 3, N'Cơm tấm sườn', 1, N'Bữa sáng', 5, 600, 70, 20, 30),
    (1, 11, N'Bánh cuốn', 1, N'Bữa trưa', 5, 300, 40, 8, 10),
    (1, 20, N'Bánh tráng trộn', 1, N'Bữa tối', 5, 250, 30, 10, 5),
    (1, 16, N'Bánh tét', 1, N'Bữa sáng', 6, 300, 40, 10, 10),
    (1, 19, N'Bánh khọt', 1, N'Bữa trưa', 6, 200, 20, 10, 10),
    (1, 13, N'Bánh bèo', 1, N'Bữa tối', 6, 250, 30, 10, 8),
    (1, 4, N'Cháo lòng', 1, N'Bữa sáng', 7, 350, 35, 12, 20),
    (1, 17, N'Bánh chưng', 1, N'Bữa trưa', 7, 350, 45, 12, 15),
    (1, 21, N'Bánh khoai mì', 1, N'Bữa tối', 7, 300, 40, 15, 10);

-- Insert data into MealPlanDetail for 'Tăng cơ bắp'
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (2, 10, N'Cá kho tộ', 1, N'Bữa sáng', 1, 400, 10, 20, 25),
    (2, 3, N'Cơm tấm sườn', 1, N'Bữa trưa', 1, 600, 70, 20, 30),
    (2, 15, N'Bánh canh cua', 1, N'Bữa tối', 1, 400, 40, 15, 20),
    
    -- Ngày 2
    (2, 9, N'Bún riêu', 1, N'Bữa sáng', 2, 350, 40, 10, 20),
    (2, 16, N'Bánh tét', 1, N'Bữa trưa', 2, 300, 40, 10, 10),
    (2, 19, N'Bánh khọt', 1, N'Bữa tối', 2, 200, 20, 10, 10),

    -- Ngày 3
    (2, 11, N'Bánh cuốn', 1, N'Bữa sáng', 3, 300, 40, 8, 10),
    (2, 6, N'Bánh xèo', 1, N'Bữa trưa', 3, 300, 30, 15, 10),
    (2, 13, N'Bánh bèo', 1, N'Bữa tối', 3, 250, 30, 10, 8),

    -- Ngày 4
    (2, 5, N'Gỏi cuốn', 2, N'Bữa sáng', 4, 200, 20, 6, 10),
    (2, 1, N'Phở bò', 1, N'Bữa trưa', 4, 450, 50, 10, 25),
    (2, 8, N'Canh chua cá lóc', 1, N'Bữa tối', 4, 200, 10, 5, 15),

    -- Ngày 5
    (2, 7, N'Chả giò', 2, N'Bữa sáng', 5, 300, 20, 10, 10),
    (2, 2, N'Bánh mì thịt', 1, N'Bữa trưa', 5, 350, 40, 12, 15),
    (2, 12, N'Chè đậu đen', 1, N'Bữa tối', 5, 200, 30, 5, 5),

    -- Ngày 6
    (2, 14, N'Bánh ướt', 1, N'Bữa sáng', 6, 200, 25, 5, 10),
    (2, 4, N'Cháo lòng', 1, N'Bữa trưa', 6, 350, 35, 12, 20),
    (2, 18, N'Bánh đúc', 1, N'Bữa tối', 6, 150, 20, 5, 5),

    -- Ngày 7
    (2, 17, N'Bánh chưng', 1, N'Bữa sáng', 7, 350, 45, 12, 15),
    (2, 20, N'Bánh tráng trộn', 1, N'Bữa trưa', 7, 250, 30, 10, 5),
    (2, 21, N'Bánh khoai mì', 1, N'Bữa tối', 7, 300, 40, 15, 10);


-- Insert data into MealPlanDetail for 'Dinh dưỡng cân bằng'
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (3, 1, N'Phở bò', 1, N'Bữa sáng', 1, 450, 50, 10, 25),
    (3, 6, N'Bánh xèo', 1, N'Bữa trưa', 1, 300, 30, 15, 10),
    (3, 12, N'Chè đậu đen', 1, N'Bữa tối', 1, 200, 30, 5, 5),

    -- Ngày 2
    (3, 11, N'Bánh cuốn', 1, N'Bữa sáng', 2, 300, 40, 8, 10),
    (3, 17, N'Bánh chưng', 1, N'Bữa trưa', 2, 350, 45, 12, 15),
    (3, 20, N'Bánh tráng trộn', 1, N'Bữa tối', 2, 250, 30, 10, 5),

    -- Ngày 3
    (3, 5, N'Gỏi cuốn', 2, N'Bữa sáng', 3, 200, 20, 6, 10),
    (3, 3, N'Cơm tấm sườn', 1, N'Bữa trưa', 3, 600, 70, 20, 30),
    (3, 15, N'Bánh canh cua', 1, N'Bữa tối', 3, 400, 40, 15, 20),

    -- Ngày 4
    (3, 7, N'Chả giò', 2, N'Bữa sáng', 4, 300, 20, 10, 10),
    (3, 2, N'Bánh mì thịt', 1, N'Bữa trưa', 4, 350, 40, 12, 15),
    (3, 8, N'Canh chua cá lóc', 1, N'Bữa tối', 4, 200, 10, 5, 15),

    -- Ngày 5
    (3, 9, N'Bún riêu', 1, N'Bữa sáng', 5, 350, 40, 10, 20),
    (3, 16, N'Bánh tét', 1, N'Bữa trưa', 5, 300, 40, 10, 10),
    (3, 19, N'Bánh khọt', 1, N'Bữa tối', 5, 200, 20, 10, 10),

    -- Ngày 6
    (3, 14, N'Bánh ướt', 1, N'Bữa sáng', 6, 200, 25, 5, 10),
    (3, 4, N'Cháo lòng', 1, N'Bữa trưa', 6, 350, 35, 12, 20),
    (3, 18, N'Bánh đúc', 1, N'Bữa tối', 6, 150, 20, 5, 5),

    -- Ngày 7
    (3, 10, N'Cá kho tộ', 1, N'Bữa sáng', 7, 400, 10, 20, 25),
    (3, 13, N'Bánh bèo', 1, N'Bữa trưa', 7, 250, 30, 10, 8),
    (3, 21, N'Bánh khoai mì', 1, N'Bữa tối', 7, 300, 40, 15, 10);

-- MealPlan: Kế hoạch ăn uống Healthy
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)  
VALUES  
 
(4, 21, N'Salad ức gà', 1, N'Bữa sáng', 1, 300, 20, 8, 35),  
(4, 24, N'Cá hồi áp chảo', 1, N'Bữa trưa', 1, 400, 10, 20, 40),  
(4, 25, N'Súp lơ hấp', 1, N'Bữa tối', 1, 180, 20, 10, 5),  
 
(4, 22, N'Cháo yến mạch', 1, N'Bữa sáng', 2, 250, 40, 6, 8),  
(4, 30, N'Salad cá ngừ', 1, N'Bữa trưa', 2, 350, 15, 10, 40),  
(4, 29, N'Súp bí đỏ', 1, N'Bữa tối', 2, 230, 35, 8, 6),  

(4, 23, N'Smoothie bơ chuối', 1, N'Bữa sáng', 3, 220, 30, 10, 5),  
(4, 24, N'Cá hồi áp chảo', 1, N'Bữa trưa', 3, 400, 10, 20, 40),  
(4, 28, N'Xôi gạo lứt', 1, N'Bữa tối', 3, 320, 55, 7, 8),  

(4, 26, N'Bánh pancake chuối yến mạch', 1, N'Bữa sáng', 4, 200, 30, 5, 6),  
(4, 25, N'Súp lơ hấp', 1, N'Bữa trưa', 4, 180, 20, 10, 5),  
(4, 27, N'Trà gừng mật ong', 1, N'Bữa tối', 4, 100, 25, 0, 0),  

(4, 21, N'Salad ức gà', 1, N'Bữa sáng', 5, 300, 20, 8, 35),  
(4, 22, N'Cháo yến mạch', 1, N'Bữa trưa', 5, 250, 40, 6, 8),  
(4, 23, N'Smoothie bơ chuối', 1, N'Bữa tối', 5, 220, 30, 10, 5),  
 
(4, 29, N'Súp bí đỏ', 1, N'Bữa sáng', 6, 230, 35, 8, 6),  
(4, 30, N'Salad cá ngừ', 1, N'Bữa trưa', 6, 350, 15, 10, 40),  
(4, 28, N'Xôi gạo lứt', 1, N'Bữa tối', 6, 320, 55, 7, 8),  

(4, 26, N'Bánh pancake chuối yến mạch', 1, N'Bữa sáng', 7, 200, 30, 5, 6),  
(4, 24, N'Cá hồi áp chảo', 1, N'Bữa trưa', 7, 400, 10, 20, 40),  
(4, 27, N'Trà gừng mật ong', 1, N'Bữa tối', 7, 100, 25, 0, 0);  

-- MealPlan: Kế hoạch ăn uống cho người tiểu đường
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)  
VALUES  

(5, 22, N'Cháo yến mạch', 1, N'Bữa sáng', 1, 250, 40, 6, 8),  
(5, 28, N'Xôi gạo lứt', 1, N'Bữa trưa', 1, 320, 55, 7, 8),  
(5, 25, N'Súp lơ hấp', 1, N'Bữa tối', 1, 180, 20, 10, 5),  
 
(5, 23, N'Smoothie bơ chuối', 1, N'Bữa sáng', 2, 220, 30, 10, 5),  
(5, 30, N'Salad cá ngừ', 1, N'Bữa trưa', 2, 350, 15, 10, 40),  
(5, 29, N'Súp bí đỏ', 1, N'Bữa tối', 2, 230, 35, 8, 6),

(5, 26, N'Bánh pancake chuối yến mạch', 1, N'Bữa sáng', 3, 200, 30, 5, 6),
(5, 28, N'Xôi gạo lứt', 1, N'Bữa trưa', 3, 320, 55, 7, 8),
(5, 27, N'Trà gừng mật ong', 1, N'Bữa tối', 3, 100, 25, 0, 0),

(5, 21, N'Salad ức gà', 1, N'Bữa sáng', 4, 300, 20, 8, 35),
(5, 22, N'Cháo yến mạch', 1, N'Bữa trưa', 4, 250, 40, 6, 8),
(5, 23, N'Smoothie bơ chuối', 1, N'Bữa tối', 4, 220, 30, 10, 5),

(5, 29, N'Súp bí đỏ', 1, N'Bữa sáng', 5, 230, 35, 8, 6),
(5, 30, N'Salad cá ngừ', 1, N'Bữa trưa', 5, 350, 15, 10, 40),
(5, 25, N'Súp lơ hấp', 1, N'Bữa tối', 5, 180, 20, 10, 5),

(5, 26, N'Bánh pancake chuối yến mạch', 1, N'Bữa sáng', 6, 200, 30, 5, 6),
(5, 24, N'Cá hồi áp chảo', 1, N'Bữa trưa', 6, 400, 10, 20, 40),
(5, 27, N'Trà gừng mật ong', 1, N'Bữa tối', 6, 100, 25, 0, 0),

(5, 21, N'Salad ức gà', 1, N'Bữa sáng', 7, 300, 20, 8, 35),
(5, 22, N'Cháo yến mạch', 1, N'Bữa trưa', 7, 250, 40, 6, 8),
(5, 23, N'Smoothie bơ chuối', 1, N'Bữa tối', 7, 220, 30, 10, 5);

-- MealPlan: Kế hoạch ăn uống cho người gan nhiễm mỡ
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
 
(1, 21, N'Bánh khoai mì', 1, N'Bữa tối', 7, 300, 40, 15, 10), 
(3, 12, N'Chè đậu đen', 1, N'Bữa phụ', 1, 200, 30, 5, 5),
(6, 24, N'Cá hồi áp chảo', 1, N'Bữa trưa', 1, 400, 10, 20, 40),  
(6, 25, N'Súp lơ hấp', 1, N'Bữa tối', 1, 180, 20, 10, 5),  

(6, 22, N'Cháo yến mạch', 1, N'Bữa sáng', 2, 250, 40, 6, 8),  
(6, 30, N'Salad cá ngừ', 1, N'Bữa trưa', 2, 350, 15, 10, 40),  
(6, 29, N'Súp bí đỏ', 1, N'Bữa tối', 2, 230, 35, 8, 6),  

(6, 26, N'Bánh pancake chuối yến mạch', 1, N'Bữa sáng', 3, 200, 30, 5, 6),
(6, 28, N'Xôi gạo lứt', 1, N'Bữa trưa', 3, 320, 55, 7, 8),
(6, 27, N'Trà gừng mật ong', 1, N'Bữa tối', 3, 100, 25, 0, 0),

(6, 21, N'Salad ức gà', 1, N'Bữa sáng', 4, 300, 20, 8, 35),
(6, 22, N'Cháo yến mạch', 1, N'Bữa trưa', 4, 250, 40, 6, 8),
(6, 23, N'Smoothie bơ chuối', 1, N'Bữa tối', 4, 220, 30, 10, 5),

(6, 29, N'Súp bí đỏ', 1, N'Bữa sáng', 5, 230, 35, 8, 6),
(6, 30, N'Salad cá ngừ', 1, N'Bữa trưa', 5, 350, 15, 10, 40),
(6, 25, N'Súp lơ hấp', 1, N'Bữa tối', 5, 180, 20, 10, 5),

(6, 26, N'Bánh pancake chuối yến mạch', 1, N'Bữa sáng', 6, 200, 30, 5, 6),
(6, 27, N'Trà gừng mật ong', 1, N'Bữa phụ', 6, 100, 25, 0, 0),
(6, 24, N'Cá hồi áp chảo', 1, N'Bữa trưa', 6, 400, 10, 20, 40),
(1, 14, N'Bánh ướt', 1, N'Bữa tối', 1, 200, 25, 5, 10),

(6, 21, N'Salad ức gà', 1, N'Bữa sáng', 7, 300, 20, 8, 35),
(6, 22, N'Cháo yến mạch', 1, N'Bữa trưa', 7, 250, 40, 6, 8),
(6, 23, N'Smoothie bơ chuối', 1, N'Bữa tối', 7, 220, 30, 10, 5);
