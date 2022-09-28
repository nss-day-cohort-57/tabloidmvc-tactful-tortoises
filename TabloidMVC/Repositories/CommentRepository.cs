using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(IConfiguration config) : base(config) { }

        public List<Comment> GetCommentsByPostId(int postId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT C.Id, C.Subject, C.Content, C.CreateDateTime, C.PostId, C.UserProfileId, U.DisplayName, P.Title
                            FROM Comment C
                            LEFT JOIN UserProfile U ON U.Id = C.UserProfileId
                            LEFT JOIN Post P ON P.Id = C.PostId
                            WHERE PostId = @postId
                            ORDER BY CreateDateTime DESC";
                    cmd.Parameters.AddWithValue("@postId", postId);
                    var reader = cmd.ExecuteReader();

                    var comments = new List<Comment>();

                    while (reader.Read())
                    {
                        comments.Add(
                            new Comment()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                                Post = new Post()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                },
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                UserProfile = new UserProfile()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName"))
                                }
                            }
                            );

                    }

                    reader.Close();

                    return comments;
                }
            }
        }

        public Comment GetCommentById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT Id, UserProfileId, Subject, Content, CreateDateTime, PostId
                         FROM Comment 
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Comment comment = null;

                    if (reader.Read())
                    {
                        comment = new Comment()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                            PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                            Subject = reader.GetString(reader.GetOrdinal("Subject")),
                            Content = reader.GetString(reader.GetOrdinal("Content")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"))
                        };

                    }


                    reader.Close();

                    return comment;
                }
            }
        }
        public void Add(Comment comment)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Comment (
                            Subject, Content, CreateDateTime, PostId, UserProfileId)
                        OUTPUT INSERTED.ID
                        VALUES (
                            @Subject, @Content, @CreateDateTime, @PostId, @UserProfileId )";
                    cmd.Parameters.AddWithValue("@Subject", comment.Subject);
                    cmd.Parameters.AddWithValue("@Content", comment.Content);
                    cmd.Parameters.AddWithValue("@CreateDateTime", comment.CreateDateTime);
                    cmd.Parameters.AddWithValue("@UserProfileId", comment.UserProfileId);
                    cmd.Parameters.AddWithValue("@PostId", comment.PostId);

                    comment.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void UpdateComment(Comment comment)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @" 
                                UPDATE Comment
                                SET
                                    Subject = @subject,
                                    Content = @content,
                                    CreateDateTime = @createDateTime,
                                    PostId = @postId,
                                    UserProfileId = @userProfileId
                                WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@subject", comment.Subject);
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.Parameters.AddWithValue("@CreateDateTime",comment.CreateDateTime);
                    cmd.Parameters.AddWithValue("@userProfileId", comment.UserProfileId);
                    cmd.Parameters.AddWithValue("@postId", comment.PostId);
                    cmd.Parameters.AddWithValue("@id", comment.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
