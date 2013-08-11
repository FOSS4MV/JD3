<?
/*
 * D3.inc : The classes for connection to jd3 server
 * Copyright (c) 2000 Christophe Marchal
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
 *
 * Christophe Marchal
 * Cedric Fontaine
 * Octobre, 2003
 * Version: 0.8
 */

if ( ! defined( '_D3_CLASS' ) ) {
  define('_D3_CLASS', true );

  /*
  * Define somes constants
  */
  define('_D3_LOGIN_',1);
  define('_D3_LOGOFF_',2);
  define('_D3_EXECUTE_',3);

  define('_D3_CALL_',4);
  define('_D3_SELECT_',5);
  define('_D3_OPEN_',6);
  define('_D3_READNEXT_',7);
  define('_D3_READ_',8);
  define('_D3_READU_',9);
  define('_D3_WRITE_',10);
  define('_D3_RELEASE_',11);
  define('_D3_DELETE_',12);
  define('_D3_CLOSE_',13);
  define('_D3_TEST_EXIST_',14);
  define('_D3_LOCK_ITEM_',15);
  define('_D3_READV_',16);
  define('_D3_NOOP_',18);
  define('_D3_SELECT_TCL_',17);
  define('_D3_STATUS_',19);
  
  define('_D3_SEP_',chr(001));
  define('_D3_AM_',chr(254));
  define('_D3_VM_',chr(253));
  define('_D3_SVM_',chr(252));
  define('_D3_DIFFDATE_',732);
  define('_D3_DIFFTIME_',86400000);
  
  define('_D3_CONNECTION_TCP_',1);
  define('_D3_CONNECTION_TCP_PROXY_',2);
 
  
  /**
  * The class for connection to jd3 server
  */
  class D3Connection {

    // Private variable, read it but never change it
    var $Port;
    var $Server;
    var $UserName;
    var $ErrMsg;
    var $Type;
    var $Socket;
    var $Connected;
    var $TimeOut = 60;
    var $SendResult;
    
    /**
    * Create new connection
    * @parms $serv Adress of the server
    * @parms $port The port for the server
    * @parms $user The user name
    * @parms $passwd The password of the user
    * @parms $type Type of connection (via proxy or not)
    */
    function D3Connection($serv="localhost",$port = 20002, $user="",$passwd = "",$type=_D3_CONNECTION_TCP_){
        $this->Port = $port;
        $this->Server = $serv;
        $this->Type = $type;
        $this->UserName = $user;
        $this->Connected = false;
        
        if($type != _D3_CONNECTION_TCP_ && $type != _D3_CONNECTION_TCP_PROXY_) return false;

        if($this->open() ){
          return $this->login($user,$passwd);    
        }else{
          return false;
        }
    }
    
    /**
    * Open the connection, automatically called by the constructor
    */
    function open(){
       if($this->Connected) { return true; }
       
       $this->Socket = fsockopen($this->Server,$this->Port);
       if(!$this->Socket) {
          $this->ErrMsg = "Erreur de connection au serveur".$this->Serveur.":".$this->Port;
          return false;
       }
      
       if($this->Type == _D3_CONNECTION_TCP_) {
         $newport = fgets($this->Socket,9);
         @fclose($this->Socket);

         if ($newport == -1) {
           $this->ErrMsg = "No free connection available";
           return false;
         }else{
           if($newport > 0) {
             $this->Port = $newport;
             $this->Socket = fsockopen($this->Server,$this->Port);
             if(!$this->Socket) {
               $this->ErrMsg = "Erreur de connection au serveur".$this->Serveur.":".$this->Port;
               return false;
             }
           }else{
              $this->ErrMsg = "New port received [$newport] is incorrect";
              return false;
           }
         }
         
       }
       
       $this->Connected = true;
      
       return true;   
    }
    
    /**
    * Close the connection
    */
    function Close(){
       if($this->Connected) {
         @fclose($this->Socket);
         $this->Connected = false;         
       }
       return true;       
    }
    
    /**
    * Create new session
    */
    function createSession(){
        return new D3Session($this);    
    }

    /**
    * Log in the user
    * @parms $user The user code
    * @parms $passwd The password
    * @return 0  if the user/password is correct
    *         -1 if the user is unknow
    *         -2 if the password is incorrect
    */
    function login($user,$passwd){
       $this->UserName = $user;
       $this->send(_D3_LOGIN_._D3_SEP_.$user._D3_SEP_.$passwd._D3_SEP_);      
       
       return $this->SendResult;
    }
    
    /**
    * Logoff the user
    */
    function logoff(){
        $this->send(_D3_LOGOFF_._D3_SEP_);
    }

    /**
    * Send information to the serveur, used only by D3Classes
    * @parms $msg The request
    * @return Array of data from the server
    */
    function send($msg) {

        set_time_limit($this->TimeOut);

		$this->SendResult = -1;

        if(!$this->Connected) {
          $this->ErrMsg = "Send : Pas de connexion";
          $res = array();
          return false;
        }
        
        if(empty($msg)) {
          $this->ErrMsg = "Send : Rien a écrire";
          $res = array();
          return false;
        }

        fwrite($this->Socket,sprintf("%08d",strlen($msg))."$msg" );

        $lonrep = fread($this->Socket,8);
        $reply="";
	if ($lonrep<2048)
		$restrep=$lonrep;
	else
		$restrep=2048;
	while ($restrep>0) {
		$reply .= fread($this->Socket, $restrep);
		$lonrep=$lonrep-$restrep;
		if ($lonrep<2048)
			$restrep=$lonrep;
		else
			$restrep=2048;
	}
        $res = explode(_D3_SEP_,$reply);

        $this->SendResult = $res[0];

        array_splice($res,0,1);

        return $res;
    }

    /**
    * Get server status
    */
    function status() {
        $statut = $this->send(_D3_STATUS_);
        return $statut[0];
    }
    
  } //D3Connection



  class D3Session {
    var $ErrMsg;
    var $Conn;
    
    function D3Session($con){
        $this->Conn = $con;
    }
    

    function noop() {
       $this->Conn->send(_D3_NOOP_._D3_SEP_."noop"._D3_SEP_);
       return $this->Conn->SendResult;
    }

    function execute($cmd) {
        if(empty($cmd)) {
            $this->ErrMsg = "Execute : Pas de commande";
            return "";
        }
        $params = $this->Conn->send(_D3_EXECUTE_._D3_SEP_.$cmd._D3_SEP_);
        return $params[0];
    }

    // Appel d'une sous routine
    function call($name , &$params) {
        if(empty($name)) {
           $this->ErrMsg = "Call : Pas de nom de sous-routine";
           return false;
        }
        $nbpar = count($params);
        if(!is_array($params) or $nbpar < 1 ) {
                $this->ErrMsg = "Call : Pas de paramètres ou pas de tableau";
                return false;
        }

        $buf = _D3_CALL_._D3_SEP_.$name._D3_SEP_.$nbpar._D3_SEP_;
        for ($i = 0; $i < $nbpar; $i++) {
            $buf .= $params[$i]._D3_SEP_;
        }

        $params = $this->Conn->send($buf);
            
        if($this->Conn->SendResult == "0") {
           return true;
        } else {
           $this->ErrMsg = "Call : Erreur d'execution";
           return false;
        }
    }

    function getErrMsg(){
       return $this->ErrMsg;    
    }

    function select($sentence){
        $res = $this->Conn->send(_D3_SELECT_TCL_._D3_SEP_.$sentence);
        return new D3SelectListTcl(explode(_D3_AM_,$res[1]));
    }
    
    function openFile($account,$filename=""){
        return new D3File($this->Conn,$account,$filename);
    }


  } //Classe D3Session
 

  class D3SelectListTcl { 
    var $lst;
    var $nbelements;
    var $cpt;
    
    function  D3SelectListTcl($array){
        $this->lst = $array;
        $this->nbelements = count($this->lst);
        $this->cpt = 0;
    }
    
    function hasMoreElements(){
        if($this->cpt < $this->nbelements) return true;
        else return false;
    }
    
    function getNextElement(){
       if($this->hasMoreElements() ) {
          $tmp = $this->lst[$this->cpt];
          $this->cpt++;
          return $tmp;
       }
       else{
          return "";
       }
    }
    
    function getNbElements(){
        return $this->nbelements;    
    }

  } // D3SelectListTcl

  class D3SelectListFile { 
    var $fd;
    var $nbelements;
    var $Conn;
    var $curkey;
    var $finished;
    
    function  D3SelectListFile($con,$nb,$fd){
        $this->fd = $fd;
        $this->Conn = $con;
        $this->nbelements = $nb;
        $this->cpt = 0;
        $this->curkey = "";
        $this->finished = false;
    }
    
    function hasMoreElements(){
        if($this->curkey == ""){
            return $this->readNext();
        }else return true;
        
    }

    function readNext(){
        if($this->curkey != "") return true;
        
        if($this->finished) return false;
        
        $rep = $this->Conn->send(_D3_READNEXT_._D3_SEP_.$this->fd._D3_SEP_);
        if( $this->Conn->SendResult == "0") {
            $this->curkey = $rep[0];    
            return true;
        }else{
            $this->finished = true;
            return false;
        }
        
    }
    
    function getNextElement(){
        if($this->hasMoreElements()){
            $tmp = $this->curkey;
            $this->curkey = "";
            return $tmp;
        }else{
           return "";
        }
    }
    
    function getNbElements(){
        return $this->nbelements;    
    }

  } // D3SelectList



  class D3File{
 	// Private
 	var $ErrMsg;
 	var $Conn;
 	var $FD;
 	var $FileName;
 	var $Account;
 	var $Opened;
 	var $Status;
 	
 	function D3File($con , $account, $filename =""){

 		$this->Conn = $con;

 		if(empty($filename) ){
 		   $this->FileName = $account;
 		   $this->Account = "";
 		} else {
 			$this->FileName = $filename;
 			$this->Account = $account;
 		}

 		$this->Opened = false;
 		
 		if(!$con->Connected) {
 			$this->ErrMsg = "Read : pas de connection";
 			return; 			
 		}

 		$rep = $this->Conn->send(_D3_OPEN_._D3_SEP_.$this->Account._D3_SEP_.$this->FileName._D3_SEP_);
 		if($this->Conn->SendResult == 0) {
			$this->FD = $rep[0];
 			$this->Opened = true;
 		}
 	}
 	
 	function read($key,&$item) {
 		$rep = $this->Conn->send(_D3_READ_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_);
 		$this->Status = $this->Conn->SendResult;
 		$item->setRecord($rep[0]);
 		
 		if( $this->Status == 0 ){
 		} elseif($this->Status == -3 ) {
 			$this->ErrMsg = "Read : Record locké";
 		} elseif($this->Status == -2 ) {
 			$this->ErrMsg = "Read : Record inexistant";
 		} elseif($this->Status == -1 ) {
 			$this->ErrMsg = "Read : fichier pas ouvert";
 		}
 		return $this->Status;
 			
 	} // Read
 	
 	function readv($key,$am){
 	    $rep = $this->Conn->send(_D3_READV_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_.$am._D3_SEP_);
 		$this->Status = $this->Conn->SendResult;
        return $rep[0]; 	    
 	}
 	
 	function readu($key,&$item){
 		$rep = $this->Conn->send(_D3_READU_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_);
 		$this->Status = $this->Conn->SendResult;
 		$item->setRecord($rep[0]);
 		
 		if( $this->Status == 0 ){
 		} elseif($this->Status == -3 ) {
 			$this->ErrMsg = "Read : Record locké";
 		} elseif($this->Status == -2 ) {
 			$this->ErrMsg = "Read : Record inexistant";
 		} elseif($this->Status == -1 ) {
 			$this->ErrMsg = "Read : fichier pas ouvert";
 		}
 		return $this->Status; 	    
 	}

    function write($key,$item){
        $rep = $this->Conn->send(_D3_WRITE_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_.$item->toString()._D3_SEP_);
 		$this->Status = $this->Conn->SendResult;
        return $this->Status;
    }
    
    function delete($key){
        $rep = $this->Conn->send(_D3_DELETE_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_);    
 		$this->Status = $this->Conn->SendResult;
        return $this->Status;        
    }
 	
    function testExist($key){
        $rep = $this->Conn->send(_D3_TEST_EXIST_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_);    
 		$this->Status = $this->Conn->SendResult;
        if($this->Status == "0" && $rep[0] == "0") return true;
        return false;                
    }
    
    function release($key){
        $rep = $this->Conn->send(_D3_RELEASE_._D3_SEP_.$this->FD._D3_SEP_.$key._D3_SEP_);    
 		$this->Status = $this->Conn->SendResult;
        return $this->Status;                
    }

    function select(){
        $rep = $this->Conn->send(_D3_SELECT_._D3_SEP_.$this->FD._D3_SEP_);    
 		$this->Status = $this->Conn->SendResult;
        return new D3SelectListFile($this->Conn,$rep[1],$rep[0]);
     }
 	
 } // D3File


  class D3Item {
	var $Record;
	var $origrec;
	
	function D3Item($rec = ""){
		if(empty($rec)){
			$this->Record = Array();
			$this->origrec = $this->Record;
			return;
		}
		$this->setRecord($rec);
    }

    function setRecord($newval){
        $rec = explode(_D3_AM_,$newval);
        $nbam = count($rec);
		$this->Record = Array();
		$this->origrec = $newval;
		for($am = 0;$am < $nbam;$am++) {
			$vam = explode(_D3_VM_,$rec[$am]);
			$nbvm = count($vam);
			$lvm = Array();
			for($vm = 0;$vm < $nbvm;$vm++){
				$vvm = explode(_D3_SVM_,$vam[$vm]);
				$lvm[$vm] = $vvm;
			}
			$this->Record[$am] = $lvm;
		}
		return true;
	}
	
	function toString(){
	   return $this->extract();    
	}
	
	function extract($am = 0,$vm = 0,$svm = 0) {
		$am --;
		$vm --;
		$svm --;
		if($vm == -1) $svm = -1;
		
		if($am == -1) {
			$nbam = count($this->Record);
			$res = "";
			for($am = 0;$am<$nbam;$am++) {
				$nbvm = count($this->Record[$am]);
				$lvm = Array();
				for($vm= 0;$vm<$nbvm;$vm++){
					if(is_array($this->Record[$am][$vm])){
					   $lvm[$vm] = implode(_D3_SVM_,$this->Record[$am][$vm]);
					}else{
					   $lvm[$vm] = $this->Record[$am][$vm];
					}
				}
				if(is_array($lvm) ){
				   $res[$am] = implode(_D3_VM_,$lvm);
				} else {
  			   	   $res[$am] = $lvm;
				}
			}
			if(is_array($res)) {
			   return implode(_D3_AM_,$res);
			} else {
			   return $res;
			}
		}
		
		if($vm == -1) {
			$nbvm = count($this->Record[$am]);
			$lvm = Array();
			for($vm= 0;$vm<$nbvm;$vm++){
				$lvm[$vm] = implode(_D3_SVM_,$this->Record[$am][$vm]);
			}
			return implode(_D3_VM_,$lvm);
		}

		if($svm == -1 ){
	       return implode(_D3_SVM_,$this->Record[$am][$vm]);
	    } else {
	    	return $this->Record[$am][$vm][$svm];
	    }
	}
	
	function replace($newval,$am = 0, $vm = 0 , $svm = 0){
        if($am == 0) return;
        if($vm == 0) $svm = 0;
        
        for($i=count($this->Record);$i < $am;$i++){
            $this->Record[$i]= Array(""); 
        }	    
        
        if($vm == 0) {
            $this->Record[$am-1] = array($newval);    
        }else{
        
           for($i = count($this->Record[$am-1]);$i < $vm ; $i++){
                $this->Record[$am-1][$i] = Array(""); 
           }
           
           if($svm == 0) {
              $this->Record[$am-1][$vm-1] = Array($newval);
           }else{
               for($i = count($this->Record[$am-1][$vm-1]);$i < $svm ; $i++){
                    $this->Record[$am-1][$vm-1][$i] = Array(""); 
               }

               $this->Record[$am-1][$vm-1][$svm-1] = Array($newval);
           }
        }
	}

    function ins(&$tab,$pos,$newval){
        $fin = $tab;
        $nb = count($tab);
        array_splice($tab,-($nb-$pos),($nb-$pos));
        array_splice($tab,-1,1,array($newval,""));
        array_splice($fin,0,($pos-1));
        array_splice($tab,-1,1,$fin);
    }
    	
	function insert($newval, $am = 0,$vm = 0,$svm = 0){
        if($am == 0) return;
        if($vm == 0) $svm = 0;
        
        if ($am > count($this->Record) ) {
            $this->replace($newval,$am);
            return;
        }
        
        if($vm == 0) {
            $this->ins($this->Record,$am,array($newval));
        }else{
            if($vm > count($this->Record[$am-1])) {
                $this->replace($newval,$am,$vm);
                return;
            }
           
            if($svm == 0) {
               $this->ins($this->Record[$am-1],$vm,$newval);
            }else{
               for($i = count($this->Record[$am-1][$vm-1]);$i < $svm -1 ; $i++){
                   $this->Record[$am-1][$vm-1][$i] = Array(""); 
               }

               $this->ins($this->Record[$am-1][$vm-1],$svm,$newval);
            }
        }
	}
	
	function delete($am = 0 ,$vm = 0,$svm = 0) {
	    if($am == 0) return;
	    if($vm == 0) $svm =0;
	    
        if($vm == 0){
           array_splice($this->Record,$am - 1,1);
        }else{
           if($svm == 0) {
                array_splice($this->Record[$am-1],$vm - 1,1);
           }else{
                array_splice($this->Record[$am-1][$vm-1],$svm - 1,1);
           }
        }
	       
	}
	
	function AMCount(){
	    return count($this->Record);
	}
	
	function VMCount($am){
	    if ($am > count($this->Record) || $am == 0) return 0;
	    $tmp = $this->Record[$am-1];
	    
	    return count($tmp);
    }
	
	
	function SVMCount($am,$vm){
	    if ($am > count($this->Record) || $am == 0) return 0;
	    
	    $tmp = $this->Record[$am-1];

	    if ($vm > count($tmp) || $vm == 0 ) return 0;
	    
	    $tmp = $tmp[$vm-1];
	    
	    return count($tmp);
	}
	
	
  } // D3Item


 
 
}
?>
