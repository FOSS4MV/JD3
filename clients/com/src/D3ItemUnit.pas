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
unit D3ItemUnit;

interface
uses
  Classes,
  SysUtils,
  ComObj,
  ComServ,

//  TVectorUnit,
  JD3COM_TLB , JD3COMCommon, JD3COMConst;

type
    TD3Item = class(TComObject,D3Item)
    public
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

     procedure Initialize; override;
     destructor Destroy; override;
    private
      attribs : TStringList;

      function trtVM(attribut : String) : TStringList;
      function trtSVM(value : String) : TStringList;

      procedure Clear;
    end;

implementation

{-------------------------------------------------------------------------------
Initialization of the item
-------------------------------------------------------------------------------}
procedure TD3Item.Initialize;
begin
  inherited;
  attribs := TStringList.Create;

end;

{-------------------------------------------------------------------------------
Clear all internal list
-------------------------------------------------------------------------------}
destructor TD3Item.Destroy;
begin
  // Not sure it is usefull here
  self.Clear;
  attribs.Free;
  inherited;
end;


{-------------------------------------------------------------------------------
Remove all elements from the item
-------------------------------------------------------------------------------}
procedure TD3Item.Clear;
var
   i,nbel : Integer;
begin
  nbel := AMCount;
  for i := 1 to nbel do
     delete(1,0,0);
  attribs.Clear;
end;

{-------------------------------------------------------------------------------
Set the value of the record
-------------------------------------------------------------------------------}
procedure TD3Item.setRecord(const newrecord: WideString);
var
   buf : String;
   posam : Integer;
begin
  buf := newrecord;
  self.clear;

  posam := pos(D3_AM,buf);
  while posam > 0 do
  begin
       attribs.addObject('',trtVM(copy(buf,1,posam-1)));
       System.delete(buf,1,posam);
       posam := Pos(D3_AM,buf);
  end;

  attribs.addObject('',trtVM(buf));
end;


function TD3Item.trtVM(attribut : String) : TStringList;
var
   attr : TStringList;
   posvm : Integer;
   buf : String;
   last : Boolean;
begin
  attr := TStringList.Create();
  buf := attribut;
  last := false;

  posvm := pos(D3_VM,buf);
  while posvm > 0 do
  begin
       attr.addObject('',trtSVM(copy(buf,1,posvm-1)));
       System.delete(buf,1,posvm);
       posvm := Pos(D3_VM,buf);
       last := true;
  end;

  if length(attribut) > 0 then
    if (attribut[length(attribut)] = D3_VM) then
       last := false;

  if (buf <> '') or (last = false) then
     attr.addObject('',trtSVM(buf));

  Result := attr;
end;


function TD3Item.trtSVM(value : String) : TStringList;
var
   val : TStringList;
   possvm : Integer;
   buf : String;
   last : Boolean;
begin
   val := TStringList.Create();
   buf := value;
   last := false;

  possvm := pos(D3_SVM,buf);
  while possvm > 0 do
  begin
       val.add(copy(buf,1,possvm-1));
       System.delete(buf,1,possvm);
       possvm := Pos(D3_SVM,buf);
       last := true;
  end;

  if length(value) > 0 then
    if (value[length(value)] = D3_SVM) then
       last := false;

  if (buf <> '') or (last = false) then
     val.add(buf);

  Result := val;
end;

{-------------------------------------------------------------------------------
Extract an attribut/value/sub-value from the item
-------------------------------------------------------------------------------}
function  TD3Item.extract(am, vm, svm: Integer): WideString;
var
  attr,value : TStringList;
  sval,i,nbel : Integer;
