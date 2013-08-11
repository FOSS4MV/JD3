#
# D3.py : The classes for connection to jd3 server
# Copyright (c) 2001 Christophe Marchal
#
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
#
# Christophe Marchal
# July, 2001
# Jose Ernesto Suarez
# June, 2008
# Version: 0.3
#
#------------------------------------------------------------------------------
# Implementation of jd3 Client library for Python
#------------------------------------------------------------------------------
import socket
from string import *

#------------------------------------------------------------------------------
# All important internal constants
#------------------------------------------------------------------------------
_D3_LOGON       = 1
_D3_LOGOFF      = 2
_D3_EXECUTE     = 3
_D3_CALL        = 4
_D3_SELECT      = 5
_D3_OPEN        = 6
_D3_READNEXT    = 7
_D3_READ        = 8
_D3_READU       = 9
_D3_WRITE       = 10
_D3_RELEASE     = 11
_D3_DELETE      = 12
_D3_CLOSE       = 13
_D3_TEST_EXIST  = 14
_D3_LOCK_ITEM   = 15
_D3_READV       = 16
_D3_SELECT_TCL  = 17
_D3_NOOP        = 18

_D3_SEPARATOR   = chr(001)

#------------------------------------------------------------------------------
# Useful constants
#------------------------------------------------------------------------------
D3_AM  = chr(254)
D3_VM  = chr(253)
D3_SVM = chr(252)

D3_ERR = 1
D3_OK  = 0

TRUE = 1
FALSE = 0

READ_FILENOTOPEN  = -1
READ_RECORDEMPTY  = -2
READ_RECORDLOCKED = -3

READNEXT_EOF = -1

USR_UNKNOW = -2
PWD_INCORRECT = -1

CONNECTION_TCP = 1
CONNECTION_TCP_PROXY = 2

# Exception used 
D3Exception = "D3Exception"

#------------------------------------------------------------------------------
# Main class for manage connection
#------------------------------------------------------------------------------
class D3Connection:
    def __init__(self,pserver = "localhost",pport = 20001,puser = "",ppwd = "",ptype = CONNECTION_TCP ):
        self.__server = pserver
        self.__port   = pport
        self.__user   = puser
        self.__type   = ptype

        # Init de la connexion
        if not(self.__initTcp() == D3_OK):
           raise D3Exception
        
        self.login(puser,ppwd)

    #
    # Close all connection and terminate with this object
    #
    def __del__(self):
        self.__socket.close()
        
    #
    # Init connection to server and get acces to D3Client
    #
    def __initTcp(self):
        newport = self.__port
        sock = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

        # Try to get the port number of a D3Client
        if self.__type == CONNECTION_TCP :
            try:
                sock.connect((self.__server,newport))
                buf = sock.recv(8)
                newport = int(buf) + 0
                sock.close()
            except:
                print "Connection error to main server " + self.__server + ":%i" % newport
                return(D3_ERR)

        self.__port = newport
        
        try:
           sock = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
           sock.connect((self.__server,newport))
           self.__socket = sock
        except:
            print "Connection error to D3Client " + self.__server + ":%i" % newport
            return(D3_ERR)
                
        return(D3_OK)

    #
    # Log in a user
    #
    def login(self,puser,ppwd):
        res = self.doit("%d%c%s%c%s%c" % (_D3_LOGON,_D3_SEPARATOR,puser,_D3_SEPARATOR,ppwd,_D3_SEPARATOR))
        return(res['status'])

    #
    # Log off the user
    #
    def logoff(self):
        self.doit("%d%c" % (_D3_LOGOFF,_D3_SEPARATOR))
        return(D3_OK)

        
    def getPort(self):
        return self.__port

    def getServer(self):
        return self.__server

    def getUserName(self):
        return self.__user


    #
    # Create new session
    #        
    def createSession(self):
        return D3Session(self)
        
    #
    # Send a message to the server and get response
    #
    def doit(self,buffer):

        # Send
        self.__socket.send("%08.d%s" %(len(buffer),buffer))

        # Receive
        long = self.__socket.recv(8)
        buf = self.__socket.recv(int(long))

        ret = {"status":D3_ERR,"data":[""]}

        pos = find(buf,_D3_SEPARATOR)
        if pos > 0:
            ret['status'] = int(buf[:pos])
            ret['data'] = split(buf[pos+1:],_D3_SEPARATOR)

        return ret
        

