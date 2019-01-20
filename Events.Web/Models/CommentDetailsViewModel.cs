namespace Events.Web.Models
{
    using Events.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;

    public class CommentDetailsViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public static Expression<Func<Comment, CommentDetailsViewModel>> ViewModel
        {
            get
            {
                return c => new CommentDetailsViewModel()
                {
                    Id = c.Id,
                    Text = c.Text
                };
            }
        }
    }
}