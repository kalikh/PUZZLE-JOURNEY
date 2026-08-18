using CaravanSecrets.Features.Gameplay;
using NUnit.Framework;

namespace CaravanSecrets.Game.Tests
{
    public sealed class ArabicTextTests
    {
        [Test]
        public void Display_PreservesDigitOrderInLevelLabel()
        {
            var shaped = ArabicText.Display("المرحلة 2/30");
            Assert.That(shaped, Does.Contain("2/30"));
            Assert.That(shaped, Does.Not.Contain("03/2"));
        }

        [Test]
        public void Display_StillShapesArabicLetters()
        {
            const string source = "إيقاف";
            var shaped = ArabicText.Display(source);
            Assert.That(shaped, Is.Not.EqualTo(source));
            Assert.That(shaped.Length, Is.EqualTo(source.Length));
        }
    }
}
