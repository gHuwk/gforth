\ search order wordset                                 14may93py

$10 constant maxvp
Variable vp
  0 A, 0 A,  0 A, 0 A,   0 A, 0 A,   0 A, 0 A, 
  0 A, 0 A,  0 A, 0 A,   0 A, 0 A,   0 A, 0 A, 

: get-current  ( -- wid )  current @ ;
: set-current  ( wid -- )  current ! ;

: context ( -- addr )  vp dup @ cells + ;
: definitions  ( -- )  context @ current ! ;

\ wordlist Vocabulary also previous                    14may93py

AVariable voclink

Defer 'initvoc
' drop IS 'initvoc

Variable slowvoc   slowvoc off

: wordlist  ( -- wid )
  here  0 A, Forth-wordlist wordlist-map @ A, voclink @ A, slowvoc @ A,
  dup wordlist-link dup voclink ! 'initvoc ;

: Vocabulary ( -- ) Create wordlist drop  DOES> context ! ;

: also  ( -- )
  context @ vp @ 1+ dup maxvp > abort" Vocstack full"
  vp ! context ! ;

: previous ( -- )  vp @ 1- dup 0= abort" Vocstack empty" vp ! ;

\ vocabulary find                                      14may93py

: (vocfind)  ( addr count nfa1 -- nfa2|false )
    \ !! generalize this to be independent of vp
    drop vp dup @ 1- cells over +
    DO  2dup I 2@ over <>
        IF  (search-wordlist) dup
	    IF  nip nip  UNLOOP EXIT
	    THEN  drop
        ELSE  drop 2drop  THEN
    [ -1 cells ] Literal +LOOP
    2drop false ;

0 value locals-wordlist

: (localsvocfind)  ( addr count nfa1 -- nfa2|false )
    \ !! use generalized (vocfind)
    drop locals-wordlist
    IF 2dup locals-wordlist (search-wordlist) dup
	IF nip nip
	    EXIT
	THEN drop
    THEN
    0 (vocfind) ;

\ In the kernal the dictionary search works on only one wordlist.
\ The following stuff builds a thing that looks to the kernal like one
\ wordlist, but when searched it searches the whole search order
\  (including locals)

\ this is the wordlist-map of the dictionary
Create vocsearch       ' (localsvocfind) A, ' (reveal) A,  ' drop A,

\ Only root                                            14may93py

wordlist \ the wordlist structure
vocsearch over wordlist-map A! \ patch the map into it

Vocabulary Forth
Vocabulary Root

: Only  vp off  also Root also definitions ;

\ set initial search order                             14may93py

Forth-wordlist @ ' Forth >body A!

vp off  also Root also definitions
Only Forth also definitions

lookup A! \ our dictionary search order becomes the law

' Forth >body constant forth-wordlist \ "forth definitions get-current" and "forth-wordlist" should produce the same wid


\ get-order set-order                                  14may93py

: get-order  ( -- wid1 .. widn n )
  vp @ 0 ?DO  vp cell+ I cells + @  LOOP  vp @ ;

: set-order  ( wid1 .. widn n / -1 -- )
  dup -1 = IF  drop Only exit  THEN  dup vp !
  ?dup IF  1- FOR  vp cell+ I cells + !  NEXT  THEN ;

: seal ( -- )  context @ 1 set-order ;

\ words visible in roots                               14may93py

: .name ( name -- ) name>string type space ;
: words  cr 0 context @
  BEGIN  @ dup  WHILE  2dup cell+ c@ $1F and 2 + dup >r +
         &79 >  IF  cr nip 0 swap  THEN
         dup .name space r> rot + swap  REPEAT 2drop ;

: body> ( data -- cfa )  0 >body - ;

: .voc  body> >name .name ;
: order ( -- )  \  search-ext
    \g prints the search order and the @code{current} wordlist.  The
    \g standard requires that the wordlists are printed in the order
    \g in which they are searched. Therefore, the output is reversed
    \g with respect to the conventional way of displaying stacks. The
    \g @code{current} wordlist is displayed last.
    get-order 0
    ?DO
	.voc
    LOOP
    4 spaces get-current .voc ;
: vocs ( -- ) \ gforth
    \g prints vocabularies and wordlists defined in the system.
    voclink
    BEGIN
	@ dup @
    WHILE
	dup 0 wordlist-link - .voc
    REPEAT
    drop ;

Root definitions

' words Alias words
' Forth Alias Forth
' forth-wordlist alias forth-wordlist
' set-order alias set-order
' order alias order

Forth definitions

include hash.fs

\ marker                                               18dec94py

\ Marker creates a mark that is removed (including everything 
\ defined afterwards) when executing the mark.

: marker, ( -- mark )  here dup A,
  voclink @ A, voclink
  BEGIN  @ dup @  WHILE  dup 0 wordlist-link - @ A,  REPEAT  drop
  udp @ , ;

: marker! ( mark -- )  dup @ swap cell+
  dup @ voclink ! cell+
  voclink
  BEGIN  @ dup @  WHILE  over @ over 0 wordlist-link - !
	 swap cell+ swap
  REPEAT  drop  voclink
  BEGIN  @ dup @  WHILE  dup 0 wordlist-link - rehash  REPEAT  drop
  @ udp !  dp ! ;

: marker ( "mark" -- )
  marker, Create A,  DOES>  @ marker! ;
