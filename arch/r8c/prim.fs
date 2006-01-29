\ r8c/m16c primitives

\ Copyright (C) 2006 Free Software Foundation, Inc.

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

\ * Register using for r8c
\  Renesas  Forth    used for
\   R0      TOS   oberstes Stackelement
\   R3      RP    Returnstack Pointer
\   SP      SP    Stack Pointer
\   A1      IP    Instruction Pointer
\   A0      W     Arbeitsregister
\
\ * Memory ( use only one 64K-Page ): TBD
\ **************************************************************


start-macros
 \ register definition
  ' R3 Alias rp
  ' R0 Alias tos
  ' R0L Alias tos.b
  ' A1 Alias ip
  ' A0 Alias w
  ' [A1] Alias [ip]
  ' [A0] Alias [w]

 \ system depending macros
  : next,
      [ip] , w mov.w:g
      # 2 , ip add.w:q
      [w] jmpi.w ;
\ note that this is really for 8086 and 286, and _not_ intented to run
\ fast on a Pentium (Pro). These old chips load their code from real RAM
\ and do it slow, anyway.
\ If you really want to have a fast 16 bit Forth on modern processors,
\ redefine it as
\ : next,  [ip] w mov,  2 # ip add,  [w] jmp, ;

end-macros

  unlock
    $0000 $FFFF region address-space
    $C000 $4000 region rom-dictionary
    $0400 $0400  region ram-dictionary
  .regions
  setup-target
  lock

\ ==============================================================
\ rom starts with jump to GFORTH-kernel (must be at $0000 !!!)
\ ==============================================================
  Label into-forth
    # $ffff , ip mov.w:g            \ ip will be patched
    # $fef0 , sp ldc                \ sp at $FD80...$FEF0
    # $fd80 , rp mov.w:g            \ rp at $F.00...$FD80
    next,
  End-Label


\ ==============================================================
\ GFORTH minimal primitive set
\ ==============================================================
 \ inner interpreter
  Code: :docol
  \     ': dout,                    \ only for debugging
     # -2 , rp add.w:q
     w , r2 mov.w:g
     rp , w mov.w:g  ip , [w] mov.w:g
     # 4 , r2 add.w:q  r2 , ip mov.w:g
     next,
   End-Code

  Code: :dovar
\    '2 dout,                    \ only for debugging
    tos push.w:g
    # 4 , w add.w:q
    w , tos mov.w:g
    next,
  End-Code

  Code: :dodoes  ( -- pfa ) \ get pfa and execute DOES> part
\    '6 dout,                    \ only for debugging
     next,                                       \ execute does> part
   End-Code


 \ program flow
  Code ;s       ( -- ) \ exit colon definition
\    '; dout,                    \ only for debugging
      rp , w mov.w:g  # 2 , rp add.w:q
      [w] , ip mov.w:g
      next,
  End-Code

  Code execute   ( xt -- ) \ execute colon definition
\    'E dout,                    \ only for debugging
    tos , w mov.w:g                             \ copy tos to w
    tos pop.w:g                                 \ get new tos
    [w] jmpi.w                                  \ execute
  End-Code

  Code ?branch   ( f -- ) \ jump on f<>0
      [ip] , w mov.w:g
      tos , tos tst.w   0<> IF  w , ip mov.w:g   THEN
      next,
  End-Code


 \ memory access
  Code @        ( addr -- n ) \ read cell
      tos , w mov.w:g  [w] , tos mov.w:g
      next,
   End-Code

  Code !        ( n addr -- ) \ write cell
      tos , w mov.w:g  tos pop.w:g  tos , [w] mov.w:g
      tos pop.w:g
      next,
   End-Code

  Code c@        ( addr -- n ) \ read cell
      tos , w mov.w:g  tos , tos xor.w  [w] , tos.b mov.b:g
      next,
   End-Code

  Code c!        ( n addr -- ) \ write cell
      tos , w mov.w:g  tos pop.w:g  tos.b , [w] mov.b:g
      tos pop.w:g
      next,
   End-Code

 \ arithmetic and logic
  Code +        ( n1 n2 -- n3 ) \ addition
      r1 pop.w:g
      r1 , tos add.w:g
      next,
  End-Code
  
  Code -        ( n1 n2 -- n3 ) \ addition
      r1 pop.w:g
      tos , r1 sub.w:g
      r1 , tos mov.w:g
      next,
  End-Code

  Code and        ( n1 n2 -- n3 ) \ addition
      r1 pop.w:g
      r1 , tos and.w:g
      next,
  End-Code
  
  Code or       ( n1 n2 -- n3 ) \ addition
      r1 pop.w:g
      r1 , tos or.w:g
      next,
  End-Code
  
  Code xor      ( n1 n2 -- n3 ) \ addition
      r1 pop.w:g
      r1 , tos xor.w
      next,
   End-Code

 \ moving datas between stacks
  Code r>       ( -- n ; R: n -- )
      tos push.w:g
      rp , w mov.w:g
      [w] , tos mov.w:g
      # 2 , rp add.w:g
      next,
   End-Code

   Code >r       ( n -- ; R: -- n )
       # -2 , rp add.w:g
       rp , w mov.w:g
       tos , [w] mov.w:g
       tos pop.w:g
       next,
   End-Code

 \ datastack and returnstack address
  Code sp@      ( -- sp ) \ get stack address
      tos push.w:g
      sp , tos stc
      next,
  End-Code

  Code sp!      ( sp -- ) \ set stack address
      tos , sp ldc
      tos pop.w:g
      next,
  End-Code

  Code rp@      ( -- rp ) \ get returnstack address
    tos push.w:g
    rp , tos mov.w:g
    next,
  End-Code

  Code rp!      ( rp -- ) \ set returnstack address
      tos , rp mov.w:g
      tos pop.w:g
      next,
  End-Code

  Code: :docon
      tos push.w:g
      4 [w] , tos mov.w:g
      next,
  End-Code

  Code: :dodefer
      4 [w] , w mov.w:g
      [w] jmpi.w
  End-Code

  Code branch   ( -- ) \ unconditional branch
      [ip] , ip mov.w:g
      next,
   End-Code

  Code lit     ( -- n ) \ inline literal
      tos push.w:g
      [ip] , tos mov.w:g
      # 2 , ip add.w:q
      next,
   End-Code

