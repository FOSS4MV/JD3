package org.jd3;

import java.util.*;
import java.lang.*;

public class D3Date{
 private long D3Valeur;
 
 public D3Date(Date init){
     D3Valeur = (long)D3Date.dateToInt(init);
 }
 
 public D3Date( D3Date init) {
   D3Valeur = init.dateInt();
 }	
 
 public D3Date(long init) {
    D3Valeur = init;
 }
 
 public D3Date(int init) {
    D3Valeur = (long)init;
 }
 
 public long dateInt() {
     return this.D3Valeur;	
 }
 
 public Date dateVal() {
    return D3Date.intToDate((int)D3Valeur);
 }

 /**
   * Conversion d'une date en format interne Pick
   */
 static public int dateToInt(Date datejava) {
     return (int)(datejava.getTime() / D3Constants._D3_DiffTime) + D3Constants._D3_DiffDate;
  }

 /**
   * Conversion d'une date format pick en java
   */
  static public Date intToDate(int pickint) {
     long tmp =  (pickint - D3Constants._D3_DiffDate);
     Date val = new Date( tmp * D3Constants._D3_DiffTime);
     return val;
  }

  /**
  * Conversion de secondes en Date Java
  */
  static public Date intToTime(int pickint) {
     Date val = new Date(pickint * 1000);
     return val;
  }
  
  /**
  * Conversion d'un Date Java en seconde Pick
  */
 static public int timeToInt(Date datejava) {
     return (int)(datejava.getTime() / 1000);
  }

 /**
  * Conversion d'un time en heure
  **/
  static public String intToMTS(int time) {
    int min,hour;
    hour = (int)(time / 3600);
    time -= (hour * 3600); 
    min = (int)(time / 60);
    time -= (min*60);
    return Integer.toString(hour) + ":" + Integer.toString(min) + ":" + Integer.toString(time);  	
  }
  
  static public String intToMT(int time){
    int min,hour;
    hour = (int)(time / 3600);
    time -= (hour * 3600); 
    min = (int)(time / 60);
    time -= (min*60);
    return Integer.toString(hour) + ":" + Integer.toString(min);  	
  }
  
  static public int MTSToInt(String heure) {
    int sec = 0;
    StringTokenizer sh = new StringTokenizer(heure,":");
    try {
    	sec += (Integer.parseInt(sh.nextToken()) * 3600 );
    }catch(Exception ee){}
    try {
    	sec += (Integer.parseInt(sh.nextToken()) * 60 );
    }catch(Exception ee){}
    try {
    	sec += Integer.parseInt(sh.nextToken());
    }catch(Exception ee){}
    
    return sec;
  }

}