begin
  sval := svm;
  Result := '';

  if am < 0 then
    exit;

  if vm <= 0 then
     sval := 0;

  if am > attribs.Count then
     exit;

  // Extract all attribute
  if am = 0 then
  begin
     nbel := self.AMCount;
     for i := 1 to nbel do
     begin
       Result := Result + extractAM(i);
       if i < nbel then
          Result := Result + D3_AM;
     end;
     exit;
  end;


  attr := attribs.Objects[am-1] as TStringList;

  // Extract all VM
  if vm = 0 then
  begin
     nbel := VMCount(am);
     for i := 1 to nbel do
     begin
        Result := Result + extractVM(am,i);
        if i < nbel then
           Result := Result + D3_VM;
     end;
     exit;
  end;


  if vm > attr.Count then
     exit;

  value := attr.objects[vm-1] as TStringList;

  if sval > value.count then
     exit;
     
  // Extract all SVM
  if sval = 0 then
  begin
     nbel := SVMCount(am,vm);
     for i := 1 to nbel do
     begin
         Result := Result + extract(am,vm,i);
         if i < nbel then
              Result := Result + D3_SVM;
      end;
      exit;
  end;

  Result := value[sval-1];
  
end;

function  TD3Item.extractAM(am: Integer): WideString;
begin
 Result := extract(am,0,0);
end;

function  TD3Item.extractVM( am, vm: Integer): WideString;
begin
 Result := extract(am,vm,0);
end;

function  TD3Item.extractSVM(am, vm, svm: Integer): WideString;
begin
  Result := extract(am,vm,svm);
end;

{-------------------------------------------------------------------------------
Convert the item into a string
-------------------------------------------------------------------------------}
function TD3Item.toString : WideString;
begin
     Result := self.extract(0,0,0);
end;

{-------------------------------------------------------------------------------
 Replace an attribut/value/sub-value by new value
-------------------------------------------------------------------------------}
procedure TD3Item.replace( am, vm, svm: Integer; const newval: WideString);
var
   sval,i : Integer;
   attr,val : TStringList;
begin
   sval := svm;

   if am <= 0 then
       exit;

   if vm = 0 then
      sval := 0;

   // insert enought AM
   for i := attribs.count to am -1 do
   begin
      val := TStringList.Create;
      val.Add('');
      attr := TSTringList.Create;
      attr.AddObject('',val);
      attribs.AddObject('',attr);
   end;

   attr := attribs.Objects[am-1] as TSTringList;

   // replace entire AM
   if vm <= 0 then
   begin
      // delete old value into AM
      for i := 0 to attr.count - 1 do begin
         val := attr.Objects[i] as TSTringList;
         val.clear;
         val.free;
      end;
      attr.Clear;

      // add new value
      val := TSTringList.Create;
      val.add(newVal);

      attr.AddObject('',val);
      exit;
   end else begin
      // add enought VM
      for i := attr.count to vm -1 do
      begin
          val := TStringList.Create;
          val.Add('');
          attr.AddObject('',val);
      end;

      val := attr.Objects[vm-1] as TStringList;

      // replace entire VM
      if sval = 0 then
      begin
         val.Clear;
         val.add(newval);
         exit;
      end else begin
         // add enought SVM
         for i := val.count to sval - 1 do
         begin
             val.add('');
         end;

         // replace entire SVM ;)
         val.Strings[sval - 1] := newval;
      end;
   end;

   // Because we could replace by another D3Item with AM,VM,SVM
   self.setRecord(self.toString);
end;

procedure TD3Item.replaceAM(am: Integer; const newval: WideString);
begin
  replace(am,0,0,newval);
end;

procedure TD3Item.replaceVM(am, vm: Integer; const newval: WideString);
begin
  replace(am,vm,0,newval);
end;

procedure TD3Item.replaceSVM(am, vm, svm: Integer; const newval: WideString);
begin
   replace(am,vm,svm,newval);
end;

{-------------------------------------------------------------------------------
Insert an attribute/value/sub-value into the item
-------------------------------------------------------------------------------}
procedure TD3Item.insert(am, vm, svm: Integer; const Value: WideString);
var
    attr,val : TStringList;
    sval : Integer;
