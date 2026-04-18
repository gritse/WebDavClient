using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace WebDav
{
    /// <summary>
    /// Parameters for the REPORT operation.
    /// </summary>
    public class ReportParameters
    {
        public ReportParameters()
        {
            Headers = new List<KeyValuePair<string, string>>();
            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the method is to be applied only to the resource, to the resource and its internal members only, or the resource and all its members.
        /// It corresponds to the WebDAV Depth header.
        /// </summary>
        public ApplyTo.Report? ApplyTo { get; set; }

        /// <summary>
        /// Gets or sets the XML request body for the REPORT operation.
        /// </summary>
        public XDocument? RequestBody { get; set; }

        /// <summary>
        /// Gets or sets the collection of headers.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<string, string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }
}
