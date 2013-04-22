#ifndef _MYSQL_DATABASE_STATEMENT_H
#define _MYSQL_DATABASE_STATEMENT_H

#include "mysql_hdr.h"
#include <string>
#include "global_macro.h"
#include "DatabaseException.h"
#include "Result.h"

namespace MY_NAME_SPACE {
namespace db {

class Statement
{
public:
	Statement();
	virtual ~Statement();
	void exec();
	void exec(Result &r);
	void set_database(MYSQL *mysql);
	void set_query(const std::string &query);
private:
	MYSQL *_mysql;
	std::string _query;
};

inline Statement::Statement() :
	_mysql(NULL)
{
}

inline Statement::~Statement()
{
}

} } /* namespace MY_NAME_SPACE::db */

#endif /* _MYSQL_DATABASE_STATEMENT_H */