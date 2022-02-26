using System;
using System.Collections.Generic;
using Xunit;

namespace IOL.Helpers.Tests;

public class StringHelpersTests
{
	[Fact]
	public void Base64_Encode_Decode_Behaves() {
		const string input = "value";
		var encoded = input.AsBase64EncodedString();
		Assert.Equal(input, encoded.AsBase64DecodedString());
	}

	[Fact]
	public void Unicorn_Format_Behaves() {
		const string input = "Hello, {name}";
		var result = input.UnicornFormat(new Dictionary<string, string> {
				{
						"name", "World"
				}
		});
		Assert.Equal("Hello, World", result);
	}

	[Fact]
	public void IsNullOrWhiteSpace_Behaves() {
		const string stringWithValue = "value";
		const string emptyString = "";
		const string onlyWhitespace = "  ";
		Assert.False(stringWithValue.IsNullOrWhiteSpace());
		Assert.True(emptyString.IsNullOrWhiteSpace());
		Assert.True(onlyWhitespace.IsNullOrWhiteSpace());
	}

	[Fact]
	public void AsSnakeCasedString_Behaves() {
		Assert.Equal("snake_case", "SNAKE Case ".AsSnakeCasedString());
	}

	[Fact]
	public void AsSlug_Behaves() {
		Assert.Equal("tromso-at-night", "Tromsø @ night".AsSlug());
	}

	[Fact]
	public void HasValue_Behaves() {
		Assert.True("asdf".HasValue());
		Assert.False("  ".HasValue());
	}

	[Fact]
	public void AsGuid_Behaves() {
		Assert.Equal(new Guid("bb7e2fd8-2ded-4fe9-a770-efeba661539c"), "bb7e2fd8-2ded-4fe9-a770-efeba661539c".AsGuid());
		Assert.Throws<FormatException>(() => "not-a-guid".AsGuid());
	}

	[Fact]
	public void AsGuidOrDefault_Behaves() {
		Assert.Equal(new Guid("bb7e2fd8-2ded-4fe9-a770-efeba661539c"), "bb7e2fd8-2ded-4fe9-a770-efeba661539c".AsGuid());
		Assert.Equal(default(Guid), "not-a-guid".AsGuidOrDefault());
	}

	[Fact]
	public void Capitalize_Behaves() {
		Assert.Equal("The File", "the file".Capitalize());
		Assert.Equal("The file", "the file".Capitalize(true));
	}

	[Fact]
	public void Obfuscate_Behaves() {
		Assert.Equal("********", "asdf".Obfuscate());
		Assert.Equal("****1234", "asdfasdf1234".Obfuscate());
	}
}
