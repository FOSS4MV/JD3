/* 
 * D3Item.java - This is the Java representation of a D3 record
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
import java.util.*;

 /**
 * This is the Java representation of a D3 record
 * with the Vector class, it can simulate a dynamic structure of data
 * @author Christophe Marchal
 * @version 1.0
 */
public class D3Item implements D3Component,Serializable {

  private Vector Item;

  public D3Item(String item){
	setRecord(item);
  }

  public D3Item(){
    this("");
  }

  public void setRecord(String newrec){
    Vector anAttribute, aValue;
    int alength, vlength, svlength;
    String abuf, vbuf, svbuf;
    int nbam,nbvm,nbsvm,l;
    int a,aa,v,vv,s,ss;

   if(Item != null) {
      Item.clear();
   } else {
      Item = new Vector();
   }

   newrec += "\001";            // make life easy
   alength = newrec.length();
   nbam = 0;
   // Boucler sur tous les caractères de la chaine
   for(a = 0,aa = 0; aa < alength; aa++){
      // Si j'ai un séparateur AM ou la fin de ligne
      if(newrec.charAt(aa) == '\u00fe' || newrec.charAt(aa) == '\001'){
        // J'ai un nouvel attribut
        anAttribute = new Vector();
        Item.addElement(anAttribute);

        // chaine des multi-valeurs
        vbuf = new String(newrec.substring(a, aa) + "\001");      // make life easy
        vlength = vbuf.length();

        nbvm = 0;
        // Boucle sur tous les caratères de la MV
        for(vv = 0,v=0; vv < vlength; vv++){
          // Si j'ai un séparateur de VM ou fin de ligne
          if(vbuf.charAt(vv) == '\u00fe' || vbuf.charAt(vv) == '\u00fd' || vbuf.charAt(vv) == '\001'){
            // On a une multi valeur
            aValue = new Vector();
            svbuf = new String(vbuf.substring(v, vv) +  "\001");       // make life easy
            svlength = svbuf.length();

            nbsvm = 0;
            s = 0;
            // Boucle sur chaque caractères de la MV pour les SVM
            for(s=0,ss = 0; ss < svlength; ss++){
              // Si on a séparateur de SVM
              if(svbuf.charAt(ss) == '\u00fe' || svbuf.charAt(ss) == '\u00fd' || svbuf.charAt(ss) == '\u00fc' || svbuf.charAt(ss) == '\001'){
                  // On a une SVM
                  String buf = new String(svbuf.substring(s, ss));
                  // On la rajoute dans le tableau si ele est pas vide ou si c'est pas la première
                  if(!buf.equals("") || nbsvm > 0 ){
                     // Si c'est pas la première mais que le tableau est vide alors c'est qu'il commance par des vides
                     if(nbsvm > 0 && aValue.size() == 0) {
                       for(l = 0 ; l < nbsvm ; l++){
                          aValue.addElement("");
                       }
                     }
                     // On ajoute la sous-valeurs
                     aValue.addElement(buf);
                  }

                s = ss + 1;
                nbsvm ++;  // Compte le nombre de sous-valeurs traitées
              }
            } // kk
            v = vv + 1;
            // Si c'est pas la première ou que qu'il y a des sous-valeurs
            if(aValue.size() > 0 || nbvm > 0 ) {
              // Si il y a déjà des VM mais que le tableau est vide alors il commence par des vides
              if(nbvm > 0 && anAttribute.size() == 0 ) {
                 for(l = 0 ; l < nbvm ; l++){
                   anAttribute.addElement(new Vector());
                 }
              }
              // Ajout du tableau des sous-valeurs
              anAttribute.addElement(aValue);
            }
            // Compte le nombre de VM
            nbvm ++;
          }
        } // jj
        a = aa + 1;
        // Compte le nombre d'attribut
        nbam ++;
      }
   }//ii

  }

/**
  * Extract <attr, value, subvalue> from a D3Item as a String
  * @param attr     attribute number
  * @param value    value number
  * @param svalue   subvalue number
  */
  public String extract(int attr, int value, int svalue){
    Vector anAttribute, aValue;
    String Result;

    if(attr == 0){
      Result = new String("");
      return(Result);
    }

    if(value == 0) svalue = 0;

    if(attr > Item.size()){
      Result = new String("");
      return(Result);
    }

    anAttribute = (Vector)Item.elementAt(attr-1);

    if(value == 0){             // return ALL values
      Result = new String("");
      int n = anAttribute.size();
      for(int i = 0; i < n; i++){
        aValue = (Vector)anAttribute.elementAt(i);
        int m = aValue.size();
        for(int j = 0; j < m; j++){
          Result = Result + (String)aValue.elementAt(j);
          if(j != m-1){
            Result = Result + "\u00fc";
          }
        }
        if(i != n-1){
          Result = Result + "\u00fd";
        }
      }
      return(Result);
    }

    if(value > anAttribute.size()){
      Result = new String("");
      return(Result);
    }

    aValue = (Vector)anAttribute.elementAt(value-1);

    if(svalue == 0){            // return ALL subvalues
      Result = new String("");
      int n = aValue.size();
      for(int i = 0; i < n; i++){
        Result = Result + (String)aValue.elementAt(i);
        if(i != n-1){
          Result = Result + "\u00fc";
        }
      }
      return(Result);
    }

    if(svalue > aValue.size()){
      Result = new String("");
      return(Result);
    }

    Result = new String((String)aValue.elementAt(svalue-1));
    return(Result);
  }

  public String extract(int attr, int value){
    return(extract(attr, value, 0));
  }

