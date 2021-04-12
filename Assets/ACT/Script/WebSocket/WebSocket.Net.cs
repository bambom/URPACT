using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.ClientEngine;

namespace WebSocket4Net
{
    public partial class WebSocket
    {
        private static List<KeyValuePair<string, string>> EmptyCookies = null;

        private bool m_AllowUnstrustedCertificate;

        /// <summary>
        /// Gets or sets a value indicating whether [allow unstrusted certificate] when connect a secure websocket uri.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow unstrusted certificate]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowUnstrustedCertificate
        {
            get { return m_AllowUnstrustedCertificate; }
            set
            {
                m_AllowUnstrustedCertificate = value;
            }
        }

        public WebSocket(string uri)
        {
            Initialize(uri);
        }
    }
}
