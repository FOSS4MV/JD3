/* 
* D3Connection.java - The main class of jd3 who communicate with the D3 server
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
	using System.IO;
	using System.Net;
	using org.jd3.utils;
	using org.jd3.exceptions;
	
	/// <summary> The main class of jd3 who communicate with the D3 server
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public class D3Connection : D3Component
	{
		public virtual int Port
		{
			get
			{
				return this.port;
			}
			
		}
		public virtual System.String Server
		{
			get
			{
				return this.server;
			}
			
		}
		public virtual System.String UserName
		{
			get
			{
				return this.username;
			}
			
		}
		
		private System.String server;
		private System.String username;
		private int port;
		private int type;
		
//		private System.IO.BinaryWriter d3sOut = null;
		private System.IO.StreamWriter d3sOut = null;
		private System.IO.BinaryReader d3sIn = null;
		private System.Net.Sockets.TcpClient d3socket = null;
		
		
		/// <summary> Create a new Connection to a D3 server or Proxy
		/// </summary>
		/// <param name="pserver">the server name or IP adress
		/// </param>
		/// <param name="pport">the port number
		/// </param>
		/// <param name="pusername"> the name of the user to connect
		/// </param>
		/// <param name="ppassword">the password for the connection
		/// </param>
		/// <param name="ptype">the type of the connection <li> D3Constants.CONNECTION_TCP = connection directly to a D3 server
		/// <li> D3Constants.CONNECTION_TCP_PROXY = connection to a D3 proxy server (usual used for applet)
		/// <li> D3Constants.CONNECTION_RMI = connection to a D3 RMI Server
		/// *
		/// <B>Todo</B> try to secure the connection with login mandatory
		/// 
		/// </param>
		public D3Connection(System.String pserver, int pport, System.String pusername, System.String ppassword, int ptype)
		{
			this.server = pserver;
			this.port = pport;
			this.username = pusername;
			this.type = ptype;
			
			if (type != D3Constants.CONNECTION_TCP && type != D3Constants.CONNECTION_RMI && type != D3Constants.CONNECTION_TCP_PROXY)
			{
				throw new D3Exception("Type of connection unknow");
			}
			
			// Init connection to the server
			if (type == D3Constants.CONNECTION_RMI)
			{
				initRMI();
			}
			else
			{
				initTcp();
			}
			
			// login(username,password)
			
		}
		
		
		/// <summary> Get the port number of the connection
		/// </summary>
		/// <returns>The port number
		/// 
		/// </returns>
		
		
		/// <summary> Get the server name
		/// </summary>
		/// <returns>The server name
		/// 
		/// </returns>
		
		
		/// <summary> Get the user name of the connection
		/// </summary>
		/// <returns>The user name
		/// 
		/// </returns>
		
		
		/// <summary> Create a new session 
		/// </summary>
		/// <returns>the session of the good type depending of the type of connection
		/// <B>todo</B> the rmi session
		/// 
		/// </returns>
		public virtual D3Session createSession()
		{
			if (this.type == D3Constants.CONNECTION_TCP || this.type == D3Constants.CONNECTION_TCP_PROXY)
			{
				return new D3SessionTcp(this);
			}
			else
			{
				return null;
			}
		}
		
		
		/// <summary> Identify the user in the server
		/// </summary>
		/// <param name="user">The user name
		/// </param>
		/// <param name="password">The password of the user
		/// 
		/// </param>
		public virtual int login(System.String user, System.String password)
		{
			D3Buffer buf = doit(D3Constants._D3_LOGON + "\x0001" + user + "\x0001" + password + "\x0001");
			this.username = user;
			return buf.Status;
		}
		
		
		/// <summary> Logoff this user and close connection
		/// <B>Todo</B> close connection
		/// </summary>
		public virtual void  logoff(System.String user)
		{
			doit(D3Constants._D3_LOGOFF + "\x0001");
			close();
		}
		
		
		/// <summary> Send information to the server and wait for response
		/// </summary>
		/// <param name="cmd">Message in the correct format
		/// </param>
		/// <returns> Un D3Buffer contenant le rsultat de la commande
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'doit'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		protected internal virtual D3Buffer doit(System.String cmd)
		{
			lock(this)
			{
				D3Buffer tmp;
				
				if (this.type == D3Constants.CONNECTION_RMI)
				{
					throw new D3Exception("This is not a TCP connection");
				}
				
				this.send(cmd);
				tmp = this.receive();
				
				return tmp;
			}
		}
		
		
		/// <summary> Init the connection to a TCP server
		/// </summary>
		private void  initTcp()
		{
			byte[] buf;
			System.String sport;
			int newport = this.port;
			
			/*
			* If it is a direct connection to D3server we must ask for
			* a free port
			*/
			if (type == D3Constants.CONNECTION_TCP)
			{
				try
				{
					
					d3socket = new System.Net.Sockets.TcpClient(server, port);
					d3sIn = new System.IO.BinaryReader((System.IO.Stream) d3socket.GetStream());
					
					buf = new byte[8];
					//UPGRADE_TODO: Equivalent of method 'java.io.DataInputStream.readFully' may not have an optimal performance in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1117"'
					//SupportClass.ReadInput(d3sIn.BaseStream, ref buf, 0, 8);
					buf = d3sIn.ReadBytes(8);
					
					char[] tmpChar;
					tmpChar = new char[buf.Length];
					buf.CopyTo(tmpChar, 0);
					sport = new System.String(tmpChar, 0, 8);
					newport = System.Int32.Parse(sport);
					
					d3sIn.Close();
					d3socket.Close();
					
				}
				catch (System.Exception e)
				{
					throw new D3Exception("Erreur de connection " + e.Message);
				}
			}
			
			/*
			* then really connect to the client
			*/
			try
			{
				d3socket = new System.Net.Sockets.TcpClient(server, newport);
				//d3sOut = new System.IO.BinaryWriter((System.IO.Stream) d3socket.GetStream());
				d3sOut = new System.IO.StreamWriter((System.IO.Stream) d3socket.GetStream());
				d3sIn = new System.IO.BinaryReader((System.IO.Stream) d3socket.GetStream());
			}
			catch (System.Exception e)
			{
				throw new D3Exception("Erreur de connection " + e.Message);
			}
			
		}
		
		
		/// <summary> Init the connection to a RMI server
		/// <G>Todo</G> all
		/// </summary>
		private void  initRMI()
		{
			throw new D3Exception("Not yet implemented");
		}
		
		/// <summary> Close everything
		/// </summary>
		public virtual void  close()
		{
			try
			{
				d3sOut.Close();
				d3sIn.Close();
				d3socket.Close();
			}
			catch 
			{
			}
		}
		
		/// <summary> Send a message to the server
		/// </summary>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'send'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		private void  send(System.String outBuffer)
		{
			lock(this)
			{
				try
				{
					int i = outBuffer.Length;
					System.String slen = new D3Format("%08d").form(i);
					
					// Send all in one 
					//UPGRADE_ISSUE: Method 'java.io.DataOutputStream.writeBytes' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioDataOutputStreamwriteBytes_javalangString"'
					//d3sOut.writeBytes(slen + outBuffer);
					d3sOut.Write(slen + outBuffer);
					
					d3sOut.Flush();
					
				}
				catch (System.IO.IOException e)
				{
					throw new D3Exception("Can't send message to " + this.server + " {" + e.Message + "}");
				}
			}
		}
		
		
		/// <summary> Receive the response from the server
		/// Message format :
		/// 8 bytes = lenght of the following message
		/// block   = error no + "\001" + datas
		/// </summary>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'receive'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		private D3Buffer receive()
		{
			lock(this)
			{
				int length;
				byte[] buf;
				System.String stat;
				int status = D3Constants.D3_ERR;
				System.Collections.ArrayList inBuffer = new System.Collections.ArrayList();
				
				try
				{
					buf = new byte[8];
					// Reading the lenght
					//UPGRADE_TODO: Equivalent of method 'java.io.DataInputStream.readFully' may not have an optimal performance in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1117"'
					//SupportClass.ReadInput(d3sIn.BaseStream, ref buf, 0, 8);
					
					buf = d3sIn.ReadBytes(8);
					
					char[] tmpChar;
					tmpChar = new char[buf.Length];
					buf.CopyTo(tmpChar, 0);
					System.String slen = new System.String(tmpChar, 0, 8);
					length = System.Int32.Parse(slen);
					
					buf = new byte[length];
					// reading datas with this lenght
					//UPGRADE_TODO: Equivalent of method 'java.io.DataInputStream.readFully' may not have an optimal performance in C#. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1117"'
					//SupportClass.ReadInput(d3sIn.BaseStream, ref buf, 0, length);
					buf = d3sIn.ReadBytes(length);
				}
				catch (System.Exception e)
				{
					throw new D3Exception("Could not receive response from " + this.server + " {" + e.Message + "}");
				}
				
				// Converting into vector with "\001" as separator
				char[] tmpChar2;
				tmpChar2 = new char[buf.Length];
				buf.CopyTo(tmpChar2, 0);
				//SupportClass.Tokenizer st = new SupportClass.Tokenizer(new System.String(tmpChar2), "\x0001");
				
				char seps = '\x0001';
				String[] st = (new System.String(tmpChar2)).Split(seps);
				// Error no is always the first
				//stat = st.NextToken();
				int cnt = 0;
				stat = "";
				foreach (String line in st) 
				{
					if (cnt == 0){
						stat = line;
					} else
						{
							if (line == String.Empty)
							{
								inBuffer.Add("");
							}else
								{
									inBuffer.Add(line);
								}
						}
					cnt++;
					
  					if (line==String.Empty) continue;
				}
				
				/*
				if (st.HasMoreTokens())
				{
					do 
					{
						inBuffer.Add(st.NextToken());
					}
					while (st.HasMoreTokens());
				}
				else
				{
					// if there is no data, we must add an empty string
					inBuffer.Add("");
				}
				
				*/
				// get error code returned as first ASCII string
				try
				{
					status = System.Int32.Parse(stat);
				}
				catch
				{
					status = - 1;
				}
				
				return new D3Buffer(status, inBuffer);
			}
		}
	}
}
