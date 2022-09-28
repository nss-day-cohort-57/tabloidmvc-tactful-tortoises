using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Models.ViewModels
{
    public class CommentPostViewModel
    {
        public int PostId { get; set; }
        public List<Comment> Comments { get; set;}
        public Post Post { get; set; }
    }
}
