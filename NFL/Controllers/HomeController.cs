using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NFL.Models;

namespace NFL.Controllers
{
	public class HomeController : Controller
	{
		private TeamContext context;

		public HomeController(TeamContext ctx)
		{
			context = ctx;
		}

		public IActionResult Index(string activeConference = "all", string activeDivision = "all")
		{
			var session = new NFLSession(HttpContext.Session);
			session.SetActiveConf(activeConference);
			session.SetActiveDiv(activeDivision);

			int? count = session.GetMyTeamCount();

			if (count == null)
			{
				var cookies = new NFLCookies(Request.Cookies);
				string[] ids = cookies.GetMyTeamIds();

				List<Team> myTeams = new List<Team>();
				if (ids.Length > 0)
				{
					myTeams = context.Teams
						.Include(c => c.Conference)
						.Include(d => d.Division)
						.Where(t => ids.Contains(t.TeamID))
						.ToList();
				}

				session.SetMyTeams(myTeams);
			}

			var data = new TeamListViewModel
			{
				ActiveConference = activeConference,
				ActiveDivision = activeDivision,
				Conferences = context.Conferences.ToList(),
				Divisions = context.Divisions.ToList()
			};

			IQueryable<Team> query = context.Teams;

			if (activeConference != "all")
			{
				query = query.Where(q => q.Conference.ConferenceID.ToLower() == activeConference.ToLower());
			}

			if (activeDivision != "all")
			{
				query = query.Where(q => q.Division.DivisionID.ToLower() == activeDivision.ToLower());
			}

			data.Teams = query.ToList();

			return View(data);
		}

		public IActionResult Details(string id)
		{
			var session = new NFLSession(HttpContext.Session);

			var model = new TeamViewModel
			{
				Team = context.Teams
							  .Include(c => c.Conference)
							  .Include(d => d.Division)
							  .FirstOrDefault(t => t.TeamID == id),

				ActiveConference = session.GetActiveConf(),
				ActiveDivision = session.GetActiveDiv()
			};

			return View(model);
		}

		[HttpPost]
		public RedirectToActionResult Add(TeamViewModel model)
		{
			model.Team = context.Teams
								.Include(c => c.Conference)
								.Include(d => d.Division)
								.Where(t => t.TeamID == model.Team.TeamID)
								.FirstOrDefault();

			var session = new NFLSession(HttpContext.Session);
			var teams = session.GetMyTeams();
			teams.Add(model.Team);
			session.SetMyTeams(teams);

			var cookies = new NFLCookies(Response.Cookies);
			cookies.SetMyTeamIds(teams);

			TempData["message"] = $"{model.Team.Name} was added to your favourites";

			return RedirectToAction("Index", new { ActiveConference = session.GetActiveConf(), ActiveDivision = session.GetActiveDiv() });
		}
	}
}
