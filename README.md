Project này có lẽ dùng database first 
Update database command
```bash
dotnet ef dbcontext scaffold "Server=(local);database=NutriDiet;uid=sa;pwd=12345;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --force
```
