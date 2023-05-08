using System.Globalization;

namespace IrisAI
{
    internal class Program
    {

        private static double aArgument;
        private static string trainSetPath, testSetPath;
        private static Dictionary<string, List<double>> preceptrons = new();
        private static CultureInfo culture = CultureInfo.GetCultureInfo("ja-JP");
        private static string projectDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        static void Main(string[] args)
        {
            if (File.Exists(projectDir + "\\CSV Files\\Train-Set.csv") && File.Exists(projectDir + "\\CSV Files\\Test-Set.csv"))
            {
                trainSetPath = projectDir + "\\CSV Files\\Train-Set.csv";
                testSetPath = projectDir + "\\CSV Files\\Test-Set.csv";
            }
            else
            {
                Console.WriteLine("Enter train-set path");
                trainSetPath = Console.ReadLine();
                Console.WriteLine("Enter test-set path");
                testSetPath = Console.ReadLine();
            }

            if (!File.Exists(trainSetPath))
            {
                throw new FileNotFoundException("File in the path: \"" + trainSetPath + "\" dosen't exit");
            }
            if (!File.Exists(testSetPath))
            {
                throw new FileNotFoundException("File in the path: \"" + testSetPath + "\" dosen't exit");
            }

            ReadA();

            TimeOnly timeOnly = TimeOnly.FromDateTime(DateTime.Now);
            Console.WriteLine("Training preceptrons");
            TrainPreceptrons();
            Console.WriteLine("Preceptrons trained in: " + (TimeOnly.FromDateTime(DateTime.Now) - timeOnly));

            Options();
        }

        private static void OwnInput()
        {
            Console.WriteLine("Provide numbers");
            List<double> test = new();
            string line = Console.ReadLine();
            while (true)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                if (double.TryParse(line, culture, out double result))
                {
                    test.Add(result);
                }
                else
                {
                    Console.WriteLine("Wrong argument format");
                }
                line = Console.ReadLine();
            }

            bool classified = false;
            foreach (string key in preceptrons.Keys)
            {
                if (PreceptronCheck(test, preceptrons[key]))
                {
                    classified = true;
                    ListToString(test);
                    Console.WriteLine(" classified as " + key);
                    break;
                }
            }
            if (!classified)
            {
                ListToString(test);
                Console.WriteLine(" haven't been classified");
            }
        }

        //Обучение(создание) прицептронов на основе train-Set
        private static void TrainPreceptrons()
        {
            using StreamReader sr = new(trainSetPath);

            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                //Сплит теста по запятым
                string[] strings = line.Split(',');
                bool isExist = false;

                //Провеерка существует ли прецептрон с таким названием(классом)
                foreach (string key in preceptrons.Keys)
                {
                    if (strings[strings.Length - 1] == key)
                    {
                        isExist = true;
                        break;
                    }
                }

                //Добавление аргументов в лист
                List<double> doubles = new();
                for (int i = 0; i < strings.Length - 1; i++)
                {
                    doubles.Add(double.Parse(strings[i], culture));
                }

                //Создание нового прицептрона если не существует с таким названием
                if (!isExist)
                {
                    doubles.Add(1d);
                    preceptrons.Add(strings[strings.Length - 1], doubles);
                    continue;
                }

                bool called;

                //Тренинг прецептронов
                do
                {
                    called = false;
                    foreach (string key in preceptrons.Keys)
                    {
                        //Тренинг кокнтреных
                        if (key == strings[strings.Length - 1] && !PreceptronCheck(doubles, preceptrons[key]))
                        {
                            called = true;
                            preceptrons[key] = TrainPreceptron(doubles, preceptrons[key], 1);
                        }
                        if (key != strings[strings.Length - 1] && PreceptronCheck(doubles, preceptrons[key]))
                        {
                            called = true;
                            preceptrons[key] = TrainPreceptron(doubles, preceptrons[key], -1);
                        }
                    }
                } while (called);
            }
        }

        //Генерация новых аргументов
        private static List<double> TrainPreceptron(List<double> test, List<double> preceptrons, int plusOrMinus)
        {
            List<double> newDoubles = new();

            for (int i = 0; i < (preceptrons.Count - 1 <= test.Count ? preceptrons.Count - 1 : test.Count); i++)
            {
                newDoubles.Add(preceptrons[i] + (plusOrMinus * aArgument * test[i]));
            }

            newDoubles.Add(preceptrons[preceptrons.Count - 1] + (plusOrMinus * aArgument * (-1)));

            return newDoubles;
        }

        //Классификация test-set
        private static void TestSetClassification()
        {
            int countTrue = 0, countAll = 0, countUnclassified = 0;

            using StreamReader sr = new(testSetPath);
            string line = null;
            while ((line = sr.ReadLine()) != null)
            {
                List<double> test = new();
                foreach (string argument in line.Split(','))
                {
                    if (double.TryParse(argument, culture, out double result))
                    {
                        test.Add(result);
                    }
                }

                bool classified = false;
                foreach (string key in preceptrons.Keys)
                {
                    if (PreceptronCheck(test, preceptrons[key]))
                    {
                        classified = true;
                        if (key == line.Split(',')[line.Split(',').Length - 1])
                        {
                            countTrue++;
                        }
                        Console.WriteLine(line + " classified as " + key);
                        break;
                    }
                }
                if (!classified)
                {
                    Console.WriteLine(line + " haven't been classified");
                    countUnclassified++;
                }
                countAll++;
            }

            Console.WriteLine("Accuracy: " + Math.Round(Convert.ToDouble(countTrue) / Convert.ToDouble(countAll) * 100, 2) + "%\nPreceptrons didn't recognized " + countUnclassified + " flowers");
        }

        //Пррохождение прицептрона
        private static bool PreceptronCheck(List<double> testDoubles, List<double> preceptronDoubles)
        {
            double sum = 0;

            //Подсчёт суммы
            for (int i = 0; i < (preceptronDoubles.Count - 1 <= testDoubles.Count ? preceptronDoubles.Count - 1 : testDoubles.Count); i++)
            {
                sum += preceptronDoubles[i] * testDoubles[i];
            }

            return sum >= preceptronDoubles[preceptronDoubles.Count - 1];
        }

        private static void ListToString(List<double> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Console.Write(list[i]);
                if (i != list.Count - 1)
                {
                    Console.Write(", ");
                }
            }
        }

        private static void Options()
        {
            while (true)
            {
                Console.WriteLine("Choose option:" +
                "\n1. Display a Test-set results" +
                "\n2. Provide your own inputs" +
                "\n3. Exit");
                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:
                            TestSetClassification();
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

        private static void ReadA()
        {
            Console.WriteLine("Give an A argument");
            string aString = Console.ReadLine();
            if (!double.TryParse(aString, culture, out aArgument))
            {
                throw new ArgumentException("Bad A argument");
            }

            if (aArgument > 1 || aArgument <= 0)
            {
                throw new ArgumentException("A argument should be more then 0 and less or equals 1");
            }
        }
    }
}