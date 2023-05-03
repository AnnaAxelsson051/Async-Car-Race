using System.Net;
using CarRace;

namespace CarRace
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Car Race 1.0 Press return key to begin");
            Console.ReadKey(); //Väntar på tangenttryckning

            //Skapar bilobjekt och tasks

            Car firstCar = new Car
            {
                id = 1,
                name = "Königsegg",

            };
            Car secondCar = new Car
            {
                id = 2,
                name = "Saab",

            };
            Car thirdCar = new Car
            {
                id = 3,
                name = "Volvo",

            };

     
            var firstCarTask = Race(firstCar);
            var secondCarTask = Race(secondCar);
            var thirdCarTask = Race(thirdCar);

            //Skickar bil-lista till carstatus som lyssnar på en readkey/knapptryck
            //och tar emot det när det kommer
            var statusCarTask = CarStatus(new List<Car> { firstCar, secondCar, thirdCar });

            var carTask = new List<Task> { firstCarTask, secondCarTask, thirdCarTask, statusCarTask };

            //Maintråden som väntar till ngn av dessa tasks är klara
            while (carTask.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(carTask);
                
                if (finishedTask == firstCarTask)
                {
                    Console.WriteLine(firstCar.name + " finish-time is " + firstCar.TimePassed);

                }
                else if (finishedTask == secondCarTask)
                {
                    Console.WriteLine(secondCar.name + " finish-time is " + secondCar.TimePassed);
                }
                else if (finishedTask == thirdCarTask)
                {
                    Console.WriteLine(thirdCar.name + " finish-time is " + thirdCar.TimePassed);
                }
            

                await finishedTask;
                carTask.Remove(finishedTask);
            }
        }

        
        //Varje gång ngn tråd gör en await 
        public static async Task Tick(int tick = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(tick));
        }


        //Startar en tråd, tar in en bil, returnerar en färdigkörd bil
        //Kallar på Events och lägger till penalties, kör while loopen
        //väntar 30 sek, undersöker om det finns en penalty, sen väntas 30 sek igen
        public async static Task<Car> Race(Car car)
        {
                while (true)
                {
                    await Tick(30);  // ?
                    car.TimeLeft = (car.DistanceLeft / (car.Speed / 3.6)) + car.Penalty;
     
                if (car.TimeLeft <= 30)
                    {
                        car.TimePassed += car.TimeLeft;
                        car.DistanceLeft = 0;
                        car.TimeLeft = 0;
                        return car;
                    }
                    car.DistanceLeft -= (30 - car.Penalty) * (car.Speed / 3.6); 

                Events(car);
                    car.TimePassed += 30;
            }
        }

        //Tråd som lyssnar på en readkey, skriver då ut statusen för bilar
        //och hur länge de kört / alt om gått i mål
        public static async Task CarStatus(List<Car> cars)
        {
            while (true)
            {
                await Task.Delay(100); 

                DateTime start = DateTime.Now; 

                bool pressedKey = false;   
               
                while ((DateTime.Now - start).TotalSeconds < 2)
                   
                {
                    if (Console.KeyAvailable) //när en knapp trycks
                    {
                        pressedKey = true;   
                        break;
                    }
                }
                if (pressedKey)
                {
                    Console.ReadKey();   
                    Console.Clear();

                    cars.ForEach(car =>  
                    { 
                            Console.WriteLine(car.name + " is traveling " + car.Speed + "km/h and has "
                                 + car.DistanceLeft + " meters left.");

                    });

                    pressedKey = false;
                }

       

            }
        }



        //Lägger till olika penalties som kan inträffa med viss %uell sannolikhet 
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
            Console.WriteLine(car.name + " is out of gas, and is stopping for 30 sec");
            car.Penalty = 30;
        }
        public static void Puncture(Car car)
        {
            Console.WriteLine(car.name + " is out of gas, and is stopping for 20 sec");
            car.Penalty = 20;
        }
        public static void BirdOnScreen(Car car)
        {
            Console.WriteLine(car.name + " is out of gas, and is stopping for 10 sec");
            car.Penalty = 10;
        }
        public static void EngineFailure(Car car)
        {
            Console.WriteLine(car.name + " has engine failure, and its speed is reduced by 1 km/h");
            car.Speed -= 1;
        }



    
    }
}



