using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GravyStream.Schema
{
    public class Person
    {
        public Guid Id { get; set; }
        [Display(Name = "Display Name"), MaxLength(150)]
        public string DisplayName { get; set; }
        [Display(Name = "Username"), Required, MaxLength(150)]
        public string UserName { get; set; }
        [Display(Name = "SSO ID"), MaxLength(300)]
        public string SsoId { get; set; }
        [Display(Name = "Image URL"), MaxLength(300), DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }
        public DateTime Registered { get; set; }

        public virtual ICollection<Contributor> Contributions { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<VideoReaction> Reactions { get; set; }
    }

    public class Reaction
    {
        public Guid Id { get; set; }
        [Required, MaxLength(30)]
        public string Description { get; set; }
        [Required, MaxLength(2)]
        public string Icon { get; set; }

        public virtual ICollection<VideoReaction> VideoReactions { get; set; }
    }

    public class Video
    {
        public Guid Id { get; set; }
        [Required, MaxLength(300)]
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "URL Name"), Required, MaxLength(100)]
        public string Slug { get; set; }
        public DateTime Uploaded { get; set; }
        public DateTime? Published { get; set; }
        public long Views { get; set; }
        [Display(Name = "Allow Comments")]
        public bool AllowsComments { get; set; }
        [Display(Name = "Allow Reactions")]
        public bool AllowsReactions { get; set; }
        [Display(Name = "Thumbnail URL"), MaxLength(500), DataType(DataType.ImageUrl)]
        public string ThumbnailUrl { get; set; }
        [Display(Name = "OriginalFilePath"), MaxLength(500), DataType(DataType.Url)]
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
        [MaxLength(150)]
        public string Role { get; set; }

        public virtual Video Video { get; set; }
        public virtual Person Person { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; }

        public Guid VideoId { get; set; }

        public Guid PersonId { get; set; }

        /// <summary>
        /// Date/time when comment was created
        /// </summary>
        public DateTime Created { get; set; }

        [DataType(DataType.MultilineText), MaxLength(2000)]
        public string Text { get; set; }

        /// <summary>
        /// Parent ID of comment, if comment is a reply
        /// </summary>
        public Guid? ParentId { get; set; }

        public bool IsVisible { get; set; }

        public virtual Video Video { get; set; }

        public virtual Person Person { get; set; }

        /// <summary>
        /// Comment that this is a response to
        /// </summary>
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

        /// <summary>
        /// Language of the stream
        /// </summary>
        [Required, MaxLength(10)]
        public string Culture { get; set; }

        [Display(Name = "File Path"), MaxLength(500)]
        public string FilePath { get; set; }

        [MaxLength(25)]
        public string Label { get; set; }

        public Guid VideoId { get; set; }

        public virtual Video Video { get; set; }
        public virtual ConversionJob ConversionJob { get; set; }
    }

    public class AudioStream : MediaStream
    {
        [MaxLength(10)]
        public string Codec { get; set; }

        /// <summary>
        /// Data rate of the stream
        /// </summary>
        /// <remarks>Unit is bits per second</remarks>
        [Range(0, long.MaxValue)]
        public long Bitrate { get; set; }

        public override string ToString() => string.IsNullOrEmpty(Label) ? $"{Culture} ({Codec})" : Label;
    }

    public class VideoStream : AudioStream
    {
        /// <summary>
        /// Horizontal resolution of the video stream
        /// </summary>
        [Display(Name = "Horizontal Resolution"), Range(0, int.MaxValue)]
        public int? ResolutionX { get; set; }

        /// <summary>
        /// Vertical resolution of the video stream
        /// </summary>
        [Display(Name = "Vertical Resolution"), Range(0, int.MaxValue)]
        public int? ResolutionY { get; set; }

        public override string ToString() => string.IsNullOrEmpty(Label) ? $"{ResolutionY}p" : Label;
    }

    public class SubtitleStream : MediaStream
    {
        public override string ToString() => string.IsNullOrEmpty(Label) ? Culture : Label;
    }

    /// <summary>
    /// Represents a queued, in-progress, or completed transcode
    /// </summary>
    public class ConversionJob
    {
        public Guid StreamId { get; set; }

        /// <summary>
        /// Error message from transcoder, if any
        /// </summary>
        [Display(Name = "Error Message"), MaxLength(1000)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Duration of stream that has been processed so far
        /// </summary>
        [Display(Name = "Completed Duration")]
        public TimeSpan? CompletedDuration { get; set; }

        /// <summary>
        /// Total duration of stream
        /// </summary>
        [Display(Name = "Total Duration")]
        public TimeSpan TotalDuration { get; set; }

        /// <summary>
        /// Time when conversion was started
        /// </summary>
        [Display(Name = "Time Started")]
        public DateTime? TimeStarted { get; set; }

        /// <summary>
        /// Time when conversion was completed, if complete
        /// </summary>
        [Display(Name = "Time Completed")]
        public DateTime? TimeCompleted { get; set; }

        public virtual MediaStream Stream { get; set; }
    }

    /// <summary>
    /// Used to tag a topic or subject on a video
    /// </summary>
    public class VideoTag
    {
        public Guid VideoId { get; set; }
        [MaxLength(50)]
        public string Tag { get; set; }

        public virtual Video Video { get; set; }
    }
}
