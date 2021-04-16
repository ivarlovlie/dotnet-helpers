using System;
using Microsoft.AspNetCore.Http;

namespace IOL.Helpers
{
	public static class HttpRequestHelpers
	{
		/// <summary>
		/// Get's the scheme and host (scheme://host) value of the current HttpRequest
		/// </summary>
		/// <param name="request">HttpRequest to retrieve value from</param>
		/// <param name="ignoreForwared">Ignore header values like X-Forwarded-Host|Proto</param>
		/// <returns></returns>
		public static string GetRequestHost(this HttpRequest request, bool ignoreForwared = false) {
			if (!ignoreForwared) {
				var forwardedHostHeader = request.Headers["X-Forwarded-Host"].ToString();
				var forwardedProtoHeader = request.Headers["X-Forwarded-Proto"].ToString();
				if (forwardedHostHeader.HasValue()) {
					return (forwardedProtoHeader ?? "https") + "://" + forwardedHostHeader;
				}
			}

			return request.Scheme + "://" + request.Host;
		}
	}
}