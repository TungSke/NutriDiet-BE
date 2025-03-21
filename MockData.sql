USE NutriDiet
GO

-- Tắt ràng buộc khóa ngoại tạm thời
ALTER TABLE UserAllergy NOCHECK CONSTRAINT ALL;
ALTER TABLE UserDisease NOCHECK CONSTRAINT ALL;
ALTER TABLE UserPackage NOCHECK CONSTRAINT ALL;
ALTER TABLE Package NOCHECK CONSTRAINT ALL;
ALTER TABLE GeneralHealthProfile NOCHECK CONSTRAINT ALL;
ALTER TABLE RecipeSuggestion NOCHECK CONSTRAINT ALL;
ALTER TABLE UserFoodPreference NOCHECK CONSTRAINT ALL;
ALTER TABLE MealPlanDetail NOCHECK CONSTRAINT ALL;
ALTER TABLE FoodSubstitution NOCHECK CONSTRAINT ALL;
ALTER TABLE Notification NOCHECK CONSTRAINT ALL;
ALTER TABLE HealthcareIndicator NOCHECK CONSTRAINT ALL;
ALTER TABLE AIRecommendMealPlan NOCHECK CONSTRAINT ALL;
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
DELETE FROM AIRecommendMealPlan;
DELETE FROM MealLogDetail;
DELETE FROM MealLog;
DELETE FROM PersonalGoal;

DELETE FROM Disease;
DELETE FROM Allergy;
DELETE FROM Food;
DELETE FROM Ingredient; -- Thêm xóa dữ liệu bảng Ingredient
DELETE FROM Package;
DELETE FROM [User];
DELETE FROM Role;
DELETE FROM CuisineType;

-- Bật lại ràng buộc khóa ngoại
ALTER TABLE UserAllergy CHECK CONSTRAINT ALL;
ALTER TABLE UserDisease CHECK CONSTRAINT ALL;
ALTER TABLE UserPackage CHECK CONSTRAINT ALL;
ALTER TABLE Package CHECK CONSTRAINT ALL;
ALTER TABLE GeneralHealthProfile CHECK CONSTRAINT ALL;
ALTER TABLE RecipeSuggestion CHECK CONSTRAINT ALL;
ALTER TABLE UserFoodPreference CHECK CONSTRAINT ALL;
ALTER TABLE MealPlanDetail CHECK CONSTRAINT ALL;
ALTER TABLE FoodSubstitution CHECK CONSTRAINT ALL;
ALTER TABLE Notification CHECK CONSTRAINT ALL;
ALTER TABLE HealthcareIndicator CHECK CONSTRAINT ALL;
ALTER TABLE AIRecommendMealPlan CHECK CONSTRAINT ALL;
ALTER TABLE MealLogDetail CHECK CONSTRAINT ALL;
ALTER TABLE MealLog CHECK CONSTRAINT ALL;
ALTER TABLE PersonalGoal CHECK CONSTRAINT ALL;

DBCC CHECKIDENT ('User', RESEED, 0);
DBCC CHECKIDENT ('Disease', RESEED, 0);
DBCC CHECKIDENT ('Allergy', RESEED, 0);
DBCC CHECKIDENT ('Food', RESEED, 0);
DBCC CHECKIDENT ('Ingredient', RESEED, 0); -- Thêm reset IDENTITY cho Ingredient
DBCC CHECKIDENT ('Package', RESEED, 0);
DBCC CHECKIDENT ('UserPackage', RESEED, 0);
DBCC CHECKIDENT ('MealPlan', RESEED, 0);
DBCC CHECKIDENT ('MealLog', RESEED, 0);
DBCC CHECKIDENT ('MealLogDetail', RESEED, 0);
DBCC CHECKIDENT ('PersonalGoal', RESEED, 0);

GO

-- Thêm dữ liệu vào bảng Ingredient
SET IDENTITY_INSERT Ingredient ON;
DBCC CHECKIDENT ('Ingredient', RESEED, 0);

