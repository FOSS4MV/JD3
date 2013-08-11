/* 
* D3Buffer.java - The message receive by the D3Connection when it speek with the D3 server
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
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	
	/// <summary> The message receive by the D3Connection when it speek with the D3 server
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	[Serializable]
	public class D3Buffer : System.Runtime.Serialization.ISerializable
	{
		public virtual int Status
		{
			get
			{
				return status;
			}
			
		}
		public virtual System.Collections.ArrayList Data
		{
			get
			{
				return buffer;
			}
			
		}
		
		private System.Collections.ArrayList buffer;
		private int status;
		
		/// <summary> Create a new buffer with status and datas
		/// </summary>
		/// <param name="stat">the status
		/// </param>
		/// <param name="info">the datas
		/// 
		/// </param>
		public D3Buffer(int stat, System.Collections.ArrayList info)
		{
			buffer = info;
			status = stat;
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context) 
		{
        	info.AddValue("MyClass_value", 2);
		}

		/// <summary> get the status
		/// </summary>
		
		/// <summary> get the data
		/// </summary>
	}
}
