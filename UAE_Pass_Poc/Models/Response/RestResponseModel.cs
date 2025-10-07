namespace UAE_Pass_Poc.Models.Response
{
    public class RestResponseModel<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        //new properties
        public string? Token { get; set; }
        public string? Timestamp { get; set; }
        public string? Code { get; set; }
    }
}
