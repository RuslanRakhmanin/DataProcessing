using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ReadOnlySpan<char> dataSpan = data;
            int year = int.Parse(dataSpan.Slice(0, 4));
            int day = int.Parse(dataSpan.Slice(5, 2));
            int month = int.Parse(dataSpan.Slice(8, 2));

            return new DateOnly(year, month, day);

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

        static public LineObject ParseLine(string line)
        {
            string[] lineArray = line.Split(separationSimbol);
            System.Collections.IEnumerator listEnumerator = lineArray.GetEnumerator();

            string firstName = GetNextString(listEnumerator);
            string lastName = GetNextString(listEnumerator);
            string address = GetNextString(listEnumerator);
            string city = address.Substring(1, address.IndexOf(",") - 1);
            decimal payment = GetNextDecimal(listEnumerator);
            DateOnly date = GetNextDate(listEnumerator);
            long accountNumber = GetNextLong(listEnumerator);
            string service = GetNextString(listEnumerator);

            
            return new LineObject(firstName, lastName, address, city, payment, date, accountNumber, service);
        }
    }
}
