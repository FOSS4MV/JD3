=======================================================================

(C)2000 Christophe Marchal
Released under GPL-2 license (see LICENSE)
mccricri@yahoo.com
http://www.sourceforge.com/projects/jd3

=======================================================================

This is the pick basic socket server.

To run it just type at TCL :

d3server

then the server is waiting for incomming connection on the port 20002.
You can specify your own port like this :

d3server -port 20002

The server start threads as phantoms. 
The number of threads is store in the equates PHANTOMS, 
and each thread has his own port. It begin at 30000 and end at (3000 + PHANTOMS - 1)

Example : 
  equ PHANTOMS   to  3
  
  there is 3 threads waiting on port 30000,30001 and 30002


You can also start one alone thread like this :

d3client -port 30000


For debugging :
you can "tandem" on the phantom line to see the message print by the subroutine
or start a tread with the -d option


=======================================================================

