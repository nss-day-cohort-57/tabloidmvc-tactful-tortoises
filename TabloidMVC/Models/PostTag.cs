﻿namespace TabloidMVC.Models
{
    public class PostTag
    {
        public int Id { get; set; }
        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}
