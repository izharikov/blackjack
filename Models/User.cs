using Microsoft.AspNetCore.Identity;

namespace BlackjackGame.Models
{
    public class User /*: IdentityUser<string>*/
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}