namespace LoginDotnet.Models.ViewModels
{
    public class ResponseViewModel<T>
    {
        public bool Result { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }
        public ErrorViewModel? Error { get; set; }

    }
    public class ResponseViewModel
    {
        public bool Result { get; set; }
        public string? Message { get; set; }
        public ErrorViewModel? Error { get; set; }

    }


}
