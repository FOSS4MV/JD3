 /* 
 * D3SelectListFile.java - The implmentation of a select list for on a file
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

import org.jd3.*;
import org.jd3.exceptions.*;

 /**
 * The implmentation of a select list for on a file
 * Ex: in pickbasic
 *  open "file" to ff
 *  select ff
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3SelectListFile implements D3SelectList{

 /**
 * The connection to the D3 server
 */
 private D3Connection con;

 /**
 * the select descriptor used by the server
 */
 private String select;

 /**
 * the current key read from the server
 */
 private String curkey = null;

 /**
 * State of the select list, only use to be more efficiency (no network communication if 
 * the select list are completely read) 
 */
 private boolean finished = false;
 
 /**
 * The number of element in the select list
 */
 private int nbelements = 0;

 
 /**
 * Create a select list from a file descriptor
 * @param pcon The D3 connection used to get the key of the select list
 * @param pselect The select descriptor use by the D3 server to manage the select list
 * @param nbelement The number of element in the select list
 */
 public D3SelectListFile(D3Connection pcon, String pselect, int nbelement) {
    this.con = pcon;
    this.select = pselect;
    this.nbelements = nbelements;
 }


 /**
 * Verify if there is one more key in the select list
 * @return true if there is one more key in the select list
 *         false if not
 */
 public boolean hasMoreElements(){
    // If there is no key, we read it from the select list
    if(curkey == null) {
       return readNext();
    }
    // We have a key
    return true;
 }


 /**
 * Get the next key in the select list
 * @return The next key in the select list, null if there is no more key
 */
 public String getNextElement() {
    String result = null;
    // See if there is a key
    if (hasMoreElements() ) {
      result = this.curkey;
      this.curkey = null;
    }
    return result;
 }

 
 /**
 * Read the next key from the select list on the server
 * @return True if we succeed in reading next key
 */
 private boolean readNext(){

    // If we have already a key in the buffer
    if(curkey != null) return true;

    // If we have already finished the select list
    if(finished) return false;

    try {
       D3Buffer buf = this.con.doit(D3Constants._D3_READNEXT + "\001"  + select + "\001" );

       // If it is the end of the list
       if(buf.getStatus() == D3Constants.READNEXT_EOF) {
          finished = true;
          return false;
       }else {
          // If there was another error
          if( buf.getStatus() != D3Constants.D3_OK){
            return false;
          }
       }

       curkey = (String)buf.getData().elementAt(0);
       return true;

    }catch(D3Exception de) {
       return false;
    }

 }

 /**
 * Get the number of elements selected in the list
 * @return The number of elements selected in the list
 */
 public int getNbElements() {
    return this.nbelements;
 }


}
