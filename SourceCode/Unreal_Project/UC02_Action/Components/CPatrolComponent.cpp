#include "CPatrolComponent.h"
#include "Global.h"
#include "Behavior/CPatrolPath.h"
#include "Components/SplineComponent.h"

UCPatrolComponent::UCPatrolComponent()
{

}

void UCPatrolComponent::BeginPlay()
{
	Super::BeginPlay();


}

bool UCPatrolComponent::GetMoveTo(FVector& OutLocation, float& OutAcceptanceRadius)
{
	OutLocation = FVector::ZeroVector;
	OutAcceptanceRadius = AcceptanceRadius;
	CheckNullResult(Path, false);

	OutLocation = Path->GetSpline()->GetLocationAtSplinePoint(Index, ESplineCoordinateSpace::World);
	return true;
}

void UCPatrolComponent::UpdateNextIndex()
{
	CheckNull(Path);

	int32 count =  Path->GetSpline()->GetNumberOfSplinePoints();

	//Is Reverse
	if (bReverse)
	{
		if (Index > 0)
		{
			Index--;
			return;
		}

		//Is Loop
		if (Path->GetSpline()->IsClosedLoop())
		{
			Index = count - 1;
			return;
		}

		//Is Not Loop
		Index = 1;
		bReverse = false;
		return;
	}

	//Is Forward
	if (Index < count - 1)
	{
		Index++;
		return;
	}

	//Is Loop
	if (Path->GetSpline()->IsClosedLoop())
	{
		Index = 0;
		return;
	}

	//Is Not Loop
	Index = count - 2;
	bReverse = true;

}





