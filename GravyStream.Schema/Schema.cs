using System;
using System.Collections.Generic;

namespace GravyStream.Schema
{
    public class Person
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string SsoId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime DateRegistered { get; set; }

        public virtual ICollection<Contributor> Contributions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<VideoReaction> Reactions { get; set; }
    }

    public class Reaction
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public virtual ICollection<VideoReaction> VideoReactions { get; set; }
    }

    public class Video
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public DateTime Uploaded { get; set; }
        public DateTime? Published { get; set; }
        public long Views { get; set; }
        public bool AllowsComments { get; set; }
        public bool AllowsReactions { get; set; }
        public string ThumbnailUrl { get; set; }
        public string OriginalFilePath { get; set; }

        public virtual ICollection<MediaStream> Streams { get; set; }
        public virtual ICollection<Contributor> Contributors { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<VideoReaction> Reactions { get; set; }
        public virtual ICollection<VideoTag> Tags { get; set; }
    }

    public class Contributor
    {
        public Guid VideoId { get; set; }
        public Guid PersonId { get; set; }
        public string Role { get; set; }

        public virtual Video Video { get; set; }
        public virtual Person Person { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }
        public Guid VideoId { get; set; }
        public Guid PersonId { get; set; }
        public DateTime Created { get; set; }
        public string Text { get; set; }
        public Guid? ParentId { get; set; }

        public virtual Video Video { get; set; }
        public virtual Person Person { get; set; }
        public virtual Comment Parent { get; set; }
        public virtual ICollection<Comment> Responses { get; set; }
    }

    public class VideoReaction
    {
        public Guid VideoId { get; set; }
        public Guid PersonId { get; set; }
        public Guid ReactionId { get; set; }

        public virtual Person Person { get; set; }
        public virtual Video Video { get; set; }
        public virtual Reaction Reaction { get; set; }
    }

    public abstract class MediaStream
    {
        public Guid Id { get; set; }
        public string Culture { get; set; }
        public string FilePath { get; set; }
        public string Label { get; set; }
        public Guid VideoId { get; set; }

        public virtual Video Video { get; set; }
        public virtual ConversionJob ConversionJob { get; set; }
    }

    public class AudioStream : MediaStream
    {
        public string Codec { get; set; }
        public long Bitrate { get; set; }
    }

    public class VideoStream : AudioStream
    {
        public int? ResolutionX { get; set; }
        public int? ResolutionY { get; set; }
    }

    public class SubtitleStream : MediaStream { }

    public class ConversionJob
    {
        public Guid StreamId { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan? CompletedDuration { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public DateTime? TimeStarted { get; set; }
        public DateTime? TimeCompleted { get; set; }

        public virtual MediaStream Stream { get; set; }
    }

    public class VideoTag
    {
        public Guid VideoId { get; set; }
        public string Tag { get; set; }

        public virtual Video Video { get; set; }
    }
}
