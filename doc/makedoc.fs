\ create a documentation file

\ Copyright (C) 1995,1999,2000,2003,2004,2007,2010 Free Software Foundation, Inc.

\ This file is part of Gforth.

\ Gforth is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation, either version 3
\ of the License, or (at your option) any later version.

\ This program is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\ GNU General Public License for more details.

\ You should have received a copy of the GNU General Public License
\ along with this program. If not, see http://www.gnu.org/licenses/.


\ the stack effect of loading this file is: ( addr u -- )
\ it takes the name of the doc-file to be generated.

\ the forth source must have the following format:
\  .... name ( stack-effect ) \ [prefix-] wordset [pronounciation]
\ \G description ...

\ The output is a file of entries that look like this:
\ make-doc [--prefix]-entry name stack-effect ) wordset [pronounciation]
\ description
\
\ (i.e., the entry is terminated by an empty line or the end-of-file)

\ this stuff uses the same mechanism as etags.fs, i.e., the
\ documentation is generated during compilation using a deferred
\ HEADER. It should be possible to use this togeter with etags.fs.

\ This is not very general. Input should come from stream files,
\ otherwise the results are unpredictable. It also does not detect
\ errors in the input (e.g., if there is something else on the
\ definition line) and reacts strangely to them.

\ possible improvements: we could analyse the defining word and guess
\ the stack effect. This would be handy for variables. Unfortunately,
\ we have to look back in the input buffer; we cannot use the cfa
\ because it does not exist when header is called.

\ This is ANS Forth with the following serious environmental
\ dependences: the variable LAST must contain a pointer to the last
\ header, NAME>STRING must convert that pointer to a string, and
\ HEADER must be a deferred word that is called to create the name.


r/w create-file throw value doc-file-id
\ contains the file-id of the documentation file

s" \ automatically generated by makedoc.fs" doc-file-id write-line throw

: >fileCR ( c-addr u -- )
    doc-file-id write-line throw ;
: >file    ( c-addr u -- )
    doc-file-id write-file throw ;

: \G ( -- )
    source >in @ /string >fileCR
    source >in ! drop ; immediate
: ?( ( -- )
    >in @ >r '(' parse + source + u<
    IF  -1 >in +! rdrop  ELSE  r> >in !  THEN ;

normal-dp value check-dp

: put-doc-entry ( -- )
    dpp @ check-dp =   \ not defining locals
    latest 0<> and	\ not an anonymous (i.e. noname) header
    if
	s" " >fileCR
	s" make-doc " >file
        >in @ >r
	?( parse-name 2dup s" (" str= if
            2drop ') parse
        else
            2dup s" {" str= if
                2drop '} parse
            else \ no stack comment or locals
                2drop
                r@ >in ! \ restore "\"
                s" unknown " \ default stack comment
            endif
        endif
	[char] \ parse 2drop
        >in @
        parse-name dup
	IF
	    2dup 1- chars + c@ [char] - =
	    IF
		s" --" >file
		>file drop
	    ELSE
		2drop >in !
	    THEN
	ELSE
	    2drop >in !
	THEN
        latest name>string >file
        s"  " >file
	>file
	s"  )" >file
	POSTPONE \g
	r> >in !
    endif ;

: (doc-header) ( -- )
    defers header
    put-doc-entry ;

' (doc-header) IS header
