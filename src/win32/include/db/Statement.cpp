#include "StdAfx.h"
#include "Statement.h"
#include <cstring>

#include "windows.h"

#ifdef DB_STATEMENT_TRACE
#include <iostream>
#endif

namespace MY_NAME_SPACE {
namespace db {

void Statement::set_database(MYSQL *mysql)
{
	_mysql = mysql;
}

void Statement::set_query(const std::string &query)
{
	_query = query;
}

void Statement::exec()
{
	int t;
#ifdef DB_STATEMENT_TRACE
	std::cout << "[db] prepare to query: \n\t" << _query << std::endl;
#endif
	t = mysql_real_query(_mysql,_query.c_str(),_query.length());
	if(t) {
		throw DatabaseException(std::string("query err: ") + _query + 
					+ mysql_error(_mysql));
	}
#ifdef DB_STATEMENT_TRACE
	std::cout << "[db] has queried: \n\t" << _query << std::endl;
#endif
}

void Statement::exec(Result &r)
{
	exec();
	r.set_result(mysql_store_result(_mysql));
}

} } /* namespace MY_NAME_SPACE::db */
