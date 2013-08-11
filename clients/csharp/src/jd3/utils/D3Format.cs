/*
* Gary Cornell and Cay S. Horstmann, Core Java (Book/CD-ROM)
* Published By SunSoft Press/Prentice-Hall
* Copyright (C) 1996 Sun Microsystems Inc.
* All Rights Reserved. ISBN 0-13-565755-5
*
* Permission to use, copy, modify, and distribute this
* software and its documentation for NON-COMMERCIAL purposes
* and without fee is hereby granted provided that this
* copyright notice appears in all copies.
*
* THE AUTHORS AND PUBLISHER MAKE NO REPRESENTATIONS OR
* WARRANTIES ABOUT THE SUITABILITY OF THE SOFTWARE, EITHER
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
* IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
* PARTICULAR PURPOSE, OR NON-INFRINGEMENT. THE AUTHORS
* AND PUBLISHER SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED
* BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
* THIS SOFTWARE OR ITS DERIVATIVES.
*/
/// <summary> A class for formatting numbers that follows printf conventions.
/// Also implements C-like atoi and atof functions
/// </summary>
/// <version> 1.01 15 Feb 1996
/// </version>
/// <author> Cay Horstmann
/// 
/// </author>
namespace org.jd3.utils
{
	using System;
	
	public class D3Format
	{
		/// <summary> Formats the number following printf conventions.
		/// Main limitation: Can only handle one format parameter at a time
		/// Use multiple D3Format objects to format more than one number
		/// </summary>
		/// <param name="s">the format string following printf conventions
		/// The string has a prefix, a format code and a suffix. The prefix and suffix
		/// become part of the formatted output. The format code directs the
		/// formatting of the (single) parameter to be formatted. The code has the
		/// following structure
		/// <ul>
		/// <li> a % (required)
		/// <li> a modifier (optional)
		/// <dl>
		/// <dt> + <dd> forces display of + for positive numbers
		/// <dt> 0 <dd> show leading zeroes
		/// <dt> - <dd> align left in the field
		/// <dt> space <dd> prepend a space in front of positive numbers
		/// <dt> # <dd> use "alternate" format. Add 0 or 0x for octal or hexadecimal numbers. Don't suppress trailing zeroes in general floating point format.
		/// </dl>
		/// <li> an integer denoting field width (optional)
		/// <li> a period followed by an integer denoting precision (optional)
		/// <li> a format descriptor (required)
		/// <dl>
		/// <dt>f <dd> floating point number in fixed format
		/// <dt>e, E <dd> floating point number in exponential notation (scientific format). The E format results in an uppercase E for the exponent (1.14130E+003), the e format in a lowercase e.
		/// <dt>g, G <dd> floating point number in general format (fixed format for small numbers, exponential format for large numbers). Trailing zeroes are suppressed. The G format results in an uppercase E for the exponent (if any), the g format in a lowercase e.
		/// <dt>d, i <dd> integer in decimal
		/// <dt>x <dd> integer in hexadecimal
		/// <dt>o <dd> integer in octal
		/// <dt>s <dd> string
		/// <dt>c <dd> character
		/// </dl>
		/// </ul>
		/// </param>
		/// <exception cref="">IllegalArgumentException if bad format
		/// 
		/// </exception>
		
