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

package org.jd3;

import org.jd3.utils.*;
import org.jd3.exceptions.*;

 /**
 * Interface of the session definition, use for basic operation on the server
 * @author Christophe Marchal
 * @version 0.8
 */
public interface D3Session extends D3Component {

  /**
  * This function do nothing, just send instruction and receive a response.
  * Use it to know the communication is OK
  *
  * @return the status of command. <li> D3_OK if the communication is OK <li> D3_ERR if not
  */
  public int noop() throws D3Exception;

  /**
  * Call a basic subroutine
  * @param name The name of the subroutine <li>mysub : must be cataloged in the account of the server 
  *<li>myaccount,myfile, mysub : the absolute path to the subroutine
  * @param parms The parameters of the subroutine
  */
  public int call(String name, D3Params parms ) throws D3Exception;

  /**
  * Execute a TCL command
  * @param sentence The TCL command to execute
  */
  public String execute(String sentence) throws D3Exception;

  /**
  * Execute a select at the TCL and return the active list.
  * @param sentence The AQL command to select key from file
  */
  public D3SelectList select(String sentence) throws D3Exception;
  
  /**
  * Open D3 File in the account you run the server
  * @param pfilename The file name to open.
  * The file must be in the account where run the server, or as a Q-pointer
  */
  public D3File openFile(String pfilename) throws D3Exception;
  
  /**
  * Open a D3 File in an account name
  * @param paccountname The name of the account where is store the d3 file
  * @param pfilename The name of the file to open, must be in the account (or as a q-pointer)
  */
  public D3File openFile(String paccountname,String pfilename) throws D3Exception;
  
}
