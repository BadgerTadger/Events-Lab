namespace Events.Web.Models
{
    using Events.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;

    public class CommentViewModel
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }

        public static Expression<Func<Comment, CommentViewModel>> ViewModel
        {
            get
            {
                return c => new CommentViewModel()
                {
                    Id = c.Id,
                    Text = c.Text,
                    Date = c.Date,
                    Author = c.Author.FullName
                };
            }
        }

        internal static CommentViewModel Add()
        {
            return new CommentViewModel();
        }
    }
}