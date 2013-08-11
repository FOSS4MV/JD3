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
unit D3FileTcpUnit;

interface
uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,
  scktcomp,

  JD3COMCommon,JD3COMConst,JD3COM_TLB,TD3BufferUnit,D3ItemUnit,D3SelectListFileUnit;

type
    TD3FileTcp = class(TComObject,D3File)
    public
      procedure init(const con: D3Connection; const paccount, pfilename: WideString; const converter: D3EscapeConverter); safecall;
      function getFileName: WideString; safecall;
      function getAccountName: WideString; safecall;
      function read(const key: WideString; var item: D3Item): Integer; safecall;
      function readv(const key: WideString; am: Integer): WideString; safecall;
      function readu(const key: WideString; var item: D3Item): Integer; safecall;
      function lock(const key: WideString): Integer; safecall;
      function write(const key: WideString; const item: D3Item): Integer; safecall;
      function delete(const key: WideString): Integer; safecall;
      function testExist(const key: WideString): WordBool; safecall;
      function Release(const key: WideString): Integer; safecall;
      function select: D3SelectList; safecall;
      procedure Initialize; override;
      destructor Destroy; override;

    private
      fd : Integer;
      initialized : Boolean;
      con : D3Connection;
      filename,accountname : String;
      converter : D3EscapeConverter;

      procedure convertAfterRead(item : D3Item);
      procedure convertBeforeWrite(item : D3Item);

    end;

implementation

{------------------------------------------------------------------------------
  constructor for COM
-------------------------------------------------------------------------------}
procedure TD3FileTcp.Initialize;
begin
    inherited;
    initialized := false;
    fd := 0;
    con := nil;
    converter := nil;
    filename := '';
    accountname := '';
end;

{------------------------------------------------------------------------------
  clean stuff
-------------------------------------------------------------------------------}
destructor TD3FileTcp.Destroy;
begin
   inherited;
end;

{------------------------------------------------------------------------------
  Initialize the file descriptor 
-------------------------------------------------------------------------------}
procedure TD3FileTcp.init(const con: D3Connection; const paccount, pfilename: WideString; const converter: D3EscapeConverter);
var
   buf : TD3Buffer;
   res : STring;
begin
   self.con := con;
   self.filename := pfilename;
   self.accountname := paccount;
   res := con.send(IntToStr(D3_OPEN) + D3_SEPARATOR + paccount + D3_SEPARATOR + pfilename + D3_SEPARATOR );
   buf := dataFormat(res);

   if buf.getStatus <> D3_OK then
      raise ED3Error.Create('Could not open ' + paccount + ',' + pfilename + ',');

   try
      self.fd := StrToInt(buf.getData[0]);
   except
    on EConvertError do
        raise ED3Error.Create('Could not open ' + paccount + ',' + pfilename + ',');
   end;

   self.converter := converter;
   self.initialized := true;
end;

{------------------------------------------------------------------------------
  Get the file name
-------------------------------------------------------------------------------}
function TD3FileTcp.getFileName: WideString;
begin
     Result := self.filename;
end;

{------------------------------------------------------------------------------
  Get the account name
-------------------------------------------------------------------------------}
function TD3FileTcp.getAccountName: WideString;
begin
    Result := self.accountname;
end;

{------------------------------------------------------------------------------
  Read an item from the file
-------------------------------------------------------------------------------}
function TD3FileTcp.read(const key: WideString; var item: D3Item): Integer;
var
   res : String;
   buf : TD3Buffer;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( intToStr(D3_READ) + D3_SEPARATOR + IntToSTr(fd) + D3_SEPARATOR + key + D3_SEPARATOR);
    buf := dataFormat(res);

    item.setRecord(buf.getData[0]);

    if converter <> nil then
       convertAfterRead(item);

    Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
  Read an attribute from the file
-------------------------------------------------------------------------------}
function TD3FileTcp.readv(const key: WideString; am: Integer): WideString;
var
   res : String;
   buf : TD3Buffer;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( intToStr(D3_READV) + D3_SEPARATOR + IntToSTr(fd) + D3_SEPARATOR + key + D3_SEPARATOR + IntToStr(am) + D3_SEPARATOR);
    buf := dataFormat(res);

    if buf.getStatus = D3_OK then
       Result := buf.getData[0]
    else
       Result := '';

end;

{------------------------------------------------------------------------------
  Read and lock an item from the file
-------------------------------------------------------------------------------}
function TD3FileTcp.readu(const key: WideString; var item: D3Item): Integer;
var
   res : String;
   buf : TD3Buffer;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( intToStr(D3_READU) + D3_SEPARATOR + IntToSTr(fd) + D3_SEPARATOR + key + D3_SEPARATOR);
    buf := dataFormat(res);

    item.setRecord(buf.getData[0]);

    if converter <> nil then
       convertAfterRead(item);

    Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
  Lock an item from the file, without reading it (lighter form network;)
