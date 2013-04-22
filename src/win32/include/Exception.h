#ifndef _MYSQL_DATABASE_EXCEPTION_H
#define _MYSQL_DATABASE_EXCEPTION_H

#include "global_macro.h"
#include <string>

namespace MY_NAME_SPACE {

class Exception
{
public:
	Exception(const std::string &msg, unsigned code = -1);
	virtual ~Exception();
	std::string msg();
	unsigned code();
private:
	std::string _msg;
	unsigned _code;
};

inline Exception::Exception(const std::string &msg, unsigned code) :
	_msg(msg), _code(code)
{
}

inline Exception::~Exception()
{
}

inline std::string Exception::msg()
{
	return _msg;
}

inline unsigned Exception::code()
{
	return _code;
}

} /* namespace MY_NAME_SPACE */

#endif /* _MYSQL_DATABASE_EXCEPTION_H */

