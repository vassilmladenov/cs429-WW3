using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameTest
{
    [TestMethod]
    public void TestPlayerRotation()
    {
        Game test = new Game();
        Player one = test.CurrentPlayer;
        test.AdvancePlayer();
        Player two = test.CurrentPlayer;
        test.AdvancePlayer();
        Player three = test.CurrentPlayer;
        Assert.AreNotSame(one, two);
        Assert.AreSame(one, three);
    }
}