-------------------------------------------------------------------------------}
function TD3FileTcp.lock(const key: WideString): Integer;
var
   res : String;
   buf : TD3Buffer;
begin
    if not self.initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( IntToStr(D3_LOCK_ITEM) + D3_SEPARATOR + IntToStr(fd) + D3_SEPARATOR + key + D3_SEPARATOR );
    buf := dataFormat(res);

    Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
  Write an item into the file
-------------------------------------------------------------------------------}
function TD3FileTcp.write(const key: WideString; const item: D3Item): Integer;
var
   res : String;
   buf : TD3Buffer;
   tmp : D3Item;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    tmp := TD3Item.Create;
    tmp.setRecord(item.toString);

    if converter <> nil then
       convertBeforeWrite(tmp);

    res := con.send( IntToStr(D3_WRITE) + D3_SEPARATOR + IntToStr(fd) + D3_SEPARATOR + key + D3_SEPARATOR + tmp.toString + D3_SEPARATOR );
    buf := dataFormat(res);

    Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
   Delete an item from the file
-------------------------------------------------------------------------------}
function TD3FileTcp.delete(const key: WideString): Integer;
var
   res : String;
   buf : TD3Buffer;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( IntToStr(D3_DELETE) + D3_SEPARATOR + IntToStr(fd) + D3_SEPARATOR + key + D3_SEPARATOR );

    buf := dataFormat(res);

    Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
  Test if an record exist or not into the file, without reading it
-------------------------------------------------------------------------------}
function TD3FileTcp.testExist(const key: WideString): WordBool;
var
   res : String;
   buf : TD3Buffer;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( IntToStr(D3_TEST_EXIST) + D3_SEPARATOR + IntToStr(fd) + D3_SEPARATOR + key + D3_SEPARATOR );
    buf := dataFormat(res);

    if buf.getStatus = D3_OK then
    begin
       if buf.getData[0] = '0' then
          Result := true
       else
          Result := false;
    end else
       Result := false;
end;

{------------------------------------------------------------------------------
  Release a lock on an item
-------------------------------------------------------------------------------}
function TD3FileTcp.Release(const key: WideString): Integer;
var
   res : String;
   buf : TD3Buffer;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( IntToStr(D3_RELEASE) + D3_SEPARATOR + IntToStr(fd) + D3_SEPARATOR + key + D3_SEPARATOR );

    buf := dataFormat(res);

    Result := buf.getStatus;
end;

{------------------------------------------------------------------------------
  Select all record from the file, for AQL see the D3Session
-------------------------------------------------------------------------------}
function TD3FileTcp.select: D3SelectList;
var
   res : String;
   buf : TD3Buffer;
   nbrecord : Integer;
   list : TD3SelectListFile;
begin
    if not initialized then
      raise ED3Error.Create('File not initialized');

    res := con.send( IntToStr(D3_SELECT) + D3_SEPARATOR + IntToStr(fd) + D3_SEPARATOR );
    buf := dataFormat(res);

    nbrecord := StrToInt(buf.getData[1]);

    list := TD3SelectListFile.Create;
    list.init(self.con,buf.getData[0],nbrecord);

    Result := list;
end;

{------------------------------------------------------------------------------
  Internal procedure to convert all the item after reading it from D3
-------------------------------------------------------------------------------}
procedure TD3FileTcp.convertAfterRead(item : D3Item);
var
   tmp : String;
   am,vm,svm : Integer;
begin
   if converter = nil then
      exit;

   for am := 1 to item.AMCount do
   begin
      for vm := 1 to item.VMCount(am) do
      begin
          for svm := 1 to item.SVMCount(am,vm) do
          begin
              tmp := item.extract(am,vm,svm);
              item.replace(am,vm,svm, converter.AfterReadConversion(tmp));
          end;
      end;
   end;

end;

{------------------------------------------------------------------------------
  Internal procedure to convert all the item before writing it to D3
-------------------------------------------------------------------------------}
procedure TD3FileTcp.convertBeforeWrite(item : D3Item);
var
   tmp : String;
   am,vm,svm : Integer;
begin
   if converter = nil then
      exit;

   for am := 1 to item.AMCount do
   begin
      for vm := 1 to item.VMCount(am) do
      begin
          for svm := 1 to item.SVMCount(am,vm) do
          begin
              tmp := item.extract(am,vm,svm);
              item.replace(am,vm,svm, converter.BeforeWriteConversion(tmp));
          end;
      end;
   end;

end;

{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
  TComObjectFactory.Create(ComServer,TD3FileTcp,CLSID_D3File,'D3File','File descriptor',ciMultiInstance);

end.
