namespace Grandmas_Cooking_MVC.Models.ChatModels
{
    public class ChatViewModel
    {
        public string UserMessage { get; set; } = string.Empty;
        public List<ChatMessage> ChatHistory { get; set; } = new();
    }
}
