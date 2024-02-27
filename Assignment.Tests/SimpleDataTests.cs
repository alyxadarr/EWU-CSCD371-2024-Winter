using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Assignment.Tests
{
    public class SampleDataTests
    {
        [Fact]
        public void Count_RowsInPeopleCsv_CorrectAmountCounted()
        {
            SampleData dataSample = new SampleData();
            int expectedRows = 50;
            int actualRows = dataSample.CsvRows.Count();
            Assert.Equal(expectedRows, actualRows);
        }
        [Fact]
        public void GetUniqueSortedListOfStatesGivenCsvRows_HardCodedList_SuccessfullySortsUniquely()
        {
            string expectedList = "AL AZ CA DC FL GA IN KS LA MD MN MO NC NE NH NV NY OH OR PA SC TN TX UT VA WA WV";
            SampleData dataSample = new SampleData();
            string actualList = string.Join(" ", dataSample.GetUniqueSortedListOfStatesGivenCsvRows());
            Assert.Equal(expectedList, actualList);
        }
    }

}
