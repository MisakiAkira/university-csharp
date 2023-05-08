namespace IrisAI
{
    internal class Program
    {

        private static int k;
        private static string? trainSetPath, testSetPath;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Welcome to Iris-AI\nProgramm to classify Iris to a type based on it's parametrs\nBefore starting please provide with k-argument");
            if (!int.TryParse(Console.ReadLine(), out int readedK))
            {
                Console.WriteLine("Please provide an int (* ￣︿￣)");
                Environment.Exit(0);
            }
            Console.WriteLine("Okay, we've got a k, now plsease provide a path to Train-set");
            string? readedTrainSetPath = Console.ReadLine();
            if (!IsFileExist(readedTrainSetPath))
            {
                Environment.Exit(0);
            }

            if (readedK > File.ReadLines(readedTrainSetPath).Count())
            {
                readedK = File.ReadLines(readedTrainSetPath).Count();
                Console.WriteLine($"The 'k' is greater than the number of flowers in Train-set, 'k' is set to flower numbers: {readedK}");
            }
            else if (readedK <= 0)
            {
                readedK = 1;
                Console.WriteLine($"The 'k' can't be lower 1, 'k' is set to: {readedK}");
            }

            Console.WriteLine(@"Last step and we done with providing information ヾ(≧▽≦*)o" + "\nI need a path to a Test-set file ( •̀ ω •́ )✧");
            string? readerTestSetPath = Console.ReadLine();
            if (!IsFileExist(readerTestSetPath))
            {
                Environment.Exit(0);
            }

            trainSetPath = readedTrainSetPath;
            testSetPath = readedTrainSetPath;
            k = readedK;

            Console.WriteLine("Okay, we've done with with providing those silly pathes etc. (～￣▽￣)～\n");
            Options();


        }

        private static void TestSetClassifing()
        {
            using StreamReader trainSetReader = new StreamReader(trainSetPath);
            using StreamReader testSetReader = new StreamReader(testSetPath);

            List<string> trainFlowers = new List<string>();
            string? line;
            while ((line = trainSetReader.ReadLine()) != null)
            {
                trainFlowers.Add(line);
            }

            Dictionary<string, List<double>> classesAndDistance = new Dictionary<string, List<double>>();
            while ((line = testSetReader.ReadLine()) != null)
            {
                string[] testFlowerStrings = line.Split(',');
                try
                {
                    double[] testFlowerDoubles = { double.Parse(testFlowerStrings[0], System.Globalization.CultureInfo.GetCultureInfo("be-BY")), 
                        double.Parse(testFlowerStrings[1], System.Globalization.CultureInfo.GetCultureInfo("be-BY")),
                        double.Parse(testFlowerStrings[2], System.Globalization.CultureInfo.GetCultureInfo("be-BY")), 
                        double.Parse(testFlowerStrings[3], System.Globalization.CultureInfo.GetCultureInfo("be-BY")) };
                    CompareFlowers(testFlowerDoubles);
                }
                catch
                {
                    throw new FormatException("Incorrect format of a file, should be: 'double', 'double', 'double', 'double'");
                }
            }
            IsContinuing();
        }

        private static void OwnInput()
        {
            try
            {
                Console.WriteLine("Please provide 4 double numbers");
                double[] testFlower = { double.Parse(Console.ReadLine()), double.Parse(Console.ReadLine()), double.Parse(Console.ReadLine()), double.Parse(Console.ReadLine()) };
                CompareFlowers(testFlower);
            }
            catch
            {
                throw new FormatException("Wrong format of number");
            }
        }

        private static void CompareFlowers(double[] testFlowerDoubles)
        {
            using StreamReader trainSetReader = new StreamReader(trainSetPath);

            List<string> trainFlowers = new List<string>();
            string? line;
            while ((line = trainSetReader.ReadLine()) != null)
            {
                trainFlowers.Add(line);
            }

            Dictionary<string, List<double>> classesAndDistance = new Dictionary<string, List<double>>();
            foreach (string trainFlower in trainFlowers)
            {
                string[] trainFlowerStrings = trainFlower.Split(',');
                double[] trainFlowerDoubles = { double.Parse(trainFlowerStrings[0]), double.Parse(trainFlowerStrings[1]), double.Parse(trainFlowerStrings[2]), double.Parse(trainFlowerStrings[3]) };
                if (classesAndDistance.ContainsKey(trainFlowerStrings[trainFlowerStrings.Length - 1]))
                {
                    classesAndDistance[trainFlowerStrings[trainFlowerStrings.Length - 1]].Add(CallculateDistance(trainFlowerDoubles, testFlowerDoubles));
                }
                else
                {
                    classesAndDistance.Add(trainFlowerStrings[trainFlowerStrings.Length - 1], new List<double>() { CallculateDistance(trainFlowerDoubles, testFlowerDoubles) });
                }
            }
            Dictionary<string, int> numberOfNeighbouring = new Dictionary<string, int>();
            foreach (string key in classesAndDistance.Keys)
            {
                classesAndDistance[key].Sort();
            }
            Dictionary<string, List<double>> copyOfClassesAndDistance = new Dictionary<string, List<double>>();
            foreach (KeyValuePair<string, List<double>> kvp in classesAndDistance)
            {
                List<double> newList = new List<double>(kvp.Value);
                copyOfClassesAndDistance.Add(kvp.Key, newList);
            }
            foreach (string key in classesAndDistance.Keys)
            {
                numberOfNeighbouring.Add(key, 0);
            }
            for (int i = 0; i < k; i++)
            {
                string? flowerClass = null;
                double flowerDistance = -1;
                foreach (string key in classesAndDistance.Keys)
                {
                    if (classesAndDistance[key][0] < flowerDistance || flowerDistance == -1)
                    {
                        flowerClass = key;
                        flowerDistance = classesAndDistance[key][0];
                    }
                }
                numberOfNeighbouring[flowerClass]++;
                classesAndDistance[flowerClass].RemoveAt(0);
            }
            string? maxNeighbouringKey = null;
            int maxNeighbourings = 0;
            bool isSameNeighbouring = false;
            string? sameNeighbourings = null;
            foreach (string key in numberOfNeighbouring.Keys)
            {
                if (numberOfNeighbouring[key] > maxNeighbourings)
                {
                    maxNeighbourings = numberOfNeighbouring[key];
                    maxNeighbouringKey = key;
                    isSameNeighbouring = false;
                }
                else
                if (numberOfNeighbouring[key] == maxNeighbourings)
                {
                    sameNeighbourings = key;
                    isSameNeighbouring = true;
                }
            }
            if (!isSameNeighbouring)
            {
                Console.WriteLine($"{testFlowerDoubles[0].ToString("F1")}," +
                    $" {testFlowerDoubles[1].ToString("F1")}," +
                    $" {testFlowerDoubles[2].ToString("F1")}," +
                    $" {testFlowerDoubles[3].ToString("F1")} is {maxNeighbouringKey} class");
            }
            else
            {
                if (copyOfClassesAndDistance[maxNeighbouringKey][0] > copyOfClassesAndDistance[sameNeighbourings][0])
                {
                    Console.WriteLine($"{testFlowerDoubles[0].ToString("F1")}," +
                        $" {testFlowerDoubles[1].ToString("F1")}," +
                        $" {testFlowerDoubles[2].ToString("F1")}," +
                        $" {testFlowerDoubles[3].ToString("F1")} is {maxNeighbouringKey} class");
                }
                else
                {
                    Console.WriteLine($"{testFlowerDoubles[0].ToString("F1")}," +
                        $" {testFlowerDoubles[1].ToString("F1")}," +
                        $" {testFlowerDoubles[2].ToString("F1")}," +
                        $" {testFlowerDoubles[3].ToString("F1")} is {sameNeighbourings} class");
                }
            }
        }

        private static double CallculateDistance(double[] trainFlower, double[] testFlower)
        {
            return Math.Pow((trainFlower[0] - testFlower[0]), 2) + Math.Pow((trainFlower[1] - testFlower[1]), 2)
                + Math.Pow((trainFlower[2] - testFlower[2]), 2) + Math.Pow((trainFlower[3] - testFlower[3]), 2);
        }

        private static bool IsFileExist(string? path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("No such file (´。＿。｀)");
                return false;
            }
            return true;
        }

        private static void IsContinuing()
        {
            while (true)
            {
                Console.WriteLine("Do you want to choose another option?\n1. Yes\n2. No");
                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:
                            Options();
                            break;
                        case 2:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid option");
                            break;
                    }
                }
                else { Console.WriteLine("Please, provide number"); }
            }
        }

        private static void Options()
        {
            while (true)
            {
                Console.WriteLine("I want you to choose what option you want to use:" +
                "\n1. Display a Test-set results" +
                "\n2. Provide your own inputs" +
                "\n3. Exit");
                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:
                            TestSetClassifing();
                            break;
                        case 2:
                            OwnInput();
                            break;
                        case 3:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid option");
                            break;
                    }
                }
                else { Console.WriteLine("Please, provide number"); }
            }
        }
    }
}