#ifndef __MYSQL_DATABASE_GLOBAL_MACRO_H
#define __MYSQL_DATABASE_GLOBAL_MACRO_H

#define MY_NAME_SPACE kort_database_interface

//#define TRACE_DEBUG		1
//#define DB_STATEMENT_TRACE		1
//#define MD5HELPER_TRACE			1
#define MYSQL_DATABASE_DEBUG			1

#define MAX(a,b)	((a) > (b) ? (a) : (b))
#define MIN(a,b)	((a) < (b) ? (a) : (b))

#ifdef TRACE_DEBUG

#ifndef MYSQL_DATABASE_DEBUG

#define MYSQL_DATABASE_DEBUG			1
#include <iostream>

#endif

#endif

#endif /* __MYSQL_DATABASE_GLOBAL_MACRO_H */