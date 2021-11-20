using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IOL.Helpers;

public static class CryptographyHelpers
{
	// https://github.com/DuendeSoftware/IdentityServer/blob/main/src/IdentityServer/Extensions/HashExtensions.cs

	private const int AES_BLOCK_BYTE_SIZE = 128 / 8;
	private static readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

	/// <summary>
	/// Creates a MD5 hash of the specified input.
	/// </summary>
	/// <returns>A hash</returns>
	public static string Md5(this string input, string salt = default) {
		if (input.IsNullOrWhiteSpace()) return string.Empty;

		var hmacMd5 = salt.HasValue() ? new HMACMD5(Encoding.UTF8.GetBytes(salt ?? "")) : new HMACMD5();
		var saltedHash = hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(input));
		return Convert.ToBase64String(saltedHash);
	}


	/// <summary>
	/// Method to perform a very simple (and classical) encryption for a string. This is NOT at
	/// all secure, it is only intended to make the string value non-obvious at a first glance.
	///
	/// The shiftOrUnshift argument is an arbitrary "key value", and must be a non-zero integer
	/// between -65535 and 65535 (inclusive). To decrypt the encrypted string you use the negative
	/// value. For example, if you encrypt with -42, then you decrypt with +42, or vice-versa.
	///
	/// This is inspired by, and largely based on, this:
	/// https://stackoverflow.com/a/13026595/253938
	/// </summary>
	/// <param name="inputString">string to be encrypted or decrypted, must not be null</param>
	/// <param name="shiftOrUnshift">see above</param>
	/// <returns>encrypted or decrypted string</returns>
	public static string CaesarCipher(string inputString, int shiftOrUnshift) {
		const int C64_K = ushort.MaxValue + 1;
		if (inputString == null) throw new ArgumentException("Must not be null.", nameof(inputString));
		switch (shiftOrUnshift) {
			case 0: throw new ArgumentException("Must not be zero.", nameof(shiftOrUnshift));
			case <= -C64_K:
			case >= C64_K: throw new ArgumentException("Out of range.", nameof(shiftOrUnshift));
		}

		// Perform the Caesar cipher shifting, using modulo operator to provide wrap-around
		var charArray = new char[inputString.Length];
		for (var i = 0; i < inputString.Length; i++) {
			charArray[i] =
					Convert.ToChar((Convert.ToInt32(inputString[i]) + shiftOrUnshift + C64_K) % C64_K);
		}

		return new string(charArray);
	}

	//https://tomrucki.com/posts/aes-encryption-in-csharp/
	public static string EncryptWithAes(this string toEncrypt, string password) {
		var key = GetKey(password);

		using var aes = CreateAes();
		var iv = GenerateRandomBytes(AES_BLOCK_BYTE_SIZE);
		var plainText = Encoding.UTF8.GetBytes(toEncrypt);

		using var encryptor = aes.CreateEncryptor(key, iv);
		var cipherText = encryptor
				.TransformFinalBlock(plainText, 0, plainText.Length);

		var result = new byte[iv.Length + cipherText.Length];
		iv.CopyTo(result, 0);
		cipherText.CopyTo(result, iv.Length);

		return Convert.ToBase64String(result);
	}

	private static Aes CreateAes() {
		var aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		return aes;
	}

	public static string DecryptWithAes(this string input, string password) {
		var key = GetKey(password);
		var encryptedData = Convert.FromBase64String(input);

		using var aes = CreateAes();
		var iv = encryptedData.Take(AES_BLOCK_BYTE_SIZE).ToArray();
		var cipherText = encryptedData.Skip(AES_BLOCK_BYTE_SIZE).ToArray();

		using var decryptor = aes.CreateDecryptor(key, iv);
		var decryptedBytes = decryptor
				.TransformFinalBlock(cipherText, 0, cipherText.Length);
		return Encoding.UTF8.GetString(decryptedBytes);
	}

	private static byte[] GetKey(string password) {
		var keyBytes = Encoding.UTF8.GetBytes(password);
		using var md5 = MD5.Create();
		return md5.ComputeHash(keyBytes);
	}

	private static byte[] GenerateRandomBytes(int numberOfBytes) {
		var randomBytes = new byte[numberOfBytes];
		_random.GetBytes(randomBytes);
		return randomBytes;
	}


	/// <summary>
	/// Creates a SHA256 hash of the specified input.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <returns>A hash</returns>
	public static string Sha256(this string input) {
		if (input.IsNullOrWhiteSpace()) return string.Empty;

		using var sha = SHA256.Create();
		var bytes = Encoding.UTF8.GetBytes(input);
		var hash = sha.ComputeHash(bytes);

		return Convert.ToBase64String(hash);
	}

	/// <summary>
	/// Creates a SHA256 hash of the specified input.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <returns>A hash.</returns>
	public static byte[] Sha256(this byte[] input) {
		if (input == null) {
			return null;
		}

		using var sha = SHA256.Create();
		return sha.ComputeHash(input);
	}

	/// <summary>
	/// Creates a SHA512 hash of the specified input.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <returns>A hash</returns>
	public static string Sha512(this string input) {
		if (input.IsNullOrWhiteSpace()) return string.Empty;

		using var sha = SHA512.Create();
		var bytes = Encoding.UTF8.GetBytes(input);
		var hash = sha.ComputeHash(bytes);

		return Convert.ToBase64String(hash);
	}


	/// <summary>
	/// Creates a SHA256 hash of the specified input.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <returns>A hash.</returns>
	public static byte[] Sha512(this byte[] input) {
		if (input == null) {
			return null;
		}

		using var sha = SHA512.Create();
		return sha.ComputeHash(input);
	}
}
