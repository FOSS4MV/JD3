 /* 
 * D3Params.java - Class to manage the parameters of a D3 Subroutine
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

import java.util.Vector;

 /**
 * Class to manage the parameters of a D3 Subroutine
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3Params {
   /**
   * Used to manage parameters
   */
   private Vector mylist;
   
   /**
   * Initialize new D3 Parameters for subroutine
   */
   public D3Params() {
      this.mylist = new Vector();	
   }

   /**
   * Get the number of parameter
   * @return The number of the parameters
   */
   public int getSize() {
     return mylist.size(); 
   }   
   
   
   /**
   * Getting value of a parameter
   * @param pos position of the parameter which we want the value
   * @return The value of the "posth" parameter 
   */
   public String getParam(int pos) {
      return (String) mylist.elementAt(pos);
   }

   /**
   * Setting a value to an element
   * @param par the new value
   * @param pos position of the parameter to set
   */   
   public void setParam(String par,int pos) {
      mylist.setElementAt(par,pos);
   }

   /**
   * Add one parameter to the list
   * @param par The value of the parameter
   */   
   public void addParam(String par) {
      mylist.addElement(par);
   }
}
