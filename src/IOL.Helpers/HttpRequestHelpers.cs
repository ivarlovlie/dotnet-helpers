using System;
using Microsoft.AspNetCore.Http;

namespace IOL.Helpers
{
	public static class HttpRequestHelpers
	{
		public static string GetAppHost(this HttpRequest request) {
			var forwardedHostHeader = request.Headers["X-Forwarded-Host"].ToString();
			var forwardedProtoHeader = request.Headers["X-Forwarded-Proto"].ToString();
			if (forwardedHostHeader.HasValue()) {
				return (forwardedProtoHeader ?? "https") + "://" + forwardedHostHeader;
			}

			return request.Scheme + "://" + request.Host;
		}
	}
}