INSERT INTO Ingredient (IngredientID, IngredientName, Calories, Protein, Carbs, Fat)
VALUES 
    (1, N'Sốt mayonnaise', 701, 0, 0.1, 77.8),
    (2, N'Xì dầu', 53, 6.3, 6.8, 0.04),
    (3, N'Tương ớt', 37, 0.5, 7.6, 0.5),
    (4, N'Tương nếp', 86, 4.3, 15.7, 0.7),
    (5, N'Nước mắm cá', 35, 5.1, 3.6, 0.01),
    (6, N'Mắm tôm loãng', 44, 7, 2.1, 0.8),
    (7, N'Magi', 65, 10.5, 5.6, 0.1),
    (8, N'Riềng', 26, 0.3, 2.5, 0),
    (9, N'Nghệ tươi', 25, 1.1, 4.4, 0.3),
    (10, N'Muối', 0, 0, 0, 0),
    (11, N'Hạt tiêu', 231, 7, 34.1, 7.4),
    (12, N'Gừng tươi', 29, 0.4, 5.1, 0.8),
    (13, N'Bột cary', 283, 8.2, 46, 7.3),
    (14, N'Bột ngọt', 260, 66, 0, 0),
    (15, N'Mật ong', 327, 0.4, 81.3, 0),
    (16, N'Đường cát', 390, 0, 97.4, 0),
    (17, N'Bột ca cao', 414, 19.6, 53, 13.7),
    (18, N'Pho mát', 380, 25.5, 0, 30.9),
    (19, N'Sữa đặc có đường', 336, 8.1, 56, 8.8),
    (20, N'Sữa chua vinalmilk có đường', 97.7, 3.5, 15.3, 2.5),
    (21, N'Sữa bò tươi', 74, 3.9, 4.8, 4.4),
    (22, N'Bột trứng', 563, 44, 1.8, 42.2),
    (23, N'Trứng vịt lộn', 182, 13.6, 4, 12.4),
    (24, N'Trứng cá', 171, 20.5, 0, 9.9),
    (25, N'Trứng cút', 154, 13.1, 0.4, 11.1),
    (26, N'Trứng vịt', 184, 13, 1, 14.2),
    (27, N'Trứng gà', 166, 14.8, 0.5, 11.6),
    (28, N'Ruốc tôm', 305, 65.5, 3.7, 3.1),
    (29, N'Bánh phồng tôm', 381, 3.4, 75.3, 7.4),
    (30, N'Tôm khô', 347, 75.6, 2.5, 3.8),
    (31, N'Tôm đồng', 90, 18.4, 0, 1.8),
    (32, N'Tôm biển', 82, 17.6, 0.9, 0.9),
    (33, N'Tép khô', 269, 59.8, 0.7, 3),
    (34, N'Sò', 78, 9.5, 4.9, 2.3),
    (35, N'Trai', 38, 4.6, 2.5, 1.1),
    (36, N'Ốc bươu', 84, 11.1, 8.3, 0.7),
    (37, N'Mực tươi', 73, 16.3, 0, 0.9),
    (38, N'Lươn', 180, 18.4, 0.2, 11.7),
    (39, N'Hến', 45, 4.5, 5.1, 0.7),
    (40, N'Ghẹ', 54, 11.9, 0, 0.7),
    (41, N'Cua đồng', 87, 12.3, 2, 3.3),
    (42, N'Cá trê', 173, 16.5, 0, 11.9),
    (43, N'Cá trạch', 110, 20.4, 0, 3.2),
    (44, N'Cá thu', 166, 18.2, 0, 10.3),
    (45, N'Cá rô phi', 100, 19.7, 0, 2.3),
    (46, N'Cá nục', 111, 20.2, 0, 3.3),
    (47, N'Cá ngừ', 87, 21, 0, 0.3),
    (48, N'Cá mòi', 124, 17.5, 0, 6),
    (49, N'Cá hồi', 136, 22, 0, 5.3),
    (50, N'Cá chép', 96, 16, 0, 3.6),
    (51, N'Cá bống', 70, 15.8, 0, 0.8),
    (52, N'Thịt bê', 85, 20, 0, 0.5),
    (53, N'Thịt dê', 122, 20.7, 0, 4.3),
    (54, N'Đuôi bò', 137, 19.7, 0, 6.5),
    (55, N'Gân bò', 124, 30.2, 0, 0.3),
    (56, N'Sườn heo', 187, 17.9, 0, 12.8),
    (57, N'Tai heo', 126, 21, 1.3, 4.1),
    (58, N'Lòng gà', 119, 17.9, 1.8, 4.5),
    (59, N'Thịt ếch', 90, 20, 0, 1.1),
    (60, N'Xúc xích', 535, 27.2, 0, 47.4),
    (61, N'Nem chua', 137, 21.7, 4.3, 3.7),
    (62, N'Lạp xưởng', 585, 20.8, 1.7, 55),
    (63, N'Chả heo', 517, 10.8, 5.1, 50.4),
    (64, N'Giò lụa', 136, 21.5, 0, 5.5),
    (65, N'Giò bò', 357, 13.8, 0, 33.5),
    (66, N'Pa tê', 326, 10.8, 15.4, 24.6),
    (67, N'Dăm bông heo', 318, 23, 0.3, 25),
    (68, N'Chân giò heo', 230, 15.7, 0, 18.6),
    (69, N'Thịt vịt', 267, 17.8, 0, 21.8),
    (70, N'Thịt trâu', 97, 20.4, 0.9, 1.4),
    (71, N'Thịt heo nạc', 139, 19, 0, 7),
    (72, N'Thịt heo (nửa nạc, nửa mỡ)', 260, 16.5, 0, 21.5),
    (73, N'Thịt heo mỡ', 394, 14.5, 0, 37.3),
    (74, N'Thịt gà ta', 199, 20.3, 0, 13.1),
    (75, N'Thịt bò', 118, 21, 0, 3.8),
    (76, N'Thịt bò (lưng, nạc và mỡ)', 182, 21.5, 0, 10.7),
    (77, N'Bì heo', 118, 23.3, 0, 2.7),
    (78, N'Tủy xương heo', 749, 2.3, 0, 82.2),
    (79, N'Tủy xương bò', 814, 1.1, 0, 89.9),
    (80, N'Dầu oliu', 900, 0, 0, 100),
    (81, N'Dầu ăn Tường An', 900, 0, 0, 100),
    (82, N'Chanh', 24, 0.9, 4.5, 0.3),
    (83, N'Cam', 38, 0.9, 8.3, 0.1),
    (84, N'Bưởi', 30, 0.2, 7.3, 0),
    (85, N'Chuối tiêu', 97, 1.5, 22.2, 0.2),
    (86, N'Chuối tây', 56, 0.9, 12.4, 0.3),
    (87, N'Dâu tây', 43, 1.8, 8.1, 0.4),
    (88, N'Dưa hấu', 16, 1.2, 2.3, 0.2),
    (89, N'Dứa', 29, 0.8, 6.5, 0),
    (90, N'Nấm rơm', 57, 3.6, 3.4, 3.2),
    (91, N'Nấm hương', 39, 5.5, 3.1, 0.5),
    (92, N'Mộc nhĩ', 304, 10.6, 65, 0.2),
    (93, N'Tỏi', 121, 6, 23, 0.5),
    (94, N'Tía tô', 25, 2.9, 3.4, 0),
    (95, N'Thìa là', 28, 2.6, 1.8, 1.1),
    (96, N'Súp lơ xanh', 26, 3, 2.9, 0.3),
    (97, N'Su su', 19, 0.8, 3.6, 0.1),
    (98, N'Su hào', 37, 2.8, 6.2, 0.1),
    (99, N'Rau thơm các loại', 18, 2, 2.4, 0),
    (100, N'Rau sà lách', 17, 1.5, 1.8, 0.4),
    (101, N'Rau ngót', 35, 5.3, 3.4, 0),
    (102, N'Rau muống', 25, 3.2, 2.1, 0.4),
    (103, N'Rau mồng tơi', 14, 2, 1.4, 0),
    (104, N'Rau má', 20, 3.2, 1.8, 0),
    (105, N'Ớt đỏ', 23, 1, 4, 0.3),
    (106, N'Ngó sen', 61, 1, 13.9, 0.1),
    (107, N'Mướp', 17, 0.9, 2.8, 0.2),
    (108, N'Mướp đắng', 16, 0.9, 2.8, 0.2),
    (109, N'Măng tây', 14, 2.2, 1.1, 0.1),
    (110, N'Măng chua', 11, 1.4, 1.4, 0),
    (111, N'Lá lốt', 39, 4.3, 5.4, 0),
    (112, N'Khế', 16, 0.6, 2.8, 0.3),
    (113, N'Hẹ lá', 18, 2.2, 1.5, 0.3),
    (114, N'Hạt sen', 161, 9.5, 29.5, 0.5),
    (115, N'Hành tây', 41, 1.8, 8.2, 0.1),
    (116, N'Hành lá', 22, 1.3, 4.3, 0),
    (117, N'Hành củ', 26, 1.3, 4.4, 0.4),
    (118, N'Giá', 44, 5.5, 5.1, 0.2),
    (119, N'Đu đủ xanh', 22, 0.8, 4.6, 0),
    (120, N'Đậu rồng', 34, 1.9, 6.3, 0.1),
    (121, N'Đậu cô ve', 73, 5, 13.3, 0),
    (122, N'Dưa leo', 16, 0.8, 2.9, 0.1),
    (123, N'Dưa gang', 11, 0.8, 2, 0),
    (124, N'Củ cải trắng', 21, 1.5, 3.6, 0.1),
    (125, N'Củ cải đỏ', 48, 1.3, 10.8, 0),
    (126, N'Cần tây', 48, 3.7, 7.9, 0.2),
    (127, N'Chuối xanh', 74, 1.2, 16.4, 0.5),
    (128, N'Cải xanh', 16, 1.7, 1.9, 0.2),
    (129, N'Cải thìa', 17, 1.4, 2.4, 0.2),
    (130, N'Cải cúc', 14, 1.6, 1.9, 0),
    (131, N'Cà tím', 22, 1, 4.5, 0),
    (132, N'Cà rốt', 39, 1.5, 7.8, 0.2),
    (133, N'Cà pháo', 20, 1.5, 3.6, 0),
    (134, N'Cà chua', 20, 0.6, 4, 0.2),
    (135, N'Bí ngô', 27, 0.3, 6.1, 0.1),
    (136, N'Bí xanh', 12, 0.6, 2.4, 0),
    (137, N'Bầu', 14, 0.6, 2.9, 0.02),
    (138, N'Sữa đậu nành', 28, 3.1, 0.4, 1.6),
    (139, N'Đậu phụ', 95, 10.9, 0.7, 5.4),
    (140, N'Đậu xanh', 328, 23.4, 53.1, 2.4),
    (141, N'Đậu đen', 325, 24.2, 53.3, 1.7),
    (142, N'Miến dong', 332, 0.6, 82.2, 0.1),
    (143, N'Khoai tây', 93, 2, 20.9, 0.1),
    (144, N'Khoai sọ', 114, 1.8, 26.5, 0.1),
    (145, N'Khoai môn', 109, 1.5, 25.2, 0.2),
    (146, N'Khoai lang', 119, 0.8, 28.5, 0.2),
    (147, N'Mì sợi', 349, 11, 74.2, 0.9),
    (148, N'Cốm', 297, 6.1, 66.3, 0.8),
    (149, N'Bún', 110, 1.7, 25.7, 0),
    (150, N'Bột ngô vàng', 361, 8.3, 73, 4),
    (151, N'Bột mì', 346, 10.3, 73.6, 1.1),
    (152, N'Bột gạo tẻ', 359, 6.6, 82.2, 0.4),
    (153, N'Bột gạo nếp', 362, 8.2, 78.8, 1.6),
    (154, N'Bánh phở', 143, 3.2, 31.7, 0.4),
    (155, N'Bánh mì', 249, 7.9, 52.6, 0.8),
    (156, N'Bánh đúc', 52, 0.9, 11.3, 0.3),
    (157, N'Bắp ngô', 196, 4.1, 39.6, 2.3),
    (158, N'Cơm gạo lứt đỏ', 111, 2.6, 23, 0.9),
    (159, N'Cơm trắng', 130, 2.7, 28.2, 0.3),
    (160, N'Cơm gạo nếp', 130, 2.5, 28, 0.3);

