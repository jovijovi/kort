#ifndef _MYSQL_DATABASE_HEADER_H
#define _MYSQL_DATABASE_HEADER_H

#ifndef WIN32
#include <mysql.h>
#endif

#ifdef WIN32
#include <winsock2.h>
#include <mysql.h>
#endif

#endif /* _MYSQL_DATABASE_HEADER_H */