  public String extract(int attr){
    return(extract(attr, 0, 0));
  }

/**
  * replace a string in a <attr, value, subvalue>
  * @param attr     attribute number
  * @param value    value number
  * @param svalue   subvalue number
  * @param newData  new string to replace
  */
  public void replace(int attr, int value, int svalue, String newData){
    Vector anAttribute, aValue;

    if(attr == 0) return;
    if(newData == null) return;
    if(value == 0) svalue = 0;
    if(svalue == 0) svalue = 1;

    if(attr > Item.size()){
      int j = Item.size();
      for(int i = j; i < attr; i++){
        anAttribute = new Vector();
        aValue = new Vector();
        Item.addElement(anAttribute);
        anAttribute.addElement(aValue);
        aValue.addElement("");
      }
    }

    anAttribute = (Vector)Item.elementAt(attr-1);

    if(value == 0){
      aValue = new Vector();
      anAttribute.setElementAt(aValue, 0);
      aValue.addElement(newData);
    }else{
      if(value > anAttribute.size()){
        int j = anAttribute.size();
        for(int i = j; i < value; i++){
          aValue = new Vector();
          anAttribute.addElement(aValue);
          aValue.addElement("");
        }
      }
      // add in the data
      aValue = (Vector)anAttribute.elementAt(value-1);
      if(svalue > aValue.size()){
        int j = aValue.size();
        for(int i = j; i < svalue; i++){
          aValue.addElement("");
        }
      }
      aValue.setElementAt(newData, svalue-1);
    }
  }

  public void replace(int attr, int value, String newData){
    replace(attr, value, 0, newData);
  }

  public void replace(int attr, String newData){
    replace(attr, 0, 0, newData);
  }

/**
  * Insert a string at <attr, value, subvalue>
  * @param attr     attribute number
  * @param value    value number
  * @param svalue   subvalue number
  * @param newData  new String to insert
  */
  public void insert(int attr, int value, int svalue, String newData){
    Vector anAttribute, aValue;

    if(attr == 0){
      anAttribute = new Vector();
      aValue = new Vector();
      Item.insertElementAt(anAttribute, 0);
      anAttribute.insertElementAt(aValue, 0);
      aValue.addElement(newData);
      return;
    }

    if(value == 0) svalue = 0;

    if(attr > Item.size()){
      int j = Item.size();
      for(int i = j; i < attr; i++){
        anAttribute = new Vector();
        aValue = new Vector();
        Item.addElement(anAttribute);
        anAttribute.addElement(aValue);
        aValue.addElement("");
      }
    }

    anAttribute = (Vector)Item.elementAt(attr-1);

    if(value == 0){
      aValue = new Vector();
      anAttribute.insertElementAt(aValue, 0);
      aValue.addElement(newData);
    }else{
      if(value > anAttribute.size()){
        int j = anAttribute.size();
        for(int i = j; i < value; i++){
          aValue = new Vector();
          anAttribute.addElement(aValue);
          aValue.addElement("");
        }
      }
      // add in the data
      aValue = (Vector)anAttribute.elementAt(value-1);
      if(svalue > aValue.size()){
        int j = aValue.size();
        for(int i = j; i < svalue; i++){
          aValue.addElement("");
        }
      }
      aValue.insertElementAt(newData, svalue-1);
    }
  }

  public void insert(int attr, int value, String newData){
    insert(attr, value, 0, newData);
  }

  public void insert(int attr, String newData){
    insert(attr, 0, 0, newData);
  }

/**
  * delete a string in a <attr, value, subvalue>
  * @param attr     attribute number
  * @param value    value number
  * @param svalue   subvalue number
  */
  public void delete(int attr, int value, int svalue){
    Vector anAttribute, aValue;

    if(attr == 0) return;
    if(value == 0) svalue = 0;

    if(attr > Item.size()) return;

    if(value == 0){
      Item.removeElementAt(attr-1);
      return;
    }
    anAttribute = (Vector)Item.elementAt(attr-1);
    if(value > anAttribute.size()) return;

    if(svalue == 0){
      anAttribute.removeElementAt(value-1);
      return;
    }
    aValue = (Vector)anAttribute.elementAt(value-1);
    if(svalue > aValue.size()) return;

    aValue.removeElementAt(svalue-1);
  }

  public void delete(int attr, int value){
    delete(attr, value, 0);
  }

  public void delete(int attr){
    delete(attr, 0, 0);
  }

  public String toString(){
    Vector anAttribute, aValue;
    String Result = new String("");

    int n = Item.size();
    for(int i = 0; i < n; i++){
      anAttribute = (Vector)Item.elementAt(i);
      int m = anAttribute.size();
      for(int j = 0; j < m; j++){
        aValue = (Vector)anAttribute.elementAt(j);
        int l = aValue.size();
        for(int k = 0; k < l; k++){
          Result = Result + (String)aValue.elementAt(k);
          if(k != l-1){
            Result = Result + "\u00fc";
          }
        }
        if(j != m-1){
          Result = Result + "\u00fd";
        }
      }
      if(i != n-1){
        Result = Result + "\u00fe";
      }
    }
    return(Result);
  }


  public int AMCount(){
    return(Item.size());
  }

  public int VMCount(int am){
    Vector anAttribute;

    if(am > Item.size() ) return 0 ;

    anAttribute = (Vector) Item.elementAt(am - 1);
    return anAttribute.size();
  }

  public int SVMCount(int am,int vm) {
    Vector anAttribute, anValue;

    if(am > Item.size() ) return 0 ;

    anAttribute = (Vector) Item.elementAt(am -1);

    if(vm > anAttribute.size() ) return 0 ;

    anValue = (Vector) anAttribute.elementAt(vm -1);
    return anValue.size();
  }

  public int dcount(String delim){
    return(Item.size());
  }
}
