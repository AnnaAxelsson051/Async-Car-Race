using CarRace;

namespace CarRace
{
    public class Program
    {
        // Mostly swiped from https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
        static async Task Main(string[] args)
        {

            Console.WriteLine("Car Race 1.0 Press return key to begin");
            Console.ReadKey(); //Väntar på en knapptryckning

            Car firstCar = new Car
            {
                id = 1,
                name = "Königsegg",

            };
            Car secondCar = new Car
            {
                id = 2,
                name = "Traktor",

            };
            Car thirdCar = new Car
            {
                id = 3,
                name = "Volvo",

            };

          //Det som manipulerar datan körs i en egen tråd
          //I main skapar vi bilobjekt och tasksen
          //Await väntar på ngt

            var firstCarTask = Race(firstCar);
            var secondCarTask = Race(secondCar);
            var thirdCarTask = Race(thirdCar);

            //Skickar bil-lista till carstatus som lyssnar på en readkey o tar emot det
            //När det kommer, bilen är en ögonblicksbild av hur det går för bilen i loppet
            var statusCarTask = CarStatus(new List<Car> { firstCar, secondCar, thirdCar });

            var carTask = new List<Task> { firstCarTask, secondCarTask, thirdCarTask, statusCarTask, finishedCarTask };


            //Maintråden väntar till ngn av dessa tasks är klara
            while (carTask.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(carTask);
                // Console.WriteLine("Task done");
                if (finishedTask == firstCarTask)
                {
                    Console.WriteLine(firstCar.name + " time is " + firstCar.ElapsedTime);

                }
                else if (finishedTask == secondCarTask)
                {
                    Console.WriteLine(secondCar.name + " time is " + secondCar.ElapsedTime);
                }
                else if (finishedTask == thirdCarTask)
                {
                    Console.WriteLine(thirdCar.name + " time is " + thirdCar.ElapsedTime);
                }
                else if (finishedTask == finishedCarTask)
                {
                    Console.WriteLine("Race is finished");
                    //carTask.Remove(statusCarTask);
                }

                await finishedTask;
                carTask.Remove(finishedTask);
            }
        }

        public static async Task Tick(int tick = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(tick));
        }


        /***********/

        //Sen köra uppdateringen på bilen, sen kommer rollen
        //Så plussas 30 sek på elapsed time
        //Omvandlar till m/s
        //Direkt efter ticket räknar den om hur långt det är kvar nu

        //Distance remaining är 10 000 - timeremaining * hastigheten
        //Och då beh man inte ta med penaltyn utan distanderemaining kommer
        //Alltid bara vara en 

        //Calling eventroll to add penalties
        //Sen kör while loopen om då väntar den 30 sek sen
        //Kollar man om det finns en penalty, sen väntas 30 sek igen
        //Och då fnns penaltyn på bilen när den kör andra sträckan o då kommer
        //den med i bilden

