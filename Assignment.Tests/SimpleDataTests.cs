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
    }

}
