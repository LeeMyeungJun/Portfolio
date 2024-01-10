#pragma once

#include "CoreMinimal.h"
#include "GameFramework/HUD.h"
#include "CHUD.generated.h"

UCLASS()
class UC02_ACTION_API ACHUD : public AHUD
{
	GENERATED_BODY()

public:
	FORCEINLINE void SetDraw() { bDraw = true; }
	FORCEINLINE void SetNotDraw() { bDraw = false; }

private:
	UPROPERTY(EditDefaultsOnly)
		class UTexture2D* Texture;

public:
	ACHUD();

	virtual void DrawHUD() override;
	

private:
	bool bDraw;
};
