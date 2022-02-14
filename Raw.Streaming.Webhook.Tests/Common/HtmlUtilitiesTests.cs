using NUnit.Framework;

namespace Raw.Streaming.Webhook.Common.Tests
{
    [TestFixture]
    public class HtmlUtilitiesTests
    {
        [TestCase("Commentary of the ACCSS Championship Series Rookie and Silver splits\u003cbr\u003eWatch live over at \u003ca href=\"https://www.twitch.tv/accsimseries\" id=\"ow397\" __is_owner=\"true\"\u003ehttps://www.twitch.tv/accsimseries\u003c/a\u003e", "Commentary of the ACCSS Championship Series Rookie and Silver splits\r\nWatch live over at https://www.twitch.tv/accsimseries")]
        [TestCase("I'm racing in the Bronze split for the Season 8 of the ACCSS Championship Series. Tonight is round 2 at Kyalami.\u003cbr\u003eWatch my perspective and the main commentary stream side-by-side at \u003ca href=\"https://multistre.am/accsimseries/royweller/layout4/\"\u003ehttps://multistre.am/accsimseries/royweller/layout4/\u003c/a\u003e", "I'm racing in the Bronze split for the Season 8 of the ACCSS Championship Series. Tonight is round 2 at Kyalami.\r\nWatch my perspective and the main commentary stream side-by-side at https://multistre.am/accsimseries/royweller/layout4/")]
        public void ConverHtmlToText_WhenValidHtml_ReturnsCorrectString(string input, string expected)
        {
            var actual = HtmlUtilities.ConverHtmlToText(input);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
