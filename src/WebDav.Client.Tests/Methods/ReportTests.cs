using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebDav.Client.Tests.TestDoubles;
using Xunit;

namespace WebDav.Client.Tests.Methods
{
    public class ReportTests
    {
        [Fact]
        public async Task When_RequestIsSuccessfull_Should_ReturnStatusCode200()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            var response = await client.Report("http://example.com/", new ReportParameters());

            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task When_RequestIsFailed_Should_ReturnStatusCode500()
        {
            var dispatcher = Dispatcher.MockFaulted();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            var response = await client.Report("http://example.com/", new ReportParameters());

            Assert.Equal(500, response.StatusCode);
        }

        [Fact]
        public async Task When_PassingRequestBody_Should_SendIt()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);

            var requestBody = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("{DAV:}version-tree",
                    new XAttribute(XNamespace.Xmlns + "D", "DAV:"),
                    new XElement("{DAV:}prop",
                        new XElement("{DAV:}version-name")
                    )
                )
            );

            await client.Report("http://example.com/", new ReportParameters
            {
                RequestBody = requestBody
            });

            const string expectedContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<D:version-tree xmlns:D=""DAV:"">
  <D:prop>
    <D:version-name />
  </D:prop>
</D:version-tree>";
            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Report,
                Arg.Is(Predicates.CompareRequestContent(expectedContent)),
                CancellationToken.None
            );
        }

        [Fact]
        public async Task When_PassingApplyTo_Should_SetDepthHeader()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            await client.Report("http://example.com/", new ReportParameters
            {
                ApplyTo = ApplyTo.Report.ResourceOnly
            });

            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Report,
                Arg.Is(Predicates.CompareHeader("Depth", "0")),
                CancellationToken.None
            );
        }

        [Fact]
        public async Task When_PassingApplyToResourceAndChildren_Should_SetDepthHeader()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            await client.Report("http://example.com/", new ReportParameters
            {
                ApplyTo = ApplyTo.Report.ResourceAndChildren
            });

            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Report,
                Arg.Is(Predicates.CompareHeader("Depth", "1")),
                CancellationToken.None
            );
        }

        [Fact]
        public async Task When_PassingApplyToResourceAndAncestors_Should_SetDepthHeader()
        {
            var dispatcher = Dispatcher.Mock();
            var client = new WebDavClient().SetWebDavDispatcher(dispatcher);
            await client.Report("http://example.com/", new ReportParameters
            {
                ApplyTo = ApplyTo.Report.ResourceAndAncestors
            });

            await dispatcher.Received(1).Send(
                Arg.Any<Uri>(),
                WebDavMethod.Report,
                Arg.Is(Predicates.CompareHeader("Depth", "infinity")),
                CancellationToken.None
            );
        }
    }
}
