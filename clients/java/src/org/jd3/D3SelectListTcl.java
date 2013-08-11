 /* 
 * D3SelectListTcl.java - The implmentation of a select list create by an TCL command
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
 * The implmentation of a select list create by an TCL command
 * Ex : at TCL
 *   :SELECT myfile WITH A1 = "a]"
 *  3 items selected
 *   >
 *
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3SelectListTcl implements D3SelectList{


 /**
 * String array which contains the key
 */
 private String[] elements;


 /**
 * The current position of the select list in the array
 */
 private int curpos = 0;

 /**
 * The limitation of the array, maybe I could use the length of this array
 */
 private int maxpos = 0;
 

 /**
 * Create a select list from the list of key return by the TCL command
 * @param pelements A string array which contains the key of the list
 */
 public D3SelectListTcl(String[] pelements) {
    this.elements = pelements;
    if(pelements != null) {
       this.maxpos = this.elements.length;
    }
 }
 
 /**
 * Verify if there is one more key in the select list
 * @return true if there is one more key in the select list
 *         false if not
 */
 public boolean hasMoreElements() {
   if(curpos < maxpos) return true;
   else return false;   
 }

 /**
 * Get the next key in the select list
 * @return The next key in the select list, null if there is no more key
 */ 
 public String getNextElement() {
    String result = null;
    if (hasMoreElements() ) {
      result = this.elements[curpos];
      curpos ++;
    }
    return result;
 }

 /**
 * Get the number of elements selected in the list
 * @return The number of elements selected in the list
 */
 public int getNbElements() {
    return elements.length;   
 }

}
