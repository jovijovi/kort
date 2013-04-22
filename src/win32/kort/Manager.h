#pragma once

#include "Engine.h"

class Manager
{
public:
	Manager(void);
	~Manager(void);
	int test(std::string host, std::string user, std::string password, 
		std::string db, unsigned port = 0, 
		std::string sock = "", unsigned flag = 0);
	int login(std::string host, std::string user, std::string password, 
		std::string db, unsigned port = 0, 
		std::string sock = "", unsigned flag = 0);
	int logout(std::string host, std::string user, std::string password, 
		std::string db, unsigned port = 0, 
		std::string sock = "", unsigned flag = 0);
};
