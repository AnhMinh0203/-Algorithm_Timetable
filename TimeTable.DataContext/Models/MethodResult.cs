namespace TimeTable.DataContext.Models
{
    public class MethodResult
    {

        #region declare property
        public int? StatusCode { get; set; } = 200;
        public string Message { get; set; } = "";
        public object Result { get; set; } = new object();
        public int? TotalRecords { get; set; }
        #endregion

        public MethodResult()
        {

        }

        public static MethodResult ResultWithError(string message = "")
        {
            return new MethodResult
            {
                Message = message
            };
        }

        public static MethodResult ResultWithError(object? result = null, int? status = null, string message = "", int totalRecords = 0)
        {
            return new MethodResult
            {
                Result = result,
                Message = message,
                StatusCode = status,
                TotalRecords = totalRecords
            };
        }
        public static MethodResult ResultWithSuccess(object? result = null, int? status = 200, string message = "", int totalRecords = 0)
        {
            return new MethodResult
            {
                Result = result,
                Message = message,
                StatusCode = status,
                TotalRecords = totalRecords
            };
        }
    }
}
