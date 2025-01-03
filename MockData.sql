USE NutriDiet;

-- Xóa dữ liệu các bảng liên quan trước khi thêm mới
DELETE FROM Food;
DELETE FROM Role;

-- Thêm vai trò người dùng
INSERT INTO Role (RoleID, RoleName)
VALUES 
    (1, N'Admin'),
    (2, N'Customer');

-- Thêm dữ liệu món ăn
INSERT INTO Food (FoodName, FoodType, Description, ServingSize)
VALUES
(N'Cơm trắng', N'Tinh bột', N'Một món ăn chủ yếu trong bữa cơm của người Việt, cung cấp năng lượng chủ yếu từ tinh bột.', N'1 chén (150g)'),
(N'Thịt gà', N'Thịt', N'Thịt gà là nguồn cung cấp protein và các vitamin như B6, B12.', N'100g'),
(N'Cá hồi', N'Hải sản', N'Cá hồi là nguồn cung cấp omega-3 và vitamin D, tốt cho tim mạch.', N'100g'),
(N'Bắp cải', N'Rau', N'Bắp cải là một loại rau giàu chất xơ, vitamin C và có tác dụng giải độc.', N'1 chén (100g)'),
(N'Khoai tây', N'Tinh bột', N'Khoai tây cung cấp carbohydrate và vitamin C, thích hợp cho các bữa ăn chính.', N'1 củ (150g)'),
(N'Cà chua', N'Rau', N'Cà chua là một nguồn vitamin C tốt, giúp tăng cường hệ miễn dịch.', N'1 quả (120g)'),
(N'Trái bơ', N'Trái cây', N'Bơ là một nguồn chất béo lành mạnh và chứa nhiều vitamin E, giúp bảo vệ tế bào.', N'1 quả (200g)'),
(N'Tôm', N'Hải sản', N'Tôm cung cấp protein và các khoáng chất như kẽm và sắt.', N'100g'),
(N'Lươn', N'Thịt', N'Lươn là nguồn protein dồi dào, dễ tiêu hóa, phù hợp cho người cần phục hồi sức khỏe.', N'100g'),
(N'Măng tây', N'Rau', N'Măng tây là một nguồn vitamin K và folate, giúp duy trì xương khỏe mạnh.', N'100g');
