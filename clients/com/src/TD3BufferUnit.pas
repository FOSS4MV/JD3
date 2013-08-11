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
unit TD3BufferUnit;

interface
uses
  Classes,
  SysUtils,

  JD3COMCommon;

type
  {------------------------------------------------------------------------------
    Buffer used internally for communication results
  ------------------------------------------------------------------------------}
  TD3Buffer = Class
  public
    constructor Create(pstatus:Integer;pdata :TStringList);
    destructor Destroy;override;

    function getStatus:Integer;
    procedure setStatus(newstatus : Integer);

    function getData : TStringList;

  private
    data : TStringList;
    status : Integer;

  end;

  function dataFormat(data : String) : TD3Buffer;

implementation

{------------------------------------------------------------------------------
 Format data into array of string with a status
------------------------------------------------------------------------------}
function dataFormat(data : String) : TD3Buffer;
var
  buf : TD3Buffer;
  list : TStringList;
  status,where : Integer;
  tmp : String;
begin
  list := TStringList.Create;

  where := Pos(D3_SEPARATOR,data);
  tmp := copy(data,1,where-1);
  delete(data,1,where);
  Status := StrToInt(tmp);

  where := Pos(D3_SEPARATOR,data);
  while where <> 0 do
  begin
    tmp := copy(data,1,where-1);
    list.Add(tmp);
    delete(data,1,where);
    where := Pos(D3_SEPARATOR,data);
  end;

  if data <> '' then
     list.add(data);

  buf := TD3Buffer.Create(status,list);
  Result := buf;
end;

{------------------------------------------------------------------------------
 D3Buffer
 ------------------------------------------------------------------------------}
constructor TD3Buffer.Create(pstatus : Integer ; pdata : TStringList);
begin
  inherited Create;
  self.status := pstatus;
  self.data := pdata;
end;

destructor TD3Buffer.Destroy;
begin
  data.clear;
  data.Free;
  self.data.Destroy;
  inherited Destroy;
end;

procedure TD3Buffer.setStatus(newstatus : Integer);
begin
  self.status := newstatus;
end;

function TD3Buffer.getStatus : Integer;
begin
  result := self.status;
end;

function TD3Buffer.getData : TStringList;
begin
  Result := self.data;
end;

end.
