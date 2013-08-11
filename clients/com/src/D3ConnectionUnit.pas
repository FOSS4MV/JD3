{

 Copyright (C) 2001 Christophe Marchal
 mccricri@yahoo.com
 http://jd3.sourceforge.com/

  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
}
unit D3ConnectionUnit;

interface

uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,
  scktcomp,

  JD3COMCommon,JD3COMConst,JD3COM_TLB,D3SessionUnit;

  {------------------------------------------------------------------------------
   D3Connection
  ------------------------------------------------------------------------------}
type

   TD3Connection = class(TComObject,D3Connection)
   public
    procedure Open(const pserver: WideString; pport: Integer; const puser, ppwd: WideString; ptype: Integer); safecall;
    function getTypeConnection: Integer; safecall;
    function getPort: Integer; safecall;
    function getUserName: WideString; safecall;
    function getServer: WideString; safecall;
    function createSession: D3Session; safecall;
    procedure Initialize; override;
    destructor Destroy; override;
    function login(const puser, ppwd: WideString): Integer; safecall;
    procedure logoff(const puser: WideString); safecall;

   private
    server,user,pwd : String;
    port,typecon : Integer;
    connected : boolean;
    socket : TClientSocket;

    function send(const cmd: WideString): WideString; safecall;

  end;


implementation

{------------------------------------------------------------------------------
 D3Connection
 ------------------------------------------------------------------------------}
procedure TD3Connection.Initialize;
begin
  inherited Initialize;
  self.connected := false;
  self.server := '';
  self.user := '';
  self.pwd := '';
  self.port := 0;
  self.typecon := 0;

  socket := nil;
end;


destructor TD3Connection.Destroy;
begin
  try
    if connected and (socket <> nil) then
       socket.Close;
  except
  end;

  inherited;
end;

procedure TD3Connection.Open(const pserver: WideString; pport: Integer; const puser, ppwd: WideString; ptype: Integer); safecall;
var
  newport : array[1..8] of char;
begin
     self.server := pserver;
     self.user := puser;
     self.pwd := ppwd;
     self.port := pport;
     self.typecon := ptype;

     // Verify type of connection
     if (typecon <> CONNECTION_TCP) and (typecon <> CONNECTION_TCP_PROXY ) then
        raise ED3ERROR.Create('Wrong type of connection');

     try
        socket := TClientSocket.Create(nil);
        socket.Host := server;
        socket.ClientType := ctBlocking;

        // Try to get the port for a free client
        if typecon = CONNECTION_TCP then
        begin
           socket.Port := port;
           socket.Open;

           socket.Socket.ReceiveBuf(newport,8);
           socket.Close;

           port := strtoint(newport);
        end;

        // Finally connect to the good server
        socket.Port := port;
        socket.Open;

        connected := true;


     except
       on E : Exception do
         raise ED3Error.Create('Error connection : ' + E.Message);
     end;

end;

function TD3Connection.getTypeConnection: Integer;
begin
 Result := self.typecon;
end;

function TD3Connection.getPort: Integer;
begin
  Result := self.port;
end;

function TD3Connection.getUserName: WideString;
begin
  Result := self.user;
end;

function TD3Connection.getServer: WideString;
begin
  Result := self.server;
end;

function TD3Connection.createSession: D3Session;
var
  sess : TD3Session;
begin
  sess := TD3Session.Create;
  sess.init(self);
  Result := sess;
end;

function TD3Connection.send(const cmd: WideString): WideString;
var
  msg  : String;
  len  : Array[1..8] of char;
  buf  : array[1..1024] of char;
  long,max,toread,lu : Integer;
begin
  msg := Format('%.8d%s',[Length(cmd),cmd]);
  socket.Socket.SendText(msg);

  socket.Socket.ReceiveBuf(len,8);
  long := StrToInt(len);
  max := long;

  msg := '';
  while max > 0 do
  begin
      if max > _MAXBLOCK then
        toread := _MAXBLOCK
      else
        toread := max;

      lu := socket.Socket.ReceiveBuf(buf,toread);
      if lu < 0 then
        raise ED3ERROR.Create('bug');

      max := max - lu;
      msg := msg + substr(buf,1,lu);
  end;
  Result := msg;
end;

function TD3Connection.login(const puser, ppwd: WideString): Integer;
begin
end;

procedure TD3Connection.logoff(const puser: WideString);
begin
end;

{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
 TComObjectFactory.Create(ComServer,TD3Connection,CLSID_D3Connection,'D3Connection','Connection to D3 server',ciMultiInstance);

end.
