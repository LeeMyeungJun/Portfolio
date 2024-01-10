#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "CUserWidget_Select.generated.h"

UCLASS()
class UC02_ACTION_API UCUserWidget_Select : public UUserWidget
{
	GENERATED_BODY()

public:
	FORCEINLINE class UCUserWidget_SelectItem* GetItem(FString InName) { return Items[InName]; }
	
public:
	void Clicked(FString InName);
	void Hovered(FString InName);
	void UnHovered(FString InName);

protected:
	virtual void NativeConstruct() override;

protected:
	UPROPERTY(BlueprintReadOnly, meta = (BindWidget))
		class UGridPanel* Grid;

	UPROPERTY(BlueprintReadOnly)
		TMap<FString, class UCUserWidget_SelectItem*> Items;

};
