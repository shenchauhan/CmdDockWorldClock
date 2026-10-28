namespace CmdDockWorldClock.Tests;

[TestClass]
public sealed class CityCatalogTests
{
    private static readonly string[] ExpectedCityOrder = ["Tokyo", "Seattle", "London"];

    [TestMethod]
    public void ResolvePreservesSelectedOrder()
    {
        var selected = CityCatalog.Resolve(["tokyo", "seattle", "london"]);

        CollectionAssert.AreEqual(
            ExpectedCityOrder,
            selected.Select(city => city.City).ToArray());
    }

    [TestMethod]
    public void ResolveOmitsNoneUnknownAndDuplicateValues()
    {
        var selected = CityCatalog.Resolve(["none", "london", "unknown", "LONDON", null]);

        Assert.HasCount(1, selected);
        Assert.AreEqual("London", selected[0].City);
    }

    [TestMethod]
    public void ResolveLimitsSelectionToFiveCities()
    {
        var selected = CityCatalog.Resolve(CityCatalog.All.Select(city => city.Id));

        Assert.HasCount(CityCatalog.MaximumSelectedCities, selected);
    }

    [TestMethod]
    public void EveryCatalogEntryHasUniqueIdAndValidWindowsTimeZone()
    {
        Assert.HasCount(
            CityCatalog.All.Count,
            CityCatalog.All.Select(city => city.Id).Distinct(StringComparer.OrdinalIgnoreCase));

        foreach (var city in CityCatalog.All)
        {
            Assert.IsNotNull(
                TimeZoneInfo.FindSystemTimeZoneById(city.TimeZoneId),
                $"{city.City} has an invalid time-zone ID.");
        }
    }
}
