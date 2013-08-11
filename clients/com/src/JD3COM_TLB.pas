unit JD3COM_TLB;

{ This file contains pascal declarations imported from a type library.
  This file will be written during each import or refresh of the type
  library editor.  Changes to this file will be discarded during the
  refresh process. }

{ JD3COM Library }
{ Version 0.1 }

interface

uses Windows, ActiveX, Classes, Graphics, OleCtrls, StdVCL;

const
  LIBID_JD3COM: TGUID = '{D5D731A1-9E06-4D2C-BF4C-CC5918BEBAEB}';

type

{ Forward declarations: Interfaces }
  D3Connection = interface;
  D3Session = interface;
  D3Params = interface;
  D3SelectList = interface;
  D3SelectListTcl = interface;
  D3File = interface;
  D3EscapeConverter = interface;
  D3Item = interface;
  D3SelectListFile = interface;
  AnsiToOemConverter = interface;

{ Connection classes to D3 }

  D3Connection = interface(IUnknown)
    ['{6EB64DFB-FCF8-45A5-AE67-6A17DE0A388E}']
    procedure Open(const pserver: WideString; pport: Integer; const puser, ppwd: WideString; ptype: Integer); safecall;
    function getTypeConnection: Integer; safecall;
    function getPort: Integer; safecall;
    function getUserName: WideString; safecall;
    function getServer: WideString; safecall;
    function createSession: D3Session; safecall;
    function send(const cmd: WideString): WideString; safecall;
    function login(const puser, ppwd: WideString): Integer; safecall;
    procedure logoff(const puser: WideString); safecall;
  end;

  D3Session = interface(IUnknown)
    ['{87667B8D-21F7-4878-9B5E-AEA978DC5779}']
    procedure init(const pcon: D3Connection); safecall;
    function noop: Integer; safecall;
    function call(const name: WideString; const params: D3Params): Integer; safecall;
    function execute(const sentence: WideString): WideString; safecall;
    function select(const sentence: WideString): D3SelectList; safecall;
    function openFile(const paccount, pfilename: WideString; const pconverter: D3EscapeConverter): D3File; safecall;
  end;

  D3Params = interface(IUnknown)
    ['{3DA4CADB-99D0-4AE1-A52E-0827A42A77FE}']
    function getSize: Integer; safecall;
    function getParam(index: Integer): OleVariant; safecall;
    procedure setParam(const param: WideString; index: Integer); safecall;
    procedure addParam(const param: WideString); safecall;
  end;

  D3SelectList = interface(IUnknown)
    ['{9B3A03E8-F1EA-498B-9310-E4753ADC3157}']
    function hasMoreElements: WordBool; safecall;
    function getNextElement: WideString; safecall;
    function getNbElements: Integer; safecall;
  end;

  D3SelectListTcl = interface(D3SelectList)
    ['{142F7CD8-7AB9-4BF6-A56D-1CD211792194}']
    procedure init(const keylist: WideString); safecall;
  end;

  D3File = interface(IUnknown)
    ['{58069922-06EF-46F7-ADDE-57122E99BA78}']
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
  end;

  D3EscapeConverter = interface(IUnknown)
    ['{6CFC6E57-7200-4A3C-A9AA-7CB8A37BC66F}']
    function AfterReadConversion(const str: WideString): WideString; safecall;
    function BeforeWriteConversion(const str: WideString): WideString; safecall;
  end;

  D3Item = interface(IUnknown)
    ['{E0A51710-1850-4122-AE2B-46C9B8CAD6DD}']
    procedure setRecord(const newrecord: WideString); safecall;
    function extract(am, vm, svm: Integer): WideString; safecall;
    function extractAM(am: Integer): WideString; safecall;
    function extractVM(am, vm: Integer): WideString; safecall;
    function extractSVM(am, vm, svm: Integer): WideString; safecall;
    procedure replace(am, vm, svm: Integer; const newval: WideString); safecall;
    procedure replaceAM(am: Integer; const newval: WideString); safecall;
    procedure replaceVM(am, vm: Integer; const newval: WideString); safecall;
    procedure replaceSVM(am, vm, svm: Integer; const newval: WideString); safecall;
    procedure insert(am, vm, svm: Integer; const Value: WideString); safecall;
    procedure insertAM(am: Integer; const Value: WideString); safecall;
    procedure insertVM(am, vm: Integer; const Value: WideString); safecall;
    procedure insertSVM(am, vm, svm: Integer; const Value: WideString); safecall;
    procedure delete(am, vm, svm: Integer); safecall;
    procedure deleteAM(am: Integer); safecall;
    procedure deleteVM(am, vm: Integer); safecall;
    procedure deleteSVM(am, vm, svm: Integer); safecall;
    function AMCount: Integer; safecall;
    function VMCount(am: Integer): Integer; safecall;
    function SVMCount(am, vm: Integer): Integer; safecall;
    function toString: WideString; safecall;
  end;

  D3SelectListFile = interface(D3SelectList)
    ['{7F03D083-217A-4865-A1E3-A3C8F7E80BA6}']
    procedure init(const con: D3Connection; const pselect: WideString; pnbelements: Integer); safecall;
  end;

  AnsiToOemConverter = interface(D3EscapeConverter)
    ['{E0E3C71C-4EA5-4021-8ABA-2D124B37666A}']
  end;



implementation


end.
