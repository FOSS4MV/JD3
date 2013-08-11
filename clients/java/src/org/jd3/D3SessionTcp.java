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


package org.jd3;

import org.jd3.utils.*;
import org.jd3.exceptions.*;

import java.lang.StringBuffer;

 /**
 * Implementation of the D3Session who uses a TCP/IP socket
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3SessionTcp implements D3Session{

  /**
  *  The D3Connection used to communicate with the D3 server
  */
  private D3Connection myconnection;


  /**
  * Create a D3Session who use a TCP/IP socket to cummunicate with the server
  */
  public D3SessionTcp(D3Connection pcon) {
      myconnection = pcon; 
  }


  /**
  * this function do nothing, just send instruction and receive a response.
  * Use it to know the communication is OK
  *
  * @return the status of command. <li> D3_OK if the communication is OK <li> D3_ERR if not
  */
  public int noop() throws D3Exception{
    D3Buffer buf = this.myconnection.doit(D3Constants._D3_NOOP + "\001");
    return buf.getStatus();
  }

  /**
  * Call a basic subroutine
  * @param name The name of the subroutine <li>mysub : must be cataloged in the account of the server 
  *<li>myaccount,myfile, mysub : the absolute path to the subroutine
  * @param parms The parameters of the subroutine
  */
  public int call(String name, D3Params parms ) throws D3Exception{
     int nargs,i,nretour,error;
     // Use StringBuffer because it is more efficient and there must be lot of parameter
     StringBuffer buffer;
     D3Buffer buf;

     // construire la phrase pour le call avec les paramètres
     nargs = parms.getSize();
     if(nargs < 0) {
        return D3Constants.D3_ERR;
     }

     buffer = new StringBuffer("");
     buffer.append(D3Constants._D3_CALL);
     buffer.append("\001");
     buffer.append(name);
     buffer.append("\001");
     buffer.append(nargs);
     buffer.append("\001");
     for(i = 0;i < nargs; i++) {
	     buffer.append((String) parms.getParam(i));
	     buffer.append("\001");
     }

     // envoit de la demande
     buf = this.myconnection.doit(buffer.toString());
     if(buf.getStatus()!= D3Constants.D3_OK) {
          return D3Constants.D3_ERR;
     }

     // garnir pour le retour
     nretour = buf.getData().size();
     for(i = 0 ; i < nretour && i < nargs;i++) {
         parms.setParam((String)buf.getData().elementAt(i),i);
     }

     return D3Constants.D3_OK;    
  }

  /**
  * Execute a TCL command
  * @param sentence The TCL command to execute
  */
  public String execute(String sentence) throws D3Exception{
    D3Buffer buf = this.myconnection.doit(D3Constants._D3_EXECUTE + "\001" + sentence + "\001");
    if( buf.getStatus()!= D3Constants.D3_OK) {
        throw new D3Exception("Erreur d' exécution de " + sentence);
    }
    return((String)buf.getData().elementAt(0));  
  }

  /**
  * Execute a select at the TCL and return the active list.
  * @param sentence The AQL command to select key from file
  */
  public D3SelectList select(String sentence) throws D3Exception{
    D3Item list;
    String[] result;
    D3Buffer buf = this.myconnection.doit(D3Constants._D3_SELECT_TCL + "\001" + sentence + "\001");

    if( buf.getStatus() != D3Constants.D3_OK ) {
       throw new D3Exception("Erreur d' exécution de " + sentence);
    }

    // first data is number of key selected
    int nbelement = Integer.parseInt((String)buf.getData().elementAt(0));
    result = new String[nbelement];
    if(nbelement > 0 ) {
       // second data is list of key separated by @AM
       // We use D3Item, which is more easy to use ;)
       list = new D3Item((String)buf.getData().elementAt(1));

       for(int i = 0;i <= (nbelement-1);i++) {
          result[i] = list.extract(i+1);
       }
    }
    return new D3SelectListTcl(result);
  }


  /**
  * Open D3 File in the account you run the server
  * @param pfilename The file name to open.
  * The file must be in the account where run the server, or as a Q-pointer
  */
  public D3File openFile(String pfilename) throws D3Exception{
    return new D3FileTcp(this.myconnection,"",pfilename);
  }
  
  /**
  * Open a D3 File in an account name
  * @param paccountname The name of the account where is store the d3 file
  * @param pfilename The name of the file to open, must be in the account (or as a q-pointer)
  */
  public D3File openFile(String paccountname,String pfilename) throws D3Exception{
      return new D3FileTcp(this.myconnection,paccountname,pfilename);
  }


}
