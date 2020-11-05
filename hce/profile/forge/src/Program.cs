﻿/**
 * Copyright (c) 2019-2020 Emilian Roman (Miris Wisdom) <roman.emilian@outlook.com>
 * 
 * This software is provided 'as-is', without any express or implied warranty. In no event will
 * the authors be held liable for any damages arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose, including commercial
 * applications, and to alter it and redistribute it freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not claim that you wrote the
 * original software. If you use this software in a product, an acknowledgment (see the following)
 * in the product documentation is required.
 * 
 *    Portions Copyright (c) 2019-2020 Emilian Roman (Miris Wisdom) <roman.emilian@outlook.com>
 * 
 * 2. Altered source versions must be plainly marked as such, and must not be misrepresented as
 * being the original software.
 * 
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;
using static System.BitConverter;
using static System.Console;
using static System.Environment;
using static System.IO.FileAccess;
using static System.IO.FileMode;

namespace Miris.HCE.Profile.Forge
{
  /// <summary>
  ///   Main Profile.Forge entry.
  /// </summary>
  internal static class Program
  {
    private const int  BinaryLength = 0x2000;              /* canonical length for hce profile binary */
    private const uint ExpectedHash = Hash.Initialisation; /* expected crc-32 hash for a valid binary */

    /// <summary>
    ///   Calculates CRC-32 for the inbound HCE profile binary and optionally patches said binary.
    /// </summary>
    /// <param name="args">
    ///   -w, --write         write hash to provided profile binary
    ///   -p, --profile=VALUE profile to calculate the hash for
    /// </param>
    private static void Main(string[] args)
    {
      /**
       * We parse the arguments provided to the CLI executable to determine what blam.sav binary the program should
       * calculate the CRC-32 for, and whether the file in question should be patched with the complement of the
       * calculated hash.
       */
      var write   = false;
      var profile = string.Empty;

      var options = new OptionSet
      {
        {
          "w|write", "write hash to provided profile binary",
          v => write = v != null
        },
        {
          "p|profile=", "profile to calculate the hash for",
          v => profile = v
        }
      };

      WriteLine(new[] {"H C E    P R O F I L E    C R C - 3 2    F O R G E    / /    ~ M I R I S"}, '*');
      options.WriteOptionDescriptions(Out);
      options.Parse(args);

      if (string.IsNullOrEmpty(profile))
      {
        Error.WriteLine("no args given for binary path on fs"!);
        Exit(1);
      }

      if (!File.Exists(profile))
      {
        Error.WriteLine("provided binary was not found on fs!");
        Exit(2);
      }

      /**
       * The HCE profile binary (blam.sav) comprises of two parts: the main data and the appended CRC-32 hash at the end
       * of the binary.
       *
       * A valid binary will always be 8192 bytes in length, and the hash will always be 4 bytes in length.
       * A valid binary will also always have 0xFFFFFFFF as its CRC-32 hash, when the whole binary is hashed.
       *
       * With the above statements, we can infer two important details:
       *
       * 1.  The offset of the hash is at ([BINARY LENGTH] - [HASH LENGTH]) .:. ([0x2000] - [0x0004]) .:. [0x1FFC],
       *     meaning that our calculated hash must overwrite at offset 0x1FFC and on (i.e. replacing the previous hash).
       *
       * 2.  To confidently assert that the patched binary will be deemed valid by HCE, we can calculate the CRC-32 of
       *     the whole binary and verify that 0xFFFFFFFF is its hash value.
       */
      using (var fs = new FileStream(profile, Open, write ? ReadWrite : FileAccess.Read))
      {
        var fileData = new byte[BinaryLength];
        fs.Seek(0, SeekOrigin.Begin);
        fs.Read(fileData, 0, BinaryLength);

        /**
         * We verify the authenticity of the binary by calculating the CRC-32 of all of its data, and then compare the
         * result to a constant value of 0xFFFFFFFF to determine if it's an authentic binary or a tampered one.
         *
         * In this context, authentic means that the binary was generated by HCE, and tampered means that its contents
         * were modified externally without forging the hash to 0xFFFFFFFF.
         */
        var fileHash  = Hash.Compute(fileData);
        var authentic = fileHash == ExpectedHash;

        WriteLine(new[]
        {
          @"BINARY FILE VERIFICATION",
          @"------------------------",
          $"binary file name on fs - {Path.GetFileName(profile)}",
          $"binary file crc32 hash - 0x{fileHash:x8} [{(authentic ? "file == authentic" : "file != authentic")}]"
        }, '=');

        /**
         * Here, we compute the hash of only the main data in the binary. The main data is the contents of the binary
         * minus the hash at the end, i.e. profile settings and information.
         *
         * We also declare the bitwise complement of the calculated hash, because it is this complement that will allow
         * the final output to be considered valid by HCE, i.e., have an overall hash of 0xFFFFFFFF.
         */
        var mainData = new byte[BinaryLength - Hash.Length];
        Array.Copy(fileData, mainData, BinaryLength - Hash.Length);

        var mainHash = Hash.Compute(mainData);
        var mainComp = ~mainHash;

        /**
         * As mentioned earlier, we can verify that the computed complement is correct by appending it to the main data
         * from the binary, and subsequently computing the hash. If the hash of this concatenation equals to 0xFFFFFFFF,
         * then the complement we calculated is correct and can be thus patched to the binary.
         */
        var testData = new byte[BinaryLength];

        Array.Copy(mainData,           0, testData, 0,               mainData.Length);
        Array.Copy(GetBytes(mainComp), 0, testData, mainData.Length, Hash.Length);

        var testHash = Hash.Compute(testData);
        var testPass = testHash == ExpectedHash;

        WriteLine(new[]
        {
          @"HASH FORGING INFORMATION",
          @"------------------------",
          $"forged crc32 hash val - 0x{mainComp:x8}   [~0x{mainHash:x8}]",
          $"validity verification - 0x{testHash:x8}   [{(testPass ? "forged hash == valid" : "forged hash != valid")}]",
          @"                        |--------| = [main data + calculated hash]"
        }, '=');

        /**
         * If the end-user requested the provided binary to be patched AND the computed complement (the forge hash) is
         * asserted as valid, we will overwrite the existing data at offset ([BINARY LENGTH] - [HASH LENGTH]) with the
         * computed complement. This forges the binary, and by virtue permit HCE to consider it valid.
         */
        if (write)
        {
          if (testPass)
          {
            fs.Seek(BinaryLength - Hash.Length, SeekOrigin.Begin);
            fs.Write(GetBytes(mainComp), 0, Hash.Length);
            fs.Flush(true);

            WriteLine(new[]
            {
              @"BINARY CRC HASH PATCHING",
              @"------------------------",
              $"wrote crc32 hash - 0x{mainComp:x8}",
              $"at binary offset - 0x{BinaryLength - Hash.Length:x8}",
              $"binary file name - {Path.GetFileName(profile)}"
            }, '=');
          }
          else
          {
            Error.WriteLine("forged hash is invalid, refusing to patch");
            Exit(3);
          }

          Exit(0);
        }
      }

      /**
       * This one's to write somewhat prettier stuff to the standard output.
       */
      static void WriteLine(IEnumerable<string> lines, char character)
      {
        var decoration = new string(character, 72);
        Console.WriteLine(NewLine + decoration);
        foreach (var line in lines) Console.WriteLine(line);
        Console.WriteLine(decoration + NewLine);
      }
    }
  }
}