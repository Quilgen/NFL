﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFL.Models
{
	public class TeamListViewModel : TeamViewModel
	{
		public List<Team> Teams { get; set; }
		private List<Conference> conferences;
		private List<Division> divisions;

		public List<Conference> Conferences
		{
			get => conferences;
			set
			{
				conferences = value;
				conferences.Insert(0, new Conference { ConferenceID = "all", Name = "All" });
			}
		}

		public List<Division> Divisions
		{
			get => divisions;
			set
			{
				divisions = value;
				divisions.Insert(0, new Division { DivisionID = "all", Name = "All" });
			}
		}

		public string CheckActiveConference(string c) => c.ToLower() == ActiveConference.ToLower() ? "active" : "";

		public string CheckActiveDivision(string d) => d.ToLower() == ActiveDivision.ToLower() ? "active" : "";
	}
}
