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
unit JD3COMCommon;

interface

uses
  Classes,
  SysUtils,
  Dialogs,

  JD3COM_TLB ,
  JD3COMConst;

{------------------------------------------------------------------------------
  Constants used by this library
------------------------------------------------------------------------------}
const
  D3_SEPARATOR : char = char(1);

  D3_LOGON : Integer = 1;
  D3_LOGOFF : Integer = 2;
  D3_EXECUTE : Integer = 3;
  D3_CALL : Integer = 4;
  D3_SELECT : Integer = 5;
  D3_OPEN : Integer = 6;
  D3_READNEXT : Integer = 7;
  D3_READ : Integer = 8;
  D3_READU : Integer = 9;
  D3_WRITE : Integer = 10;
  D3_RELEASE : Integer = 11;
  D3_DELETE : Integer = 12;
  D3_CLOSE : Integer = 13;
  D3_TEST_EXIST : Integer = 14;
  D3_LOCK_ITEM : Integer = 15;
  D3_READV : Integer = 16;
  D3_SELECT_TCL : Integer = 17;
  D3_NOOP : Integer = 18;

  D3_DIFF_DATE : Integer = 732;
  D3_DIFF_TIME : Integer = 86400000;

  _MAXBLOCK : Integer = 1024;

type

  ED3Error = class(Exception);

  function substr(str : String; start,stop : Integer):String;
  procedure debug(msg : String);

{------------------------------------------------------------------------------
 Implementation of classes
------------------------------------------------------------------------------}
implementation

{------------------------------------------------------------------------------
 Extract a sub string
------------------------------------------------------------------------------}
function substr(str : String; start,stop : Integer):String;
begin
    Result := '';
    if start < 1 then exit;
    if stop >= length(str) then
    begin
       stop := length(str);
       if start = 1 then
       begin
         Result := str;
         exit;
       end;
    end;

    Result := Copy(str,start,(stop-start));
end;

{------------------------------------------------------------------------------
  Show dialog box for debug message
 ------------------------------------------------------------------------------}
procedure debug(msg:String);
begin
  MessageDlg('Debug : ' + msg,mtInformation,[mbok],0);
end;

end.
