\ machpc.fs is generated; source: machpc.fs.in
\ generic mach file for pc gforth				03sep97jaw

\ Copyright (C) 1995-2003 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program; if not, write to the Free Software
\ Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111, USA.

true DefaultValue NIL  \ relocating

>ENVIRON

true DefaultValue file		\ controls the presence of the
				\ file access wordset
true DefaultValue OS		\ flag to indicate a operating system

true DefaultValue prims		\ true: primitives are c-code

true DefaultValue floating	\ floating point wordset is present

true DefaultValue glocals	\ gforth locals are present
				\ will be loaded
true DefaultValue dcomps	\ double number comparisons

true DefaultValue hash		\ hashing primitives are loaded/present

true DefaultValue xconds	\ used together with glocals,
				\ special conditionals supporting gforths'
				\ local variables
true DefaultValue header	\ save a header information

true DefaultValue backtrace	\ enables backtrace code

true DefaultValue new-input	\ enables object oriented input

true DefaultValue peephole      \ enables peephole optimizer

true DefaultValue abranch       \ enables absolute branches

false DefaultValue control-rack \ disable return stack use for control flow

false DefaultValue ec
false DefaultValue crlf

$100 DefaultValue kernel-start
cell 2 = [IF] &32 KB [ELSE] $100000 cells [THEN] DefaultValue kernel-size

&16 KB		DefaultValue stack-size
&16 KB		DefaultValue fstack-size
&15 KB		DefaultValue rstack-size
&14 KB &512 +	DefaultValue lstack-size
