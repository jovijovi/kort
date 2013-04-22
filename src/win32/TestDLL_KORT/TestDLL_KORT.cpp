// TestDLL_KORT.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "TestDLL_KORT.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// 唯一的应用程序对象

CWinApp theApp;

using namespace std;

int TestDLL();

int _tmain(int argc, TCHAR* argv[], TCHAR* envp[])
{
	int nRetCode = 0;

	// 初始化 MFC 并在失败时显示错误
	if (!AfxWinInit(::GetModuleHandle(NULL), NULL, ::GetCommandLine(), 0))
	{
		// TODO: 更改错误代码以符合您的需要
		_tprintf(_T("错误: MFC 初始化失败\n"));
		nRetCode = 1;
	}
	else
	{
		// TODO: 在此处为应用程序的行为编写代码。
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