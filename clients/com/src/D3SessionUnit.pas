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
unit D3SessionUnit;

interface

uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,

  JD3COM_TLB , JD3COMCommon, JD3COMConst, TD3BufferUnit, D3SelectListTclUnit, D3FileTcpUnit;

type
  TD3Session = Class(TComObject,D3Session)
  public
    procedure init(const pcon: D3Connection);safecall;
    function noop: Integer; safecall;
    function call(const name: WideString;const params: D3Params): Integer; safecall;
    function execute(const sentence: WideString): WideString; safecall;
    function select(const sentence: WideString): D3SelectList; safecall;
    function openFile(const paccount, pfilename: WideString; const pconverter: D3EscapeConverter): D3File; safecall;
    procedure Initialize; override;
    destructor Destroy; override;

  private
    con : D3Connection;

  end;

implementation

{------------------------------------------------------------------------------
  Constructor for COM
------------------------------------------------------------------------------}
procedure TD3Session.Initialize;
begin
   inherited;
   con := nil;
end;

{------------------------------------------------------------------------------
  Clean stuff
------------------------------------------------------------------------------}
destructor TD3Session.Destroy;
begin
   inherited;
end;

{------------------------------------------------------------------------------
  Initialize the session with a connection
------------------------------------------------------------------------------}
procedure TD3Session.init(const pcon : D3Connection);
begin
  self.con := pcon;
end;

{------------------------------------------------------------------------------
  Just test the network
------------------------------------------------------------------------------}
function TD3Session.noop: Integer;
var
  res : String;
  buf : TD3Buffer;
begin
  if con = nil then
    raise ED3ERROR.Create('Session not initialized');

  res := con.send(IntToStr(D3_NOOP) + D3_SEPARATOR);
  buf := dataFormat(res);
  Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
  Execute a TCL sentence, and get the capturing result
------------------------------------------------------------------------------}
function TD3Session.execute(const sentence: WideString): WideString;
var
  res : String;
  buf : TD3Buffer;
begin
  if con = nil then
    raise ED3ERROR.Create('Session not initialized');

  res := con.send(IntToStr(D3_EXECUTE) + D3_SEPARATOR + sentence + D3_SEPARATOR );
  buf := dataFormat(res);

  if buf.getStatus <> D3_OK then
     raise ED3Error.Create('Error executing ' + sentence);

  Result := PChar(buf.getdata[0]);

end;

{------------------------------------------------------------------------------
  Call a subroutine
------------------------------------------------------------------------------}
function TD3Session.call(const name: WideString;const params: D3Params): Integer;
var
  msg,res : String;
  i : Integer;
  buf : TD3Buffer;
begin
  if con = nil then
    raise ED3ERROR.Create('Session not initialized');

  msg := IntToStr(D3_CALL) + D3_SEPARATOR + name + D3_SEPARATOR + IntToStr(params.getSize) + D3_SEPARATOR;

  for i := 1 to params.getSize do
  begin
      msg := msg + params.getParam(i) + D3_SEPARATOR;
  end;

  res := con.send(msg);
  buf := dataFormat(res);

  Result := D3_ERR;
  if buf.getStatus <> D3_OK then
     exit;

  for i := 1 to params.getSize do
  begin
     params.setParam(buf.getData[i-1],i);
  end;

  Result := D3_OK;
end;

{------------------------------------------------------------------------------
  Execute an AQL sentence and get a Select list
------------------------------------------------------------------------------}
function TD3Session.select(const sentence: WideString): D3SelectList;
var
  res : String;
  buf : TD3Buffer;
  list : TD3SelectListTcl;
begin
  if con = nil then
    raise ED3ERROR.Create('Session not initialized');

  res := con.send(IntToStr(D3_SELECT_TCL) + D3_SEPARATOR + sentence + D3_SEPARATOR);
  buf := dataFormat(res);

  list := TD3SelectListTcl.Create;
  if buf.getData.count > 1 then
    list.init(buf.getData[1])
  else
    list.init('');

  Result := list;
end;

{------------------------------------------------------------------------------
  Open a file
------------------------------------------------------------------------------}
function TD3Session.openFile(const paccount, pfilename: WideString; const pconverter: D3EscapeConverter): D3File;
var
   tmp : TD3FileTcp;
begin
    tmp := TD3FileTcp.Create;
    tmp.init(con,paccount,pfilename,pconverter);

    Result := tmp;
end;

{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
  TComObjectFactory.Create(ComServer,TD3Session,CLSID_D3Session,'D3Session','Session',ciMultiInstance);

end.
