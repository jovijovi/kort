#include "StdAfx.h"
#include "manager.h"

Manager::Manager(void)
{
}

Manager::~Manager(void)
{
}

int Manager::test(std::string host, std::string user, 
				  std::string password, std::string db, unsigned port, 
				  std::string sock, unsigned flag)
{
	try
	{
		std::list<std::string> ret;
		db::Database db_zero(host,user,password,db);	
		Engine engine(db_zero);
		engine.query("select * from kort_log", ret);
		printf("DONE\r\n");	//FOR TEST
	}
	catch (Exception &e)
	{
		std::cerr << e.msg() << ":" << e.code() << std::endl;	
	}

	return 0;
}

int Manager::login(std::string host, std::string user, 
				   std::string password, std::string db, unsigned port, 
				   std::string sock, unsigned flag)
{
	//TODO:
	// 1. search user
	// 2. verify license
	// 3. database operation
	// 4. return

	printf("LOGIN\r\n");	//FOR TEST

	return 0;
}

int Manager::logout(std::string host, std::string user, 
					std::string password, std::string db, unsigned port, 
					std::string sock, unsigned flag)
{
	//TODO:
	// 1. search user
	// 2. verify license
	// 3. database operation
	// 4. return

	printf("LOGOUT\r\n");	//FOR TEST

	return 0;
}
