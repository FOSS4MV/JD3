/* 
* D3SelectList.java - The interface of a D3 select list
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
	
	/// <summary> The interface of a D3 select list
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public interface D3SelectList
		{
			System.String NextElement
			{
				get;
				
			}
			int NbElements
			{
				get;
				
			}
			/// <summary> Verify if there is one more key in the select list
			/// </summary>
			/// <returns>true if there is one more key in the select list
			/// false if not
			/// 
			/// </returns>
			bool hasMoreElements();
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