begin
   sval := svm;

   if am = 0 then
      exit;

   if vm = 0 then
      sval := 0;

   if am > attribs.count then
   begin
      // Use replace, it already work ;)
      replace(am,vm,sval,Value);
      exit;
   end;

   // insert entire AM
   if vm <= 0 then
   begin
      val := TSTringList.Create;
      val.add(Value);
      attr := TStringList.Create;
      attr.addObject('',val);
      attribs.insertObject(am - 1,'',attr);
      exit;
   end else begin
      attr := attribs.Objects[am-1] as TSTringList;
      if vm > attr.count then
      begin
         // Use replace, it already work ;)
         replace(am,vm,sval,Value);
         exit;
      end;

      // insert entire VM
      if sval <= 0 then
      begin
         val := TStringList.Create;
         val.Add(Value);
         attr.insertObject(vm-1,'',val);
      end else begin
         val := attr.Objects[vm-1] as TStringList;
         if sval > val.count then
         begin
            // Use replace, it already work ;)
            replace(am,vm,sval,Value);
            exit;
         end;

         // insert entire SVM ;)
         val.Insert(sval - 1,Value);
      end;
   end;

   // Because we could replace by another D3Item with AM,VM,SVM
   self.setRecord(self.toString);
end;

procedure TD3Item.insertAM(am: Integer; const Value: WideString);
begin
     insert(am,0,0,value);
end;

procedure TD3Item.insertVM(am, vm: Integer; const Value: WideString);
begin
     insert(am,vm,0,value);
end;

procedure TD3Item.insertSVM(am, vm, svm: Integer; const Value: WideString);
begin
     insert(am,vm,svm,value);
end;

{-------------------------------------------------------------------------------
Delete an attribut/value/sub-value from the item
-------------------------------------------------------------------------------}
procedure TD3Item.delete(am, vm, svm: Integer);
var
    attr,val : TStringList;
    sval,nbel,i : Integer;
begin
   sval := svm;

   if am <= 0 then
      exit;

   if vm <= 0 then
      sval := 0;

   if am > attribs.Count then
      exit;

   attr := attribs.Objects[am-1] as TStringList;

   if vm > attr.Count then
      exit;

   // Delete entire AM
   if vm = 0 then
   begin
      nbel := VMCount(am);
      for i := 1 to nbel do
      begin
         delete(am,1,0);
      end;
      attr.Clear;
      attr.Free;
      attribs.Delete(am-1);
      exit;
   end;

   val := attr.Objects[vm-1] as TStringList;

   if sval > val.Count then
      exit;

   if sval = 0 then
   begin
        nbel := SVMCount(am,vm);
        for i := 1 to nbel do
        begin
             delete(am,vm,1);
        end;
        val.Clear;
        val.Free;
        attr.Delete(vm-1);
        exit;
   end;

   val.Delete(sval-1);

end;

procedure TD3Item.deleteAM(am: Integer);
begin
   delete(am,0,0);
end;

procedure TD3Item.deleteVM(am, vm: Integer);
begin
   delete(am,vm,0);
end;

procedure TD3Item.deleteSVM(am, vm, svm: Integer);
begin
   delete(am,vm,svm);
end;

{-------------------------------------------------------------------------------
Get the number of attribute in the item
-------------------------------------------------------------------------------}
function  TD3Item.AMCount: Integer;
begin
  Result := attribs.count;
end;

{-------------------------------------------------------------------------------
Get the number of value into an attribute
-------------------------------------------------------------------------------}
function  TD3Item.VMCount(am: Integer): Integer;
var
   list : TStringList;
begin
  Result := 0 ;
  if (am <= 0) or (am > attribs.count) then
     exit;

  list := attribs.objects[am-1] as TStringList;
  Result := list.count;

end;

{-------------------------------------------------------------------------------
Get the number of sub-value into a value
-------------------------------------------------------------------------------}
function  TD3Item.SVMCount(am, vm: Integer): Integer;
var
  list : TStringList;
begin
  Result := 0 ;
  if (am <= 0) or (am > attribs.count) or (vm <= 0) then
      exit;

  list := attribs.objects[am-1] as TStringList;
  if vm > list.count then
     exit ;

  list := list.objects[vm-1] as TStringList;
  Result := list.count;
end;

{-------------------------------------------------------------------------------
  Register COM Object
-------------------------------------------------------------------------------}
initialization
  TComObjectFactory.Create(ComServer,TD3Item,CLSID_D3Item,'D3Item','D3 record',ciMultiInstance);

end.
