using System.Net;
using CarRace;

//[ ] Varje bil ska vara ett objekt
//[ ] Varje bil ska ha ett namn
//[ ] Vi ska ha minst två bilar i denna tävling
//[ ] Tävlingen går ut på att bilarna ska köra en sträcka på exempelvis 10 km
// [ ] Alla bilar ska börja på 0 km
//[ ] Alla bilar har en hastighet i grunden på exempelvis 120 km/h. Ingen bil är långsammare eller snabbare än de andra från början.
//[ ] Bilarna behöver inte accelerera. De kommer direkt upp i sin hastighet.
// [ ] Varje Bil-objekt ska köra i en egen tråd
//[ ] Det ska finnas några slumpmässiga händelser som kan inträffa för en bil.
//[ ] Händelserna listas nedan och där ser du vad som kan hända, hur troligt det är att det händer samt vad som händer. Du får hitta på fler eller andra händelser om du vill.
// [ ] Var 30e sekund ska det för varje bil slumpas fram en händelse. Bara en händelse kan inträffa per gång.
// [ ] Alla bilar ska starta samtidigt
// [ ] Skriva ut i konsolen när bilarna startar
// [ ] Skriv ut i konsolen när en bil får ett problem. Skriv vilken bil och vilket problem.
//TODO [ ] Skriv ut när en bil kommer i mål. Om det är den första ska det också
//skrivas ut att den vann!
//TODO [ ] Användaren ska kunna välja att få läget i tävlingen utskrivet på kommando.
//Exempelvis att användaren kan tycka på enter eller skriva in typ “status” som en
//input. När detta görs ska alla bilar och hur långt de kommit skrivas ut samt deras
//hastighet.

namespace CarRace
{
    public class Program
    {
        // Mostly swiped from https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
        static async Task Main(string[] args)
        {

            Console.WriteLine("Car Race 1.0 Press return key to begin");
            Console.ReadKey(); //Väntar på en knapptryckning

            //Skapa bilobjekt och tasks
            //Det som manipulerar datan körs i en egen tråd

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

            //var carTask = new List<Task> { firstCarTask, secondCarTask, thirdCarTask, statusCarTask, finishedCarTask };
            var carTask = new List<Task> { firstCarTask, secondCarTask, thirdCarTask, statusCarTask };

            //Maintråden väntar till ngn av dessa tasks är klara
            while (carTask.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(carTask);
                // Console.WriteLine("Task done");
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
                /*
                else if (finishedTask == finishedTask)
                {
                    Console.WriteLine("Race is finished");
                    //carTask.Remove(statusCarTask);
                }*/

                await finishedTask;
                carTask.Remove(finishedTask);
            }
        }
        
        //Varje gång ngn tråd gör en await skriver den ut en punkt?
        public static async Task Tick(int tick = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(tick));
        }
        /*
        public async static Task Wait(int delay = 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay / 10));
            //Console.Write($".");
        }*/


        /***********/
        //01.46 i Egg boiler live code pratar han om race metoden
        //40?

        //Det tar 5 min dvs 300 sek för en bil att köra i mål utan penalties

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

        //Startar en tråd, tar in en bil vi skapat
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
                    //Om tid kvar är mindre än 30 sek = dvs har gått i mål?
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

        //När time temaining är 0 för alla bilar har de gått i mål
        //Går att veta vid varje tidpunkt hur länge det är kvar
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

                        //Console.WriteLine(car.name + " has " + car.RemainingTime() + " seconds remaining");
                        //Console.WriteLine(car.name + " has been traveling for {egg.egg_time} and has a temperature of {egg.egg_temperature}");

                    });

                    pressedKey = false;
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
                //Linkq method syntax: givet en lista av objekt kör ngt på varje obj
                //Kör TimeToFinish metod, som blir en ny lista med alla cars remaining time
                var totalRemaining = cars.Select(car => car.TimeLeft).Sum();
                //För att veta när alla bilar gått i mål o avsluta simuleringen
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
            car.Speed -= 1;
        }



    
    }
}



