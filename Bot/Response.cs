namespace MythicNights
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public static Response Success(string message = "")
        {
            if(message == "")
            {
                message = "success";
            }
            return new Response() { IsSuccess = true, Message = message };
        }
        public static Response Failulre(string error)
        {
            return new Response() { IsSuccess = false, Message = error };
        }
    }
}
