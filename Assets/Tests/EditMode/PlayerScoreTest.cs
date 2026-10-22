using NUnit.Framework;

public class PlayerScoreTest
{
    [Test]
    public void TestPlayerScoreIncrement()
    {
        // محاكاة نظام النقاط البسيط في اللعبة
        int currentScore = 0;
        int pointsToAdd = 100;

        currentScore += pointsToAdd;

        // التحقق من أن النقاط تم تحديثها بشكل صحيح
        Assert.AreEqual(100, currentScore, "Player score should be 100 after collecting items.");
    }
}