/* 
* D3File.java - The interface of the Java representation for a D3 File
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
	
	/// <summary> The interface of the Java representation for a D3 File which define 
	/// basic operation that could be done with a file
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 0.8
	/// 
	/// </version>
	public interface D3File : D3Component
		{
			/// <summary> Lecture d'un record
			/// </summary>
			/// <param name="key">la cl�
			/// </param>
			/// <param name="PItem">le record � mettre � jour, doit �tre cr�� avant l'appel (�ventuellement vide)
			/// </param>
			/// <returns>r�sultat de la lecture
			/// READ_FILENOTOPEN   Le fichier n'est pas ouvert, le record est mis � vide
			/// READ_RECORDEMPTY   le record n'existe pas pour la cl� demand�e, il est mis � vide
			/// READ_RECORDLOCKED  le record est d�j� lock�, dans ce cas le record contient
			/// en <1> : le num�ro de port qui lock
			/// en <2> : le code user qui lock (si il est connu)
			/// D3_OK              si tout va bien, le record est mis � jour avec les infos
			/// 
			/// </returns>
			int read(System.String key, D3Item pitem);
			/// <summary> Lecture d'un attribut d'un record
			/// </summary>
			/// <param name="key"> la cl� du record � lire
			/// </param>
			/// <param name="am">  la position de l'attribut � lire
			/// </param>
			/// <returns> String la valeur de l'attribut lu, vide s'il y a eu un probl�me ou s'il est vide
			/// 
			/// </returns>
			System.String readv(System.String key, int am);
			/// <summary> Lecture et lock d'un record
			/// </summary>
			/// <param name="key">la cl�
			/// </param>
			/// <param name="PItem">le record � mettre � jour, doit �tre cr�� avant l'appel (�ventuellement vide)
			/// </param>
			/// <returns>r�sultat de la lecture
			/// READ_FILENOTOPEN   Le fichier n'est pas ouvert, le record est mis � vide
			/// READ_RECORDEMPTY   le record n'existe pas pour la cl� demand�e, il est mis � vide
			/// READ_RECORDLOCKED  le record est d�j� lock�, dans ce cas le record contient
			/// en <1> : le num�ro de port qui lock
			/// en <2> : le code user qui lock (si il est connu)
			/// D3_OK              si tout va bien, le record est mis � jour avec les infos
			/// 
			/// </returns>
			int readu(System.String key, D3Item PItem);
			/// <summary> Ecrire un record
			/// </summary>
			/// <param name="key">la cl�
			/// </param>
			/// <param name="PItem">le record
			/// </param>
			/// <returns>int r�sultat de l'�criture
			/// 
			/// </returns>
			int write(System.String key, D3Item PItem);
			/// <summary> Effacer un record
			/// </summary>
			/// <param name="key">la cl�
			/// </param>
			/// <returns>int r�sultat du delete
			/// 
			/// </returns>
			int delete(System.String key);
			/// <summary> Tester l'existance d'un record dans le fichier
			/// </summary>
			/// <param name="key">la cl�
			/// </param>
			/// <returns>boolean false si le record n'existe pas ou s'il y a eu un probl�me
			/// true  si le record existe
			/// 
			/// </returns>
			bool testExist(System.String key);
			/// <summary> Relacher un verrou
			/// </summary>
			/// <param name="key">la cl� du verrou � lacher
			/// </param>
			/// <returns>int r�sultat de la commande
			/// 
			/// </returns>
			int release(System.String key);
			/// <summary> Effectue un select sur un fichier
			/// </summary>
			/// <returns>int r�sultat de la commande
			/// 
			/// </returns>
			D3SelectList select();
		}
}