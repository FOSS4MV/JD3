 /* 
 * D3Constants.java - The constants used by jd3
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

 /**
 * The constants used by jd3
 * @author Christophe Marchal
 * @version 0.8
 */
public final class D3Constants{

  
  //Logon command
  public static final int _D3_LOGON = 1;
  // Logoff
  public static final int _D3_LOGOFF = 2;
  // Execute an AQL statement
  public static final int _D3_EXECUTE = 3;
  // Call a subroutine
  public static final int _D3_CALL = 4;
  // Do a select on an open file
  public static final int _D3_SELECT = 5;
  // Open a file
  public static final int _D3_OPEN = 6;
  // Do a readnext on a select list open with _D3_SELECT
  public static final int _D3_READNEXT = 7;
  // Read a record from a file opened
  public static final int _D3_READ = 8;
  // Read and lock a record from a file opened
  public static final int _D3_READU = 9;
  // Write a record in an opened file
  public static final int _D3_WRITE = 10;
  // Release a lock on a record
  public static final int _D3_RELEASE = 11;
  // Delete a record from a file
  public static final int _D3_DELETE = 12;
  // Close an opened file
  public static final int _D3_CLOSE = 13;
  // Just test if the record exist or not, without reading it (less data on network;)
  public static final int _D3_TEST_EXIST = 14;
  // Just lock a record without reading it (less data on network ;)
  public static final int _D3_LOCK_ITEM = 15;
  // read an attribute from a record
  public static final int _D3_READV = 16;
  // Execute an AQL statement at the TCL
  public static final int _D3_SELECT_TCL = 17;
  // Just test the communication with the server
  public static final int _D3_NOOP = 18;

 // Attribute Mark
  public static final char _D3_AM = (char)254;
  // Value mark
  public static final char _D3_VM = (char)253;
  // Subvalue mark
  public static final char _D3_SVM = (char)252;

  // Number of day between the day 0 (in internal format) in D3 and in Java
  public static final int _D3_DiffDate = 732;

  // Number of milliseconds in one day
  public static final int _D3_DiffTime = 86400000;

  // Error code
  public static final int D3_ERR =  1;
  public static final int D3_OK  = 0;

  public static final int READ_FILENOTOPEN = -1;
  public static final int READ_RECORDEMPTY = -2;
  public static final int READ_RECORDLOCKED = -3;

  public static final int READNEXT_EOF = -1;

  //Error code for the login inthe D3Connection
  public static final int USR_INCONNU   = -2;
  public static final int PWD_INCORRECT =  -1;

  // Type of the connection 
  public static final int CONNECTION_TCP = 1;
  public static final int CONNECTION_TCP_PROXY = 2;
  public static final int CONNECTION_RMI = 3;

}
