using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessing.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataProcessing
{
    public class LineParser
    {
        const char separationSimbol = ',';
        static readonly string quoteSimbols = "'\"“<";
        static readonly string quoteSimbolsPair = "'\"”>";

        static string CleanFromQuotes(string data)
        {
            int quoteIndex = quoteSimbols.IndexOf(data[0]);
            if (quoteIndex != -1)
            {
                char quoteSimbol = quoteSimbolsPair[quoteIndex];

                if (data.EndsWith(quoteSimbol))
                {
                    return data.Substring(1, data.Length - 2);
                }
                else
                {
                    throw new System.InvalidOperationException("Can't clean quotes.");
                }
            }
            else
            {
                return data;
            }
        }
        static decimal GetNextDecimal(System.Collections.IEnumerator input)
        {
            string data = CleanFromQuotes(GetNextString(input));

            if (data.Length == 0)
            {
                throw new System.InvalidOperationException("Can't get decimal data.");
            };

            return Decimal.Parse(data);

        }
        static long GetNextLong(System.Collections.IEnumerator input)
        {
            string data = CleanFromQuotes(GetNextString(input));

            if (data.Length == 0)
            {
                throw new System.InvalidOperationException("Can't get long data.");
            };

            return long.Parse(data);

        }

        static DateOnly GetNextDate(System.Collections.IEnumerator input)
        {
            string data = CleanFromQuotes(GetNextString(input));

            if (data.Length == 0)
            {
                throw new System.InvalidOperationException("Can't get date.");
            };
            //ReadOnlySpan<char> dataSpan = data;
            //int year = int.Parse(dataSpan.Slice(0, 4));
            //int day = int.Parse(dataSpan.Slice(5, 2));
            //int month = int.Parse(dataSpan.Slice(8, 2));

            //return new DateOnly(year, month, day);
            return DateOnly.ParseExact(data, "yyyy-dd-MM");

        }

        static string GetNextString(System.Collections.IEnumerator input)
        {

            if (!input.MoveNext())
            {
                throw new System.InvalidOperationException("Can't get next field.");
            };

            string data = input.Current.ToString().TrimStart();
            string dataTrimEnd = data.TrimEnd();

            if (data.Length == 0) { return ""; };

            int quoteIndex = quoteSimbols.IndexOf(data[0]);
            if (quoteIndex == -1)
            {
                return dataTrimEnd;
            }

            char quoteSimbol = quoteSimbolsPair[quoteIndex];

            if (dataTrimEnd.EndsWith(quoteSimbol))
            {
                return dataTrimEnd;
            }

            StringBuilder result = new StringBuilder();
            result.Append(data);


            while (input.MoveNext())
            {
                string current = input.Current.ToString();
                result.Append(separationSimbol);
                result.Append(current);
                if (current.TrimEnd().EndsWith(quoteSimbol))
                {
                    break;
                }
            };

            return result.ToString().TrimEnd();

        }

        static public (LineObject?, bool) ParseLine(string line)
        {
            string firstName = "";
            string lastName = "";
            string address = "";
            string city = "";
            decimal payment = 0;
            DateOnly date = DateOnly.MinValue;
            long accountNumber = 0;
            string service = "";

            string[] lineArray = line.Split(separationSimbol);
            System.Collections.IEnumerator listEnumerator = lineArray.GetEnumerator();

            try
            {
                firstName = GetNextString(listEnumerator);
                lastName = GetNextString(listEnumerator);
                address = GetNextString(listEnumerator);
                city = address.Substring(1, address.IndexOf(",") - 1);
                payment = GetNextDecimal(listEnumerator);
                date = GetNextDate(listEnumerator);
                accountNumber = GetNextLong(listEnumerator);
                service = GetNextString(listEnumerator);
            }
            catch (Exception e)
            {
                return (null, true);
            }

            
            return (new LineObject(firstName, lastName, address, city, payment, date, accountNumber, service), false);
        }
    }
}
