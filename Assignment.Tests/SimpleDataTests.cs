
using Xunit;

namespace Assignment.Tests;

public class SampleDataTests 
{
#pragma warning disable CS8618
    public SampleData DataSample { get; private set; }
    // we are going to disable the nullable warning for initialization and testing purposes.
    // We know right below in the setup method that DataSample gets initialized so we can safely assume this
    public SampleDataTests()
    {
         DataSample = new SampleData("People.csv");
    }
#pragma warning restore CS8618

    // use System.Linq.Enumerable methods Zip, Count, Sort and Contains methods
    // for testing collections
    [Fact]
    public void Count_RowsInPeopleCsv_CorrectAmountCounted()
    {
        int expectedRows = 50;
        int actualRows = DataSample.CsvRows.Count();
        Assert.Equal<int>(expectedRows, actualRows);
    }
    [Fact]
    public void GetUniqueSortedListOfStatesGivenCsvRows_HardCodedList_SuccessfullySortsUniquely()
    {
        string expectedList = "AL AZ CA DC FL GA IN KS LA MD MN MO MT NC NE NH NV NY OR PA SC TN TX UT VA WA WV";
        string actualList = string.Join(" ", DataSample.GetUniqueSortedListOfStatesGivenCsvRows());
        Assert.Equal(expectedList, actualList);
    }
    [Fact]
    public void GetUniqueSortedListOfStatesGivenCsvRows_NonHardCodedList_SuccessfullySorts()
    {
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
        string actualList = string.Join(", ", DataSample.GetAggregateSortedListOfStatesUsingCsvRows());
        Assert.Equal(expectedList, actualList);
    }
    [Fact]
    public void PeopleCount_NumOfPeople_ReturnsCorrectNum()
    {
        var expectedCount = 50; // 50 people in the CSV rows

        // Act
        var actualCount = DataSample.People.Count();

        // Assert
        Assert.Equal(expectedCount, actualCount);
    }
    [Fact]
    public void People_CSVRows_SortedCorrectly()
    {
        List<IPerson> people = DataSample.People.ToList();

        Assert.Equal("AL", people[0].Address.State);
        Assert.Equal("Mobile", people[0].Address.City);
        Assert.Equal("37308", people[0].Address.Zip);
    }
        [Fact]
        public void People_CSVRows_SortedCorrectly2()
        {
         List<IPerson> people = DataSample.People.ToList();


        Assert.Equal("AZ", people[1].Address.State);
        Assert.Equal("Tucson", people[1].Address.City);
        Assert.Equal("94123", people[1].Address.Zip);
    }

    [Fact]
    public void FilterByEmail_FiltersByName_Success() {
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
    public void FilterByEmailAddress_FilterByEmail_ReturnsCorrectly()
    {
        var expectedFilteredEmails = new List<(string, string)> { ("Darline","Brauner"), ("Melisa","Kerslake") };
        Assert.Equal<int>(expectedFilteredEmails.Count, expectedFilteredEmails.Intersect(DataSample.FilterByEmailAddress((email) => email.Contains(".ne.jp"))).Count());
    }
    [Fact]
    public void GetAggregatedListOfStatesGivenPeopleCollection_ReturnsCorrectList_Success()
    {
        //Arrange
        string expected = string.Join(", ", DataSample.GetUniqueSortedListOfStatesGivenCsvRows());

        // Act
        string actual = DataSample.GetAggregateListOfStatesGivenPeopleCollection(DataSample.People);

        // Assert
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void GetAggregateListOfStatesGivenPeopleCollection_NullArgument_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => DataSample.GetAggregateListOfStatesGivenPeopleCollection(null!));
    }
}

