/* 
* D3Session.java - Interface of the session definition, use for basic operation on the server
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
	
	/// <summary> Interface of the session definition, use for basic operation on the server
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public interface D3Session : D3Component
		{
			/// <summary> This function do nothing, just send instruction and receive a response.
			/// Use it to know the communication is OK
			/// *
			/// </summary>
			/// <returns>the status of command. <li> D3_OK if the communication is OK <li> D3_ERR if not
			/// 
			/// </returns>
			int noop();
			/// <summary> Call a basic subroutine
			/// </summary>
			/// <param name="name">The name of the subroutine <li>mysub : must be cataloged in the account of the server 
			/// <li>myaccount,myfile, mysub : the absolute path to the subroutine
			/// </param>
			/// <param name="parms">The parameters of the subroutine
			/// 
			/// </param>
			int call(System.String name, D3Params parms);
			/// <summary> Execute a TCL command
			/// </summary>
			/// <param name="sentence">The TCL command to execute
			/// 
			/// </param>
			System.String execute(System.String sentence);
			/// <summary> Execute a select at the TCL and return the active list.
			/// </summary>
			/// <param name="sentence">The AQL command to select key from file
			/// 
			/// </param>
			D3SelectList select(System.String sentence);
			/// <summary> Open D3 File in the account you run the server
			/// </summary>
			/// <param name="pfilename">The file name to open.
			/// The file must be in the account where run the server, or as a Q-pointer
			/// 
			/// </param>
			D3File openFile(System.String pfilename);
			/// <summary> Open a D3 File in an account name
			/// </summary>
			/// <param name="paccountname">The name of the account where is store the d3 file
			/// </param>
			/// <param name="pfilename">The name of the file to open, must be in the account (or as a q-pointer)
			/// 
			/// </param>
			D3File openFile(System.String paccountname, System.String pfilename);
		}
}