        public async static Task<Car> Race(Car car)
        {
                while (true)
                {
                    await Tick();
                    Console.WriteLine("30 seconds of racing has occured");
                    car.TimeToFinish = (car.DistanceLeft / (car.Velocity / 3.6)) + car.Penalty;
                                   //= 10000 (tot sträcka) / 120 (hastighet) / 3.6 + minustiden
                    if (car.TimeToFinish <= 30)
                    {
                    //Har gått i mål?
                        car.ElapsedTime += car.TimeToFinish;
                        car.DistanceLeft = 0;
                        car.TimeToFinish = 0;
                        return car;
                    }

                    //Gör en ny uträkning varje gång på hur mkt man ska ta bort 
                    car.DistanceLeft -= (30 - car.Penalty) * (car.Velocity / 3.6);
                    //Total distans sen minus hur länge bilen kört
                    Events(car);
                    car.ElapsedTime += 30;

                    //60 min på en timme
                    //120 halva min på en timme
                    //120/120
                    //Hinner 1 km på 30 sek
                    /*
                       * Uppdatera äggets temp(bilens sträcka?)
                       * Uppdatera tiden ägget kokat
                       * Äggets temperatur ökar med 1 grad per 10 sek
                       */

                    // 1 C per 10 sekunder
                    // 3 C per 30 sekunder
                    //

                    // 20 C @ 0 S
                    // 23 C @ 30 S
                    // 26 C @ 60 S
                    // Y = k * x + 20
                    // 26 =  k * 60 + 20
                    // k = 0.1
                    // 26 - 20 = k * 60

                    // 6 / 60

                    //egg.egg_temperature = egg.egg_temperature + (0.1M * boilingTime);
                    //lock (egg)
                    //{ 

                    car.distance += (0.1M * racingSpeed);
                    car.egg_time += racingSpeed;

                /*
                 * Är ägget färdigkokt?
                 * Är ÄT => 70C ?
                 */

                if (car.egg_temperature >= car.done_temperature)
                {
                    //Console.WriteLine("Egg has reached temp");
                    // egg is done!
                    return car;
                }
                //}
            }
        }


        /* public static async Task<Car> Race(Car car)
         {
             while (true)
             {
                 await Tick();
                 car.TimeToFinish = (car.DistanceLeft / (car.Velocity / 3.6)) + car.Penalty;
                 if (car.TimeToFinish <= 30)
                 {
                     car.ElapsedTime += car.TimeToFinish;
                     car.DistanceLeft = 0;
                     car.TimeToFinish = 0;
                     return car;
                 }

                 //Gör en ny uträkning varje gång på hur mkt man ska ta bort 
                 car.DistanceLeft -= (30 - car.Penalty) * (car.Velocity / 3.6);
                 //Total distans sen minus hur länge bilen kört
                 Events(car);
                 car.ElapsedTime += 30;
             }
         }*/

        //Tiden är okänd

        //Tråd som lyssnar på en readkey och tar emot det när det kommer
        //Skriver då ut statusen för bilar o hur länge dee kört
        //Går att veta vid varje tidpunkt hur länge det är kvar
        public static async Task CarStatus(List<Car> cars)
        {
            while (true)
            {
                await Task.Delay(100); //Väntar en millisekund

                DateTime start = DateTime.Now; //Starta tidräkning från nu

                bool gotKey = false;   //?
               
                while ((DateTime.Now - start).TotalSeconds < 2)
                    //Om det gått mindre än 2 sek
                {
                    if (Console.KeyAvailable)
                    {
                        gotKey = true;   //?
                        break;
                    }
                }
                if (gotKey)
                {
                    Console.ReadKey();   //Om anv trycker tangent
                    Console.Clear();

                    cars.ForEach(car =>  //Skriva ut status för varje bil
                    {
                        Console.WriteLine(car.name + " is traveling " + car.Velocity + "km/h and has "
                                 + car.DistanceLeft + " meters left.");
                    });

                    gotKey = false;
                }

                /*
                //Checks if race is over
                var totalRemaining = cars.Select(car => car.DistanceLeft).Sum();
                Console.WriteLine("TR: " + totalRemaining);

                if (totalRemaining == 0)
                {
                    Console.WriteLine("Race over" + totalRemaining);
                    return;
                }*/

            }
        }


        //Checks if car has finished
        public static async Task CarFinished(List<Car> cars)
        {
            while (true)
            {
                var totalRemaining = cars.Select(car => car.TimeToFinish).Sum();

                if (totalRemaining == 0)
                {
                    return;
                }
            }
        }

        //Kör ett tick först
        //I början av loppet kör alla bilar full hastighet i 30 sek
       
        
        //

