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
unit JD3COMConst;

interface
{------------------------------------------------------------------------------
 Constants to work with D3
------------------------------------------------------------------------------}
const
  CONNECTION_TCP : Integer = 1;
  CONNECTION_TCP_PROXY : Integer = 2;

  D3_AM : char = char(254);
  D3_VM : char = char(253);
  D3_SVM : char = char(252);

  D3_ERR : Integer = 1;
  D3_OK : Integer = 0;

  READ_FILENOTOPEN : Integer = -1;
  READ_RECORDEMPTY : Integer = -2;
  READ_RECORDLOCKED : Integer = -3;

  READNEXT_EOF : Integer = -1;

  USR_INCONNU : Integer = -2;
  PWD_INCORRECT  : Integer = -1;

  CLSID_D3Connection :TGUID = '{6EB64DFB-FCF8-45A5-AE67-6A17DE0A388E}';
  CLSID_D3Session : TGUID = '{87667B8D-21F7-4878-9B5E-AEA978DC5779}';
  CLSID_D3Params : TGUID = '{3DA4CADB-99D0-4AE1-A52E-0827A42A77FE}';
  CLSID_D3SelectList : TGUID = '{9B3A03E8-F1EA-498B-9310-E4753ADC3157}';
  CLSID_D3SelectListTcl : TGUID = '{142F7CD8-7AB9-4BF6-A56D-1CD211792194}';
  CLSID_D3SelectListFile : TGUID = '{7F03D083-217A-4865-A1E3-A3C8F7E80BA6}';
  CLSID_D3File : TGUID = '{58069922-06EF-46F7-ADDE-57122E99BA78}';
  CLSID_D3EscapeConverter : TGUID = '{6CFC6E57-7200-4A3C-A9AA-7CB8A37BC66F}';
  CLSID_D3Item : TGUID = '{E0A51710-1850-4122-AE2B-46C9B8CAD6DD}';
  CLSID_AnsiToOemConverter : TGUID = '{E0E3C71C-4EA5-4021-8ABA-2D124B37666A}';

implementation

end.
 