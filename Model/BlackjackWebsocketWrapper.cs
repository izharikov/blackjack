namespace BlackjackGame.Model
{
    public class BlackjackWebsocketWrapper : WebscoketWrapper
    {
        public BlackjackUser BlackjackUser { get; set; }
        public int Place { get; set; }
    }
}