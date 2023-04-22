using System;
namespace CarRace
{
    public class Car
    {
        public int? id { get; set; }

        public string? name { get; set; }

        public double Velocity { get; set; }

        public double RemainingDistance { get; set; }

        //public double Distance { get; set; }

        public double ElapsedTime { get; set; }

        public double Penalty { get; set; }

        public double RemainingTime { get; set; }

   public Car()
        {
            RemainingDistance = 10000;
            //Distance = 0;
            Velocity = 120;
            ElapsedTime = 0;
            Penalty = 0;
            RemainingTime = RemainingDistance;
        }
    }
}


