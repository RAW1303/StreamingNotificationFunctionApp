using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Common
{
    public sealed class JsonContent : HttpContent
    {
        private readonly object _content;
        private readonly JsonSerializerOptions _options;

        public JsonContent(object content, JsonSerializerOptions options = default)
        {
            _content = content;
            _options = options;
            Headers.ContentType = new MediaTypeHeaderValue("application/json")
            {
                CharSet = Encoding.UTF8.WebName
            };
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return JsonSerializer.SerializeAsync(stream, _content, _options);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
