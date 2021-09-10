using System;
using System.Collections.Generic;
using System.Text;

namespace PolyphasicScheduleFinder
{
    class SleepBlock
    {
        #region attributes
        public SleepBlock(string inLength, string ST, string ET, string EST, string LET, string MD, Program.sleepBlockType inType)
        {
            length = inLength;
            startTime = ST;
            endTime = ET;
            earliestStartTime = EST;
            latestEndTime = LET;
            maxDistanceFromPreviousSleepBlock = MD;
            type = inType;
        }

        public SleepBlock(string inLength, string ST, string ET)
        {
            length = inLength;
            startTime = ST;
            endTime = ET;
        }
        
        /// <summary> Length of sleep block </summary>
        internal string length;
        /// <summary> Start time of sleep block (00:00) </summary>
        internal string startTime;
        /// <summary> End time of sleep block (00:00) </summary>
        internal string endTime;
        /// <summary> Earliest that the sleep block can begin (00:00) </summary>
        internal string earliestStartTime;
        /// <summary> Latest that the sleep block can end (00:00) </summary>
        internal string latestEndTime;
        /// <summary> The amount of time that this sleep block can have between its start time and the end time of the previous sleep block (00:00) </summary>
        internal string maxDistanceFromPreviousSleepBlock;
        /// <summary> Type of sleepblock (Nap/Core) </summary>
        internal Program.sleepBlockType type;
        #endregion
    }
}
