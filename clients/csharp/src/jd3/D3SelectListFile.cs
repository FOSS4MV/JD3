/* 
* D3SelectListFile.java - The implmentation of a select list for on a file
* Copyright (C) 2000 Christophe Marchal 
* mccricri@jd3.sourceforges.com
* http://jd3.sourceforge.com/ 
* 
* This program is free software; you can redistribute it and/or 
* modify it under the terms of the GNU General Public License 
* as published by the Free Software Foundation; either version 2 
* of the License, or any later version. 
* 
* This program is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of 
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU General Public License for more details. 
* 
* You should have received a copy of the GNU General Public License 
* along with this program; if not, write to the Free Software 
* Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. 
*/
namespace org.jd3
{
	using System;
	using org.jd3;
	using org.jd3.exceptions;
	
	/// <summary> The implmentation of a select list for on a file
	/// Ex: in pickbasic
	/// open "file" to ff
	/// select ff
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public class D3SelectListFile : D3SelectList
	{
		public virtual System.String NextElement
		{
			get
			{
				System.String result = null;
				// See if there is a key
				if (hasMoreElements())
				{
					result = this.curkey;
					this.curkey = null;
				}
				return result;
			}
			
		}
		public virtual int NbElements
		{
			get
			{
				return this.nbelements;
			}
			
		}
		
		/// <summary> The connection to the D3 server
		/// </summary>
		private D3Connection con;
		
		/// <summary> the select descriptor used by the server
		/// </summary>
		private System.String select;
		
		/// <summary> the current key read from the server
		/// </summary>
		private System.String curkey = null;
		
		/// <summary> State of the select list, only use to be more efficiency (no network communication if 
		/// the select list are completely read) 
		/// </summary>
		private bool finished = false;
		
		/// <summary> The number of element in the select list
		/// </summary>
		private int nbelements = 0;
		
		
		/// <summary> Create a select list from a file descriptor
		/// </summary>
		/// <param name="pcon">The D3 connection used to get the key of the select list
		/// </param>
		/// <param name="pselect">The select descriptor use by the D3 server to manage the select list
		/// </param>
		/// <param name="nbelement">The number of element in the select list
		/// 
		/// </param>
		public D3SelectListFile(D3Connection pcon, System.String pselect, int nbelement)
		{
			this.con = pcon;
			this.select = pselect;
			this.nbelements = nbelements;
		}
		
		
		/// <summary> Verify if there is one more key in the select list
		/// </summary>
		/// <returns>true if there is one more key in the select list
		/// false if not
		/// 
		/// </returns>
		public virtual bool hasMoreElements()
		{
			// If there is no key, we read it from the select list
			if (curkey == null)
			{
				return readNext();
			}
			// We have a key
			return true;
		}
		
		
		/// <summary> Get the next key in the select list
		/// </summary>
		/// <returns>The next key in the select list, null if there is no more key
		/// 
		/// </returns>
		
		
		/// <summary> Read the next key from the select list on the server
		/// </summary>
		/// <returns>True if we succeed in reading next key
		/// 
		/// </returns>
		private bool readNext()
		{
			
			// If we have already a key in the buffer
			if (curkey != null)
				return true;
			
			// If we have already finished the select list
			if (finished)
				return false;
			
			try
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_READNEXT + "\x0001" + select + "\x0001");
				
				// If it is the end of the list
				if (buf.Status == D3Constants.READNEXT_EOF)
				{
					finished = true;
					return false;
				}
				else
				{
					// If there was another error
					if (buf.Status != D3Constants.D3_OK)
					{
						return false;
					}
				}
				
				curkey = (System.String) buf.Data[0];
				return true;
				
			}
			catch
			{
				return false;
			}
			
		}
		
		/// <summary> Get the number of elements selected in the list
		/// </summary>
		/// <returns>The number of elements selected in the list
		/// 
		/// </returns>
	}
}
