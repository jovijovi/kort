#include "StdAfx.h"
#include "Database.h"

namespace MY_NAME_SPACE {
namespace db {


void Database::new_query(Statement &stmt, const std::string &query)
{
	stmt.set_database(&_mysql);
	stmt.set_query(query);
}

void Database::set_charset(const std::string &charset)
{
	if(mysql_set_character_set(&_mysql, charset.c_str()))
		throw DatabaseException(std::string("Failed to set charset: ")
				+ mysql_error(&_mysql));
}

void Database::get_charset(std::string &charset)
{
	charset = mysql_character_set_name(&_mysql);
}

void Database::escape_string(const std::string &from, std::string &to)
{
	char *str = new char[from.length()*2 + 1];
	mysql_real_escape_string(&_mysql, str, from.c_str(), from.length());
	to = str;
	delete []str;
}

void Database::to_string(const std::string &from, std::string &to)
{
	escape_string(from, to);
	unsigned pos = to.find_first_of('\\');
	if(pos == std::string::npos)
		return;
	to = to.substr(0, pos);
}

} } /* namespace MY_NAME_SPACE::db */