        //Is adding penalties
        public static void Events(Car car)
        {
            Random random = new Random();
            int roll = random.Next(1, 101);
            if (roll <= 2)
                OutOfGas(car);
            else if (roll > 2 && roll < 7)
                Puncture(car);
            else if (roll > 6 && roll < 17)
                BirdOnScreen(car);
            else if (roll > 16 && roll < 37)
                EngineFailure(car);
            else
                car.Penalty = 0;
        }

        public static void OutOfGas(Car car)
        {
            Console.WriteLine(car.name + " is out of gas, 30 sec stop");
            car.Penalty = 30;
        }
        public static void Puncture(Car car)
        {
            Console.WriteLine(car.name + " is out of gas, 30 sec stop");
            car.Penalty = 20;
        }
        public static void BirdOnScreen(Car car)
        {
            Console.WriteLine(car.name + " is out of gas, 30 sec stop");
            car.Penalty = 10;
        }
        public static void EngineFailure(Car car)
        {
            Console.WriteLine(car.name + " has engine failure, speed reduced by 1 km/h");
            car.Velocity -= 1;
        }



    
    }
}






            /********/
/*
            public async static Task CarStatus(List<Car> eggs)
    {
        // Skriv ut status på alla ägg, dvs temperatur och hur länge de kokat


        while (true)
        {

            DateTime start = DateTime.Now;

            bool gotKey = false;

            while ((DateTime.Now - start).TotalSeconds < 2)
            {
                if (Console.KeyAvailable)
                {
                    gotKey = true;
                    break;
                }
            }

            if (gotKey)
            {
                Console.ReadKey();
                Console.Clear();
                eggs.ForEach(egg =>
                {

                    Console.WriteLine($"{egg.RemainingTime()} seconds remaining");
                    Console.WriteLine($"{egg.name} has been boiling for {egg.egg_time} and has a temperature of {egg.egg_temperature}");
                });
                gotKey = false;
            }*/


/*
    // Måste delaya här för att övriga tasks ska kunna gå klart.
    await Task.Delay(10);

    // När alla egg's remaining time är noll, avsluta simuleringen

    var totalRemaining = eggs.Select(egg => egg.RemainingTime()).Sum();

    //var totalRemaining = (from egg in eggs
    //                     let remaining = egg.RemainingTime()
    //                     select remaining).Sum();
    if (totalRemaining == 0)
    {
        return;
    }

}*/


/*
    }
    public async static Task<Car> Race(Car egg)
    {
        int boilingTime = 10;
        while (true)
        {
            await Wait(boilingTime);
            //Console.WriteLine("10 seconds of boiling has occured");
            // vad ska göras här och hur ska det göras?
            /*
             * Uppdatera äggets temp
             * Uppdatera tiden ägget kokat
             * Äggets temperatur ökar med 1 grad per 10 sek
             */

// 1 C per 10 sekunder
// 3 C per 30 sekunder
//

// 20 C @ 0 S
// 23 C @ 30 S
// 26 C @ 60 S
// Y = k * x + 20
// 26 =  k * 60 + 20
// k = 0.1
// 26 - 20 = k * 60

// 6 / 60

//egg.egg_temperature = egg.egg_temperature + (0.1M * boilingTime);
//lock (egg)
//{ */
/*
egg.egg_temperature += (0.1M * boilingTime);
egg.egg_time += boilingTime;*/

/*
 * Är ägget färdigkokt?
 * Är ÄT => 70C ?
 */
/*
            if (egg.egg_temperature >= egg.done_temperature)
            {
         
                return egg;
            }
            //}
        }
    }

    public async static void SomeMethod()
    {
        Console.WriteLine("Some Method Started......");
        await Wait(5);
        Console.WriteLine("Some Method End");
    }
    public async static Task Wait(int delay = 1)
    {
        await Task.Delay(TimeSpan.FromSeconds(delay / 10));
        
    }

    public static void PrintEgg(Car egg)
    {
        Console.WriteLine($"{egg.name} has an inner temperature of {egg.egg_temperature} and has been boiling for {egg.egg_time} seconds");
    }

}

}*/


