USE NutriDiet;
GO

-- Insert Roles with explicit IDs
INSERT INTO Role (RoleID, RoleName) VALUES
(1, 'Admin'),
(2, 'Customer');

SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserID], [FullName], [Email], [Password], [Phone], [Age], [Gender], [Location], [Avatar], [fcmToken], [Status], [RoleID]) VALUES (1, N'New User', N'user@example.com', N'AQAAAAIAAYagAAAAEJC6r2LMSMDB1nSfXeYadVFihZL+PHOrpKK4g6s0kDy9LRR4sYRlbYbjDh3pF95RZg==', NULL, NULL, NULL, NULL, N'', NULL, N'Active', 2)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
