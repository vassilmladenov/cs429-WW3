using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameTest
{
    [TestMethod]
    public void TestPlayerRotation()
    {
        Game test = new Game();
        Player one = test.GetCurrentPlayer();
        test.AdvancePlayer();
        Player two = test.GetCurrentPlayer();
        test.AdvancePlayer();
        Player three = test.GetCurrentPlayer();
        Assert.AreNotSame(one, two);
        Assert.AreSame(one, three);
    }
}