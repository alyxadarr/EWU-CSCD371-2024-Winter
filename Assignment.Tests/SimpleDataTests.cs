﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Assignment.Tests
{
    public class SampleDataTests
    {
        // use System.Linq.Enumerable methods Zip, Count, Sort and Contains methods
        // for testing collections
        [Fact]
        public void Count_RowsInPeopleCsv_CorrectAmountCounted()
        {
            SampleData dataSample = new SampleData();
            int expectedRows = 50;
            int actualRows = dataSample.CsvRows.Count();
            Assert.Equal<int>(expectedRows, actualRows);
        }
        [Fact]
        public void GetUniqueSortedListOfStatesGivenCsvRows_HardCodedList_SuccessfullySortsUniquely()
        {
            string expectedList = "AL AZ CA DC FL GA IN KS LA MD MN MO MT NC NE NH NV NY OR PA SC TN TX UT VA WA WV";
            SampleData dataSample = new SampleData();
            string actualList = string.Join(" ", dataSample.GetUniqueSortedListOfStatesGivenCsvRows());
            Assert.Equal(expectedList, actualList);
        }
        [Fact]
        public void GetUniqueSortedListOfStatesGivenCsvRows_NonHardCodedList_SuccessfullySorts()
        {
            SampleData dataSample = new SampleData();

            string expectedList =  string.Join(", ", dataSample.CsvRows.Select(item => item.Split(',')[6])
                   .OrderBy(state => state)
                   .Distinct());
            string actualList = string.Join(", ", dataSample.GetUniqueSortedListOfStatesGivenCsvRows());
            Assert.Equal(expectedList, actualList);
            //could we use IEnumerable sort for this? idk
        }
        [Fact]
        public void GetAggregateSortedListOfStatesUsingCsvRows_HardCodedList_SuccesfullySorts()
        {
            string expectedList = "AL, AZ, CA, DC, FL, GA, IN, KS, LA, MD, MN, MO, MT, NC, NE, NH, NV, NY, OR, PA, SC, TN, TX, UT, VA, WA, WV";
            SampleData dataSample = new SampleData();
            string actualList = string.Join(", ", dataSample.GetAggregateSortedListOfStatesUsingCsvRows());
            Assert.Equal(expectedList, actualList);
        }
        [Fact]
        public void PersonCount_NumOfPeople_ReturnsCorrectNum()
        {
            var expectedCount = 50; 
            SampleData dataSample = new SampleData();

            var actualCount = dataSample.People.Count();

            Assert.Equal(expectedCount, actualCount);
        }



    }

}
