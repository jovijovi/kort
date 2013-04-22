#ifndef _MYSQL_DATABASE_COMMON_H
#define _MYSQL_DATABASE_COMMON_H

#include "mysql_hdr.h"

#include <string>
#include "global_macro.h"
#include "Statement.h"
#include "DatabaseException.h"

#ifdef MYSQL_DATABASE_COMMON_DEBUG
#include <iostream>
#endif

namespace MY_NAME_SPACE {
namespace db {

class Database
{
public:
	Database(std::string host, std::string user, std::string password, 
			std::string db, unsigned port = 0, 
			std::string sock = "", unsigned flag = 0);
	virtual ~Database();
	void new_query(Statement &stmt, const std::string &query);
	void set_charset(const std::string &charset);
	void get_charset(std::string &charset);
	void escape_string(const std::string &from, std::string &to);
	void to_string(const std::string &from, std::string &to);
private:
	MYSQL _mysql;
};

inline Database::Database(std::string host, std::string user, 
		std::string password, std::string db, unsigned port, 
		std::string sock, unsigned flag)
{
	const char *us;
	mysql_init(&_mysql);
	if(!host.length())
		host = "localhost";
	if(!sock.length())
		us = NULL;
	else
		us = sock.c_str();
	if(!mysql_real_connect(&_mysql,host.c_str(), user.c_str(),
			password.c_str(), db.c_str(), port, us, flag))
		throw DatabaseException(std::string("Err when real connect ")
				+ mysql_error(&_mysql));
#ifdef TRACE_DEBUG
	std::cout << "[db] database connect: " << user << "@" << host << ":" << port
		<< "\n\tdatabase: " << db << std::endl;
#endif
	set_charset("gb2312");
}

inline Database::~Database()
{
	mysql_close(&_mysql);
}

} } /* namespace MY_NAME_SPACE::db */

#endif /* _MYSQL_DATABASE_COMMON_H */