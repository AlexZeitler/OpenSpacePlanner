﻿using System.Web.Mvc;

namespace OpenSpacePlanner.WebAdmin.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() {
			ViewBag.Message = "Welcome to ASP.NET MVC!";

			return View();
		}

		public ActionResult About() {
			return View();
		}
	}
}
