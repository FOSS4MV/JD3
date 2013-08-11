/* 
 * D3Exception.java - The exception class for jd3
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

 /**
 * The exception class for jd3
 * @author Christophe Marchal
 * @version 1.0
 */
 
package org.jd3.exceptions;

public class D3Exception extends Exception {

  private int errorno = 0;

  public D3Exception(int perrorno,String pmsg) {
    super(pmsg);
    this.errorno = perrorno;
  }

  public D3Exception(String pmsg) {
     this(0,pmsg);
  }

  public D3Exception() {
    this(0,"Unknow error");
  }

  public int getErrorNo() {
     return this.errorno;
  }

  /**
   * print error message with error number
   *
   */
  public String toString(){
    StringBuffer tmp = new StringBuffer("[");
    tmp.append(getErrorNo());
    tmp.append("] ");
    tmp.append(getMessage());
    return tmp.toString();
  }

}
