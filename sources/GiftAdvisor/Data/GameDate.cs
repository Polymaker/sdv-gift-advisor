using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftAdvisor.Data
{
	public struct GameDate
	{
		public static readonly GameDate MinDate = new GameDate();

		public int Year => GetDatePart(0);
		public Season Season => (Season)GetDatePart(1);
		public int Day => GetDatePart(2);
		public int Hours => GetDatePart(3);
		public int Minutes => GetDatePart(4);
		public int Ticks { get; private set; }

		public static GameDate Today => Context.IsWorldReady ? new GameDate(Game1.year, ParseSeason(Game1.currentSeason), Game1.dayOfMonth) : MinDate;

		public static GameDate Now => Context.IsWorldReady ? new GameDate(Game1.year, ParseSeason(Game1.currentSeason), Game1.dayOfMonth, Game1.timeOfDay / 100, Game1.timeOfDay % 100) : MinDate;

		#region Convertion fields

		private const int TicksPerYear = 161280;
		private const int TicksPerSeason = 40320;
		private const int TicksPerDay = 1440;
		private const int TicksPerHour = 60;

		#endregion

		public GameDate(int year, Season season, int day)
		{
			Ticks = GetTicks(year, (int)season, day, 0, 0);
		}

		public GameDate(int year, Season season, int day, int hours, int minutes)
		{
			Ticks = GetTicks(year, (int)season, day, hours, minutes);
		}

		private static int GetTicks(int year, int season, int day, int hours, int minutes)
		{
			return ((year - 1) * TicksPerYear) 
				+ (season + TicksPerSeason) 
				+ ((day - 1) * TicksPerDay) 
				+ (hours * TicksPerHour)
				+ minutes;
		}

		private int GetDatePart(int index)
		{
			int wholeYears = (int)Math.Floor(Ticks / (double)TicksPerYear);
			if (index == 0)
				return wholeYears + 1;

			int remaining = Ticks - (wholeYears * TicksPerYear);
			int season = (int)Math.Floor(remaining / (double)TicksPerSeason);

			if (index == 1)
				return season;

			remaining -= (season * TicksPerSeason);
			int wholeDays = (int)Math.Floor(remaining / (double)TicksPerDay);

			if (index == 2)
				return wholeDays + 1;

			remaining -= (wholeDays * TicksPerDay);
			int wholeHours = (int)Math.Floor(remaining / (double)TicksPerHour);

			if (index == 3)
				return wholeHours;
			remaining -= (wholeHours * TicksPerHour);

			return remaining;
		}

		public static Season ParseSeason(string season)
		{
			return (Season)Enum.Parse(typeof(Season), season, true);
		}

		#region Operators

		public static bool operator >(GameDate d1, GameDate d2)
		{
			return d1.Ticks > d2.Ticks;
		}

		public static bool operator <(GameDate d1, GameDate d2)
		{
			return d1.Ticks < d2.Ticks;
		}

		public static bool operator >=(GameDate d1, GameDate d2)
		{
			return d1.Ticks >= d2.Ticks;
		}

		public static bool operator <=(GameDate d1, GameDate d2)
		{
			return d1.Ticks <= d2.Ticks;
		}

		public static bool operator ==(GameDate d1, GameDate d2)
		{
			return d1.Equals(d2);
		}

		public static bool operator !=(GameDate d1, GameDate d2)
		{
			return !d1.Equals(d2);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GameDate))
			{
				return false;
			}

			var date = (GameDate)obj;
			return Ticks == date.Ticks;
		}

		public override int GetHashCode()
		{
			var hashCode = -188244015;
			hashCode = hashCode * -1521134295 + Ticks.GetHashCode();
			return hashCode;
		}

		#endregion

	}
}
