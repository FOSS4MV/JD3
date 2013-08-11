/* 
* D3Params.java - Class to manage the parameters of a D3 Subroutine
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
	
	/// <summary> Class to manage the parameters of a D3 Subroutine
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public class D3Params
	{
		public virtual int Size
		{
			get
			{
				return mylist.Count;
			}
			
		}
		/// <summary> Used to manage parameters
		/// </summary>
		private System.Collections.ArrayList mylist;
		
		/// <summary> Initialize new D3 Parameters for subroutine
		/// </summary>
		public D3Params()
		{
			this.mylist = new System.Collections.ArrayList();
		}
		
		/// <summary> Get the number of parameter
		/// </summary>
		/// <returns>The number of the parameters
		/// 
		/// </returns>
		
		
		/// <summary> Getting value of a parameter
		/// </summary>
		/// <param name="pos">position of the parameter which we want the value
		/// </param>
		/// <returns>The value of the "posth" parameter 
		/// 
		/// </returns>
		public virtual System.String getParam(int pos)
		{
			return (System.String) mylist[pos];
		}
		
		/// <summary> Setting a value to an element
		/// </summary>
		/// <param name="par">the new value
		/// </param>
		/// <param name="pos">position of the parameter to set
		/// 
		/// </param>
		public virtual void  setParam(System.String par, int pos)
		{
			mylist[pos] = par;
		}
		
		/// <summary> Add one parameter to the list
		/// </summary>
		/// <param name="par">The value of the parameter
		/// 
		/// </param>
		public virtual void  addParam(System.String par)
		{
			mylist.Add(par);
		}
	}
}