using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ArmyTest
{
    [TestMethod]
    public void TestConstruction()
    {
        var a = new Army(new Pos(0, 3), 100);
        Assert.AreEqual(100, a.Health);
        Assert.AreEqual(0, a.Position.X);
        Assert.AreEqual(3, a.Position.Y);
    }
}
