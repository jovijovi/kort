// TestDLL_KORT.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include "TestDLL_KORT.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// Ψһ��Ӧ�ó������

CWinApp theApp;

using namespace std;

int TestDLL();

int _tmain(int argc, TCHAR* argv[], TCHAR* envp[])
{
	int nRetCode = 0;

	// ��ʼ�� MFC ����ʧ��ʱ��ʾ����
	if (!AfxWinInit(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), 0))
	{
		// TODO: ���Ĵ�������Է���������Ҫ
		_tprintf(_T("����: MFC ��ʼ��ʧ��\n"));
		nRetCode = 1;
	}
	else
	{
		// TODO: �ڴ˴�ΪӦ�ó������Ϊ��д���롣
		TestDLL();
	}

	return nRetCode;
}

int TestDLL()
{
	HINSTANCE	hmod_pub = NULL;
	TCHAR	szFull[_MAX_PATH];
	TCHAR	szDrive[_MAX_DRIVE];
	TCHAR	szDir[_MAX_DIR];
	GetModuleFileName(NULL, szFull, sizeof(szFull)/sizeof(TCHAR));
	_tsplitpath(szFull, szDrive, szDir, NULL, NULL);
	_tcscpy(szFull, szDrive);
	_tcscat(szFull, szDir);
	_tcscat(szFull,_T("\\kort.dll"));

	hmod_pub = ::LoadLibrary(szFull);	

	if (!hmod_pub)
	{
		return -1;
	}

	int ret = GetLastError();
	typedef int (*pDLL)();
	pDLL DLL_TEST_INTERFACE = (pDLL)::GetProcAddress(hmod_pub,"DLL_TEST_INTERFACE");
	pDLL DLL_LOGIN = (pDLL)::GetProcAddress(hmod_pub,"DLL_LOGIN");
	pDLL DLL_LOGOUT = (pDLL)::GetProcAddress(hmod_pub,"DLL_LOGOUT");

	if (DLL_TEST_INTERFACE)
	{
		DLL_TEST_INTERFACE();		
	}

	if (DLL_LOGIN)
	{
		DLL_LOGIN();		
	}
	
	if (DLL_LOGOUT)
	{
		DLL_LOGOUT();		
	}

	if (hmod_pub)
	{		
		FreeLibrary(hmod_pub);
		hmod_pub = NULL;		
	}

	system("PAUSE");

	return 0;
}