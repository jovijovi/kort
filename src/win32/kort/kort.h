// kort.h : kort DLL ����ͷ�ļ�
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�ڰ������ļ�֮ǰ������stdafx.h�������� PCH �ļ�"
#endif

#include "resource.h"		// ������
#include "Manager.h"

// CkortApp
// �йش���ʵ�ֵ���Ϣ������� kort.cpp
//

class KORT : public CWinApp
{
public:
	KORT();	

// ��д
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
	int DLL_TEST_INTERFACE(void);
	int DLL_LOGIN(void);
	int DLL_LOGOUT(void);
};
