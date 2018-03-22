using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts
{
    public class DebugTimer
    {
        private float _startTime;
        private string _name;

        public DebugTimer(string name)
        {
            _name = name;
        }

        public void StartTimer()
        {
            _startTime = Time.time;
        }

        public void CheckTimer(int step)
        {
            UberDebug.LogChannel(LogChannels.Performance, string.Format("{0} {1}: {2}", _name, step, Time.time - _startTime));
        }
    }
}