#------------------------------------------------------------------------------
# The D3Session class
#------------------------------------------------------------------------------
class D3Session:
    def __init__(self,pcon):
        self.__connection = pcon
        
    def noop(self):
        res = self.__connection.doit("%d%c" % (_D3_NOOP,_D3_SEPARATOR) )
        return  res['status']

    def execute(self,cmd):
        res = self.__connection.doit("%d%c%s%c" % (_D3_EXECUTE,_D3_SEPARATOR,cmd,_D3_SEPARATOR))
        if res['status'] != D3_OK:
            raise D3Exception
            
        return split(res['data'][0],D3_AM)

    def call(self,name,params):
        res = self.__connection.doit( "%d%c%s%c%d%c%s%c" % (_D3_CALL,_D3_SEPARATOR,name,_D3_SEPARATOR,len(params),_D3_SEPARATOR, join(params,_D3_SEPARATOR),_D3_SEPARATOR ) )    
        
        if res['status'] != D3_OK:
            raise D3Exception

        for i in range(len(params)):
            params[i] = res['data'][i]
            
        return res['status']

    def select(self,cmd):
        res = self.__connection.doit("%d%c%s%c" % (_D3_SELECT_TCL,_D3_SEPARATOR,cmd,_D3_SEPARATOR) )
        if res['status'] != D3_OK:
           raise D3Exception
           
        return D3SelectListTcl(split(res['data'][1],D3_AM))

    def openFile(self,filename,account = "",convert=""):
        return D3File(self.__connection,filename,account,convert)
    
    
#------------------------------------------------------------------------------
# D3SelectList
#------------------------------------------------------------------------------
class D3SelectListTcl:
    def __init__(self,array):
        self.__lst = array
        self.__curpos = 0
        
    def hasMoreElements(self):
        if self.__curpos < len(self.__lst):
            return TRUE
        else:
            return FALSE
            
    def getNextElement(self):
        if self.hasMoreElements():
            res = self.__lst[self.__curpos]
            self.__curpos += 1
            return res
        else:
            return ""
            
    def getNbElements(self):            
        return len(self.__lst)


class D3SelectListFile:
    def __init__(self,con,fd,nb):
        self.__connection = con
        self.__fd = fd
        self.__max = nb
        self.__curkey = ""
        self.__finished = FALSE

    def hasMoreElements(self):
        if self.__curkey == "":
           return self.__readnext()
      
        return TRUE

    def getNextElement(self):
        if self.hasMoreElements():
            result = self.__curkey
            self.__curkey = ""
            return result
        else:
            return ""

    def getNbElements(self):
        return self.__max

    def __readnext(self):
        if self.__curkey != "":
            return TRUE

        if self.__finished:
           return FALSE

        try:
            res = self.__connection.doit("%d%c%s%c" % (_D3_READNEXT,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR))

            if res['status'] == READNEXT_EOF:
                self.__finished = TRUE
                return FALSE

            if res['status'] != 0:
                return FALSE

            self.__curkey = res['data'][0]
            return TRUE

        except:
            return FALSE

        return FALSE

#------------------------------------------------------------------------------
# D3File
#------------------------------------------------------------------------------       
class D3File:
    def __init__(self,pcon,pfilename,paccount,pconvert):
        self.__connection = pcon
        self.__converter  = pconvert
        self.__account    = paccount
        self.__filename   = pfilename
        self.__fd         = 0
        
        res = self.__connection.doit("%d%c%s%c%s%c" % (_D3_OPEN,_D3_SEPARATOR,self.__account,_D3_SEPARATOR,self.__filename,_D3_SEPARATOR))
        if res['status'] != D3_OK:
            raise D3Exception
            
        self.__fd = int(res['data'][0])
        return        

         
    def getFileName(self): 
        return self.__filename
     
    def getAccountName(self):
        return self.__account
        
    def read(self,key,item):
        res = self.__connection.doit("%d%c%d%c%s%c" % (_D3_READ,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR))

        item.setRecord(res['data'][0])

        if self.__converter != "":
            self.__convertAfterRead(item)
        
        return res['status']

    def readv(self,key,am):
        res = self.__connection.doit("%d%c%d%c%s%c%d%c" % (_D3_READV,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR,am,_D3_SEPARATOR))

        if res['status'] == D3_OK:
            return res['data'][0]
        else:
            return ""

    def readu(self,key,item):
        res = self.__connection.doit("%d%c%d%c%s%c" % (_D3_READU,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR))

        if res['status'] == D3_OK:
           item.setRecord(res['data'][0])
        
           if self.__converter != "":
              self.__convertAfterRead(item)
        
        return res['status']

    def lock(self,key):
        res = self.__connection.doit("%d%c%d%c%s%c" % (_D3_LOCK_ITEM,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR))

        return res['status']


    def write(self,key,item):
        # Copy the item for internal manipulation
        tmp = D3Item(item.__str__() )
        if self.__converter != "":
            self.__convertBeforeWrite(tmp)

        res = self.__connection.doit("%d%c%d%c%s%c%s%c" % (_D3_WRITE,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR,tmp.__str__(),_D3_SEPARATOR))

        return res['status']

    def delete(self,key):
        res = self.__connection.doit("%d%c%d%c%s%c" % (_D3_DELETE,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR))

        return res['status']

    def testExist(self,key):
        res = self.__connection.doit("%d%c%d%c%s%c" % (_D3_TEST_EXIST,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR))

        if res['status'] == D3_OK :
           return (int(res['data'][0]) == 0)

        return FALSE

    def release(self,key):
        res = self.__connection.doit("%d%c%d%c%s%c" % (_D3_RELEASE,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR,key,_D3_SEPARATOR))

        return res['status']

    def select(self):
        res = self.__connection.doit("%d%c%d%c" % (_D3_SELECT,_D3_SEPARATOR,self.__fd,_D3_SEPARATOR))

        return D3SelectListFile(self.__connection,res['data'][0],int(res['data'][1]))

        
    # Convert item readed from file with the good converter        
    def __convertAfterRead(self,item):
        for i in range(item.AMCount()):
            for j in range(item.VMCount(i+1)):
                for k in range(item.SVMCount(i+1,j+1)):
                    item.replace(self.__converter.afterReadConversion(item.extract(i+1,j+1,k+1)),i+1,j+1,k+1)
    
    def __convertBeforeWrite(self,item):
        for i in range(item.AMCount()):
            for j in range(item.VMCount(i+1)):
                for k in range(item.SVMCount(i+1,j+1)):
                    item.replace(self.__converter.beforeWriteConversion(item.extract(i+1,j+1,k+1)),i+1,j+1,k+1)

