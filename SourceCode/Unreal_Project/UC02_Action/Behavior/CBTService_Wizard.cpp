#include "CBTService_Wizard.h"
#include "Global.h"
#include "Characters/CAIController.h"
#include "Characters/CEnemy_AI.h"
#include "Characters/CPlayer.h"
#include "Components/CBehaviorComponent.h"
#include "Components/CStateComponent.h"

UCBTService_Wizard::UCBTService_Wizard()
{
	NodeName = "Wizard";
}

void UCBTService_Wizard::TickNode(UBehaviorTreeComponent& OwnerComp, uint8* NodeMemory, float DeltaSeconds)
{
	Super::TickNode(OwnerComp, NodeMemory, DeltaSeconds);

	ACAIController* controller = Cast<ACAIController>(OwnerComp.GetOwner());
	UCBehaviorComponent* behavior = CHelpers::GetComponent<UCBehaviorComponent>(controller);

	ACEnemy_AI* ai = Cast<ACEnemy_AI>(controller->GetPawn());
	UCStateComponent* state = CHelpers::GetComponent<UCStateComponent>(ai);

	//Hitted
	if (state->IsHittedMode())
	{
		behavior->SetHittedMode();
		return;
	}

	//No Target
	ACPlayer* target = behavior->GetTargetPlayer();
	if (target == nullptr)
	{
		behavior->SetWaitMode();
		controller->ClearFocus(EAIFocusPriority::Gameplay);

		return;
	}
	else
	{
		UCStateComponent* targetState = CHelpers::GetComponent<UCStateComponent>(target);

		if (targetState->IsDeadMode())
		{
			behavior->SetWaitMode();
			return;
		}
	}

	//Sensed Target
	controller->SetFocus(target);
	
	float distance = ai->GetDistanceTo(target);
	CheckTrue(state->IsDeadMode());

	//In BehaviorRange -> Warp
	if (distance < controller->GetBehaviorRange())
	{
		behavior->SetAvoidMode();
		return;
	}

	//In Sight -> IceBall
	if (distance < controller->GetSightRadius())
	{
		behavior->SetActionMode();
		return;
	}
}