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

        public ActionResult AddComment(int eventId)
        {
            var commentDetails = this.db.Comments
                .Add(new Data.Comment());
                
            return this.PartialView("_AddComment", new CommentInputModel(eventId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveComment(CommentInputModel model)
        {
            if (model != null && this.ModelState.IsValid)
            {
                var c = new Comment()
                {
                    Text = model.Text,
                    Date = DateTime.Now,
                    AuthorId = this.User.Identity.GetUserId(),
                    EventId = model.EventId,
                    Event = LoadEvent(model.EventId)
                };
                this.db.Comments.Add(c);
                this.db.SaveChanges();
                this.AddNotification("Comment created.", NotificationType.INFO);

                return this.RedirectToAction("Index");
            }

            return this.View(model);
        }

        public ActionResult DeleteComment(int id)
        {

            Comment comment = LoadComment(id);
            this.db.Comments.Remove(comment);
            this.db.SaveChanges();
            this.AddNotification("Comment deleted.", NotificationType.INFO);

            return this.RedirectToAction("Index");
        }

        private Comment LoadComment(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var commentToDelete = this.db.Comments
                .Where(c => c.Id == id)
                .FirstOrDefault(c => c.AuthorId == currentUserId || isAdmin);

            return commentToDelete;
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