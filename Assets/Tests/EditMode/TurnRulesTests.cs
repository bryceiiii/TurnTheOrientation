using NUnit.Framework;

namespace TurnTheOrientation.Tests
{
    public sealed class TurnRulesTests
    {
        [TestCase(0, 0)]
        [TestCase(90, 90)]
        [TestCase(360, 0)]
        [TestCase(-90, 270)]
        [TestCase(450, 90)]
        public void AngleNormalizationWrapsToQuarterTurns(int input, int expected)
        {
            Assert.AreEqual(expected, ViewMath.NormalizeAngle(input));
        }

        [TestCase(0, PuzzleViewMode.TopDown)]
        [TestCase(90, PuzzleViewMode.SideView)]
        [TestCase(180, PuzzleViewMode.TopDown)]
        [TestCase(270, PuzzleViewMode.SideView)]
        public void ViewModeAlternatesEveryQuarterTurn(int angle, PuzzleViewMode expected)
        {
            Assert.AreEqual(expected, ViewMath.ModeForAngle(angle));
        }

        [Test]
        public void DemoContainsThreePlayableRooms()
        {
            Assert.AreEqual(3, LevelCatalog.All.Count);
            for (int i = 0; i < LevelCatalog.All.Count; i++)
            {
                Assert.Greater(LevelCatalog.All[i].Plates.Count, 0, $"Room {i + 1} needs a socket.");
                Assert.AreEqual(
                    LevelCatalog.All[i].Crates.Count,
                    LevelCatalog.All[i].Plates.Count,
                    $"Room {i + 1} needs one core per socket.");
            }
        }
    }
}
