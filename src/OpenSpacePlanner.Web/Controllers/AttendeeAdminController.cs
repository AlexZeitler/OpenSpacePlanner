using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Web.Controllers
{
    public class AttendeeAdminController : Controller
    {
    	readonly IAttendeeRepository _attendeeRepository;

    	public AttendeeAdminController(IAttendeeRepository attendeeRepository ) {
    		_attendeeRepository = attendeeRepository;
    	}

    	//
        // GET: /AttendeeAdmin/

        public ActionResult Index() {
        	var model = _attendeeRepository.Get().OrderBy(a=>a.LastName);
            return View(model);
        }

    }
}
