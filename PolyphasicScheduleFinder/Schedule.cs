using System;
using System.Collections.Generic;
using System.Text;

namespace PolyphasicScheduleFinder
{
    class Schedule
    {
        #region attributes
        public Schedule(string inName, List<SleepBlock> inSleeps, bool rec, string inDPS, string inDPE, string inLink, bool consoleWrite)
        {
            name = inName;
            sleeps = inSleeps;
            if(inLink == "")
            {
                link = "https://www.polyphasic.net/schedules/" + name.Replace(" ", "-") + "/";
            }
            else
            {
                link = inLink;
            }
            recommended = rec;
            DPEarliestStart = inDPS;
            DPLatestEnd = inDPE;

            double totalSleepTime = 0;
            foreach (SleepBlock block in sleeps) totalSleepTime += Program.convertDifferenceToDouble("00:00", block.length);
            tst = Program.convertDoubleToTime(totalSleepTime);
            if (consoleWrite) Console.WriteLine($"  {name}{new string(' ', 20 - name.Length)}  |  {tst}  |  {link}");
        }
        
        /// <summary> Name of schedule </summary>
        internal string name;
        /// <summary> Length of schedule (00:00) </summary>
        internal string tst;
        /// <summary> Hyperlink to Polyphasic.net link for this schedule </summary>
        internal string link;
        /// <summary> List of sleepblocks that the user should sleep in for this schedule </summary>
        internal List<SleepBlock> sleeps;
        /// <summary> Is this schedule recommended for average sleepers? (TST >= 5h?) </summary>
        internal bool recommended;
        /// <summary> Earliest time the dark period could start for this schedule </summary>
        internal string DPEarliestStart;
        /// <summary> Latest time the dark period could end for this schedule </summary>
        internal string DPLatestEnd;

        internal static int maxScheduleNameLength = 19;
        #endregion

        #region utilities
        /// <summary>
        /// Find TST based on sleep block lengths
        /// </summary>
        internal string getTST()
        {
            int sleepTime = 0;

            foreach(SleepBlock s in sleeps)
            {
                sleepTime += Program.convertTimeToInt(Program.getDifferenceTime("00:00", s.length.Trim()));
            }

            return Program.convertDoubleToTime((double)sleepTime / 60);
        }
        #endregion
    }
}
