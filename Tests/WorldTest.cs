using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class WorldTest
{
    [TestMethod]
    public void TestParseWorldCSV()
    {
        World world = new World("maps.csv");
        Province prov = world.GetProvinceAt(new Pos(256, 128));
        Assert.IsNotNull(prov.City);
        Assert.AreEqual("Oymyakon", prov.City.Name);
        Assert.AreEqual(486, prov.City.Points);
    }
}