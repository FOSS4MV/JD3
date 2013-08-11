unit JD3COMImpl;

interface

uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,
  scktcomp,
  Dialogs,

  JD3COM_TLB , JD3COMConst;

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
function substr(str : String; start,stop : Integer):String;
procedure debug(msg : String);

{------------------------------------------------------------------------------
 Implementation of classes
------------------------------------------------------------------------------}
implementation

{------------------------------------------------------------------------------
 Format data into array of string with a status
------------------------------------------------------------------------------}
function dataFormat(data : String) : TD3Buffer;
var
  buf : TD3Buffer;
  list : TStringList;
  status : Integer;
  start,where : Integer;
  tmp : String;
begin
  list := TStringList.Create;
  status := -1;

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
  end;

  if data <> '' then
     list.add(data);

  buf := TD3Buffer.Create(status,list);
  Result := buf;
end;

{------------------------------------------------------------------------------
 Extract a sub string
------------------------------------------------------------------------------}
function substr(str : String; start,stop : Integer):String;
var
  cpt : Integer;
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
