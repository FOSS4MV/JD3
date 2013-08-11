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
library JD3COM;

uses
  ComServ,
  JD3COM_TLB in 'JD3COM_TLB.pas',
  JD3COMCommon in 'JD3COMCommon.pas',
  JD3COMInterfaces in 'JD3COMInterfaces.pas',
  D3ConnectionUnit in 'D3ConnectionUnit.pas',
  JD3COMConst in 'JD3COMConst.pas',
  D3SessionUnit in 'D3SessionUnit.pas',
  TD3BufferUnit in 'TD3BufferUnit.pas',
  D3ParamsUnit in 'D3ParamsUnit.pas',
  D3SelectListTclUnit in 'D3SelectListTclUnit.pas',
  D3ItemUnit in 'D3ItemUnit.pas',
  D3FileTcpUnit in 'D3FileTcpUnit.pas',
  D3SelectListFileUnit in 'D3SelectListFileUnit.pas',
  AnsiToOemConverterUnit in 'AnsiToOemConverterUnit.pas';

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer;

{$R *.TLB}

{$R *.RES}

begin
end.
