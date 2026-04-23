#include "pch.h"
#include "Core.h"
#include "ExitButton.h"

void ExitButton::OnClick()
{
	DestroyWindow( GET_SINGLE(Core)->GetHwnd());
}
