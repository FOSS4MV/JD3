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
unit D3SelectListTclUnit;

interface

uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,

  JD3COM_TLB , JD3COMCommon, JD3COMConst;

type
  TD3SelectListTcl = class(TComObject,D3SelectList,D3SelectListTcl)
  public
    function hasMoreElements: WordBool; safecall;
    function getNextElement: widestring; safecall;
    function getNbElements: Integer; safecall;
    procedure init(const keylist: WideString); safecall;
    procedure Initialize; override;
    destructor Destroy; override;

  private
    list : TStringList;
    curpos,maxpos : Integer;

  end;

implementation

procedure TD3SelectListTcl.Initialize;
begin
  inherited;
  list := TStringList.Create;
  curpos := 0;
  maxpos := 0;
end;

destructor TD3SelectListTcl.Destroy;
begin
   list.Clear;
   list.Free;
   inherited;
end;

procedure TD3SelectListTcl.init(const keylist: WideString);
var
   where : Integer;
   data : String;
begin
  data := keylist;

  where := Pos(D3_AM,data);
  while where <> 0 do
  begin
    list.Add(copy(data,1,where-1));
    delete(data,1,where);
    where := Pos(D3_AM,data);
  end;

  if data <> '' then
     list.add(data);

  maxpos := list.Count;

end;

function TD3SelectListTcl.hasMoreElements: WordBool;
begin
  if curpos < maxpos then
    Result := true
  else
    Result := false;

end;

function TD3SelectListTcl.getNextElement: WideString;
begin
  Result := '';
  if hasMoreElements then
  begin
     Result := list[curpos];
     curpos := curpos + 1;
  end;
end;

function TD3SelectListTcl.getNbElements: Integer;
begin
  Result := maxpos;
end;


{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
  TComObjectFactory.Create(ComServer,TD3SelectListTcl,CLSID_D3SelectListTcl,'D3SelectListTcl','Select list from a AQL sentence',ciMultiInstance);

end.
