namespace ClickView.GoodStuff.AspNetCore.Authentication.Abstractions
{
    public sealed class UserSession
    {
        public UserSession(string key, byte[] ticket)
        {
            Key = key;
            Ticket = ticket;
        }

        public string Key { get; }
        public byte[] Ticket { get; }
        public string? SessionId { get; set; }
        public string? Subject { get; set; }
    }
}
