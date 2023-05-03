using System;
namespace CarRace
{
    public class Car
    {
        public int? id { get; set; }

        public string? name { get; set; }

        public double Speed { get; set; }

        public double DistanceLeft { get; set; }

        public double TimePassed { get; set; }

        public double Penalty { get; set; }

        public double TimeLeft { get; set; }

   public Car()
        {
            DistanceLeft = 10000;
            Speed = 120;
            TimePassed = 0;
            Penalty = 0;
            TimeLeft = DistanceLeft;
        }

       

    }
}


