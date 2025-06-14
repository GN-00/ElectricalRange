using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectsNow.DataInput
{
    public static class Input
    {

        public static List<Key> ModifierKeys = new()
        {
            Key.Enter, Key.Left, Key.Right, Key.Up, Key.Down, Key.Escape, Key.Back, Key.Delete, Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.Tab, Key.CapsLock
        };

        public static List<Key> Systemkeys = new()
        {
            Key.Enter, Key.Left, Key.Right, Key.Up, Key.Down, Key.Escape, Key.Back, Key.Delete, Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.Tab, Key.CapsLock
        };

        public static List<Key> Numberskeys = new()
        {
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
            Key.NumPad0, Key.NumPad1, Key.NumPad2, Key.NumPad3, Key.NumPad4, Key.NumPad5, Key.NumPad6, Key.NumPad7, Key.NumPad8, Key.NumPad9
        };

        public static void IntOnly(KeyEventArgs e, int digit)
        {
            TextBox textBox = e.OriginalSource as TextBox;

            if (!Numberskeys.Contains(e.Key) && !Systemkeys.Contains(e.Key))
            {
                e.Handled = true;
            }

            if (textBox.Text.Length >= digit && textBox.SelectedText.Length == 0 && !Systemkeys.Contains(e.Key))
            {
                e.Handled = true;
            }
        }


        public static void DoubleOnly(KeyEventArgs e)
        {
            TextBox textBox = e.OriginalSource as TextBox;

            if (textBox.Text.Contains(".") && e.Key == Key.Decimal)
            {
                e.Handled = true;
            }

            if (!textBox.Text.Contains(".") && e.Key == Key.Decimal)
            {
                e.Handled = false;
            }
            else
            {
                if (!Numberskeys.Contains(e.Key) && !Systemkeys.Contains(e.Key))
                {
                    e.Handled = true;
                }
            }
        }

        public static void ArrowsOnly(KeyEventArgs e)
        {
            if (!Systemkeys.Contains(e.Key))
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                e.Handled = true;
            }
        }


        public static string NumberToSRWords(decimal number)
        {
            string result;
            string digit = null;
            string price;

            price = NumberToWords((int)number);

            if (number.ToString("N2").Contains("."))
            {
                digit = number.ToString("N2").Substring(
                        number.ToString("N2").LastIndexOf(".") + 1,
                        number.ToString("N2").Substring(number.ToString("N2").LastIndexOf(".")).Length - 1);
            }

            result = $"Saudi Riyals {price}";

            if (digit != "00")
            {
                result += $" and {digit}/100 Only";
            }
            else
            {
                result += $" Only";
            }

            return result;
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
            {
                return "Zero";
            }

            if (number < 0)
            {
                return "Minus " + NumberToWords(Math.Abs(number));
            }

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                {
                    words += "and ";
                }

                string[] unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                string[] tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                    {
                        words += "-" + unitsMap[number % 10];
                    }
                }
            }

            return words;
        }

        public static string ToArabicNumbers(this string number)
        {
            return number.Replace('0', '٠')
                         .Replace('1', '١')
                         .Replace('2', '٢')
                         .Replace('3', '٣')
                         .Replace('4', '٤')
                         .Replace('5', '٥')
                         .Replace('6', '٦')
                         .Replace('7', '٧')
                         .Replace('8', '٨')
                         .Replace('9', '٩');
        }
    }
}
