namespace NutriDiet.Common
{
    public class Const
    {
        #region General
        public static int ERROR_EXCEPTION = -1;
        public static string ERROR_EXCEPTION_MSG = "An unexpected error occurred.";
        public static int SUCCESS = 1;
        public static int FAILURE = 0;
        #endregion

        #region Success Messages
        public static string SUCCESS_CREATE_MSG = "Data created successfully.";
        public static string SUCCESS_READ_MSG = "Data retrieved successfully.";
        public static string SUCCESS_UPDATE_MSG = "Data updated successfully.";
        public static string SUCCESS_DELETE_MSG = "Data deleted successfully.";
        public static string SUCCESS_LOGIN_MSG = "Login successful.";
        public static string SUCCESS_LOGOUT_MSG = "Logout successful.";
        #endregion

        #region Failure Messages
        public static string FAIL_CREATE_MSG = "Failed to create data.";
        public static string FAIL_READ_MSG = "Failed to retrieve data.";
        public static string FAIL_UPDATE_MSG = "Failed to update data.";
        public static string FAIL_DELETE_MSG = "Failed to delete data.";
        public static string FAIL_LOGIN_MSG = "Invalid email or password.";
        public static string FAIL_LOGOUT_MSG = "Failed to logout.";
        #endregion
    }
}
