using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Miracle.Arguments
{
	/// <summary>
	/// WordWrap functionality
	/// </summary>
	public static class WordWrap
	{
		/// <summary>
		/// Wraps the specified text into lines with indent.
		/// </summary>
		/// <param name="writer">TextWriter to write output to</param>
		/// <param name="text">The text that are to be split into lines.</param>
		/// <param name="lineLength">Maximum line length.</param>
		/// <param name="indent">Number of spaces to indent lines</param>
		public static void WordWrapText(TextWriter writer, string text, int lineLength, int indent)
		{
			if (text != null)
			{
				foreach (var line in WordWrapText(text,lineLength))
				{
					for(int i = 0; i < indent; i++)
						writer.Write(' ');
					writer.WriteLine(line);
				}
			}
		}

		/// <summary>
		/// Wraps the specified text into lines. CR/LF are honored.
		/// </summary>
		/// <param name="text">The text that are to be split into lines.</param>
		/// <param name="lineLength">Maximum line length.</param>
		/// <returns>List of lines </returns>
		public static IEnumerable<string> WordWrapText(string text, int lineLength)
		{
			var lines = new List<string>();
			var paragraphs = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var paragraph in paragraphs)
			{
				lines.AddRange(WordWrapParagraph(paragraph, lineLength));
			}

			return lines;
		}

		/// <summary>
		/// Wraps the specified text into lines.
		/// </summary>
		/// <param name="paragraph">The text that are to be split into lines.</param>
		/// <param name="lineLength">Maximum line length.</param>
		/// <returns>List of lines </returns>
		public static IEnumerable<string> WordWrapParagraph(this string paragraph, int lineLength)
		{
			var lines = new List<string>();
			int startPos = 0;

			while (startPos < paragraph.Length)
			{
				int endPos = startPos + lineLength - 1;
				if(endPos < paragraph.Length - 1)
				{
					while (endPos > startPos && !Char.IsSeparator(paragraph, endPos))
						endPos--;
					if (endPos == startPos)
						endPos = startPos + lineLength - 1;
				}
				else
					endPos = paragraph.Length - 1;

				var line = paragraph.Substring(startPos, endPos - startPos + 1);
				Debug.Assert(line.Length > 0, "Line length is 0");
				Debug.Assert(line.Length <= lineLength, "LineLength exceeded");

				lines.Add(line);

				startPos = endPos + 1;
			}

			return lines;
		}
	}
}
