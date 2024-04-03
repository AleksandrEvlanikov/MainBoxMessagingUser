namespace FinWorkGBMailBox.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsRead { get; set; }

    }
}
