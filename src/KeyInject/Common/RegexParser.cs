using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KeyInject.Common
{
	internal class RegexParser
	{
		private static readonly TimeSpan MaxRegexTimeout = TimeSpan.FromSeconds(10);
		private static readonly RegexOptions IgnoreCaseOptions = RegexOptions.IgnoreCase & RegexOptions.CultureInvariant;
		public static bool TryParse(string regexPattern, bool ignoreCase, out Regex? regex)
		{
			regex = null;
			if (string.IsNullOrWhiteSpace(regexPattern)) return false;
			var options = ignoreCase ? IgnoreCaseOptions : RegexOptions.None;
			try
			{
				regex = new Regex(regexPattern, options, MaxRegexTimeout);
				return true;
			}
			catch
			{
				Debug.WriteLine(string.Format("Failed to parse regex: {0}", regexPattern));
				return false;
			}
		}
	}
}