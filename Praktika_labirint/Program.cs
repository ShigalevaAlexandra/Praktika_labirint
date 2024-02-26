using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace labirint
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string map_file = "map.txt";        //название текстового файла с картой лабиринта
            string help_file = "help_map.txt";   //название текстового файла с указанием выхода из лабиринта

            char[,] map, help_map;   //хранение массива символов карт

            char user = '@',   //символ игрока на карте
                enemy = '%',   //символ врага
                exit = 'X',    //символ выхода из лабиринта
                wall = '#';    //символ стены лабиринта

            int user_step = 1,    //размер шага игрока и врага
                enemy_step = 1,
                user_x = 0,           //начальные позиции игрока на карте
                user_y = 0,
                exit_x = 0,           //позиция выхода из лабиринта
                exit_y = 0,
                enemy_x = 0,          //начальная позиция врага на карте
                enemy_y = 0;

            bool game_end = false;   //определение: закончена игра или нет
            string end = "";         //причина завершения игры

            /*----------------------------------------------------------------------------------------------------*/

            string title_game = ">> ЛАБИРИНТ <<\r\n";   //название головоломки

            int center_x = (Console.WindowWidth / 2) - (title_game.Length / 2);   //центр консоли по горизонтали

            Console.SetCursorPosition(center_x, 1);   //перевод курсора в центр консоли по горизонтали
            Console.WriteLine(title_game);

            Console.SetCursorPosition(0, 2);
            Console.WriteLine(new string('_', Console.WindowWidth));  //рисование линии во всю ширину рабочего окна

            Console.WriteLine(
                "\r\n   * Правила игры:\r\n" +
                "         1. Для одержания победы необходимо найти выход из лабиринта\r\n" +
                "            (@ - символ игрока, X - выход из лабиринта)\r\n" +
                "         2. Остерегайтесь врагов. Случайная встреча с врагом приведет к завершению уровня!\r\n" +
                "            (% - символ врага)\r\n" +
                "         3. В случае затруднения прохождения лабиринта, можно использовть подсказу\r\n" +
                "            (Q - будет показан правильный путь из символов '.')\r\n");

            Console.WriteLine(
                "\r\n   * Горячие клавиши:\r\n" +
                "         Up, Down, Left, Right - передвижение персонажа по карте\r\n" +
                "         Q - использование подсказки\r\n");

            Console.SetCursorPosition(0, 18);
            Console.WriteLine(new string('_', Console.WindowWidth));

            Console.SetCursorPosition(3, 21);
            Console.WriteLine("--> Начать игру? (да/нет)");
            Console.SetCursorPosition(3, 22);

            string start_game = Console.ReadLine();  //условие для начала игры

            if (start_game == "да")
            {
                Console.CursorVisible = false;   //скрываем курсор

                //считываем карту из текстового файла 
                //map.txt

                string[] read_map = File.ReadAllLines(map_file);      //считываем карту из файла

                int map_count_i = read_map.Length;                    //кол-во строк
                int map_count_j = read_map[0].ToCharArray().Length;   //кол-во столбцов

                map = new char[map_count_i, map_count_j];             //создаем карту(массив строк)

                for (int i = 0; i < map_count_i; i++)
                {
                    for (int j = 0; j < map_count_j; j++)
                    {
                        map[i, j] = read_map[i].ToCharArray()[j];    //получаем карту в виде массива символов
                    }
                }

                //hepl_map.txt

                string[] read_help_map = File.ReadAllLines(help_file);      //считываем карту из файла

                int help_map_count_i = read_help_map.Length;                    //кол-во строк
                int help_map_count_j = read_help_map[0].ToCharArray().Length;   //кол-во столбцов

                help_map = new char[help_map_count_i, help_map_count_j];             //создаем карту(массив строк)

                for (int i = 0; i < help_map_count_i; i++)
                {
                    for (int j = 0; j < help_map_count_j; j++)
                    {
                        help_map[i, j] = read_help_map[i].ToCharArray()[j];    //получаем карту в виде массива символов
                    }
                }

                //получаем местонахождение выхода из лабиринта

                for (int i = 0; i < map.GetLength(0); i++)        //перебираем массив символов на наличие символа игрока
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        if (map[i, j] == exit)                   //при нахождении символа, присваимваем ему координаты
                        {
                            exit_x = j;
                            exit_y = i;
                        }
                    }
                }

                //создание врага на карте в рандомном (возможном) месте на карте

                int max_i_map = map.GetLength(0);      //ширина карты
                int max_j_map = map.GetLength(1);      //длина карты

                Random random = new Random();          //создание рандомных позиций врага
                int create_enemy_x = random.Next(1, max_j_map);
                int create_enemy_y = random.Next(1, max_i_map);

                //цикл для перебора возможных рандомных позиций врага 
                while (map[create_enemy_y, create_enemy_x] == user || map[create_enemy_y, create_enemy_x] == exit || map[create_enemy_y, create_enemy_x] == wall)  //будет создан один враг
                {
                    create_enemy_x = random.Next(1, max_j_map);
                    create_enemy_y = random.Next(1, max_i_map);

                }

                map[create_enemy_y, create_enemy_x] = enemy;   //подходящая позиция врага

                //запускаем игру
                while (game_end == false)
                {
                    Console.Clear();

                    //выводим карту(map) на консолль
                    for (int i = 0; i < map.GetLength(0); i++)
                    {
                        for (int j = 0; j < map.GetLength(1); j++)
                        {
                            Console.Write(map[i, j]);
                        }

                        Console.WriteLine();
                    }

                    //получаем местонахождение игрока на карте

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

                    //получаем местонахождение врага на карте

                    for (int i = 0; i < map.GetLength(0); i++)        //перебираем массив символов на наличие символа игрока
                    {
                        for (int j = 0; j < map.GetLength(1); j++)
                        {
                            if (map[i, j] == enemy)                   //при нахождении символа, присваимваем ему координаты
                            {
                                enemy_x = j;
                                enemy_y = i;
                            }
                        }
                    }

                    //варианты команд игрока
                    int user_step_x = 0,          //координаты изменения движения
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

                        case ConsoleKey.Q:  //использование подсказки

                            //выводим карту(help_map) на консолль
                            Console.WriteLine("\r\n\r\n** ПРАВИЛЬНЫЙ ПУТЬ **\r\n");
                            for (int i = 0; i < help_map.GetLength(0); i++)
                            {
                                for (int j = 0; j < help_map.GetLength(1); j++)
                                {
                                    Console.Write(help_map[i, j]);
                                }

                                Console.WriteLine();
                            }

                            Thread.Sleep(1500);
                            break;

                        default:                               //бездействие
                            user_step_x = 0;
                            user_step_y = 0;
                            break;

                    }

                    //варианты команд врага
                    int enemy_step_x = 0,          //координаты изменения движения
                        enemy_step_y = 0;

                    Random command = new Random();          //создание рандомных напревлений движения
                    int enemy_command = command.Next(1, 5);

                    switch (enemy_command)
                    {
                        case 1:                                //движение вверх
                            enemy_step_x = enemy_step * 0;
                            enemy_step_y = enemy_step * -1;
                            break;

                        case 2:                                //движение вниз
                            enemy_step_x = enemy_step * 0;
                            enemy_step_y = enemy_step * 1;
                            break;

                        case 3:                                //движение влево
                            enemy_step_x = enemy_step * -1;
                            user_step_y = enemy_step * 0;
                            break;

                        case 4:                                //движение вправо
                            enemy_step_x = enemy_step * 1;
                            enemy_step_y = enemy_step * 0;
                            break;

                    }

                    //обновление карты после движения игрока и врага
                    int user_new_x = 0, user_new_y = 0,    //следующий шаг игрока и врага
                        enemy_new_x = 0, enemy_new_y = 0;

                    if (map[user_y + user_step_y, user_x + user_step_x] != '#')
                    {
                        map[user_y + user_step_y, user_x + user_step_x] = user;         //показываем символ игрока на новой позиции
                        map[user_y, user_x] = ' ';

                        user_new_x = user_x + user_step_x;                              //получение координаты следующего шага игрока
                        user_new_y = user_y + user_step_y;

                    }

                    if (map[enemy_y + enemy_step_y, enemy_x + enemy_step_x] != '#')
                    {
                        map[enemy_y + enemy_step_y, enemy_x + enemy_step_x] = enemy;         //показываем символ врага на новой позиции
                        map[enemy_y, enemy_x] = ' ';

                        enemy_new_x = enemy_x + enemy_step_x;                              //получение координаты следующего шага врага
                        enemy_new_y = enemy_y + enemy_step_y;

                    }

                    //определение причины завершения игры
                    if (user_new_x == exit_x && user_new_y == exit_y) end = "Найден выход";
                    else if (user_new_x == enemy_x && user_new_y == enemy_y) end = "Захвачен врагом";

                    //определение завершен уровень или нет
                    if (user_new_x == exit_x && user_new_y == exit_y) game_end = true;
                    if (user_new_x == enemy_x && user_new_y == enemy_y) game_end = true;

                }

                Console.Clear();

                int end_center_x = (Console.WindowWidth / 2) - (end.Length / 2); //центр консоли по горизонтали
                Console.SetCursorPosition(end_center_x, 1);  //перевод курсора в центр консоли по горизонтали

                if (end == "Найден выход") Console.WriteLine("Вы победили!!!  +100 exp");
                else if (end == "Захвачен врагом") Console.WriteLine("Вы захвачены врагом!!! (((");

                Console.ReadKey();

            }

            else
            {
                Console.SetCursorPosition(3, 24);
                Console.WriteLine("Выход из игры..\r\n");
            }
        }
    }
}