		public D3Format(System.String s)
		{
			width = 0;
			precision = - 1;
			pre = "";
			post = "";
			leading_zeroes = false;
			show_plus = false;
			alternate = false;
			show_space = false;
			left_align = false;
			fmt = ' ';
			
			int length = s.Length;
			int parse_state = 0;
			// 0 = prefix, 1 = flags, 2 = width, 3 = precision,
			// 4 = format, 5 = end
			int i = 0;
			
			while (parse_state == 0)
			{
				if (i >= length)
					parse_state = 5;
				else if (s[i] == '%')
				{
					if (i < length - 1)
					{
						if (s[i + 1] == '%')
						{
							pre = pre + '%';
							i++;
						}
						else
							parse_state = 1;
					}
					else
						throw new System.ArgumentException();
				}
				else
					pre = pre + s[i];
				i++;
			}
			while (parse_state == 1)
			{
				if (i >= length)
					parse_state = 5;
				else if (s[i] == ' ')
					show_space = true;
				else if (s[i] == '-')
					left_align = true;
				else if (s[i] == '+')
					show_plus = true;
				else if (s[i] == '0')
					leading_zeroes = true;
				else if (s[i] == '#')
					alternate = true;
				else
				{
					parse_state = 2; i--;
				}
				i++;
			}
			while (parse_state == 2)
			{
				if (i >= length)
					parse_state = 5;
				else if ('0' <= s[i] && s[i] <= '9')
				{
					width = width * 10 + s[i] - '0';
					i++;
				}
				else if (s[i] == '.')
				{
					parse_state = 3;
					precision = 0;
					i++;
				}
				else
					parse_state = 4;
			}
			while (parse_state == 3)
			{
				if (i >= length)
					parse_state = 5;
				else if ('0' <= s[i] && s[i] <= '9')
				{
					precision = precision * 10 + s[i] - '0';
					i++;
				}
				else
					parse_state = 4;
			}
			if (parse_state == 4)
			{
				if (i >= length)
					parse_state = 5;
				else
					fmt = s[i];
				i++;
			}
			if (i < length)
				post = s.Substring(i, (length) - (i));
		}
		
		/// <summary> prints a formatted number following printf conventions
		/// </summary>
		/// <param name="s">a PrintStream
		/// </param>
		/// <param name="fmt">the format string
		/// </param>
		/// <param name="x">the double to print
		/// 
		/// </param>
		
		public static void  print(System.IO.StreamWriter s, System.String fmt, double x)
		{
			s.Write(new D3Format(fmt).form(x));
		}
		
		/// <summary> prints a formatted number following printf conventions
		/// </summary>
		/// <param name="s">a PrintStream
		/// </param>
		/// <param name="fmt">the format string
		/// </param>
		/// <param name="x">the long to print
		/// 
		/// </param>
		public static void  print(System.IO.StreamWriter s, System.String fmt, long x)
		{
			s.Write(new D3Format(fmt).form(x));
		}
		
		/// <summary> prints a formatted number following printf conventions
		/// </summary>
		/// <param name="s">a PrintStream
		/// </param>
		/// <param name="fmt">the format string
		/// </param>
		/// <param name="x">the character to
		/// 
		/// </param>
		
		public static void  print(System.IO.StreamWriter s, System.String fmt, char x)
		{
			s.Write(new D3Format(fmt).form(x));
		}
		
		/// <summary> prints a formatted number following printf conventions
		/// </summary>
		/// <param name="s">a PrintStream, fmt the format string
		/// </param>
		/// <param name="x">a string that represents the digits to print
		/// 
		/// </param>
		
		public static void  print(System.IO.StreamWriter s, System.String fmt, System.String x)
		{
			s.Write(new D3Format(fmt).form(x));
		}
		
		/// <summary> Converts a string of digits (decimal, octal or hex) to an integer
		/// </summary>
		/// <param name="s">a string
		/// </param>
		/// <returns>the numeric value of the prefix of s representing a base 10 integer
		/// 
		/// </returns>
		
		public static int atoi(System.String s)
		{
			return (int) atol(s);
		}
		
		/// <summary> Converts a string of digits (decimal, octal or hex) to a long integer
		/// </summary>
		/// <param name="s">a string
		/// </param>
		/// <returns>the numeric value of the prefix of s representing a base 10 integer
		/// 
		/// </returns>
		
		public static long atol(System.String s)
		{
			int i = 0;
			
			while (i < s.Length && System.Char.IsWhiteSpace(s[i]))
			{
				i++;
			}
			if (i < s.Length && s[i] == '0')
			{
				if (i + 1 < s.Length && (s[i + 1] == 'x' || s[i + 1] == 'X'))
					return parseLong(s.Substring(i + 2), 16);
				else
					return parseLong(s, 8);
			}
			else
				return parseLong(s, 10);
		}
		
