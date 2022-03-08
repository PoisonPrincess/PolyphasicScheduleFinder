using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PolyphasicScheduleFinder
{
    static class Program
    {
        #region attributes
        internal enum sleepBlockType { Core, Nap };
        /// <summary> Database of potential schedules </summary>
        internal static List<Schedule> _scheduleDB = new List<Schedule>();
        /// <summary> Ratings for how good sleep would be at 5m marks. Overall; SWS; REM </summary>
        internal static int[,] _hourRatings = new int[24, 3];
        /// <summary> Used to confirm user has no scheduling restrictions </summary>
        internal static bool _noRestrictions = false;
        const bool _consoleWrite = true;

        /// <summary> The main entry point for the application. </summary>
        [STAThread]
        static void Main()
        {
            populateScheduleDB(_consoleWrite); //read in schedule data from DB

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FinderWindow(_consoleWrite));
        }
        #endregion

        #region setup
        /// <summary> Read in data from schedule database and adds it to the scheduleDB </summary>
        internal static void populateScheduleDB(bool consoleWrite)
        {
            string name, DPS, DPE, link;
            int numSleeps;
            bool recommended;
            string length, startTime, endTime, earliestStartTime, latestEndTime, maxDistanceFromPreviousSleepBlock;

            string data = "";
            try
            {
                data = new StreamReader(@"..\..\Schedules.txt").ReadToEnd(); //try to get the schedules
            }
            catch (IOException e)
            {
                try
                {
                    data = new StreamReader(@".\Schedules.txt").ReadToEnd(); //try to get the schedules
                }
                catch (IOException f)
                {
                    Console.WriteLine("Schedules.txt could not be read:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(f.Message);
                }
            }

            int numSchedules = data.Length - data.Replace("\n", "").Length;

            string[] schedules = data.Split('\n');

            for (int i = 0; i < numSchedules; i++) //Populate schedule database
            {
                name = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                link = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                numSleeps = Int32.Parse(schedules[i].Substring(0, schedules[i].IndexOf(';')));
                schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                recommended = schedules[i].Substring(0, schedules[i].IndexOf(';')) == "true";
                schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                DPS = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                DPE = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);

                List<SleepBlock> blocks = new List<SleepBlock>();

                for (int j = 0; j < numSleeps; j++)
                {
                    length = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    startTime = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    endTime = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    earliestStartTime = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    latestEndTime = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    Enum.TryParse(schedules[i].Substring(0, schedules[i].IndexOf(';')), out sleepBlockType type);
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    maxDistanceFromPreviousSleepBlock = schedules[i].Substring(0, schedules[i].IndexOf(';'));
                    schedules[i] = schedules[i].Remove(0, schedules[i].IndexOf(';') + 1);
                    blocks.Add(new SleepBlock(length, startTime, endTime, earliestStartTime, latestEndTime, maxDistanceFromPreviousSleepBlock, type));
                }
                _scheduleDB.Add(new Schedule(name, blocks, recommended, DPS, DPE, link, consoleWrite));
            }
        }

        /// <summary> Start schedulefinding </summary>
        /// <param name="age">User's age</param>
        /// <param name="activity">User's physical activity level</param>
        /// <param name="monoBaseline">User's mono baseline length (hours)</param>
        /// <param name="userSleepTimes">List of sleepblocks that show when the user can sleep</param>
        /// <param name="noRestrict">If the user has no sleep time restrictions</param>
        /// <param name="experience">User's polyphasic sleep experience</param>
        internal static List<Schedule> start(int age, int activity, double monoBaseline, List<SleepBlock> userSleepTimes, bool noRestrict, int experience)
        {
            _noRestrictions = noRestrict;

            if (monoBaseline != convertDifferenceToDouble("00:00", _scheduleDB[_scheduleDB.Count - 1].tst) && monoBaseline <= 14 && monoBaseline > 0) //update mono length based on user entry
                modifyMono(monoBaseline);

            if (!_noRestrictions) populateHourRatings(activity - 1); //determine what hours would be best for each vital sleep

            return findValidSchedules(userSleepTimes, age, activity, monoBaseline, experience);
        }

        /// <summary> Populate the HRatings based on how good that time is for each vital sleep </summary>
        /// <param name="exercise">User's exercise level</param>
        private static void populateHourRatings(int exercise)
        {
            int hour = 0;
            _hourRatings[hour, 1] = 59; _hourRatings[hour, 2] = 56; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //0
            hour++;
            _hourRatings[hour, 1] = 58; _hourRatings[hour, 2] = 57; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //01
            hour++;
            _hourRatings[hour, 1] = 57; _hourRatings[hour, 2] = 58; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //02
            hour++;
            _hourRatings[hour, 1] = 56; _hourRatings[hour, 2] = 59; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //03
            hour++;
            _hourRatings[hour, 1] = 55; _hourRatings[hour, 2] = 60; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //04
            hour++;
            _hourRatings[hour, 1] = 54; _hourRatings[hour, 2] = 61; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //05
            hour++;
            _hourRatings[hour, 1] = 53; _hourRatings[hour, 2] = 62; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //06
            hour++;
            _hourRatings[hour, 1] = 52; _hourRatings[hour, 2] = 61; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //07
            hour++;
            _hourRatings[hour, 1] = 51; _hourRatings[hour, 2] = 60; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //08
            hour++;
            _hourRatings[hour, 1] = 50; _hourRatings[hour, 2] = 59; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //09
            hour++;
            _hourRatings[hour, 1] = 51; _hourRatings[hour, 2] = 58; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //10
            hour++;
            _hourRatings[hour, 1] = 52; _hourRatings[hour, 2] = 57; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] + 3; //11
            hour++;
            _hourRatings[hour, 1] = 53; _hourRatings[hour, 2] = 56; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] + 5; //12
            hour++;
            _hourRatings[hour, 1] = 54; _hourRatings[hour, 2] = 55; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] + 5; //13
            hour++;
            _hourRatings[hour, 1] = 55; _hourRatings[hour, 2] = 54; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] + 5; //14
            hour++;
            _hourRatings[hour, 1] = 56; _hourRatings[hour, 2] = 53; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] + 3; //15
            hour++;
            _hourRatings[hour, 1] = 57; _hourRatings[hour, 2] = 52; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //16
            hour++;
            _hourRatings[hour, 1] = 58; _hourRatings[hour, 2] = 51; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] - 5; //17
            hour++;
            _hourRatings[hour, 1] = 59; _hourRatings[hour, 2] = 50; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] - 3; //18
            hour++; //Exercise level is used to help prioritize SWS peak for those who are more active
            _hourRatings[hour, 1] = 60 + exercise; _hourRatings[hour, 2] = 51; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] - 16; //19
            hour++;
            _hourRatings[hour, 1] = 61 + exercise; _hourRatings[hour, 2] = 52; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2] - 16; //20
            hour++;
            _hourRatings[hour, 1] = 62 + exercise; _hourRatings[hour, 2] = 53; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //21
            hour++;
            _hourRatings[hour, 1] = 61 + exercise; _hourRatings[hour, 2] = 54; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //22
            hour++;
            _hourRatings[hour, 1] = 60 + exercise; _hourRatings[hour, 2] = 55; _hourRatings[hour, 0] = _hourRatings[hour, 1] + _hourRatings[hour, 2]; //23
        }
        #endregion

        #region schedule finder
        /// <summary> Determines which schedules user should be able to do based on personal parameters </summary>
        /// <param name="userSleepTimes">List of sleepblocks that show when the user can sleep</param>
        /// <param name="age">User's age</param>
        /// <param name="activity">User's physical activity level</param>
        /// <param name="monoBaseline">User's mono baseline length (hours)</param>
        /// <param name="experience">User's polyphasic sleep experience</param>
        private static List<Schedule> findValidSchedules(List<SleepBlock> userSleepTimes, int age, int activity, double monoBaseline, int experience)
        {
            bool consoleWrite = false;
            List<Schedule> possibleSchedules = new List<Schedule>();

            //tryFitSchedule(userSleepTimes, scheduleDB.Find(s => s.name == "Segmented")); return;  //Use for testing specific schedules

            //non-recommended schedules, for those with low needs or extreme experience and no activity
            if ((monoBaseline <= 6 &&
                (age >= 22 || (age >= 20 && experience >= 2)) &&
                (experience >= 2 || (experience >= 1 && monoBaseline <= 5)) && activity < 1) ||
                ((activity < 2 && monoBaseline <= 4) || monoBaseline <= 3))
            {
                foreach (Schedule s in _scheduleDB.FindAll(sched => !sched.recommended && shorterThanMono(monoBaseline, sched.tst)))
                {
                    if (!_noRestrictions)
                    {
                        Schedule proposed = tryFitSchedule(userSleepTimes, s);
                        if (proposed != null)
                        {
                            possibleSchedules.Add(proposed);
                            if (consoleWrite) Debug.WriteLine("Added: " + proposed.name);
                        }
                        else if (consoleWrite) Debug.WriteLine("Couldn't find valid mapping");
                    }
                    else //no restrictions, then add basic schedule
                    {
                        possibleSchedules.Add(s);
                    }
                }
            }
            //Tst between 4-5, for experienced people or those with low needs
            else if (((monoBaseline <= 7 || (monoBaseline <= 8 && age >= 20 && experience >= 2 && activity <= 0)) &&
                (age >= 21 || ((age >= 18 && experience >= 2 && monoBaseline <= 6) || (age >= 19 && experience >= 2))) &&
                (experience >= 2 || (experience >= 1 && monoBaseline <= 5)) &&
                (activity <= 0 || (activity <= 1 && monoBaseline <= 6))) ||
                ((activity <= 2 && monoBaseline <= 5) || monoBaseline <= 4))
            {
                foreach (Schedule s in _scheduleDB.FindAll(sched => !sched.recommended && convertDifferenceToDouble("00:00", sched.tst) >= 4 && shorterThanMono(monoBaseline, sched.tst)))
                {
                    if (!_noRestrictions)
                    {
                        Schedule proposed = tryFitSchedule(userSleepTimes, s);
                        if (proposed != null)
                        {
                            possibleSchedules.Add(proposed);
                            if (consoleWrite) Debug.WriteLine("Added: " + proposed.name);
                        }
                        else if (consoleWrite) Debug.WriteLine("Couldn't find valid mapping");
                    }
                    else //no restrictions, then add basic schedule
                    {
                        possibleSchedules.Add(s);
                    }
                }
            }

            if ((monoBaseline <= 9 &&
                (age >= 20 || (age >= 18 && experience >= 2) || (age >= 19 && experience >= 1)) &&
                ((activity >= 2 && monoBaseline <= 7) || (activity >= 1 && monoBaseline <= 8) || activity <= 0 || experience >= 2)) ||
                ((experience >= 1 && monoBaseline <= 7) || monoBaseline <= 6))
            {
                foreach (Schedule s in _scheduleDB.FindAll(sched => sched.recommended && (shorterThanMono(monoBaseline, sched.tst) || sched.name == "Monophasic")))
                {
                    //Quad-Core is an advanced schedule, so don't recommend it if experience is not medium or higher
                    //Seg-short is a difficult schedule, only recommend for reduced needs
                    //Only recommend tst >= 7.5 if activity level is high
                    if ((s.name != "Quad-Core" || experience >= 1) && (s.name != "Segmented-Shortened" || monoBaseline < 8) && (convertDifferenceToDouble("00:00", s.tst) < 7.5 || activity >= 2 || s.name == "Monophasic"))
                    {
                        if (!_noRestrictions)
                        {
                            Schedule proposed = tryFitSchedule(userSleepTimes, s);
                            if (proposed != null)
                            {
                                possibleSchedules.Add(proposed);
                                if (consoleWrite) Debug.WriteLine("Added: " + proposed.name);
                            }
                            else if (consoleWrite) Debug.WriteLine("Couldn't find valid mapping");
                        }
                        else //no restrictions, then add basic schedule
                        {
                            possibleSchedules.Add(s);
                        }
                    }
                }
            } //all recommended schedules, for average people
            else if ((((age >= 19) || (age >= 18 && experience >= 1) || (age >= 17 && experience >= 1)) &&
                (((activity <= 2 && monoBaseline <= 8) || (activity <= 1 && monoBaseline <= 9) || (activity <= 0 && monoBaseline <= 10)) || experience >= 1)) ||
                monoBaseline <= 7)
            {
                foreach (Schedule s in _scheduleDB.FindAll(sched => convertDifferenceToDouble("00:00", sched.tst) >= 6 && shorterThanMono(monoBaseline, sched.tst)))
                {

                    //Quad-Core is an advanced schedule, so don't recommend it if experience is not medium or higher
                    //Seg-Short is a difficult schedule, do not recommend
                    //Only recommend tst >= 7.5 if activity level is high
                    if ((s.name != "Quad-Core" || experience >= 1) && s.name != "Segmented-Shortened" && (convertDifferenceToDouble("00:00", s.tst) < 7.5 || activity >= 2 || s.name == "Monophasic"))
                    {
                        if (!_noRestrictions)
                        {
                            Schedule proposed = tryFitSchedule(userSleepTimes, s);
                            if (proposed != null)
                            {
                                possibleSchedules.Add(proposed);
                                if (consoleWrite) Debug.WriteLine("Added: " + proposed.name);
                            }
                            else if (consoleWrite) Debug.WriteLine("Couldn't find valid mapping");
                        }
                        else //no restrictions, then add basic schedule
                        {
                            possibleSchedules.Add(s);
                        }
                    }
                }
            } //higher tst schedules, for those with higher than average needs
            else if ((monoBaseline <= 11 && activity <= 0) ||
                (monoBaseline <= 10 && activity <= 1) ||
                (monoBaseline <= 9 && activity <= 2))
            {
                foreach (Schedule s in _scheduleDB.FindAll(sched => convertDifferenceToDouble("00:00", sched.tst) >= 7.5))
                {
                    if (!_noRestrictions)
                    {
                        Schedule proposed = tryFitSchedule(userSleepTimes, s);
                        if (proposed != null)
                        {
                            possibleSchedules.Add(proposed);
                            if (consoleWrite) Debug.WriteLine("Added: " + proposed.name);
                        }
                        else if (consoleWrite) Debug.WriteLine("Couldn't find valid mapping");
                    }
                    else //no restrictions, then add basic schedule
                    {
                        possibleSchedules.Add(s);
                    }
                }
            } //higher tst schedules
            else
            {
                if (!_noRestrictions)
                {
                    Schedule proposed = tryFitSchedule(userSleepTimes, _scheduleDB.Find(s => s.name == "Monophasic"));
                    if (proposed != null)
                    {
                        possibleSchedules.Add(proposed);
                        if (consoleWrite) Debug.WriteLine("Added: " + proposed.name);
                    }
                    else if (consoleWrite) Debug.WriteLine("Couldn't find valid mapping");
                }
                else //no restrictions, then add basic schedule
                {
                    possibleSchedules.Add(_scheduleDB.Find(s => s.name == "Monophasic"));
                }
            } //mono for super high needs or high needs & high activity

            return possibleSchedules;
        }

        /// <summary> Attempts to crudely fit schedule to user's availability </summary>
        /// <param name="userSleepTimes">List of sleepblocks that show when the user can sleep</param>
        /// <param name="s">Schedule to attempt to match with the user's sleep times</param>
        private static Schedule tryFitSchedule(List<SleepBlock> userSleepTimes, Schedule s)
        {
            bool fullConsoleWrite = false; //for testing purposes only- spams console
            bool formatConsoleWrite = false;
            bool consoleWrite = false;

            if (consoleWrite) Console.WriteLine("\n");
            List<SleepBlock> sleeps = new List<SleepBlock>();
            Schedule proposed = new Schedule(s.name, sleeps, s.recommended, "", "", "", consoleWrite);
            bool basic = true;
            List<int[]> hRatingsList = new List<int[]>();
            Console.WriteLine(s.name + ": " + s.tst + ": " + s.getTST());

            foreach (SleepBlock block in s.sleeps) //goes through each sleep in the schedule being tried to place it in a sleep block that the user can sleep during
            {
                if (formatConsoleWrite) Console.WriteLine("start block " + block.startTime + "-" + block.endTime + " = " + getDifferenceTime(block.startTime, block.endTime) + " => " + convertTimeToInt(getDifferenceTime(block.startTime, block.endTime)) + "\n[");
                foreach (SleepBlock userBlock in userSleepTimes) //goes through each of the user's available sleep blocks to attempt to place sleep
                {
                    if (formatConsoleWrite) Console.WriteLine("\tstart userBlock " + userBlock.startTime + "-" + userBlock.endTime + " = " + getDifferenceTime(userBlock.startTime, userBlock.endTime) + " => " + convertTimeToInt(getDifferenceTime(userBlock.startTime, userBlock.endTime)) + "\n\t{");
                    if (checkSleepOverlapsEarliestToLatest(userBlock, block)) //does sleep's scheduling range overlap with user's sleep block?
                    {
                        if (formatConsoleWrite) Console.WriteLine("\t\t1: Sleep can overlap with this userBlock");
                        if (checkSleepNests(block, userBlock)) //if the basic time for this sleep fits into user's sleep block, use basic sleep time
                        {
                            sleeps.Add(block);
                            if (consoleWrite) Console.WriteLine("\t\t2: Sleep nests with this userBlock; Added basic sleep time: " + block.startTime + "-" + block.endTime);


                            hRatingsList.Add(uptateHRatings(block, userBlock)); //update hour ratings for gap refinement
                            if (formatConsoleWrite) Console.WriteLine("\t}");
                            if (formatConsoleWrite) Console.WriteLine("]");
                            break; //sleep is placed, so move on to avoid double placing sleep.
                        }
                        else //if it doesn't nest, try to customize sleep so that it does fit with user's sleep time and sleep range of that sleep as defined by Schedules.txt
                        {
                            if (formatConsoleWrite) Console.WriteLine("\t\t3: Sleep does not nest");
                            int blockLength = convertTimeToInt(getDifferenceTime(block.startTime, block.endTime));
                            if (formatConsoleWrite) Console.WriteLine("\t\t" + convertTimeToInt(getDifferenceTime(userBlock.startTime, userBlock.endTime)) + " >= " + blockLength + " | " + checkSleepOverlapsEarliestToLatest(userBlock, block));
                            if (convertTimeToInt(getDifferenceTime(userBlock.startTime, userBlock.endTime)) >= blockLength && checkSleepOverlapsEarliestToLatest(userBlock, block)) //makes sure sleep can fit into user's sleep block
                            {
                                if (formatConsoleWrite) Console.WriteLine("\t\t4: Sleep can fit into this userBlock");
                                basic = false; //not the basic sleep time for sleep
                                int[] hRatings = uptateHRatings(block, userBlock); //update hour ratings to find optimal customization of sleep based on availability and vital sleep avaiability rating

                                int bestTime = 0; bool bestSet = false; //used to find the optimal time
                                for (int i = 0; i < 288; i++) //goes through each 5m block to see if it's better than the current best option
                                {
                                    if (fullConsoleWrite) Console.WriteLine((convertDoubleToTime((double)i / 12)) + ": " + hRatings[i] + ", " + hRatings[bestTime]);
                                    if ((hRatings[i] > hRatings[bestTime]) || (!bestSet && hRatings[i] == hRatings[bestTime] && hRatings[i] != 0)) //if this option is better than anything else so far, update bestTime
                                    {
                                        bestSet = true; //tracks whether a best time has been set (in case bestTime = 0)
                                        bestTime = i;
                                        if (formatConsoleWrite) Console.WriteLine("bestTime = " + bestTime + ", " + bestSet);
                                    }
                                }
                                if (bestSet)
                                {
                                    double bestTimeD = (double)bestTime * 5 / 60; //convert to hour double

                                    double bestTimeStart, bestTimeEnd, blockLengthDouble;

                                    blockLengthDouble = convertDifferenceToDouble(block.startTime, block.endTime);
                                    bestTimeStart = bestTimeD;
                                    bestTimeEnd = bestTimeD + blockLengthDouble;
                                    if (bestTimeEnd > 24) bestTimeEnd -= 24;

                                    proposed.sleeps.Add(new SleepBlock(convertDoubleToTime(blockLengthDouble), convertDoubleToTime(bestTimeStart), convertDoubleToTime(bestTimeEnd)));
                                    proposed.sleeps[proposed.sleeps.Count - 1].type = block.type;
                                    hRatingsList.Add(hRatings);
                                    if (consoleWrite) Console.WriteLine("\t\t5: Customized sleep time to: " + convertDoubleToTime(bestTimeStart) + "-" + convertDoubleToTime(bestTimeEnd));
                                    if (formatConsoleWrite) Console.WriteLine("\t}");
                                    if (formatConsoleWrite) Console.WriteLine("]");
                                    break;
                                }
                            }
                            else if (formatConsoleWrite) Console.WriteLine("\t\t6: Sleep cannot fit within this block");
                        }
                    }
                    if (formatConsoleWrite) Console.WriteLine("\t}");
                }
                if (formatConsoleWrite) Console.WriteLine("]");
            }

            if (proposed.sleeps.Count != s.sleeps.Count) return null; //if too few sleeps were set, this schedule didn't fit all sleeps, so it didn't work
            else if(!basic) proposed = fixGaps(proposed, s, hRatingsList, new List<double>(), new List<int>(), new int[] { 0, 0 }); //fix gaps to confirm it's viable

            if (proposed == null) return null; //checks that fixGaps didn't return null, meaning that it wasn't able to fit the sleeps with reasonable gaps
            if (consoleWrite)
            {
                if (basic) Console.WriteLine("Returning basic " + s.name + " schedule");
                else Console.WriteLine("Returning customized " + s.name + " schedule");
            }

            return proposed;
        }

        /// <summary> Updates hour ratings based on schedule availability to help prioritize sleep placement </summary>
        /// <param name="block">Sleep block of the schedule</param>
        /// <param name="userBlock">Sleep block of user's availability</param>
        private static int[] uptateHRatings(SleepBlock block, SleepBlock userBlock)
        {
            bool consoleWrite = false;

            int[] hRatings = new int[288];
            for (int i = 0; i < 288; i++) 
                hRatings[i] = _hourRatings[(int)Math.Floor((double)(i * 5 / 60)), 0];
            
            double blockLength = convertDifferenceToDouble(block.startTime, block.endTime);
            double[] start = addTime(convertDifferenceToDouble("00:00", block.startTime), 0);
            double[] end = addTime(convertDifferenceToDouble("00:00", block.endTime), 0);
            double[] earliest = addTime(convertDifferenceToDouble("00:00", block.earliestStartTime), 0);
            double[] latest = addTime(convertDifferenceToDouble("00:00", block.latestEndTime), 0);
            double[] uStart = addTime(convertDifferenceToDouble("00:00", userBlock.startTime), 0);
            double[] uEnd = addTime(convertDifferenceToDouble("00:00", userBlock.endTime), 0);

            if (consoleWrite) Console.WriteLine("\n" + convertDoubleToTime(uStart[0]) + "-" + convertDoubleToTime(uEnd[0]) + "; " + convertDoubleToTime(start[0]) + "-" + convertDoubleToTime(end[0]) + " (" + convertDoubleToTime(blockLength) + "); " + convertDoubleToTime(earliest[0]) + "-" + convertDoubleToTime(latest[0]) + " -> ");

            if (consoleWrite) Console.WriteLine(
                "earliest = " +    shorten(earliest[0]) + "/" +  shorten(earliest[1]) +
                "; latest = " +    shorten(latest[0]) + "/" +    shorten(latest[1]) +
                "; start = " +     shorten(start[0]) + "/" +     shorten(start[1]) +
                "; end = " +       shorten(end[0]) + "/" +       shorten(end[1]) +
                "; uStart = " +    shorten(uStart[0]) + "/" +    shorten(uStart[1]) +
                "; uEnd = " +      shorten(uEnd[0]) + "/" +      shorten(uEnd[1]));

            for (int i = 0; i < 288; i++)
            {
                double[] iStart = addTime((double)i * 5 / 60, 0); //i converted to hour double
                double[] iEnd = addTime(iStart[0], blockLength); //end of block shifted
                SleepBlock iBlock = new SleepBlock(convertDoubleToTime(blockLength), convertDoubleToTime(iStart[0]), convertDoubleToTime(iEnd[0]));

                if (consoleWrite) Console.Write("\n/" + i + ": iStart = " + shorten(iStart[0]) + "; iEnd = " + shorten(iEnd[0]) + "; " + hRatings[i] + " -> /");
                
                if (!checkSleepNests(iBlock, userBlock) || !checkSleepNestsEarliestToLatest(iBlock, block)) //if outside the range of the block, set to 0 so that it isn't an option
                {
                    if (consoleWrite) Console.Write(//"|" + checkSleepNests(iBlock, userBlock) + "," + checkSleepNestsEarliestToLatest(iBlock, block) + " | " + 
                        convertDoubleToTime(iStart[0]) + ": 1 ((" + convertDoubleToTime(iStart[0]) + "-" + convertDoubleToTime(iEnd[0]) + ") - (" + convertDoubleToTime(earliest[0]) + "-" + convertDoubleToTime(latest[0]) + ")): " + "|");
                    hRatings[i] = 0;
                }
                else if (((iStart[0] > iEnd[0]) && (start[0] < end[0]) && (iStart[0] < start[1])) || (iStart[0] < start[0])) //if in range, but before the beginning of the sample block, reduce the rating by decreasing amounts as distance decreases- prioritizes proximity to basic sleep times
                {
                    if (consoleWrite) Console.Write("(" + convertDoubleToTime(iStart[0]) + ": 2 ((" + convertDoubleToTime(iStart[0]) + "-" + convertDoubleToTime(iEnd[0]) + ") - (" + convertDoubleToTime(earliest[0]) + "-" + convertDoubleToTime(latest[0]) + ")): " + ")");
                    hRatings[i] = hRatings[i] + (int)(iStart[0] - Math.Ceiling(start[0]));
                }
                else if (((iStart[0] > iEnd[0]) && (start[0] < end[0]) && (iEnd[0] > end[1])) || (iEnd[0] > end[0])) //if in range, but after the end of the sample block, reduce the rating by increasing amounts as distance increases- prioritizes proximity to basic sleep times
                {
                    if (consoleWrite) Console.Write("<" + convertDoubleToTime(iStart[0]) + ": 3 ((" + convertDoubleToTime(iStart[0]) + "-" + convertDoubleToTime(iEnd[0]) + ") - (" + convertDoubleToTime(earliest[0]) + "-" + convertDoubleToTime(latest[0]) + ")): " + ">");
                    hRatings[i] = hRatings[i] + (int)(Math.Ceiling(end[0]) - iEnd[0]);
                }
                else //catches anything else as not viable
                {
                    hRatings[i] = 0;
                }
                if (consoleWrite && hRatings[i] > 0) Console.WriteLine("{" + i + " -> " + shorten(iStart[0]) + ": " + hRatings[i] + "}");
                else if (consoleWrite) Console.WriteLine("[" + hRatings[i] + "]");
            }
            return hRatings;
        }

        /// <summary> Refines gaps to be within reasonable ranges, as defined by Schedules.txt </summary>
        /// <param name="userSched">Custom schedule fit to user's sleep times</param>
        /// <param name="basicSched">Basic version of the same schedule</param>
        /// <param name="hrl">Hour Rating List (list of ratings of how viable each 5m mark is for scheduling sleep</param>
        /// <param name="gaps">Distances between each sleep</param>
        /// <param name="indexesOfProblemGaps">Index of problematic gaps in the gaps list/param>
        /// <param name="depth">Number of recursions of fixGaps()</param>
        private static Schedule fixGaps(Schedule userSched, Schedule basicSched, List<int[]> hrl, List<double> gaps, List<int> indexesOfProblemGaps, int[] depth)
        {
            bool consoleWrite = false;

            if (depth[0] > 340) return null; //if recursively tried too many times, it has hit a deadlock or cycle, and that means that this isn't a fixable gap, so the schedule doesn't work
            int previousDepth = depth[1]; //Tracks number of problem gaps

            if (depth[0] == 0) //setup on entry
            {
                for (int i = 0; i < userSched.sleeps.Count; i++) //goes through gaps to gather data about gaps
                {
                    gaps.Add(convertDifferenceToDouble(userSched.sleeps[i].endTime, userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime)); //get gap length between each sleep
                    if (consoleWrite) Console.WriteLine(userSched.sleeps[i].endTime + " + " + userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime + " = " + gaps[i]);

                    if (gaps[i] < 3 || gaps[i] > convertDifferenceToDouble("00:00", basicSched.sleeps[i + 1 < basicSched.sleeps.Count ? i + 1 : 0].maxDistanceFromPreviousSleepBlock)) //finds problem gaps (less than 3h or greater than max gap length defined in Schedules.txt)
                    {
                        indexesOfProblemGaps.Add(i);
                        if (consoleWrite) Console.WriteLine("a) Problem Gap!");
                    }
                }
            }

            List<int> iopg = new List<int>(indexesOfProblemGaps);

            for (int x = 0; x < indexesOfProblemGaps.Count; x++) //fixing problem gaps
            {
                int indx = indexesOfProblemGaps[x];
                if (consoleWrite) Console.WriteLine("\n" + x + ": entering tests");
                if (gaps[indx] < 3) //fix gaps that are too short
                {
                    double gap = gaps[indx];

                    while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) < 3 &&
                        hrl[indx][stringToMinutes(userSched.sleeps[indx].startTime, -1)] != 0) //checks that sleep at the end of the gap can be shifted forward
                    {
                        userSched.sleeps[indx].startTime = convertDoubleToTime(convertDifferenceToDouble("00:05", userSched.sleeps[indx].startTime)); //moves by 5m at a time to ensure optimal placement
                        userSched.sleeps[indx].endTime = convertDoubleToTime(convertDifferenceToDouble("00:05", userSched.sleeps[indx].endTime));
                        if (consoleWrite) Console.Write("moved 1 forward 5m: ");
                        if (consoleWrite) Console.WriteLine(userSched.sleeps[indx].startTime + "-" + userSched.sleeps[indx].endTime);
                    }

                    if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) >= 3) //checks if gap was fixed
                    {
                        if (consoleWrite) Console.WriteLine("short1!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ": " +
                            userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                        iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                    }
                    else //if it wasn't fixed, try moving other sleep
                    {
                        while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) < 3 &&
                        hrl[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0][stringToMinutes(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime, 1)] != 0) //checks that sleep at the beginning of the gap can be moved backwards
                        {
                            userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime =
                                convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime)); //moves by 5m at a time to ensure optimal placement
                            userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime =
                                convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime));
                            if (consoleWrite) Console.Write("moved 2 forward 5m: ");
                            if (consoleWrite) Console.WriteLine(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime + "-" + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime);
                        }

                        if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) >= 3) //checks if gap was fixed
                        {
                            if (consoleWrite) Console.WriteLine("short2!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ": " +
                            userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                            iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                        }
                        else if (consoleWrite) Console.WriteLine("short!!!!!!Couldn't move!");
                    }
                }
                else //fixing gaps that are too long, as defined by Schedules.txt
                {
                    double gap = gaps[indx];
                    double maxDistance = convertDifferenceToDouble("00:00", basicSched.sleeps[indx + 1 < basicSched.sleeps.Count ? indx + 1 : 0].maxDistanceFromPreviousSleepBlock);

                    while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) > maxDistance &&
                        hrl[indx][stringToMinutes(userSched.sleeps[indx].startTime, 1)] != 0)//checks that sleep at the end of the gap can be moved backwards
                    {
                        userSched.sleeps[indx].startTime = convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx].startTime)); //moves by 5m at a time to ensure optimal placement
                        userSched.sleeps[indx].endTime = convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx].endTime));
                        if (consoleWrite) Console.Write("moved 1 forward 5m: ");
                        if (consoleWrite) Console.WriteLine(userSched.sleeps[indx].startTime + "-" + userSched.sleeps[indx].endTime);
                    }

                    if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) <= maxDistance) //checks if gap was fixed
                    {
                        if (consoleWrite) Console.WriteLine("long1!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ": " +
                        userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                        iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                    }
                    else //if it wasn't fixed, try moving other sleep
                    {
                        while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) < 3 &&
                        hrl[indx][stringToMinutes(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime, -1)] != 0) //checks that sleep at the beginning of the gap can be shifted forward 
                        {
                            userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime =
                                convertDoubleToTime((double)convertDifferenceToDouble("00:05", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + (double)(5 / 60)); //moves by 5m at a time to ensure optimal placement
                            userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime =
                                convertDoubleToTime((double)convertDifferenceToDouble("00:05", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime) + (double)(5 / 60));
                            if (consoleWrite) Console.Write("moved 2 back 5m: ");
                            if (consoleWrite) Console.WriteLine(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime + "-" + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime);
                        }

                        if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) <= maxDistance) //checks if gap was fixed
                        {
                            if (consoleWrite) Console.WriteLine("long2!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ":" +
                                userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                            iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                        }
                        else if (consoleWrite) Console.WriteLine("long!!!!!!Couldn't move!");
                    }
                }
            }
            indexesOfProblemGaps = iopg;
            if (consoleWrite) Console.WriteLine(indexesOfProblemGaps.Count + "\n");

            if (indexesOfProblemGaps.Count == 0) //if there are no more problem gaps, check to see if any new problem gaps have been created by moving around the sleeps
            {
                gaps = new List<double>();
                for (int i = 0; i < userSched.sleeps.Count; i++)
                {
                    gaps.Add(convertDifferenceToDouble(userSched.sleeps[i].endTime, userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime));
                    if (consoleWrite) Console.WriteLine(userSched.sleeps[i].endTime + " + " + userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime + " = " + gaps[i]);

                    if (gaps[i] < 3 || gaps[i] > convertDifferenceToDouble("00:00", basicSched.sleeps[i + 1 < basicSched.sleeps.Count ? i + 1 : 0].maxDistanceFromPreviousSleepBlock))
                    {
                        indexesOfProblemGaps.Add(i);
                        if (consoleWrite) Console.WriteLine("b) Problem Gap!");
                    }
                }
            }

            depth[1] = indexesOfProblemGaps.Count; //tracks count of problem gaps for each recursion
            depth[0]++; //tracks number of problem gaps


            if (consoleWrite) Console.WriteLine("\n");
            if (indexesOfProblemGaps.Count > 0 || previousDepth > 0) //if there are still problem gaps, try again
            {
                if(new Random().Next(0, 2) == 0) userSched = fixGaps(userSched, basicSched, hrl, gaps, indexesOfProblemGaps, depth);
                else userSched = fixGapsReverse(userSched, basicSched, hrl, gaps, indexesOfProblemGaps, depth);
            }

            return userSched;
        }

        /// <summary> Refines gaps to be within reasonable ranges, as defined by Schedules.txt 
        /// but tries moving the sleeps in reverse order in case there is a gap that can't be fixed by 
        /// moving the first sleep back or forward first. </summary>
        /// <param name="userSched">Custom schedule fit to user's sleep times</param>
        /// <param name="basicSched">Basic version of the same schedule</param>
        /// <param name="hrl">Hour Rating List (list of ratings of how viable each 5m mark is for scheduling sleep</param>
        /// <param name="gaps">Distances between each sleep</param>
        /// <param name="indexesOfProblemGaps">Index of problematic gaps in the gaps list/param>
        /// <param name="depth">Number of recursions of fixGaps()</param>
        private static Schedule fixGapsReverse(Schedule userSched, Schedule basicSched, List<int[]> hrl, List<double> gaps, List<int> indexesOfProblemGaps, int[] depth)
        {
            bool consoleWrite = false;

            if (depth[0] > 340) return null; //if recursively tried too many times, it has hit a deadlock or cycle, and that means that this isn't a fixable gap, so the schedule doesn't work
            int previousDepth = depth[1]; //Tracks number of problem gaps

            if (depth[0] == 0) //setup on entry
            {
                for (int i = 0; i < userSched.sleeps.Count; i++) //goes through gaps to gather data about gaps
                {
                    gaps.Add(convertDifferenceToDouble(userSched.sleeps[i].endTime, userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime)); //get gap length between each sleep
                    if (consoleWrite) Console.WriteLine(userSched.sleeps[i].endTime + " + " + userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime + " = " + gaps[i]);

                    if (gaps[i] < 3 || gaps[i] > convertDifferenceToDouble("00:00", basicSched.sleeps[i + 1 < basicSched.sleeps.Count ? i + 1 : 0].maxDistanceFromPreviousSleepBlock)) //finds problem gaps (less than 3h or greater than max gap length defined in Schedules.txt)
                    {
                        indexesOfProblemGaps.Add(i);
                        if (consoleWrite) Console.WriteLine("a) Problem Gap!");
                    }
                }
            }

            List<int> iopg = new List<int>(indexesOfProblemGaps);

            for (int x = 0; x < indexesOfProblemGaps.Count; x++) //fixing problem gaps
            {
                int indx = indexesOfProblemGaps[x];
                if (consoleWrite) Console.WriteLine("\n" + x + ": entering tests");
                if (gaps[indx] < 3) //fix gaps that are too short
                {
                    double gap = gaps[indx];

                    while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) < 3 &&
                    hrl[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0][stringToMinutes(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime, 1)] != 0) //checks that sleep at the beginning of the gap can be moved backwards
                    {
                        userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime =
                            convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime)); //moves by 5m at a time to ensure optimal placement
                        userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime =
                            convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime));
                        if (consoleWrite) Console.Write("moved 2 forward 5m: ");
                        if (consoleWrite) Console.WriteLine(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime + "-" + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime);
                    }

                    if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) >= 3) //checks if gap was fixed
                    {
                        if (consoleWrite) Console.WriteLine("Rshort1!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ": " +
                            userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                        iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                    }
                    else //if it wasn't fixed, try moving other sleep
                    {
                        while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) < 3 &&
                            hrl[indx][stringToMinutes(userSched.sleeps[indx].startTime, -1)] != 0) //checks that sleep at the end of the gap can be shifted forward
                        {
                            userSched.sleeps[indx].startTime = convertDoubleToTime(convertDifferenceToDouble("00:05", userSched.sleeps[indx].startTime)); //moves by 5m at a time to ensure optimal placement
                            userSched.sleeps[indx].endTime = convertDoubleToTime(convertDifferenceToDouble("00:05", userSched.sleeps[indx].endTime));
                            if (consoleWrite) Console.Write("moved 1 forward 5m: ");
                            if (consoleWrite) Console.WriteLine(userSched.sleeps[indx].startTime + "-" + userSched.sleeps[indx].endTime);
                        }

                        if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) >= 3) //checks if gap was fixed
                        {
                            if (consoleWrite) Console.WriteLine("Rshort2!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ": " +
                            userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                            iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                        }
                        else if (consoleWrite) Console.WriteLine("Rshort!!!!!!Couldn't move!");
                    }
                }
                else //fixing gaps that are too long, as defined by Schedules.txt
                {
                    double gap = gaps[indx];
                    double maxDistance = convertDifferenceToDouble("00:00", basicSched.sleeps[indx + 1 < basicSched.sleeps.Count ? indx + 1 : 0].maxDistanceFromPreviousSleepBlock);

                    while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) < 3 &&
                    hrl[indx][stringToMinutes(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime, -1)] != 0) //checks that sleep at the beginning of the gap can be shifted forward 
                    {
                        userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime =
                            convertDoubleToTime((double)convertDifferenceToDouble("00:05", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + (double)(5 / 60)); //moves by 5m at a time to ensure optimal placement
                        userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime =
                            convertDoubleToTime((double)convertDifferenceToDouble("00:05", userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime) + (double)(5 / 60));
                        if (consoleWrite) Console.Write("moved 2 back 5m: ");
                        if (consoleWrite) Console.WriteLine(userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime + "-" + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].endTime);
                    }

                    if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) <= maxDistance) //checks if gap was fixed
                    {
                        if (consoleWrite) Console.WriteLine("Rlong1!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ": " +
                        userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                        iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                    }
                    else //if it wasn't fixed, try moving other sleep
                    {
                        while (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) > maxDistance &&
                            hrl[indx][stringToMinutes(userSched.sleeps[indx].startTime, 1)] != 0)//checks that sleep at the end of the gap can be moved backwards
                        {
                            userSched.sleeps[indx].startTime = convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx].startTime)); //moves by 5m at a time to ensure optimal placement
                            userSched.sleeps[indx].endTime = convertDoubleToTime(convertDifferenceToDouble("23:55", userSched.sleeps[indx].endTime));
                            if (consoleWrite) Console.Write("moved 1 forward 5m: ");
                            if (consoleWrite) Console.WriteLine(userSched.sleeps[indx].startTime + "-" + userSched.sleeps[indx].endTime);
                        }

                        if (convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) <= maxDistance) //checks if gap was fixed
                        {
                            if (consoleWrite) Console.WriteLine("Rlong2!!!!!!!!! " + gap + " -> " + convertDifferenceToDouble(userSched.sleeps[indx].endTime, userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime) + ":" +
                                userSched.sleeps[indx].endTime + ", " + userSched.sleeps[indx + 1 < userSched.sleeps.Count ? indx + 1 : 0].startTime);
                            iopg.Remove(indexesOfProblemGaps[x]); //remove from the to-fix list
                        }
                        else if (consoleWrite) Console.WriteLine("Rlong!!!!!!Couldn't move!");
                    }
                }
            }
            indexesOfProblemGaps = iopg;
            if (consoleWrite) Console.WriteLine(indexesOfProblemGaps.Count + "\n");

            if (indexesOfProblemGaps.Count == 0) //if there are no more problem gaps, check to see if any new problem gaps have been created by moving around the sleeps
            {
                gaps = new List<double>();
                for (int i = 0; i < userSched.sleeps.Count; i++)
                {
                    gaps.Add(convertDifferenceToDouble(userSched.sleeps[i].endTime, userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime));
                    if (consoleWrite) Console.WriteLine(userSched.sleeps[i].endTime + " + " + userSched.sleeps[i + 1 < userSched.sleeps.Count ? i + 1 : 0].startTime + " = " + gaps[i]);

                    if (gaps[i] < 3 || gaps[i] > convertDifferenceToDouble("00:00", basicSched.sleeps[i + 1 < basicSched.sleeps.Count ? i + 1 : 0].maxDistanceFromPreviousSleepBlock))
                    {
                        indexesOfProblemGaps.Add(i);
                        if (consoleWrite) Console.WriteLine("b) Problem Gap!");
                    }
                }
            }

            depth[1] = indexesOfProblemGaps.Count; //tracks count of problem gaps for each recursion
            depth[0]++; //tracks number of problem gaps


            if (consoleWrite) Console.WriteLine("\n");
            if (indexesOfProblemGaps.Count > 0 || previousDepth > 0) //if there are still problem gaps, try again
            {
                if (new Random().Next(0, 2) == 0) userSched = fixGaps(userSched, basicSched, hrl, gaps, indexesOfProblemGaps, depth);
                else userSched = fixGapsReverse(userSched, basicSched, hrl, gaps, indexesOfProblemGaps, depth);
            }

            return userSched;
        }
        #endregion

        #region converters
        /// <summary> modifies mono entry in db to fit user's mono length </summary>
        public static void modifyMono(double monoBaseline)
        {
            int mono = _scheduleDB.Count - 1;
            _scheduleDB[mono].tst = convertDoubleToTime(monoBaseline);
            _scheduleDB[mono].sleeps[0].length = convertDoubleToTime(monoBaseline);
            double newEnd = convertDifferenceToDouble("00:00", _scheduleDB[mono].sleeps[0].startTime) + monoBaseline;
            if (newEnd > 23.99) newEnd -= 24;
            _scheduleDB[mono].sleeps[0].endTime = convertDoubleToTime(newEnd);
        }

        /// <summary> 6.25 -> 06:15 </summary>
        public static string convertDoubleToTime(double time)
        {
            double hour = Math.Floor(time);

            string minutes = Math.Round((time - Math.Floor(time)) * 60).ToString();
            if (minutes.Length < 2) minutes = "0" + minutes;
            if (minutes == "60")
            {
                minutes = "00";
                hour++;
            }

            if (hour >= 24) hour -= 24;
            string hours = hour.ToString();
            if (hours.Length < 2) hours = "0" + hours;

            return hours + ":" + minutes;
        }

        /// <summary> 02:15 -> 135 </summary>
        public static int convertTimeToInt(string time)
        {
            int hours, minutes;
            Int32.TryParse(time.Substring(0, time.IndexOf(":")), out hours);
            Int32.TryParse(time.Remove(0, time.IndexOf(":") + 1), out minutes);

            hours *= 60;
            return hours + minutes;
        }

        /// <summary> (00:15), (07:45) -> 7.5 </summary>
        public static double convertDifferenceToDouble(string startTime, string endTime) => (double)convertTimeToInt(getDifferenceTime(startTime, endTime)) / 60;

        /// <summary> (00:15), (07:45) -> 07:30 </summary>
        public static string getDifferenceTime(string startTime, string endTime)
        {
            int min, hours, min1, min2, h1, h2;

            h1 = Int32.Parse(startTime.Substring(0, startTime.IndexOf(':')));
            h2 = Int32.Parse(endTime.Substring(0, endTime.IndexOf(':')));
            min1 = Int32.Parse(startTime.Substring(startTime.IndexOf(':') + 1));
            min2 = Int32.Parse(endTime.Substring(endTime.IndexOf(':') + 1));

            if (min2 < min1)
            {
                h1++;
                min2 += 60;
            }

            min = min2 - min1;

            if (h2 < h1) h2 += 24;

            hours = h2 - h1;

            return ((hours > 0 ? (hours < 10 ? "0" + hours : hours.ToString()) : "00") + ":" + (min > 0 ? (min < 10 ? "0" + min : min.ToString()) : "00")).Trim();
        }

        /// <summary> (03:15), (25) -> 64 </summary>
        public static int stringToMinutes(string minutes, int difference)
        {
            double time = convertDifferenceToDouble("00:00", minutes); //(03:15) -> 3.25
            time = (time * 60 / 5) + difference; //3.25 -> 39 + 25 = 64

            if (time < 0) time += 288;
            else if (time >= 288) time -= 288;

            return (int)Math.Floor(time);
        }

        /// <summary> (2.30), (2) -> (4.30, 28.30) </summary>
        /// <param name="time">The time to shift</param>
        /// <param name="add">Hours to shift by</param>
        /// <param name="rollOverToNewDay">Whether or not to reduce time by 24h if it crosses into a new day</param>
        private static double[] addTime(double time, double add) 
        {
            double[] summed = new double[2];

            double sum = time + add;

            summed[0] = sum >= 24 ? sum - 24 : sum;
            summed[1] = sum >= 24 ? sum : sum + 24;

            return summed;
        }

        /// <summary> Reduces doubles to 2 decimals or less (for printing purposes) </summary>
        private static string shorten(double input)
        {
            string i = input.ToString();
            if (i.Contains("."))
            {
                if (i.Remove(0, i.IndexOf(".") + 1).Length > 2)
                {
                    return i.Remove(i.IndexOf(".") + 2);
                }
            }

            return i;
        }
        #endregion

        #region checks
        /// <summary> Check that sleep range is in a valid format </summary>
        /// <param name="result">Sleep time to check (00:00)</param>
        internal static bool checkLongSleepTime(string result)
        {
            if (result.Length < 9) return false;

            if (Regex.IsMatch(result, "([0-1]?[0-9]|2[0-3]):[0-5][0-9]-([0-1]?[0-9]|2[0-3]):[0-5][0-9]") &&
                (Int32.Parse(result.Substring(0, result.IndexOf(":"))) < 24) &&
                (Int32.Parse(result.Remove(0, result.IndexOf("-") + 1).Substring(0, result.Remove(0, result.IndexOf("-") + 1).IndexOf(":"))) < 24) &&
                result.Substring(0, result.IndexOf("-")) != result.Remove(0, result.IndexOf("-") + 1)) return true;
            else
            {
                Console.WriteLine("ERROR: Invalid sleep times! Please enter in the format: [00:00-23:59]");
                return false;
            }
        }

        /// <summary> Check overlaps by start time and end time in the format "22:30" and "07:59" and by sleepblock lists </summary>
        internal static bool checkSleepOverlaps(string rStartTime, string rEndTime, List<SleepBlock> userSleepTimes)
        {
            bool consoleWrite = false;

            for (int i = 0; i < userSleepTimes.Count; i++) //goes through each block in user's sleep times to ensure no overlaps
            {
                int h1, h2, rh1, rh2, m1, m2, rm1, rm2;
                string startTime = userSleepTimes[i].startTime;
                string endTime = userSleepTimes[i].endTime;

                h1 = Int32.Parse(startTime.Substring(0, startTime.IndexOf(":")));
                h2 = Int32.Parse(endTime.Substring(0, endTime.IndexOf(":")));
                m1 = Int32.Parse(startTime.Remove(0, startTime.IndexOf(":") + 1));
                m2 = Int32.Parse(endTime.Remove(0, endTime.IndexOf(":") + 1));
                rh1 = Int32.Parse(rStartTime.Substring(0, rStartTime.IndexOf(":")));
                rh2 = Int32.Parse(rEndTime.Substring(0, rEndTime.IndexOf(":")));
                rm1 = Int32.Parse(rStartTime.Remove(0, rStartTime.IndexOf(":") + 1));
                rm2 = Int32.Parse(rEndTime.Remove(0, rEndTime.IndexOf(":") + 1));

                if (h2 < h1) //shift so that blocks are in the right order to check
                {
                    h2 += 24;
                    rh1 += 24;
                    rh2 += 24;
                }

                if (rh2 < rh1) //shift so that blocks are in the right order to check
                {
                    rh2 += 24;
                    h1 += 24;
                    h2 += 24;
                }

                if (consoleWrite) Console.WriteLine(h1 + ":" + m1 + ", " + h2 + ":" + m2 + ", " + rh1 + ":" + rm1 + ", " + rh2 + ":" + rm2);

                bool r1LessThanH1, r2LessThanH2, r1LessThanH2, r2LessThanH1;

                if (rh1 < h1 || (rh1 == h1 && rm1 < m1)) r1LessThanH1 = true; //simple checks to see if there is any form of overlap
                else r1LessThanH1 = false;

                if (rh2 < h2 || (rh2 == h2 && rm2 < m2)) r2LessThanH2 = true;
                else r2LessThanH2 = false;

                if (rh1 < h2 || (rh1 == h2 && rm1 < m2)) r1LessThanH2 = true;
                else r1LessThanH2 = false;

                if (rh2 < h1 || (rh2 == h1 && rm2 < m1)) r2LessThanH1 = true;
                else r2LessThanH1 = false;

                if (!((r1LessThanH1 && r2LessThanH1) || (!r1LessThanH2 && !r2LessThanH2))) return true; //if there's an overlap, return
            }

            return false; //if no overlaps have been detected, return
        }

        /// <summary> Compares a block with a block's earliestStart-latestEnd variables to see if they overlap </summary>
        /// <param name="uBlock">The block to compare</param>
        /// <param name="extendedBlock">The block to get earliest-latest from</param>
        internal static bool checkSleepOverlapsEarliestToLatest(SleepBlock uBlock, SleepBlock extendedBlock)
        {
            double[] start = addTime(convertDifferenceToDouble("00:00", uBlock.startTime), 0);
            double[] end = addTime(convertDifferenceToDouble("00:00", uBlock.endTime), 0);
            double[] earliest = addTime(convertDifferenceToDouble("00:00", extendedBlock.earliestStartTime), 0);
            double[] latest = addTime(convertDifferenceToDouble("00:00", extendedBlock.latestEndTime), 0);

            if (((start[0] < end[0]) && (earliest[0] > latest[0]) &&
                (((start[1] >= earliest[0]) && (start[0] < latest[0])) || ((end[0] <= latest[1]) && (end[0] > earliest[0])))) ||
                ((start[0] > end[0]) && (earliest[0] > latest[0]) &&
                (((start[0] >= earliest[0]) && (start[0] < latest[1])) || ((end[0] <= latest[0]) && (end[1] > earliest[0])))) ||
                ((start[0] > end[0]) && (earliest[0] < latest[0]) &&
                (((start[0] >= earliest[0]) && (start[0] < latest[0])) || ((end[0] <= latest[0]) && (end[0] > earliest[0])))) ||
                ((start[0] < end[0]) && (earliest[0] < latest[0]) &&
                (((start[0] >= earliest[0]) && (start[0] < latest[0])) || ((end[0] <= latest[0]) && (end[0] > earliest[0])))) ||
                checkSleepNests(extendedBlock, uBlock))
            {
                return true;
            }
            else return false;
        }

        /// <summary> Checks to see if a block fits completely within another block's earliest-latest range  </summary>
        /// <param name="block">The block to try nesting</param>
        /// <param name="extendedBlock">The block to get earliest-latest from to nest the other block within</param>
        internal static bool checkSleepNestsEarliestToLatest(SleepBlock block, SleepBlock extendedBlock)
        {
            double[] start = addTime(convertDifferenceToDouble("00:00", block.startTime), 0);
            double[] end = addTime(convertDifferenceToDouble("00:00", block.endTime), 0);
            double[] earliest = addTime(convertDifferenceToDouble("00:00", extendedBlock.earliestStartTime), 0);
            double[] latest = addTime(convertDifferenceToDouble("00:00", extendedBlock.latestEndTime), 0);

            if ((start[0] > end[0]) && (earliest[0] < latest[0])) return false;
            else if (
            ((((start[0] < end[0]) && (earliest[0] < latest[0])) || ((start[0] > end[0]) && (earliest[0] > latest[0]))) &&
            start[0] >= earliest[0] && end[0] <= latest[0]) ||
            ((start[0] < end[0]) && (earliest[0] > latest[0]) && start[0] < earliest[0] &&
            start[1] >= earliest[0] && end[0] <= latest[0]) ||
            ((start[0] < end[0]) && (earliest[0] > latest[0]) && start[0] >= earliest[0] &&
            start[0] >= earliest[0] && end[0] <= latest[1]))
            {
                return true;
            }
            else return false;
        }

        /// <summary> Checks to see if a block fits completely within another block </summary>
        /// <param name="block">The block to try nesting</param>
        /// <param name="userBlock">The block to nest the other block within</param>
        internal static bool checkSleepNests(SleepBlock block, SleepBlock userBlock)
        {
            double[] start = addTime(convertDifferenceToDouble("00:00", block.startTime), 0);
            double[] end = addTime(convertDifferenceToDouble("00:00", block.endTime), 0);
            double[] uStart = addTime(convertDifferenceToDouble("00:00", userBlock.startTime), 0);
            double[] uEnd = addTime(convertDifferenceToDouble("00:00", userBlock.endTime), 0);

            if ((start[0] > end[0]) && (uStart[0] < uEnd[0])) return false;
            else if (
            ((((start[0] < end[0]) && (uStart[0] < uEnd[0])) || ((start[0] > end[0]) && (uStart[0] > uEnd[0]))) &&
            start[0] >= uStart[0] && end[0] <= uEnd[0]) ||
            ((start[0] < end[0]) && (uStart[0] > uEnd[0]) && start[0] < uStart[0] &&
            start[1] >= uStart[0] && end[0] <= uEnd[0]) ||
            ((start[0] < end[0]) && (uStart[0] > uEnd[0]) && start[0] >= uStart[0] &&
            start[0] >= uStart[0] && end[0] <= uEnd[1]))
            {
                return true;
            }
            else return false;
        }

        /// <summary> Checks if the chosen schedule has a higher tst than their mono baseline </summary>
        internal static bool shorterThanMono(double monoBaseline, string schedTST) => monoBaseline >= convertDifferenceToDouble("00:00", schedTST);
        #endregion
    }
}
