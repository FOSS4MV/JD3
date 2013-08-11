/* 
 * D3Connection.java - The main class of jd3 who communicate with the D3 server
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

import java.io.*;
import java.net.*;
import java.util.*;

 /**
 * The main class of jd3 who communicate with the D3 server
 * @author Christophe Marchal
 * @version 0.8
 */
public class D3Connection implements D3Component{

  private String server;
  private String username;
  private int port;
  private int type;

  private DataOutputStream d3sOut = null;
  private DataInputStream d3sIn = null;
  private Socket d3socket = null;


  /**
  * Create a new Connection to a D3 server or Proxy
  * @param pserver the server name or IP adress
  * @param pport the port number
  * @param pusername  the name of the user to connect
  * @param ppassword the password for the connection
  * @param ptype the type of the connection <li> D3Constants.CONNECTION_TCP = connection directly to a D3 server
  * <li> D3Constants.CONNECTION_TCP_PROXY = connection to a D3 proxy server (usual used for applet)
  * <li> D3Constants.CONNECTION_RMI = connection to a D3 RMI Server
  *
  * <B>Todo</B> try to secure the connection with login mandatory
  */
  public D3Connection(String pserver, int pport, String pusername, String ppassword, int ptype) throws D3Exception{
    this.server = pserver;
    this.port = pport;
    this.username = pusername;
    this.type = ptype;

    if(type != D3Constants.CONNECTION_TCP && type != D3Constants.CONNECTION_RMI && type != D3Constants.CONNECTION_TCP_PROXY) {
      throw new D3Exception("Type of connection unknow");
    }

    // Init connection to the server
    if(type == D3Constants.CONNECTION_RMI) {
        initRMI();
    }else{
        initTcp();
    }
  
    // login(username,password)

  }

  
  /**
  * Get the port number of the connection
  * @return The port number
  */
  public int getPort(){
    return this.port;
  }


  /**
  * Get the server name
  * @return The server name
  */
  public String getServer() {
    return this.server;
  }


  /**
  * Get the user name of the connection
  * @return The user name
  */
  public String getUserName() {
    return this.username;
  }


  /**
  * Create a new session 
  * @return the session of the good type depending of the type of connection
  * <B>todo</B> the rmi session
  */
  public D3Session createSession() {
    if(this.type == D3Constants.CONNECTION_TCP || this.type == D3Constants.CONNECTION_TCP_PROXY) {
        return new D3SessionTcp(this);
    }else{
        return null;
    }  
  }
  

  /**
  * Identify the user in the server
  * @param user The user name
  * @param password The password of the user
  */
  public int login(String user, String password) throws D3Exception{
    D3Buffer buf = doit(D3Constants._D3_LOGON + "\001" + user + "\001" + password + "\001" );
    this.username = user;
    return buf.getStatus();
  }


  /**
  * Logoff this user and close connection
  * <B>Todo</B> close connection
  */
  public void logoff(String user) throws D3Exception{
    doit(D3Constants._D3_LOGOFF + "\001"  );
    close();
  }


  /**
  * Send information to the server and wait for response
  * @param cmd Message in the correct format
  * @return  Un D3Buffer contenant le résultat de la commande
  */
  protected synchronized D3Buffer doit(String cmd) throws D3Exception{
     D3Buffer tmp;
      
     if(this.type == D3Constants.CONNECTION_RMI) {
        throw new D3Exception("This is not a TCP connection"); 
     }

     this.send(cmd);
     tmp = this.receive();

     return tmp;
  }


  /**
  * Init the connection to a TCP server
  */
  private void initTcp() throws D3Exception{
    byte buf[];
    String sport;
    int newport = this.port;
 
    /*
    * If it is a direct connection to D3server we must ask for
    * a free port
    */
    if(type == D3Constants.CONNECTION_TCP) {
     try{

        d3socket = new Socket(server, port);
        d3sIn = new DataInputStream(d3socket.getInputStream());

        buf = new byte[8];
        d3sIn.readFully(buf, 0, 8);
        sport = new String(buf,0,8);
        newport = Integer.parseInt(sport);

        d3sIn.close();
        d3socket.close();
        
      }catch(Exception e){
        throw new D3Exception("Erreur de connection " + e.getMessage());
      }
    }

    /*
    * then really connect to the client
    */
    try{
       d3socket = new Socket(server, newport);
       d3sOut = new DataOutputStream(d3socket.getOutputStream());
       d3sIn = new DataInputStream(d3socket.getInputStream());
    }catch(Exception e){
      throw new D3Exception("Erreur de connection " + e.getMessage());
    }
   
  }
  

  /**
  * Init the connection to a RMI server
  * <G>Todo</G> all
  */
  private void initRMI() throws D3Exception {
     throw new D3Exception("Not yet implemented"); 
  }

  /**
  * Close everything
  */
  public void close() {
    try{
     d3sOut.close();
     d3sIn.close();
     d3socket.close();    
    }catch(IOException ioe){}
  }
  
  /**
  * Send a message to the server
  */
  private synchronized void send(String outBuffer) throws D3Exception{
    try{
      int i = outBuffer.length();
      String slen = new D3Format("%08d").form(i);

      // Send all in one 
      d3sOut.writeBytes(slen + outBuffer);
      d3sOut.flush();

    }
    catch(IOException e){
      throw new D3Exception("Can't send message to " + this.server + " {" + e.getMessage() + "}");
    }
  }


  /**
  * Receive the response from the server
  * Message format :
  *    8 bytes = lenght of the following message
  *    block   = error no + "\001" + datas
  */
  private synchronized D3Buffer receive() throws D3Exception{
    int length;
    byte buf[];
    String stat;
    int status = D3Constants.D3_ERR;
    Vector inBuffer = new Vector();

    try{
      buf = new byte[8];
      // Reading the lenght
      d3sIn.readFully(buf, 0, 8);

      String slen = new String(buf, 0, 8);
      length = Integer.parseInt(slen);

      buf = new byte[length];
      // reading datas with this lenght
      d3sIn.readFully(buf, 0, length);
    }
    catch(Exception e){
      throw new D3Exception("Could not receive response from " + this.server + " {" + e.getMessage() + "}");
    }

    // Converting into vector with "\001" as separator
    StringTokenizer st = new StringTokenizer(new String(buf),"\001");
    // Error no is always the first
    stat = st.nextToken();
    
    if(st.hasMoreTokens()) {
       do{
          inBuffer.addElement(st.nextToken());
       }while(st.hasMoreTokens());
    }else {
      // if there is no data, we must add an empty string
      inBuffer.addElement("");
    }

    // get error code returned as first ASCII string
    try{
      status = Integer.parseInt(stat);
    }
    catch(NumberFormatException ex){
     status = -1;
    }

    return new D3Buffer(status,inBuffer);
  }

  
}
