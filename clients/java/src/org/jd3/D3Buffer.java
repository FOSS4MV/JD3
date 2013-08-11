/* 
 * D3Buffer.java - The message receive by the D3Connection when it speek with the D3 server
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

import java.util.*;
import java.io.*;

 /**
 * The message receive by the D3Connection when it speek with the D3 server
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3Buffer implements Serializable {

  private Vector buffer;
  private int status;

  /**
  * Create a new buffer with status and datas
  * @param stat the status
  * @param info the datas
  */ 
  public D3Buffer(int stat, Vector info) {
      buffer = info;
      status = stat;
  }

  /**
  * get the status
  */
  public int getStatus() {
     return status;
  }

  /**
  * get the data
  */
  public Vector getData() {
     return buffer;
  }
}
