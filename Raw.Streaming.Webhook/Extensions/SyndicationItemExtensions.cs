using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace Raw.Streaming.Webhook.Extensions;
internal static class SyndicationItemExtensions
{
    public static string GetElementExtensionValueByOuterName(this SyndicationItem item, string outerName)
    {
        if (item.ElementExtensions.All(x => x.OuterName != outerName)) 
            return null;

        return item.ElementExtensions.Single(x => x.OuterName == outerName).GetObject<XElement>().Value;
    }
}
