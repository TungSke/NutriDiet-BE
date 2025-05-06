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
DELETE FROM SystemConfiguration;

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
    (20, N'Sữa chua có đường', 97.7, 3.5, 15.3, 2.5),
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
(2, 'Customer'),
(3, 'Nutritionist');

SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserID], [FullName], [Email], [Password], [Phone], [Age], [Gender], [Location], [Avatar], [fcmToken], [Status], [RoleID]) VALUES 
(1, N'New User', N'user@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '21', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(2, N'Admin', N'admin@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, NULL, NULL, NULL, N'', NULL, N'Active', 1),
(3, N'Tran Van Tai', N'tranvantai@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '20', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(4, N'Nguyen Thi Hanh', N'hanhnt@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '18', 'Female', 'VietNam', N'', NULL, N'Active', 2),
(5, N'Pham Nguyen', N'nguyenpham@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '21', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(6, N'Nguyen Tuan Kiet', N'kietnguyen@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '22', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(7, N'Dinh Hoang Nam', N'namdh@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '22', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(8, N'Tran Thi Thao', N'thaotran@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '25', 'Female', 'VietNam', N'', NULL, N'Active', 2),
(9, N'Nguyen Thanh Phat', N'phat123@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '21', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(10, N'Chau Nhuan Phat', N'chaunhuanphat@gmail.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '33', 'Male', 'VietNam', N'', NULL, N'Active', 2),
(11, N'Nutritionist', N'nutritionist@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, '35', 'Male', 'VietNam', N'', NULL, N'Active', 3);

GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO

SET IDENTITY_INSERT [dbo].[Package] ON 
INSERT Package (PackageID, PackageName, PackageType, Price, Duration, Description) 
VALUES (1, 'Premium Basic 1 Tháng', 'Basic', 99000.00, 30, N'Truy cập các tính năng AI cơ bản ngoại trừ tính năng phân tích hình ảnh món ăn trong 1 tháng'),
(2, 'Premium Advanced 1 Tháng', 'Advanced', 139000.00, 30, N'Truy cập toàn bộ tính năng AI trong 1 tháng bao gồm tính năng phân tích hình ảnh món ăn trong 1 tháng');
SET IDENTITY_INSERT [dbo].[Package] OFF

SET IDENTITY_INSERT [dbo].[UserPackage] ON 
INSERT UserPackage (UserPackageID, UserID, PackageID, StartDate, ExpiryDate, Status, PriceAtPurchase) VALUES 
(1, 1, 1, GETDATE(), GETDATE() + 30, 'Active',990000),
(2, 3, 1, '2024-06-20', DATEADD(DAY, 30, '2024-06-20'), 'Expired',99000.00),
(3, 4, 1, '2024-02-05', DATEADD(DAY, 30, '2024-02-05'), 'Expired',99000.00),
(4, 5, 1, '2024-12-01', DATEADD(DAY, 30, '2024-12-01'), 'Expired',99000.00),
(5, 6, 1, '2025-01-01', DATEADD(DAY, 30, '2025-01-01'), 'Expired',99000.00),
(6, 7, 1, '2025-02-14', DATEADD(DAY, 30, '2025-02-14'), 'Expired',99000.00),
(7, 8, 1, '2025-02-14', DATEADD(DAY, 30, '2025-02-14'), 'Expired',99000.00),
(8, 9, 1, '2025-02-14', DATEADD(DAY, 30, '2025-02-14'), 'Expired',99000.00),
(9, 10, 1, '2025-03-31', DATEADD(DAY, 30, '2025-03-31'), 'Expired',99000.00);

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

-- Hạn chế nguyên liệu của các loại dị ứng
INSERT INTO AllergyIngredient (AllergyID, IngredientID)
VALUES
    -- 1. Dị ứng đậu phộng
    (1, 140), -- Đậu xanh (có thể liên quan đến họ đậu)
    
    -- 2. Dị ứng hải sản
    (2, 30),  -- Tôm khô
    (2, 31),  -- Tôm đồng
    (2, 32),  -- Tôm biển
    (2, 33),  -- Tép khô
    (2, 34),  -- Sò
    (2, 35),  -- Trai
    (2, 36),  -- Ốc bươu
    (2, 37),  -- Mực tươi
    (2, 40),  -- Ghẹ
    (2, 41),  -- Cua đồng

    -- 3. Dị ứng sữa
    (3, 18),  -- Pho mát
    (3, 19),  -- Sữa đặc có đường
    (3, 20),  -- Sữa chua Vinamilk có đường
    (3, 21),  -- Sữa bò tươi
    (3, 138), -- Sữa đậu nành (có thể gây nhầm lẫn với dị ứng sữa)

    -- 4. Dị ứng trứng
    (4, 22),  -- Bột trứng
    (4, 23),  -- Trứng vịt lộn
    (4, 24),  -- Trứng cá
    (4, 25),  -- Trứng cút
    (4, 26),  -- Trứng vịt
    (4, 27),  -- Trứng gà

    -- 5. Dị ứng gluten
    (5, 147), -- Mì sợi
    (5, 151), -- Bột mì
    (5, 155), -- Bánh mì

    -- 6. Dị ứng đậu nành
    (6, 138), -- Sữa đậu nành
    (6, 139), -- Đậu phụ
    (6, 140), -- Đậu xanh (có thể liên quan đến họ đậu)
    (6, 141), -- Đậu đen

    -- 7. Dị ứng hạt cây
    (7, 114), -- Hạt sen

    -- 8. Dị ứng mè (vừng)
    -- Không có nguyên liệu mè cụ thể trong danh sách của bạn

    -- 9. Dị ứng lúa mì
    (9, 147), -- Mì sợi
    (9, 151), -- Bột mì
    (9, 155), -- Bánh mì

    -- 10. Dị ứng động vật có vỏ
    (10, 30), -- Tôm khô
    (10, 31), -- Tôm đồng
    (10, 32), -- Tôm biển
    (10, 33), -- Tép khô
    (10, 34), -- Sò
    (10, 40), -- Ghẹ
    (10, 41), -- Cua đồng

    -- 11. Dị ứng cá
    (11, 42), -- Cá trê
    (11, 43), -- Cá trạch
    (11, 44), -- Cá thu
    (11, 45), -- Cá rô phi
    (11, 46), -- Cá nục
    (11, 47), -- Cá ngừ
    (11, 48), -- Cá mòi
    (11, 49), -- Cá hồi
    (11, 50), -- Cá chép
    (11, 51), -- Cá bống

    -- 18. Dị ứng trái cây có múi
    (18, 82), -- Chanh
    (18, 83), -- Cam
    (18, 84); -- Bưởi

-- Hạn chế nguyên liệu của các loại bệnh
INSERT INTO DiseaseIngredient (DiseaseID, IngredientID)
VALUES
    -- 1. Bệnh tiểu đường (hạn chế đường, tinh bột)
    (1, 15),  -- Mật ong
    (1, 16),  -- Đường cát
    (1, 19),  -- Sữa đặc có đường
    (1, 20),  -- Sữa chua Vinamilk có đường
    (1, 85),  -- Chuối tiêu
    (1, 86),  -- Chuối tây
    (1, 143), -- Khoai tây
    (1, 146), -- Khoai lang
    (1, 147), -- Mì sợi
    (1, 149), -- Bún
    (1, 155), -- Bánh mì
    (1, 159), -- Cơm trắng

    -- 2. Bệnh tim mạch (hạn chế chất béo bão hòa, cholesterol)
    (2, 18),  -- Pho mát
    (2, 60),  -- Xúc xích
    (2, 62),  -- Lạp xưởng
    (2, 63),  -- Chả heo
    (2, 65),  -- Giò bò
    (2, 66),  -- Pa tê
    (2, 67),  -- Dăm bông heo
    (2, 73),  -- Thịt heo mỡ
    (2, 78),  -- Tủy xương heo
    (2, 79),  -- Tủy xương bò
    (2, 80),  -- Dầu oliu (dùng ít)
    (2, 81),  -- Dầu ăn Tường An

    -- 3. Bệnh cao huyết áp (hạn chế muối, chất béo)
    (3, 5),   -- Nước mắm cá
    (3, 6),   -- Mắm tôm loãng
    (3, 7),   -- Magi
    (3, 10),  -- Muối
    (3, 60),  -- Xúc xích
    (3, 62),  -- Lạp xưởng
    (3, 63),  -- Chả heo
    (3, 73),  -- Thịt heo mỡ

    -- 4. Bệnh thận (hạn chế protein, muối)
    (4, 5),   -- Nước mắm cá
    (4, 10),  -- Muối
    (4, 30),  -- Tôm khô
    (4, 32),  -- Tôm biển
    (4, 37),  -- Mực tươi
    (4, 49),  -- Cá hồi
    (4, 71),  -- Thịt heo nạc
    (4, 75),  -- Thịt bò

    -- 5. Bệnh gout (hạn chế purine từ hải sản, thịt đỏ)
    (5, 30),  -- Tôm khô
    (5, 32),  -- Tôm biển
    (5, 37),  -- Mực tươi
    (5, 41),  -- Cua đồng
    (5, 49),  -- Cá hồi
    (5, 53),  -- Thịt dê
    (5, 70),  -- Thịt trâu
    (5, 75),  -- Thịt bò

    -- 6. Không dung nạp lactose (hạn chế sữa)
    (6, 18),  -- Pho mát
    (6, 19),  -- Sữa đặc có đường
    (6, 20),  -- Sữa chua Vinamilk có đường
    (6, 21),  -- Sữa bò tươi

    -- 7. Hội chứng ruột kích thích (hạn chế đồ cay, dầu mỡ)
    (7, 3),   -- Tương ớt
    (7, 11),  -- Hạt tiêu
    (7, 105), -- Ớt đỏ
    (7, 80),  -- Dầu oliu
    (7, 81),  -- Dầu ăn Tường An

    -- 8. Béo phì (hạn chế calo cao, chất béo)
    (8, 60),  -- Xúc xích
    (8, 62),  -- Lạp xưởng
    (8, 63),  -- Chả heo
    (8, 73),  -- Thịt heo mỡ
    (8, 80),  -- Dầu oliu
    (8, 81);  -- Dầu ăn Tường An

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
    (50, N'Gỏi ngó sen tôm thịt', 'Appetizer', 'Salad', N'Gỏi ngó sen tôm thịt chua ngọt, ăn kèm bánh phồng tôm', N'1 đĩa', 220, 15, 20, 8, 4, 3, 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/Ph%E1%BB%93ng_t%C3%B4m.jpg/640px-Ph%E1%BB%93ng_t%C3%B4m.jpg'),
	(51, N'Thịt bò xào hành tây', 'Lunch', 'Main Course', N'Thịt bò xào với hành tây', N'1 đĩa', 132, 11.8, 5.8, 6.9, 5.03, 0.77, 'https://daotaobeptruong.vn/wp-content/uploads/2019/12/thit-bo-xao-hanh-tay.jpg'),
    (52, N'Bầu xào trứng', 'Lunch', 'Main Course', N'Bầu xào với trứng gà', N'1 đĩa', 109, 4, 4, 8.5, 7.2, 1.3, 'https://img-global.cpcdn.com/recipes/8a8df44ab98f2162/400x400cq70/photo.jpg'),
    (53, N'Cá hú kho', 'Lunch', 'Seafood', N'Cá hú kho với nước mắm', N'1 đĩa', 109, 4, 8.5, 4, 7.2, 1.3, 'https://homestory.com.vn/wp-content/uploads/2023/06/ca-hu-kho-tieu-dam-da-hao-com.jpg'),
    (54, N'Cá lóc chiên', 'Lunch', 'Fish', N'Cá lóc chiên giòn', N'1 lát', 169, 14.9, 12.2, 0, 12.2, 0, 'https://khaihoanphuquoc.com.vn/wp-content/uploads/2023/11/kho%CC%82-ca%CC%81-lo%CC%81c-kho-tho%CC%9Bm.jpg'),
    (55, N'Canh bắp cải', 'Lunch', 'Soup', N'Canh bắp cải nấu đơn giản', N'1 chén', 37, 1.8, 2.8, 2.1, 1.98, 0.82, 'https://cdn.tgdd.vn/2021/04/CookRecipe/GalleryStep/7f05d5e84b27b979e036.jpg'),
    (56, N'Canh bầu', 'Lunch', 'Soup', N'Canh bầu nấu với tôm', N'1 chén', 30, 1.2, 1.5, 2.1, 0.98, 0.52, 'https://suckhoedoisong.qltns.mediacdn.vn/324455921873985536/2023/5/22/canh-hen-nau-bau-816769-1684773425707149599131.jpg'),
    (57, N'Canh bí đao', 'Lunch', 'Soup', N'Canh bí đao thanh mát', N'1 chén', 29, 1.2, 1.3, 2.1, 0.78, 0.52, 'https://cdn.tgdd.vn/2021/05/CookRecipe/GalleryStep/thanh-pham-761.jpg'),
    (58, N'Canh cải ngọt', 'Lunch', 'Soup', N'Canh cải ngọt bổ dưỡng', N'1 chén', 30, 1.7, 1.1, 2.1, 0.2, 0.9, 'https://cdn.tgdd.vn/2021/04/CookProduct/caingotthitbamthumb-1200x676-1200x676.jpg'),
    (59, N'Đậu hũ dồn thịt', 'Lunch', 'Main Course', N'Đậu hũ nhồi thịt heo', N'1 miếng lớn', 328, 18.7, 5.3, 25.8, 4.72, 0.58, 'https://cdn-i.vtcnews.vn/resize/th/upload/2024/11/17/dau-hu-3-22133709.jpg'),
    (60, N'Gà kho gừng', 'Lunch', 'Poultry', N'Gà kho với gừng thơm lừng', N'1 đĩa', 301, 21.9, 10.3, 19.1, 9.39, 0.91, 'https://www.cet.edu.vn/wp-content/uploads/2021/03/ga-kho-gung-nghe-vua-thom-ngon.jpg'),
    (61, N'Gà xào sả ớt', 'Lunch', 'Poultry', N'Gà xào sả ớt cay nồng', N'1 đĩa', 272, 20.4, 4.7, 19.1, 4.7, 0, 'https://beptruong.edu.vn/wp-content/uploads/2013/01/ga-xao-sa-ot.jpg'),
    (62, N'Gan heo xào', 'Lunch', 'Poultry', N'Gan heo xào hành tây', N'1 đĩa', 200, 24.8, 3.4, 9.7, 3.25, 0.15, 'https://cdn.tgdd.vn/2021/04/CookProduct/GANHEOXAOTOImemngotbeobeo-monngondelam5-44screenshot-1200x676.jpg'),
    (63, N'Thịt heo quay', 'Lunch', 'Meat', N'Thịt heo quay giòn da', N'1 đĩa', 250, 7, 23.7, 14.1, 22.32, 1.38, 'https://cdn.tgdd.vn/Files/2021/08/03/1372804/bi-quyet-che-bien-thit-heo-quay-chao-gion-tan-khong-bi-vang-dau-202108040638158166.jpg'),
    (64, N'Thịt bò xào đậu que', 'Lunch', 'Meat', N'Thịt bò xào đậu que giòn', N'1 đĩa', 195, 16.8, 16.6, 6.9, 15.35, 1.25, 'https://cdn.tgdd.vn/2021/07/CookProduct/thumct1-1200x676.jpg'),
    (65, N'Thịt bò xào nấm rơm', 'Lunch', 'Meat', N'Thịt bò xào nấm rơm thơm', N'1 đĩa', 152, 13.5, 2.9, 9.6, 1.98, 0.92, 'https://anhhoangthy.com/wp-content/uploads/2024/06/cach-lam-thit-bo-xao-nam-3.jpeg'),
    (66, N'Thịt heo xào giá hẹ', 'Lunch', 'Meat', N'Thịt heo xào giá và hẹ', N'1 đĩa', 188, 19.3, 4.8, 10.2, 2.93, 1.87, 'https://cdn3.ivivu.com/2020/12/gia-do-xao-thit-ivivu-1.jpg'),
    (67, N'Thịt heo kho tiêu', 'Lunch', 'Meat', N'Thịt heo kho với hạt tiêu', N'1 đĩa', 200, 21.2, 11.5, 7.6, 11.33, 0.17, 'https://cdn.tgdd.vn/2021/01/CookProduct/Thitkhotieu-1200x676.jpg'),
    (68, N'Thịt kho trứng', 'Lunch', 'Meat', N'Thịt heo kho với trứng gà', N'1 trứng+2 miếng thịt', 315, 19.8, 7.5, 22.9, 7.5, 0, 'https://cdn.tgdd.vn/Files/2017/03/28/965845/cach-lam-thit-kho-trung-thom-ngon-dam-da-bat-com-tai-nha-202202261110487084.jpg'),
    (69, N'Bún thịt nướng', 'Breakfast', 'Noodle', N'Bún với thịt nướng thơm', N'1 tô', 451, 14.7, 67.3, 13.7, 63.34, 3.96, 'https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/Bun_thit_nuong.jpg/1200px-Bun_thit_nuong.jpg'),
    (70, N'Bún riêu cua', 'Breakfast', 'Noodle', N'Bún riêu cua truyền thống', N'1 tô', 414, 17.8, 58, 12.2, 55.24, 2.76, 'https://cdn.tgdd.vn/2020/08/CookProduct/Untitled-1-1200x676-10.jpg'),
    (71, N'Đậu hủ chiên sả', 'Lunch', 'Main Course', N'Đậu hủ chiên với sả', N'1 miếng', 148, 11.6, 0.7, 11, 0.3, 0.4, 'https://i.ytimg.com/vi/cv2CfQjZoVY/sddefault.jpg'),
    (72, N'Đậu hủ sốt cà', 'Lunch', 'Main Course', N'Đậu hủ sốt cà chua', N'1 đĩa', 239, 18.1, 11, 13.6, 9.56, 1.44, 'https://cdn.tgdd.vn/2021/05/CookRecipe/GalleryStep/thanh-pham-1052.jpg'),
    (73, N'Hủ tiếu bò kho', 'Breakfast', 'Noodle', N'Hủ tiếu với bò kho', N'1 tô', 410, 17, 55.4, 13.4, 52.2, 3.2, 'https://tiki.vn/blog/wp-content/uploads/2023/04/hu-tieu-bo-kho-3.jpg'),
    (74, N'Bánh canh thịt heo', 'Breakfast', 'Noodle', N'Bánh canh với thịt heo', N'1 tô', 322, 12.8, 48.5, 8.5, 47.5, 1, 'https://cdn3.ivivu.com/2021/05/banh-canh-thit-bam-ivivu-1.jpg'),
    (75, N'Bánh canh giò heo', 'Breakfast', 'Noodle', N'Bánh canh với giò heo', N'1 tô', 483, 19, 48.6, 23.6, 47.59, 1.01, 'https://daynauan.info.vn/wp-content/uploads/2019/05/banh-canh-gio-heo.jpg'),
    (76, N'Miến gà', 'Breakfast', 'Noodle', N'Miến nấu với thịt gà', N'1 tô', 635, 17.8, 100.2, 18.1, 93.8, 6.4, 'https://daynauan.info.vn/wp-content/uploads/2020/04/mien-ga-la-mon-an-dan-da.jpg'),
    (77, N'Bánh bao nhân thịt', 'Breakfast', 'Bread', N'Bánh bao nhân thịt heo', N'1 cái', 328, 16.1, 48.1, 7.9, 47.2, 0.9, 'https://thophat.com/wp-content/uploads/2022/03/BB-Thit-Heo-1.jpg'),
    (78, N'Bánh cuốn chả thịt', 'Breakfast', 'Rice', N'Bánh cuốn với chả và thịt', N'1 đĩa', 590, 25.7, 64.3, 25.6, 62.77, 1.53, 'https://file.hstatic.net/200000667673/file/045b2aae-3699-4ce7-afec-9f0087c89f62_50c07454f52d4e97bfcf710638ce5b31_grande.jpeg'),
    (79, N'Bắp luộc', 'Dinner', 'Grain', N'Bắp luộc nguyên trái', N'1 trái', 192, 4.5, 37.8, 2.5, 36.42, 1.38, 'https://cdn.tgdd.vn/2022/04/CookDishThumb/3-cach-luoc-bap-nhanh-mem-ngon-ngot-sieu-don-gian-ai-cung-lam-thumb-620x620.jpeg'),
    (80, N'Khoai lang luộc', 'Dinner', 'Grain', N'Khoai lang luộc bổ dưỡng', N'1 củ', 131, 0.3, 30.6, 0.3, 29.7, 0.9, 'https://images2.thanhnien.vn/528068263637045248/2023/11/30/sk291103-anh1-17013387164901983869702.jpg'),
    (81, N'Xôi bắp', 'Breakfast', 'Rice', N'Xôi nấu với bắp', N'1 gói', 313, 8.2, 51.3, 8.3, 49.75, 1.55, 'https://i.ytimg.com/vi/uS-FBhXhemE/maxresdefault.jpg'),
    (82, N'Hủ tiếu thịt heo', 'Breakfast', 'Noodle', N'Hủ tiếu với thịt heo', N'1 tô', 361, 14.4, 47.8, 12.5, 46.57, 1.23, 'https://i.ytimg.com/vi/E2bG25UOySg/sddefault.jpg'),
    (83, N'Canh bún', 'Breakfast', 'Noodle', N'Canh bún truyền thống', N'1 tô', 296, 13.6, 44.6, 6.9, 43.05, 1.55, 'https://cdn.tgdd.vn/Files/2019/10/16/1209065/cach-nau-canh-bun-chuan-vi-an-la-ghien-201910160754243097.jpg'),
    (84, N'Bánh canh thịt gà', 'Breakfast', 'Noodle', N'Bánh canh với thịt gà', N'1 tô', 346, 12.8, 48.5, 11.1, 47.5, 1, 'https://cdn.tgdd.vn/Files/2020/12/19/1314847/cach-nau-banh-canh-ga-ngon-mieng-thit-ngot-da-gion-ca-nha-thich-me-202012191421273526.jpg'),
    (85, N'Sườn ram', 'Dinner', 'Meat', N'Sườn heo ram mặn ngọt', N'1 miếng', 264, 8.3, 46.7, 5.8, 46.51, 0.19, 'https://i.ytimg.com/vi/UlnY2tlt5rE/maxresdefault.jpg'),
    (86, N'Cơm chiên dương châu', 'Lunch', 'Rice', N'Cơm chiên với trứng và rau củ', N'1 đĩa', 530, 14.9, 92.7, 11.3, 91.14, 1.56, 'https://www.cet.edu.vn/wp-content/uploads/2018/03/com-chien-duong-chau.jpg'),
    (87, N'Bún xào', 'Breakfast', 'Noodle', N'Bún xào với thịt và rau', N'1 đĩa', 570, 23.4, 56, 28, 53.83, 2.17, 'https://cdn.tgdd.vn/2021/04/CookProduct/2cachlambunxaoraucai-1200x676.jpg'),
    (88, N'Thịt heo xào đậu que', 'Lunch', 'Main Course', N'Thịt heo xào đậu que giòn', N'1 đĩa', 240, 20.5, 16.6, 10.2, 15.35, 1.25, 'https://cdn.tgdd.vn/Files/2019/06/15/1173363/cach-lam-dau-que-xao-thit-heo-gion-ngon-8_800x450.jpg'),
    (89, N'Thịt bò xào măng', 'Lunch', 'Main Course', N'Thịt bò xào măng tươi', N'1 đĩa', 104, 10.5, 0, 6.9, 0, 0, 'https://img-global.cpcdn.com/recipes/nvmgfm5opnwl1hqdgnda/400x400cq70/photo.jpg'),
    (90, N'Thịt bò xào giá hẹ', 'Lunch', 'Main Course', N'Thịt bò xào giá và hẹ', N'1 đĩa', 143, 15.6, 4.8, 6.9, 2.93, 1.87, 'https://img-global.cpcdn.com/recipes/2bbec9a18fc6657b/680x482cq70/th%E1%BB%8Bt-bo-xao-gia-h%E1%BA%B9-recipe-main-photo.jpg'),
    (91, N'Chả lụa kho', 'Lunch', 'Meat', N'Chả lụa kho với nước mắm', N'1 khoanh', 102, 11.7, 3.5, 4.6, 3.49, 0.01, 'https://cdn.tgdd.vn/2020/11/CookRecipe/GalleryStep/thanh-pham-690.jpg'),
    (92, N'Cá lóc kho', 'Lunch', 'Seafood', N'Cá lóc kho tộ', N'1 lát', 131, 15.7, 8.7, 3.8, 8.66, 0.04, 'https://cdn.tgdd.vn/Files/2019/09/02/1194292/cach-lam-ca-loc-kho-to-ngon-com-chuan-vi-mien-nam-202201041313092690.jpg'),
    (93, N'Bún riêu ốc', 'Breakfast', 'Soup', N'Bún riêu với ốc', N'1 tô', 531, 28.4, 65.5, 17.2, 62.77, 2.73, 'https://cdn.tgdd.vn/2021/04/CookProduct/thum-1200x676-25.jpg'),
    (94, N'Chả giò tôm thịt', 'Snack', 'Wrap', N'Chả giò nhân tôm và thịt', N'3 cuốn', 300, 12, 25, 18, 23, 2, 'https://cdn.tgdd.vn/Files/2020/01/16/1231776/cach-lam-cha-gio-tom-gion-rum-an-hoai-khong-chan-202110261450249960.jpg'),
    (95, N'Gỏi bắp chuối', 'Lunch', 'Salad', N'Gỏi bắp chuối trộn tôm', N'1 đĩa', 180, 8, 20, 6, 17, 3, 'https://img-global.cpcdn.com/recipes/0c74d70196aac354/1200x630cq70/photo.jpg'),
    (96, N'Rau muống luộc', 'Dinner', 'Main Course', N'Rau muống luộc chấm mắm', N'1 đĩa', 60, 3, 10, 1, 8, 2, 'https://i-giadinh.vnecdn.net/2024/05/30/Buoc-4-Anh-dai-dien-4-3836-1717053570.jpg'),
    (97, N'Canh mồng tơi tôm', 'Dinner', 'Soup', N'Canh mồng tơi nấu tôm', N'1 chén', 80, 6, 5, 3, 4, 1, 'https://cdn.tgdd.vn/2021/01/CookProduct/Untitled-1-1200x676-1.jpg'),
    (98, N'Cá thu chiên nước mắm', 'Dinner', 'Fish', N'Cá thu chiên với nước mắm', N'1 miếng', 280, 22, 5, 20, 5, 0, 'https://cdn.tgdd.vn/Files/2020/05/24/1257970/2-cach-lam-ca-thu-chien-nuoc-mam-va-chien-gion-tho-760x367.png'),
	(99, N'Thịt gà nướng mật ong', 'Dinner', 'Poultry', N'Gà nướng ướp mật ong', N'1 đĩa', 320, 25, 10, 18, 10, 0, 'https://www.cet.edu.vn/wp-content/uploads/2018/03/ga-nuong-mat-ong.jpg'),
	(100, N'Bánh mì xíu mại', 'Breakfast', 'Bread', N'Bánh mì xíu mại đầy đủ', N'1 ổ', 400, 15, 50, 18, 48, 2, 'https://cdn.tgdd.vn/2021/09/CookDish/cach-lam-banh-mi-xiu-mai-trung-muoi-sieu-ngon-hap-dan-cho-bua-avt-1200x676-1.jpg'),
	(101, N'Cơm trắng', 'Main', 'Rice', N'Cơm trắng', N'1 chén', 130, 2.7, 28.2, 0.3, 0.05, 0.4, 'https://cdn.nhathuoclongchau.com.vn/unsafe/800x0/https://cms-prod.s3-sgn09.fptcloud.com/4_tac_dung_bat_ngo_tu_viec_an_com_gao_trang_1_235f8a42fd.png');

SET IDENTITY_INSERT Food OFF;

-- Bật chế độ IDENTITY_INSERT
SET IDENTITY_INSERT CuisineType ON;

-- Thực hiện chèn dữ liệu
INSERT INTO CuisineType (CuisineID, CuisineName) VALUES
(1, N'Ẩm thực miền Bắc'),
(2, N'Ẩm thực miền Trung'),
(3, N'Ẩm thực miền Nam'),
(4, N'Ẩm thực Tây Nguyên'),
(5, N'Ẩm thực Nam Bộ'),
(6, N'Ẩm thực Trung Hoa');

-- Tắt chế độ IDENTITY_INSERT
SET IDENTITY_INSERT CuisineType OFF;

-- Insert data into MealPlan
SET IDENTITY_INSERT MealPlan ON;

-- Thêm 10 thực đơn mẫu
INSERT INTO MealPlan (MealPlanID, UserID, PlanName, HealthGoal, Duration, CreatedBy, UpdatedBy, Status)
VALUES
    (1, 2, N'Thực đơn tăng cân - Cao năng lượng', N'Tăng cân', 7, N'Nutritionist', N'Nutritionist', 'Active'),
    (2, 2, N'Thực đơn tăng cân - Tăng cơ', N'Tăng cân', 7, N'Nutritionist', N'Nutritionist', 'Active'),

    (3, 2, N'Thực đơn giảm cân - Thấp năng lượng', N'Giảm cân', 7, N'Nutritionist', N'Nutritionist', 'Active'),
    (4, 2, N'Thực đơn giảm cân - Ít tinh bột', N'Giảm cân', 7, N'Nutritionist', N'Nutritionist', 'Active'),

    (5, 2, N'Thực đơn duy trì cân nặng - Cân bằng', N'Duy trì cân nặng', 7, N'Nutritionist', N'Nutritionist', 'Active'),
    (6, 2, N'Thực đơn duy trì cân nặng - Đủ chất', N'Duy trì cân nặng', 7, N'Nutritionist', N'Nutritionist', 'Active'),

    (7, 2, N'Thực đơn duy trì cân cho người tiểu đường', N'Duy trì cân nặng', 7, N'Nutritionist', N'Nutritionist', 'Active'),
    (8, 2, N'Thực đơn giảm cân cho béo phì cấp 1', N'Giảm cân', 7, N'Nutritionist', N'Nutritionist', 'Active'),
    (9, 2, N'Thực đơn giảm cân cho béo phì cấp 2', N'Giảm cân', 7, N'Nutritionist', N'Nutritionist', 'Active'),
    (10, 2, N'Thực đơn tăng cân cho người tim mạch', N'Tăng cân', 7, N'Nutritionist', N'Nutritionist', 'Active');

SET IDENTITY_INSERT MealPlan OFF;

-- Insert data into MealPlanDetail
-- Thực đơn 1: Tăng cân - Cao năng lượng (~3000 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (1, 76, N'Miến gà', 1, N'Breakfast', 1, 635, 100.2, 18.1, 17.8),
    (1, 86, N'Cơm chiên dương châu', 2, N'Lunch', 1, 1060, 185.4, 22.6, 29.8),
    (1, 77, N'Bánh bao nhân thịt', 2, N'Snacks', 1, 656, 96.2, 15.8, 32.2),
    (1, 31, N'Cơm gà', 1, N'Dinner', 1, 450, 50, 12, 35),
    -- Ngày 2
    (1, 3, N'Cơm tấm sườn', 1, N'Breakfast', 2, 600, 70, 20, 30),
    (1, 34, N'Bún bò Huế', 2, N'Lunch', 2, 960, 110, 24, 60),
    (1, 94, N'Chả giò tôm thịt', 2, N'Snacks', 2, 600, 50, 36, 24),
    (1, 43, N'Cơm gà', 1, N'Dinner', 2, 450, 50, 12, 35),
    -- Ngày 3
    (1, 69, N'Bún thịt nướng', 1, N'Breakfast', 3, 451, 67.3, 13.7, 14.7),
    (1, 1, N'Phở bò', 2, N'Lunch', 3, 900, 100, 20, 50),
    (1, 17, N'Bánh chưng', 1, N'Dinner', 3, 350, 45, 12, 15),
    -- Ngày 4
    (1, 78, N'Bánh cuốn chả thịt', 1, N'Breakfast', 4, 590, 64.3, 25.6, 25.7),
    (1, 15, N'Bánh canh cua', 1, N'Lunch', 4, 400, 40, 15, 20),
    (1, 49, N'Bánh khoai mì nướng', 2, N'Snacks', 4, 560, 80, 24, 6),
    (1, 60, N'Gà kho gừng', 1, N'Dinner', 4, 301, 10.3, 19.1, 21.9),
    -- Ngày 5
    (1, 37, N'Mì Quảng', 1, N'Breakfast', 5, 500, 55, 15, 35),
    (1, 33, N'Cơm gà', 1, N'Lunch', 5, 450, 50, 12, 35),
    (1, 7, N'Chả giò', 3, N'Snacks', 5, 450, 30, 24, 15),
    (1, 68, N'Thịt kho trứng', 1, N'Dinner', 5, 315, 7.5, 22.9, 19.8),
    -- Ngày 6
    (1, 75, N'Bánh canh giò heo', 1, N'Breakfast', 6, 483, 48.6, 23.6, 19),
    (1, 86, N'Cơm chiên dương châu', 2, N'Lunch', 6, 1060, 185.4, 22.6, 29.8),
    (1, 63, N'Thịt heo quay', 1, N'Dinner', 6, 250, 23.7, 14.1, 7),
    -- Ngày 7
    (1, 4, N'Bún chả', 2, N'Breakfast', 7, 800, 100, 30, 40),
    (1, 32, N'Cơm tấm sườn', 1, N'Lunch', 7, 600, 70, 20, 30),
    (1, 20, N'Bánh tráng trộn', 2, N'Snacks', 7, 500, 60, 20, 10),
    (1, 99, N'Thịt gà nướng mật ong', 1, N'Dinner', 7, 320, 10, 18, 25);

-- Thực đơn 2: Tăng cân - Tăng cơ (~2800 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (2, 3, N'Cơm tấm sườn', 1, N'Breakfast', 1, 600, 70, 20, 30),
    (2, 24, N'Cá hồi áp chảo', 1, N'Lunch', 1, 400, 10, 20, 40),
    (2, 94, N'Chả giò tôm thịt', 2, N'Snacks', 1, 600, 50, 36, 24),
    (2, 43, N'Cơm gà', 1, N'Dinner', 1, 450, 50, 12, 35),
    -- Ngày 2
    (2, 37, N'Mì Quảng', 1, N'Breakfast', 2, 500, 55, 15, 35),
    (2, 33, N'Cơm chiên dương châu', 1, N'Lunch', 2, 530, 92.7, 11.3, 14.9),
    (2, 10, N'Cá kho tộ', 1, N'Dinner', 2, 400, 10, 20, 25),
    -- Ngày 3
    (2, 69, N'Bún thịt nướng', 1, N'Breakfast', 3, 451, 67.3, 13.7, 14.7),
    (2, 1, N'Phở bò', 1, N'Lunch', 3, 450, 50, 10, 25),
    (2, 77, N'Bánh bao nhân thịt', 2, N'Snacks', 3, 656, 96.2, 15.8, 32.2),
    (2, 60, N'Gà kho gừng', 1, N'Dinner', 3, 301, 10.3, 19.1, 21.9),
    -- Ngày 4
    (2, 78, N'Bánh cuốn chả thịt', 1, N'Breakfast', 4, 590, 64.3, 25.6, 25.7),
    (2, 31, N'Cơm chiên dương châu', 1, N'Lunch', 4, 530, 92.7, 11.3, 14.9),
    (2, 15, N'Bánh canh cua', 1, N'Dinner', 4, 400, 40, 15, 20),
    -- Ngày 5
    (2, 34, N'Bún bò Huế', 1, N'Breakfast', 5, 480, 55, 12, 30),
    (2, 24, N'Cá hồi áp chảo', 2, N'Lunch', 5, 800, 20, 40, 80),
    (2, 7, N'Chả giò', 2, N'Snacks', 5, 300, 20, 16, 10),
    (2, 99, N'Thịt gà nướng mật ong', 1, N'Dinner', 5, 320, 10, 18, 25),
    -- Ngày 6
    (2, 75, N'Bánh canh giò heo', 1, N'Breakfast', 6, 483, 48.6, 23.6, 19),
    (2, 32, N'Cơm gà', 1, N'Lunch', 6, 450, 50, 12, 35),
    (2, 68, N'Thịt kho trứng', 1, N'Dinner', 6, 315, 7.5, 22.9, 19.8),
    -- Ngày 7
    (2, 4, N'Bún chả', 1, N'Breakfast', 7, 400, 50, 15, 20),
    (2, 86, N'Cơm chiên dương châu', 2, N'Lunch', 7, 1060, 185.4, 22.6, 29.8),
    (2, 49, N'Bánh khoai mì nướng', 1, N'Snacks', 7, 280, 40, 12, 3),
    (2, 43, N'Cơm gà', 1, N'Dinner', 7, 450, 50, 12, 35);

-- Thực đơn 3: Giảm cân - Thấp năng lượng (~1200 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (3, 22, N'Cháo yến mạch', 1, N'Breakfast', 1, 250, 40, 6, 8),
    (3, 21, N'Salad ức gà', 1, N'Lunch', 1, 300, 20, 8, 35),
    (3, 23, N'Smoothie bơ chuối', 1, N'Snacks', 1, 220, 30, 10, 5),
    (3, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 1, 80, 5, 3, 6),
    -- Ngày 2
    (3, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 2, 200, 30, 5, 6),
    (3, 30, N'Salad cá ngừ', 1, N'Lunch', 2, 350, 15, 10, 40),
    (3, 96, N'Rau muống luộc', 1, N'Dinner', 2, 60, 10, 1, 3),
    -- Ngày 3
    (3, 5, N'Gỏi cuốn', 2, N'Breakfast', 3, 200, 20, 6, 10),
    (3, 24, N'Cá hồi áp chảo', 1, N'Lunch', 3, 400, 10, 20, 40),
    (3, 55, N'Canh bắp cải', 2, N'Dinner', 3, 74, 5.6, 4.2, 3.6),
    -- Ngày 4
    (3, 29, N'Súp bí đỏ', 1, N'Breakfast', 4, 230, 35, 8, 6),
    (3, 95, N'Gỏi bắp chuối', 1, N'Lunch', 4, 180, 20, 6, 8),
    (3, 27, N'Trà gừng mật ong', 1, N'Snacks', 4, 100, 25, 0, 0),
    (3, 98, N'Cá thu chiên nước mắm', 1, N'Dinner', 4, 280, 5, 20, 22),
    -- Ngày 5
    (3, 22, N'Cháo yến mạch', 1, N'Breakfast', 5, 250, 40, 6, 8),
    (3, 50, N'Gỏi ngó sen tôm thịt', 1, N'Lunch', 5, 220, 20, 8, 15),
    (3, 56, N'Canh bầu', 2, N'Dinner', 5, 60, 3, 4.2, 2.4),
    -- Ngày 6
    (3, 25, N'Súp lơ hấp', 1, N'Breakfast', 6, 180, 20, 10, 5),
    (3, 21, N'Salad ức gà', 1, N'Lunch', 6, 300, 20, 8, 35),
    (3, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 6, 80, 5, 3, 6),
    -- Ngày 7
    (3, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 7, 200, 30, 5, 6),
    (3, 30, N'Salad cá ngừ', 1, N'Lunch', 7, 350, 15, 10, 40),
    (3, 57, N'Canh bí đao', 2, N'Dinner', 7, 58, 2.6, 4.2, 2.4);

-- Thực đơn 4: Giảm cân - Ít tinh bột (~1300 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (4, 5, N'Gỏi cuốn', 2, N'Breakfast', 1, 200, 20, 6, 10),
    (4, 24, N'Cá hồi áp chảo', 1, N'Lunch', 1, 400, 10, 20, 40),
    (4, 95, N'Gỏi bắp chuối', 1, N'Snacks', 1, 180, 20, 6, 8),
    (4, 98, N'Cá thu chiên nước mắm', 1, N'Dinner', 1, 280, 5, 20, 22),
    -- Ngày 2
    (4, 21, N'Salad ức gà', 1, N'Breakfast', 2, 300, 20, 8, 35),
    (4, 40, N'Cá nướng', 1, N'Lunch', 2, 400, 10, 20, 35),
    (4, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 2, 80, 5, 3, 6),
    -- Ngày 3
    (4, 25, N'Súp lơ hấp', 1, N'Breakfast', 3, 180, 20, 10, 5),
    (4, 30, N'Salad cá ngừ', 1, N'Lunch', 3, 350, 15, 10, 40),
    (4, 27, N'Trà gừng mật ong', 1, N'Snacks', 3, 100, 25, 0, 0),
    (4, 54, N'Cá lóc chiên', 1, N'Dinner', 3, 169, 12.2, 0, 14.9),
    -- Ngày 4
    (4, 22, N'Cháo yến mạch', 1, N'Breakfast', 4, 250, 40, 6, 8),
    (4, 24, N'Cá hồi áp chảo', 1, N'Lunch', 4, 400, 10, 20, 40),
    (4, 96, N'Rau muống luộc', 1, N'Dinner', 4, 60, 10, 1, 3),
    -- Ngày 5
    (4, 5, N'Gỏi cuốn', 2, N'Breakfast', 5, 200, 20, 6, 10),
    (4, 99, N'Thịt gà nướng mật ong', 1, N'Lunch', 5, 320, 10, 18, 25),
    (4, 23, N'Smoothie bơ chuối', 1, N'Snacks', 5, 220, 30, 10, 5),
    (4, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 5, 80, 5, 3, 6),
    -- Ngày 6
    (4, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 6, 200, 30, 5, 6),
    (4, 21, N'Salad ức gà', 1, N'Lunch', 6, 300, 20, 8, 35),
    (4, 55, N'Canh bắp cải', 2, N'Dinner', 6, 74, 5.6, 4.2, 3.6),
    -- Ngày 7
    (4, 29, N'Súp bí đỏ', 1, N'Breakfast', 7, 230, 35, 8, 6),
    (4, 30, N'Salad cá ngừ', 1, N'Lunch', 7, 350, 15, 10, 40),
    (4, 98, N'Cá thu chiên nước mắm', 1, N'Dinner', 7, 280, 5, 20, 22);

-- Thực đơn 5: Duy trì cân nặng - Cân bằng (~2000 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (5, 69, N'Bún thịt nướng', 1, N'Breakfast', 1, 451, 67.3, 13.7, 14.7),
    (5, 1, N'Phở bò', 1, N'Lunch', 1, 450, 50, 10, 25),
    (5, 13, N'Bánh bèo', 1, N'Snacks', 1, 250, 30, 10, 8),
    (5, 60, N'Gà kho gừng', 1, N'Dinner', 1, 301, 10.3, 19.1, 21.9),
    -- Ngày 2
    (5, 34, N'Bún bò Huế', 1, N'Breakfast', 2, 480, 55, 12, 30),
    (5, 86, N'Cơm chiên dương châu', 1, N'Lunch', 2, 530, 92.7, 11.3, 14.9),
    (5, 24, N'Cá hồi áp chảo', 1, N'Dinner', 2, 400, 10, 20, 40),
    -- Ngày 3
    (5, 3, N'Cơm tấm sườn', 1, N'Breakfast', 3, 600, 70, 20, 30),
    (5, 43, N'Cơm gà', 1, N'Lunch', 3, 450, 50, 12, 35),
    (5, 7, N'Chả giò', 1, N'Snacks', 3, 150, 10, 8, 5),
    (5, 8, N'Canh chua cá lóc', 1, N'Dinner', 3, 200, 10, 5, 15),
    -- Ngày 4
    (5, 4, N'Bún chả', 1, N'Breakfast', 4, 400, 50, 15, 20),
    (5, 15, N'Bánh canh cua', 1, N'Lunch', 4, 400, 40, 15, 20),
    (5, 99, N'Thịt gà nướng mật ong', 1, N'Dinner', 4, 320, 10, 18, 25),
    -- Ngày 5
    (5, 37, N'Mì Quảng', 1, N'Breakfast', 5, 500, 55, 15, 35),
    (5, 10, N'Cá kho tộ', 1, N'Lunch', 5, 400, 10, 20, 25),
    (5, 94, N'Chả giò tôm thịt', 1, N'Snacks', 5, 300, 25, 18, 12),
    (5, 60, N'Gà kho gừng', 1, N'Dinner', 5, 301, 10.3, 19.1, 21.9),
    -- Ngày 6
    (5, 78, N'Bánh cuốn chả thịt', 1, N'Breakfast', 6, 590, 64.3, 25.6, 25.7),
    (5, 31, N'Lẩu gà lá é', 1, N'Lunch', 6, 600, 50, 20, 40),
    (5, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 6, 80, 5, 3, 6),
    -- Ngày 7
    (5, 69, N'Bún thịt nướng', 1, N'Breakfast', 7, 451, 67.3, 13.7, 14.7),
    (5, 24, N'Cá hồi áp chảo', 1, N'Lunch', 7, 400, 10, 20, 40),
    (5, 43, N'Cơm gà', 1, N'Dinner', 7, 450, 50, 12, 35);

-- Thực đơn 6: Duy trì cân nặng - Đủ chất (~2100 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (6, 34, N'Bún bò Huế', 1, N'Breakfast', 1, 480, 55, 12, 30),
    (6, 86, N'Cơm chiên dương châu', 1, N'Lunch', 1, 530, 92.7, 11.3, 14.9),
    (6, 42, N'Bánh bột lọc', 1, N'Snacks', 1, 270, 35, 8, 8),
    (6, 24, N'Cá hồi áp chảo', 1, N'Dinner', 1, 400, 10, 20, 40),
    -- Ngày 2
    (6, 3, N'Cơm tấm sườn', 1, N'Breakfast', 2, 600, 70, 20, 30),
    (6, 15, N'Bánh canh cua', 1, N'Lunch', 2, 400, 40, 15, 20),
    (6, 99, N'Thịt gà nướng mật ong', 1, N'Dinner', 2, 320, 10, 18, 25),
    -- Ngày 3
    (6, 69, N'Bún thịt nướng', 1, N'Breakfast', 3, 451, 67.3, 13.7, 14.7),
    (6, 31, N'Lẩu gà lá é', 1, N'Lunch', 3, 600, 50, 20, 40),
    (6, 8, N'Canh chua cá lóc', 1, N'Dinner', 3, 200, 10, 5, 15),
    -- Ngày 4
    (6, 4, N'Bún chả', 1, N'Breakfast', 4, 400, 50, 15, 20),
    (6, 43, N'Cơm gà', 1, N'Lunch', 4, 450, 50, 12, 35),
    (6, 60, N'Gà kho gừng', 1, N'Dinner', 4, 301, 10.3, 19.1, 21.9),
    -- Ngày 5
    (6, 37, N'Mì Quảng', 1, N'Breakfast', 5, 500, 55, 15, 35),
    (6, 24, N'Cá hồi áp chảo', 1, N'Lunch', 5, 400, 10, 20, 40),
    (6, 94, N'Chả giò tôm thịt', 1, N'Snacks', 5, 300, 25, 18, 12),
    (6, 10, N'Cá kho tộ', 1, N'Dinner', 5, 400, 10, 20, 25),
    -- Ngày 6
    (6, 78, N'Bánh cuốn chả thịt', 1, N'Breakfast', 6, 590, 64.3, 25.6, 25.7),
    (6, 1, N'Phở bò', 1, N'Lunch', 6, 450, 50, 10, 25),
    (6, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 6, 80, 5, 3, 6),
    -- Ngày 7
    (6, 69, N'Bún thịt nướng', 1, N'Breakfast', 7, 451, 67.3, 13.7, 14.7),
    (6, 33, N'Lẩu bò nhúng giấm', 1, N'Lunch', 7, 750, 40, 30, 60),
    (6, 24, N'Cá hồi áp chảo', 1, N'Dinner', 7, 400, 10, 20, 40);

	-- Thực đơn 7: Duy trì cân nặng - Người tiểu đường (~1800 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (7, 22, N'Cháo yến mạch', 1, N'Breakfast', 1, 250, 40, 6, 8),
    (7, 30, N'Salad cá ngừ', 1, N'Lunch', 1, 350, 15, 10, 40),
    (7, 25, N'Súp lơ hấp', 1, N'Snacks', 1, 180, 20, 10, 5),
    (7, 24, N'Cá hồi áp chảo', 1, N'Dinner', 1, 400, 10, 20, 40),
    -- Ngày 2
    (7, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 2, 200, 30, 5, 6),
    (7, 21, N'Salad ức gà', 1, N'Lunch', 2, 300, 20, 8, 35),
    (7, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 2, 80, 5, 3, 6),
    -- Ngày 3
    (7, 28, N'Xôi gạo lứt', 1, N'Breakfast', 3, 320, 55, 7, 8),
    (7, 24, N'Cá hồi áp chảo', 1, N'Lunch', 3, 400, 10, 20, 40),
    (7, 25, N'Súp lơ hấp', 1, N'Dinner', 3, 180, 20, 10, 5),
    -- Ngày 4
    (7, 22, N'Cháo yến mạch', 1, N'Breakfast', 4, 250, 40, 6, 8),
    (7, 30, N'Salad cá ngừ', 1, N'Lunch', 4, 350, 15, 10, 40),
    (7, 23, N'Smoothie bơ chuối', 1, N'Snacks', 4, 220, 30, 10, 5),
    (7, 98, N'Cá thu chiên nước mắm', 1, N'Dinner', 4, 280, 5, 20, 22),
    -- Ngày 5
    (7, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 5, 200, 30, 5, 6),
    (7, 40, N'Cá nướng', 1, N'Lunch', 5, 400, 10, 20, 35),
    (7, 8, N'Canh chua cá lóc', 1, N'Dinner', 5, 200, 10, 5, 15),
    -- Ngày 6
    (7, 28, N'Xôi gạo lứt', 1, N'Breakfast', 6, 320, 55, 7, 8),
    (7, 21, N'Salad ức gà', 1, N'Lunch', 6, 300, 20, 8, 35),
    (7, 25, N'Súp lơ hấp', 1, N'Dinner', 6, 180, 20, 10, 5),
    -- Ngày 7
    (7, 22, N'Cháo yến mạch', 1, N'Breakfast', 7, 250, 40, 6, 8),
    (7, 24, N'Cá hồi áp chảo', 1, N'Lunch', 7, 400, 10, 20, 40),
    (7, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 7, 80, 5, 3, 6);

-- Thực đơn 8: Giảm cân - Béo phì cấp 1 (~1400 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (8, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 1, 200, 30, 5, 6),
    (8, 21, N'Salad ức gà', 1, N'Lunch', 1, 300, 20, 8, 35),
    (8, 23, N'Smoothie bơ chuối', 1, N'Snacks', 1, 220, 30, 10, 5),
    (8, 55, N'Canh bắp cải', 2, N'Dinner', 1, 74, 5.6, 4.2, 3.6),
    -- Ngày 2
    (8, 22, N'Cháo yến mạch', 1, N'Breakfast', 2, 250, 40, 6, 8),
    (8, 30, N'Salad cá ngừ', 1, N'Lunch', 2, 350, 15, 10, 40),
    (8, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 2, 80, 5, 3, 6),
    -- Ngày 3
    (8, 96, N'Rau muống luộc', 1, N'Breakfast', 3, 60, 10, 1, 3),
    (8, 24, N'Cá hồi áp chảo', 1, N'Lunch', 3, 400, 10, 20, 40),
    (8, 27, N'Trà gừng mật ong', 1, N'Snacks', 3, 100, 25, 0, 0),
    (8, 25, N'Súp lơ hấp', 1, N'Dinner', 3, 180, 20, 10, 5),
    -- Ngày 4
    (8, 5, N'Gỏi cuốn', 2, N'Breakfast', 4, 200, 20, 6, 10),
    (8, 98, N'Cá thu chiên nước mắm', 1, N'Lunch', 4, 280, 5, 20, 22),
    (8, 8, N'Canh chua cá lóc', 1, N'Dinner', 4, 200, 10, 5, 15),
    -- Ngày 5
    (8, 29, N'Súp bí đỏ', 1, N'Breakfast', 5, 230, 35, 8, 6),
    (8, 21, N'Salad ức gà', 1, N'Lunch', 5, 300, 20, 8, 35),
    (8, 56, N'Canh bầu', 1, N'Dinner', 5, 30, 1.5, 2.1, 1.2),
    -- Ngày 6
    (8, 22, N'Cháo yến mạch', 1, N'Breakfast', 6, 250, 40, 6, 8),
    (8, 30, N'Salad cá ngừ', 1, N'Lunch', 6, 350, 15, 10, 40),
    (8, 95, N'Gỏi bắp chuối', 1, N'Snacks', 6, 180, 20, 6, 8),
    (8, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 6, 80, 5, 3, 6),
    -- Ngày 7
    (8, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 7, 200, 30, 5, 6),
    (8, 24, N'Cá hồi áp chảo', 1, N'Lunch', 7, 400, 10, 20, 40),
    (8, 58, N'Canh cải ngọt', 1, N'Dinner', 7, 30, 1.1, 2.1, 1.7);

-- Thực đơn 9: Giảm cân - Béo phì cấp 2 (~1100 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (9, 96, N'Rau muống luộc', 1, N'Breakfast', 1, 60, 10, 1, 3),
    (9, 30, N'Salad cá ngừ', 1, N'Lunch', 1, 350, 15, 10, 40),
    (9, 27, N'Trà gừng mật ong', 1, N'Snacks', 1, 100, 25, 0, 0),
    (9, 97, N'Canh mồng tơi tôm', 2, N'Dinner', 1, 160, 10, 6, 12),
    -- Ngày 2
    (9, 22, N'Cháo yến mạch', 1, N'Breakfast', 2, 250, 40, 6, 8),
    (9, 21, N'Salad ức gà', 1, N'Lunch', 2, 300, 20, 8, 35),
    (9, 55, N'Canh bắp cải', 1, N'Dinner', 2, 37, 2.8, 2.1, 1.8),
    -- Ngày 3
    (9, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 3, 200, 30, 5, 6),
    (9, 24, N'Cá hồi áp chảo', 1, N'Lunch', 3, 400, 10, 20, 40),
    (9, 23, N'Smoothie bơ chuối', 1, N'Snacks', 3, 220, 30, 10, 5),
    (9, 56, N'Canh bầu', 1, N'Dinner', 3, 30, 1.5, 2.1, 1.2),
    -- Ngày 4
    (9, 5, N'Gỏi cuốn', 2, N'Breakfast', 4, 200, 20, 6, 10),
    (9, 98, N'Cá thu chiên nước mắm', 1, N'Lunch', 4, 280, 5, 20, 22),
    (9, 25, N'Súp lơ hấp', 1, N'Dinner', 4, 180, 20, 10, 5),
    -- Ngày 5
    (9, 96, N'Rau muống luộc', 1, N'Breakfast', 5, 60, 10, 1, 3),
    (9, 30, N'Salad cá ngừ', 1, N'Lunch', 5, 350, 15, 10, 40),
    (9, 27, N'Trà gừng mật ong', 1, N'Snacks', 5, 100, 25, 0, 0),
    (9, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 5, 80, 5, 3, 6),
    -- Ngày 6
    (9, 22, N'Cháo yến mạch', 1, N'Breakfast', 6, 250, 40, 6, 8),
    (9, 21, N'Salad ức gà', 1, N'Lunch', 6, 300, 20, 8, 35),
    (9, 58, N'Canh cải ngọt', 1, N'Dinner', 6, 30, 1.1, 2.1, 1.7),
    -- Ngày 7
    (9, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 7, 200, 30, 5, 6),
    (9, 24, N'Cá hồi áp chảo', 1, N'Lunch', 7, 400, 10, 20, 40),
    (9, 95, N'Gỏi bắp chuối', 1, N'Snacks', 7, 180, 20, 6, 8),
    (9, 25, N'Súp lơ hấp', 1, N'Dinner', 7, 180, 20, 10, 5);

-- Thực đơn 10: Tăng cân - Người tim mạch (~2500 kcal/ngày)
INSERT INTO MealPlanDetail (MealPlanID, FoodID, FoodName, Quantity, MealType, DayNumber, TotalCalories, TotalCarbs, TotalFat, TotalProtein)
VALUES
    -- Ngày 1
    (10, 28, N'Xôi gạo lứt', 1, N'Breakfast', 1, 320, 55, 7, 8),
    (10, 24, N'Cá hồi áp chảo', 2, N'Lunch', 1, 800, 20, 40, 80),
    (10, 23, N'Smoothie bơ chuối', 1, N'Snacks', 1, 220, 30, 10, 5),
    (10, 99, N'Thịt gà nướng mật ong', 1, N'Dinner', 1, 320, 10, 18, 25),
    -- Ngày 2
    (10, 22, N'Cháo yến mạch', 1, N'Breakfast', 2, 250, 40, 6, 8),
    (10, 30, N'Salad cá ngừ', 1, N'Lunch', 2, 350, 15, 10, 40),
    (10, 40, N'Cá nướng', 1, N'Dinner', 2, 400, 10, 20, 35),
    -- Ngày 3
    (10, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 3, 200, 30, 5, 6),
    (10, 24, N'Cá hồi áp chảo', 1, N'Lunch', 3, 400, 10, 20, 40),
    (10, 25, N'Súp lơ hấp', 1, N'Snacks', 3, 180, 20, 10, 5),
    (10, 21, N'Salad ức gà', 1, N'Dinner', 3, 300, 20, 8, 35),
    -- Ngày 4
    (10, 28, N'Xôi gạo lứt', 1, N'Breakfast', 4, 320, 55, 7, 8),
    (10, 98, N'Cá thu chiên nước mắm', 1, N'Lunch', 4, 280, 5, 20, 22),
    (10, 8, N'Canh chua cá lóc', 1, N'Dinner', 4, 200, 10, 5, 15),
    -- Ngày 5
    (10, 22, N'Cháo yến mạch', 1, N'Breakfast', 5, 250, 40, 6, 8),
    (10, 24, N'Cá hồi áp chảo', 2, N'Lunch', 5, 800, 20, 40, 80),
    (10, 97, N'Canh mồng tơi tôm', 1, N'Dinner', 5, 80, 5, 3, 6),
    -- Ngày 6
    (10, 26, N'Bánh pancake chuối yến mạch', 1, N'Breakfast', 6, 200, 30, 5, 6),
    (10, 30, N'Salad cá ngừ', 1, N'Lunch', 6, 350, 15, 10, 40),
    (10, 23, N'Smoothie bơ chuối', 1, N'Snacks', 6, 220, 30, 10, 5),
    (10, 99, N'Thịt gà nướng mật ong', 1, N'Dinner', 6, 320, 10, 18, 25),
    -- Ngày 7
    (10, 28, N'Xôi gạo lứt', 1, N'Breakfast', 7, 320, 55, 7, 8),
    (10, 24, N'Cá hồi áp chảo', 1, N'Lunch', 7, 400, 10, 20, 40),
    (10, 40, N'Cá nướng', 1, N'Dinner', 7, 400, 10, 20, 35);

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
    (50, 107),  -- Ngó sen
    (50, 32),   -- Tôm biển
    (50, 71),   -- Thịt heo nạc
    (50, 5),    -- Nước mắm cá
    (50, 82),   -- Chanh
    (50, 16),   -- Đường cát
    (50, 93),   -- Tỏi

    -- 51. Thịt bò xào hành tây
    (51, 75),   -- Thịt bò
    (51, 115),  -- Hành tây
    (51, 80),   -- Dầu oliu
    (51, 5),    -- Nước mắm cá
    (51, 10),   -- Muối
    (51, 93),   -- Tỏi

    -- 52. Bầu xào trứng
    (52, 137),  -- Bầu
    (52, 27),   -- Trứng gà
    (52, 80),   -- Dầu oliu
    (52, 10),   -- Muối
    (52, 116),  -- Hành lá

    -- 53. Cá hú kho
    (53, 42),   -- Cá trê (thay cá hú)
    (53, 5),    -- Nước mắm cá
    (53, 16),   -- Đường cát
    (53, 11),   -- Hạt tiêu
    (53, 93),   -- Tỏi

    -- 54. Cá lóc chiên
    (54, 42),   -- Cá trê (thay cá lóc)
    (54, 80),   -- Dầu oliu
    (54, 10),   -- Muối
    (54, 93),   -- Tỏi

    -- 55. Canh bắp cải
    (55, 128),  -- Cải xanh (thay bắp cải)
    (55, 10),   -- Muối
    (55, 116),  -- Hành lá
    (55, 11),   -- Hạt tiêu

    -- 56. Canh bầu
    (56, 137),  -- Bầu
    (56, 32),   -- Tôm biển
    (56, 10),   -- Muối
    (56, 116),  -- Hành lá

    -- 57. Canh bí đao
    (57, 136),  -- Bí xanh (thay bí đao)
    (57, 10),   -- Muối
    (57, 116),  -- Hành lá

    -- 58. Canh cải ngọt
    (58, 128),  -- Cải xanh (thay cải ngọt)
    (58, 10),   -- Muối
    (58, 116),  -- Hành lá

    -- 59. Đậu hũ dồn thịt
    (59, 139),  -- Đậu phụ
    (59, 71),   -- Thịt heo nạc
    (59, 80),   -- Dầu oliu
    (59, 5),    -- Nước mắm cá
    (59, 116),  -- Hành lá

    -- 60. Gà kho gừng
    (60, 74),   -- Thịt gà ta
    (60, 12),   -- Gừng tươi
    (60, 5),    -- Nước mắm cá
    (60, 16),   -- Đường cát
    (60, 11),   -- Hạt tiêu

    -- 61. Gà xào sả ớt
    (61, 74),   -- Thịt gà ta
    (61, 105),  -- Ớt đỏ (thay sả)
    (61, 80),   -- Dầu oliu
    (61, 5),    -- Nước mắm cá
    (61, 93),   -- Tỏi

    -- 62. Gan heo xào
    (62, 57),   -- Tai heo (thay gan heo)
    (62, 115),  -- Hành tây
    (62, 80),   -- Dầu oliu
    (62, 5),    -- Nước mắm cá
    (62, 93),   -- Tỏi

    -- 63. Thịt heo quay
    (63, 73),   -- Thịt heo mỡ
    (63, 5),    -- Nước mắm cá
    (63, 10),   -- Muối
    (63, 93),   -- Tỏi
    (63, 16),   -- Đường cát

    -- 64. Thịt bò xào đậu que
    (64, 75),   -- Thịt bò
    (64, 121),  -- Đậu cô ve (thay đậu que)
    (64, 80),   -- Dầu oliu
    (64, 5),    -- Nước mắm cá
    (64, 93),   -- Tỏi

    -- 65. Thịt bò xào nấm rơm
    (65, 75),   -- Thịt bò
    (65, 90),   -- Nấm rơm
    (65, 80),   -- Dầu oliu
    (65, 5),    -- Nước mắm cá
    (65, 93),   -- Tỏi

    -- 66. Thịt heo xào giá hẹ
    (66, 71),   -- Thịt heo nạc
    (66, 118),  -- Giá
    (66, 113),  -- Hẹ lá
    (66, 80),   -- Dầu oliu
    (66, 5),    -- Nước mắm cá

    -- 67. Thịt heo kho tiêu
    (67, 71),   -- Thịt heo nạc
    (67, 11),   -- Hạt tiêu
    (67, 5),    -- Nước mắm cá
    (67, 16),   -- Đường cát
    (67, 93),   -- Tỏi

    -- 68. Thịt kho trứng
    (68, 71),   -- Thịt heo nạc
    (68, 27),   -- Trứng gà
    (68, 5),    -- Nước mắm cá
    (68, 16),   -- Đường cát
    (68, 11),   -- Hạt tiêu

    -- 69. Bún thịt nướng
    (69, 149),  -- Bún
    (69, 71),   -- Thịt heo nạc
    (69, 5),    -- Nước mắm cá
    (69, 116),  -- Hành lá
    (69, 93),   -- Tỏi

    -- 70. Bún riêu cua
    (70, 149),  -- Bún
    (70, 41),   -- Cua đồng
    (70, 134),  -- Cà chua
    (70, 5),    -- Nước mắm cá
    (70, 139),  -- Đậu phụ

    -- 71. Đậu hủ chiên sả
    (71, 139),  -- Đậu phụ
    (71, 80),   -- Dầu oliu
    (71, 105),  -- Ớt đỏ (thay sả)
    (71, 10),   -- Muối

    -- 72. Đậu hủ sốt cà
    (72, 139),  -- Đậu phụ
    (72, 134),  -- Cà chua
    (72, 80),   -- Dầu oliu
    (72, 5),    -- Nước mắm cá
    (72, 10),   -- Muối

    -- 73. Hủ tiếu bò kho
    (73, 149),  -- Bún (thay hủ tiếu)
    (73, 75),   -- Thịt bò
    (73, 5),    -- Nước mắm cá
    (73, 116),  -- Hành lá
    (73, 132),  -- Cà rốt

    -- 74. Bánh canh thịt heo
    (74, 152),  -- Bột gạo tẻ
    (74, 71),   -- Thịt heo nạc
    (74, 5),    -- Nước mắm cá
    (74, 116),  -- Hành lá
    (74, 10),   -- Muối

    -- 75. Bánh canh giò heo
    (75, 152),  -- Bột gạo tẻ
    (75, 68),   -- Chân giò heo
    (75, 5),    -- Nước mắm cá
    (75, 116),  -- Hành lá
    (75, 10),   -- Muối

    -- 76. Miến gà
    (76, 142),  -- Miến dong
    (76, 74),   -- Thịt gà ta
    (76, 5),    -- Nước mắm cá
    (76, 116),  -- Hành lá
    (76, 10),   -- Muối

    -- 77. Bánh bao nhân thịt
    (77, 151),  -- Bột mì
    (77, 71),   -- Thịt heo nạc
    (77, 5),    -- Nước mắm cá
    (77, 116),  -- Hành lá
    (77, 27),   -- Trứng gà

    -- 78. Bánh cuốn chả thịt
    (78, 152),  -- Bột gạo tẻ
    (78, 64),   -- Giò lụa (thay chả)
    (78, 71),   -- Thịt heo nạc
    (78, 5),    -- Nước mắm cá
    (78, 80),   -- Dầu oliu

    -- 79. Bắp luộc
    (79, 157),  -- Bắp ngô

    -- 80. Khoai lang luộc
    (80, 146),  -- Khoai lang

    -- 81. Xôi bắp
    (81, 160),  -- Cơm gạo nếp
    (81, 157),  -- Bắp ngô
    (81, 80),   -- Dầu oliu

    -- 82. Hủ tiếu thịt heo
    (82, 149),  -- Bún (thay hủ tiếu)
    (82, 71),   -- Thịt heo nạc
    (82, 5),    -- Nước mắm cá
    (82, 116),  -- Hành lá
    (82, 118),  -- Giá

    -- 83. Canh bún
    (83, 149),  -- Bún
    (83, 41),   -- Cua đồng
    (83, 5),    -- Nước mắm cá
    (83, 116),  -- Hành lá
    (83, 134),  -- Cà chua

    -- 84. Bánh canh thịt gà
    (84, 152),  -- Bột gạo tẻ
    (84, 74),   -- Thịt gà ta
    (84, 5),    -- Nước mắm cá
    (84, 116),  -- Hành lá
    (84, 10),   -- Muối

    -- 85. Sườn ram
    (85, 56),   -- Sườn heo
    (85, 5),    -- Nước mắm cá
    (85, 16),   -- Đường cát
    (85, 93),   -- Tỏi
    (85, 11),   -- Hạt tiêu

    -- 86. Cơm chiên dương châu
    (86, 159),  -- Cơm trắng
    (86, 27),   -- Trứng gà
    (86, 132),  -- Cà rốt
    (86, 80),   -- Dầu oliu
    (86, 71),   -- Thịt heo nạc

    -- 87. Bún xào
    (87, 149),  -- Bún
    (87, 71),   -- Thịt heo nạc
    (87, 118),  -- Giá
    (87, 80),   -- Dầu oliu
    (87, 5),    -- Nước mắm cá

    -- 88. Thịt heo xào đậu que
    (88, 71),   -- Thịt heo nạc
    (88, 121),  -- Đậu cô ve (thay đậu que)
    (88, 80),   -- Dầu oliu
    (88, 5),    -- Nước mắm cá
    (88, 93),   -- Tỏi

    -- 89. Thịt bò xào măng
    (89, 75),   -- Thịt bò
    (89, 110),  -- Măng chua
    (89, 80),   -- Dầu oliu
    (89, 5),    -- Nước mắm cá
    (89, 93),   -- Tỏi

    -- 90. Thịt bò xào giá hẹ
    (90, 75),   -- Thịt bò
    (90, 118),  -- Giá
    (90, 113),  -- Hẹ lá
    (90, 80),   -- Dầu oliu
    (90, 5),    -- Nước mắm cá

    -- 91. Chả lụa kho
    (91, 64),   -- Giò lụa (thay chả lụa)
    (91, 5),    -- Nước mắm cá
    (91, 16),   -- Đường cát
    (91, 11),   -- Hạt tiêu

    -- 92. Cá lóc kho
    (92, 42),   -- Cá trê (thay cá lóc)
    (92, 5),    -- Nước mắm cá
    (92, 16),   -- Đường cát
    (92, 11),   -- Hạt tiêu
    (92, 93),   -- Tỏi

    -- 93. Bún riêu ốc
    (93, 149),  -- Bún
    (93, 36),   -- Ốc bươu
    (93, 134),  -- Cà chua
    (93, 5),    -- Nước mắm cá
    (93, 139),  -- Đậu phụ

    -- 94. Chả giò tôm thịt
    (94, 152),  -- Bột gạo tẻ
    (94, 32),   -- Tôm biển
    (94, 71),   -- Thịt heo nạc
    (94, 80),   -- Dầu oliu
    (94, 118),  -- Giá

    -- 95. Gỏi bắp chuối
    (95, 85),   -- Chuối tiêu (thay bắp chuối)
    (95, 32),   -- Tôm biển
    (95, 5),    -- Nước mắm cá
    (95, 82),   -- Chanh
    (95, 16),   -- Đường cát

    -- 96. Rau muống luộc
    (96, 102),  -- Rau muống
    (96, 5),    -- Nước mắm cá (dùng để chấm)

    -- 97. Canh mồng tơi tôm
    (97, 103),  -- Rau mồng tơi
    (97, 32),   -- Tôm biển
    (97, 10),   -- Muối
    (97, 116),  -- Hành lá

    -- 98. Cá thu chiên nước mắm
    (98, 44),   -- Cá thu
    (98, 5),    -- Nước mắm cá
    (98, 80),   -- Dầu oliu
    (98, 10),   -- Muối
    (98, 93),   -- Tỏi

    -- 99. Thịt gà nướng mật ong
    (99, 74),   -- Thịt gà ta
    (99, 15),   -- Mật ong
    (99, 5),    -- Nước mắm cá
    (99, 93),   -- Tỏi
    (99, 11),   -- Hạt tiêu

    -- 100. Bánh mì xíu mại
    (100, 155), -- Bánh mì
    (100, 71),  -- Thịt heo nạc
    (100, 27),  -- Trứng gà
    (100, 134), -- Cà chua
    (100, 5),   -- Nước mắm cá
    (100, 80),  -- Dầu oliu

	-- 101. Cơm trắng
	(101, 159);

GO

SET IDENTITY_INSERT SystemConfiguration ON;

INSERT INTO SystemConfiguration (ConfigID, Name, MinValue, MaxValue, Unit, IsActive, Description)
VALUES 
(1, 'UserAge', 13, 100, 'years', 1, N'Độ tuổi đăng ký tài khoản sử dụng hệ thống'),
(2, 'UserHeight', 100, 220, 'cm', 1, N'Mức chiều cao cho phép của người dùng'),
(3, 'UserWeight', 30, 250, 'kg', 1, N'Mức cân nặng cho phép của người dùng'),
(4, 'TargetWeight', 30, 250, 'kg', 1, N'Mục tiêu cân nặng cho phép của người dùng'),
(5, 'NumberAllergy', 0, 10, N'Dị ứng', 1, N'Số dị ứng có thể chọn'),
(6, 'NumberDisease', 0, 5, N'Bệnh', 1, N'Số bệnh có thể chọn');

SET IDENTITY_INSERT SystemConfiguration OFF;
