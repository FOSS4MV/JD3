/* 
* D3SessionTcp.java - Implementation of the D3Session who uses a TCP/IP socket
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
	using org.jd3.utils;
	using org.jd3.exceptions;
	
	/// <summary> Implementation of the D3Session who uses a TCP/IP socket
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public class D3SessionTcp : D3Session
	{
		
		/// <summary>  The D3Connection used to communicate with the D3 server
		/// </summary>
		private D3Connection myconnection;
		
		
		/// <summary> Create a D3Session who use a TCP/IP socket to cummunicate with the server
		/// </summary>
		public D3SessionTcp(D3Connection pcon)
		{
			myconnection = pcon;
		}
		
		
		/// <summary> this function do nothing, just send instruction and receive a response.
		/// Use it to know the communication is OK
		/// *
		/// </summary>
		/// <returns>the status of command. <li> D3_OK if the communication is OK <li> D3_ERR if not
		/// 
		/// </returns>
		public virtual int noop()
		{
			D3Buffer buf = this.myconnection.doit(D3Constants._D3_NOOP + "\x0001");
			return buf.Status;
		}
		
		/// <summary> Call a basic subroutine
		/// </summary>
		/// <param name="name">The name of the subroutine <li>mysub : must be cataloged in the account of the server 
		/// <li>myaccount,myfile, mysub : the absolute path to the subroutine
		/// </param>
		/// <param name="parms">The parameters of the subroutine
		/// 
		/// </param>
		public virtual int call(System.String name, D3Params parms)
		{
			int nargs, i, nretour;
			// Use StringBuffer because it is more efficient and there must be lot of parameter
			System.Text.StringBuilder buffer;
			D3Buffer buf;
			
			// construire la phrase pour le call avec les paramtres
			nargs = parms.Size;
			if (nargs < 0)
			{
				return D3Constants.D3_ERR;
			}
			
			buffer = new System.Text.StringBuilder("");
			buffer.Append(D3Constants._D3_CALL);
			buffer.Append("\x0001");
			buffer.Append(name);
			buffer.Append("\x0001");
			buffer.Append(nargs);
			buffer.Append("\x0001");
			 for (i = 0; i < nargs; i++)
			{
				buffer.Append((System.String) parms.getParam(i));
				buffer.Append("\x0001");
			}
			
			// envoit de la demande
			buf = this.myconnection.doit(buffer.ToString());
			if (buf.Status != D3Constants.D3_OK)
			{
				return D3Constants.D3_ERR;
			}
			
			// garnir pour le retour
			nretour = buf.Data.Count;
			 for (i = 0; i < nretour && i < nargs; i++)
			{
				parms.setParam((System.String) buf.Data[i], i);
			}
			
			return D3Constants.D3_OK;
		}
		
		/// <summary> Execute a TCL command
		/// </summary>
		/// <param name="sentence">The TCL command to execute
		/// 
		/// </param>
		public virtual System.String execute(System.String sentence)
		{
			D3Buffer buf = this.myconnection.doit(D3Constants._D3_EXECUTE + "\x0001" + sentence + "\x0001");
			if (buf.Status != D3Constants.D3_OK)
			{
				throw new D3Exception("Erreur d' excution de " + sentence);
			}
			return ((System.String) buf.Data[0]);
		}
		
		/// <summary> Execute a select at the TCL and return the active list.
		/// </summary>
		/// <param name="sentence">The AQL command to select key from file
		/// 
		/// </param>
		public virtual D3SelectList select(System.String sentence)
		{
			D3Item list;
			System.String[] result;
			D3Buffer buf = this.myconnection.doit(D3Constants._D3_SELECT_TCL + "\x0001" + sentence + "\x0001");
			
			if (buf.Status != D3Constants.D3_OK)
			{
				throw new D3Exception("Erreur d' excution de " + sentence);
			}
			
			// first data is number of key selected
			int nbelement = System.Int32.Parse((System.String) buf.Data[0]);
			result = new System.String[nbelement];
			if (nbelement > 0)
			{
				// second data is list of key separated by @AM
				// We use D3Item, which is more easy to use ;)
				list = new D3Item((System.String) buf.Data[1]);
				
				 for (int i = 0; i <= (nbelement - 1); i++)
				{
					result[i] = list.extract(i + 1);
				}
			}
			return new D3SelectListTcl(result);
		}
		
		
		/// <summary> Open D3 File in the account you run the server
		/// </summary>
		/// <param name="pfilename">The file name to open.
		/// The file must be in the account where run the server, or as a Q-pointer
		/// 
		/// </param>
		public virtual D3File openFile(System.String pfilename)
		{
			return new D3FileTcp(this.myconnection, "", pfilename);
		}
		
		/// <summary> Open a D3 File in an account name
		/// </summary>
		/// <param name="paccountname">The name of the account where is store the d3 file
		/// </param>
		/// <param name="pfilename">The name of the file to open, must be in the account (or as a q-pointer)
		/// 
		/// </param>
		public virtual D3File openFile(System.String paccountname, System.String pfilename)
		{
			return new D3FileTcp(this.myconnection, paccountname, pfilename);
		}
	}
}
