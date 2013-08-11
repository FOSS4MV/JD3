/* 
* D3Exception.java - The exception class for jd3
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
/// <summary> The exception class for jd3
/// </summary>
/// <author> Christophe Marchal
/// </author>
/// <version> 1.0
/// 
/// </version>
namespace org.jd3.exceptions
{
	using System;
	
	public class D3Exception:System.Exception
	{
		public virtual int ErrorNo
		{
			get
			{
				return this.errorno;
			}
			
		}
		
		private int errorno = 0;
		
		public D3Exception(int perrorno, System.String pmsg):base(pmsg)
		{
			this.errorno = perrorno;
		}
		
		public D3Exception(System.String pmsg):this(0, pmsg)
		{
		}
		
		public D3Exception():this(0, "Unknow error")
		{
		}
		
		
		/// <summary> print error message with error number
		/// *
		/// </summary>
		public override System.String ToString()
		{
			System.Text.StringBuilder tmp = new System.Text.StringBuilder("[");
			tmp.Append(ErrorNo);
			tmp.Append("] ");
			tmp.Append(Message);
			return tmp.ToString();
		}
	}
}