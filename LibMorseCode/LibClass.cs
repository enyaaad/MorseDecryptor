using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace LibMorseCode
{
    public class MorseCode
    {
        //Адреса файлов со словарями
        private const string PATH_EN = @"C:\Users\enyaaad\source\repos\git\Morse Code\EnglishDictionary.txt"; //Константа, хранящая адрес английского словаря
        private const string PATH_RUS = @"C:\Users\enyaaad\source\repos\git\Morse Code\RussianDictionary.txt"; //Константа, хранящая адрес русского словаря
        private const string PATH_TRANSLIT = @"C:\Users\enyaaad\source\repos\git\Morse Code\Translit.txt"; //Константа, хранящая адрес словаря с транслитом

        private string _language; //Язык сообщения
       
        //Словарь алфавита
        private Dictionary<string, char> alphaviteDictionary = new Dictionary<string, char> { };
        //Словарь кода 
        private Dictionary<char, string> codeDictionary = new Dictionary<char, string> { };

        //Делегат для вывода сообщений об ошибках
        public delegate void ErrorMessage(string textOfError);
        public event ErrorMessage NotifyError;
        
        //Конструктор
        public MorseCode (String language)
        {
            _language = language;

            //В зависимости от того, какой язык был введен, создаются словари
            switch (_language)
            {
               case "en":
                        CreateDictionary(PATH_EN);
                    break;
                case "rus":
                        CreateDictionary(PATH_RUS);
                    break;
                case "translit":
                        CreateDictionary(PATH_TRANSLIT);
                    break;
                default:
                        Console.WriteLine("Language isn't correct!");
                    break;
            }
        }

        //Функция создания словаря
        private void CreateDictionary (string path)
        {
            String mesFromFile = null, code = null;
            int index = 2, length;

            using (StreamReader fileStream = File.OpenText(path)) //Создание файлового потока для чтения
            {
                while (fileStream.Peek() != -1) // /// - конец словаря
                {
                    mesFromFile = fileStream.ReadLine();

                    length = mesFromFile.Length;
                         
                    //Разделение ключа и значения
                    while (index < length)
                    {
                        code += mesFromFile[index];
                        index++;
                    }

                    //Запись данных в словарь
                    if (_language != "translit")
                        alphaviteDictionary.Add(code, mesFromFile[0]);

                    codeDictionary.Add(mesFromFile[0], code);

                    index = 2;
                    code = null;
                }
            }
        }

        //Функция проверки коррекстности ввода шифра
        private bool IsCode(char symbol)
        { 
            //Сообщение должно содержать только символы '-', '.' и пробел
            if (symbol == '-' || symbol == '.' || symbol == ' ' || symbol == '\n')
                return true;
            else
                return false;
        }

        //Функция проверки коррекстности ввода сообщения 
        private bool IsLetter(char symbol) 
        {
            //Сообщение должно содержать только буквы латинского или русского алфавита и цифры
            if (_language == "en")
            {
                if ((symbol >= 'A' && symbol <= 'Z') ||
                    (symbol >= 'a' && symbol <= 'z') ||
                    (symbol >= '0' && symbol <= '9') ||
                    (symbol == ' '))
                    return true;
            }

            if (_language == "rus")
            {
                if ((symbol >= 'А' && symbol <= 'Я') ||
                    (symbol >= 'а' && symbol <= 'я') ||
                    (symbol >= '0' && symbol <= '9') ||
                    (symbol == ' '))
                    return true;
            }

            if (_language == "translit")
                if (codeDictionary.ContainsKey(symbol) || symbol == ' ') 
                    return true;
            
            return false;
        }

        //Функция перевода в верхний регистр
        private char CharToUpper(char symbol)
        {
            if ((symbol >= 'a' && symbol <= 'z') || (symbol >= 'а' && symbol <= 'я'))
                return (char)(symbol - ' ');
            else
                return symbol;
        }

        //Функция шифрования сообщения 
        public string Code(char stringLetter)
        {
            stringLetter = CharToUpper(stringLetter); //Перевод символа в верхний регистр

            //Если символ введен корректно, выполняем перевод
            if (IsLetter(stringLetter) == true)
            {
                //Перевод символа в шифр 
                if (codeDictionary.ContainsKey(stringLetter))
                {
                    if (_language != "translit")
                        return codeDictionary[stringLetter] + " ";
                    
                    return codeDictionary[stringLetter];
                }                  
                else
                {
                    if (stringLetter == '\n')
                        return "\n";

                    //Если на вход попал пробел, возвращаем больший отступ между шифрами
                    if (stringLetter == ' ')
                        return "   ";

                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        //Функция расшифровки кода
        public char Decode(string shifr) 
        {
            //Если сообщение введено корректно (только '-' и '.'), выполняем перевод
            if (IsCode(shifr[0]) == true)
            {
                if (shifr[0] == ' ')
                    return ' ';

                if (shifr == "\n")
                    return '\n';

                //Перевод шифра в символ
                if (alphaviteDictionary.ContainsKey(shifr))
                    return alphaviteDictionary[shifr];
                else
                {
                    if (NotifyError != null)
                        NotifyError("Invalid code!");
                    else
                        Console.WriteLine("Invalid code!");

                    return '\0';
                }
            }
            else 
            {
                return '\0';
            }
        }
    }
}