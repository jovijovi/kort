#ifndef _MYSQL_DATABASE_ROW_H
#define _MYSQL_DATABASE_ROW_H

#include <mysql.h>
#include "global_macro.h"

namespace MY_NAME_SPACE {
namespace db {

class Row
{
public:
	Row();
	virtual ~Row();
	const char *get(unsigned col);
	void set_row(MYSQL_ROW row, unsigned nr);
	unsigned num_fields();
private:
	MYSQL_ROW _row;
	unsigned _nr;
};

inline Row::Row() :
	_row(NULL), _nr(0)
{
}

inline Row::~Row()
{
}

inline void Row::set_row(MYSQL_ROW row, unsigned nr)
{
	_row = row;
	_nr = nr;
}

inline const char *Row::get(unsigned col)
{
	if(col >= _nr || NULL == _row)
		return NULL;
	return _row[col];
}

inline unsigned Row::num_fields()
{
	return _nr;
}

} } /* namespace MY_NAME_SPACE::db */

#endif /* _MYSQL_DATABASE_ROW_H */