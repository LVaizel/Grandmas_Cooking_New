// Gaganvir - ChatMessage.cs created for representing individual chat messages in the chat functionality.

namespace Grandmas_Cooking_MVC.Models.ChatModels
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
