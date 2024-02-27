using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment
{
    public class SampleData : ISampleData
    {
        public SampleData() { }
        // 1.
        public IEnumerable<string> CsvRows { get; } = File.ReadAllLines("People.csv").Skip(1);
        //skip the first row

        // 2.
        public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        {
            return CsvRows.Select(item => item.Split(',')[6])
                   .OrderBy(state => state)
                   .Distinct();
        }

        // 3.
        public string GetAggregateSortedListOfStatesUsingCsvRows()
            => throw new NotImplementedException();

        // 4.
        public IEnumerable<IPerson> People => throw new NotImplementedException();

        // 5.
        public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
            Predicate<string> filter) => throw new NotImplementedException();

        // 6.
        public string GetAggregateListOfStatesGivenPeopleCollection(
            IEnumerable<IPerson> people) => throw new NotImplementedException();
    }
}