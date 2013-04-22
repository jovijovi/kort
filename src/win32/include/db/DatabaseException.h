#ifndef _MYSQL_DATABASE_COMMON_EXCEPTION_H
#define _MYSQL_DATABASE_COMMON_EXCEPTION_H

#include "Exception.h"

namespace MY_NAME_SPACE {
namespace db {

class DatabaseException : public Exception
{
public:
	DatabaseException(std::string msg, unsigned code = -1);
};

inline DatabaseException::DatabaseException(std::string msg, unsigned code) :
	Exception(msg,code)
{
}

} } /* namespace MY_NAME_SPACE::db */

#endif /* _MYSQL_DATABASE_COMMON_EXCEPTION_H */