/* 
 * test_session.java - Another program to test the D3 classes
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
 * Another program to test the jd3 classes
 * @author Christophe Marchal
 * @version 1.0
 */

import org.jd3.*;
import org.jd3.exceptions.*;

public class test_session{

  static public void main(String[] agrs){
    try {
      D3Connection con = new D3Connection("linuxdev",20001,"cm","",D3Constants.CONNECTION_TCP_PROXY);

      D3Session sess = con.createSession();
      System.out.println("Connecté...");

      D3Params par = new D3Params();
      System.out.println("calling toto...");
      sess.call("toto",par);

      System.out.println("fini");

    }catch(Exception ee) {
        System.out.println("Oups " + ee.getMessage());
        ee.printStackTrace();
    }
  }

}
