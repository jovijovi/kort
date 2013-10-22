namespace KORT.Network
{
    public static class FieldKeyword
    {
        public const string Token = "Token";
        public const string Language = "Language";

        public const string Success = "Success";
        public const string ResultType = "ResultType";
        public const string Result = "Result";
        public const string CommonError = "CommonError";
        public const string ErrorDetail = "ErrorDetail";
    }
    public static class ResultType
    {
        public const string Null = "Null";//no object
        public const string Object = "Object";//one object
        public const string Array = "Array";//lots of objects with same type
        public const string List = "List";//logs of objects with different type
        public const string Boolean = "Boolean";//no object
        public const string Message = "Message";//for message only
    }
}