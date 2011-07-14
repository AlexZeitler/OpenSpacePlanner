using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSpacePlanner.Contracts;

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
	}
}
