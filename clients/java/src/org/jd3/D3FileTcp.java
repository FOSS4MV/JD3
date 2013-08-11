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

package org.jd3;

import java.io.*;
import java.util.*;

import org.jd3.utils.*;
import org.jd3.exceptions.*;

 /**
 * The implementation of a D3File who use a TCP/IP socket to communicate with the D3 server
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3FileTcp implements D3File{

  /**
  * The connection to the D3 server
  */
  private D3Connection con;

  /**
  * the file descriptor
  */
  private int fd;

  /**
  * The name of the file
  */
  private String filename;
  
  /**
  * The name of the account where is the file
  */
  private String accountname;
  
  /**
  * Open a file store on the D3 server
  * @param pcon The conneciton to the server
  * @param paccount The account name where is store the file
  * @param pfilename The name of the file to open
  */
  public D3FileTcp(D3Connection pcon, String paccount, String pfilename) throws D3Exception{
    D3Buffer buf;
    this.con = pcon;
    fd = 0;
    filename = pfilename;
    accountname = paccount;
    
    buf = this.con.doit(D3Constants._D3_OPEN + "\001" + paccount + "\001" + pfilename + "\001");
    if(buf.getStatus() != D3Constants.D3_OK ) {
      throw new D3Exception("Erreur d'ouverture de " + paccount + "," + pfilename + ",");
    }

    try{
      fd = Integer.parseInt((String)buf.getData().elementAt(0));
    }
    catch(NumberFormatException e){
      throw new D3Exception("Erreur d'ouverture de " + paccount+ "," + pfilename + ",");
    }
  }

  /**
  * Open a file store on the D3 server using the account where the server is running
  * @param pcon The conneciton to the server
  * @param pfilename The name of the file to open
  */
  public D3FileTcp(D3Connection pcon, String pfilename) throws D3Exception{
       this(pcon, "", pfilename);
  }

  /**
  * Get the name of the file
  * @return The name of the file
  */
  public String getFileName() {
     return this.filename;
  }
  
  /**
  * Get the name of the accoutn where is the file
  * @return The name of the account where is the file
  */
  public String getAccountName() {
     return this.accountname;
  }
  
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
  public synchronized int read(String key, D3Item PItem) throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_READ + "\001" + fd + "\001" + key + "\001");
    PItem.setRecord((String)buf.getData().elementAt(0));
    return buf.getStatus();
  }

  /**
  * Lecture d'un attribut d'un record
  * @param key  la clé du record à lire
  * @param am   la position de l'attribut à lire
  * @return  String la valeur de l'attribut lu, vide s'il y a eu un problème ou s'il est vide
  */
  public synchronized String readv(String key,int am) throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_READV + "\001" + fd + "\001" + key + "\001" + Integer.toString(am) + "\001");
    if(buf.getStatus() == D3Constants.D3_OK ) {
       return (String)buf.getData().elementAt(0);
    }else{
       return "";
    }
  }

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
  public synchronized int readu(String key, D3Item PItem) throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_READU + "\001" + fd + "\001" + key + "\001");
    if( buf.getStatus() == D3Constants.D3_OK){
       PItem.setRecord((String)buf.getData().elementAt(0));
    }
    return buf.getStatus();
  }

  /**
  * Ecrire un record
  * @param key la clé
  * @param PItem le record
  * @return int résultat de l'écriture
  */
  public synchronized int write(String key, D3Item PItem)  throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_WRITE + "\001" + fd + "\001" + key + "\001" + PItem.toString() + "\001");
    return buf.getStatus();
  }

  /**
  * Effacer un record
  * @param key la clé
  * @return int résultat du delete
  */
  public synchronized int delete(String key) throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_DELETE + "\001" + fd + "\001" + key + "\001");
    return buf.getStatus();
  }

  /**
  * Tester l'existance d'un record dans le fichier
  * @param key la clé
  * @return boolean false si le record n'existe pas ou s'il y a eu un problème
  *                 true  si le record existe
  */
  public synchronized boolean testExist(String key) throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_TEST_EXIST + "\001" + fd + "\001" + key + "\001");
    if ( buf.getStatus() == D3Constants.D3_OK){
      return ((String)buf.getData().elementAt(0)).equals("0");
    }else {
      return false;
    }
  }

  /**
  * Relacher un verrou
  * @param key la clé du verrou à lacher
  * @return int résultat de la commande
  */
  public synchronized int release(String key) throws D3Exception{
    D3Buffer buf = this.con.doit(D3Constants._D3_RELEASE + "\001" + fd + "\001" + key + "\001");
    return buf.getStatus();
  }

  /**
  * Effectue un select sur un fichier
  * @return int résultat de la commande
  */
  public synchronized D3SelectList select() throws D3Exception{
    int nb = 0;
    D3Buffer buf = this.con.doit(D3Constants._D3_SELECT + "\001" + fd + "\001");
    nb = Integer.parseInt((String)buf.getData().elementAt(1));
    return new D3SelectListFile(this.con,(String)buf.getData().elementAt(0),nb);
  }

}