SET IDENTITY_INSERT Ingredient OFF;
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

SET IDENTITY_INSERT [dbo].[Package] ON 
INSERT Package (PackageID, PackageName, Price, Duration, Description) VALUES (1, 'Premium 1 Tháng', 99.00, 30, N'Truy cập đầy đủ tính năng AI trong 1 tháng');
SET IDENTITY_INSERT [dbo].[Package] OFF

SET IDENTITY_INSERT [dbo].[UserPackage] ON 
INSERT UserPackage (UserPackageID, UserID, PackageID, StartDate, ExpiryDate, Status) VALUES (1, 1, 1, GETDATE(), GETDATE() + 30, 'Active');
SET IDENTITY_INSERT [dbo].[UserPackage] OFF

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

--Thêm nguyên liệu vào cho Food
INSERT INTO FoodIngredient (FoodID, IngredientID)
VALUES
    -- 1. Phở bò
    (1, 154),    -- Bánh phở
    (1, 75),     -- Thịt bò
    (1, 12),     -- Gừng tươi
    (1, 116),    -- Hành lá
    (1, 5),      -- Nước mắm cá
    (1, 10),     -- Muối

    -- 2. Bánh mì thịt
    (2, 155),    -- Bánh mì
    (2, 71),     -- Thịt heo nạc
    (2, 122),    -- Dưa leo
    (2, 116),    -- Hành lá
    (2, 5),      -- Nước mắm cá
    (2, 1),      -- Sốt mayonnaise

    -- 3. Cơm tấm sườn
    (3, 159),    -- Cơm trắng
    (3, 56),     -- Sườn heo
    (3, 5),      -- Nước mắm cá
    (3, 93),     -- Tỏi
    (3, 16),     -- Đường cát

    -- 4. Bún chả
    (4, 149),    -- Bún
    (4, 71),     -- Thịt heo nạc
    (4, 5),      -- Nước mắm cá
    (4, 93),     -- Tỏi
    (4, 16),     -- Đường cát
    (4, 100),    -- Rau sà lách

    -- 5. Gỏi cuốn
    (5, 32),     -- Tôm biển
    (5, 71),     -- Thịt heo nạc
    (5, 149),    -- Bún
    (5, 100),    -- Rau sà lách
    (5, 5),      -- Nước mắm cá

    -- 6. Bánh xèo
    (6, 152),    -- Bột gạo tẻ
    (6, 32),     -- Tôm biển
    (6, 71),     -- Thịt heo nạc
    (6, 80),     -- Dầu oliu
    (6, 118),    -- Giá

    -- 7. Chả giò
    (7, 152),    -- Bột gạo tẻ
    (7, 71),     -- Thịt heo nạc
    (7, 92),     -- Mộc nhĩ
    (7, 80),     -- Dầu oliu
    (7, 5),      -- Nước mắm cá

    -- 8. Canh chua cá lóc
    (8, 42),     -- Cá trê (thay cá lóc)
    (8, 134),    -- Cà chua
    (8, 89),     -- Dứa
    (8, 5),      -- Nước mắm cá
    (8, 10),     -- Muối

    -- 9. Bún riêu
    (9, 149),    -- Bún
    (9, 41),     -- Cua đồng
    (9, 134),    -- Cà chua
    (9, 5),      -- Nước mắm cá
    (9, 116),    -- Hành lá

    -- 10. Cá kho tộ
    (10, 49),    -- Cá hồi
    (10, 5),     -- Nước mắm cá
    (10, 16),    -- Đường cát
    (10, 11),    -- Hạt tiêu
    (10, 93),    -- Tỏi

    -- 11. Bánh cuốn
    (11, 152),   -- Bột gạo tẻ
    (11, 71),    -- Thịt heo nạc
    (11, 92),    -- Mộc nhĩ
    (11, 80),    -- Dầu oliu
    (11, 5),     -- Nước mắm cá

    -- 12. Chè đậu đen
    (12, 141),   -- Đậu đen
    (12, 16),    -- Đường cát
    (12, 19),    -- Sữa đặc có đường

    -- 13. Bánh bèo
    (13, 152),   -- Bột gạo tẻ
    (13, 32),    -- Tôm biển
    (13, 116),   -- Hành lá
    (13, 80),    -- Dầu oliu
    (13, 5),     -- Nước mắm cá

    -- 14. Bánh ướt
    (14, 152),   -- Bột gạo tẻ
    (14, 71),    -- Thịt heo nạc
    (14, 116),   -- Hành lá
    (14, 5),     -- Nước mắm cá

    -- 15. Bánh canh cua
    (15, 152),   -- Bột gạo tẻ
    (15, 41),    -- Cua đồng
    (15, 116),   -- Hành lá
    (15, 5),     -- Nước mắm cá

    -- 16. Bánh tét
    (16, 160),   -- Cơm gạo nếp
    (16, 140),   -- Đậu xanh
    (16, 71),    -- Thịt heo nạc
    (16, 11),    -- Hạt tiêu

    -- 17. Bánh chưng
    (17, 160),   -- Cơm gạo nếp
    (17, 140),   -- Đậu xanh
    (17, 71),    -- Thịt heo nạc
    (17, 11),    -- Hạt tiêu

    -- 18. Bánh đúc
    (18, 152),   -- Bột gạo tẻ
    (18, 116),   -- Hành lá
    (18, 5),     -- Nước mắm cá
    (18, 80),    -- Dầu oliu

    -- 19. Bánh khọt
    (19, 152),   -- Bột gạo tẻ
    (19, 32),    -- Tôm biển
    (19, 80),    -- Dầu oliu
    (19, 116),   -- Hành lá

    -- 20. Bánh tráng trộn
    (20, 152),   -- Bột gạo tẻ (bánh tráng)
    (20, 27),    -- Trứng gà
    (20, 30),    -- Tôm khô
    (20, 5),     -- Nước mắm cá
    (20, 16),    -- Đường cát

    -- 21. Salad ức gà
    (21, 74),    -- Thịt gà ta
    (21, 100),   -- Rau sà lách
    (21, 82),    -- Chanh
    (21, 80),    -- Dầu oliu
    (21, 134),   -- Cà chua

    -- 22. Cháo yến mạch
    (22, 152),   -- Bột gạo tẻ (thay yến mạch)
    (22, 85),    -- Chuối tiêu
    (22, 21),    -- Sữa bò tươi
    (22, 16),    -- Đường cát

    -- 23. Smoothie bơ chuối
    (23, 85),    -- Chuối tiêu
    (23, 21),    -- Sữa bò tươi
    (23, 15),    -- Mật ong

    -- 24. Cá hồi áp chảo
    (24, 49),    -- Cá hồi
    (24, 80),    -- Dầu oliu
    (24, 96),    -- Súp lơ xanh
    (24, 11),    -- Hạt tiêu
    (24, 10),    -- Muối

    -- 25. Súp lơ hấp
    (25, 96),    -- Súp lơ xanh
    (25, 80),    -- Dầu oliu
    (25, 10),    -- Muối

    -- 26. Bánh pancake chuối yến mạch
    (26, 152),   -- Bột gạo tẻ (thay yến mạch)
    (26, 85),    -- Chuối tiêu
    (26, 27),    -- Trứng gà
    (26, 80),    -- Dầu oliu

    -- 27. Trà gừng mật ong
    (27, 12),    -- Gừng tươi
    (27, 15),    -- Mật ong
    (27, 16),    -- Đường cát

    -- 28. Xôi gạo lứt
    (28, 158),   -- Cơm gạo lứt đỏ
    (28, 140),   -- Đậu xanh
    (28, 80),    -- Dầu oliu

    -- 29. Súp bí đỏ
    (29, 135),   -- Bí ngô
    (29, 21),    -- Sữa bò tươi
    (29, 10),    -- Muối

    -- 30. Salad cá ngừ
    (30, 47),    -- Cá ngừ
    (30, 100),   -- Rau sà lách
    (30, 80),    -- Dầu oliu
    (30, 134),   -- Cà chua

    -- 31. Lẩu gà lá é
    (31, 74),    -- Thịt gà ta
    (31, 111),   -- Lá lốt
    (31, 5),     -- Nước mắm cá
    (31, 12),    -- Gừng tươi
    (31, 93),    -- Tỏi

    -- 32. Lẩu thái hải sản
    (32, 32),    -- Tôm biển
    (32, 37),    -- Mực tươi
    (32, 34),    -- Sò
    (32, 134),   -- Cà chua
    (32, 5),     -- Nước mắm cá

    -- 33. Lẩu bò nhúng giấm
    (33, 75),    -- Thịt bò
    (33, 5),     -- Nước mắm cá
    (33, 149),   -- Bún
    (33, 100),   -- Rau sà lách

    -- 34. Bún bò Huế
    (34, 149),   -- Bún
    (34, 75),    -- Thịt bò
    (34, 68),    -- Chân giò heo
    (34, 5),     -- Nước mắm cá
    (34, 105),   -- Ớt đỏ

    -- 35. Hủ tiếu Nam Vang
    (35, 149),   -- Bún
    (35, 32),    -- Tôm biển
    (35, 71),    -- Thịt heo nạc
    (35, 25),    -- Trứng cút
    (35, 116),   -- Hành lá

    -- 36. Bánh đúc mặn
    (36, 152),   -- Bột gạo tẻ
    (36, 140),   -- Đậu xanh
    (36, 5),     -- Nước mắm cá
    (36, 116),   -- Hành lá

    -- 37. Mì Quảng
    (37, 149),   -- Bún
    (37, 32),    -- Tôm biển
    (37, 71),    -- Thịt heo nạc
    (37, 27),    -- Trứng gà
    (37, 5),     -- Nước mắm cá

    -- 38. Bánh giò
    (38, 152),   -- Bột gạo tẻ
    (38, 71),    -- Thịt heo nạc
    (38, 92),    -- Mộc nhĩ
    (38, 11),    -- Hạt tiêu

    -- 39. Xôi xéo
    (39, 160),   -- Cơm gạo nếp
    (39, 140),   -- Đậu xanh
    (39, 80),    -- Dầu oliu
    (39, 116),   -- Hành lá

    -- 40. Cá nướng
    (40, 49),    -- Cá hồi
    (40, 5),     -- Nước mắm cá
    (40, 93),    -- Tỏi
    (40, 11),    -- Hạt tiêu

    -- 41. Ốc hương xào bơ tỏi
    (41, 36),    -- Ốc bươu (thay ốc hương)
    (41, 93),    -- Tỏi
    (41, 80),    -- Dầu oliu
    (41, 11),    -- Hạt tiêu

    -- 42. Bánh bột lọc
    (42, 152),   -- Bột gạo tẻ
    (42, 32),    -- Tôm biển
    (42, 71),    -- Thịt heo nạc
    (42, 5),     -- Nước mắm cá

    -- 43. Cơm gà
    (43, 159),   -- Cơm trắng
    (43, 74),    -- Thịt gà ta
    (43, 5),     -- Nước mắm cá
    (43, 122),   -- Dưa leo

    -- 44. Chè bắp
    (44, 157),   -- Bắp ngô
    (44, 16),    -- Đường cát
    (44, 19),    -- Sữa đặc có đường

    -- 45. Gỏi bò bóp thấu
    (45, 75),    -- Thịt bò
    (45, 82),    -- Chanh
    (45, 5),     -- Nước mắm cá
    (45, 115),   -- Hành tây

    -- 46. Tiết canh vịt
    (46, 69),    -- Thịt vịt
    (46, 5),     -- Nước mắm cá
    (46, 116),   -- Hành lá

    -- 47. Lòng xào dưa
    (47, 57),    -- Tai heo (thay lòng)
    (47, 110),   -- Măng chua
    (47, 93),    -- Tỏi
    (47, 80),    -- Dầu oliu

    -- 48. Dê tái chanh
    (48, 53),    -- Thịt dê
    (48, 82),    -- Chanh
    (48, 5),     -- Nước mắm cá
    (48, 12),    -- Gừng tươi

    -- 49. Bánh khoai mì nướng
    (49, 145),   -- Khoai môn (thay khoai mì)
    (49, 16),    -- Đường cát
    (49, 19),    -- Sữa đặc có đường

    -- 50. Gỏi ngó sen tôm thịt
    (50, 106),   -- Ngó sen
    (50, 32),    -- Tôm biển
    (50, 71),    -- Thịt heo nạc
    (50, 5),     -- Nước mắm cá
    (50, 29);    -- Bánh phồng tôm

GO