		private static long parseLong(System.String s, int base_Renamed)
		{
			int i = 0;
			int sign = 1;
			long r = 0;
			
			while (i < s.Length && System.Char.IsWhiteSpace(s[i]))
			{
				i++;
			}
			if (i < s.Length && s[i] == '-')
			{
				sign = - 1; i++;
			}
			else if (i < s.Length && s[i] == '+')
			{
				i++;
			}
			while (i < s.Length)
			{
				char ch = s[i];
				if ('0' <= ch && ch < '0' + base_Renamed)
					r = r * base_Renamed + ch - '0';
				else if ('A' <= ch && ch < 'A' + base_Renamed - 10)
					r = r * base_Renamed + ch - 'A' + 10;
				else if ('a' <= ch && ch < 'a' + base_Renamed - 10)
					r = r * base_Renamed + ch - 'a' + 10;
				else
					return r * sign;
				i++;
			}
			return r * sign;
		}
		
		/// <summary> Converts a string of digits to an double
		/// </summary>
		/// <param name="s">a string
		/// 
		/// </param>
		
		public static double atof(System.String s)
		{
			int i = 0;
			int sign = 1;
			double r = 0; // integer part
//			double f = 0; // fractional part
			double p = 1; // exponent of fractional part
			int state = 0; // 0 = int part, 1 = frac part
			
			while (i < s.Length && System.Char.IsWhiteSpace(s[i]))
			{
				i++;
			}
			if (i < s.Length && s[i] == '-')
			{
				sign = - 1; i++;
			}
			else if (i < s.Length && s[i] == '+')
			{
				i++;
			}
			while (i < s.Length)
			{
				char ch = s[i];
				if ('0' <= ch && ch <= '9')
				{
					if (state == 0)
						r = r * 10 + ch - '0';
					else if (state == 1)
					{
						p = p / 10;
						r = r + p * (ch - '0');
					}
				}
				else if (ch == '.')
				{
					if (state == 0)
						state = 1;
					else
						return sign * r;
				}
				else if (ch == 'e' || ch == 'E')
				{
					long e = (int) parseLong(s.Substring(i + 1), 10);
					return sign * r * System.Math.Pow(10, e);
				}
				else
					return sign * r;
				i++;
			}
			return sign * r;
		}
		
		/// <summary> Formats a double into a string (like sprintf in C)
		/// </summary>
		/// <param name="x">the number to format
		/// </param>
		/// <returns>the formatted string
		/// </returns>
		/// <exception cref="">IllegalArgumentException if bad argument
		/// 
		/// </exception>
		
		public virtual System.String form(double x)
		{
			System.String r;
			if (precision < 0)
				precision = 6;
			int s = 1;
			if (x < 0)
			{
				x = - x; s = - 1;
			}
			if (fmt == 'f')
				r = fixed_format(x);
			else if (fmt == 'e' || fmt == 'E' || fmt == 'g' || fmt == 'G')
				r = exp_format(x);
			else
				throw new System.ArgumentException();
			
			return pad(sign(s, r));
		}
		
		/// <summary> Formats a long integer into a string (like sprintf in C)
		/// </summary>
		/// <param name="x">the number to format
		/// </param>
		/// <returns>the formatted string
		/// 
		/// </returns>
		
		public virtual System.String form(long x)
		{
			System.String r;
			int s = 0;
			if (fmt == 'd' || fmt == 'i')
			{
				s = 1;
				if (x < 0)
				{
					x = - x; s = - 1;
				}
				r = "" + x;
			}
			else if (fmt == 'o')
				r = convert(x, 3, 7, "01234567");
			else if (fmt == 'x')
				r = convert(x, 4, 15, "0123456789abcdef");
			else if (fmt == 'X')
				r = convert(x, 4, 15, "0123456789ABCDEF");
			else
				throw new System.ArgumentException();
			
			return pad(sign(s, r));
		}
		
		/// <summary> Formats a character into a string (like sprintf in C)
		/// </summary>
		/// <param name="x">the value to format
		/// </param>
		/// <returns>the formatted string
		/// 
		/// </returns>
		
		public virtual System.String form(char c)
		{
			if (fmt != 'c')
				throw new System.ArgumentException();
			
			System.String r = "" + c;
			return pad(r);
		}
		
		/// <summary> Formats a string into a larger string (like sprintf in C)
		/// </summary>
		/// <param name="x">the value to format
		/// </param>
		/// <returns>the formatted string
		/// 
		/// </returns>
		