Code: :doesjump
end-code

\ ==============================================================
\ usefull lowlevel words
\ ==============================================================
 \ word definitions


 \ branch and literal

 \ data stack words
  Code dup      ( n -- n n )
    tos push.w:g
    next,
   End-Code

  Code 2dup     ( d -- d d )
    r1 pop.w:g
    r1 push.w:g
    tos push.w:g
    r1 push.w:g
    next,
   End-Code

  Code drop     ( n -- )
    tos pop.w:g
    next,
   End-Code

  Code 2drop    ( d -- )
    tos pop.w:g
    tos pop.w:g
    next,
   End-Code

0 [IF]

  Code swap     ( n1 n2 -- n2 n1 )
    ax pop,
    tos push,
    ax tos mov,
    next,
   End-Code

  Code over     ( n1 n2 -- n1 n2 n1 )
    tos ax mov,
    tos pop,
    tos push,
    ax push,
    next,
   End-Code

  Code rot      ( n1 n2 n3 -- n2 n3 n1 )
    tos ax mov,
    cx pop,
    tos pop,
    cx push,
    ax push,
    next,
   End-Code

  Code -rot     ( n1 n2 n3 -- n3 n1 n2 )
    tos ax mov,
    tos pop,
    cx pop,
    ax push,
    cx push,
    next,
   End-Code


 \ return stack
  Code r@       ( -- n ; R: n -- n )
    tos push,
    frp ) tos mov,
    next,
  End-Code


 \ arithmetic
  Code -        ( n1 n2 -- n3 ) \ Subtraktion
    ax pop,
    tos ax sub,
    ax tos mov,
    next,
   End-Code

  Code um*      ( u1 u2 -- ud ) \ unsigned multiply
    tos ax mov,
    cx pop,
    cx mul,
    ax push,
    dx tos mov,
    next,
   End-Code

  Code um/mod   ( ud u -- r q ) \ unsiged divide
    tos cx mov,
    dx pop,
    ax pop,
    cx div,
    dx push,
    ax tos mov,
    next,
   End-Code


 \ logic
  Code or       ( n1 n2 -- n3 ) \ logic OR
    ax pop,   ax tos or,   next,
   End-Code


 \ shift
  Code 2/       ( n1 -- n2 ) \ arithmetic shift right
     tos sar,
     next,
   End-Code

  Code lshift   ( n1 n2 -- n3 ) \ shift n1 left n2 bits
     tos cx mov,
     tos pop,
     cx cx or,  0<> IF, tos c* shl, THEN,
     next,
   End-Code

  Code rshift   ( n1 n2 -- n3 ) \ shift n1 right n2 bits
     tos cx mov,
     tos pop,
     cx cx or,  0<> IF, tos c* shr, THEN,
     next,
   End-Code


 \ compare
  Code 0=       ( n -- f ) \ Test auf 0
    tos tos or,
    0 # tos mov,
    0= IF, tos dec, THEN,
    next,
   End-Code

  Code =        ( n1 n2 -- f ) \ Test auf Gleichheit
    ax pop,
    ax tos sub,
    0= IF,  -1 # tos mov,   next,
    ELSE,   0  # tos mov,   next,
    THEN,
   End-Code


 \ i/o
  Variable lastkey      \ Flag und Zeichencode der letzen Taste

  Code (key)    ( -- char ) \ get character
    tos push,
    lastkey #) ax mov,
    ah ah or,  0= IF, 7 # ah mov,  $21 int, THEN,
    0 # lastkey #) mov,
    ah ah xor,
    ax tos mov,
    next,
   End-Code

  Code (emit)     ( char -- ) \ output character
    tosl dl mov,
    6 # ah mov,
    $ff # dl cmp,  0= IF, dl dec, THEN,
    $21 int,
    tos pop,
    next,
  End-Code

 \ additon io routines
  Code (key?)     ( -- f ) \ check for read sio character
    tos push, lastkey # tos mov,
    1 tos d) ah mov,   ah ah or,
    0= IF,  $ff # dl mov,  6 # ah mov,  $21 int,
            0 # ah mov,
            0<> IF, dl ah mov,   ax tos ) mov, THEN,
    THEN,  ah tosl mov,   ah tosh mov,
    next,
   End-Code

  Code emit?    ( -- f ) \ check for write character to sio
    tos push,
    -1 # tos mov,             \ output always possible
    next,
   End-Code

[then]
: (bye)     ( 0 -- ) \ back to DOS
    drop ;

: bye ( -- )  0 (bye) ;
    
: compile-prim1 ;
: finish-code ;
: emit-file ;
: x@+/string ( addr u -- addr' u' c )
    over c@ >r 1 /string r> ;
: xkey ( -- key )  key ;
