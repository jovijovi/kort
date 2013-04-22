#include "StdAfx.h"
#include "engine.h"

Engine::Engine(db::Database &db):_db(db)
{
}

Engine::~Engine(void)
{
}

int Engine::query(const std::string sql, std::list<std::string> &ret)
{
	db::Statement stmt;
	db::Result result;
	db::Row row;
	_db.new_query(stmt, sql);
	stmt.exec(result);
	int nr = 0;
	char *str = NULL;
	while(result.fetch_row(row))
	{
		if(NULL == row.get(0))
			continue;

		//FOR TEST		
		for(unsigned int i = 0; i < row.num_fields(); i++)
		{
			std::cout << row.get(i) << "\t";
			ret.push_back(row.get(i));
		}
		std::cout << std::endl;
	}

	return 0;
}

int Engine::add(const std::string sql)
{
	db::Statement stmt;
	_db.new_query(stmt, sql);
	stmt.exec();

	return 0;
}

int Engine::update(const std::string sql)
{
	db::Statement stmt;
	_db.new_query(stmt, sql);
	stmt.exec();

	return 0;
}

int Engine::remove(const std::string sql)
{
	db::Statement stmt;
	_db.new_query(stmt, sql);
	stmt.exec();

	return 0;
}
