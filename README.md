Problem 
We have a standard 40-hour work week; 8 am to 5 pm Monday through Friday. There is a one-hour break for lunch (12:00 to 1:00 pm). We have six holidays per year: 

New Year’s Day, Memorial Day, 4th of July, Labor Day, Thanksgiving and Christmas. 

For any given task, we assign the number of minutes it should take for the task to be completed. So, when a task is assigned, the start time is recorded, as well as the number of minutes assigned to complete the task. 

Write a function getEndDate(start, minutes) that when given the start date/time and the number of minutes; return the ending date/time. 

Notes: 
1.	Tasks can only be worked on during work hours. 
2.	Tasks can be assigned at any time. 
3.	3. The number of minutes will never exceed 1 year. 

Samples: 
Start = Wednesday, January 03, 2018 06:00:00 AM Minutes = 55 Answer = Wednesday, January 03, 2018 08:55:00 AM 
 
Start = Tuesday, July 3, 2018 09:00:00 PM Minutes = 720 Answer = Thursday, July 5, 2018 02:00:00 PM 
 
Start = Tuesday, August 21, 2018 05:00:00 AM Minutes = 4800 Answer = Tuesday, September 04, 2018 05:00:00 PM 
