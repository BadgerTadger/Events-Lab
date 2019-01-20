namespace Events.Web.Controllers
{
    using Events.Data;
    using Events.Web.Extensions;
    using Events.Web.Models;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Linq;
    using System.Web.Mvc;


    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var events = this.db.Events
                .OrderBy(e => e.StartDateTime)
                .Where(e => e.IsPublic)
                .Select(EventViewModel.ViewModel);

            var upcomingEvents = events.Where(e => e.StartDateTime > DateTime.Now);
            var passedEvents = events.Where(e => e.StartDateTime <= DateTime.Now);
            return View(new UpcomingPassedEventsViewModel()
            {
                UpcomingEvents = upcomingEvents,
                PassedEvents = passedEvents
            });
        }

        public ActionResult EventDetailsById(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var eventDetails = this.db.Events
                .Where(e => e.Id == id)
                .Where(e => e.IsPublic || isAdmin || (e.AuthorId != null && e.AuthorId == currentUserId))
                .Select(EventDetailsViewModel.ViewModel)
                .FirstOrDefault();

            var isOwner = (eventDetails != null && eventDetails.AuthorId != null && eventDetails.AuthorId == currentUserId);
            this.ViewBag.CanEdit = isOwner || isAdmin;

            return this.PartialView("_EventDetails", eventDetails);
        }

        public ActionResult AddComment()
        {
            var commentDetails = this.db.Comments
                .Add(new Data.Comment());
                
            return this.PartialView("_AddComment", new CommentInputModel());
        }

        public ActionResult SaveComment(CommentInputModel model)
        {
            if (model != null && this.ModelState.IsValid)
            {
                var c = new Comment()
                {
                    Author = this.db.Users.First(),
                    Date = DateTime.Now,
                    AuthorId = this.User.Identity.GetUserId(),
                    //EventId = id,
                    //Event = LoadEvent(id),
                    Text = model.Text
                };
                this.db.Comments.Add(c);
                this.db.SaveChanges();
                this.AddNotification("Comment created.", NotificationType.INFO);

                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        private Event LoadEvent(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var eventToEdit = this.db.Events
                .Where(e => e.Id == id)
                .FirstOrDefault(e => e.AuthorId == currentUserId || isAdmin);

            return eventToEdit;
        }
    }
}