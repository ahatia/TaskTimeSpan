using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTimeSpan
{
    public class TaskCalculator
    {
        public readonly int StartWorkDay;
        public readonly int EndWorkDay;

        public readonly int StartLuchBreak;
        public readonly int EndLunchBreak;

        public readonly int WorkWeeksPerYear;
        public readonly int WorkDaysPerWeek;
        public readonly int WorkHoursPerDay;
        public readonly int WorkMinutesPerYear;
        public readonly int MinutesPerHour;


        public HashSet<DateTime> Holidays;

        public TaskCalculator()
        {
            StartWorkDay = 8;
            EndWorkDay = 17;

            StartLuchBreak = 12;
            EndLunchBreak = 13;

            MinutesPerHour = 60;
            WorkHoursPerDay = 8;
            WorkDaysPerWeek = 5;
            WorkWeeksPerYear = 52;
            WorkMinutesPerYear = WorkHoursPerDay * MinutesPerHour * WorkDaysPerWeek * WorkWeeksPerYear;
        }

        public DateTime GetEndDate(DateTime start, int minutes)
        {
            if(minutes > WorkMinutesPerYear)
            {
                throw new Exception("The number of minutes can not exceed 1 year(" + WorkMinutesPerYear.ToString() + " minutes)");
            }
            
            var taskHours = minutes / MinutesPerHour;
            var taskDays = Convert.ToInt32(Math.Ceiling((Convert.ToDecimal(minutes) / (MinutesPerHour * WorkHoursPerDay)))) - 1;
            var taskMinutesRemained = minutes % (MinutesPerHour * WorkHoursPerDay);
            var startTaskHour = start.Hour;
            DateTime endTaskDateTime = AddBusinessDays(start, taskDays);            
            

            //start time was selected after hours
            if (startTaskHour >= EndWorkDay)
            {
                start = new DateTime(start.Year, start.Month, start.Day + 1, StartWorkDay, 0, 0);
            }
            //start time falls on lunch break
            else if (startTaskHour >= StartLuchBreak && startTaskHour <= EndLunchBreak)
            {
                start = new DateTime(start.Year, start.Month, start.Day, EndLunchBreak, 0, 0);
            }            
            //start time was selected after hours
            else if (startTaskHour < StartWorkDay)
            {
                start = new DateTime(start.Year, start.Month, start.Day, StartWorkDay, 0, 0);                
            }

            Holidays = GetHolidays(start, minutes);

            if (Holidays.Contains(start.Date) && !IsWeekendDate(start.Date))
            {
                start = start.AddDays(1);
            }

            startTaskHour = start.Hour;

            if (taskDays <= 0)
            {
                endTaskDateTime = AddBusinessDays(start, 0);
            }
            else
            {
                endTaskDateTime = AddBusinessDays(start, taskDays);
            }

            var tempDate = start.Date;

            while (tempDate <= endTaskDateTime)
            {
                if (Holidays.Contains(tempDate.Date) && !IsWeekendDate(tempDate.Date))
                {
                    endTaskDateTime = endTaskDateTime.AddDays(1);
                }

                tempDate = tempDate.AddDays(1);
            }

            if (taskMinutesRemained == 0)
            {
                endTaskDateTime = endTaskDateTime.AddHours(WorkHoursPerDay);
            }
            else
            {
                endTaskDateTime = endTaskDateTime.AddMinutes(taskMinutesRemained);
            }

            if (startTaskHour <= StartLuchBreak && endTaskDateTime.Hour >= EndLunchBreak)
            {
                endTaskDateTime =  endTaskDateTime.AddHours(1);
            }

            //Task hours less than a day worth of work
            if (endTaskDateTime.Hour > EndWorkDay ||
                (endTaskDateTime.Hour == EndWorkDay && endTaskDateTime.Minute > 0))
            {
                var endOfWorkDay = new DateTime(start.Year, start.Month, start.Day, EndWorkDay, 0, 0);

                TimeSpan carryOverMinutes = endTaskDateTime.Subtract(endOfWorkDay);

                var nextDayStart = new DateTime(start.Year, start.Month, start.Day + 1, StartWorkDay, 0, 0);

                endTaskDateTime = nextDayStart.AddMinutes(carryOverMinutes.TotalMinutes);

            }

            return endTaskDateTime;
        }

        public HashSet<DateTime> GetHolidays(DateTime startTaskDate, int minutes)
        {
            
            HashSet<DateTime> holidays = new HashSet<DateTime>();

            int year = startTaskDate.Year;

            // New Years
            DateTime newYearsDate = AdjustForWeekendHoliday(new DateTime(year, 1, 1));
            holidays.Add(newYearsDate);

            //Memorial Day
            DateTime memorialDay = GetMeorialDay(year);
            holidays.Add(memorialDay);

            // Independence Day
            DateTime independenceDay = AdjustForWeekendHoliday(new DateTime(year, 7, 4));
            holidays.Add(independenceDay);

            //Labour Day
            DateTime laborDay =  GeLaborDay(year);
            holidays.Add(laborDay);

            //Thanksgiving Day
            DateTime thanksgivingDay = GetThanksgivingDay(year);
            holidays.Add(thanksgivingDay);

            // Christmas Day 
            DateTime christmasDay = AdjustForWeekendHoliday(new DateTime(year, 12, 25));
            holidays.Add(christmasDay);

            // Next year's new years check
            DateTime nextYearNewYearsDate = AdjustForWeekendHoliday(new DateTime(year + 1, 1, 1));
            holidays.Add(nextYearNewYearsDate);

            //Memorial Day
            DateTime memorialDay2 = GetMeorialDay(year + 1);
            holidays.Add(memorialDay2);

            // Independence Day
            DateTime independenceDay2 = AdjustForWeekendHoliday(new DateTime(year + 1, 7, 4));
            holidays.Add(independenceDay2);

            //Labour Day
            DateTime laborDay2 = GeLaborDay(year + 1);
            holidays.Add(laborDay2);

            //Thanksgiving Day
            DateTime thanksgivingDay2 = GetThanksgivingDay(year + 1);
            holidays.Add(thanksgivingDay2);

            // Christmas Day 
            DateTime christmasDay2 = AdjustForWeekendHoliday(new DateTime(year + 1, 12, 25));
            holidays.Add(christmasDay2);

            return holidays;
        }

        private DateTime AdjustForWeekendHoliday(DateTime holiday)
        {
            if (holiday.DayOfWeek == DayOfWeek.Saturday)
            {
                return holiday.AddDays(-1);
            }
            else if (holiday.DayOfWeek == DayOfWeek.Sunday)
            {
                return holiday.AddDays(1);
            }
            else
            {
                return holiday;
            }
        }

        public DateTime  GetMeorialDay(int year)
        {
            // Memorial Day falls on last Monday in May 
            DateTime memorialDay = new DateTime(year, 5, 31);
            DayOfWeek dayOfWeek = memorialDay.DayOfWeek;
            while (dayOfWeek != DayOfWeek.Monday)
            {
                memorialDay = memorialDay.AddDays(-1);
                dayOfWeek = memorialDay.DayOfWeek;
            }
            return memorialDay;
        }

        public DateTime GeLaborDay(int year)
        {

            // Labor Day falls on 1st Monday in September 
            DateTime laborDay = new DateTime(year, 9, 1);
            DayOfWeek dayOfWeek = laborDay.DayOfWeek;
            while (dayOfWeek != DayOfWeek.Monday)
            {
                laborDay = laborDay.AddDays(1);
                dayOfWeek = laborDay.DayOfWeek;
            }
            return laborDay;
        }

        public DateTime GetThanksgivingDay(int year)
        {

            // Thanksgiving Day falls 4th Thursday in November 
            var thanksgiving = (from day in Enumerable.Range(1, 30)
                                where new DateTime(year, 11, day).DayOfWeek == DayOfWeek.Thursday
                                select day).ElementAt(3);
            DateTime thanksgivingDay = new DateTime(year, 11, thanksgiving);            

            return thanksgivingDay;
        }

        public bool IsWeekendDate(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }           
            else
            {
                return false;
            }
        }

        public DateTime AddBusinessDays(DateTime source, int businessDays)
        {
            var dayOfWeek = businessDays < 0
                                ? ((int)source.DayOfWeek - 12) % 7
                                : ((int)source.DayOfWeek + 6) % 7;

            switch (dayOfWeek)
            {
                case 6:
                    businessDays--;
                    break;
                case -6:
                    businessDays++;
                    break;
            }

            return source.AddDays(businessDays + ((businessDays + dayOfWeek) / 5) * 2);
        }        

    }
}
