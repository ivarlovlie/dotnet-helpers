using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace IOL.Helpers
{
	public static class InMemoryZipArchive
	{
		public static byte[] Create(IEnumerable<InMemoryFile> files, string unixPermissionString = "664") {
			using var archiveStream = new MemoryStream();
			using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true)) {
				foreach (var file in files) {
					var zipArchiveEntry = archive.CreateEntry(file.FileName, CompressionLevel.Fastest);
					zipArchiveEntry.ExternalAttributes |= Convert.ToInt32(unixPermissionString, 8) << 16;
					using var zipStream = zipArchiveEntry.Open();
					zipStream.Write(file.Content, 0, file.Content.Length);
				}
			}

			return archiveStream.ToArray();
		}

		public class InMemoryFile
		{
			public string FileName { get; set; }
			public byte[] Content { get; set; }
		}
	}
}
