#include "CUserWidget_SelectItem.h"
#include "Global.h"
#include "Characters/CPlayer.h"
#include "Widgets/CUserWidget_Select.h"

void UCUserWidget_SelectItem::Click()
{
	GetSelectWidget()->Clicked(GetName());
}

void UCUserWidget_SelectItem::Hover()
{
	GetSelectWidget()->Hovered(GetName());
}

void UCUserWidget_SelectItem::UnHover()
{
	GetSelectWidget()->UnHovered(GetName());
}

UCUserWidget_Select* UCUserWidget_SelectItem::GetSelectWidget()
{
	ACPlayer* player = Cast<ACPlayer>(UGameplayStatics::GetPlayerCharacter(GetWorld(), 0));

	return player->GetSelectWidget();
}
