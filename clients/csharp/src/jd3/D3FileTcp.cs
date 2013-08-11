/* 
* D3FileTcp.java - The implementation of a D3File who use a TCP/IP socket to communicate with the D3 server
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
	
	/// <summary> The implementation of a D3File who use a TCP/IP socket to communicate with the D3 server
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public class D3FileTcp : D3File
	{
		public virtual System.String FileName
		{
			get
			{
				return this.filename;
			}
			
		}
		public virtual System.String AccountName
		{
			get
			{
				return this.accountname;
			}
			
		}
		
		/// <summary> The connection to the D3 server
		/// </summary>
		private D3Connection con;
		
		/// <summary> the file descriptor
		/// </summary>
		private int fd;
		
		/// <summary> The name of the file
		/// </summary>
		private System.String filename;
		
		/// <summary> The name of the account where is the file
		/// </summary>
		private System.String accountname;
		
		/// <summary> Open a file store on the D3 server
		/// </summary>
		/// <param name="pcon">The conneciton to the server
		/// </param>
		/// <param name="paccount">The account name where is store the file
		/// </param>
		/// <param name="pfilename">The name of the file to open
		/// 
		/// </param>
		public D3FileTcp(D3Connection pcon, System.String paccount, System.String pfilename)
		{
			D3Buffer buf;
			this.con = pcon;
			fd = 0;
			filename = pfilename;
			accountname = paccount;
			
			buf = this.con.doit(D3Constants._D3_OPEN + "\x0001" + paccount + "\x0001" + pfilename + "\x0001");
			if (buf.Status != D3Constants.D3_OK)
			{
				throw new D3Exception("Erreur d'ouverture de " + paccount + "," + pfilename + ",");
			}
			
			try
			{
				fd = System.Int32.Parse((System.String) buf.Data[0]);
			}
			catch (System.FormatException e)
			{
				throw new D3Exception("Erreur d'ouverture de " + paccount + "," + pfilename + "," + e.ToString());
			}
		}
		
		/// <summary> Open a file store on the D3 server using the account where the server is running
		/// </summary>
		/// <param name="pcon">The conneciton to the server
		/// </param>
		/// <param name="pfilename">The name of the file to open
		/// 
		/// </param>
		public D3FileTcp(D3Connection pcon, System.String pfilename):this(pcon, "", pfilename)
		{
		}
		
		/// <summary> Get the name of the file
		/// </summary>
		/// <returns>The name of the file
		/// 
		/// </returns>
		
		/// <summary> Get the name of the accoutn where is the file
		/// </summary>
		/// <returns>The name of the account where is the file
		/// 
		/// </returns>
		
		/// <summary> Lecture d'un record
		/// </summary>
		/// <param name="key">la cl
		/// </param>
		/// <param name="PItem">le record  mettre  jour, doit tre cr avant l'appel (ventuellement vide)
		/// </param>
		/// <returns>rsultat de la lecture
		/// READ_FILENOTOPEN   Le fichier n'est pas ouvert, le record est mis  vide
		/// READ_RECORDEMPTY   le record n'existe pas pour la cl demande, il est mis  vide
		/// READ_RECORDLOCKED  le record est dj lock, dans ce cas le record contient
		/// en <1> : le numro de port qui lock
		/// en <2> : le code user qui lock (si il est connu)
		/// D3_OK              si tout va bien, le record est mis  jour avec les infos
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'read'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual int read(System.String key, D3Item PItem)
		{
			lock(this)
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_READ + "\x0001" + fd + "\x0001" + key + "\x0001");
				PItem.Record = (System.String) buf.Data[0];
				return buf.Status;
			}
		}
		
		/// <summary> Lecture d'un attribut d'un record
		/// </summary>
		/// <param name="key"> la cl du record  lire
		/// </param>
		/// <param name="am">  la position de l'attribut  lire
		/// </param>
		/// <returns> String la valeur de l'attribut lu, vide s'il y a eu un problme ou s'il est vide
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'readv'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual System.String readv(System.String key, int am)
		{
			lock(this)
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_READV + "\x0001" + fd + "\x0001" + key + "\x0001" + System.Convert.ToString(am) + "\x0001");
				if (buf.Status == D3Constants.D3_OK)
				{
					return (System.String) buf.Data[0];
				}
				else
				{
					return "";
				}
			}
		}
		
		/// <summary> Lecture et lock d'un record
		/// </summary>
		/// <param name="key">la cl
		/// </param>
		/// <param name="PItem">le record  mettre  jour, doit tre cr avant l'appel (ventuellement vide)
		/// </param>
		/// <returns>rsultat de la lecture
		/// READ_FILENOTOPEN   Le fichier n'est pas ouvert, le record est mis  vide
		/// READ_RECORDEMPTY   le record n'existe pas pour la cl demande, il est mis  vide
		/// READ_RECORDLOCKED  le record est dj lock, dans ce cas le record contient
		/// en <1> : le numro de port qui lock
		/// en <2> : le code user qui lock (si il est connu)
		/// D3_OK              si tout va bien, le record est mis  jour avec les infos
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'readu'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual int readu(System.String key, D3Item PItem)
		{
			lock(this)
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_READU + "\x0001" + fd + "\x0001" + key + "\x0001");
				if (buf.Status == D3Constants.D3_OK)
				{
					PItem.Record = (System.String) buf.Data[0];
				}
				return buf.Status;
			}
		}
		
		/// <summary> Ecrire un record
		/// </summary>
		/// <param name="key">la cl
		/// </param>
		/// <param name="PItem">le record
		/// </param>
		/// <returns>int rsultat de l'criture
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'write'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual int write(System.String key, D3Item PItem)
		{
			lock(this)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
				D3Buffer buf = this.con.doit(D3Constants._D3_WRITE + "\x0001" + fd + "\x0001" + key + "\x0001" + PItem.ToString() + "\x0001");
				return buf.Status;
			}
		}
		
		/// <summary> Effacer un record
		/// </summary>
		/// <param name="key">la cl
		/// </param>
		/// <returns>int rsultat du delete
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'delete'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual int delete(System.String key)
		{
			lock(this)
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_DELETE + "\x0001" + fd + "\x0001" + key + "\x0001");
				return buf.Status;
			}
		}
		
		/// <summary> Tester l'existance d'un record dans le fichier
		/// </summary>
		/// <param name="key">la cl
		/// </param>
		/// <returns>boolean false si le record n'existe pas ou s'il y a eu un problme
		/// true  si le record existe
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'testExist'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual bool testExist(System.String key)
		{
			lock(this)
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_TEST_EXIST + "\x0001" + fd + "\x0001" + key + "\x0001");
				if (buf.Status == D3Constants.D3_OK)
				{
					return ((System.String) buf.Data[0]).Equals("0");
				}
				else
				{
					return false;
				}
			}
		}
		
		/// <summary> Relacher un verrou
		/// </summary>
		/// <param name="key">la cl du verrou  lacher
		/// </param>
		/// <returns>int rsultat de la commande
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'release'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual int release(System.String key)
		{
			lock(this)
			{
				D3Buffer buf = this.con.doit(D3Constants._D3_RELEASE + "\x0001" + fd + "\x0001" + key + "\x0001");
				return buf.Status;
			}
		}
		
		/// <summary> Effectue un select sur un fichier
		/// </summary>
		/// <returns>int rsultat de la commande
		/// 
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'select'. Lock expression was added. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual D3SelectList select()
		{
			lock(this)
			{
				int nb = 0;
				D3Buffer buf = this.con.doit(D3Constants._D3_SELECT + "\x0001" + fd + "\x0001");
				nb = System.Int32.Parse((System.String) buf.Data[1]);
				return new D3SelectListFile(this.con, (System.String) buf.Data[0], nb);
			}
		}
	}
}