#------------------------------------------------------------------------------
# D3Item        
#------------------------------------------------------------------------------
class D3Item:
    def __init__(self,prec = ""):
        self.__origrec = prec
        self.setRecord(prec)
     

    def setRecord(self,newone):
        self.__record = []
        rec = split(newone,D3_AM)
        nbam = len(rec)
        for i in range(nbam):
            vam = split(rec[i],D3_VM)
            nbvm = len(vam)
            lvm = []
            for j in range(nbvm):
                svm = split(vam[j],D3_SVM)
                lvm.append(svm)
            self.__record.append(lvm)


    def extract(self,am = 0,vm = 0, svm = 0):
        am -= 1
        vm -= 1
        svm -= 1

        if vm == -1 :
            svm = -1

        # Get the entire record
        if am == -1:
           res = []
           for am in range(len(self.__record)):
               res.append(self.extract(am+1))
           return join(res,D3_AM)

        # Get an attribute that doesn't exist
        if am >= len(self.__record):
            return ""

        # Get the entire attribute
        if vm == -1:
            tmp = []
            for vm in range(len(self.__record[am])):
                tmp.append(self.extract(am+1,vm+1))
            return join(tmp,D3_VM)


        # Get a Multivalue that doesn't exist
        if vm >= len(self.__record[am]):
            return ""

        # Get the entire Multivalue
        if svm == -1:
            return join(self.__record[am][vm],D3_SVM)

        # Get a subvalue that doesn't exist
        if svm >= len(self.__record[am][vm]):
            return ""

        # Get the subvalue
        return self.__record[am][vm][svm]


    def delete(self,am = 0, vm = 0 , svm = 0):
        if am == 0 :
           return

        if vm == 0 :
            svm = 0

        if am > len(self.__record):
            return

        if vm == 0:
           del self.__record[am-1]
        else:
            if vm > len(self.__record[am-1]):
               return

            if svm == 0:
               del self.__record[am-1][vm-1]
            else:
                if svm > len(self.__record[am-1][vm-1]):
                    return
                del self.__record[am-1][vm-1][svm-1]

    def replace(self,newval,am = 0, vm = 0, svm = 0):
        if am == 0:
           return

        if vm == 0:
            svm = 0

        for i in range(len(self.__record),am):
            self.__record.append([[]])

        if vm == 0:
            self.__record[am-1] = [[newval]]
        else:
            for i in range(len(self.__record[am-1]),vm):
                self.__record[am-1].append([])

            if svm == 0:
                self.__record[am-1][vm-1] = [newval]
            else:
                for i in range(len(self.__record[am-1][vm-1]),svm):
                    self.__record[am-1][vm-1].append("")
            
                self.__record[am-1][vm-1][svm-1] = newval

        # To re-translate all the item into a list
        # --> you can replace a SVM by an attribute
        self.setRecord(self.__str__())

    def insert(self,newval,am = 0,vm = 0,svm = 0):
        if am == 0:
           return

        if vm ==0:
            svm = 0

        if am > len(self.__record):
            self.replace(newval,am,vm,svm)
            return

        if vm == 0:
           # insert am
           self.__record.insert(am-1,[[newval]])   
        else: 
            if vm > len(self.__record[am-1]):
                self.replace(newval,am,vm,svm)
                return
    
            if svm == 0:
               # insert vm
               self.__record[am-1].insert(vm-1,[newval])
            else:
                if svm > len(self.__record[am-1][vm-1]):
                   self.replace(newval,am,vm,svm)
                   return
    
                #insert svm
                self.__record[am-1][vm-1].insert(svm-1,newval)

        # To re-translate all the item into a list
        self.setRecord(self.__str__())


    def AMCount(self):
        return len(self.__record)


    def VMCount(self,am):
        if am == 0 or am > len(self.__record):
           return 0

        return len(self.__record[am-1])


    def SVMCount(self,am,vm):
        if am == 0 or am > len(self.__record):
           return 0

        if vm == 0 or vm > len(self.__record[am-1]):
            return 0
    
        return len(self.__record[am-1][vm-1])


    def __str__(self):
        return self.extract()


