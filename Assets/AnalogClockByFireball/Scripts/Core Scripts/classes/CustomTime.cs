using Assets.AnalogClockByFireball.Scripts.Core_Scripts.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class CustomTime
    {
        private double currentTimeInCycle { get; set; }
        private int hoursInCycle = 12;
        private int minutesInHour = 60;
        private float secondsInMinute = 60;
        private int roundTo = 2;

        private int hours { get; set; }
        private int minutes { get; set; }
        private double seconds { get; set; }
        private double secondsInFullCycle { get; }

        public int Hours { get => hours; }
        public int Minutes { get => minutes; }
        public double Seconds { get => seconds; }
        public double SecondsInFullCycle { get => secondsInFullCycle; }
        public double CurrentTimeInCycle {
            get => currentTimeInCycle;
            set {
                if (value < 0)
                {
                    currentTimeInCycle = Math.Round(secondsInFullCycle - Math.Abs(value), roundTo);
                    return;
                }
                if (value > secondsInFullCycle)
                {
                    currentTimeInCycle = Math.Round(value - secondsInFullCycle, roundTo);
                    return;
                }
                currentTimeInCycle = Math.Round(value, roundTo);
            }
        }
        public int HoursInCycle { get => hoursInCycle; }
        public float SecondsInMinute { get => secondsInMinute; }
        public int MinutesInHour { get => minutesInHour; }

        public CustomTime() : this(12, 60, 60f) { }

        public CustomTime(int hoursInCycle, int minutesInHour, float secondsInMinute, int roundTo = 2)
        {
            this.hoursInCycle = hoursInCycle >= 0 ? hoursInCycle : throw new ArgumentOutOfRangeException("Time: hoursInCycle must be zero or possitive whole number");
            this.minutesInHour = minutesInHour >= 0 ? minutesInHour : throw new ArgumentOutOfRangeException("Time: minutesInHour must be zero or possitive whole number"); ;
            this.secondsInMinute = secondsInMinute > 0 ? secondsInMinute : throw new ArgumentOutOfRangeException("Time: secondsInMinute must be positive whole number.");
            this.roundTo = roundTo;
            this.secondsInFullCycle = returnOneIfIsZero(HoursInCycle) * returnOneIfIsZero(MinutesInHour) * SecondsInMinute;
        }

        public static CustomTime getCopyOf(CustomTime originalCustomTime)
        {
            CustomTime newTime = new CustomTime(
                originalCustomTime.hoursInCycle,
                originalCustomTime.minutesInHour,
                originalCustomTime.secondsInMinute
                );

            newTime.SetTime(originalCustomTime.hours, originalCustomTime.minutes, originalCustomTime.seconds);

            return newTime;
        }

        public double AddTime(float addedTime)
        {
            this.CurrentTimeInCycle += addedTime;

            setStructuredTime();

            return CurrentTimeInCycle;
        }

        public double SetTime(int hours, int minutes, double seconds)
        {
            this.hours = hoursInCycle == 0 ? 0 : hours % hoursInCycle;
            this.minutes = minutesInHour == 0 ? 0 : minutes % MinutesInHour;
            this.seconds = seconds % secondsInMinute;

            this.currentTimeInCycle = StructeredTimeToCycleTime(hours, minutes, seconds, hoursInCycle, MinutesInHour, secondsInMinute);

            return this.currentTimeInCycle;
        }

        public void SetTime(int cycleTime)
        {
            this.currentTimeInCycle = cycleTime % currentTimeInCycle;

            setStructuredTime();
        }

        public static double StructeredTimeToCycleTime(int hours, int minutes, double seconds, int hoursInCycle = 12, int minutesInHour = 60, float secondsInMinute = 60f)
        {
            return (hours * minutesInHour * secondsInMinute 
                + minutes * secondsInMinute 
                + seconds)
                % (returnOneIfIsZero(hoursInCycle) * returnOneIfIsZero(minutesInHour) * secondsInMinute);
        }

        public static StructuredTime CycleTimeToStructuredTime(double cycleTime, int hoursInCycle, int minutesInHour, float secondsInMinute)
        {
            int hours = hoursInCycle == 0 ? 0 : (int)(cycleTime / (minutesInHour * secondsInMinute));
            int minutes = minutesInHour == 0 ? 0 : (int)((cycleTime - (hours * minutesInHour * secondsInMinute)) / secondsInMinute);
            double seconds = minutesInHour == 0 ? cycleTime : (cycleTime - (hours * minutesInHour * secondsInMinute) - (minutes * secondsInMinute));

            return new StructuredTime(hours, minutes, seconds);
        }

        public StructuredTime GetNormalizedStructuredTime(StructuredTime originalTime, int newHoursInCycle, int newMinutesInHour, float newSecondsInMinute)
        {
            double originalTimeInCycleTime = StructeredTimeToCycleTime(
                originalTime.Hours,
                originalTime.Minutes,
                originalTime.Seconds,
                newHoursInCycle,
                newMinutesInHour,
                newSecondsInMinute);

            return CycleTimeToStructuredTime(originalTimeInCycleTime, hoursInCycle, minutesInHour, secondsInMinute);
        }

        private void setStructuredTime()
        {
            StructuredTime sTime = CycleTimeToStructuredTime(currentTimeInCycle, hoursInCycle, minutesInHour, secondsInMinute);

            hours = sTime.Hours;
            minutes = sTime.Minutes;
            seconds = Math.Round(sTime.Seconds, roundTo);
        }

        private static float returnOneIfIsZero(float checkedNumber)
        {
            return checkedNumber == 0 ? 1 : checkedNumber;
        }
    }
}