		public virtual System.String form(System.String s)
		{
			if (fmt != 's')
				throw new System.ArgumentException();
			if (precision >= 0)
				s = s.Substring(0, (precision) - (0));
			return pad(s);
		}
		
		
		private static System.String repeat(char c, int n)
		{
			if (n <= 0)
				return "";
			System.Text.StringBuilder s = new System.Text.StringBuilder(n);
			 for (int i = 0; i < n; i++)
				s.Append(c);
			return s.ToString();
		}
		
		private static System.String convert(long x, int n, int m, System.String d)
		{
			if (x == 0)
				return "0";
			System.String r = "";
			while (x != 0)
			{
				r = d[(int) (x & m)] + r;
				//x = SupportClass.URShift(x, n);
				x = x >> n;
			}
			return r;
		}
		
		private System.String pad(System.String r)
		{
			System.String p = repeat(' ', width - r.Length);
			if (left_align)
				return pre + r + p + post;
			else
				return pre + p + r + post;
		}
		
		private System.String sign(int s, System.String r)
		{
			System.String p = "";
			if (s < 0)
				p = "-";
			else if (s > 0)
			{
				if (show_plus)
					p = "+";
				else if (show_space)
					p = " ";
			}
			else
			{
				if (fmt == 'o' && alternate && r.Length > 0 && r[0] != '0')
					p = "0";
				else if (fmt == 'x' && alternate)
					p = "0x";
				else if (fmt == 'X' && alternate)
					p = "0X";
			}
			int w = 0;
			if (leading_zeroes)
				w = width;
			else if ((fmt == 'd' || fmt == 'i' || fmt == 'x' || fmt == 'X' || fmt == 'o') && precision > 0)
				w = precision;
			
			return p + repeat('0', w - p.Length - r.Length) + r;
		}
		
		
		private System.String fixed_format(double d)
		{
			System.String f = "";
			
			if (d > 0x7FFFFFFFFFFFFFFFL)
				return exp_format(d);
			
			long l = (long) (precision == 0?d + 0.5:d);
			f = f + l;
			
			double fr = d - l; // fractional part
			if (fr >= 1 || fr < 0)
				return exp_format(d);
			
			return f + frac_part(fr);
		}
		
		private System.String frac_part(double fr)
		{
			// precondition: 0 <= fr < 1
			System.String z = "";
			if (precision > 0)
			{
				double factor = 1;
				System.String leading_zeroes = "";
				 for (int i = 1; i <= precision && factor <= 0x7FFFFFFFFFFFFFFFL; i++)
				{
					factor *= 10;
					leading_zeroes = leading_zeroes + "0";
				}
				long l = (long) (factor * fr + 0.5);
				
				z = leading_zeroes + l;
				z = z.Substring(z.Length - precision, (z.Length) - (z.Length - precision));
			}
			
			
			if (precision > 0 || alternate)
				z = "." + z;
			if ((fmt == 'G' || fmt == 'g') && !alternate)
			{
				// remove trailing zeroes and decimal point
				int t = z.Length - 1;
				while (t >= 0 && z[t] == '0')
				{
					t--;
				}
				if (t >= 0 && z[t] == '.')
					t--;
				z = z.Substring(0, (t + 1) - (0));
			}
			return z;
		}
		
		private System.String exp_format(double d)
		{
			System.String f = "";
			int e = 0;
			double dd = d;
			double factor = 1;
			while (dd > 10)
			{
				e++; factor /= 10; dd = dd / 10;
			}
			while (dd < 1)
			{
				e--; factor *= 10; dd = dd * 10;
			}
			if ((fmt == 'g' || fmt == 'G') && e >= - 4 && e < precision)
				return fixed_format(d);
			
			d = d * factor;
			f = f + fixed_format(d);
			
			if (fmt == 'e' || fmt == 'g')
				f = f + "e";
			else
				f = f + "E";
			
			System.String p = "000";
			if (e >= 0)
			{
				f = f + "+";
				p = p + e;
			}
			else
			{
				f = f + "-";
				p = p + (- e);
			}
			
			return f + p.Substring(p.Length - 3, (p.Length) - (p.Length - 3));
		}
		
		private int width;
		private int precision;
		private System.String pre;
		private System.String post;
		private bool leading_zeroes;
		private bool show_plus;
		private bool alternate;
		private bool show_space;
		private bool left_align;
		private char fmt; // one of cdeEfgGiosxXos
	}
}
