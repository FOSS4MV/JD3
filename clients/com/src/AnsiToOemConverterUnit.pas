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
unit AnsiToOemConverterUnit;

interface

uses
  Classes,
  SysUtils,
  Windows,
  ComObj,
  ComServ,

  JD3COM_TLB , JD3COMCommon, JD3COMConst;

type
  TAnsiToOemConverter = class(TComObject,AnsiToOemConverter,D3EscapeConverter)
    function AfterReadConversion(const str: WideString): WideString; safecall;
    function BeforeWriteConversion(const str: WideString): WideString; safecall;
  end;

implementation

function TAnsiToOemConverter.AfterReadConversion(const str: WideString) : WideString;
var
  avant,apres : String;
begin
  avant := str;
  SetLength(apres,length(avant));
  if length(apres) > 0 then
    OemToAnsi(PChar(avant), PChar(apres));

  Result := apres;
end;

function TAnsiToOemConverter.BeforeWriteConversion(const str: WideString) : WideString;
var
  avant,apres : String;
begin
  avant := str;
  SetLength(apres,length(avant));
  if length(apres) > 0 then
    AnsiToOem(PChar(avant), PChar(apres));

  Result := apres;
end;


{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
 TComObjectFactory.Create(ComServer,TAnsiToOemCOnverter,CLSID_AnsiToOemConverter,'AnsiToOemConverter','Conversion for OEM/ANSI',ciMultiInstance);

end.
