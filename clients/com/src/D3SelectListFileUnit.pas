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
unit D3SelectListFileUnit;

interface
uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,

  JD3COM_TLB , JD3COMCommon, JD3COMConst, TD3BufferUnit;

type
  TD3SelectListFile = class(TComObject,D3SelectList,D3SelectListFile)
  public
    function hasMoreElements: WordBool; safecall;
    function getNextElement: widestring; safecall;
    function getNbElements: Integer; safecall;
    procedure init(const con: D3Connection; const pselect: WideString; pnbelements: Integer); safecall;
    procedure Initialize; override;
    destructor Destroy; override;

  private
     con : D3Connection;
     select,curkey : String;
     finished : Boolean;
     initialized : Boolean;
     nbelements : Integer;

     function readNext : Boolean;
  end;

implementation

procedure TD3SelectListFile.Initialize;
begin
  inherited;
  con := nil;
  finished := true;
  initialized := false;
  select := '';
  curkey := '';
end;

destructor TD3SelectListFile.Destroy;
begin
   inherited;
end;

procedure TD3SelectListFile.init(const con: D3Connection; const pselect: WideString; pnbelements: Integer);
begin
    self.con := con;
    self.select := pselect;
    self.nbelements := pnbelements;
    self.curkey := '';

    initialized := true;
    finished := false;
end;

function TD3SelectListFile.hasMoreElements: WordBool;
begin
  if not initialized then
      raise ED3Error.Create('Select list not initialized');

  if curkey = '' then
  begin
    Result := false;
    if readNext then
       Result := true;
  end else begin
    Result := true;
  end
end;

function TD3SelectListFile.getNextElement: WideString;
begin
  if not initialized then
      raise ED3Error.Create('Select list not initialized');

  Result := '';

  if hasMoreElements then
  begin
     Result := curkey;
     curkey := '';
  end;

end;

function TD3SelectListFile.getNbElements: Integer;
begin
  if not initialized then
      raise ED3Error.Create('Select list not initialized');

  Result := nbelements;
end;

function TD3SelectListFile.readNext : Boolean;
var
   res : String;
   buf : TD3Buffer;
begin
  if not initialized then
      raise ED3Error.Create('Select list not initialized');

  if curkey <> '' then
  begin
     Result := true;
     exit;
  end;

  if finished then
  begin
     Result := false;
     exit;
  end;

  res := con.send( IntToStr(D3_READNEXT) + D3_SEPARATOR + self.select + D3_SEPARATOR  );
  buf := dataFormat(res);

  if buf.getStatus = READNEXT_EOF then
  begin
     Result := false;
     finished := true;
     exit;
  end;

  if buf.getStatus <> D3_OK then
  begin
     Result := False;
     exit;
  end;

  if buf.getdata.Count > 0 then
     curkey := buf.getData[0];

  Result := True;

end;

{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
  TComObjectFactory.Create(ComServer,TD3SelectListFile,CLSID_D3SelectListFile,'D3SelectListFile','Select list on an opened file',ciMultiInstance);

end.
