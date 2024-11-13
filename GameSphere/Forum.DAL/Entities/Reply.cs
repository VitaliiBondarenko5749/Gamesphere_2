﻿namespace Forum.DAL.Entities;

public class Reply
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public Guid PostId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Guid? ReplyToId { get; set; }
}