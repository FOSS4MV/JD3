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
unit D3ParamsUnit;

interface
uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,

  JD3COM_TLB,JD3COMConst ;

type
  TD3Params = class(TComObject,D3Params)
  public
    function getSize: Integer; safecall;
    function getParam(index: Integer): OleVariant; safecall;
    procedure setParam(const param: WideString; index: Integer); safecall;
    procedure addParam(const param: WideString); safecall;
    procedure Initialize; override;
    destructor Destroy; override;

  private
    paramlist : TStringList;

  end;

implementation

procedure TD3Params.Initialize;
begin
   inherited;
   paramlist := TStringList.Create;
end;

destructor TD3Params.Destroy;
begin
   paramlist.Clear;
   Paramlist.Free;
   inherited;
end;

function TD3Params.getSize : Integer;
begin
   Result := paramlist.Count;
end;

function TD3Params.getParam(index: Integer): OleVariant;
begin
  Result := paramlist[index-1];
end;

procedure TD3Params.setParam(const param: WideString; index: Integer);
begin
  paramlist[index-1] := param;
end;

procedure TD3Params.addParam(const param: WideString);
begin
  paramlist.Add(param);
end;


{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
  TComObjectFactory.Create(ComServer,TD3Params,CLSID_D3Params,'D3Params','Parameters for a call',ciMultiInstance);

end.
