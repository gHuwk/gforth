/* This is the machine-specific part for a SPARC

  Copyright (C) 1995-2003 Free Software Foundation, Inc.

  This file is part of Gforth.

  Gforth is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111, USA.
*/

#ifndef THREADING_SCHEME
#define THREADING_SCHEME 5
#endif

#if !defined(USE_TOS) && !defined(USE_NO_TOS)
#define USE_TOS
#endif

#include "../generic/machine.h"

#define FLUSH_ICACHE(addr,size) \
  ({void *_addr=(addr); void *_end=_addr+((Cell)(size)); \
    for (_addr=((long)_addr)&~7; _addr<_end; _addr += 8) \
       asm("iflush %0+0"::"r"(_addr)); \
   })
/* the +0 in the iflush instruction is needed by gas */
