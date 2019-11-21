using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AnalogClockByFireball.Scripts.Core_Scripts.classes
{
    public class StructuredTime
    {
        public int Hours;
        public int Minutes;
        public double Seconds;

        public StructuredTime() : this(0, 0, 0) {}

        public StructuredTime(int hours, int minutes, double seconds)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }
    }
}
