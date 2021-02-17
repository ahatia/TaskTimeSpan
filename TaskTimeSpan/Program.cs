using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTimeSpan
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskCalculator tc = new TaskCalculator();

            DateTime start = DateTime.Parse("Friday, August 17, 2018 11:00:00 AM");
            int timeInMinutes = 720;

            var endDate = tc.GetEndDate(start, timeInMinutes);

            Debug.WriteLine(endDate.ToLongDateString() + " " + endDate.ToLongTimeString());
        }
    }
}
