/* 
* D3SelectListTcl.java - The implmentation of a select list create by an TCL command
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
	
	/// <summary> The implmentation of a select list create by an TCL command
	/// Ex : at TCL
	/// :SELECT myfile WITH A1 = "a]"
	/// 3 items selected
	/// >
	/// *
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public class D3SelectListTcl : D3SelectList
	{
		public virtual System.String NextElement
		{
			get
			{
				System.String result = null;
				if (hasMoreElements())
				{
					result = this.elements[curpos];
					curpos++;
				}
				return result;
			}
			
		}
		public virtual int NbElements
		{
			get
			{
				return elements.Length;
			}
			
		}
		
		
		/// <summary> String array which contains the key
		/// </summary>
		private System.String[] elements;
		
		
		/// <summary> The current position of the select list in the array
		/// </summary>
		private int curpos = 0;
		
		/// <summary> The limitation of the array, maybe I could use the length of this array
		/// </summary>
		private int maxpos = 0;
		
		
		/// <summary> Create a select list from the list of key return by the TCL command
		/// </summary>
		/// <param name="pelements">A string array which contains the key of the list
		/// 
		/// </param>
		public D3SelectListTcl(System.String[] pelements)
		{
			this.elements = pelements;
			if (pelements != null)
			{
				this.maxpos = this.elements.Length;
			}
		}
		
		/// <summary> Verify if there is one more key in the select list
		/// </summary>
		/// <returns>true if there is one more key in the select list
		/// false if not
		/// 
		/// </returns>
		public virtual bool hasMoreElements()
		{
			if (curpos < maxpos)
				return true;
			else
				return false;
		}
		
		/// <summary> Get the next key in the select list
		/// </summary>
		/// <returns>The next key in the select list, null if there is no more key
		/// 
		/// </returns>
		
		/// <summary> Get the number of elements selected in the list
		/// </summary>
		/// <returns>The number of elements selected in the list
		/// 
		/// </returns>
	}
}