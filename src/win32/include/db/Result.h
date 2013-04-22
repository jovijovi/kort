#ifndef _MYSQL_DATABASE_RESULT_H
#define _MYSQL_DATABASE_RESULT_H

#include <mysql.h>
#include <string>
#include "global_macro.h"
#include "DatabaseException.h"
#include "Row.h"

namespace MY_NAME_SPACE {
namespace db {

class Result
{
public:
	Result();
	virtual ~Result();
	int fetch_row(Row &row);
	void set_result(MYSQL_RES *res);
	void free_result();
	my_ulonglong num_rows();
private:
	MYSQL_RES *_res;
	my_ulonglong _nr_rows;
};

inline Result::Result()
{
	_res = NULL;
	_nr_rows = 0;
}

inline Result::~Result()
{
	free_result();
}

inline void Result::free_result()
{
	if(NULL != _res) {
		mysql_free_result(_res);
		_res = NULL;
		_nr_rows = 0;
	}
}

inline void Result::set_result(MYSQL_RES *res)
{
	_res = res;
	_nr_rows = mysql_num_rows(_res);
}

inline int Result::fetch_row(Row &row)
{
	MYSQL_ROW r = mysql_fetch_row(_res);
	if(NULL == r)
		return 0;
	row.set_row(r, mysql_num_fields(_res));
	return 1;
}

inline my_ulonglong Result::num_rows()
{
	return _nr_rows;
}

} } /* namespace MY_NAME_SPACE::db */

#endif /* _MYSQL_DATABASE_RESULT_H */ 
