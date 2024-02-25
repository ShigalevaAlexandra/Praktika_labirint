using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praktika_labirint
{
    internal class Program
    { //Функция считывания карты из макета (файла.txt)
        static char[,] Read_map(string map_file)
        {
            string[] read_map = File.ReadAllLines(map_file);      //считываем карту из файла

            int map_count_i = read_map.Length;                    //кол-во строк
            int map_count_j = read_map[0].ToCharArray().Length;   //кол-во столбцов

            char[,] map = new char[map_count_i, map_count_j];     //создаем карту(массив строк)

            for (int i = 0; i < map_count_i; i++)
            {
                for (int j = 0; j < map_count_j; j++)
                {
                    map[i, j] = read_map[i].ToCharArray()[j];    //получаем карту в виде массива символов
                }
            }

            return map;
        }

        //Функция создания карты (вывода ее на консоль)
        static void Create_map(char[,] map)
        {

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j]);
                }

                Console.WriteLine();
            }
        }

        //Функция определения позиции символа выхода из лабиринта
        static void Get_position_level_end(char[,] map, char level_end,
            out int level_end_x, out int level_end_y)
        {
            level_end_x = 0;
            level_end_y = 0;

            for (int i = 0; i < map.GetLength(0); i++)        //перебираем массив символов на наличие символа выхода из лабиринта
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == level_end)              //при нахождении символа, присваимваем ему координаты
                    {
                        level_end_x = j;
                        level_end_y = i;
                    }
                }
            }
        }

        //Функция определения позиции игрока на карте
        static void Get_user_position(char[,] map, char user, out int user_x, out int user_y)
        {
            user_x = 0;
            user_y = 0;

            for (int i = 0; i < map.GetLength(0); i++)        //перебираем массив символов на наличие символа игрока
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == user)                   //при нахождении символа, присваимваем ему координаты
                    {
                        user_x = j;
                        user_y = i;
                    }
                }
            }
        }

        //Функция создания врага на карте (рандомная позиция)
        static void Create_enemy(char[,] map, char enemy, out int enemy_x, out int enemy_y)
        {
            enemy_x = 0;
            enemy_y = 0;
            int count_enemy = 0;                   //счетчик кол-ва врагов

            int max_i_map = map.GetLength(0);      //ширина карты
            int max_j_map = map.GetLength(1);      //длина карты

            Random random = new Random();

            //цикл для перебора возможных рандомных позиций врага 
            while (count_enemy <= 0) //будет создан одтн враг
            {
                enemy_x = random.Next(1, max_j_map);
                enemy_y = random.Next(1, max_i_map);

                if (map[enemy_y, enemy_x] == '#' || map[enemy_y, enemy_x] == '@' || map[enemy_y, enemy_x] == 'X')
                {
                    enemy_x = random.Next(1, max_j_map);
                    enemy_y = random.Next(1, max_i_map);
                }
                else
                {
                    map[enemy_y, enemy_x] = enemy;
                    count_enemy++;
                }
            }
        }


        //Функция выбора действия игрока
        static void Get_user_command(int user_step, out int user_step_x, out int user_step_y)
        {
            user_step_x = 0;
            user_step_y = 0;

            ConsoleKey user_command = Console.ReadKey().Key;

            switch (user_command)
            {
                case ConsoleKey.UpArrow:               //движение вверх
                    user_step_x = user_step * 0;
                    user_step_y = user_step * -1;
                    break;

                case ConsoleKey.DownArrow:             //движение вниз
                    user_step_x = user_step * 0;
                    user_step_y = user_step * 1;
                    break;

                case ConsoleKey.LeftArrow:             //движение влево
                    user_step_x = user_step * -1;
                    user_step_y = user_step * 0;
                    break;

                case ConsoleKey.RightArrow:            //движение вправо
                    user_step_x = user_step * 1;
                    user_step_y = user_step * 0;
                    break;

                default:                               //бездействие
                    user_step_x = 0;
                    user_step_y = 0;
                    break;

            }
        }

        //Функция обновления карты после движения игрока
        static char[,] Update_map(char[,] map,
            char user, int user_x, int user_y, int user_step_x, int user_step_y, out int user_new_x, out int user_new_y)
        {
            user_new_x = 0;
            user_new_y = 0;

            if (map[user_y + user_step_y, user_x + user_step_x] != '#')
            {
                map[user_y + user_step_y, user_x + user_step_x] = user;         //показываем символ игрока на новой позиции
                map[user_y, user_x] = ' ';

                user_new_x = user_x + user_step_x;                              //получение координаты следующего шага игрока
                user_new_y = user_y + user_step_y;

            }

            return map;
        }

        //Функция для определения причины завершения уровня
        private static string Check_end(char[,] map, int user_new_x, int user_new_y,
            int level_end_x, int level_end_y, int enemy_x, int enemy_y)
        {
            string end = " ";

            //если новая позиция игрока совпала с позицией символа выхода из лабиринта, уровень завершен
            if (user_new_x == level_end_x && user_new_y == level_end_y) end = "Найден выход";

            else if (user_new_x == enemy_x && user_new_y == enemy_y) end = "Захвачен врагом";

            return end;

        }

        //Функция для определения завершен уровень или нет
        private static bool Check_game_end(char[,] map, int user_new_x, int user_new_y,
            int level_end_x, int level_end_y, int enemy_x, int enemy_y)
        {
            bool game_end = false;

            //если новая позиция игрока совпала с позицией символа выхода из лабиринта, уровень завершен
            if (user_new_x == level_end_x && user_new_y == level_end_y) game_end = true;

            if (user_new_x == enemy_x && user_new_y == enemy_y) game_end = true;

            return game_end;

        }

        static void Main(string[] args)
        {
            string map_file = "map.txt";  //название текстового файла с картой лабиринта
            string help_file = "help_map.txt";
            char user = '@';              //вид игрока
            char enemy = '%';             //вид врага
            char level_end = 'X';         //обозначение выхода из лабиринта
            char[,] map;                  //хранение массива символов карты

            int user_step = 1;            //размер шага игрока

            bool game_end = false;        //определение конца игры                 
            string end = " ";                   //определение причины завершения игры

            string title_game = "*!ЛАБИРИНТ!*\r\n";      //название головоломки

            int center_x = (Console.WindowWidth / 2) - (title_game.Length / 2); //центр консоли по горизонтали

            Console.SetCursorPosition(center_x, 1);  //перевод курсора в центр консоли по горизонтали
            Console.WriteLine(title_game);

            Console.SetCursorPosition(0, 2);
            Console.WriteLine(new string('_', Console.WindowWidth));  //рисование линии во всю ширину рабочего окна

            Console.WriteLine(
                "\r\n   Правила игры:\r\n" +
                "      1. Для одержания победы необходимо найти выход из лабиринта\r\n" +
                "         (@ - символ игрока, X - выход из лабиринта)\r\n" +
                "      2. Остерегайтесь врагов. Случайная встреча с врагом приведет к завершению уровня!\r\n" +
                "         (% - символ врага)\r\n" +
                "      3. В случае затруднения прохождения лабиринта, можно использовть подсказу\r\n" +
                "         (Q - будет показан правильный путь из символов '.')\r\n");

            Console.WriteLine(
                "\r\n   Горячие клавиши:\r\n" +
                "      Up, Down, Left, Right - передвижение персонажа по карте\r\n" +
                "      Q - использование подсказки\r\n");

            Console.SetCursorPosition(0, 18);
            Console.WriteLine(new string('_', Console.WindowWidth));

            Console.SetCursorPosition(3, 21);
            Console.WriteLine("Начать игру? (да/нет)");
            Console.SetCursorPosition(3, 22);
            string start_game = Console.ReadLine();  //условие для начала игры

            if (start_game == "да")
            {
                Console.CursorVisible = false;

                map = Read_map(map_file);                             //считываем карту из файла.txt

                Get_position_level_end(map, level_end,                //получение местоположения выхода из лабиринта
                    out int level_end_x, out int level_end_y);

                Create_enemy(map, enemy,
                        out int enemy_x, out int enemy_y);            //создаем врага

                while (game_end == false)
                {
                    Console.Clear();

                    Create_map(map);                                  //рисуем карту

                    Get_user_position(map, user,
                        out int user_x, out int user_y);              //находим игрока
                    Get_user_command(user_step,
                        out int user_step_x, out int user_step_y);    //выбираем направление движения

                    map = Update_map(map, user, user_x, user_y,       //обновляем карту и позицию игрока
                        user_step_x, user_step_y,
                        out int user_new_x, out int user_new_y);

                    end = Check_end(map, user_new_x, user_new_y,
                        level_end_x, level_end_y, enemy_x, enemy_y); //провяряем, игрок нашёл выход или встретил врага

                    game_end = Check_game_end(map, user_new_x, user_new_y,
                        level_end_x, level_end_y, enemy_x, enemy_y); //проверяем, игра окончена или нет

                }

                Console.Clear();

                int end_center_x = (Console.WindowWidth / 2) - (end.Length / 2); //центр консоли по горизонтали

                Console.SetCursorPosition(end_center_x, 1);  //перевод курсора в центр консоли по горизонтали

                if (end == "Найден выход") Console.WriteLine("Вы победили!!!  +100 exp");
                else if (end == "Захвачен врагом")
                {
                    Console.WriteLine("Вы захвачены врагом!!! (((\r\n" +
                        "\r\n Q - показать правильный путь");

                    ConsoleKey help_command = Console.ReadKey().Key;

                    //Использование подсказки
                    if (help_command == ConsoleKey.Q)
                    {
                        string[] read_map = File.ReadAllLines(help_file);     //считываем карту из файла

                        int map_count_i = read_map.Length;                    //кол-во строк
                        int map_count_j = read_map[0].ToCharArray().Length;   //кол-во столбцов

                        char[,] help_map = new char[map_count_i, map_count_j];     //создаем карту(массив строк)

                        for (int i = 0; i < map_count_i; i++)
                        {
                            for (int j = 0; j < map_count_j; j++)
                            {
                                help_map[i, j] = read_map[i].ToCharArray()[j];    //получаем карту в виде массива символов
                            }
                        }

                        Console.SetCursorPosition(0, 7);

                        for (int i = 0; i < help_map.GetLength(0); i++)
                        {
                            for (int j = 0; j < help_map.GetLength(1); j++)
                            {
                                Console.Write(help_map[i, j]);
                            }

                            Console.WriteLine();
                        }
                    }

                }

                Console.ReadKey();

            }
            else
            {
                Console.SetCursorPosition(3, 21);
                Console.WriteLine("Выход из игры..\r\n");
            }
        }
    }
}
