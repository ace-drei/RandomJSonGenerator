using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Globalization;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    //Expect to see the "Address Data" header on the page Test1
    [Test]
    public async Task DisplayAddressDataAsHeader()
    {
        await Page.GotoAsync("https://ace-drei.github.io/RandomJSonGenerator/");

        // Click on the "Addresses" link
        await Page.GetByRole(AriaRole.Link, new() { Name = "Addresses" }).ClickAsync();

        // Assert that the URL contains '/addresses' after clicking the "Addresses" link
        await Expect(Page).ToHaveURLAsync("https://ace-drei.github.io/RandomJSonGenerator/addresses");

        // Await the task returned by QuerySelectorAsync to get the IElementHandle object
        var h3ElementHandle = await Page.QuerySelectorAsync("h3");

        // Now retrieve the text content from the IElementHandle object
        var h3Content = await h3ElementHandle.TextContentAsync();
        Assert.AreEqual("Address Data", h3Content?.Trim());
    }

    //Show contents of the table when clicking on the "Load Address Data" button
    [Test]
    public async Task ShowExpectedContentsWhenClickingLoadData()
    {
        await Page.GotoAsync("https://ace-drei.github.io/RandomJSonGenerator/");

        // Click on the "Addresses" link
        await Page.GetByRole(AriaRole.Link, new() { Name = "Addresses" }).ClickAsync();

        // Assert that the URL contains '/addresses' after clicking the "Addresses" link
        await Expect(Page).ToHaveURLAsync("https://ace-drei.github.io/RandomJSonGenerator/addresses");

        // Click on the "Load Address Data" button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load Address Data" }).ClickAsync();

        // Use a locator to find the table headers and assert their text content
        var headers = Page.Locator("table thead th");
        await Expect(headers.Nth(0)).ToHaveTextAsync("ID");
        await Expect(headers.Nth(1)).ToHaveTextAsync("Street");
        await Expect(headers.Nth(2)).ToHaveTextAsync("City");
        await Expect(headers.Nth(3)).ToHaveTextAsync("Country");
    }

    //Show Male undermale section when loading data to display males 
    [Test]
    public async Task DisplayMaleGenderInPersonsWhenClickingLoadDataAsMaleTab()
    {
        await Page.GotoAsync("https://ace-drei.github.io/RandomJSonGenerator/");

        // Click on the "Persons" link within the navigation area
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new() { Name = "Persons" }).ClickAsync();

        // Click on the "Load Person Data" button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load Person Data" }).ClickAsync();

        // Wait for the table to be populated
        await Page.WaitForSelectorAsync("table tbody tr");

        // Create a locator for the rows in the tbody of the table
        var rows = Page.Locator("table tbody tr");

        // Count the number of rows
        int rowCount = await rows.CountAsync();

        // Iterate through each row and assert that the 'Gender' column is 'male', ignoring case
        for (int i = 0; i < rowCount; i++)
        {
            var genderCellText = await rows.Nth(i).Locator("td:nth-child(7)").TextContentAsync();
            Assert.That(genderCellText?.Trim(), Is.EqualTo("male").IgnoreCase, $"Row {i + 1} does not contain 'male' in the Gender column.");
        }
    }


    [Test]
    public async Task DisplayDOBAsOlderThanStandard2005()
    {
        await Page.GotoAsync("https://ace-drei.github.io/RandomJSonGenerator/");

        // Click on the "Persons" link within the navigation area
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new() { Name = "Persons" }).ClickAsync();

        // Click on the "Load Person Data" button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Load Person Data" }).ClickAsync();

        // Wait for the table to be populated
        await Page.WaitForSelectorAsync("table tbody tr");

        // Create a locator for the rows in the tbody of the table
        var rows = Page.Locator("table tbody tr");

        // Count the number of rows
        int rowCount = await rows.CountAsync();

        // Create a flag to track if at least one row has a birthday older than 2005
        bool hasOlderThan2005 = false;

        // Iterate through each row and check the 'Gender' column and 'Birthday' column
        for (int i = 0; i < rowCount; i++)
        {
            // Check the 'Gender' column
            var genderCellText = await rows.Nth(i).Locator("td:nth-child(7)").TextContentAsync();
            Assert.That(genderCellText?.Trim(), Is.EqualTo("male").IgnoreCase, $"Row {i + 1} does not contain 'male' in the Gender column.");

            // Check the 'Birthday' column
            var birthdayCellText = await rows.Nth(i).Locator("td:nth-child(6)").TextContentAsync();
            Assert.IsNotNull(birthdayCellText, $"Row {i + 1} Birthday column is empty.");

            DateTime birthday;
            bool isValidDate = DateTime.TryParse(birthdayCellText.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out birthday);
            Assert.IsTrue(isValidDate, $"Row {i + 1} Birthday column does not contain a valid date.");

            if (birthday.Year <= 2005)
            {
                hasOlderThan2005 = true;
            }
        }

        // Assert that at least one row has a birthday older than 2005
        Assert.IsTrue(hasOlderThan2005, "No rows found with a birthday older than the year 2005.");
    }
}