#------------------------------------------------------------------------------
# Default converter, all converter must derived from this one and
# overwrite the 2 methods
#------------------------------------------------------------------------------
class D3EscapeConverter:
    def __init__(self):
        pass

    def beforeWriteConversion(self,str):
        return str
        
    def afterReadConversion(self,str):
        return str

#------------------------------------------------------------------------------
# For french accent
#------------------------------------------------------------------------------
class Ansi2UnicodeConverter(D3EscapeConverter):
    def __init__(self):
        D3EscapeConverter.__init__(self)

    def beforeWriteConversion(self,str):
        newval = ""
        for i in str:
            if  i == "é":
                newval += '\202'
            elif  i == "è":
                newval += '\212'
            elif  i == "ê":
                newval += '\210'
            elif  i == "ë":
                newval += '\211'
            elif  i == "à":
                newval += '\205'
            elif  i == "á":
                newval += '\240'
            elif  i == "â":
                newval += '\203'
            elif  i == "ä":
                newval += '\204'
            elif  i == "û":
                newval += '\226'
            elif  i == "ü":
#                newval += '\201'
#               because it's chr(252) in python
                 newval += "u"
            elif  i == "ú":
                newval += '\243'
            elif  i == "ù":
                newval += '\227'
            elif  i == "î":
                newval += '\214'
            elif  i == "í":
                newval += '\241'
            elif  i == "ì":
                newval += '\215'
            elif  i == "ï":
                newval += '\213'
            elif  i == "ô":
                newval += '\223'
            elif  i == "ó":
                newval += '\242'
            elif  i == "ò":
                newval += '\225'
            elif  i == "ö":
                newval += '\224'
            elif  i == "ç":
                newval += '\207'
            else:
                newval += i

        return newval

    def afterReadConversion(self,str):
        newval = ""
        for i in str:
            if  i == '\202':
                newval += 'é'
            elif i == '\212':
                newval += "è"
            elif i == '\210':
                newval += "ê"
            elif i == '\211':
                newval += "ë"
            elif i == '\205':
                newval += "à"
            elif i == '\240':
                newval += "á"
            elif i == '\203':
                newval += "â"
            elif i == '\204':
                newval += "ä"
            elif i == '\226':
                newval += "û"
            elif i == '\201':
#                newval += 'ü'
#               because it's chr(252) in python
                 newval += "u"
            elif i == '\243':
                newval += "ú"
            elif i == '\227':
                newval += "ù"
            elif i == '\214':
                newval += "î"
            elif i == '\241':
                newval += "í"
            elif i == '\215':
                newval += "ì"
            elif i == '\213':
                newval += "ï"
            elif i == '\223':
                newval += "ô"
            elif i == '\242':
                newval += "ó"
            elif i == '\225':
                newval += "ò"
            elif i == '\224':
                newval += "ö"
            elif i == '\207':
                newval += "ç"
            else:
                newval += i

        return newval
        
#------------------------------------------------------------------------------
# Main function for the package to test this package
#------------------------------------------------------------------------------
if __name__ == "__main__":
    print "Ok je lance la connexion"
    con = D3Connection("linuxdev",40005,"cm","",CONNECTION_TCP_PROXY)        
#    con = D3Connection("localhost",20002,"cm","",CONNECTION_TCP_PROXY)        
    sess = con.createSession()
    conv = Ansi2UnicodeConverter()
    fich = sess.openFile("jbasic","",conv)

    rec = D3Item()

    fich.read("toto2",rec)
    print "rec",rec 

    str = rec.extract(1)
    print "str,",str
    rec.replace(str,2)

    print "write"
    fich.write("toto3",rec)

    print "\n\nC'est fini pour le test de D3"

