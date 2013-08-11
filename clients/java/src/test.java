/* 
 * test.java - Program to test the jd3 classes
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
 * Program to test the jd3 classes
 * @author Christophe Marchal
 * @version 1.0
 */

import java.io.*;
import java.net.*;

import org.jd3.*;
import org.jd3.exceptions.*;

public class test{

  static public void main(String[] agrs){

    long deb,fin;
    int port;
    String serv;
    
    serv = agrs[0];
   
    port = Integer.parseInt(agrs[1]);

    System.out.println("connexion sur " + serv + ":" + port);

    String tutu = 3 + "tutu";
    System.out.println("tutu = " + tutu);

    try {
      D3Connection con = new D3Connection(serv,port,"cm","",D3Constants.CONNECTION_TCP_PROXY);

      D3Session sess = con.createSession();
      System.out.println("Connecté...");

      D3File fd = sess.openFile("jbasic");
      D3Item rec = new D3Item();

      deb = System.currentTimeMillis();
      
      for(int j =0;j < 1000 ; j++){
         System.out.print(j + "\r");
         sess.noop();
//         fd.read("toto",rec);
//         System.out.println(rec.toString());
      }
      
      fin = System.currentTimeMillis();
      
      System.out.println("fini, durrée = " + (fin-deb) + " milli");


    }catch(Exception ee) {
        System.out.println("Oups " + ee.getMessage());
        ee.printStackTrace();
    }

/*  
        byte buf[];
        int pickport;
        String bufstring;
        Socket D3Socket;
        DataOutputStream outVersD3;
        DataInputStream inVersD3;
        
        // Première connection vers Pick
        try {
           D3Socket = new Socket(serv,port);
        
        System.out.println("Yahoo");
        
        // Ouverture des flux
            outVersD3 = new DataOutputStream(D3Socket.getOutputStream());
            inVersD3 = new DataInputStream(D3Socket.getInputStream());
        
        // Envoit d'une requete pour savoir sur quel port on va converser avec D3
        buf = new byte[8];
            inVersD3.readFully(buf, 0, 8);
            D3Socket.close();
            bufstring = new String(buf,0,8);
            pickport = Integer.parseInt(bufstring);
            System.out.println("Re-Connection to " + serv + ":" + pickport);
            
        }
        catch(Exception e){
            System.out.println("bug " + e);
        }

*/        
        /*
        // Nouvelle initialisation sur le nouveau port défini par le serveur D3
        try {
           D3Socket = new Socket(D3HostName,D3Port);
        }catch(Exception ie) {
            System.out.println(" Cannot open connection to " +D3HostName + ":" + D3Port);
        }
        
        // RéOuverture des flux sur le nouveau socket D3
        try {
            outVersD3 = new DataOutputStream(D3Socket.getOutputStream());
            inVersD3  = new DataInputStream(D3Socket.getInputStream());
        } catch(IOException e) {
            throw new Exception(" Cannot open connection to "+ D3HostName + ":" + D3Port);
        }
        */
        
    }



}
