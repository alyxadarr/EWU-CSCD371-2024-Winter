using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace Assignment.Tests;

public class SampleDataTests 
{
#pragma warning disable CS8618
    public SampleData DataSample { get; set; }
#pragma warning restore CS8618
    //we are going to disable this warning for nullability and testing purposes. We know right below in the setup method that DataSample gets initialized 
    [Fact]
    public void SetUp()
    {
        SampleData DataSample = new SampleData("People.CSV");
    }
    // use System.Linq.Enumerable methods Zip, Count, Sort and Contains methods
    // for testing collections
    [Fact]
    public void Count_RowsInPeopleCsv_CorrectAmountCounted()
    {
        SetUp();       
        int expectedRows = 50;
        int actualRows = DataSample.CsvRows.Count();
        Assert.Equal<int>(expectedRows, actualRows);
    }
    [Fact]
    public void GetUniqueSortedListOfStatesGivenCsvRows_HardCodedList_SuccessfullySortsUniquely()
    {
        string expectedList = "AL AZ CA DC FL GA IN KS LA MD MN MO MT NC NE NH NV NY OR PA SC TN TX UT VA WA WV";
        SetUp();
        string actualList = string.Join(" ", DataSample.GetUniqueSortedListOfStatesGivenCsvRows());
        Assert.Equal(expectedList, actualList);
    }
    [Fact]
    public void GetUniqueSortedListOfStatesGivenCsvRows_NonHardCodedList_SuccessfullySorts()
    {
        SetUp();
        string expectedList =  string.Join(", ", DataSample.CsvRows.Select(item => item.Split(',')[6])
               .OrderBy(state => state)
               .Distinct());
        string actualList = string.Join(", ", DataSample.GetUniqueSortedListOfStatesGivenCsvRows());
        Assert.Equal(expectedList, actualList);
        //could we use IEnumerable sort for this? idk
    }
    [Fact]
    public void GetAggregateSortedListOfStatesUsingCsvRows_HardCodedList_SuccesfullySorts()
    {
        string expectedList = "AL, AZ, CA, DC, FL, GA, IN, KS, LA, MD, MN, MO, MT, NC, NE, NH, NV, NY, OR, PA, SC, TN, TX, UT, VA, WA, WV";
        SetUp();
        string actualList = string.Join(", ", DataSample.GetAggregateSortedListOfStatesUsingCsvRows());
        Assert.Equal(expectedList, actualList);
    }
    [Fact]
    public void PersonCount_NumOfPeople_ReturnsCorrectNum()
    {
        var expectedCount = 50; // Assuming there are 50 people in the CSV rows
        SetUp();

        // Act
        var actualCount = DataSample.People.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }

    [Fact]
    public void FilterByEmail_FiltersCorrectly_Success() {
        SetUp();
        Predicate<string> emailFilter = email => email.EndsWith("@live.com", StringComparison.OrdinalIgnoreCase);

        // Act
        var matchingNames = DataSample.FilterByEmailAddress(emailFilter);

        // Assert
        Assert.Equal(2, matchingNames.Count()); // Assuming 2 matching emails in the sample data
        Assert.Contains(("Stern", "Aucutt"), matchingNames);
        Assert.Contains(("Mayor", "Thurnham"), matchingNames);
        
        // Ensure that other names are not included
        Assert.DoesNotContain(("Charlie", "Brown"), matchingNames);
    }
 


    [Fact]
    public void GetAggregatedListOfStatesGivenPeopleCollection_ReturnsCorrectList_Success()
    {
        //Arrange
        SetUp();
        // string people = sampleData.People;
        string expected = string.Join(", ", DataSample.GetUniqueSortedListOfStatesGivenCsvRows());

        // Act
        string actual = DataSample.GetAggregateListOfStatesGivenPeopleCollection(DataSample.People);

        // Assert
        Assert.Equal(expected, actual);
    }
}


