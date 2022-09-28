using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICommentRepository
    {
        List<Comment> GetCommentsByPostId(int postId);
        void Add(Comment comment);  
        Comment GetCommentById(int id);
        void UpdateComment (Comment comment);   
        void DeleteComment(int id);
    }
}
