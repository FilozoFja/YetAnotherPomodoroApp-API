namespace YAPA.Models.Response
{
    public class ResponseModel<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }
}
