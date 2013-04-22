#pragma once

#include "Database.h"
#include <iostream>
#include <list>

using namespace MY_NAME_SPACE;

class Engine
{
public:	
	Engine(db::Database &db);
	~Engine(void);
	int query(const std::string sql, std::list<std::string> &ret);
	int add(const std::string sql);
	int update(const std::string sql);
	int remove(const std::string sql);

private:
	db::Database &_db;	
};
