using System.Collections.Generic;
using System.Net.Mime;
using Newtonsoft.Json;

namespace BlackjackGame.WebSockets
{
    public class WebSocketTextMessage
    {
        [JsonProperty("text")]
        public Dictionary<string, object> Text { get; set; }
    }
}