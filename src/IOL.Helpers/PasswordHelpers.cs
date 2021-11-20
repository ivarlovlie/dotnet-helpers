using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IOL.Helpers;

public static class PasswordHelper
{
	private const int ITERATION_COUNT = 10000;
	private const int SALT_SIZE = 128 / 8;
	private const KeyDerivationPrf PRF = KeyDerivationPrf.HMACSHA256;

	public static string HashPassword(string value) {
		using var rng = RandomNumberGenerator.Create();
		var salt = new byte[SALT_SIZE];
		rng.GetBytes(salt);
		var subkey = KeyDerivation.Pbkdf2(value, salt, PRF, ITERATION_COUNT, 256 / 8);
		var outputBytes = new byte[13 + salt.Length + subkey.Length];
		WriteNetworkByteOrder(outputBytes, 1, (uint)PRF);
		WriteNetworkByteOrder(outputBytes, 5, ITERATION_COUNT);
		WriteNetworkByteOrder(outputBytes, 9, SALT_SIZE);
		Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
		Buffer.BlockCopy(subkey, 0, outputBytes, 13 + SALT_SIZE, subkey.Length);
		return Convert.ToBase64String(outputBytes);
	}

	public static bool Verify(string password, string hashedPassword) {
		var decodedHashedPassword = Convert.FromBase64String(hashedPassword);
		if (decodedHashedPassword.Length == 0) return false;
		try {
			// Read header information
			var networkByteOrder = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
			var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

			// Read the salt: must be >= 128 bits
			if (saltLength < SALT_SIZE) return false;
			var salt = new byte[saltLength];
			Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

			// Read the subkey (the rest of the payload): must be >= 128 bits
			var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
			if (subkeyLength < SALT_SIZE) return false;
			var expectedSubkey = new byte[subkeyLength];
			Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

			// Hash the incoming password and verify it
			var actualSubkey =
					KeyDerivation.Pbkdf2(password, salt, networkByteOrder, ITERATION_COUNT, subkeyLength);
			return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
		} catch {
			return false;
		}
	}

	private static uint ReadNetworkByteOrder(IReadOnlyList<byte> buffer, int offset) {
		return ((uint)buffer[offset + 0] << 24)
			   | ((uint)buffer[offset + 1] << 16)
			   | ((uint)buffer[offset + 2] << 8)
			   | buffer[offset + 3];
	}

	private static void WriteNetworkByteOrder(IList<byte> buffer, int offset, uint value) {
		buffer[offset + 0] = (byte)(value >> 24);
		buffer[offset + 1] = (byte)(value >> 16);
		buffer[offset + 2] = (byte)(value >> 8);
		buffer[offset + 3] = (byte)(value >> 0);
	}
}
