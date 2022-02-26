using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IOL.Helpers;

public static class CryptographyHelpers
{
	private const int AES_BLOCK_BYTE_SIZE = 128 / 8;
	private static readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

	/// <summary>
	/// Creates a MD5 hash of the specified input.
	/// </summary>
	/// <returns>A hash</returns>
	public static byte[] AsMD5Hash(this string input, string salt = default) {
		if (input.IsNullOrWhiteSpace()) {
			return default;
		}

		var hmacMd5 = salt.HasValue() ? new HMACMD5(Encoding.UTF8.GetBytes(salt ?? "")) : new HMACMD5();
		return hmacMd5.ComputeHash(Encoding.UTF8.GetBytes(input));
	}

	public static byte[] AsMD5Hash(this byte[] input, string salt = default) {
		if (input == null) {
			return default;
		}

		var hmacMd5 = salt.HasValue() ? new HMACMD5(Encoding.UTF8.GetBytes(salt ?? "")) : new HMACMD5();
		return hmacMd5.ComputeHash(input);
	}

	// https://stackoverflow.com/a/13026595/253938
	/// <summary>
	/// Method to perform a very simple (and classical) encryption for a string. This is NOT at
	/// all secure, it is only intended to make the string value non-obvious at a first glance.
	///
	/// The shiftOrUnshift argument is an arbitrary "key value", and must be a non-zero integer
	/// between -65535 and 65535 (inclusive). To decrypt the encrypted string you use the negative
	/// value. For example, if you encrypt with -42, then you decrypt with +42, or vice-versa.
	/// </summary>
	/// <param name="input">string to be encrypted or decrypted, must not be null</param>
	/// <param name="shiftOrUnshift">see above</param>
	/// <returns>encrypted or decrypted string</returns>
	public static string CaesarCipher(this string input, int shiftOrUnshift) {
		if (input.IsNullOrWhiteSpace()) {
			return default;
		}

		const int C64_K = ushort.MaxValue + 1;
		if (input == null) throw new ArgumentException("Must not be null.", nameof(input));
		switch (shiftOrUnshift) {
			case 0: throw new ArgumentException("Must not be zero.", nameof(shiftOrUnshift));
			case <= -C64_K:
			case >= C64_K: throw new ArgumentException("Out of range.", nameof(shiftOrUnshift));
		}

		// Perform the Caesar cipher shifting, using modulo operator to provide wrap-around
		var charArray = new char[input.Length];
		for (var i = 0; i < input.Length; i++) {
			charArray[i] = Convert.ToChar((Convert.ToInt32(input[i]) + shiftOrUnshift + C64_K) % C64_K);
		}

		return new string(charArray);
	}

	//https://tomrucki.com/posts/aes-encryption-in-csharp/
	/// <summary>
	/// Encrypts a string input with aes encryption
	/// </summary>
	/// <param name="input">The string to encrypt</param>
	/// <param name="encryptionKey">The key to encrypt the input with</param>
	/// <returns>A base 64 encoded string of the encrypted input</returns>
	/// <exception cref="ArgumentNullException">Throws null if encryptionKey is empty or null</exception>
	public static string EncryptWithAes(this string input, string encryptionKey) {
		if (input.IsNullOrWhiteSpace()) {
			return default;
		}

		if (encryptionKey.IsNullOrWhiteSpace()) {
			throw new ArgumentNullException(nameof(encryptionKey));
		}

		var key = encryptionKey.AsMD5Hash();

		using var aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;

		var iv = new byte[AES_BLOCK_BYTE_SIZE];
		_random.GetBytes(iv);

		var plainText = Encoding.UTF8.GetBytes(input);
		using var encryptor = aes.CreateEncryptor(key, iv);
		var cipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

		var result = new byte[iv.Length + cipherText.Length];
		iv.CopyTo(result, 0);
		cipherText.CopyTo(result, iv.Length);

		return Convert.ToBase64String(result);
	}

	/// <summary>
	/// Decrypts a string input with aes encryption
	/// </summary>
	/// <param name="input">The base 64 encoded string to decrypt</param>
	/// <param name="encryptionKey">The key to decrypt the input with</param>
	/// <returns>A string of the decrypted input</returns>
	/// <exception cref="ArgumentNullException">Throws null if encryptionKey is empty or null</exception>
	public static string DecryptWithAes(this string input, string encryptionKey) {
		if (input.IsNullOrWhiteSpace()) {
			return default;
		}

		if (encryptionKey.IsNullOrWhiteSpace()) {
			throw new ArgumentNullException(nameof(encryptionKey));
		}

		var key = encryptionKey.AsMD5Hash();
		var encryptedData = Convert.FromBase64String(input);
		using var aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		var iv = encryptedData.Take(AES_BLOCK_BYTE_SIZE).ToArray();
		var cipherText = encryptedData.Skip(AES_BLOCK_BYTE_SIZE).ToArray();
		using var decryptor = aes.CreateDecryptor(key, iv);
		var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
		return Encoding.UTF8.GetString(decryptedBytes);
	}

	/// <summary>
	/// Creates a SHA256 hash of the specified input.
	/// </summary>
	/// <param name="input">The input</param>
	/// <returns>A base 64 encoded hash string of the input</returns>
	public static string AsSHA256Hash(this string input) {
		if (input.IsNullOrWhiteSpace()) return default;
		using var sha = SHA256.Create();
		var bytes = Encoding.UTF8.GetBytes(input);
		var hash = sha.ComputeHash(bytes);
		return Convert.ToBase64String(hash);
	}

	/// <summary>
	/// Creates a SHA256 hash of the specified input.
	/// </summary>
	/// <param name="input">The input</param>
	/// <returns>A byte array containing the hash of the input</returns>
	public static byte[] AsSHA256Hash(this byte[] input) {
		if (input == null) return default;
		using var sha = SHA256.Create();
		return sha.ComputeHash(input);
	}

	/// <summary>
	/// Creates a SHA512 hash of the specified input.
	/// </summary>
	/// <param name="input">The input</param>
	/// <returns>A base 64 encoded hash string of the input</returns>
	public static string AsSHA512(this string input) {
		if (input.IsNullOrWhiteSpace()) return default;
		using var sha = SHA512.Create();
		var bytes = Encoding.UTF8.GetBytes(input);
		var hash = sha.ComputeHash(bytes);
		return Convert.ToBase64String(hash);
	}

	/// <summary>
	/// Creates a SHA256 hash of the specified input.
	/// </summary>
	/// <param name="input">The input</param>
	/// <returns>A byte array containing the hash of the input</returns>
	public static byte[] AsSHA512(this byte[] input) {
		if (input == null) return default;
		using var sha = SHA512.Create();
		return sha.ComputeHash(input);
	}
}
