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

            //Skapa bilobjekt och tasks

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
            //o tar emot det när det kommer, bilen är en ögonblicksbild av hur
            //det går för bilen i loppet
            var statusCarTask = CarStatus(new List<Car> { firstCar, secondCar, thirdCar });

            var carTask = new List<Task> { firstCarTask, secondCarTask, thirdCarTask, statusCarTask };

            //Maintråden väntar till ngn av dessa tasks är klara
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
  

        //Kallar på eventroll och lägger till penalties
        //Kör while loopen väntar 30 sek
        //Kollar om det finns en penalty, sen väntas 30 sek igen
        //Då fnns penaltyn på bilen när den kör andra sträckan o kommer då
        //med i bilden
        //Startar en tråd, tar in en bil 
        //När den är klar sen returneras en färdigkörd bil
        public async static Task<Car> Race(Car car)
        {
                while (true)
                {
                    await Tick(30);  // ?
                    car.TimeLeft = (car.DistanceLeft / (car.Speed / 3.6)) + car.Penalty;
                //= 10000 (tot sträcka) / 120 (hastighet) / 3.6 + minustiden
                //3.6 tror jag är 60 * 60(dvs 60 sek på 60 minuter) vilket
                //är 3600, delat på tusen.Det har med m/ s <> km / h att göra
                if (car.TimeLeft <= 30)
                    {
                    //Om tid kvar är mindre än 30 sek = dvs har gått i mål
                        car.TimePassed += car.TimeLeft;
                        car.DistanceLeft = 0;
                        car.TimeLeft = 0;
                        return car;
                    }
                    car.DistanceLeft -= (30 - car.Penalty) * (car.Speed / 3.6);
                //Total distans sen minus hur länge bilen kört
                //Gör en ny uträkning varje gång på hur mkt man ska ta bort 

                Events(car);
                    car.TimePassed += 30;
                //Var 30 sek slumpas ny händelse fram för bilarna
            }
        }

        //Tråd som lyssnar på en readkey och tar emot det när det kommer
        //Skriver då ut statusen för bilar o hur länge dee kört eller om de gått i mål
        public static async Task CarStatus(List<Car> cars)
        {
            while (true)
            {
                await Task.Delay(100); //Väntar en millisekund

                DateTime start = DateTime.Now; //Starta tidräkning från nu

                bool pressedKey = false;   
               
                while ((DateTime.Now - start).TotalSeconds < 2)
                    //Om det gått mindre än 2 sek
                {
                    if (Console.KeyAvailable) //blir true när man trycker på en knapp
                    {
                        pressedKey = true;   
                        break;
                    }
                }
                if (pressedKey)
                {
                    Console.ReadKey();   //Om anv trycker tangent
                    Console.Clear();

                    cars.ForEach(car =>  //Skriva ut status för varje bil
                    { 
                            Console.WriteLine(car.name + " is traveling " + car.Speed + "km/h and has "
                                 + car.DistanceLeft + " meters left.");

                    });

                    pressedKey = false;
                }

       

            }
        }


        /*
        //Kollar om bil är klar
        public static async Task CarFinished(List<Car> cars)
        {
            while (true)
            {
                //Linkq method syntax: givet en lista av objekt kör ngt på varje obj
                //Kör TimeToFinish metod, som blir en ny lista med alla cars remaining time
                var totalRemaining = cars.Select(car => car.TimeLeft).Sum();
                //För att veta när alla bilar gått i mål o avsluta simuleringen
                if (totalRemaining == 0)
                {
                    return;
                }
            }
        }*/

        //Lägger till penalties
        //Med hjälp av ett slumptal mellan 1-100 och med viss procentuell
        //sannolikhet för varje händelse enligt specen
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



