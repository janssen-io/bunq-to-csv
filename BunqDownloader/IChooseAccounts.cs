using System;
using System.Collections.Generic;
using System.Linq;
using Bunq.Sdk.Model.Generated.Endpoint;
using CsvHelper;

namespace BunqDownloader
{
    public interface IChooseAccounts
    {
        /// <summary>
        /// Returns at least 1 account from the given array of accounts.
        /// </summary>
        /// <exception cref="InvalidOperationException">When accounts is an empty array.</exception>
        /// <exception cref="FormatException">When the provided choice is not formatted correctly.</exception>
        /// <exception cref="IndexOutOfRangeException">When the provided choice is not part of the array.</exception>
        /// <param name="accounts"></param>
        /// <returns></returns>
        IEnumerable<MonetaryAccountBank> Choose(MonetaryAccountBank[] accounts);
    }

    public class ConsoleAccountChooser : IChooseAccounts
    {
        public IEnumerable<MonetaryAccountBank> Choose(MonetaryAccountBank[] accounts)
        {
            if (accounts.Length == 0)
            {
                throw new InvalidOperationException("No accounts to choose from");
            }
            Console.WriteLine("Choose one or more of the following accounts (comma separated):");
            int i = 0;
            var orderedAccounts = new MonetaryAccountBank[accounts.Length];
            var accountsByName = accounts.GroupBy(a => a.DisplayName);
            foreach (var accountGroup in accountsByName)
            {
                Console.WriteLine();
                Console.WriteLine(accountGroup.Key);
                foreach (var account in accountGroup)
                {
                    Console.WriteLine($"    [{i}] {account.Description}");
                    orderedAccounts[i] = account;
                    i++;
                }
            }
            Console.Write("> ");
            string? choice = Console.ReadLine();

            if (choice is null)
            {
                throw new FormatException("User input invalid.");
            }

            int[] choices = Parse(choice);
            if (choices.Length == 0)
            {
                Console.WriteLine("No choice was made. Please select an account.");
                Choose(accounts);
            }

            return choices.Select(i => orderedAccounts[i]);
        }

        private static int[] Parse(string choiceInput)
        {
            var parts = choiceInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Select(int.Parse).ToArray();
        }
    }
}