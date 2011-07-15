using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.SessionWeb.Controllers {
	public class HomeController : Controller {
		readonly IAttendeeRepository _attendeeRepository;
		readonly INosSessionRepository _sessionRepository;
		readonly INosClient _nosClient;

		public HomeController(IAttendeeRepository attendeeRepository, INosSessionRepository sessionRepository, INosClient nosClient) {
			_attendeeRepository = attendeeRepository;
			_sessionRepository = sessionRepository;
			_nosClient = nosClient;
		}

		public ActionResult Index() {
			ViewBag.Message = "Welcome to ASP.NET MVC!";
			var model = _nosClient.GetPlannedSessions();
			return View(model);
		}

		public ActionResult About() {
			return View();
		}

		[HttpGet]
		public ActionResult Create() {
			return View();
		}

		[HttpPost]
		public ActionResult Create(NosSession session) {
			if (null != session && !string.IsNullOrEmpty(session.OwnerTag)) {
				IAttendee attendee = _attendeeRepository.Get(session.OwnerTag);
				session.Owner = attendee.FirstName + " " + attendee.LastName;
				session.CreatedOn = DateTime.Now;
				session.Start = DateTime.Now;
				session.End = DateTime.Now;
				_sessionRepository.Insert(session);
			}
			return RedirectToAction("Index");
		}

		public ActionResult Sessions() {
			var model = _nosClient.GetPlannedSessions();
			return PartialView("Sessions", model);
		}

		public ActionResult Samstag() {
			var model = _nosClient.GetPlannedSessions();
			return PartialView("Samstag", model);
		}

		public ActionResult Sonntag() {
			var model = _nosClient.GetPlannedSessions();
			return PartialView("Sonntag", model);
		}

		[HttpGet]
		public ActionResult SessionDetails(Guid id) {
			var model = _sessionRepository.Get(id);
			ViewData.Add("session", model.Room);
			ViewData.Add("details", true);
			return PartialView("Session", new List<INosSession> {model});
		}
	}
}
