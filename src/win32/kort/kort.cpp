// kort.cpp : ���� DLL �ĳ�ʼ�����̡�
//

#include "stdafx.h"
#include "kort.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: ����� DLL ����� MFC DLL �Ƕ�̬���ӵģ�
//		��Ӵ� DLL �������κε���
//		MFC �ĺ������뽫 AFX_MANAGE_STATE ����ӵ�
//		�ú�������ǰ�档
//
//		����:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// �˴�Ϊ��ͨ������
//		}
//
//		�˺������κ� MFC ����
//		������ÿ��������ʮ����Ҫ������ζ��
//		��������Ϊ�����еĵ�һ�����
//		���֣������������ж������������
//		������Ϊ���ǵĹ��캯���������� MFC
//		DLL ���á�
//
//		�й�������ϸ��Ϣ��
//		����� MFC ����˵�� 33 �� 58��
//

// CkortApp

BEGIN_MESSAGE_MAP(KORT, CWinApp)
END_MESSAGE_MAP()


// CkortApp ����

KORT::KORT()
{	
}

// Ψһ��һ�� CkortApp ����

KORT _kort;

// CkortApp ��ʼ��

BOOL KORT::InitInstance()
{
	CWinApp::InitInstance();

	return TRUE;
}

int KORT::DLL_TEST_INTERFACE(void)
{
	Manager manager;	
	manager.test("192.168.10.39","717root","123qweasdzxc","kort");

	return 0;
}

int KORT::DLL_LOGIN(void)
{
	Manager manager;	
	manager.login("192.168.10.39","717root","123qweasdzxc","kort");

	return 0;
}

int KORT::DLL_LOGOUT(void)
{
	Manager manager;	
	manager.logout("192.168.10.39","717root","123qweasdzxc","kort");

	return 0;
}
