/* 
* D3Item.java - This is the Java representation of a D3 record
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
	using org.jd3.utils;
	using org.jd3.exceptions;
	
	/// <summary> This is the Java representation of a D3 record
	/// with the Vector class, it can simulate a dynamic structure of data
	/// </summary>
	/// <author> Christophe Marchal
	/// </author>
	/// <version> 1.0
	/// 
	/// </version>
	[Serializable]
	public class D3Item : D3Component, System.Runtime.Serialization.ISerializable
	{
		public virtual System.String Record
		{
			set
			{
				System.Collections.ArrayList anAttribute, aValue;
				int alength, vlength, svlength;
				System.String vbuf, svbuf;
				int nbam, nbvm, nbsvm, l;
				int a, aa, v, vv, s, ss;
				
				if (Item != null)
				{
					Item.Clear();
					
				}
				else
				{
					Item = new System.Collections.ArrayList();
				}
				
				value += "\x0001"; // make life easy
				alength = value.Length;
				nbam = 0;
				// Boucler sur tous les caractres de la chaine
				 for (a = 0, aa = 0; aa < alength; aa++)
				{
					// Si j'ai un sparateur AM ou la fin de ligne
					if (value[aa] == '\u00fe' || value[aa] == '\x0001')
					{
						// J'ai un nouvel attribut
						anAttribute = new System.Collections.ArrayList();
						Item.Add(anAttribute);
						
						// chaine des multi-valeurs
						vbuf = new System.String((value.Substring(a, (aa) - (a)) + "\x0001").ToCharArray()); // make life easy
						vlength = vbuf.Length;
						
						nbvm = 0;
						// Boucle sur tous les caratres de la MV
						 for (vv = 0, v = 0; vv < vlength; vv++)
						{
							// Si j'ai un sparateur de VM ou fin de ligne
							if (vbuf[vv] == '\u00fe' || vbuf[vv] == '\u00fd' || vbuf[vv] == '\x0001')
							{
								// On a une multi valeur
								aValue = new System.Collections.ArrayList();
								svbuf = new System.String((vbuf.Substring(v, (vv) - (v)) + "\x0001").ToCharArray()); // make life easy
								svlength = svbuf.Length;
								
								nbsvm = 0;
								s = 0;
								// Boucle sur chaque caractres de la MV pour les SVM
								 for (s = 0, ss = 0; ss < svlength; ss++)
								{
									// Si on a sparateur de SVM
									if (svbuf[ss] == '\u00fe' || svbuf[ss] == '\u00fd' || svbuf[ss] == '\u00fc' || svbuf[ss] == '\x0001')
									{
										// On a une SVM
										System.String buf = new System.String(svbuf.Substring(s, (ss) - (s)).ToCharArray());
										// On la rajoute dans le tableau si ele est pas vide ou si c'est pas la premire
										if (!buf.Equals("") || nbsvm > 0)
										{
											// Si c'est pas la premire mais que le tableau est vide alors c'est qu'il commance par des vides
											if (nbsvm > 0 && aValue.Count == 0)
											{
												 for (l = 0; l < nbsvm; l++)
												{
													aValue.Add("");
												}
											}
											// On ajoute la sous-valeurs
											aValue.Add(buf);
										}
										
										s = ss + 1;
										nbsvm++; // Compte le nombre de sous-valeurs traites
									}
								}
								// kk
								v = vv + 1;
								// Si c'est pas la premire ou que qu'il y a des sous-valeurs
								if (aValue.Count > 0 || nbvm > 0)
								{
									// Si il y a dj des VM mais que le tableau est vide alors il commence par des vides
									if (nbvm > 0 && anAttribute.Count == 0)
									{
										 for (l = 0; l < nbvm; l++)
										{
											anAttribute.Add(new System.Collections.ArrayList());
										}
									}
									// Ajout du tableau des sous-valeurs
									anAttribute.Add(aValue);
								}
								// Compte le nombre de VM
								nbvm++;
							}
						}
						// jj
						a = aa + 1;
						// Compte le nombre d'attribut
						nbam++;
					}
				}
				//ii
				
			}
			
		}
		
		private System.Collections.ArrayList Item;
		
		public D3Item(System.String item)
		{
			Record = item;
		}
		
		public D3Item():this("")
		{
		}
		
		
		/// <summary> Extract <attr, value, subvalue> from a D3Item as a String
		/// </summary>
		/// <param name="attr">    attribute number
		/// </param>
		/// <param name="value">   value number
		/// </param>
		/// <param name="svalue">  subvalue number
		/// 
		/// </param>
		public virtual System.String extract(int attr, int value_Renamed, int svalue)
		{
			System.Collections.ArrayList anAttribute, aValue;
			System.String Result;
			
			if (attr == 0)
			{
				Result = new System.String("".ToCharArray());
				return (Result);
			}
			
			if (value_Renamed == 0)
				svalue = 0;
			
			if (attr > Item.Count)
			{
				Result = new System.String("".ToCharArray());
				return (Result);
			}
			
			anAttribute = (System.Collections.ArrayList) Item[attr - 1];
			
			if (value_Renamed == 0)
			{
				// return ALL values
				Result = new System.String("".ToCharArray());
				int n = anAttribute.Count;
				 for (int i = 0; i < n; i++)
				{
					aValue = (System.Collections.ArrayList) anAttribute[i];
					int m = aValue.Count;
					 for (int j = 0; j < m; j++)
					{
						Result = Result + (System.String) aValue[j];
						if (j != m - 1)
						{
							Result = Result + "\u00fc";
						}
					}
					if (i != n - 1)
					{
						Result = Result + "\u00fd";
					}
				}
				return (Result);
			}
			
			if (value_Renamed > anAttribute.Count)
			{
				Result = new System.String("".ToCharArray());
				return (Result);
			}
			
			aValue = (System.Collections.ArrayList) anAttribute[value_Renamed - 1];
			
			if (svalue == 0)
			{
				// return ALL subvalues
				Result = new System.String("".ToCharArray());
				int n = aValue.Count;
				 for (int i = 0; i < n; i++)
				{
					Result = Result + (System.String) aValue[i];
					if (i != n - 1)
					{
						Result = Result + "\u00fc";
					}
				}
				return (Result);
			}
			
			if (svalue > aValue.Count)
			{
				Result = new System.String("".ToCharArray());
				return (Result);
			}
			
			Result = new System.String(((System.String) aValue[svalue - 1]).ToCharArray());
			return (Result);
		}
		
		public virtual System.String extract(int attr, int value_Renamed)
		{
			return (extract(attr, value_Renamed, 0));
		}
		
		public virtual System.String extract(int attr)
		{
			return (extract(attr, 0, 0));
		}

        // D3 VB ODBC equivalents...
        public virtual System.String brExtractStr(int attr, int value_Renamed, int svalue) {
            return(extract(attr, value_Renamed, svalue));
        }

        public virtual System.String brExtractStr(int attr, int value_Renamed) {
            return (extract(attr, value_Renamed));
        }

        public virtual System.String brExtractStr(int attr) {
            return (extract(attr));
        }

		/// <summary> replace a string in a <attr, value, subvalue>
		/// </summary>
		/// <param name="attr">    attribute number
		/// </param>
		/// <param name="value">   value number
		/// </param>
		/// <param name="svalue">  subvalue number
		/// </param>
		/// <param name="newData"> new string to replace
		/// 
		/// </param>
		public virtual void  replace(int attr, int value_Renamed, int svalue, System.String newData)
		{
			System.Collections.ArrayList anAttribute, aValue;
			
			if (attr == 0)
				return ;
			if (newData == null)
				return ;
			if (value_Renamed == 0)
				svalue = 0;
			if (svalue == 0)
				svalue = 1;
			
			if (attr > Item.Count)
			{
				int j = Item.Count;
				 for (int i = j; i < attr; i++)
				{
					anAttribute = new System.Collections.ArrayList();
					aValue = new System.Collections.ArrayList();
					Item.Add(anAttribute);
					anAttribute.Add(aValue);
					aValue.Add("");
				}
			}
			
			anAttribute = (System.Collections.ArrayList) Item[attr - 1];
			
			if (value_Renamed == 0)
			{
				aValue = new System.Collections.ArrayList();
				anAttribute[0] = aValue;
				aValue.Add(newData);
			}
			else
			{
				if (value_Renamed > anAttribute.Count)
				{
					int j = anAttribute.Count;
					 for (int i = j; i < value_Renamed; i++)
					{
						aValue = new System.Collections.ArrayList();
						anAttribute.Add(aValue);
						aValue.Add("");
					}
				}
				// add in the data
				aValue = (System.Collections.ArrayList) anAttribute[value_Renamed - 1];
				if (svalue > aValue.Count)
				{
					int j = aValue.Count;
					 for (int i = j; i < svalue; i++)
					{
						aValue.Add("");
					}
				}
				aValue[svalue - 1] = newData;
			}
		}
		
		public virtual void  replace(int attr, int value_Renamed, System.String newData)
		{
			replace(attr, value_Renamed, 0, newData);
		}
		
		public virtual void  replace(int attr, System.String newData)
		{
			replace(attr, 0, 0, newData);
		}

        // D3 VB ODBC equivalents
        public virtual void brReplaceStr(System.String newData, int attr, int value_Renamed, int svalue) {
            replace(attr, value_Renamed, svalue, newData);
        }

        public virtual void brReplaceStr(System.String newData, int attr, int value_Renamed) {
            replace(attr, value_Renamed, newData);
        }

        public virtual void brReplaceStr(System.String newData, int attr) {
            replace(attr, newData);
        }

		
		/// <summary> Insert a string at <attr, value, subvalue>
		/// </summary>
		/// <param name="attr">    attribute number
		/// </param>
		/// <param name="value">   value number
		/// </param>
		/// <param name="svalue">  subvalue number
		/// </param>
		/// <param name="newData"> new String to insert
		/// 
		/// </param>
		public virtual void  insert(int attr, int value_Renamed, int svalue, System.String newData)
		{
			System.Collections.ArrayList anAttribute, aValue;
			
			if (attr == 0)
			{
				anAttribute = new System.Collections.ArrayList();
				aValue = new System.Collections.ArrayList();
				Item.Insert(0, anAttribute);
				anAttribute.Insert(0, aValue);
				aValue.Add(newData);
				return ;
			}
			
			if (value_Renamed == 0)
				svalue = 0;
			
			if (attr > Item.Count)
			{
				int j = Item.Count;
				 for (int i = j; i < attr; i++)
				{
					anAttribute = new System.Collections.ArrayList();
					aValue = new System.Collections.ArrayList();
					Item.Add(anAttribute);
					anAttribute.Add(aValue);
					aValue.Add("");
				}
			}
			
			anAttribute = (System.Collections.ArrayList) Item[attr - 1];
			
			if (value_Renamed == 0)
			{
				aValue = new System.Collections.ArrayList();
				anAttribute.Insert(0, aValue);
				aValue.Add(newData);
			}
			else
			{
				if (value_Renamed > anAttribute.Count)
				{
					int j = anAttribute.Count;
					 for (int i = j; i < value_Renamed; i++)
					{
						aValue = new System.Collections.ArrayList();
						anAttribute.Add(aValue);
						aValue.Add("");
					}
				}
				// add in the data
				aValue = (System.Collections.ArrayList) anAttribute[value_Renamed - 1];
				if (svalue > aValue.Count)
				{
					int j = aValue.Count;
					 for (int i = j; i < svalue; i++)
					{
						aValue.Add("");
					}
				}
				aValue.Insert(svalue - 1, newData);
			}
		}
		
		public virtual void  insert(int attr, int value_Renamed, System.String newData)
		{
			insert(attr, value_Renamed, 0, newData);
		}
		
		public virtual void  insert(int attr, System.String newData)
		{
			insert(attr, 0, 0, newData);
		}
		
        // D3 VB ODBC equivalents

        public virtual void brInsertStr(System.String newData, int attr, int value_Renamed, int svalue) {
            insert(attr, value_Renamed, svalue, newData);
        }

        public virtual void brInsertStr(System.String newData, int attr, int value_Renamed) {
            insert(attr, value_Renamed, newData);
        }

        public virtual void brInsertStr(System.String newData,int attr) {
            insert(attr, newData);
        }

		/// <summary> delete a string in a <attr, value, subvalue>
		/// </summary>
		/// <param name="attr">    attribute number
		/// </param>
		/// <param name="value">   value number
		/// </param>
		/// <param name="svalue">  subvalue number
		/// 
		/// </param>
		public virtual void  delete(int attr, int value_Renamed, int svalue)
		{
			System.Collections.ArrayList anAttribute, aValue;
			
			if (attr == 0)
				return ;
			if (value_Renamed == 0)
				svalue = 0;
			
			if (attr > Item.Count)
				return ;
			
			if (value_Renamed == 0)
			{
				Item.RemoveAt(attr - 1);
				return ;
			}
			anAttribute = (System.Collections.ArrayList) Item[attr - 1];
			if (value_Renamed > anAttribute.Count)
				return ;
			
			if (svalue == 0)
			{
				anAttribute.RemoveAt(value_Renamed - 1);
				return ;
			}
			aValue = (System.Collections.ArrayList) anAttribute[value_Renamed - 1];
			if (svalue > aValue.Count)
				return ;
			
			aValue.RemoveAt(svalue - 1);
		}
		
		public virtual void  delete(int attr, int value_Renamed)
		{
			delete(attr, value_Renamed, 0);
		}
		
		public virtual void  delete(int attr)
		{
			delete(attr, 0, 0);
		}
		
        // D3 VB ODBC equivalents..

        public virtual void brDeleteStr(int attr, int value_Renamed, int svalue) {
            delete(attr, value_Renamed, svalue);
        }

        public virtual void brDeleteStr(int attr, int value_Renamed) {
            delete(attr, value_Renamed);
        }

        public virtual void brDeleteStr(int attr) {
            delete(attr);
        }

		public override System.String ToString()
		{
			System.Collections.ArrayList anAttribute, aValue;
			System.String Result = new System.String("".ToCharArray());
			
			int n = Item.Count;
			 for (int i = 0; i < n; i++)
			{
				anAttribute = (System.Collections.ArrayList) Item[i];
				int m = anAttribute.Count;
				 for (int j = 0; j < m; j++)
				{
					aValue = (System.Collections.ArrayList) anAttribute[j];
					int l = aValue.Count;
					 for (int k = 0; k < l; k++)
					{
						Result = Result + (System.String) aValue[k];
						if (k != l - 1)
						{
							Result = Result + "\u00fc";
						}
					}
					if (j != m - 1)
					{
						Result = Result + "\u00fd";
					}
				}
				if (i != n - 1)
				{
					Result = Result + "\u00fe";
				}
			}
			return (Result);
		}
		
		
		public virtual int AMCount()
            // Returns the number of attribute marks in the item.
            // Equivalent to dcount(foo,@FM) (or @AM)
		{
			return (Item.Count);
		}
		
		public virtual int VMCount(int am)
            // am - The attribute mark to begin counting value marks on.
            // Equivalent to dcount(foo<am>,@VM)
		{
			System.Collections.ArrayList anAttribute;
			
			if (am > Item.Count)
				return 0;
			
			anAttribute = (System.Collections.ArrayList) Item[am - 1];
			return anAttribute.Count;
		}
		
		public virtual int SVMCount(int am, int vm)
            // am, vm - the position to begin counting sub-value marks on.
            // Equivalent to  dcount(foo<am,vm>,@SVM)
		{
			System.Collections.ArrayList anAttribute, anValue;
			
			if (am > Item.Count)
				return 0;
			
			anAttribute = (System.Collections.ArrayList) Item[am - 1];
			
			if (vm > anAttribute.Count)
				return 0;
			
			anValue = (System.Collections.ArrayList) anAttribute[vm - 1];
			return anValue.Count;
		}
		
		public virtual int dcount(System.String delim)
		{
			return (Item.Count);
		}
		
		public void GetObjectData(SerializationInfo info, StreamingContext context) 
		{
        	info.AddValue("MyClass_value", 1);
		}

	}
}
