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

package org.jd3;

import org.jd3.utils.*;
import org.jd3.exceptions.*;

 /**
 * The interface of the Java representation for a D3 File which define 
 * basic operation that could be done with a file
 * @author Christophe Marchal
 * @version 0.8
 */
public interface D3File extends D3Component{

  /**
  * Lecture d'un record
  * @param key la clé
  * @param PItem le record à mettre à jour, doit être créé avant l'appel (éventuellement vide)
  * @return résultat de la lecture
  *     READ_FILENOTOPEN   Le fichier n'est pas ouvert, le record est mis à vide
  *     READ_RECORDEMPTY   le record n'existe pas pour la clé demandée, il est mis à vide
  *     READ_RECORDLOCKED  le record est déjà locké, dans ce cas le record contient
  *                          en <1> : le numéro de port qui lock
  *                          en <2> : le code user qui lock (si il est connu)
  *     D3_OK              si tout va bien, le record est mis à jour avec les infos
  */
  public int read(String key,D3Item pitem) throws D3Exception;

  /**
  * Lecture d'un attribut d'un record
  * @param key  la clé du record à lire
  * @param am   la position de l'attribut à lire
  * @return  String la valeur de l'attribut lu, vide s'il y a eu un problème ou s'il est vide
  */
  public String readv(String key,int am) throws D3Exception;

  /**
  * Lecture et lock d'un record
  * @param key la clé
  * @param PItem le record à mettre à jour, doit être créé avant l'appel (éventuellement vide)
  * @return résultat de la lecture
  *     READ_FILENOTOPEN   Le fichier n'est pas ouvert, le record est mis à vide
  *     READ_RECORDEMPTY   le record n'existe pas pour la clé demandée, il est mis à vide
  *     READ_RECORDLOCKED  le record est déjà locké, dans ce cas le record contient
  *                          en <1> : le numéro de port qui lock
  *                          en <2> : le code user qui lock (si il est connu)
  *     D3_OK              si tout va bien, le record est mis à jour avec les infos
  */
  public int readu(String key, D3Item PItem) throws D3Exception;

  /**
  * Ecrire un record
  * @param key la clé
  * @param PItem le record
  * @return int résultat de l'écriture
  */
  public int write(String key, D3Item PItem) throws D3Exception;

  /**
  * Effacer un record
  * @param key la clé
  * @return int résultat du delete
  */
  public int delete(String key) throws D3Exception;

  /**
  * Tester l'existance d'un record dans le fichier
  * @param key la clé
  * @return boolean false si le record n'existe pas ou s'il y a eu un problème
  *                 true  si le record existe
  */
  public boolean testExist(String key) throws D3Exception;

  /**
  * Relacher un verrou
  * @param key la clé du verrou à lacher
  * @return int résultat de la commande
  */
  public int release(String key) throws D3Exception;

  /**
  * Effectue un select sur un fichier
  * @return int résultat de la commande
  */
  public D3SelectList select() throws D3Exception;

}

