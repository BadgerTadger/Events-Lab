namespace Events.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class CommentInputModel
    {
        public CommentInputModel()
        {

        }

        public CommentInputModel(int eventId)
        {
            EventId = eventId;
        }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Comment text is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Title *")]
        public string Text { get; set; }
    }
}